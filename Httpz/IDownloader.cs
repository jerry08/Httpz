using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Httpz;

public interface IDownloader
{
    ValueTask DownloadAsync(
        string url,
        Stream destination,
        IDictionary<string, string?>? headers = null,
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default
    );

    ValueTask DownloadAsync(
        Uri uri,
        Stream destination,
        IDictionary<string, string?>? headers = null,
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default
    );

    ValueTask DownloadAsync(
        string url,
        string filePath,
        IDictionary<string, string?>? headers = null,
        IProgress<double>? progress = null,
        bool append = false,
        CancellationToken cancellationToken = default
    );

    ValueTask DownloadAsync(
        Uri uri,
        string filePath,
        IDictionary<string, string?>? headers = null,
        IProgress<double>? progress = null,
        bool append = false,
        CancellationToken cancellationToken = default
    );
}
