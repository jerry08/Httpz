using System;
using Httpz.Domain;

namespace Httpz.Exceptions;

/// <summary>
/// Parse Exception
/// </summary>
public class DomainParseException : Exception
{
    /// <summary>
    /// Reason of exception
    /// </summary>
    public TldRule? WinningRule { get; }

    /// <summary>
    /// Reason of exception
    /// </summary>
    public string ErrorMessage { get; }

    /// <summary>
    /// Parse Exception
    /// </summary>
    /// <param name="errorMessage"></param>
    /// <param name="winningRule"></param>
    public DomainParseException(string errorMessage, TldRule? winningRule = null)
    {
        ErrorMessage = errorMessage;
        WinningRule = winningRule;
    }

    /// <summary>
    /// Message
    /// </summary>
    public override string Message => ErrorMessage;
}
