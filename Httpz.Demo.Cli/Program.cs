using Httpz;
using Spectre.Console;

namespace ConsoleApp1;

internal static class Program
{
    static async Task Main()
    {
        Console.Title = "Httpz Demo";

        var url = "http://sample.vodobox.net/skate_phantom_flex_4k/skate_phantom_flex_4k.m3u8";

        var filepath1 = Path.Join(Environment.CurrentDirectory, "example_video.ts");
        var hlsDownloader = new HlsDownloader();
        var qualities = await hlsDownloader.GetQualitiesAsync(url);

        await AnsiConsole.Progress().StartAsync(async ctx =>
        {
            var progressTask = ctx.AddTask("[cyan]Downloading test video[/]");
            progressTask.MaxValue = 1;

            await hlsDownloader.DownloadAsync(qualities[0].Stream!, filepath1, null, progressTask);
        });
    }
}