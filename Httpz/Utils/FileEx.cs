using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Httpz.Utils;

internal static class FileEx
{
    public static async ValueTask Combine(IEnumerable<string> inputFilePaths, string outputFilePath)
    {
        Debug.WriteLine($"Number of files: {inputFilePaths.Count()}.");
        using var outputStream = File.Create(outputFilePath);
        foreach (var inputFilePath in inputFilePaths)
        {
            using var inputStream = File.OpenRead(inputFilePath);
            // Buffer size can be passed as the second argument.
            await inputStream.CopyToAsync(outputStream);
            Debug.WriteLine($"The file {inputFilePath} has been processed.");
        }
    }
}
