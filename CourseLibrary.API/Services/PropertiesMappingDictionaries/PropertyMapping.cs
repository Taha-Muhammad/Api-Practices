namespace CourseLibrary.API.Services.PropertiesMappingDictionaries;

public abstract class PropertyMapping<T> : Dictionary<string, PropertyMappingValue<T>>
{
	protected PropertyMapping():base(StringComparer.OrdinalIgnoreCase) { }
	public virtual bool ValidMappingExistsFor(string fields)
	{
		if(string.IsNullOrWhiteSpace(fields))
			return true;
		
		var fieldsAfterSplit = fields.Split(',');

		foreach (var field in fieldsAfterSplit)
		{
			var trimmedField = field.Trim();

			var indexOfFirstSpace = trimmedField.IndexOf(" ");

			var propertyName = indexOfFirstSpace == -1 ?
				trimmedField : trimmedField.Remove(indexOfFirstSpace);

			if(!this.ContainsKey(propertyName))
				return false;
		}
		return true;
	}
}
