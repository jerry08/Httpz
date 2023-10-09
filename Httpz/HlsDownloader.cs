using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Httpz.Exceptions;
using Httpz.Hls;
using Httpz.Utils;
using Httpz.Utils.Extensions;

namespace Httpz;

public class HlsDownloader : Downloader
{
    private static readonly MediaFormat PlaylistFormat = new("application/x-mpegURL", "m3u8");
    private static readonly MediaFormat OutputFormat = new("video/MP2T", "ts");

    /// <summary>
    /// Initializes an instance of <see cref="HlsDownloader" />.
    /// </summary>
    public HlsDownloader(IHttpClientFactory httpClientFactory)
        : base(httpClientFactory) { }

    /// <summary>
    /// Initializes an instance of <see cref="HlsDownloader" />.
    /// </summary>
    public HlsDownloader(Func<HttpClient> httpClientFunc)
        : base(httpClientFunc) { }

    /// <summary>
    /// Initializes an instance of <see cref="HlsDownloader" />.
    /// </summary>
    public HlsDownloader()
        : this(new HttpClientFactory()) { }

    public async ValueTask<List<HlsStreamMetadata>> GetQualitiesAsync(
        string url,
        IDictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default
    ) => await GetQualitiesAsync(new Uri(url), headers, cancellationToken);

    public async ValueTask<List<HlsStreamMetadata>> GetQualitiesAsync(
        Uri uri,
        IDictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default
    )
    {
        var http = _httpClientFactory.CreateClient();

        var stream = await http.ExecuteAsync(uri, headers, cancellationToken);
        var doc = await GetPlaylistDocumentAsync(stream, uri);

        var grabResult = GetStreams(uri, doc).ToList();

        for (var i = 0; i < grabResult.Count; i++)
        {
            stream = await http.ExecuteAsync(grabResult[i].ResourceUri, headers, cancellationToken);

            doc = await GetPlaylistDocumentAsync(stream, grabResult[i].ResourceUri);

            var segments = GetSegments(grabResult[i].ResourceUri, doc);

            grabResult[i].Stream = segments.FirstOrDefault()?.Stream;
        }

        return grabResult;
    }

    private async ValueTask<PlaylistDocument> GetPlaylistDocumentAsync(
        //Stream stream,
        string stream,
        Uri uri
    )
    {
        var doc = new PlaylistDocument();

        try
        {
            await doc.LoadAsync(stream, uri);

            return doc;
        }
        catch (PlaylistDocumentLoadException loadException)
        {
            throw new HttpzException("Failed to load the M3U8 playlist.", loadException);
        }
    }

    /// <summary>
    /// Downloads a hls/m3u8 video from a url. To prevent slight non synchronization
    /// with the audio/video, you can run the ffmpeg command:
    /// ffmpeg -i C:\path\video.ts -acodec copy -vcodec copy C:\path\video.mp4
    /// </summary>
    public async ValueTask DownloadAsync(
        HlsStream stream,
        string filePath,
        IDictionary<string, string>? headers = null,
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default
    )
    {
        for (var i = 0; i < stream.Segments.Count; i++)
        {
            var segment = stream.Segments[i];

            await DownloadAsync(
                segment.Uri.AbsoluteUri,
                filePath,
                headers,
                null,
                true,
                cancellationToken
            );

            progress?.Report((i / (double)stream.Segments.Count * 100.0) / 100.0);
        }
    }

    /// <summary>
    /// Downloads an hls/m3u8 video from a url.
    /// </summary>
    public async ValueTask DownloadAllThenMergeAsync(
        HlsStream stream,
        IDictionary<string, string> headers,
        string filePath,
        IProgress<double>? progress = null,
        int maxParallelDownloads = 10,
        CancellationToken cancellationToken = default
    )
    {
        var tempFiles = new List<string>();

        try
        {
            using var downloadSemaphore = new ResizableSemaphore
            {
                MaxCount = maxParallelDownloads
            };

            var total = 0;

            var tasks = Enumerable
                .Range(0, stream.Segments.Count)
                .Select(
                    i =>
                        Task.Run(async () =>
                        {
                            using var access = await downloadSemaphore.AcquireAsync(
                                cancellationToken
                            );

                            var segment = stream.Segments[i];

                            var outputPath =
                                Path.Combine(Path.GetTempPath(), DateTime.Now.Ticks.ToString())
                                + $"_{i}.tmp";

                            tempFiles.Add(outputPath);

                            await DownloadAsync(
                                segment.Uri.AbsoluteUri,
                                outputPath,
                                headers,
                                null,
                                false,
                                cancellationToken
                            );

                            total++;

                            progress?.Report(
                                (total / (double)stream.Segments.Count * 100.0) / 100.0
                            );
                        })
                );

            await Task.WhenAll(tasks);

            progress?.Report(1);

            tempFiles = tempFiles
                .OrderBy(
                    x =>
                        Convert.ToInt64(
                            Path.GetFileNameWithoutExtension(x).Split('_').LastOrDefault()
                        )
                )
                .ToList();

            await FileEx.Combine(tempFiles, filePath);
        }
        finally
        {
            foreach (var tempFile in tempFiles)
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }
    }

    public bool Supports(Uri uri) =>
        uri.AbsolutePath.EndsWith(".m3u8", StringComparison.InvariantCultureIgnoreCase);

    private IList<HlsStreamMetadata> GetStreams(Uri originalUri, PlaylistDocument doc)
    {
        var list = new List<HlsStreamMetadata>();

        foreach (var stream in doc.Streams)
        {
            var uri = new Uri(originalUri, stream.Uri);

            var streamMetadata = new HlsStreamMetadata(
                uri,
                stream.Name,
                stream.Resolution,
                stream.Bandwidth,
                PlaylistFormat,
                OutputFormat,
                null
            );

            list.Add(streamMetadata);
        }

        return list.OrderByDescending(s => s.Resolution?.Height).ToList();
    }

    private IList<HlsStreamMetadata> GetSegments(Uri originalUri, PlaylistDocument doc)
    {
        var list = new List<HlsStreamMetadata>();
        var segments = new List<MediaSegment>();
        var totalDuration = TimeSpan.Zero;

        foreach (var segment in doc.Segments)
        {
            totalDuration += segment.Duration;
            var uri = new Uri(originalUri, segment.Uri);
            segments.Add(new MediaSegment(segment.Title, uri, segment.Duration));
        }

        var hlsStream = new HlsStream
        {
            OriginalUri = originalUri,
            ResourceUri = originalUri,
            Length = totalDuration,
            Segments = segments,
        };

        list.Add(
            new HlsStreamMetadata(
                originalUri,
                "Media",
                null,
                0,
                PlaylistFormat,
                OutputFormat,
                hlsStream
            )
        );

        return list;
    }
}
