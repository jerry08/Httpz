﻿using System;

namespace Httpz.Hls;

/// <summary>
/// Implements <see cref="IGrabbed"/> to provide information about HLS master playlist streams. This class represents
/// a single stream, which refers to an M3U8 playlist containing the actual media files.
/// </summary>
public class HlsStreamMetadata
{
    public HlsStreamMetadata(
        Uri resourceUri,
        string? name,
        RectSize? resolution,
        int bandwidth,
        MediaFormat format,
        MediaFormat outputFormat,
        HlsStream? stream
    )
    {
        ResourceUri = resourceUri;
        Name = name;
        Resolution = resolution;
        Bandwidth = bandwidth;
        StreamFormat = format;
        OutputFormat = outputFormat;
        Stream = stream;
    }

    public Uri? OriginalUri { get; }

    public Uri ResourceUri { get; set; }

    /// <summary>
    /// Optional name for the stream
    /// </summary>
    public string? Name { get; }

    /// <summary>
    /// Optional resolution of the stream
    /// </summary>
    public RectSize? Resolution { get; }

    /// <summary>
    /// Bandwidth - or 0 if unknown
    /// </summary>
    public int Bandwidth { get; }

    /// <summary>
    /// Format of the stream file
    /// </summary>
    public MediaFormat StreamFormat { get; }

    /// <summary>
    /// Expected format of the output media
    /// </summary>
    public MediaFormat OutputFormat { get; }

    /// <summary>
    /// Resolves the stream associated with current metadata.
    /// </summary>
    public HlsStream? Stream { get; set; }
}
