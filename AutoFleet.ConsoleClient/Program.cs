using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http; // Requires Microsoft.Extensions.Http.Polly
using System.Net.Http.Json;  // Requires System.Net.Http.Json
using System.Net.Http.Headers;
using Newtonsoft.Json;       // Requires Newtonsoft.Json

class Program
{
    static async Task Main(string[] args)
    {
        Console.Title = "AutoFleet Load Simulator";
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("=============================================");
        Console.WriteLine("   AUTOFLEET - FLEET LOADER & SIMULATOR 🚀   ");
        Console.WriteLine("=============================================");
        Console.ResetColor();

        // 1. Configure Polly (Retry Policy)
        // Strategy: "Exponential Backoff". If network fails or 5XX/408 error occurs, 
        // wait 2s, then 4s, then 8s.
        var retryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (outcome, timespan, retryCount, context) =>
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"⚠️ Network glitch detected. Retrying attempt #{retryCount} in {timespan.TotalSeconds}s...");
                    Console.ResetColor();
                });

        // 2. Configure DI and HttpClient
        var serviceCollection = new ServiceCollection();
        
        serviceCollection.AddHttpClient("AutoFleetApi", client =>
        {
            client.BaseAddress = new Uri("http://localhost:5216"); 
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        })
        .AddPolicyHandler(retryPolicy); // Hooking up Polly

        var services = serviceCollection.BuildServiceProvider();
        var httpClientFactory = services.GetRequiredService<IHttpClientFactory>();
        var client = httpClientFactory.CreateClient("AutoFleetApi");

        // --- START PROCESS ---

        // A. Authentication (Get Token)
        Console.WriteLine("\n🔑 Authenticating as Admin...");
        
        // Note: Make sure this user exists in your DB (via Postman or previous migration seed)
        var loginData = new { Username = "Admin", Password = "Password123!" }; 
        
        try 
        {
            var loginResponse = await client.PostAsJsonAsync("/api/auth/login", loginData);

            if (!loginResponse.IsSuccessStatusCode)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ Login Failed. Status: {loginResponse.StatusCode}");
                Console.WriteLine("   (Did you create the user via /api/auth/register first?)");
                Console.ResetColor();
                return;
            }

            var loginContent = await loginResponse.Content.ReadAsStringAsync();
            
            // Flexible Deserialization (Handles "token" or "Token")
            dynamic tokenObj = JsonConvert.DeserializeObject(loginContent);
            string token = tokenObj.token ?? tokenObj.Token;

            if (string.IsNullOrEmpty(token))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("❌ Error: Token is null or empty.");
                return;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✅ Login Successful! Token acquired.");
            Console.ResetColor();

            // B. Set Header for subsequent requests
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ Connection Error: Is the API running? {ex.Message}");
            return;
        }

        // C. Define Vehicle Batch
        Console.WriteLine("\n📦 Preparing Vehicle Batch...");
        
        var vehiclesToUpload = new[]
        {
            // Valid Data
            new { Vin = "TSLA-Y-2024-001", Brand = "Tesla", Model = "Model Y", Year = 2024, Price = 55000m, Status = 1, PassengerCapacity = 4, KmPerLiter = 15.0m },
            // Valid Data (High Efficiency)
            new { Vin = "TSLA-3-2025-002", Brand = "Tesla", Model = "Model 3", Year = 2025, Price = 48000m, Status = 1, PassengerCapacity = 4, KmPerLiter = 16.5m },
            // Valid Data (Van)
            new { Vin = "FORD-TR-2024-03", Brand = "Ford", Model = "Transit", Year = 2024, Price = 45000m, Status = 1, PassengerCapacity = 15, KmPerLiter = 9.0m },
            // Valid Data (Hybrid)
            new { Vin = "TYT-PRIUS-24-04", Brand = "Toyota", Model = "Prius", Year = 2024, Price = 30000m, Status = 1, PassengerCapacity = 5, KmPerLiter = 22.0m },
            // Edge Case: Future Car (Changed 2077 to 2055 to pass validation Range[1900-2059])
            new { Vin = "CYBER-TRK-2055", Brand = "Tesla", Model = "CyberTruck", Year = 2055, Price = 99000m, Status = 1, PassengerCapacity = 6, KmPerLiter = 12.0m },
        };

        // D. Send Loop with Resilience
        Console.WriteLine($"🚀 Starting upload of {vehiclesToUpload.Length} vehicles...\n");

        foreach (var vehicle in vehiclesToUpload)
        {
            try
            {
                Console.Write($"   📤 Uploading {vehicle.Brand} {vehicle.Model} ({vehicle.Vin})... ");
                
                // Polly wraps this call automatically
                var response = await client.PostAsJsonAsync("/api/vehicles", vehicle);
                
                if (response.IsSuccessStatusCode)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("OK");
                }
                else
                {
                    var errorDetails = await response.Content.ReadAsStringAsync();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"FAILED");
                    Console.WriteLine($"      Status: {response.StatusCode}");
                    Console.WriteLine($"      Details: {errorDetails}");
                }
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n💀 Fatal Request Error: {ex.Message}");
                Console.ResetColor();
            }
            
            // Small delay to visualize the process
            await Task.Delay(500);
        }

        Console.WriteLine("\n=============================================");
        Console.WriteLine("🏁 Process Finished.");
        Console.WriteLine("=============================================");
        Console.ReadKey();
    }
}
