using CourseLibrary.API.ResourceParameters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CourseLibrary.API.Helpers;
public class PagedList<T> : List<T>
{
	public int CurrentPage { get; private set; }
	public int TotalPages { get; private set; }
	public int PageSize { get; private set; }
	public int TotalCount { get; private set; }
	public bool HasPrevious => (CurrentPage > 1);
	public bool HasNext => (CurrentPage < TotalPages);

	public PagedList(List<T> items, int count, int pageNumber, int pageSize)
	{
		PageSize = pageSize;
		CurrentPage = pageNumber;
		TotalCount = count;
		TotalPages = (int)Math.Ceiling(count / (double)pageSize);
		AddRange(items);
	}
	public static async Task<PagedList<T>> CreateAsync(
		IQueryable<T> source, int pageNumber, int pageSize)
	{
		var count = source.Count();
		var items = await source.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize).ToListAsync();
		return new PagedList<T>(items, count, pageNumber, pageSize);
	}
	public string CreatePaginationHeaderContent(BaseResourceParameters resourceParameters,
		PagedList<T> pagedList,
		IUrlHelper Url,
		string routeName,
		string? mainCategory = null,
		string? title = null)
	{
		var previousPageLink = pagedList.HasPrevious ?
					ResourceUris.CreateResourceUri(
					resourceParameters,
					ResourceUris.ResourceUriType.PreviousPage,
					routeName,
					Url, mainCategory, title) : null;

		var nextPageLink = pagedList.HasNext ?
			ResourceUris.CreateResourceUri(
		resourceParameters,
			ResourceUris.ResourceUriType.NextPage,
			routeName,
			Url, mainCategory, title) : null;

		var paginationMetadata = new
		{
			totalCount = pagedList.TotalCount,
			pageSize = pagedList.PageSize,
			currentPage = pagedList.CurrentPage,
			totalPages = pagedList.TotalPages,
			previousPageLink,
			nextPageLink
		};
		return JsonSerializer.Serialize(paginationMetadata);
	}
}
