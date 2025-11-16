using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;

namespace Infrastructure.Persistence.Repositories;

public class CityRepository(DataContext context) : BaseRepository<City>(context), ICityRepository { }