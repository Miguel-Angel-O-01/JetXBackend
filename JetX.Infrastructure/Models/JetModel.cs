using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace JetX.Infrastructure.Models;

[Table("jets")]
public class JetModel : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("matricula")]
    public string Matricula { get; set; } = string.Empty;

    [Column("modelo")]
    public string Modelo { get; set; } = string.Empty;

    [Column("capacidad_pasajeros")]
    public int CapacidadPasajeros { get; set; }

    [Column("costo_hora_usd")]
    public decimal CostoHoraUsd { get; set; }

    [Column("estado")]
    public string Estado { get; set; } = "Disponible";

    [Column("aeropuerto_base_id")]
    public Guid? AeropuertoBaseId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}