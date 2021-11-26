using System.Threading.Tasks;
using WepA.Data.Repositories.Interfaces;

namespace WepA.Data.Repositories
{
	public class UserRepository : IUserRepository
	{
		private readonly WepADbContext _context;
		public UserRepository(WepADbContext context) => _context = context;
		public async Task SaveAsync() => await _context.SaveChangesAsync();
	}
}