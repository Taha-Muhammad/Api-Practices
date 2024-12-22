using Microsoft.AspNetCore.Mvc;
using CityInfo.API.Entities;
using CityInfo.API.Services.Interfaces;
using AutoMapper;
using CityInfo.API.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace CityInfo.API.Controllers
{
	[Route("api/cities/{cityId}/pointsofinterest")]
    [ApiController]
    public class PointsOfInterestController : ControllerBase
    {
		private readonly ICityInfoRepository _cityInfoRepository;
		private readonly IMapper _mapper;

		public PointsOfInterestController(ICityInfoRepository cityInfoRepository,IMapper mapper)
        {
			_cityInfoRepository = cityInfoRepository;
			_mapper = mapper;
		}

        [HttpGet]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest(int cityId)
        {
            var exists = await _cityInfoRepository.CityExistAsync(cityId);
            if(!exists)
                return NotFound();

			return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>
                ( await _cityInfoRepository.GetPointsOfInterestForCityAsync(cityId)));
        }

        [HttpGet("{pointOfInterestId}",Name = "GetPointOfInterest")]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterest(int cityId,int pointOfInterestId)
        {
            var exists = await _cityInfoRepository.CityExistAsync(cityId);
            if(!exists)
                return NotFound();
            var pointOfInterest = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
            if (pointOfInterest == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PointOfInterestDto>(pointOfInterest));
        }

        [HttpPost]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status201Created)]
		public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest(int cityId,
            PointOfInterestForCreateDto pointOfInterest)
        {
            if(!await _cityInfoRepository.CityExistAsync(cityId))
                return NotFound();

            var pointOfInterestToCreate = _mapper.Map<PointOfInterest>(pointOfInterest);

            await _cityInfoRepository.AddPointOfInterestForCityAsync(cityId,pointOfInterestToCreate);
            await _cityInfoRepository.SaveChangesAsync();

            var createdPointOfInterest = _mapper.Map<PointOfInterestDto>(pointOfInterestToCreate);
            return CreatedAtAction("GetPointOfInterest",
                new {cityId = cityId, pointOfInterestId = createdPointOfInterest.Id },
                 createdPointOfInterest);
        }

        [HttpPut("{pointOfInterestId}")]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<IActionResult> UpdatePointOfInterest(int cityId ,int pointOfInterestId, PointOfInterestForUpdateDto pointOfInterest)
        {
            if(!await _cityInfoRepository.CityExistAsync(cityId))
                return NotFound();

            var pointOfInterestEntity = 
                await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
            if(pointOfInterestEntity == null)
                return NotFound();
            _mapper.Map(pointOfInterest, pointOfInterestEntity);
            await _cityInfoRepository.SaveChangesAsync();

            return NoContent();
        }


        [HttpPatch("{pointOfInterestId}")]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> PartiallyUpdatePointOfInterest(
            int cityId,int pointOfInterestId,JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
        {
            if(! await _cityInfoRepository.CityExistAsync(cityId))
                return NotFound();

            var pointOfInterest = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId,pointOfInterestId);
            if(pointOfInterest == null)
                return NotFound();

            var pointOfInterestToUpdate = _mapper.Map<PointOfInterestForUpdateDto>(pointOfInterest);
            patchDocument.ApplyTo(pointOfInterestToUpdate, ModelState);

            if (!ModelState.IsValid)
                return BadRequest();
            if(!TryValidateModel(pointOfInterestToUpdate))
                return BadRequest();

            _mapper.Map(pointOfInterestToUpdate, pointOfInterest);
            await _cityInfoRepository.SaveChangesAsync();
            return NoContent();

        }

        [HttpDelete("{pointOfInterestId}")]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<IActionResult> DeletePointOfInterest(int cityId,int pointOfInterestId)
        {
            if(!await _cityInfoRepository.CityExistAsync(cityId))
                return NotFound();

            var pointOfInterest = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
            if (pointOfInterest == null)
                return NotFound();
            
            _cityInfoRepository.DeletePointOfInterest(pointOfInterest);
            await _cityInfoRepository.SaveChangesAsync();

            return NoContent();
        }
    }
}
