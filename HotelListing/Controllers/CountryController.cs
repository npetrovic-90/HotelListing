using AutoMapper;
using HotelListing.Data;
using HotelListing.IRepository;
using HotelListing.Models;
using Microsoft.AspNetCore.Authorization;
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
	public class CountryController : ControllerBase
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogger<CountryController> _logger;
		private readonly IMapper _mapper;

		public CountryController(IUnitOfWork unitOfWork, ILogger<CountryController> logger, IMapper mapper)
		{
			_logger = logger;
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}


		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetCountries()
		{
			try
			{
				var countries = await _unitOfWork.Countries.GetAll();

				var results = _mapper.Map<IList<CountryDTO>>(countries);

				return Ok(results);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Something went wrong in the {nameof(GetCountries)}");
				return StatusCode(500, "Internal Server Error, Please Try Again later.");
			}
		}
		[HttpGet("{id:int}", Name = "GetCountry")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetCountry(int id)
		{
			try
			{
				var country = await _unitOfWork.Countries.Get(q => q.Id == id, new List<string> { "Hotels" });

				var results = _mapper.Map<CountryDTO>(country);

				return Ok(results);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Something went wrong in the {nameof(GetCountries)}");
				return StatusCode(500, "Internal Server Error, Please Try Again later.");
			}
		}

		[HttpPost]
		[Authorize(Roles = "Administrator")]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> CreateCountry([FromBody] CreateCountryDTO countryDTO)
		{
			if (!ModelState.IsValid)
			{
				_logger.LogError($"Invalid POST attempt in {nameof(CreateCountry)}");
				return BadRequest(ModelState);
			}

			try
			{
				var country = _mapper.Map<Country>(countryDTO);
				await _unitOfWork.Countries.Insert(country);
				await _unitOfWork.Save();

				return CreatedAtRoute("GetCountry", new { id = country.Id }, country);

			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Something went wrong in the {nameof(CreateCountry)}");
				return Problem($"Internal Server Error, Please Try Again Later {nameof(CreateCountry)}", statusCode: 500);

			}
		}

		[Authorize]
		[HttpPut("{id:int}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> UpdateCountry(int id, [FromBody] UpdateCountryDTO countryDTO)
		{
			if (!ModelState.IsValid || id < 1)
			{
				_logger.LogError($"Invalid PUT attempt in {nameof(UpdateCountry)}");
				return BadRequest(ModelState);
			}

			try
			{
				var country = await _unitOfWork.Countries.Get(q => q.Id == id);

				if (country == null)
				{
					_logger.LogError($"Invalid PUT attempt in {nameof(UpdateCountry)}");
					return BadRequest("Submitted data is invalid");

				}

				_mapper.Map(countryDTO, country);
				_unitOfWork.Countries.Update(country);
				await _unitOfWork.Save();

				return NoContent();
			}
			catch (Exception ex)
			{

				_logger.LogError(ex, $"Something went wrong in the {nameof(UpdateCountry)}");
				return Problem($"Internal Server Error, Please Try Again Later {nameof(UpdateCountry)}", statusCode: 500);
			}
		}

		[Authorize()]
		[HttpDelete("{id:int}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> DeleteCountry(int id)
		{
			if (id < 1)
			{
				_logger.LogError($"Invalid DELETE attempt in{nameof(DeleteCountry)}");
				return BadRequest();
			}

			try
			{
				var country = await _unitOfWork.Countries.Get(h => h.Id == id);

				if (country == null)
				{
					_logger.LogError($"Invalid DELETE attempt in {nameof(DeleteCountry)}");
					return BadRequest("Submitted data is invalid");
				}

				await _unitOfWork.Countries.Delete(id);
				await _unitOfWork.Save();

				return NoContent();
			}
			catch (Exception ex)
			{

				_logger.LogError(ex, $"Something went wrong in the {nameof(DeleteCountry)}");
				return Problem($"Internal Server Error, Please Try Again Later {nameof(DeleteCountry)}", statusCode: 500);
			}
		}


	}
}
