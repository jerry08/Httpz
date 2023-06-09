using System;

namespace Httpz.Hls;

public class HlsKey
{
    public HlsKeyMethod Method { get; }

    public Uri Uri { get; }

    public byte[]? Iv { get; }

    public HlsKey(HlsKeyMethod method, Uri uri, byte[]? iv)
    {
        Method = method;
        Uri = uri;
        Iv = iv;
    }
}