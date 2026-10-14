namespace JetX.Domain.Entities
{
    public class Waypoint
    {
        public Guid Id { get; set; }
        public string Codigo { get; set; } = string.Empty; 
        public string Nombre { get; set; } = string.Empty;
        public string Tipo { get; set; } = "Aeropuerto";  
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public double Latitud { get; set; }
        public double Longitud { get; set; }
    }
}
