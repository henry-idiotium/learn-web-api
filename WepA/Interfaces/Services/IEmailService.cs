using System.Threading.Tasks;
using WepA.Models.Domains;
using WepA.Models.Dtos;

namespace WepA.Interfaces.Services
{
	public interface IEmailService
	{
		Task SendConfirmEmailAsync(ApplicationUser user, string encodedConfirmString);
	}
}