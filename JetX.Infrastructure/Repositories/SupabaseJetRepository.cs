using Microsoft.Extensions.Configuration;
using Supabase;
using JetX.Domain.Entities;
using JetX.Application.Interfaces;
using JetX.Infrastructure.Models;

namespace JetX.Infrastructure.Repositories
{
    public class SupabaseJetRepository : IJetRepository
    {
        private readonly Client _supabaseClient;

        public SupabaseJetRepository(IConfiguration configuration)
        {
            var url = configuration["SupabaseSettings:Url"]
                ?? throw new ArgumentNullException(nameof(configuration), "La URL de Supabase no está configurada.");
            var anonKey = configuration["SupabaseSettings:AnonKey"]
                ?? throw new ArgumentNullException(nameof(configuration), "La AnonKey de Supabase no está configurada.");

            var options = new SupabaseOptions { AutoConnectRealtime = false };
            _supabaseClient = new Client(url, anonKey, options);
        }

        public async Task<IEnumerable<Jet>> ObtenerTodosLosJetsAsync()
        {
            await _supabaseClient.InitializeAsync();
            var response = await _supabaseClient.From<JetModel>().Get();
            return response.Models.Select(MapToDomain);
        }

        public async Task<Jet?> ObtenerJetPorIdAsync(Guid id)
        {
            await _supabaseClient.InitializeAsync();
            var response = await _supabaseClient.From<JetModel>()
                .Where(x => x.Id == id)
                .Single();

            return response != null ? MapToDomain(response) : null;
        }

        public async Task<bool> ActualizarPosicionJetAsync(Guid jetId, double latitud, double longitud)
        {
            await _supabaseClient.InitializeAsync();

            var posicion = new JetPositionModel
            {
                JetId = jetId,
                Latitud = latitud,
                Longitud = longitud,
                UpdatedAt = DateTime.UtcNow
            };

            var response = await _supabaseClient.From<JetPositionModel>().Upsert(posicion);
            return response.ResponseMessage != null && response.ResponseMessage.IsSuccessStatusCode;
        }

        private static Jet MapToDomain(JetModel model)
        {
            return new Jet
            {
                Id = model.Id,
                Matricula = model.Matricula,
                Modelo = model.Modelo,
                CapacidadPasajeros = model.CapacidadPasajeros,
                CostoHoraUsd = model.CostoHoraUsd,
                Estado = model.Estado,
                AeropuertoBaseId = model.AeropuertoBaseId
            };
        }
    }
}