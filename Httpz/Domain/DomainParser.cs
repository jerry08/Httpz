using System;
using System.Collections.Generic;
using System.Linq;
using Httpz.Exceptions;
using Httpz.Utils.Extensions;

namespace Httpz.Domain;

/// <summary>
/// Domain parser
/// </summary>
public class DomainParser : IDomainParser
{
    private DomainDataStructure? _domainDataStructure;
    private readonly IDomainNormalizer _domainNormalizer;
    private readonly TldRule _rootTldRule = new("*");

    /// <summary>
    /// Creates and initializes a DomainParser
    /// </summary>
    /// <param name="rules">The list of rules.</param>
    /// <param name="domainNormalizer">An <see cref="IDomainNormalizer"/>.</param>
    public DomainParser(
        IEnumerable<TldRule> rules,
        IDomainNormalizer? domainNormalizer = null) : this(domainNormalizer)
    {
        if (rules is null)
        {
            throw new ArgumentNullException(nameof(rules));
        }

        AddRules(rules);
    }

    /// <summary>
    /// Creates and initializes a DomainParser
    /// </summary>
    /// <param name="ruleProvider">A rule provider from interface <see cref="ITldRuleProvider"/>.</param>
    /// <param name="domainNormalizer">An <see cref="IDomainNormalizer"/>.</param>
    public DomainParser(ITldRuleProvider ruleProvider, IDomainNormalizer? domainNormalizer = null)
        : this(domainNormalizer)
    {
        var rules = ruleProvider.BuildAsync().GetAwaiter().GetResult();
        AddRules(rules);
    }

    /// <summary>
    /// Creates a DomainParser based on an already initialzed tree.
    /// </summary>
    /// <param name="initializedDataStructure">An already initialized tree.</param>
    /// <param name="domainNormalizer">An <see cref="IDomainNormalizer"/>.</param>
    public DomainParser(
        DomainDataStructure initializedDataStructure,
        IDomainNormalizer? domainNormalizer = null) : this(domainNormalizer)
    {
        _domainDataStructure = initializedDataStructure;
    }

    private DomainParser(IDomainNormalizer? domainNormalizer)
    {
        _domainNormalizer = domainNormalizer ?? new UriNormalizer();
    }

    [Obsolete("Get is deprecated, please use Parse instead.", error: true)]
    public DomainInfo Get(Uri domain)
    {
        return Parse(domain);
    }

    [Obsolete("Get is deprecated, please use Parse instead.", error: true)]
    public DomainInfo Get(string domain)
    {
        return Parse(domain);
    }

    ///<inheritdoc/>
    public DomainInfo Parse(Uri domain)
    {
        var partlyNormalizedDomain = domain.Host;
        var normalizedHost = domain.GetComponents(UriComponents.NormalizedHost, UriFormat.UriEscaped); //Normalize punycode

        var parts = normalizedHost
            .Split('.')
            .Reverse()
            .ToList();

        return GetDomainFromParts(partlyNormalizedDomain, parts);
    }

    ///<inheritdoc/>
    public DomainInfo Parse(string domain)
    {
        var parts = _domainNormalizer.PartlyNormalizeDomainAndExtractFullyNormalizedParts(domain, out var partlyNormalizedDomain);
        return GetDomainFromParts(partlyNormalizedDomain, parts);
    }

    ///<inheritdoc/>
    public bool IsValidDomain(string domain)
    {
        if (string.IsNullOrEmpty(domain))
            return false;

        if (Uri.TryCreate(domain, UriKind.Absolute, out _))
            return false;

        if (!Uri.TryCreate($"http://{domain}", UriKind.Absolute, out var uri))
            return false;

        if (!uri.DnsSafeHost.Equals(domain, StringComparison.OrdinalIgnoreCase))
            return false;

        if (domain[0] == '*')
            return false;

        try
        {
            var parts = _domainNormalizer.PartlyNormalizeDomainAndExtractFullyNormalizedParts(domain, out var partlyNormalizedDomain);

            var domainName = GetDomainFromParts(partlyNormalizedDomain, parts);
            if (domainName is null)
                return false;

            return !domainName.TLDRule!.Equals(_rootTldRule);
        }
        catch (DomainParseException)
        {
            return false;
        }
    }

    private void AddRules(IEnumerable<TldRule> tldRules)
    {
        _domainDataStructure ??= new DomainDataStructure("*", _rootTldRule);
        _domainDataStructure.AddRules(tldRules);
    }

    private DomainInfo GetDomainFromParts(string domain, List<string> parts)
    {
        if (parts is null || parts.Count == 0 || parts.Any(x => x.Equals(string.Empty)))
            throw new DomainParseException("Invalid domain part detected");

        var structure = _domainDataStructure;
        var matches = new List<TldRule>();
        FindMatches(parts, structure!, matches);

        //Sort so exceptions are first, then by biggest label count (with wildcards at bottom) 
        var sortedMatches = matches.OrderByDescending(x => x.Type == TldRuleType.WildcardException ? 1 : 0)
            .ThenByDescending(x => x.LabelCount)
            .ThenByDescending(x => x.Name);

        var winningRule = sortedMatches.FirstOrDefault();

        //Domain is TLD
        if (parts.Count == winningRule?.LabelCount)
        {
            parts.Reverse();
            var tld = string.Join(".", parts);

            if (winningRule.Type == TldRuleType.Wildcard)
            {
                if (tld.EndsWith(winningRule.Name.Substring(1)))
                    throw new DomainParseException("Domain is a TLD according publicsuffix", winningRule);
            }
            else if (tld.Equals(winningRule.Name))
            {
                throw new DomainParseException("Domain is a TLD according publicsuffix", winningRule);
            }

            throw new DomainParseException($"Unknown domain {domain}");
        }

        return new DomainInfo(domain, winningRule);
    }

    private void FindMatches(
        IEnumerable<string> parts,
        DomainDataStructure structure,
        List<TldRule> matches)
    {
        if (structure.TldRule is not null)
            matches.Add(structure.TldRule);

        var part = parts.FirstOrDefault();
        if (string.IsNullOrEmpty(part))
            return;

        if (structure.Nested.TryGetValue(part, out var foundStructure))
            FindMatches(parts.Skip(1), foundStructure, matches);

        if (structure.Nested.TryGetValue("*", out foundStructure))
            FindMatches(parts.Skip(1), foundStructure, matches);
    }
}