using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleCheckController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public VehicleCheckController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("{licensePlate}")]
        public async Task<IActionResult> CheckCar(string licensePlate, [FromQuery] string lang = "ua")
        {
            var cleanPlate = licensePlate.Replace(" ", "").ToUpper();

            var client = _httpClientFactory.CreateClient("BazaGaiClient");

            try
            {
                var response = await client.GetAsync(cleanPlate);
                var jsonString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        return NotFound(new { message = "Авто не знайдено в базі" });

                    return StatusCode((int)response.StatusCode, new
                    {
                        message = "Помилка зовнішнього API",
                        statusCode = response.StatusCode,
                        externalResponse = jsonString
                    });
                }

                using var document = JsonDocument.Parse(jsonString);
                var root = document.RootElement;

                if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("error", out var errorElement))
                {
                    return BadRequest(new { message = "API Error", details = errorElement.ToString() });
                }

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
            catch (HttpRequestException ex)
            {
                return StatusCode(500, new
                {
                    message = "Помилка з'єднання з базою",
                    details = ex.Message
                });
            }
            catch (JsonException ex)
            {
                return StatusCode(500, new
                {
                    message = "Некорректный ответ от внешнего API (не JSON)",
                    details = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Внутрішня помилка сервера",
                    details = ex.Message
                });
            }
        }
    }
}