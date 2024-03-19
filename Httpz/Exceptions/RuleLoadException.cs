using System;

namespace Httpz.Exceptions;

/// <summary>
/// Rule Load Exception
/// </summary>
/// <remarks>
/// Rule Load Exception
/// </remarks>
/// <param name="error"></param>
public class RuleLoadException(string error) : Exception
{
    /// <summary>
    /// Error Message
    /// </summary>
    public string Error { get; set; } = error;
}
