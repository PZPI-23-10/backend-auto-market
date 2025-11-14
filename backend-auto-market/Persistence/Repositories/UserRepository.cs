using backend_auto_market.Persistence.Models;

namespace backend_auto_market.Persistence.Repositories;

public class UserRepository(DataContext context) : BaseRepository<User>(context) { }