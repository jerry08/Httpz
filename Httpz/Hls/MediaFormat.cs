﻿namespace Httpz.Hls;

/// <summary>
/// Represents format of media e.g. mp4
/// </summary>
public class MediaFormat
{
    /// <summary>
    /// Mime of the media e.g. video/mp4, image/jpeg
    /// </summary>
    public string Mime { get; set; } = default!;

    /// <summary>
    /// Suggested extension for the file e.g. mp4, mp3 etc.
    /// </summary>
    public string Extension { get; set; } = default!;

    public MediaFormat() { }

    public MediaFormat(string mime, string extension)
    {
        Mime = mime;
        Extension = extension;
    }
}