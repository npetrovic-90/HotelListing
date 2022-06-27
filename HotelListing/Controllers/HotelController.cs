using AutoMapper;
using HotelListing.IRepository;
using HotelListing.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotelListing.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class HotelController : ControllerBase
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogger<HotelController> _logger;
		private readonly IMapper _mapper;

		public HotelController(IUnitOfWork unitOfWork, ILogger<HotelController> logger, IMapper mapper)
		{
			_logger = logger;
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetHotels()
		{
			try
			{
				var countries = await _unitOfWork.Hotels.GetAll();

				var results = _mapper.Map<IList<HotelDTO>>(countries);

				return Ok(results);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Something went wrong in the {nameof(GetHotels)}");
				return StatusCode(500, "Internal Server Error, Please Try Again later.");
			}

		}
		[HttpGet("{id:int}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetHotel(int id)
		{
			try
			{
				var country = await _unitOfWork.Hotels.Get(q => q.Id == id, new List<string> { "Country" });

				var results = _mapper.Map<HotelDTO>(country);

				return Ok(results);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Something went wrong in the {nameof(GetHotels)}");
				return StatusCode(500, "Internal Server Error, Please Try Again later.");
			}
		}
	}
}
