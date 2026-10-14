namespace JetX.Domain.Entities
{
    public class Airway
    {
        public Guid Id { get; set; }
        public string CodigoAerovia { get; set; } = string.Empty; 
        public Guid WaypointOrigenId { get; set; }
        public Guid WaypointDestinoId { get; set; }
        public int AltitudMinimaPies { get; set; }
        public int AltitudMaximaPies { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Waypoint? WaypointOrigen { get; set; }
        public Waypoint? WaypointDestino { get; set; }
    }
}
