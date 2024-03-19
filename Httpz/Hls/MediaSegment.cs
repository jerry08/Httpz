using System;

namespace Httpz.Hls;

/// <summary>
/// Represents a single segment of a <see cref="HlsStream"/>.
/// </summary>
public class MediaSegment(string? title, Uri uri, TimeSpan? duration)
{
    /// <summary>
    /// Optional title
    /// </summary>
    public string? Title { get; } = title;

    /// <summary>
    /// Absolute URI of the media segment
    /// </summary>
    public Uri Uri { get; } = uri;

    /// <summary>
    /// Optional duration of the segment
    /// </summary>
    public TimeSpan? Duration { get; } = duration;
}
