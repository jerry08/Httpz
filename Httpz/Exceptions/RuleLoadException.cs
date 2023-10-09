﻿using System;

namespace Httpz.Exceptions;

/// <summary>
/// Rule Load Exception
/// </summary>
public class RuleLoadException : Exception
{
    /// <summary>
    /// Error Message
    /// </summary>
    public string Error { get; set; }

    /// <summary>
    /// Rule Load Exception
    /// </summary>
    /// <param name="error"></param>
    public RuleLoadException(string error)
    {
        Error = error;
    }
}
