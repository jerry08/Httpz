using System;

namespace Httpz.Exceptions;

public class PlaylistDocumentLoadException(string message) : Exception(message)
{
    public PlaylistDocumentLoadException()
        : this("Invalid M3U8 file.") { }
}
