﻿using GloboTicket.TicketManagement.Domain.Entities;

namespace GloboTicket.TicketManagement.Application.Contracts.Persistence
{
	public interface ICategoryRepository : IAsyncRepository<Category>
	{
		Task<List<Category>> GetCategoriesWithEventsAsync(bool includePassedEvents);
		Task<Category?> GetCategoryWithEventsAsync(Guid categoryId);
	}
}
