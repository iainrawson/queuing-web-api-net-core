using DataSources.TidalData;

namespace DataSourcesTests;

public class AdmiraltyTidalApiClientStubTests
{
    [Test]
    public async Task AdmiraltyTidalApiClientStubShouldDeserializeTidalData()
    {
        var client = new AdmiraltyTidalApiClientStub();
        var result = await client.GetTidalEventsByStationId(123);

        Assert.That(result.Count, Is.EqualTo(2));
    }

        [Test]
    public async Task AdmiraltyTidalApiClientStubShouldDeserializeStationData()
    {
        var client = new AdmiraltyTidalApiClientStub();
        var result = await client.GetStationById(123);

        Assert.That(result, Is.Not.Null);
    }

}