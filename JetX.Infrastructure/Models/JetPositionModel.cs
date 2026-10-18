using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace JetX.Infrastructure.Models;

[Table("jet_positions")]
public class JetPositionModel : BaseModel
{
    [PrimaryKey("jet_id", false)]
    public Guid JetId { get; set; }

    [Column("latitud")]
    public double Latitud { get; set; }

    [Column("longitud")]
    public double Longitud { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}