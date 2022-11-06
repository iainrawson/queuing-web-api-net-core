using System.Text.Json;
using System.Text.Json.Serialization;

namespace DataSources.TidalData;

using TidalEvents = List<TidalEvent>;

public class AdmiraltyTidalApiClientStub
{
    private static JsonSerializerOptions GetJsonSerializerOptions() {
        return new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = {
                new JsonStringEnumConverter()
            }
        };
    }
    public async Task<TidalEvents> GetTidalEventsByStationId(int stationId) {

        TidalEvents? result = JsonSerializer.Deserialize<TidalEvents>(_dummyResult, GetJsonSerializerOptions());
        return result;
    }

    private readonly string _dummyResult = @"
    [
        {
            ""EventType"": ""LowWater"",
            ""DateTime"": ""2022-01-01T01:23:45.333"",
            ""IsApproximateTime"": false,
            ""Height"": 1.1231234564567891,
            ""IsApproximateHeight"": false,
            ""Filtered"": false,
            ""Date"": ""2022-01-01T00:00:00""
        },
        {
            ""EventType"": ""HighWater"",
            ""DateTime"": ""2022-11-02T03:45:43.012"",
            ""IsApproximateTime"": false,
            ""Height"": 3.4512235623123412,
            ""IsApproximateHeight"": false,
            ""Filtered"": false,
            ""Date"": ""2022-01-01T00:00:00""
        }
    ]";

}




