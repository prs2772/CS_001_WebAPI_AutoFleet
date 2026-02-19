namespace AutoFleet.Application.DTOs
{
    public class VehicleDto
    {
        public string Brand { get; set; }
        public string Model { get; set; }
        public decimal Price { get; set; }
        // Ocultamos el ID interno o el VIN si es sensible
    }
}
