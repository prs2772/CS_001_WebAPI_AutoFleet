using AutoFleet.Application.DTOs;
using AutoFleet.Application.Interfaces;
using AutoFleet.Core.Interfaces;

namespace AutoFleet.Application.Services;
public class FleetOptimizerService : IFleetOptimizerService
{
    private readonly IVehicleRepository _repository; // Ahora necesitamos acceso a BD

    public FleetOptimizerService(IVehicleRepository repository)
    {
        _repository = repository;
    }

    public async Task<FleetAllocationResultDto> OptimizeAllocationAsync(int targetPassengers)
    {
        // 1. OBTENER DATOS REALES DE LA BD
        var availableFleet = await _repository.GetAvailableFleetSummaryAsync();

        // Ordenamos por capacidad descendente (Estrategia Greedy: llenar los grandes primero)
        var sortedFleet = availableFleet.OrderByDescending(x => x.Capacity).ToList();

        var result = new FleetAllocationResultDto();
        int remainingPassengers = targetPassengers;

        // 2. ALGORITMO DE ASIGNACIÓN (Con Stock Finito)
        foreach (var vehicleType in sortedFleet)
        {
            if (remainingPassengers <= 0) break;

            // ¿Cuántos de este tipo necesito teóricamente?
            int needed = remainingPassengers / vehicleType.Capacity;
            
            // Si la división no es exacta, a veces conviene tomar uno extra si es el último recurso,
            // pero por ahora llenamos al máximo posible.
            
            // ¿Cuántos tengo realmente?
            int take = Math.Min(needed, vehicleType.AvailableCount);

            if (take > 0)
            {
                result.VehicleBreakdown.Add(vehicleType.VehicleName, take);
                
                for(int i=0; i<take; i++) result.DetailedList.Add(vehicleType.VehicleName);

                remainingPassengers -= (take * vehicleType.Capacity);
                result.TotalVehiclesNeeded += take;
            }
        }

        // 3. MANEJO DE RESIDUOS (El "Resto")
        // Si todavía queda gente (ej. quedan 3 personas) y ya usamos los grandes,
        // buscamos el vehículo más pequeño disponible que pueda llevarlos.
        if (remainingPassengers > 0)
        {
            // Buscamos en la flota (incluso los que ya usamos, si queda stock)
            // uno que tenga capacidad >= remainingPassengers y tenga stock > 0
            
            // Nota: Para hacerlo perfecto, esto requiere actualizar el 'AvailableCount' localmente
            // mientras iteramos arriba. Asumamos lógica simple:
            
            var smallBus = sortedFleet
                .Where(v => v.Capacity >= remainingPassengers)
                .OrderBy(v => v.Capacity) // El más chico que sirva
                .FirstOrDefault(v => !result.VehicleBreakdown.ContainsKey(v.VehicleName) || 
                                     result.VehicleBreakdown[v.VehicleName] < v.AvailableCount);

            if (smallBus != null)
            {
                if (!result.VehicleBreakdown.ContainsKey(smallBus.VehicleName))
                    result.VehicleBreakdown[smallBus.VehicleName] = 0;
                
                result.VehicleBreakdown[smallBus.VehicleName]++;
                result.DetailedList.Add(smallBus.VehicleName);
                result.TotalVehiclesNeeded++;
                remainingPassengers = 0;
            }
        }

        result.IsPossible = remainingPassengers <= 0;
        
        if (!result.IsPossible)
        {
            // Caso de borde: No hay suficientes coches en toda la empresa
            result.DetailedList.Clear(); // O dejarla parcial
            // Agregar mensaje de error
        }

        return result;
    }
}
