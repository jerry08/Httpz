using System.Threading.Tasks;

namespace Httpz.Domain;

/// <summary>
/// ICacheProvider
/// </summary>
public interface ICacheProvider
{
    /// <summary>
    /// Get the data of the cache file
    /// </summary>
    /// <returns>Returns null if the cache has expired</returns>
    Task<string> GetAsync();

    /// <summary>
    /// Create or update the cache file
    /// </summary>
    /// <param name="data"></param>
    Task SetAsync(string data);

    /// <summary>
    /// Check if the cache is still valid
    /// </summary>
    bool IsCacheValid();
}