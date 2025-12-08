using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleCheckController : ControllerBase
    {
        private const string ApiKey = "62fee967df633514fcee3765522226dc";
        private const string BaseUrl = "https://baza-gai.com.ua/nomer/";

        private readonly IHttpClientFactory _httpClientFactory;

        public VehicleCheckController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("{licensePlate}")]
        public async Task<IActionResult> CheckCar(string licensePlate, [FromQuery] string lang = "ua")
        {
            var cleanPlate = licensePlate.Replace(" ", "").ToUpper();

            var request = new HttpRequestMessage(HttpMethod.Get, BaseUrl + cleanPlate);
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("X-Api-Key", ApiKey);

            var client = _httpClientFactory.CreateClient();
            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return NotFound(new { message = "Авто не знайдено в базі МВС" });

                return StatusCode((int)response.StatusCode, new { message = "Помилка зовнішнього API" });
            }

            var jsonString = await response.Content.ReadAsStringAsync();

            using var document = JsonDocument.Parse(jsonString);
            var root = document.RootElement;

            JsonElement lastOperation = new JsonElement();
            bool hasOperations = root.TryGetProperty("operations", out var operations) && operations.GetArrayLength() > 0;

            if (hasOperations)
            {
                lastOperation = operations[0];
            }

            string colorValue = "Не вказано";
            if (hasOperations && lastOperation.TryGetProperty("color", out var c))
            {
                if (lang == "en" && c.TryGetProperty("slug", out var slug)) colorValue = slug.GetString();
                else if (c.TryGetProperty("ua", out var ua)) colorValue = ua.GetString();
            }

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
                IsStolen = root.TryGetProperty("is_stolen", out var s) && s.GetBoolean(),

                Color = colorValue,

                EngineCapacity = hasOperations && lastOperation.TryGetProperty("displacement", out var d) ? d.GetInt32() : 0,

                Fuel = "Gas/Petrol", 

                LastOperationDate = hasOperations && lastOperation.TryGetProperty("registered_at", out var dReg) ? dReg.GetString() : "",
                LastOperationName = operationValue
            };

            return Ok(result);
        }
    }
}