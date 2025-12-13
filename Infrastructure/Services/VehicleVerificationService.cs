using Application.DTOs.Vehicle;
using Application.Interfaces.Services;
using System.Net;
using System.Text.Json;

namespace Infrastructure.Services;

public class VehicleVerificationService(IHttpClientFactory httpClientFactory) : IVehicleVerificationService
{
    private const string ApiKey = "62fee967df633514fcee3765522226dc";
    private const string BaseUrl = "https://baza-gai.com.ua/";

    public async Task<VehicleCheckResultDto> CheckVehicleAsync(string identifier, string lang = "ua")
    {
        if (string.IsNullOrWhiteSpace(identifier))
            return new VehicleCheckResultDto { IsFound = false, ErrorMessage = "Empty identifier" };

        var cleanId = identifier.Replace(" ", "").ToUpper();

        // Визначаємо ендпоінт: якщо 17 символів - це VIN, інакше - номер
        string endpoint = cleanId.Length == 17 ? $"vin/{cleanId}" : $"nomer/{cleanId}";

        var client = httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(10);

        var request = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}{endpoint}");
        request.Headers.Add("Accept", "application/json");
        request.Headers.Add("X-Api-Key", ApiKey);
        // User-Agent обов'язковий для деяких API
        request.Headers.Add("User-Agent", "AutoMarketBackend/1.0");

        try
        {
            var response = await client.SendAsync(request);
            var jsonString = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return new VehicleCheckResultDto { IsFound = false, ErrorMessage = "Авто не знайдено в базі" };
            }

            if (!response.IsSuccessStatusCode)
            {
                return new VehicleCheckResultDto { IsFound = false, ErrorMessage = $"API Error: {response.StatusCode}" };
            }

            using var document = JsonDocument.Parse(jsonString);
            var root = document.RootElement;

            if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("error", out var errorElement))
            {
                return new VehicleCheckResultDto { IsFound = false, ErrorMessage = errorElement.ToString() };
            }

            // --- Парсинг успішної відповіді ---
            return ParseSuccessResponse(root, lang);
        }
        catch (Exception ex)
        {
            return new VehicleCheckResultDto { IsFound = false, ErrorMessage = ex.Message };
        }
    }

    private VehicleCheckResultDto ParseSuccessResponse(JsonElement root, string lang)
    {
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

        return new VehicleCheckResultDto
        {
            IsFound = true,
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
    }
}