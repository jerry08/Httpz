﻿namespace Httpz.Zip;

internal class HttpZipDirectory
{
    public int Offset { get; set; }

    public int Size { get; set; }

    public short Entries { get; set; }
}
