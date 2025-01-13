using CourseLibrary.API.ResourceParameters;
using Microsoft.AspNetCore.Mvc;

namespace CourseLibrary.API.Helpers;

public class ResourceUris
{
	public static string? CreateResourceUri(
		BaseResourceParameters resourceParameters,
		ResourceUriType type,
		string routeName,
		IUrlHelper url,
		string? mainCategory = null,
		string? title = null)
	{
		switch (type)
		{
			case ResourceUriType.PreviousPage:
				return url.Link(routeName,
					new
					{
						pageNumber = resourceParameters.PageNumber - 1,
						pageSize = resourceParameters.PageSize,
						fields = resourceParameters.Fields,
						mainCategory,
						title,
						orderBy = resourceParameters.OrderBy,
						searchQuery = resourceParameters.SearchQuery
					});
			case ResourceUriType.NextPage:
				var result = url.Link(routeName,
					new
					{
						pageNumber = resourceParameters.PageNumber + 1,
						pageSize = resourceParameters.PageSize,
						fields = resourceParameters.Fields,
						mainCategory,
						title,
						orderBy = resourceParameters.OrderBy,
						searchQuery = resourceParameters.SearchQuery
					});
				return result;
			case ResourceUriType.Current:
			default:
				return url.Link(routeName,
					new
					{
						pageNumber = resourceParameters.PageNumber,
						pageSize = resourceParameters.PageSize,
						fields = resourceParameters.Fields,
						mainCategory,
						title,
						orderBy = resourceParameters.OrderBy,
						searchQuery = resourceParameters.SearchQuery
					});
		}
	}

	public enum ResourceUriType
	{
		PreviousPage,
		NextPage,
		Current
	}
}
