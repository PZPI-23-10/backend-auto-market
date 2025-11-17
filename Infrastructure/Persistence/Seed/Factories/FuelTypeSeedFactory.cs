using Domain.Entities;

namespace Infrastructure.Persistence.Seed.Factories;

public static class FuelTypeSeedFactory
{
    public static IEnumerable<FuelType> CreateSeedData()
    {
        return
        [
            new FuelType { Name = "BENSIN" },
            new FuelType { Name = "DIESEL" },
            new FuelType { Name = "GAS" },
            new FuelType { Name = "ELECTRIC" },
        ];
    }
}