using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Seed.Factories;

public static class VehicleModelSeedFactory
{
    public static async Task<IEnumerable<VehicleModel>> CreateSeedData(DbContext context,
        CancellationToken cancellationToken = default)
    {
        var bmw = await context.Set<VehicleBrand>()
            .FirstAsync(b => b.Name == "BMW", cancellationToken);
        
        var audi = await context.Set<VehicleBrand>()
            .FirstAsync(b => b.Name == "AUDI", cancellationToken);
        
        var mercedes = await context.Set<VehicleBrand>()
            .FirstAsync(b => b.Name == "MERCEDES-BENZ", cancellationToken); 
        
        var toyota = await context.Set<VehicleBrand>()
            .FirstAsync(b => b.Name == "TOYOTA", cancellationToken);

        var carType = await context.Set<VehicleType>()
            .FirstAsync(t => t.Name == "PASSENGER_CAR", cancellationToken);
        
        var truckType = await context.Set<VehicleType>()
            .FirstAsync(t => t.Name == "TRUCK", cancellationToken);
            
        var motoType = await context.Set<VehicleType>()
            .FirstAsync(t => t.Name == "MOTORCYCLE", cancellationToken);

        return new List<VehicleModel>
        {
            new() { Name = "3 Series", Brand = bmw, VehicleType = carType },
            new() { Name = "X5", Brand = bmw, VehicleType = carType },
            new() { Name = "R 1250 GS", Brand = bmw, VehicleType = motoType },
            new() { Name = "S 1000 RR", Brand = bmw, VehicleType = motoType }, 

            new() { Name = "C-Class", Brand = mercedes, VehicleType = carType },
            new() { Name = "GLE", Brand = mercedes, VehicleType = carType },
            new() { Name = "Actros", Brand = mercedes, VehicleType = truckType }, 
            new() { Name = "Sprinter", Brand = mercedes, VehicleType = truckType }, 

            new() { Name = "A4", Brand = audi, VehicleType = carType },
            new() { Name = "A6", Brand = audi, VehicleType = carType },
            new() { Name = "Q7", Brand = audi, VehicleType = carType },
            new() { Name = "e-tron GT", Brand = audi, VehicleType = carType },

            new() { Name = "Camry", Brand = toyota, VehicleType = carType },
            new() { Name = "RAV4", Brand = toyota, VehicleType = carType },
            new() { Name = "Hilux", Brand = toyota, VehicleType = truckType }, 
            new() { Name = "Tundra", Brand = toyota, VehicleType = truckType } 
        };
    }
}