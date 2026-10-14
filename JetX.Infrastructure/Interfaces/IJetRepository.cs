using JetX.Domain.Entities;

namespace JetX.Application.Interfaces
{
    public interface IJetRepository
    {
        Task<IEnumerable<Jet>> ObtenerTodosLosJetsAsync();
        Task<Jet?> ObtenerJetPorIdAsync(Guid id);
        Task<bool> ActualizarPosicionJetAsync(Guid jetId, double latitud, double longitud);
    }
}
