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
            new VehicleBodyType { Name = "LIFTBACK" },
            new VehicleBodyType { Name = "COUPE" },
            new VehicleBodyType { Name = "CONVERTIBLE" },
            new VehicleBodyType { Name = "ROADSTER" },
            new VehicleBodyType { Name = "LIMOUSINE" },

            new VehicleBodyType { Name = "SUV" },
            new VehicleBodyType { Name = "CROSSOVER" },

            new VehicleBodyType { Name = "PICKUP" },
            new VehicleBodyType { Name = "MINIVAN" },
            new VehicleBodyType { Name = "VAN" },
            new VehicleBodyType { Name = "MINIBUS" },

            new VehicleBodyType { Name = "TRUCK_CAB" },
            new VehicleBodyType { Name = "CHASSIS_CAB" }, 
            new VehicleBodyType { Name = "FLATBED" }, 
            new VehicleBodyType { Name = "DUMP_TRUCK" }, 

            new VehicleBodyType { Name = "MOTORCYCLE" },
            new VehicleBodyType { Name = "SCOOTER" },
            new VehicleBodyType { Name = "ATV" }
        ];
    }
}