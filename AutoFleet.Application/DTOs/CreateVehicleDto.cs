using System.ComponentModel.DataAnnotations; // Para Validaciones básicas

namespace AutoFleet.Application.DTOs
{
    public class CreateVehicleDto
    {
        [Required(ErrorMessage = "El VIN es obligatorio")]
        [StringLength(17, MinimumLength = 17, ErrorMessage = "El VIN debe tener 17 caracteres")]
        public string Vin { get; set; }

        [Required]
        public string Brand { get; set; }
        
        [Required]
        public string Model { get; set; }
        
        [Range(1900, 2100)]
        public int Year { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
    }
}
