using GloboTicket.TicketManagement.Application.Features.Categories.Commands.CreateCategory;
using GloboTicket.TicketManagement.Application.Features.Categories.Queries.GetCategoriesList;
using GloboTicket.TicketManagement.Application.Features.Categories.Queries.GetCategoriesListWithEvent;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GloboTicket.TicketManagement.Api.Controllers
{
	[Route("api/categories")]
	[ApiController]
	public class CategoryController : ControllerBase
	{
		private readonly IMediator _mediator;

		public CategoryController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<ActionResult<List<CategoryListVm>>> GetAllCategories
			(bool withEvents=false,bool includeHistory =false)
		{
			if(withEvents)
			{
				var listVms = await _mediator
				.Send(new GetCategoriesListWithEventsQuery() { IncludeHistory = includeHistory });
				return Ok(listVms);
			}
			var dtos = await _mediator.Send(new GetCategoriesListQuery());
			return Ok(dtos);
		}

		[HttpPost]
		public async Task<ActionResult<CreateCategoryDto>> CreateCategory(
			CreateCategoryCommand createCategoryCommand)
		{
			var response = await _mediator.Send(createCategoryCommand);
			return Ok(response.Category);
		}
	}
}
