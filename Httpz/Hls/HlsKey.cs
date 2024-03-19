using System;

namespace Httpz.Hls;

public class HlsKey(HlsKeyMethod method, Uri uri, byte[]? iv)
{
    public HlsKeyMethod Method { get; } = method;

    public Uri Uri { get; } = uri;

    public byte[]? Iv { get; } = iv;
}
