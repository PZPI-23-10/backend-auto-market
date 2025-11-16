using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;

namespace Infrastructure.Persistence.Repositories;

public class RegionRepository(DataContext context) : BaseRepository<Region>(context), IRegionRepository { }