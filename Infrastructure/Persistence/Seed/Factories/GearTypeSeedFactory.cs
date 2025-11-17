using Domain.Entities;

namespace Infrastructure.Persistence.Seed.Factories;

public static class GearTypeSeedFactory
{
    public static IEnumerable<GearType> CreateSeedData()
    {
        return
        [
            new GearType { Name = "MANUAL" },
            new GearType { Name = "AUTOMATIC" },
            new GearType { Name = "ROBOTIC" },
        ];
    }
}