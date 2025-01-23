using GloboTicket.TicketManagement.Application.Exceptions;
using GloboTicket.TicketManagement.Application.Features.Events.Commands.CreateEvent;
using GloboTicket.TicketManagement.Application.Features.Events.Commands.DeleteEvent;
using GloboTicket.TicketManagement.Application.Features.Events.Commands.UpdateEvent;
using GloboTicket.TicketManagement.Application.Features.Events.Queries.GetEventDetail;
using GloboTicket.TicketManagement.Application.Features.Events.Queries.GetEventsExport;
using GloboTicket.TicketManagement.Application.Features.Events.Queries.GetEventsList;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GloboTicket.TicketManagement.Api.Controllers
{
	[Route("api/events")]
	[ApiController]
	public class EventController : ControllerBase
	{
		private readonly IMediator _mediator;

		public EventController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<ActionResult<List<EventListVm>>> GetAllEvents()
		{
			var result = await _mediator.Send(new GetEventsListQuery());
			return Ok(result);
		}
		[HttpGet("{eventId}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<ActionResult<EventDetailVm>> GetEventById(Guid eventId)
		{
			var result = await _mediator.
				Send(new GetEventDetailQuery() { Id = eventId });
			return Ok(result);
		}

		[HttpPost]
		public async Task<ActionResult<Guid>> CreateEvent(CreateEventCommand createEventCommand)
		{
			var id = await _mediator.Send(createEventCommand);
			return CreatedAtAction(nameof(GetEventById),
				new { eventId = id }, id);
		}

		[HttpPut]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult> UpdateEvent(UpdateEventCommand updateEventCommand)
		{
			await _mediator.Send(updateEventCommand);
			return NoContent();
		}

		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult> DeleteEvent(Guid id)
		{
			await _mediator.Send(new DeleteEventCommand() { EventId = id});
			return NoContent();
		}
		[HttpGet("export", Name = "ExportEvents")]
		public async Task<FileResult> ExportEvents()
		{
			var fileDto = await _mediator.Send(new GetEventsExportQuery());

			return File(fileDto.Data!, 
				fileDto.ContentType,
				fileDto.EventExportFileName);
		}
	}
}
