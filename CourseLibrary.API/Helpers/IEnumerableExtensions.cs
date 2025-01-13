using System.Dynamic;
using System.Reflection;

namespace CourseLibrary.API.Helpers;

public static class IEnumerableExtensions
{
	public static IEnumerable<ExpandoObject> ShapeData<TSource>(
		this IEnumerable<TSource> source,
		string? fields)
	{
		ArgumentNullException.ThrowIfNull(source, nameof(source));

		var expandoObjectList = new List<ExpandoObject>();

		//create a list with PropertyInfo objects on TSource.
		//Reflection is expensive, so rather than doing it for each object in the list,
		//we do it once and reuse the results.
		var propertyInfoList = new List<PropertyInfo>();

		if (string.IsNullOrWhiteSpace(fields))
		{
			//if no fields are requested that means we should return all public properties.
			var propertiesInfo = typeof(TSource)
				.GetProperties(BindingFlags.IgnoreCase
				| BindingFlags.Public | BindingFlags.Instance);
			propertyInfoList.AddRange(propertiesInfo);
		}
		else
		{
			var fieldsAfterSplit = fields.Split(',');

			foreach (var field in fieldsAfterSplit)
			{
				var propertyName = field.Trim();

				var propertyInfo = typeof(TSource)
					.GetProperty(propertyName, BindingFlags.IgnoreCase |
					BindingFlags.Public | BindingFlags.Instance);
				if (propertyInfo == null)
				{
					throw new Exception($"Property {propertyName} wasn't found on " +
						$"{typeof(TSource)}");
				}
				propertyInfoList.Add(propertyInfo);
			}
		}
		foreach (var sourceObject in source)
		{
			var dataShapedObject = new ExpandoObject();

			foreach (var propertyInfo in propertyInfoList)
			{
				var propertyValue = propertyInfo.GetValue(sourceObject);

				((IDictionary<string, object?>)dataShapedObject)
					.Add(propertyInfo.Name, propertyValue);
			}
			expandoObjectList.Add(dataShapedObject);
		}
		return expandoObjectList;
	}
}
