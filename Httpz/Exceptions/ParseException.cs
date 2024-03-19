using System;
using Httpz.Domain;

namespace Httpz.Exceptions;

/// <summary>
/// Parse Exception
/// </summary>
/// <remarks>
/// Parse Exception
/// </remarks>
/// <param name="errorMessage"></param>
/// <param name="winningRule"></param>
public class DomainParseException(string errorMessage, TldRule? winningRule = null) : Exception
{
    /// <summary>
    /// Reason of exception
    /// </summary>
    public TldRule? WinningRule { get; } = winningRule;

    /// <summary>
    /// Reason of exception
    /// </summary>
    public string ErrorMessage { get; } = errorMessage;

    /// <summary>
    /// Message
    /// </summary>
    public override string Message => ErrorMessage;
}
