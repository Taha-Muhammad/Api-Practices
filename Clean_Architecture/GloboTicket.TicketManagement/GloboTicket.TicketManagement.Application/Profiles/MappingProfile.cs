using AutoMapper;
using GloboTicket.TicketManagement.Application.Features.Categories.Commands.CreateCategory;
using GloboTicket.TicketManagement.Application.Features.Categories.Queries.GetCategoriesList;
using GloboTicket.TicketManagement.Application.Features.Categories.Queries.GetCategoriesListWithEvent;
using GloboTicket.TicketManagement.Application.Features.Events.Commands.CreateEvent;
using GloboTicket.TicketManagement.Application.Features.Events.Commands.UpdateEvent;
using GloboTicket.TicketManagement.Application.Features.Events.Queries.GetEventDetail;
using GloboTicket.TicketManagement.Application.Features.Events.Queries.GetEventsList;
using GloboTicket.TicketManagement.Domain.Entities;

namespace GloboTicket.TicketManagement.Application.Profiles
{
	internal class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<Event,EventListVm>().ReverseMap();
			CreateMap<Event,EventDetailVm>().ReverseMap();
			CreateMap<Event,CategoryEventDto>().ReverseMap();
			CreateMap<Event,CreateEventCommand>().ReverseMap();
			CreateMap<Event,UpdateEventCommand>().ReverseMap();

			CreateMap<Category,CategoryDto>().ReverseMap();
			CreateMap<Category,CategoryListVm>().ReverseMap();
			CreateMap<Category,CreateCategoryDto>().ReverseMap();
			CreateMap<Category,CategoryEventListVm>().ReverseMap();
		}
	}
}
