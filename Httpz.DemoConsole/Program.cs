using Httpz;
using Juro.Clients;
using Juro.Models.Videos;

namespace ConsoleApp1;

internal class Program
{
    static async Task Main()
    {
        Console.WriteLine("Hello, World!");

        var client = new AnimeClient();
        var animes = await client.Gogoanime.SearchAsync("anohana");
        var episodes = await client.Gogoanime.GetEpisodesAsync(animes[0].Id);
        var servers = await client.Gogoanime.GetVideoServersAsync(episodes[0].Id);
        var videos = await client.Gogoanime.GetVideosAsync(servers[0]);

        var video = videos.FirstOrDefault()!;

        if (video.Format == VideoType.Container)
        {
            var filePath = Path.Join(Environment.CurrentDirectory, "Downloads", "my video.mp4");

            var downloader = new Downloader();
            await downloader.DownloadAsync(video.VideoUrl, filePath, videos[0].Headers);
        }
        else
        {
            var filePath = Path.Join(Environment.CurrentDirectory, "Downloads", "my video.ts");

            var downloader = new HlsDownloader();
            var streams = await downloader.GetQualitiesAsync(videos[0].VideoUrl, videos[0].Headers);

            var quality = streams.FirstOrDefault();

            await downloader.DownloadAsync(quality!.Stream!, filePath, videos[0].Headers);
        }
    }
}