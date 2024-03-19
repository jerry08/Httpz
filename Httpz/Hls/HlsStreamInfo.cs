using System;

namespace Httpz.Hls;

public class HlsStreamInfo(
    Uri uri,
    int programId,
    int bandwidth,
    RectSize? resolution,
    string? name
)
{
    public int ProgramId { get; } = programId;

    public int Bandwidth { get; } = bandwidth;

    public RectSize? Resolution { get; } = resolution;

    public string? Name { get; } = name;

    public Uri Uri { get; } = uri;
}
