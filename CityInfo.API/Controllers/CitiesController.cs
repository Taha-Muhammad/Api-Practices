using Microsoft.AspNetCore.Mvc;
using CityInfo.API.Entities;
using CityInfo.API.Services.Interfaces;
using CityInfo.API.Models;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;

namespace CityInfo.API.Controllers
{
	[Route("api/cities")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private readonly ICityInfoRepository _cityInfoRepository;
		private readonly IMapper _mapper;

		public CitiesController(ICityInfoRepository cityInfoRepository,IMapper mapper)
        {
            _cityInfoRepository = cityInfoRepository;
			_mapper = mapper;
		}

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CityWithoutPointOfInterestDto>>> GetCities(string? name,string? searchQuery)
        {
            var cities = await _cityInfoRepository.GetCitiesAsync(name,searchQuery);
			return Ok(_mapper.Map<IEnumerable<CityWithoutPointOfInterestDto>>(cities));
        }

        [HttpGet("{cityId}",Name ="GetCity")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetCity(int cityId,bool includePointsOfInterest = false)
        {
            var city = await _cityInfoRepository.GetCityAsync(cityId,includePointsOfInterest);

            if (city == null)
                return NotFound();
            
            if (includePointsOfInterest)
                return Ok(_mapper.Map<CityDto>(city));

            return Ok(_mapper.Map<CityWithoutPointOfInterestDto>(city));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
		public async Task<ActionResult<CityDto>> CreateCity(CityWithoutPointOfInterestForCreateDto cityForCreation)
        {
            var cityEntity  = _mapper.Map<City>(cityForCreation);
            
            _cityInfoRepository.AddCity(cityEntity);
            await _cityInfoRepository.SaveChangesAsync();

            var cityToReturn = _mapper.Map<CityDto>(cityEntity);
            return CreatedAtAction("GetCity", new { cityId = cityToReturn.Id }, cityToReturn);
        }

        [HttpPut("{cityId}")]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<IActionResult> UpdateCity(int cityId, CityWithoutPointOfInterestForUpdateDto cityForUpdate)
        {
            var cityEntity = await _cityInfoRepository.GetCityAsync(cityId,false);
            if (cityEntity == null)
                return NotFound();
            
            _mapper.Map(cityForUpdate, cityEntity);
            await _cityInfoRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{cityId}")]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> PartiallyUpdateCity(int cityId,JsonPatchDocument<CityWithoutPointOfInterestForUpdateDto> jsonPatchDocument)
        {
            var cityEntity = await _cityInfoRepository.GetCityAsync(cityId,false);
            if(cityEntity == null)
                return NotFound();

            var cityForUpdate = _mapper.Map<CityWithoutPointOfInterestForUpdateDto>(cityEntity);

            jsonPatchDocument.ApplyTo(cityForUpdate,ModelState);
            if(!ModelState.IsValid)
                return BadRequest();

            if(!TryValidateModel(cityForUpdate))
                return BadRequest();

            _mapper.Map(cityForUpdate,cityEntity);
            await _cityInfoRepository.SaveChangesAsync();

            return NoContent();
            
        }

		[HttpDelete("{cityId}")]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<IActionResult> DeleteCity(int cityId)
        {
            var city = await _cityInfoRepository.GetCityAsync(cityId,false);
            if (city == null)
            {
                return NotFound();
            }

            _cityInfoRepository.DeleteCity(city);
            await _cityInfoRepository.SaveChangesAsync();

            return NoContent();
        }

        
    }
}
