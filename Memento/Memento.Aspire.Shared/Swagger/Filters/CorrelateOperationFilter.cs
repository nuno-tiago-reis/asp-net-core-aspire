namespace Memento.Aspire.Shared.Swagger.Filters;

using Memento.Aspire.Shared.Api;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

/// <summary>
/// Implements an <see cref="IOperationFilter" />.
/// Applies the correlation requirement conditionally based on the configuration.
/// </summary>
public sealed class CorrelateOperationFilter : IOperationFilter
{
	#region [Methods]
	/// <inheritdoc />
	public void Apply(OpenApiOperation operation, OperationFilterContext context)
	{
		// Check if the controller/method/handler has the correlation attribute
		var hasCorrelationAttribute = context.ApiDescription.ActionDescriptor.EndpointMetadata.Any((metadata) => metadata is CorrelateAttribute);

		if (hasCorrelationAttribute)
		{
			operation.Parameters.Add(new OpenApiParameter
			{
				Name = CorrelationExtensions.HeaderName,
				Description = string.Format("The '{0}' is used to ensure requests are traceable across multiple systems.", CorrelationExtensions.HeaderName),
				In = ParameterLocation.Header,
				Required = false,
				Schema = new OpenApiSchema
				{
					Type = "string",
					Format = "uuid",
					Default = new OpenApiString(Guid.NewGuid().ToString())
				}
			});
		}
	}
	#endregion
}
