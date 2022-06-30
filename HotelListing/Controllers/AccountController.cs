using AutoMapper;
using HotelListing.Data;
using HotelListing.Models;
using HotelListing.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace HotelListing.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AccountController : ControllerBase
	{
		private readonly UserManager<ApiUser> _userManager;

		private readonly ILogger<AccountController> _logger;
		private readonly IMapper _mapper;
		private readonly IAuthManager _authManager;

		public AccountController(UserManager<ApiUser> userManager,
								ILogger<AccountController> logger, IMapper mapper, IAuthManager authManager)
		{
			_userManager = userManager;
			_authManager = authManager;
			_logger = logger;
			_mapper = mapper;
		}

		[HttpPost]
		[Route("register")]
		public async Task<IActionResult> Register([FromBody] UserDTO userDTO)
		{

			_logger.LogInformation($"Registration Attempt for {userDTO.Email}");
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{
				var user = _mapper.Map<ApiUser>(userDTO);

				user.UserName = userDTO.Email;

				var result = await _userManager.CreateAsync(user, userDTO.Password);

				if (!result.Succeeded)
				{
					foreach (var error in result.Errors)
					{
						ModelState.AddModelError(error.Code, error.Description);
					}
					return BadRequest(ModelState);
				}

				await _userManager.AddToRolesAsync(user, userDTO.Roles);
				return Accepted();

			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Something went wrong in the {nameof(Register)}");
				return StatusCode(500, "Internal Server Error, Please Try Again Later");

			}
		}

		[HttpPost]
		[Route("login")]
		public async Task<IActionResult> LogIn([FromBody] LogInUserDTO userDTO)
		{

			_logger.LogInformation($"Login Attempt for {userDTO.Email}");
			if (!ModelState.IsValid)
			{
				_logger.LogInformation($"Model state is not valid for {userDTO.Email}");
				return BadRequest(ModelState);
			}

			try
			{


				if (!await _authManager.ValidateUser(userDTO))
				{
					_logger.LogInformation($"Is not allowed to login {userDTO.Email}");
					return Unauthorized();
				}


				return Accepted(new { Token = await _authManager.CreateToken() });

			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Something went wrong in the {nameof(LogIn)}");
				return Problem($"Internal Server Error, Please Try Again Later {nameof(LogIn)}", statusCode: 500);

			}
		}

	}
}
