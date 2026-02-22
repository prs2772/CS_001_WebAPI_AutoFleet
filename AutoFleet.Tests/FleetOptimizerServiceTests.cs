using Xunit;
using Moq;
using AutoFleet.Application.Services;
using AutoFleet.Core.Interfaces;
using AutoFleet.Core.Models;
using AutoFleet.Core.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AutoFleet.Tests
{
    public class FleetOptimizerServiceTests
    {
        private readonly Mock<IVehicleRepository> _mockRepo;
        // private readonly Mock<ILogger<VehicleService>> _mockLogger; // Mock del logger
        private readonly FleetOptimizerService _service;

        public FleetOptimizerServiceTests()
        {
            _mockRepo = new Mock<IVehicleRepository>();
            // 2. Configurar comportamiento por defecto del Mock (Opcional pero recomendado)
            // Si tu servicio filtra por SourceName="SQL", necesitamos que el mock diga que es SQL
            _mockRepo.Setup(r => r.Source).Returns(RepositorySource.MICROSOFT_SQL);

            // 3. Crear la lista (IEnumerable) que contiene al Mock
            var reposList = new List<IVehicleRepository> { _mockRepo.Object };
            
            // 4. Inyectar la LISTA, no el objeto suelto
            _service = new FleetOptimizerService(reposList);
            // _mockLogger = new Mock<ILogger<VehicleService>>();

            // // Pasar una lista con el repo mockeado y el logger mockeado
            // var repos = new List<IVehicleRepository> { _mockRepo.Object };
        
            // _service = new VehicleService(repos, _mockLogger.Object); // <--- Inyectar el objeto simulado
        }

        [Fact]
        public async Task OptimizeAllocation_ShouldReturnCorrectVehicles_WhenExactMatchExists()
        {
            // --- ARRANGE ---
            int passengers = 60;

            // Usamos InventoryItem, tal como lo pide la interfaz actualizada del Core
            var fakeInventory = new List<InventoryItem>
            {
                new InventoryItem { VehicleName = "Bus Volvo", Capacity = 50, AvailableCount = 5 },
                new InventoryItem { VehicleName = "Van Mercedes", Capacity = 10, AvailableCount = 10 }
            };

            // Setup: Cuando llamen al repo, devuelve la lista falsa
            _mockRepo.Setup(repo => repo.GetAvailableFleetSummaryAsync())
                     .ReturnsAsync(fakeInventory);

            // --- ACT ---
            var result = await _service.GetSimpleAllocationAsync(passengers);

            // --- ASSERT ---
            Assert.True(result.IsPossible);
            Assert.Equal(2, result.TotalVehiclesNeeded); // 1 Bus + 1 Van
            Assert.Equal(1, result.VehicleBreakdown["Bus Volvo"]);
            Assert.Equal(1, result.VehicleBreakdown["Van Mercedes"]);
        }

        [Fact]
        public async Task OptimizeAllocation_ShouldFail_WhenNotEnoughFleet()
        {
            // --- ARRANGE ---
            int passengers = 1000;

            var fakeInventory = new List<InventoryItem>
            {
                new InventoryItem { VehicleName = "Sedan", Capacity = 4, AvailableCount = 2 }
            };

            _mockRepo.Setup(r => r.GetAvailableFleetSummaryAsync())
                     .ReturnsAsync(fakeInventory);

            // --- ACT ---
            var result = await _service.GetSimpleAllocationAsync(passengers);

            // --- ASSERT ---
            Assert.False(result.IsPossible);
        }
    }
}
