using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace JetX.Infrastructure.Models;

[Table("airways")]
public class AirwayModel : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("codigo_aerovia")]
    public string CodigoAerovia { get; set; } = string.Empty;

    [Column("waypoint_origen_id")]
    public Guid WaypointOrigenId { get; set; }

    [Column("waypoint_destino_id")]
    public Guid WaypointDestinoId { get; set; }

    [Column("altitud_minima_pies")]
    public int AltitudMinimaPies { get; set; }

    [Column("altitud_maxima_pies")]
    public int AltitudMaximaPies { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}