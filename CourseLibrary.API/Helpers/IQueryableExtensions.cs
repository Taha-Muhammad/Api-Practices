﻿using CourseLibrary.API.Services;
using System.Linq.Dynamic.Core;

namespace CourseLibrary.API.Helpers;
public static class IQueryableExtensions
{
	public static IQueryable<T> ApplySort<T>(this IQueryable<T> source,
		string orderBy,
		Dictionary<string,PropertyMappingValue<T>> mappingDictionary)
	{
		ArgumentNullException.ThrowIfNull(source);
		ArgumentNullException.ThrowIfNull(mappingDictionary);
		if(string.IsNullOrWhiteSpace(orderBy))
			return source;

		var orderByString = string.Empty;
		var orderByAfterSplit  = orderBy.Split(',');

		foreach (var orderByClause in orderByAfterSplit)
		{
			var trimmedOrderByClause = orderByClause.Trim();

			var orderDescending = trimmedOrderByClause.EndsWith(" desc");

			var indexOfFirstSpace = trimmedOrderByClause.IndexOf(" ");
			var propertyName = indexOfFirstSpace == -1 ?
				trimmedOrderByClause : trimmedOrderByClause
				.Remove(indexOfFirstSpace);

			if (!mappingDictionary.ContainsKey(propertyName))
				throw new ArgumentException($"Key mapping for {propertyName} is missing.");
			
			var propertyMappingValue = mappingDictionary[propertyName];

			ArgumentNullException.ThrowIfNull(propertyMappingValue);

			if (propertyMappingValue.Revert)
				orderDescending = !orderDescending;

			foreach (var destinationProperty in propertyMappingValue.DestinationProperties)
			{
				orderByString = orderByString +
					(string.IsNullOrWhiteSpace(orderByString) ? string.Empty : ", ")
					+ destinationProperty
					+ (orderDescending ? " descending" : " ascending");
			}
		}
		return source.OrderBy(orderByString);
	}
}
