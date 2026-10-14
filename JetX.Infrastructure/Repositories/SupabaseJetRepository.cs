using Supabase;
using JetX.Domain.Entities;
using JetX.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace JetX.Infrastructure.Repositories
{
    public class SupabaseJetRepository : IJetRepository
    {
        private readonly Client _supabaseClient;

        // El constructor recibe la configuración general (User Secrets / appsettings)
        public SupabaseJetRepository(IConfiguration configuration)
        {
            var url = configuration["SupabaseSettings:Url"]
                ?? throw new ArgumentNullException("La URL de Supabase no está configurada.");
            var anonKey = configuration["SupabaseSettings:AnonKey"]
                ?? throw new ArgumentNullException("La AnonKey de Supabase no está configurada.");

            // Inicializamos el cliente oficial de la nube
            var options = new SupabaseOptions { AutoConnectRealtime = true };
            _supabaseClient = new Client(url, anonKey, options);
        }

        public async Task<IEnumerable<Jet>> ObtenerTodosLosJetsAsync()
        {
            // Inicializa la conexión si no se ha hecho
            await _supabaseClient.InitializeAsync();

            // Hace un mapeo automático a tu entidad de dominio desde la tabla public.jets
            var response = await _supabaseClient.From<Jet>().Get();
            return response.Models;
        }

        public async Task<Jet?> ObtenerJetPorIdAsync(Guid id)
        {
            await _supabaseClient.InitializeAsync();

            var response = await _supabaseClient.From<Jet>()
                .Where(x => x.Id == id)
                .Get();

            return response.Model;
        }

        public async Task<bool> ActualizarPosicionJetAsync(Guid jetId, double latitud, double longitud)
        {
            await _supabaseClient.InitializeAsync();

            // Preparamos los datos volátiles de telemetría para la simulación en tiempo real
            var updateData = new Dictionary<string, object>
            {
                { "latitud", latitud },
                { "longitud", longitud },
                { "updated_at", DateTime.UtcNow }
            };

            // Actualizamos la tabla optimizada jet_positions que habilitamos con replicación
            var response = await _supabaseClient.From<Jet>()
                .Table("jet_positions")
                .Where(x => x.Id == jetId)
                .Update(updateData);

            return response.ResponseMessage.IsSuccessStatusCode;
        }
    }
}
