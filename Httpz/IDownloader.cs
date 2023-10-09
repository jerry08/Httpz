using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Httpz;

public interface IDownloader
{
    ValueTask DownloadAsync(
        string url,
        string filePath,
        Dictionary<string, string>? headers = null,
        IProgress<double>? progress = null,
        bool append = false,
        CancellationToken cancellationToken = default
    );

    ValueTask DownloadAsync(
        Uri url,
        string filePath,
        Dictionary<string, string>? headers = null,
        IProgress<double>? progress = null,
        bool append = false,
        CancellationToken cancellationToken = default
    );
}
