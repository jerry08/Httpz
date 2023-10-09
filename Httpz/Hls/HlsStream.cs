using System;
using System.Collections.Generic;

namespace Httpz.Hls;

/// <summary>
/// Describes a grabbed HLS stream.
/// </summary>
public class HlsStream
{
    public Uri? OriginalUri { get; set; }

    public Uri? ResourceUri { get; set; }

    /// <summary>
    /// Gets the total duration of the stream.
    /// </summary>
    public TimeSpan Length { get; set; }

    /// <summary>
    /// Gets the segments of the stream.
    /// </summary>
    public IReadOnlyList<MediaSegment> Segments { get; set; } = new List<MediaSegment>();
}
