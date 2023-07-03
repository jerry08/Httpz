// ReSharper disable CheckNamespace

#if !NET5_0
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

internal static class StreamPolyfills
{
#if !NETSTANDARD2_1 && !NETCOREAPP3_0
    public static async ValueTask<int> ReadAsync(this Stream stream, byte[] buffer, CancellationToken cancellationToken) =>
        await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
#endif

    public static async ValueTask<Stream> ReadAsStreamAsync(
        this HttpContent httpContent,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await httpContent.ReadAsStreamAsync();
    }

    public static async ValueTask<string> ReadAsStringAsync(
        this HttpContent httpContent,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await httpContent.ReadAsStringAsync();
    }
}
#endif