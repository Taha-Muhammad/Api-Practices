﻿using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel;
using System.Reflection;

namespace CourseLibrary.API.Helpers;
public class ArrayModelBinder : IModelBinder
{
	public Task BindModelAsync(ModelBindingContext bindingContext)
	{
		if (!bindingContext.ModelMetadata.IsEnumerableType)
		{
			bindingContext.Result = ModelBindingResult.Failed();
			return Task.CompletedTask;
		}

		var value = bindingContext.ValueProvider
			.GetValue(bindingContext.ModelName).ToString();

		if (string.IsNullOrWhiteSpace(value))
		{
			bindingContext.Result = ModelBindingResult.Success(null);
			return Task.CompletedTask;
		}

		var elementType = bindingContext.ModelType.GetTypeInfo().GenericTypeArguments[0];
		var converter = TypeDescriptor.GetConverter(elementType);

		var values = value.Split([","],
			StringSplitOptions.RemoveEmptyEntries)
			.Select(x => converter.ConvertFromString(x.Trim())).ToArray();

		var typedValues = Array.CreateInstance(elementType, values.Length);
		values.CopyTo(typedValues, 0);
		bindingContext.Model = typedValues;

		bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);
		return Task.CompletedTask;
	}
}
