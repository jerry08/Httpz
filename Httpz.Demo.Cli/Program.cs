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

        var tt = "https://fonts.gstatic.com/s/i/short-term/release/materialsymbolsoutlined/10k/fill1/48px.svg\r\n";

        var fileName = $"test.svg";

        var filepath = Path.Join(Environment.CurrentDirectory, "Downloads", fileName);

        var downloader1 = new Downloader();
        await downloader1.DownloadAsync(tt, filepath);

        //var client = new AnimeClient();
        //var animes = await client.Gogoanime.SearchAsync("anohana");
        //var episodes = await client.Gogoanime.GetEpisodesAsync(animes[0].Id);
        //var servers = await client.Gogoanime.GetVideoServersAsync(episodes[0].Id);
        //var videos = await client.Gogoanime.GetVideosAsync(servers[0]);
        //
        //var video = videos.FirstOrDefault()!;
        //
        //if (video.Format == VideoType.Container)
        //{
        //    var filePath = Path.Join(Environment.CurrentDirectory, "Downloads", "my video.mp4");
        //
        //    var downloader = new Downloader();
        //    await downloader.DownloadAsync(video.VideoUrl, filePath, videos[0].Headers);
        //}
        //else
        //{
        //    var filePath = Path.Join(Environment.CurrentDirectory, "Downloads", "my video.ts");
        //
        //    var downloader = new HlsDownloader();
        //    var streams = await downloader.GetQualitiesAsync(videos[0].VideoUrl, videos[0].Headers);
        //
        //    var quality = streams.FirstOrDefault();
        //
        //    await downloader.DownloadAsync(quality!.Stream!, filePath, videos[0].Headers);
        //}
    }
}