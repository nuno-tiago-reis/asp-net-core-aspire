namespace Memento.Aspire.Core.Swagger.Filters;

using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

/// <summary>
/// Implements an <see cref="ISchemaFilter" />.
/// Applies the necessary restrictions based on the required fields.
/// </summary>
public sealed class NotNullableWhenRequiredSchemaFilter : ISchemaFilter
{
	#region [Methods]
	/// <inheritdoc />
	public void Apply(OpenApiSchema schema, SchemaFilterContext context)
	{
		schema.Required ??= new HashSet<string>();

		foreach (var key in schema.Properties.Keys)
		{
			if (schema.Required.Contains(key))
			{
				schema.Properties[key].Nullable = false;
			}
		}
	}
	#endregion
}
