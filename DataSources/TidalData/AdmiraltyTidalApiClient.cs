using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace DataSources.TidalData;

using TidalEvents = List<TidalEvent>;

public class AdmiraltyTidalApiClient
{
    public AdmiraltyTidalApiClient(ILogger<AdmiraltyTidalApiClient> logger, HttpClient httpClient) => (_httpClient, _logger) = (httpClient, logger);
        
    private readonly HttpClient _httpClient;
    private readonly ILogger<AdmiraltyTidalApiClient> _logger;

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

}