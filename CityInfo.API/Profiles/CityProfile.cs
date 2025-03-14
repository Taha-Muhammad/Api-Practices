﻿using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Models;

namespace CityInfo.API.Profiles
{
	public class CityProfile : Profile
	{
		public CityProfile() 
		{
			CreateMap<City,CityDto>().ReverseMap();
			CreateMap<City,CityWithoutPointOfInterestDto>().ReverseMap();
			CreateMap<City,CityWithoutPointOfInterestForCreateDto>().ReverseMap();
			CreateMap<City,CityWithoutPointOfInterestForUpdateDto>().ReverseMap();
			CreateMap<CityDto,CityWithoutPointOfInterestForCreateDto>().ReverseMap();

		}
	}
}
