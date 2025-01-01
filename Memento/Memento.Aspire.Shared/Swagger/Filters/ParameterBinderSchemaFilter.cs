namespace Memento.Aspire.Shared.Swagger.Filters;

using Memento.Aspire.Shared.Binding;
using Memento.Aspire.Shared.Extensions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

/// <summary>
/// Implements an <see cref="ISchemaFilter" />.
/// Unpacks the <see cref="ParameterBinder{T}"/> to expose the underlying type.
/// </summary>
public sealed class ParameterBinderSchemaFilter : ISchemaFilter
{
	#region [Methods]
	/// <inheritdoc />
	public void Apply(OpenApiSchema model, SchemaFilterContext context)
	{
		if (context.Type.IsGenericType && context.Type.GetGenericTypeDefinition() == typeof(ParameterBinder<>))
		{
			var type = context.Type.GetGenericArguments()[0];

			model.Type = "string";
			model.Enum = Enum.GetNames(type).Select(IOpenApiAny (value) => new OpenApiString(value)).ToList();
		}
	}
	#endregion
}
