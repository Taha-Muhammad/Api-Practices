using GloboTicket.TicketManagement.Application.Features.Categories.Commands.CreateCategory;
using GloboTicket.TicketManagement.Application.Features.Categories.Queries.GetCategoriesList;
using GloboTicket.TicketManagement.Application.Features.Categories.Queries.GetCategoriesListWithEvent;
using GloboTicket.TicketManagement.Application.Features.Categories.Queries.GetCategoryWithEvents;
using MediatR;
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
		[HttpGet("{id}")]
		public async Task<ActionResult<CategoryEventListVm>> GetCategoryWithEventsById(Guid id)
		{
			return Ok(await _mediator.Send(
				new GetCategoryWithEventsQuery()
				{ CategoryId = id }));
		}

		[HttpPost]
		public async Task<ActionResult<CreateCategoryDto>> CreateCategory(
			CreateCategoryCommand createCategoryCommand)
		{
			var response = await _mediator.Send(createCategoryCommand);
			return CreatedAtAction(
			nameof(GetCategoryWithEventsById),
			new {id=response.Category.CategoryId}
			,response.Category);
		}
	}
}
