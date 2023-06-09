using System.Collections.Generic;
using System.Threading.Tasks;

namespace Httpz.Domain;

/// <summary>
/// ITldRuleProvider
/// </summary>
public interface ITldRuleProvider
{
    /// <summary>
    /// Loads the plain text data from a source and parse the public suffix rules
    /// </summary>
    /// <returns>Returns the TldRules</returns>
    Task<IEnumerable<TldRule>> BuildAsync();
}