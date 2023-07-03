using System;
using System.IO;
using System.Threading.Tasks;

namespace Httpz.Domain;

/// <summary>
/// FileCacheProvider
/// Write the data to a cache file in the temp directory
/// </summary>
public class FileCacheProvider : ICacheProvider
{
    private readonly string _cacheFilePath;
    private readonly TimeSpan _timeToLive;

    /// <summary>
    /// FileCacheProvider
    /// </summary>
    /// <param name="cacheFileName"></param>
    /// <param name="cacheTimeToLive"></param>
    public FileCacheProvider(
        string cacheFileName = "publicsuffixcache.dat",
        TimeSpan? cacheTimeToLive = null)
    {
        _timeToLive = cacheTimeToLive.HasValue
            ? cacheTimeToLive.Value
            : TimeSpan.FromDays(1);

        var tempPath = Path.GetTempPath();
        _cacheFilePath = Path.Combine(tempPath, cacheFileName);
    }

    ///<inheritdoc/>
    public bool IsCacheValid()
    {
        var cacheInvalid = true;

        var fileInfo = new FileInfo(_cacheFilePath);
        if (fileInfo.Exists
            && fileInfo.LastWriteTimeUtc > DateTime.UtcNow.Subtract(_timeToLive))
        {
            cacheInvalid = false;
        }

        return !cacheInvalid;
    }

    ///<inheritdoc/>
    public async ValueTask<string> GetAsync()
    {
        if (!IsCacheValid())
        {
            return string.Empty;
        }

        using var reader = File.OpenText(_cacheFilePath);
        return await reader.ReadToEndAsync();
    }

    ///<inheritdoc/>
    public async ValueTask SetAsync(string data)
    {
        using var streamWriter = File.CreateText(_cacheFilePath);
        await streamWriter.WriteAsync(data).ConfigureAwait(false);
    }
}