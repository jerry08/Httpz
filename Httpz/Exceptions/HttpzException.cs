using System;

namespace Httpz.Exceptions;

/// <summary>
/// Exception thrown within <see cref="HttpzException"/>.
/// </summary>
public class HttpzException : Exception
{
    /// <summary>
    /// Initializes an instance of <see cref="HttpzException"/>.
    /// </summary>
    public HttpzException()
        : this("Failed to grab the target.") { }

    /// <summary>
    /// Initializes an instance of <see cref="HttpzException"/>.
    /// </summary>
    /// <param name="message"></param>
    public HttpzException(string message)
        : base(message) { }

    /// <summary>
    /// Initializes an instance of <see cref="HttpzException"/>.
    /// </summary>
    public HttpzException(string message, Exception innerException)
        : base(message, innerException) { }
}
