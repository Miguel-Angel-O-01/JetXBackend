using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.Text.Json;

namespace JetX.Infrastructure.Models;

[Table("waypoints")]
public class WaypointModel : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("codigo")]
    public string Codigo { get; set; } = string.Empty;

    [Column("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Column("coordenadas")]
    public object? Coordenadas { get; set; }

    [Column("tipo")]
    public string Tipo { get; set; } = "Aeropuerto";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    public (double Lat, double Lon) ObtenerLatLon()
    {
        if (Coordenadas == null) return (0.0, 0.0);
        try
        {
            if (Coordenadas is JsonElement elem && elem.ValueKind == JsonValueKind.Object && elem.TryGetProperty("coordinates", out var coords))
            {
                return (coords[1].GetDouble(), coords[0].GetDouble());
            }
        }
        catch { }
        return (0.0, 0.0);
    }
}