using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Httpz.Domain;

/// <summary>
/// FileTldRuleProvider
/// </summary>
/// <remarks>
/// FileTldRuleProvider
/// </remarks>
/// <param name="fileName"></param>
public class FileTldRuleProvider(string fileName) : ITldRuleProvider
{
    ///<inheritdoc/>
    public async ValueTask<IEnumerable<TldRule>> BuildAsync()
    {
        var ruleData = await LoadFromFile().ConfigureAwait(false);

        var ruleParser = new TldRuleParser();
        var rules = ruleParser.ParseRules(ruleData);
        return rules;
    }

    private async ValueTask<string> LoadFromFile()
    {
        if (!File.Exists(fileName))
        {
            throw new FileNotFoundException("Rule file does not exist");
        }

        using var reader = File.OpenText(fileName);
        return await reader.ReadToEndAsync().ConfigureAwait(false);
    }
}
