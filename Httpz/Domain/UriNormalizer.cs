using System;
using System.Collections.Generic;
using System.Linq;
using Httpz.Exceptions;

namespace Httpz.Domain;

public class UriNormalizer : IDomainNormalizer
{
    public List<string> PartlyNormalizeDomainAndExtractFullyNormalizedParts(
        string domain,
        out string partlyNormalizedDomain)
    {
        partlyNormalizedDomain = string.Empty;

        if (string.IsNullOrEmpty(domain))
        {
            return new();
        }

        //We use Uri methods to normalize host (So Punycode is converted to UTF-8)
        if (!domain.Contains("https://"))
        {
            domain = string.Concat("https://", domain);
        }

        if (!Uri.TryCreate(domain, UriKind.RelativeOrAbsolute, out var uri))
        {
            throw new DomainParseException("Cannot parse domain to an uri");
        }

        partlyNormalizedDomain = uri.Host;
        var normalizedHost = uri.GetComponents(UriComponents.NormalizedHost, UriFormat.UriEscaped); //Normalize punycode

        return normalizedHost
            .Split('.')
            .Reverse()
            .ToList();
    }
}