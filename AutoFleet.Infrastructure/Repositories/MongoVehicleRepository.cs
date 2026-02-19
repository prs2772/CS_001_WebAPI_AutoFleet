using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using AutoFleet.Core.Entities;
using AutoFleet.Core.Interfaces;

namespace AutoFleet.Infrastructure.Repositories
{
    public class MongoVehicleRepository : IVehicleRepository
    {
        private readonly IMongoCollection<MongoVehicleDocument> _collection;

        // Constructor: Recibimos la cadena de conexión directamente o vía configuración
        public MongoVehicleRepository()
        {
            // Para simplificar este ejemplo hardcodeamos la conexión local de Docker
            // En PROD, esto vendría de IConfiguration, igual que SQL
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("AutoFleetMongoDb");
            _collection = database.GetCollection<MongoVehicleDocument>("Vehicles");
        }

        public async Task AddAsync(Vehicle vehicle)
        {
            // Mapeamos de la Entidad de Dominio (Core) -> Documento Mongo (Infra)
            var doc = new MongoVehicleDocument
            {
                Vin = vehicle.Vin,
                Brand = vehicle.Brand,
                Model = vehicle.Model,
                Year = vehicle.Year,
                Price = vehicle.Price,
                IsSold = vehicle.IsSold,
                StoredAt = DateTime.UtcNow
            };

            await _collection.InsertOneAsync(doc);
        }

        // Para este ejemplo, solo implementamos Add. 
        // En un escenario real (CQRS), leeríamos de SQL o Mongo según convenga.
        public async Task<IEnumerable<Vehicle>> GetAllAsync()
        {
            // Retornamos lista vacía para no duplicar datos en la vista por ahora
            return await Task.FromResult(new List<Vehicle>());
        }

        public Task<Vehicle?> GetByIdAsync(int id)
        {
            throw new NotImplementedException("Por ahora solo escribimos en Mongo");
        }

        // --- Clase interna para representar el documento en Mongo ---
        private class MongoVehicleDocument
        {
            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)]
            public string Id { get; set; } // Mongo necesita su propio ID

            public string Vin { get; set; }
            public string Brand { get; set; }
            public string Model { get; set; }
            public int Year { get; set; }
            public decimal Price { get; set; }
            public bool IsSold { get; set; }
            public DateTime StoredAt { get; set; }
        }
    }
}
