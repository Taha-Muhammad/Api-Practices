using CityInfo.API.Entities;

namespace CityInfo.API.Services.Interfaces
{
	public interface ICityInfoRepository
	{
		Task<IEnumerable<City>> GetCitiesAsync();
		Task<IEnumerable<City>> GetCitiesAsync(string? name,string? searchQuery);
		Task<City?> GetCityAsync(int cityId,bool includePointsOfInterest);
		Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId);
		Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId);
		void AddCity(City city);
		Task AddPointOfInterestForCityAsync(int cityId,PointOfInterest pointOfInterest);
		void DeleteCity(City city);
		void DeletePointOfInterest(PointOfInterest pointOfInterest);
		Task<bool> CityExistAsync(int cityId);
		Task<int> SaveChangesAsync();
	}
}
