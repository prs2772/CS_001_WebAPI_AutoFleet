using Xunit;
using Moq;
using AutoFleet.Application.Services;
using AutoFleet.Core.Interfaces;
using AutoFleet.Core.Models; // <--- La clave del éxito
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AutoFleet.Tests
{
    public class FleetOptimizerServiceTests
    {
        private readonly Mock<IVehicleRepository> _mockRepo;
        private readonly FleetOptimizerService _service;

        public FleetOptimizerServiceTests()
        {
            _mockRepo = new Mock<IVehicleRepository>();
            _service = new FleetOptimizerService(_mockRepo.Object);
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
            var result = await _service.OptimizeAllocationAsync(passengers);

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
            var result = await _service.OptimizeAllocationAsync(passengers);

            // --- ASSERT ---
            Assert.False(result.IsPossible);
        }
    }
}
