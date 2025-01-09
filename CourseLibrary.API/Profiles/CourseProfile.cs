using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Models;

namespace CourseLibrary.API.Profiles;
public class CoursesProfile : Profile
{
	public CoursesProfile()
	{
		CreateMap<Course, CourseDto>().ReverseMap();
		CreateMap<CourseForCreationDto, Course>().ReverseMap();
		CreateMap<CourseForUpdateDto, Course>().ReverseMap();
		CreateMap<CourseForCreationDto, CourseDto>().ReverseMap();
	}
}