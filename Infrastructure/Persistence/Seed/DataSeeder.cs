using Domain.Entities;
using Infrastructure.Persistence.Seed.Factories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Seed;

public class DataSeeder
{
    public static async Task Seed(DbContext context, bool flag, CancellationToken cancellationToken = default)
    {
        if (!await context.Set<VehicleCondition>().AnyAsync(cancellationToken: cancellationToken))
        {
            await context.Set<VehicleCondition>()
                .AddRangeAsync(VehicleConditionsSeedFactory.CreateSeedData(), cancellationToken);
            
            await context.SaveChangesAsync(cancellationToken);
        }

        if (!await context.Set<VehicleBodyType>().AnyAsync(cancellationToken: cancellationToken))
        {
            await context.Set<VehicleBodyType>()
                .AddRangeAsync(VehicleBodyTypeSeedFactory.CreateSeedData(), cancellationToken);
            
            await context.SaveChangesAsync(cancellationToken);
        }

        if (!await context.Set<VehicleBrand>().AnyAsync(cancellationToken: cancellationToken))
        {
            await context.Set<VehicleBrand>()
                .AddRangeAsync(VehicleBrandSeedFactory.CreateSeedData(), cancellationToken);
            
            await context.SaveChangesAsync(cancellationToken);
        }

        if (!await context.Set<VehicleModel>().AnyAsync(cancellationToken))
        {
            var modelsData = await VehicleModelSeedFactory.CreateSeedData(context, cancellationToken);

            await context.Set<VehicleModel>().AddRangeAsync(
                modelsData,
                cancellationToken
            );
            
            await context.SaveChangesAsync(cancellationToken);
        }

        if (!await context.Set<VehicleModelBodyType>().AnyAsync(cancellationToken))
        {
            var modelBodyTypes = await VehicleModelBodyTypeSeedFactory.CreateSeedData(context, cancellationToken);

            await context.Set<VehicleModelBodyType>().AddRangeAsync(
                modelBodyTypes,
                cancellationToken
            );

            await context.SaveChangesAsync(cancellationToken);
        }

        await LocationSeeder.Seed(context, flag, cancellationToken);
    }
}