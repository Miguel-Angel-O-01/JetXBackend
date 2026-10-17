using Supabase;
using JetX.Domain.Entities;
using JetX.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace JetX.Infrastructure.Repositories
{
    public class SupabaseRouteRepository : IRouteRepository
    {
        private readonly Client _supabaseClient;

        // Inyectamos la configuración general para leer las llaves de los User Secrets
        public SupabaseRouteRepository(IConfiguration configuration)
        {
            var url = configuration["SupabaseSettings:Url"]
                ?? throw new ArgumentNullException("La URL de Supabase no está configurada.");
            var anonKey = configuration["SupabaseSettings:AnonKey"]
                ?? throw new ArgumentNullException("La AnonKey de Supabase no está configurada.");

            var options = new SupabaseOptions { AutoConnectRealtime = true };
            _supabaseClient = new Client(url, anonKey, options);
        }

        public async Task<IEnumerable<Waypoint>> ObtenerTodosLosWaypointsAsync()
        {
            await _supabaseClient.InitializeAsync();

            // Descarga todos los fijos, aeropuertos y radioayudas mapeados en la nube
            var response = await _supabaseClient.From<Waypoint>().Get();
            return response.Models;
        }

        public async Task<IEnumerable<Airway>> ObtenerTodasLasAeroviasAsync()
        {
            await _supabaseClient.InitializeAsync();

            // Descarga las conexiones (aristas) del grafo del cielo
            var response = await _supabaseClient.From<Airway>().Get();
            return response.Models;
        }

        public async Task<Waypoint?> ObtenerWaypointPorCodigoAsync(string codigo)
        {
            await _supabaseClient.InitializeAsync();

            // Busca un aeropuerto o fijo específico por su identificador único 
            var response = await _supabaseClient.From<Waypoint>()
                .Where(x => x.Codigo == codigo)
                .Get();

            return response.Model;
        }
    }
}
