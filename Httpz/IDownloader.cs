using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Httpz;

public interface IDownloader
{
    Task DownloadAsync(
        string url,
        string filePath,
        Dictionary<string, string>? headers = null,
        IProgress<double>? progress = null,
        bool append = false,
        CancellationToken cancellationToken = default);

    Task DownloadAsync(
        Uri url,
        string filePath,
        Dictionary<string, string>? headers = null,
        IProgress<double>? progress = null,
        bool append = false,
        CancellationToken cancellationToken = default);
}