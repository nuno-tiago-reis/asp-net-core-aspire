namespace Memento.Aspire.Core.Swagger.Filters;

using FluentValidation;
using FluentValidation.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text.Json.Serialization;

/// <summary>
/// Implements an <see cref="ISchemaFilter" />.
/// Applies the necessary restrictions based on the implementations of <see cref="IValidator"/>.
/// </summary>
public sealed class FluentValidationSchemaFilter : ISchemaFilter
{
	#region [Properties]
	/// <summary>
	/// The service provider.
	/// </summary>
	private readonly IServiceProvider ServiceProvider;
	#endregion

	#region [Constructors]
	/// <summary>
	/// Initializes a new instance of the <see cref="FluentValidationSchemaFilter"/> class.
	/// </summary>
	///
	/// <param modelName="serviceProvider">The service provider.</param>
	public FluentValidationSchemaFilter(IServiceProvider serviceProvider)
	{
		this.ServiceProvider = serviceProvider;
	}
	#endregion

	#region [Methods]
	/// <inheritdoc />
	public void Apply(OpenApiSchema schema, SchemaFilterContext context)
	{
		schema.Required ??= new HashSet<string>();

		foreach (var key in schema.Properties.Keys)
		{
			var validators = this.GetValidatorsForMember(context.Type, key);

			foreach (var validator in validators)
			{
				switch (validator)
				{
					case INotNullValidator _:
					{
						schema.Required.Add(key);
						break;
					}
					case INotEmptyValidator _:
					{
						schema.Required.Add(key);
						break;
					}
					case ILengthValidator lengthValidator:
					{
						if (lengthValidator.Max > 0)
						{
							schema.Properties[key].MaxLength = lengthValidator.Max;
						}

						if (lengthValidator.Min > 0)
						{
							schema.Properties[key].MinLength = lengthValidator.Min;
						}

						break;
					}
					case IRegularExpressionValidator expressionValidator:
					{
						schema.Properties[key].Pattern = expressionValidator.Expression;
						break;
					}
				}
			}
		}
	}

	/// <summary>
	/// Gets the implementations of <see cref="IPropertyValidator"/> for the given <paramref name="modelType"/> property <paramref name="modelName"/>.
	/// </summary>
	///
	/// <param name="modelType">The schema modelType.</param>
	/// <param name="modelName">The schema property name.</param>
	private IEnumerable<IPropertyValidator> GetValidatorsForMember(Type modelType, string modelName)
	{
		var validatorType = typeof(IValidator<>).MakeGenericType(modelType);

		if (this.ServiceProvider.GetService(validatorType) is IValidator validator)
		{
			var descriptor = validator.CreateDescriptor();

			foreach (var property in modelType.GetProperties())
			{
				var fromQuery = property.GetCustomAttribute<FromQueryAttribute>();
				var jsonPropertyName = property.GetCustomAttribute<JsonPropertyNameAttribute>();

				if (string.Equals(property.Name, modelName, StringComparison.OrdinalIgnoreCase))
				{
					return descriptor.GetValidatorsForMember(property.Name).Select((validator) => validator.Validator);
				}

				if (string.Equals(fromQuery?.Name, modelName, StringComparison.OrdinalIgnoreCase))
				{
					return descriptor.GetValidatorsForMember(property.Name).Select((validator) => validator.Validator);
				}

				if (string.Equals(jsonPropertyName?.Name, modelName, StringComparison.OrdinalIgnoreCase))
				{
					return descriptor.GetValidatorsForMember(property.Name).Select((validator) => validator.Validator);
				}
			}
		}

		return [];
	}
	#endregion
}
