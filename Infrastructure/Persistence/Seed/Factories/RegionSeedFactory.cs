using Domain.Entities;
using Infrastructure.Persistence.Seed.Factories.DTOs;

namespace Infrastructure.Persistence.Seed.Factories;

public static class RegionSeedFactory
{
    public static IEnumerable<Region> CreateSeedData(List<UkrainianCityDto> cities)
    {
        IEnumerable<string> regionNames = cities
            .Select(c => c.RegionName)
            .Distinct();

        return regionNames.Select(name => new Region { Name = name });
    }
}