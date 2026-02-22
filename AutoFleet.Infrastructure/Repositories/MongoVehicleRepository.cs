using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Microsoft.Extensions.Configuration;
using AutoFleet.Core.Entities;
using AutoFleet.Core.Models;
using AutoFleet.Core.Interfaces;
using AutoFleet.Core.Enums;

namespace AutoFleet.Infrastructure.Repositories;

public class MongoVehicleRepository : IVehicleRepository
{
    // Using Enum as requested
    public RepositorySource Source => RepositorySource.MONGO;

    private readonly IMongoCollection<MongoVehicleDocument> _collection;

    public MongoVehicleRepository(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MongoDbConnection");
        var databaseName = configuration.GetConnectionString("MongoDbName") ?? "AutoFleetMongoDb";

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        _collection = database.GetCollection<MongoVehicleDocument>("Vehicles");
    }

    public async Task AddAsync(Vehicle vehicle)
    {
        var doc = new MongoVehicleDocument
        {
            Vin = vehicle.Vin,
            Brand = vehicle.Brand,
            Model = vehicle.Model,
            Year = vehicle.Year,
            Price = vehicle.Price,
            IsSold = vehicle.IsSold,
            PassengerCapacity = vehicle.PassengerCapacity,
            Status = vehicle.Status.ToString(), // Storing Enum as String for readability
            KmPerLiter = vehicle.KmPerLiter,
            StoredAt = DateTime.UtcNow
        };

        await _collection.InsertOneAsync(doc);
    }

    public async Task<IEnumerable<Vehicle>> GetAllAsync()
    {
        // Mapping: Mongo Document -> Core Entity
        var docs = await _collection.Find(_ => true).ToListAsync();

        return docs.Select(d => MapToEntity(d));
    }

    // Now we can implement this! No more NotImplementedException.
    public async Task<Vehicle?> GetByVinAsync(string vin)
    {
        var doc = await _collection.Find(v => v.Vin == vin).FirstOrDefaultAsync();
        if (doc == null) return null;

        return MapToEntity(doc);
    }

    public async Task<List<InventoryItem>> GetAvailableFleetSummaryAsync()
    {
        // Mongo LINQ Aggregation
        var query = _collection.AsQueryable()
            .Where(v => v.Status == "Available")
            .GroupBy(v => new { v.Brand, v.Model, v.Year, v.PassengerCapacity, v.KmPerLiter })
            .Select(g => new InventoryItem
            {
                VehicleName = g.Key.Brand + " " + g.Key.Model,
                Year = g.Key.Year,
                Capacity = g.Key.PassengerCapacity,
                KmPerLiter = g.Key.KmPerLiter,
                AvailableCount = g.Count()
            });

        return await query.ToListAsync();
    }

    // Helper method to avoid code repetition
    private static Vehicle MapToEntity(MongoVehicleDocument d)
    {
        return new Vehicle
        {
            // We don't map the Mongo ID to the SQL Int ID. ID stays 0 (default).
            // The VIN is the shared identity.
            Vin = d.Vin,
            Brand = d.Brand,
            Model = d.Model,
            Year = d.Year,
            Price = d.Price,
            IsSold = d.IsSold,
            PassengerCapacity = d.PassengerCapacity,
            KmPerLiter = d.KmPerLiter,
            Status = Enum.TryParse<VehicleStatus>(d.Status, out var status) ? status : VehicleStatus.Available
        };
    }

    // Internal Schema for Mongo
    private class MongoVehicleDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        public string Vin { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public decimal Price { get; set; }
        public bool IsSold { get; set; }
        public DateTime StoredAt { get; set; }
        public int PassengerCapacity { get; set; }
        public string Status { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.Decimal128)]
        public decimal KmPerLiter { get; set; }
    }
}
