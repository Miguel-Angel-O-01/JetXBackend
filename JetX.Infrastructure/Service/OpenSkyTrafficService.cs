using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JetX.Infrastructure.Services;

public class LiveAircraftState
{
    public string Icao24 { get; set; } = string.Empty;
    public string Callsign { get; set; } = string.Empty;
    public double Longitud { get; set; }
    public double Latitud { get; set; }
    public double AltitudMetros { get; set; }
    public double RumboHeading { get; set; }
    public double VelocidadNudos { get; set; }
    public bool EnTierra { get; set; }
}

public class OpenSkyTrafficService
{
    private readonly HttpClient _httpClient;

    public OpenSkyTrafficService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.Timeout = TimeSpan.FromSeconds(8);
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "JetX-Aviation-Platform");
    }

    /// <summary>
    /// Consulta tráfico real ADS-B vía OpenSky en un Bounding Box (ej. México / Golfo / Centroamérica)
    /// lamin: Latitud mínima, lamax: Latitud máxima, lomin: Longitud mínima, lomax: Longitud máxima
    /// </summary>
    public async Task<List<LiveAircraftState>> ObtenerTraficoAereoAsync(
        double lamin = 12.0, double lamax = 28.0, double lomin = -105.0, double lomax = -85.0)
    {
        var lista = new List<LiveAircraftState>();
        var url = $"https://opensky-network.org/api/states/all?lamin={lamin}&lamax={lamax}&lomin={lomin}&lomax={lomax}";

        try
        {
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return lista;

            using var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
            if (!doc.RootElement.TryGetProperty("states", out var statesArray) || statesArray.ValueKind != JsonValueKind.Array)
                return lista;

            foreach (var state in statesArray.EnumerateArray())
            {
                // Vector OpenSky:
                // [0] icao24, [1] callsign, [5] longitud, [6] latitud, [7] baro_altitude, [8] on_ground, [9] velocity, [10] true_track
                if (state[5].ValueKind == JsonValueKind.Null || state[6].ValueKind == JsonValueKind.Null)
                    continue;

                var lon = state[5].GetDouble();
                var lat = state[6].GetDouble();
                var enTierra = !state[8].ValueKind.Equals(JsonValueKind.Null) && state[8].GetBoolean();

                if (enTierra) continue; // Solo nos interesan los que están en el aire

                lista.Add(new LiveAircraftState
                {
                    Icao24 = state[0].GetString() ?? "",
                    Callsign = state[1].GetString()?.Trim() ?? "UNKNOWN",
                    Longitud = lon,
                    Latitud = lat,
                    AltitudMetros = state[7].ValueKind == JsonValueKind.Number ? state[7].GetDouble() : 8000,
                    VelocidadNudos = state[9].ValueKind == JsonValueKind.Number ? state[9].GetDouble() * 1.94384 : 450,
                    RumboHeading = state[10].ValueKind == JsonValueKind.Number ? state[10].GetDouble() : 0,
                    EnTierra = enTierra
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[OpenSky Service] Aviso: {ex.Message}");
        }

        return lista;
    }
}