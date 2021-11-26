using System.Threading.Tasks;

namespace WepA.Data.Repositories.Interfaces
{
	public interface IUserRepository
	{
		Task SaveAsync();
	}
}