using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Models;

namespace CityInfo.API.Profiles
{
	public class PointOfInterestProfile : Profile
	{
		public PointOfInterestProfile()
		{
			CreateMap<PointOfInterest,PointOfInterestDto>().ReverseMap();
			CreateMap<PointOfInterest,PointOfInterestForUpdateDto>().ReverseMap();
			CreateMap<PointOfInterest,PointOfInterestForCreateDto>().ReverseMap();
			CreateMap<PointOfInterestDto,PointOfInterestForCreateDto>().ReverseMap();
			CreateMap<PointOfInterestDto,PointOfInterestForUpdateDto>().ReverseMap();
		}
	}
}
