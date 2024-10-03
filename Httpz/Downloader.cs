using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Httpz.Exceptions;
using Httpz.Utils.Extensions;

namespace Httpz;

public class Downloader : IDownloader
{
    internal readonly IHttpClientFactory _httpClientFactory;

    /// <summary>
    /// Initializes an instance of <see cref="Downloader" />.
    /// </summary>
    public Downloader(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// Initializes an instance of <see cref="Downloader" />.
    /// </summary>
    public Downloader(Func<HttpClient> httpClientFunc)
    {
        _httpClientFactory = new HttpClientFactory(httpClientFunc);
    }

    /// <summary>
    /// Initializes an instance of <see cref="Downloader" />.
    /// </summary>
    public Downloader()
        : this(new HttpClientFactory()) { }

    public async ValueTask DownloadAsync(
        string url,
        string filePath,
        IDictionary<string, string?>? headers = null,
        IProgress<double>? progress = null,
        bool append = false,
        CancellationToken cancellationToken = default
    ) => await DownloadAsync(new Uri(url), filePath, headers, progress, append, cancellationToken);

    public async ValueTask DownloadAsync(
        Uri uri,
        string filePath,
        IDictionary<string, string?>? headers = null,
        IProgress<double>? progress = null,
        bool append = false,
        CancellationToken cancellationToken = default
    )
    {
        var http = _httpClientFactory.CreateClient();

        var request = new HttpRequestMessage(HttpMethod.Get, uri);

        if (headers is not null)
        {
            foreach (var (key, value) in headers)
                request.Headers.TryAddWithoutValidation(key, value);
        }

        var response = await http.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken
        );

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Response status code does not indicate success: {(int)response.StatusCode} ({response.StatusCode})."
                    + Environment.NewLine
                    + "Request:"
                    + Environment.NewLine
                    + request
            );
        }

        //var totalLength = progress is not null ?
        //    await http.GetFileSizeAsync(url,
        //        headers, cancellationToken) : 0;

        var totalLength = response.Content.Headers.ContentLength ?? 0;

        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

        var dir = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(dir))
            throw new HttpzException($"Directory: '{dir}' does not exist.");

        //var file = File.Create(filePath);
        using var file = new FileStream(filePath, FileMode.OpenOrCreate);

        if (append)
            file.Seek(0, SeekOrigin.End);

        try
        {
            await stream.CopyToAsync(
                file,
                progress,
                totalLength,
                cancellationToken: cancellationToken
            );
        }
        finally
        {
            file?.Close();
            stream?.Close();
        }
    }

    public async ValueTask DownloadAsync(
        string url,
        Stream destination,
        IDictionary<string, string?>? headers = null,
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default
    ) => await DownloadAsync(new Uri(url), destination, headers, progress, cancellationToken);

    public async ValueTask DownloadAsync(
        Uri uri,
        Stream destination,
        IDictionary<string, string?>? headers = null,
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default
    )
    {
        var http = _httpClientFactory.CreateClient();

        var request = new HttpRequestMessage(HttpMethod.Get, uri);

        if (headers is not null)
        {
            foreach (var (key, value) in headers)
                request.Headers.TryAddWithoutValidation(key, value);
        }

        var response = await http.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken
        );

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Response status code does not indicate success: {(int)response.StatusCode} ({response.StatusCode})."
                    + Environment.NewLine
                    + "Request:"
                    + Environment.NewLine
                    + request
            );
        }

        var totalLength = response.Content.Headers.ContentLength ?? 0;

        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

        try
        {
            await stream.CopyToAsync(
                destination,
                progress,
                totalLength,
                cancellationToken: cancellationToken
            );
        }
        finally
        {
            stream?.Close();
        }
    }
}
