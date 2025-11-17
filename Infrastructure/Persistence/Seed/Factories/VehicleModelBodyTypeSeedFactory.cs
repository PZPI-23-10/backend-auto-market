using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Seed.Factories;

public static class VehicleModelBodyTypeSeedFactory
{
    public static async Task<IEnumerable<VehicleModelBodyType>> CreateSeedData(
        DbContext context,
        CancellationToken cancellationToken = default)
    {
        var series3 = await context.Set<VehicleModel>().FirstAsync(m => m.Name == "3 Series", cancellationToken);
        var x5 = await context.Set<VehicleModel>().FirstAsync(m => m.Name == "X5", cancellationToken);
        var r1250gs = await context.Set<VehicleModel>().FirstAsync(m => m.Name == "R 1250 GS", cancellationToken);
        var s1000rr = await context.Set<VehicleModel>().FirstAsync(m => m.Name == "S 1000 RR", cancellationToken);

        var cClass = await context.Set<VehicleModel>().FirstAsync(m => m.Name == "C-Class", cancellationToken);
        var gle = await context.Set<VehicleModel>().FirstAsync(m => m.Name == "GLE", cancellationToken);
        var actros = await context.Set<VehicleModel>().FirstAsync(m => m.Name == "Actros", cancellationToken);
        var sprinter = await context.Set<VehicleModel>().FirstAsync(m => m.Name == "Sprinter", cancellationToken);

        var a4 = await context.Set<VehicleModel>().FirstAsync(m => m.Name == "A4", cancellationToken);
        var a6 = await context.Set<VehicleModel>().FirstAsync(m => m.Name == "A6", cancellationToken);
        var q7 = await context.Set<VehicleModel>().FirstAsync(m => m.Name == "Q7", cancellationToken);
        var etronGT = await context.Set<VehicleModel>().FirstAsync(m => m.Name == "e-tron GT", cancellationToken);

        var camry = await context.Set<VehicleModel>().FirstAsync(m => m.Name == "Camry", cancellationToken);
        var rav4 = await context.Set<VehicleModel>().FirstAsync(m => m.Name == "RAV4", cancellationToken);
        var hilux = await context.Set<VehicleModel>().FirstAsync(m => m.Name == "Hilux", cancellationToken);
        var tundra = await context.Set<VehicleModel>().FirstAsync(m => m.Name == "Tundra", cancellationToken);

        var sedan = await context.Set<VehicleBodyType>().FirstAsync(b => b.Name == "SEDAN", cancellationToken);
        var coupe = await context.Set<VehicleBodyType>().FirstAsync(b => b.Name == "COUPE", cancellationToken);
        var wagon = await context.Set<VehicleBodyType>().FirstAsync(b => b.Name == "WAGON", cancellationToken);
        var liftback = await context.Set<VehicleBodyType>().FirstAsync(b => b.Name == "LIFTBACK", cancellationToken);

        var suv = await context.Set<VehicleBodyType>().FirstAsync(b => b.Name == "SUV", cancellationToken);
        var crossover = await context.Set<VehicleBodyType>().FirstAsync(b => b.Name == "CROSSOVER", cancellationToken);

        var pickup = await context.Set<VehicleBodyType>().FirstAsync(b => b.Name == "PICKUP", cancellationToken);
        var van = await context.Set<VehicleBodyType>().FirstAsync(b => b.Name == "VAN", cancellationToken);
        var minibus = await context.Set<VehicleBodyType>().FirstAsync(b => b.Name == "MINIBUS", cancellationToken);

        var truckCab = await context.Set<VehicleBodyType>().FirstAsync(b => b.Name == "TRUCK_CAB", cancellationToken);

        var motorcycle =
            await context.Set<VehicleBodyType>().FirstAsync(b => b.Name == "MOTORCYCLE", cancellationToken);

        return new List<VehicleModelBodyType>
        {
            new() { VehicleModel = series3, BodyType = sedan },
            new() { VehicleModel = series3, BodyType = wagon },
            new() { VehicleModel = series3, BodyType = coupe },
            new() { VehicleModel = x5, BodyType = suv },
            new() { VehicleModel = r1250gs, BodyType = motorcycle },
            new() { VehicleModel = s1000rr, BodyType = motorcycle },

            new() { VehicleModel = cClass, BodyType = sedan },
            new() { VehicleModel = cClass, BodyType = wagon },
            new() { VehicleModel = cClass, BodyType = coupe },
            new() { VehicleModel = gle, BodyType = suv },
            new() { VehicleModel = actros, BodyType = truckCab },
            new() { VehicleModel = sprinter, BodyType = van },
            new() { VehicleModel = sprinter, BodyType = minibus },

            new() { VehicleModel = a4, BodyType = sedan },
            new() { VehicleModel = a4, BodyType = wagon },
            new() { VehicleModel = a6, BodyType = sedan },
            new() { VehicleModel = a6, BodyType = wagon },
            new() { VehicleModel = q7, BodyType = suv },
            new() { VehicleModel = etronGT, BodyType = liftback },

            new() { VehicleModel = camry, BodyType = sedan },
            new() { VehicleModel = rav4, BodyType = crossover },
            new() { VehicleModel = hilux, BodyType = pickup },
            new() { VehicleModel = tundra, BodyType = pickup }
        };
    }
}