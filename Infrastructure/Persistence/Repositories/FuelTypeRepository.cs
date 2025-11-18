using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;

namespace Infrastructure.Persistence.Repositories;

public class FuelTypeRepository(DataContext context)
    : BaseRepository<FuelType>(context), IFuelTypeRepository { }