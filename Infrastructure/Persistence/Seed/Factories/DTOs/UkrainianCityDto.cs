using System.Text.Json.Serialization;

namespace Infrastructure.Persistence.Seed.Factories.DTOs;

[Serializable]
public class UkrainianCityDto
{
    [JsonPropertyName("city")] public string CityName { get; set; }

    [JsonPropertyName("admin_name")] public string RegionName { get; set; }
}