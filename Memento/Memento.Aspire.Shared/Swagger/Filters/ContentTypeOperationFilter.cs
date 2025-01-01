namespace Memento.Aspire.Shared.Swagger.Filters;

using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

/// <summary>
/// Implements an <see cref="IOperationFilter" />.
/// Applies the content type conditionally based on the configuration.
/// </summary>
public sealed class ContentTypeOperationFilter : IOperationFilter
{
	#region [Methods]
	/// <inheritdoc />
	public void Apply(OpenApiOperation operation, OperationFilterContext context)
	{
		if (operation.RequestBody != null)
		{
			foreach (var key in operation.RequestBody.Content.Keys.Where((key) => !key.Contains("json")))
			{
				operation.RequestBody.Content.Remove(key);
			}
		}

		if (operation.Responses != null)
		{
			foreach (var (_, response) in operation.Responses)
			{
				foreach (var key in response.Content.Keys.Where((key) => !key.Contains("json")))
				{
					response.Content.Remove(key);
				}
			}
		}
	}
	#endregion
}
