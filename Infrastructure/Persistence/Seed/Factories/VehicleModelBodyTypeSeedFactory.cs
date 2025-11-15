using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Seed.Factories;

public static class VehicleModelBodyTypeSeedFactory
{
    public static async Task<IEnumerable<VehicleModelBodyType>> CreateSeedData(
        DbContext context,
        CancellationToken cancellationToken = default)
    {
        var bodyTypes = await context.Set<VehicleBodyType>()
            .ToDictionaryAsync(b => b.Name, b => b, cancellationToken);

        var bmw3 = await context.Set<VehicleModel>().FirstAsync(m => m.Name == "3 Series", cancellationToken);
        var bmwX5 = await context.Set<VehicleModel>().FirstAsync(m => m.Name == "X5", cancellationToken);
        var audiA4 = await context.Set<VehicleModel>().FirstAsync(m => m.Name == "A4", cancellationToken);
        var audiQ7 = await context.Set<VehicleModel>().FirstAsync(m => m.Name == "Q7", cancellationToken);
        var mercC = await context.Set<VehicleModel>().FirstAsync(m => m.Name == "C-Class", cancellationToken);
        var mercGLE = await context.Set<VehicleModel>().FirstAsync(m => m.Name == "GLE", cancellationToken);
        var mercE = await context.Set<VehicleModel>().FirstAsync(m => m.Name == "E-Class", cancellationToken);

        return new List<VehicleModelBodyType>
        {
            new() { VehicleModel = bmw3, BodyType = bodyTypes["SEDAN"] },
            new() { VehicleModel = bmw3, BodyType = bodyTypes["WAGON"] },

            new() { VehicleModel = bmwX5, BodyType = bodyTypes["SUV"] },

            new() { VehicleModel = audiA4, BodyType = bodyTypes["SEDAN"] },
            new() { VehicleModel = audiA4, BodyType = bodyTypes["WAGON"] },

            new() { VehicleModel = audiQ7, BodyType = bodyTypes["SUV"] },

            new() { VehicleModel = mercC, BodyType = bodyTypes["SEDAN"] },
            new() { VehicleModel = mercC, BodyType = bodyTypes["COUPE"] },

            new() { VehicleModel = mercE, BodyType = bodyTypes["SEDAN"] },

            new() { VehicleModel = mercGLE, BodyType = bodyTypes["SUV"] }
        };
    }
}