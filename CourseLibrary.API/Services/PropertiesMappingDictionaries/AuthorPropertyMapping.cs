using CourseLibrary.API.Entities;

namespace CourseLibrary.API.Services.PropertiesMappingDictionaries;

public class AuthorPropertyMapping : PropertyMapping<Author>
{
	public AuthorPropertyMapping()
		: base()
	{
		var s = new Dictionary<string, PropertyMappingValue<Author>>()
		{
			{"Id",new(new[] { "Id" }) },
			{"MainCategory", new(new[]{"MainCategory"}) },
			{"Age",new(new[]{"DateOfBirth" },true ) },
			{"Name",new(new[]{ "FirstName","LastName" }) }
		};
		foreach (var item in s)
		{
			Add(item.Key, item.Value);
		}

	}
}
