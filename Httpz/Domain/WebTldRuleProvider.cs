using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Httpz.Exceptions;

namespace Httpz.Domain;

/// <summary>
/// WebTldRuleProvider
/// </summary>
public class WebTldRuleProvider : ITldRuleProvider
{
    private readonly string _fileUrl;
    private readonly ICacheProvider _cacheProvider;

    /// <summary>
    /// Returns the cache provider
    /// </summary>
    public ICacheProvider CacheProvider { get { return _cacheProvider; } }

    /// <summary>
    /// WebTldRuleProvider<br/>
    /// Loads the public suffix definition file from a given url
    /// </summary>
    /// <param name="url"></param>
    /// <param name="cacheProvider">default is <see cref="FileCacheProvider"/></param>
    public WebTldRuleProvider(
        string url = "https://publicsuffix.org/list/public_suffix_list.dat",
        ICacheProvider? cacheProvider = null)
    {
        _fileUrl = url;

        if (cacheProvider is null)
        {
            _cacheProvider = new FileCacheProvider();
            return;
        }

        _cacheProvider = cacheProvider;
    }

    ///<inheritdoc/>
    public async Task<IEnumerable<TldRule>> BuildAsync()
    {
        var ruleParser = new TldRuleParser();

        string ruleData;
        if (!_cacheProvider.IsCacheValid())
        {
            ruleData = await LoadFromUrlAsync(_fileUrl).ConfigureAwait(false);
            await _cacheProvider.SetAsync(ruleData).ConfigureAwait(false);
        }
        else
        {
            ruleData = await _cacheProvider.GetAsync().ConfigureAwait(false);
        }

        var rules = ruleParser.ParseRules(ruleData);
        return rules;
    }

    /// <summary>
    /// Load the public suffix data from the given url
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public async Task<string> LoadFromUrlAsync(string url)
    {
        using var httpClient = new HttpClient();
        using var response = await httpClient.GetAsync(url).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            throw new RuleLoadException($"Cannot load from {url} {response.StatusCode}");
        }

        return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
    }
}