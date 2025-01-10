using CourseLibrary.API.Entities;

namespace CourseLibrary.API.Services.PropertiesMappingDictionaries;

public class CoursePropertyMapping : Dictionary<string, PropertyMappingValue<Course>>
{
	public CoursePropertyMapping()
		: base(StringComparer.OrdinalIgnoreCase)
	{
		var s = new Dictionary<string, PropertyMappingValue<Course>>()
		{
			{ "Id",new(new[] { "Id" }) },
			{ "Title", new(new[] { "Title" }) },
			{ "Description",new(new[] { "Description" }) }
		};
		foreach (var item in s)
		{
			Add(item.Key, item.Value);
		}
	}
}
