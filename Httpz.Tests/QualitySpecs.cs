using System.Threading.Tasks;
using FluentAssertions;
using Httpz.Tests.TestData;
using Xunit;

namespace Httpz.Tests;

public class QualitySpecs
{
    [Fact]
    public async Task I_can_get_quality_results_from_an_hls_url()
    {
        // Arrange
        var downloader = new HlsDownloader();

        // Act
        var results = await downloader.GetQualitiesAsync(HlsUrls.Url1);

        // Assert
        results.Should().HaveCountGreaterOrEqualTo(1);
    }
}
