namespace Httpz.Domain;

/// <summary>
/// TLD Rule type, defined by www.publicsuffix.org
/// </summary>
public enum TldRuleType
{
    Normal,
    Wildcard,
    WildcardException,
}
