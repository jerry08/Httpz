using System;

namespace Httpz.Hls;

/// <summary>
/// Represents a single segment of a <see cref="HlsStream"/>.
/// </summary>
public class MediaSegment
{
    /// <summary>
    /// Optional title
    /// </summary>
    public string? Title { get; }

    /// <summary>
    /// Absolute URI of the media segment
    /// </summary>
    public Uri Uri { get; }

    /// <summary>
    /// Optional duration of the segment
    /// </summary>
    public TimeSpan? Duration { get; }

    public MediaSegment(string? title, Uri uri, TimeSpan? duration)
    {
        Title = title;
        Uri = uri;
        Duration = duration;
    }
}