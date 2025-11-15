using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Seed.Factories;

public static class VehicleBrandSeedFactory
{
    public static IEnumerable<VehicleBrand> CreateSeedData()
    {
        return
        [
            new VehicleBrand { Name = "BMW" },
            new VehicleBrand { Name = "Mercedes-Benz" },
            new VehicleBrand { Name = "Audi" }
        ];
    }
}

public static class VehicleModelSeedFactory
{
    public static async Task<IEnumerable<VehicleModel>> CreateSeedData(DbContext context,
        CancellationToken cancellationToken = default)
    {
        var bmw = await context.Set<VehicleBrand>()
            .FirstAsync(b => b.Name == "BMW", cancellationToken);

        var mercedes = await context.Set<VehicleBrand>()
            .FirstAsync(b => b.Name == "Mercedes-Benz", cancellationToken);

        var audi = await context.Set<VehicleBrand>()
            .FirstAsync(b => b.Name == "Audi", cancellationToken);

        return new List<VehicleModel>
        {
            new() { Name = "3 Series", Brand = bmw },
            new() { Name = "5 Series", Brand = bmw },
            new() { Name = "X3", Brand = bmw },
            new() { Name = "X5", Brand = bmw },
            new() { Name = "i7", Brand = bmw },

            new() { Name = "C-Class", Brand = mercedes },
            new() { Name = "E-Class", Brand = mercedes },
            new() { Name = "S-Class", Brand = mercedes },
            new() { Name = "GLC", Brand = mercedes },
            new() { Name = "GLE", Brand = mercedes },

            new() { Name = "A4", Brand = audi },
            new() { Name = "A6", Brand = audi },
            new() { Name = "Q5", Brand = audi },
            new() { Name = "Q7", Brand = audi },
            new() { Name = "e-tron GT", Brand = audi }
        };
    }
}