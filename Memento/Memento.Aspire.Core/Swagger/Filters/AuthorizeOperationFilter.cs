namespace Memento.Aspire.Core.Swagger.Filters;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

/// <summary>
/// Implements an <see cref="IOperationFilter" />.
/// Applies the authorization requirement conditionally based on the configuration.
/// </summary>
public sealed class AuthorizeOperationFilter : IOperationFilter
{
	#region [Properties]
	/// <summary>
	/// The service provider.
	/// </summary>
	private readonly IServiceProvider ServiceProvider;
	#endregion

	#region [Constructors]
	/// <summary>
	/// Initializes a new instance of the <see cref="AuthorizeOperationFilter"/> class.
	/// </summary>
	///
	/// <param modelName="serviceProvider">The service provider.</param>
	public AuthorizeOperationFilter(IServiceProvider serviceProvider)
	{
		this.ServiceProvider = serviceProvider;
	}
	#endregion

	#region [Methods]
	/// <inheritdoc />
	public void Apply(OpenApiOperation operation, OperationFilterContext context)
	{
		// Check if the controller/method/handler has the authorize attribute
		var hasAuthorizeAttribute = context.ApiDescription.ActionDescriptor.EndpointMetadata.Any((metadata) => metadata is AuthorizeAttribute);
		var hasAllowAnonymousAttribute = context.ApiDescription.ActionDescriptor.EndpointMetadata.Any((metadata) => metadata is AllowAnonymousAttribute);

		if (hasAuthorizeAttribute && !hasAllowAnonymousAttribute)
		{
			// Try to get the internal server error response
			if (operation.Responses.TryGetValue("500", out var response))
			{
				// Match the unauthorized error response with the internal server error response
				var unauthorizedResponse = new OpenApiResponse
				{
					Description = "Unauthorized",
					Content = response.Content,
					Headers = response.Headers,
					Extensions = response.Extensions,
					Links = response.Links,
					Reference = response.Reference,
					UnresolvedReference = response.UnresolvedReference
				};

				operation.Responses.TryAdd("401", unauthorizedResponse);

				// Match the forbidden error response with the internal server error response
				var forbiddenResponse = new OpenApiResponse
				{
					Description = "Forbidden",
					Content = response.Content,
					Headers = response.Headers,
					Extensions = response.Extensions,
					Links = response.Links,
					Reference = response.Reference,
					UnresolvedReference = response.UnresolvedReference
				};

				operation.Responses.TryAdd("403", forbiddenResponse);
			}

			// Add the security requirement
			var securityOptions = this.ServiceProvider.GetService<SwaggerOptions>()?.Security;
			var securityRequirement = new OpenApiSecurityRequirement();

			if (securityOptions?.OpenIdConnect is not null)
			{
				var securityScheme = new OpenApiSecurityScheme
				{
					Reference = new OpenApiReference
					{
						Id = nameof(SecuritySchemeType.OpenIdConnect),
						Type = ReferenceType.SecurityScheme
					}
				};
				var securityScopes = Array.Empty<string>();

				securityRequirement.Add(securityScheme, securityScopes);
			}
			else if (securityOptions?.OAuth is not null)
			{
				var securityScheme = new OpenApiSecurityScheme
				{
					Reference = new OpenApiReference
					{
						Id = nameof(SecuritySchemeType.OAuth2),
						Type = ReferenceType.SecurityScheme
					}
				};
				var securityScopes = Array.Empty<string>();

				securityRequirement.Add(securityScheme, securityScopes);
			}

			operation.Security.Add(securityRequirement);
		}
	}
	#endregion
}
