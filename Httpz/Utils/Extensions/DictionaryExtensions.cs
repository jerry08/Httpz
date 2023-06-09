using System.Collections.Generic;

namespace Httpz.Utils.Extensions;

internal static class DictionaryExtensions
{
    public static TValue? GetOrDefault<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key)
        where TKey : notnull
    {
        dictionary.TryGetValue(key, out var value);
        return value;
    }
}