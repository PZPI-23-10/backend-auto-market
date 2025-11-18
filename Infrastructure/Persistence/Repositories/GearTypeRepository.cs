using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;

namespace Infrastructure.Persistence.Repositories;

public class GearTypeRepository(DataContext context)
    : BaseRepository<GearType>(context), IGearTypeRepository { }