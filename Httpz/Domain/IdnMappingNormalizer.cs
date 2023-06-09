using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Httpz.Domain;

/// <summary>
/// IdnMappingNormalizer
/// </summary>
public class IdnMappingNormalizer : IDomainNormalizer
{
    private readonly IdnMapping _idnMapping = new();

    public List<string> PartlyNormalizeDomainAndExtractFullyNormalizedParts(
        string domain,
        out string partlyNormalizedDomain)
    {
        partlyNormalizedDomain = string.Empty;

        if (string.IsNullOrEmpty(domain))
        {
            return new();
        }

        partlyNormalizedDomain = domain.ToLowerInvariant();

        var punycodeConvertedDomain = partlyNormalizedDomain;
        if (partlyNormalizedDomain.Contains("xn--"))
        {
            punycodeConvertedDomain = _idnMapping.GetUnicode(partlyNormalizedDomain);
        }

        return punycodeConvertedDomain
            .Split('.')
            .Reverse()
            .ToList();
    }
}