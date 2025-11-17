using Domain.Entities;

namespace Infrastructure.Persistence.Seed.Factories;

public static class VehicleTypeSeedFactory
{
    public static IEnumerable<VehicleType> CreateSeedData()
    {
        return
        [
            new VehicleType { Name = "PASSENGER_CAR" },
            new VehicleType { Name = "TRUCK" },
            new VehicleType { Name = "MOTORCYCLE" },
        ];
    }
}