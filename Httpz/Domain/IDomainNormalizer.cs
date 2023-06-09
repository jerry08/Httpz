using System.Collections.Generic;

namespace Httpz.Domain;

public interface IDomainNormalizer
{
    List<string> PartlyNormalizeDomainAndExtractFullyNormalizedParts(
        string domain,
        out string partlyNormalizedDomain);
}