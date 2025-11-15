using Domain.Entities;

namespace Infrastructure.Persistence.Seed.Factories;

public static class VehicleConditionsSeedFactory
{
    public static IEnumerable<VehicleCondition> CreateSeedData()
    {
        return
        [
            new VehicleCondition { Name = "UNDAMAGED" },
            new VehicleCondition { Name = "REPAIRED" },
            new VehicleCondition { Name = "UNREPAIRED" },
            new VehicleCondition { Name = "FOR_PARTS" }
        ];
    }
}