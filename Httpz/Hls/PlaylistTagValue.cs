using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Httpz.Hls;

public class PlaylistTagValue(string key, string? wholeValue, IDictionary<string, string?>? values)
{
    public string Key { get; } = key;
    public string? WholeValue { get; } = wholeValue;
    public IDictionary<string, string?>? Values { get; } = values;

    public static PlaylistTagValue Parse(string content)
    {
        var split = content.Split(new[] { ':' }, 2);
        var key = split[0];
        var val = split.Length > 1 ? split[1] : null;
        return new PlaylistTagValue(key, val, ParseValues(val));
    }

    private static readonly Regex _valueRegex =
        new(@"(^|,?)([\w\-]+)=(""([^""]+)""|([^,]+))", RegexOptions.Compiled);

    private static IDictionary<string, string?>? ParseValues(string? content)
    {
        if (content is null)
            return null;

        var matches = _valueRegex.Matches(content);
        if (matches.Count == 0)
            return null;

        var dic = new Dictionary<string, string?>(StringComparer.InvariantCultureIgnoreCase);
        foreach (var match in matches.Cast<Match>())
        {
            var name = match.Groups[2].Value;
            var val = match.Groups[4].Value;
            if (string.IsNullOrEmpty(val))
                val = match.Groups[5].Value;
            dic.Add(name, val);
        }

        return dic;
    }
}
