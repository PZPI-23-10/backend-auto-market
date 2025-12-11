using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Net.Http;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleCheckController : ControllerBase
    {
        private const string ApiKey = "62fee967df633514fcee3765522226dc";
        private const string BaseUrlNomer = "https://baza-gai.com.ua/nomer/";
        private const string BaseUrlVin = "https://baza-gai.com.ua/vin/";

        private readonly IHttpClientFactory _httpClientFactory;

        public VehicleCheckController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // 1. Пошук по Номеру (старий метод)
        [HttpGet("plate/{licensePlate}")]
        public async Task<IActionResult> CheckByPlate(string licensePlate, [FromQuery] string lang = "ua")
        {
            var cleanPlate = licensePlate.Replace(" ", "").ToUpper();
            return await MakeRequest(BaseUrlNomer + cleanPlate, lang);
        }

        // 2. НОВИЙ: Пошук по VIN
        [HttpGet("vin/{vin}")]
        public async Task<IActionResult> CheckByVin(string vin, [FromQuery] string lang = "ua")
        {
            var cleanVin = vin.Replace(" ", "").ToUpper();
            // VIN має бути 17 символів
            if (cleanVin.Length != 17)
                return BadRequest(new { message = "VIN має складатись з 17 символів" });

            return await MakeRequest(BaseUrlVin + cleanVin, lang);
        }

        // Спільна логіка запиту та обробки
        private async Task<IActionResult> MakeRequest(string url, string lang)
        {
            var handler = new HttpClientHandler();
            // Ігноруємо SSL помилки (для локальної розробки)
            handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

            // ВАЖЛИВО: Не вказуємо протокол жорстко, нехай система вибере сама
            // handler.SslProtocols = ... (прибрали)

            using var client = new HttpClient(handler);
            client.Timeout = TimeSpan.FromSeconds(30);

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("X-Api-Key", ApiKey);
            request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");

            try
            {
                var response = await client.SendAsync(request);
                var jsonString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        return NotFound(new { message = "Авто не знайдено в базі" });

                    return StatusCode((int)response.StatusCode, new { message = "Помилка зовнішнього API", details = jsonString });
                }

                using var document = JsonDocument.Parse(jsonString);
                var root = document.RootElement;

                // Перевірка на помилки всередині JSON
                if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("error", out var errorElement))
                {
                    return BadRequest(new { message = "API Error", details = errorElement.ToString() });
                }

                // --- Парсинг даних ---
                JsonElement lastOperation = new JsonElement();
                bool hasOperations = root.TryGetProperty("operations", out var operations) && operations.GetArrayLength() > 0;

                if (hasOperations) lastOperation = operations[0];

                // Колір
                string colorValue = "Не вказано";
                if (hasOperations && lastOperation.TryGetProperty("color", out var c))
                {
                    if (lang == "en" && c.TryGetProperty("slug", out var slug)) colorValue = slug.GetString();
                    else if (c.TryGetProperty("ua", out var ua)) colorValue = ua.GetString();
                }

                // Операція
                string operationValue = "Інформація відсутня";
                if (hasOperations && lastOperation.TryGetProperty("operation", out var op))
                {
                    if (op.TryGetProperty("ua", out var opUa)) operationValue = opUa.GetString();
                }

                var result = new
                {
                
                    Brand = root.TryGetProperty("vendor", out var v) ? v.GetString() : "Unknown",
                    Model = root.TryGetProperty("model", out var m) ? m.GetString() : "Unknown",
                    Year = root.TryGetProperty("model_year", out var y) ? y.GetInt32() : 0,
                    PhotoUrl = root.TryGetProperty("photo_url", out var p) ? p.GetString() : null,

                    Digits = root.TryGetProperty("digits", out var d) ? d.GetString() : null, 
                    Vin = root.TryGetProperty("vin", out var vin) ? vin.GetString() : null,  

                    IsStolen = root.TryGetProperty("is_stolen", out var s) && s.GetBoolean(),
                    Color = colorValue,
                    EngineCapacity = hasOperations && lastOperation.TryGetProperty("displacement", out var disp) ? disp.GetInt32() : 0,
                    Fuel = "Gas/Petrol",
                    LastOperationDate = hasOperations && lastOperation.TryGetProperty("registered_at", out var dReg) ? dReg.GetString() : "",
                    LastOperationName = operationValue
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Помилка сервера", details = ex.Message });
            }
        }
    }
}