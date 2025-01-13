using CourseLibrary.API.Entities;

namespace CourseLibrary.API.Services.PropertiesMappingDictionaries;

public class CoursePropertyMapping : PropertyMapping<Course>
{
	public CoursePropertyMapping()
		: base()
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
