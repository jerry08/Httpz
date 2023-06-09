using System;

namespace Httpz.Hls;

public class HlsStreamInfo
{
    public int ProgramId { get; }

    public int Bandwidth { get; }

    public RectSize? Resolution { get; }

    public string? Name { get; }

    public Uri Uri { get; }

    public HlsStreamInfo(
        Uri uri,
        int programId,
        int bandwidth,
        RectSize? resolution,
        string? name)
    {
        ProgramId = programId;
        Bandwidth = bandwidth;
        Resolution = resolution;
        Name = name;
        Uri = uri;
    }
}