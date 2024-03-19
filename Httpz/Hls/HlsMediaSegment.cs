using System;

namespace Httpz.Hls;

public class HlsMediaSegment(Uri uri, TimeSpan duration, string? title = null)
{
    /// <summary>
    /// Optional title of the segment
    /// </summary>
    public string? Title { get; } = title;

    /// <summary>
    /// Duration of the segment
    /// </summary>
    public TimeSpan Duration { get; } = duration;

    /// <summary>
    /// URI of the segment file - may be either relative or absolute
    /// </summary>
    public Uri Uri { get; } = uri;
}
