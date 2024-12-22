using CityInfo.API.DbContexts;
using CityInfo.API.Entities;
using CityInfo.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services
{
	public class CityInfoRepository : ICityInfoRepository
	{
		private readonly CityInfoContext _context;

		public CityInfoRepository(CityInfoContext context)
        {
			_context = context;
        }

		public async Task<IEnumerable<City>> GetCitiesAsync()
		{
			return await _context.Cities.OrderBy(c=>c.Name).ToListAsync();
		}

		public async Task<IEnumerable<City>> GetCitiesAsync(string? name, string? searchQuery)
		{
			if(string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace( searchQuery))
				return await GetCitiesAsync();

			var result = _context.Cities as IQueryable<City>;

			if (!string.IsNullOrWhiteSpace(name))
			{
				name = name.Trim();
				result = result.Where(c => c.Name == name);
			}
			if(!string.IsNullOrWhiteSpace(searchQuery))
			{
				searchQuery = searchQuery.Trim();
				result = result.Where(c => c.Name.Contains(searchQuery) || (c.Description !=null && c.Description.Contains(searchQuery)));
			}
			return await result.OrderBy(c=>c.Name).ToListAsync();
		}


		public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId )
		{
			return await _context.PointsOfInterest
				.Where(p=>p.CityId == cityId).ToListAsync();
		}

		public async Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest)
		{
			if (includePointsOfInterest)
				return await _context.Cities.Include(c=>c.PointsOfInterest).FirstOrDefaultAsync(c => c.Id == cityId);
			return await _context.Cities.FirstOrDefaultAsync(c=>c.Id ==cityId);
		}

		public async Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId)
		{
			return await _context.PointsOfInterest
				.FirstOrDefaultAsync(p => p.CityId == cityId &&p.Id == pointOfInterestId);
		}

		public void AddCity(City city)
		{
			if (city is not null)
				_context.Cities.Add(city);
		}

		public async Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest)
		{
			var city = await GetCityAsync(cityId,false);
			if (city is not null)
				city.PointsOfInterest.Add(pointOfInterest);
		}

		public void DeleteCity(City city)
		{
			_context.Cities.Remove(city);
		}

		public void DeletePointOfInterest(PointOfInterest pointOfInterest)
		{
			_context.PointsOfInterest.Remove(pointOfInterest);
		}

		public async Task<bool> CityExistAsync(int cityId)
		{
			return await _context.Cities.AnyAsync(c=>c.Id == cityId);
		}

		public async Task<int> SaveChangesAsync()
		{
			return await _context.SaveChangesAsync();
		}
	}
}
