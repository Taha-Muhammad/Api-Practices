﻿using GloboTicket.TicketManagement.Application.Contracts.Persistence;
using GloboTicket.TicketManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GloboTicket.TicketManagement.Persistence.Repositories
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(GloboTicketDbContext dbContext) : base(dbContext){}

		public async Task<List<Category>> GetCategoriesWithEventsAsync(bool includePassedEvents)
		{
			var allCategories = await _dbContext.Categories
                .Include(x => x.Events).ToListAsync();
			if (!includePassedEvents)
			{
				allCategories.ForEach(p => p.Events?.ToList()
				.RemoveAll(c => c.Date < DateTime.Today));
			}
			return allCategories;
		}

		public async Task<Category?> GetCategoryWithEventsAsync(Guid categoryId)
		{
			return await _dbContext.Categories.Include(c => c.Events)
				.FirstOrDefaultAsync(c=>c.CategoryId == categoryId);
		}
	}
}
