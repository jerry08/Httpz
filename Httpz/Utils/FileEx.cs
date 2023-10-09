using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Httpz.Utils;

internal static class FileEx
{
    public static async ValueTask CombineAsync(
        IEnumerable<string> inputFilePaths,
        string outputFilePath,
        int bufferSize = 0x1000,
        CancellationToken cancellationToken = default
    )
    {
        Debug.WriteLine($"Number of files: {inputFilePaths.Count()}.");
        using var outputStream = File.Create(outputFilePath);
        foreach (var inputFilePath in inputFilePaths)
        {
            using var inputStream = File.OpenRead(inputFilePath);
            await inputStream.CopyToAsync(outputStream, bufferSize, cancellationToken);
            Debug.WriteLine($"The file {inputFilePath} has been processed.");
        }
    }
}
