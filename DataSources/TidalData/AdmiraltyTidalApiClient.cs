using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace DataSources.TidalData;

using TidalEvents = List<TidalEvent>;

public class AdmiraltyTidalApiClient
{
    public AdmiraltyTidalApiClient(ILogger<AdmiraltyTidalApiClient> logger, HttpClient httpClient) => (_httpClient) = (httpClient);
    private readonly HttpClient _httpClient;

    private static JsonSerializerOptions GetJsonSerializerOptions() {
        return new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = {
                new JsonStringEnumConverter()
            }
        };
    }
    public async Task<TidalEvents> GetTidalEventsByStationId(string stationId) {
        string? responseString = await _httpClient.GetStringAsync($"/uktidalapi/api/V1/Stations/{stationId}/TidalEvents");
        return JsonSerializer.Deserialize<TidalEvents>(responseString, GetJsonSerializerOptions());
    }

    public async Task<Station> GetStationById(string stationId) {
        string? responseString = await _httpClient.GetStringAsync($"/uktidalapi/api/V1/Stations/{stationId}");
        return JsonSerializer.Deserialize<Station>(responseString, GetJsonSerializerOptions());
    }

}