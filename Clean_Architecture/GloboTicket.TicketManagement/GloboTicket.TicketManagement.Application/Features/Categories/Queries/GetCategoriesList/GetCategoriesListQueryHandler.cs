using AutoMapper;
using GloboTicket.TicketManagement.Application.Contracts.Persistence;
using GloboTicket.TicketManagement.Domain.Entities;
using MediatR;

namespace GloboTicket.TicketManagement.Application.Features.Categories.Queries.GetCategoriesList
{
	public class GetCategoriesListQueryHandler : IRequestHandler<GetCategoriesListQuery,
		List<CategoryListVm>>
	{
		private readonly IMapper _mapper;
		private readonly IAsyncRepository<Category> _categoryRepository;

		public GetCategoriesListQueryHandler(IAsyncRepository<Category> categoryRepository, IMapper mapper)
		{
			_categoryRepository = categoryRepository;
			_mapper = mapper;
		}

		public async Task<List<CategoryListVm>> Handle(GetCategoriesListQuery request,
			CancellationToken cancellationToken)
		{
			var categoriesEntities = (await _categoryRepository
				.ListAllAsync()).OrderBy(c=>c.Name);
			
			return _mapper.Map<List<CategoryListVm>>(categoriesEntities);
		}
	}
}
