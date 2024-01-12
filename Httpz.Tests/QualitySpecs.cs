using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Httpz.Tests;

public class QualitySpecs
{
    [Theory]
    [InlineData("http://playertest.longtailvideo.com/adaptive/wowzaid3/playlist.m3u8")]
    [InlineData("http://sample.vodobox.net/skate_phantom_flex_4k/skate_phantom_flex_4k.m3u8")]
    public async Task I_can_get_quality_results_from_an_hls_url(string url)
    {
        // Arrange
        var downloader = new HlsDownloader();

        // Act
        var results = await downloader.GetQualitiesAsync(url);

        // Assert
        results.Should().NotBeEmpty();
    }
}
