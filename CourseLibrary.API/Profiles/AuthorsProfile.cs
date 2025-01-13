using AutoMapper;
using CourseLibrary.API.Helpers;
using CourseLibrary.API.Models.AuthorDtos;

namespace CourseLibrary.API.Profiles;
public class AuthorsProfile : Profile
{
	public AuthorsProfile()
	{
		CreateMap<Entities.Author, AuthorDto>()
			.ForMember(dest => dest.Name, opt =>
				opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
			.ForMember(dest => dest.Age, opt =>
				opt.MapFrom(src => src.DateOfBirth.GetCurrentAge(src.DateOfDeath)));

		CreateMap<Entities.Author, AuthorForCreationDto>().ReverseMap();
		CreateMap<Entities.Author, AuthorForUpdateDto>().ReverseMap();
		CreateMap<Entities.Author, FullAuthorDto>().ReverseMap();
		CreateMap<Entities.Author, AuthorWithDateOfDeathForCreationDto>().ReverseMap();
	}
}
