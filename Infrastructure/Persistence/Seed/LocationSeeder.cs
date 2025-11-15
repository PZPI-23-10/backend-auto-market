using System.Reflection;
using System.Text.Json;
using Domain.Entities;
using Infrastructure.Persistence.Seed.Factories;
using Infrastructure.Persistence.Seed.Factories.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Seed;

public static class LocationSeeder
{
    public static async Task Seed(DbContext context, bool flag, CancellationToken cancellationToken = default)
    {
        List<UkrainianCityDto>? cities = await ReadEmbeddedFileAsync<List<UkrainianCityDto>>(
            "Infrastructure.Persistence.Seed.SourceFiles.ukrainianCities.json"
        );

        if (cities != null)
        {
            if (!await context.Set<Region>().AnyAsync(cancellationToken: cancellationToken))
            {
                await context.Set<Region>().AddRangeAsync(RegionSeedFactory.CreateSeedData(cities), cancellationToken);
                await context.SaveChangesAsync(cancellationToken);
            }

            if (!await context.Set<City>().AnyAsync(cancellationToken))
            {
                IEnumerable<City> seedData = await CitySeedFactory.CreateSeedData(cities, context, cancellationToken);

                await context.Set<City>().AddRangeAsync(seedData, cancellationToken);

                await context.SaveChangesAsync(cancellationToken);
            }
        }
    }

    private static async Task<T?> ReadEmbeddedFileAsync<T>(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        await using var stream = assembly.GetManifestResourceStream(resourceName);

        if (stream == null)
        {
            throw new FileNotFoundException(
                $"Could not find embedded resource: {resourceName}. Check 'Build Action' (it should be 'Embedded resource') and path.");
        }

        return await JsonSerializer.DeserializeAsync<T>(stream, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }
}