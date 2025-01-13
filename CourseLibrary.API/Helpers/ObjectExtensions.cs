using System.Dynamic;
using System.Reflection;

namespace CourseLibrary.API.Helpers;

public static class ObjectExtensions
{
	//we didn't use this in the IEnumerable one because the reflection will damage the performance
	public static ExpandoObject ShapeData<TSource>(this TSource source,
		string? fields)
	{
		ArgumentNullException.ThrowIfNull(source, nameof(source));

		var dataShapedObject = new ExpandoObject();
		if (string.IsNullOrWhiteSpace(fields))
		{
			//if no fields are requested that means we should return all public properties.
			var propertiesInfo = typeof(TSource)
				.GetProperties(BindingFlags.IgnoreCase
				| BindingFlags.Public | BindingFlags.Instance);

			foreach (var propertyInfo in propertiesInfo)
			{
				var propertyValue = propertyInfo.GetValue(source);
				((IDictionary<string, object?>)dataShapedObject)
					.Add(propertyInfo.Name, propertyValue);
			}
			return dataShapedObject;
		}
		var fieldsAfterSplit = fields.Split(',');

		foreach (var field in fieldsAfterSplit)
		{
			var propertyName = field.Trim();

			var propertyInfo = typeof(TSource)
				.GetProperty(propertyName,
				BindingFlags.IgnoreCase | BindingFlags.Public
				| BindingFlags.Instance);

			if (propertyInfo == null)
			{
				throw new Exception($"Property {propertyName} wasn't found on " +
					$"{typeof(TSource)}");
			}

			var propertyValue = propertyInfo.GetValue(source);
			((IDictionary<string, object?>)dataShapedObject)
				.Add(propertyInfo.Name, propertyValue);

		}
		return dataShapedObject;
	}
}
