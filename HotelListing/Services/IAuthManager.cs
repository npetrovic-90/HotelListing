using HotelListing.Models;
using System.Threading.Tasks;

namespace HotelListing.Services
{
	public interface IAuthManager
	{
		Task<bool> ValidateUser(LogInUserDTO userDTO);
		Task<string> CreateToken();
	}
}
