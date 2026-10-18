using Supabase;
using JetX.Domain.Entities;
using JetX.Application.Interfaces;
using JetX.Infrastructure.Models;
using Microsoft.Extensions.Configuration;

namespace JetX.Infrastructure.Repositories
{
    public class SupabaseRouteRepository : IRouteRepository
    {
        private readonly Client _supabaseClient;

        public SupabaseRouteRepository(IConfiguration configuration)
        {
            var url = configuration["SupabaseSettings:Url"]
                ?? throw new ArgumentNullException(nameof(configuration), "La URL de Supabase no está configurada.");
            var anonKey = configuration["SupabaseSettings:AnonKey"]
                ?? throw new ArgumentNullException(nameof(configuration), "La AnonKey de Supabase no está configurada.");

            var options = new SupabaseOptions { AutoConnectRealtime = false };
            _supabaseClient = new Client(url, anonKey, options);
        }

        public async Task<IEnumerable<Waypoint>> ObtenerTodosLosWaypointsAsync()
        {
            await _supabaseClient.InitializeAsync();
            var response = await _supabaseClient.From<WaypointModel>().Get();
            return response.Models.Select(MapToDomain);
        }

        public async Task<IEnumerable<Airway>> ObtenerTodasLasAeroviasAsync()
        {
            await _supabaseClient.InitializeAsync();
            var response = await _supabaseClient.From<AirwayModel>().Get();
            return response.Models.Select(MapToDomain);
        }

        public async Task<Waypoint?> ObtenerWaypointPorCodigoAsync(string codigo)
        {
            await _supabaseClient.InitializeAsync();
            var response = await _supabaseClient.From<WaypointModel>()
                .Where(x => x.Codigo == codigo)
                .Single();

            return response != null ? MapToDomain(response) : null;
        }

        private static Waypoint MapToDomain(WaypointModel model)
        {
            var (lat, lon) = model.ObtenerLatLon();
            return new Waypoint
            {
                Id = model.Id,
                Codigo = model.Codigo,
                Nombre = model.Nombre,
                Tipo = model.Tipo,
                Latitud = lat,
                Longitud = lon,
                CreatedAt = model.CreatedAt
            };
        }

        private static Airway MapToDomain(AirwayModel model)
        {
            return new Airway
            {
                Id = model.Id,
                CodigoAerovia = model.CodigoAerovia,
                WaypointOrigenId = model.WaypointOrigenId,
                WaypointDestinoId = model.WaypointDestinoId,
                AltitudMinimaPies = model.AltitudMinimaPies,
                AltitudMaximaPies = model.AltitudMaximaPies,
                CreatedAt = model.CreatedAt
            };
        }
    }
}