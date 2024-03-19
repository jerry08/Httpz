using System;

namespace Httpz.Hls;

/// <summary>
/// Provides information about HLS master playlist streams. This class represents
/// a single stream, which refers to an M3U8 playlist containing the actual media files.
/// </summary>
public class HlsStreamMetadata(
    Uri resourceUri,
    string? name,
    RectSize? resolution,
    int bandwidth,
    MediaFormat format,
    MediaFormat outputFormat,
    HlsStream? stream
)
{
    public Uri? OriginalUri { get; }

    public Uri ResourceUri { get; set; } = resourceUri;

    /// <summary>
    /// Optional name for the stream
    /// </summary>
    public string? Name { get; } = name;

    /// <summary>
    /// Optional resolution of the stream
    /// </summary>
    public RectSize? Resolution { get; } = resolution;

    /// <summary>
    /// Bandwidth - or 0 if unknown
    /// </summary>
    public int Bandwidth { get; } = bandwidth;

    /// <summary>
    /// Format of the stream file
    /// </summary>
    public MediaFormat StreamFormat { get; } = format;

    /// <summary>
    /// Expected format of the output media
    /// </summary>
    public MediaFormat OutputFormat { get; } = outputFormat;

    /// <summary>
    /// Resolves the stream associated with current metadata.
    /// </summary>
    public HlsStream? Stream { get; set; } = stream;

    public override string? ToString() => $"Quality ({Resolution})";
}
