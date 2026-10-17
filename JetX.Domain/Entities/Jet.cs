namespace JetX.Domain.Entities
{
    public class Jet
    {
        public Guid Id { get; set; }
        public string Matricula { get; set; } = string.Empty; // Ejemplo: N123UA
        public string Modelo { get; set; } = string.Empty;    // Ejemplo: Gulfstream G650
        public int CapacidadPasajeros { get; set; }
        public decimal CostoHoraUsd { get; set; }
        public string Estado { get; set; } = "Disponible";    // Disponible, EnVuelo, Mantenimiento
        public Guid? AeropuertoBaseId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
