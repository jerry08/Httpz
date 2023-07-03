using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Httpz.Domain;

/// <summary>
/// FileTldRuleProvider
/// </summary>
public class FileTldRuleProvider : ITldRuleProvider
{
    private readonly string _fileName;

    /// <summary>
    /// FileTldRuleProvider
    /// </summary>
    /// <param name="fileName"></param>
    public FileTldRuleProvider(string fileName)
    {
        _fileName = fileName;
    }

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
        if (!File.Exists(_fileName))
        {
            throw new FileNotFoundException("Rule file does not exist");
        }

        using var reader = File.OpenText(_fileName);
        return await reader.ReadToEndAsync().ConfigureAwait(false);
    }
}
