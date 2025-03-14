﻿namespace GloboTicket.TicketManagement.Application.Features.Categories.Queries.GetCategoriesListWithEvent
{
	public class CategoryEventDto
	{
		public Guid EventId { get; set; }
		public string Name { get; set; } = string.Empty;
		public int Price { get; set; }
		public string Artist { get; set; } = string.Empty;
		public DateTime Date {  get; set; }
		public Guid CategoryId { get; set; }
	}
}
