using Domain.Entities;
using Infrastructure.Persistence.Seed.Factories.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Seed.Factories;

public static class CitySeedFactory
{
    public static async Task<IEnumerable<City>> CreateSeedData(
        List<UkrainianCityDto> cities,
        DbContext context,
        CancellationToken cancellationToken = default
    )
    {
        Dictionary<string, Region> regionsMap = await context.Set<Region>()
            .ToDictionaryAsync(r => r.Name, r => r, cancellationToken);

        var cityEntities = new List<City>();
        foreach (var cityDto in cities)
        {
            if (regionsMap.TryGetValue(cityDto.RegionName, out var region))
            {
                cityEntities.Add(new City
                {
                    Name = cityDto.CityName,
                    Region = region
                });
            }
        }

        return cityEntities
            .GroupBy(c => new { c.Name, c.Region.Id })
            .Select(g => g.First());
    }
}