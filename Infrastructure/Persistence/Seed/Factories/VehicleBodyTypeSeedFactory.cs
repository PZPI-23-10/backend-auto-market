using Domain.Entities;

namespace Infrastructure.Persistence.Seed.Factories;

public static class VehicleBodyTypeSeedFactory
{
    public static IEnumerable<VehicleBodyType> CreateSeedData()
    {
        return
        [
            new VehicleBodyType { Name = "SEDAN" },
            new VehicleBodyType { Name = "HATCHBACK" },
            new VehicleBodyType { Name = "WAGON" },
            new VehicleBodyType { Name = "COUPE" },
            new VehicleBodyType { Name = "LIMOUSINE" },
            new VehicleBodyType { Name = "CONVERTIBLE" },
            new VehicleBodyType { Name = "CROSSOVER" },
            new VehicleBodyType { Name = "SUV" },
            new VehicleBodyType { Name = "PICKUP" },
            new VehicleBodyType { Name = "MINIVAN" },
        ];
    }
}