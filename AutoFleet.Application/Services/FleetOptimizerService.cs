using AutoFleet.Application.DTOs;
using AutoFleet.Application.Interfaces;

namespace AutoFleet.Application.Services
{
    public class FleetOptimizerService : IFleetOptimizerService
    {
        // Definimos nuestra "flota disponible" (Capacidad -> Nombre)
        // En un futuro, esto vendría de la Base de Datos (SQL/Mongo)
        private readonly Dictionary<int, string> _vehicleTypes = new()
        {
            { 50, "Autobús (50 pax)" },
            { 20, "Minibus (20 pax)" },
            { 10, "Van (10 pax)" },
            { 4,  "Sedán (4 pax)" }  // "La moneda de a 1 peso" para asegurar cambio
        };

        public FleetAllocationResultDto OptimizeAllocation(int targetPassengers)
        {
            var capacities = _vehicleTypes.Keys.OrderByDescending(x => x).ToList();
            
            // --- ALGORITMO DP (Igual que Coin Change) ---
            
            // dp[i] = mínimo número de vehículos para 'i' pasajeros
            int[] dp = new int[targetPassengers + 1];
            int[] vehicleUsed = new int[targetPassengers + 1]; // Para reconstruir la solución
            
            Array.Fill(dp, targetPassengers + 1); // Llenamos con infinito
            dp[0] = 0;

            for (int i = 1; i <= targetPassengers; i++)
            {
                foreach (var cap in capacities)
                {
                    if (i - cap >= 0)
                    {
                        if (dp[i - cap] + 1 < dp[i])
                        {
                            dp[i] = dp[i - cap] + 1;
                            vehicleUsed[i] = cap;
                        }
                    }
                }
            }

            // --- Construcción de Respuesta ---
            var result = new FleetAllocationResultDto();

            // Si dp[target] sigue siendo > target, significa que no hay solución exacta
            // (Por ejemplo, si pides 3 pax y solo tienes buses de 50)
            if (dp[targetPassengers] > targetPassengers)
            {
                result.IsPossible = false;
                // Aquí en la vida real, sugerirías "Sobredimensionar" (usar un sedán aunque sobre 1 lugar)
                // Pero por pureza algorítmica:
                return result; 
            }

            result.IsPossible = true;
            result.TotalVehiclesNeeded = dp[targetPassengers];

            // Reconstruir (Backtracking)
            int remaining = targetPassengers;
            while (remaining > 0)
            {
                int cap = vehicleUsed[remaining];
                string name = _vehicleTypes[cap];

                // Agregar a la lista detallada
                result.DetailedList.Add(name);

                // Agregar al resumen (diccionario)
                if (!result.VehicleBreakdown.ContainsKey(name))
                    result.VehicleBreakdown[name] = 0;
                
                result.VehicleBreakdown[name]++;

                remaining -= cap;
            }

            return result;
        }
    }
}
