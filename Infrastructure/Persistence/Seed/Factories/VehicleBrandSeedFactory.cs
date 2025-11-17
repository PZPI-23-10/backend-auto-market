using Domain.Entities;

namespace Infrastructure.Persistence.Seed.Factories;

public static class VehicleBrandSeedFactory
{
    public static IEnumerable<VehicleBrand> CreateSeedData()
    {
        return
        [
            new VehicleBrand { Name = "BMW" },
            new VehicleBrand { Name = "AUDI" },
            new VehicleBrand { Name = "TOYOTA" },
            new VehicleBrand { Name = "MERCEDES-BENZ" }
        ];
    }
}