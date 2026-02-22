using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Newtonsoft.Json;

// 1. Configurar Polly (La política de reintentos)
// "Si falla por error de red o 5XX o 408, espera y reintenta 3 veces exponencialmente"
var retryPolicy = HttpPolicyExtensions
    .HandleTransientHttpError() 
    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
        (outcome, timespan, retryCount, context) =>
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"⚠️ Fallo detectado. Reintentando por vez #{retryCount} en {timespan.TotalSeconds}s...");
            Console.ResetColor();
        });

// 2. Configurar HttpClient con DI
var serviceCollection = new ServiceCollection();
serviceCollection.AddHttpClient("AutoFleetApi", client =>
{
    client.BaseAddress = new Uri("http://localhost:5216"); // <--- CAMBIA ESTO POR TU PUERTO REAL
})
.AddPolicyHandler(retryPolicy); // <--- Aquí conectamos Polly

var services = serviceCollection.BuildServiceProvider();
var httpClientFactory = services.GetRequiredService<IHttpClientFactory>();
var client = httpClientFactory.CreateClient("AutoFleetApi");

// --- INICIO DEL FLUJO ---

Console.WriteLine("🚀 Iniciando simulador de carga de vehículos...");

// A. Autenticación (Obtener Token)
Console.WriteLine("🔑 Autenticando como Admin...");
var loginData = new { Username = "admin", Password = "admin123" };
var loginResponse = await client.PostAsJsonAsync("/api/auth/login", loginData);

if (!loginResponse.IsSuccessStatusCode)
{
    Console.WriteLine("❌ Error de login. Terminando.");
    return;
}

// ... código anterior ...
var loginContent = await loginResponse.Content.ReadAsStringAsync();

// DEBUG: Ver qué llegó realmente
Console.WriteLine($"Respuesta cruda: {loginContent}");

dynamic tokenObj = JsonConvert.DeserializeObject(loginContent);

// CORRECCIÓN: Intenta acceder con minúscula 'token' si la API lo serializó así
string token = tokenObj.token ?? tokenObj.Token; // Busca ambos por si acaso

if (string.IsNullOrEmpty(token))
{
    Console.WriteLine("❌ ERROR: El token llegó vacío o nulo.");
    return;
}

Console.WriteLine($"✅ Token recibido: {token.Substring(0, 10)}..."); // Imprime solo el inicio
// ... resto del código ...

Console.WriteLine("✅ Login exitoso. Token recibido.");

// B. Configurar Token en Headers
client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

// C. Generar Lote de Vehículos (JSON Simulado)
var vehiclesToUpload = new[]
{
    // VINs ficticios de 17 caracteres
    new { Vin = "SIMTESLAMODEL3001", Brand = "Tesla", Model = "Model 3", Year = 2025, Price = 45000, Status = 1, PassengerCapacity = 5 },
    new { Vin = "SIMTESLAMODELY002", Brand = "Tesla", Model = "Model Y", Year = 2025, Price = 55000, Status = 1, PassengerCapacity = 7 },
    new { Vin = "SIMFORDMACHE00003", Brand = "Ford", Model = "Mach-E", Year = 2024, Price = 40000, Status = 1, PassengerCapacity = 5 }
};

// D. Enviar con Resiliencia
foreach (var vehicle in vehiclesToUpload)
{
    try
    {
        Console.WriteLine($"📤 Enviando {vehicle.Brand} {vehicle.Model}...");
        
        // Polly está "envolviendo" esta llamada. Si la API estuviera apagada, reintentaría.
        var response = await client.PostAsJsonAsync("/api/vehicles", vehicle);
        
        if (response.IsSuccessStatusCode)
             Console.WriteLine($"✅ {vehicle.Model} guardado correctamente.");
        else
             Console.WriteLine($"❌ Error al guardar {vehicle.Model}: {response.StatusCode}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"💀 Error fatal: {ex.Message}");
    }
}

Console.WriteLine("🏁 Proceso finalizado.");
