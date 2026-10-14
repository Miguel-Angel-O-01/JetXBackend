using JetX.Domain.Entities;

namespace JetX.Application.Interfaces
{
    public interface IRouteRepository
    {
        Task<IEnumerable<Waypoint>> ObtenerTodosLosWaypointsAsync();
        Task<IEnumerable<Airway>> ObtenerTodasLasAeroviasAsync();
        Task<Waypoint?> ObtenerWaypointPorCodigoAsync(string codigo);
    }
}
