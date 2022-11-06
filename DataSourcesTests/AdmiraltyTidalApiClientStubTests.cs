namespace DataSourcesTests;

public class AdmiraltyTidalApiClientStubTests
{
    [Test]
    public async Task AdmiraltyTidalApiClientStubShouldDeserializeTestData()
    {
        var client = new AdmiraltyTidalApiClientStub();
        var result = await client.GetTidalEventsByStationId(123);

        Assert.That(result.Count, Is.EqualTo(2));
    }
}