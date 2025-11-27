namespace Memento.Aspire.Core.Swagger;

using Memento.Aspire.Core.Swagger;
using Memento.Aspire.Core.Swagger.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

/// <summary>
/// Implements the necessary methods to add the <see cref="Swashbuckle"/> middleware to the ASP.NET Core dependencies.
/// </summary>
public static class SwaggerExtensions
{
	#region [Extensions]
	/// <summary>
	/// Adds the necessary <see cref="Swashbuckle"/> dependencies to the specified <seealso cref="IServiceCollection"/>.
	/// Uses the specified <seealso cref="SwaggerOptions"/>
	/// </summary>
	///
	/// <param name="services">The service collection.</param>
	/// <param name="options">The options.</param>
	public static IServiceCollection AddSwagger(this IServiceCollection services, SwaggerOptions options)
	{
		// Validate the options
		ValidateOptions(options);

		// Register the options
		services.AddSingleton(options);

		// Register the swashbuckle generation service
		services.AddSwaggerGen((swashbuckleOptions) =>
		{
			// Documentation
			swashbuckleOptions.SwaggerDoc(options.Version, new OpenApiInfo
			{
				Title = options.Title,
				Version = options.Version
			});
			swashbuckleOptions.SwaggerDocFiles();

			// Security (OpenIdConnect)
			if (options.Security?.OpenIdConnect is not null)
			{
				AddOpenIdConnectSecurity(options.Security.OpenIdConnect, swashbuckleOptions);
			}
			// Security (OAuth)
			else if (options.Security?.OAuth is not null)
			{
				AddOAuthSecurity(options.Security.OAuth, swashbuckleOptions);
			}

			// Operation Filters
			swashbuckleOptions.OperationFilter<AuthorizeOperationFilter>();
			swashbuckleOptions.OperationFilter<ContentTypeOperationFilter>();
			swashbuckleOptions.OperationFilter<CorrelateOperationFilter>();
			swashbuckleOptions.OperationFilter<IdempotentOperationFilter>();

			// Schema Filters
			swashbuckleOptions.SchemaFilter<FluentValidationSchemaFilter>();
			swashbuckleOptions.SchemaFilter<NotNullableWhenRequiredSchemaFilter>();
		});

		return services;
	}

	/// <summary>
	/// Adds the necessary <see cref="Swashbuckle"/> dependencies to the specified <seealso cref="IServiceCollection"/>.
	/// Configures the options using specified <seealso cref="Action{SwaggerOptions}"/>
	/// </summary>
	///
	/// <param name="services">The service collection.</param>
	/// <param name="action">The action that configures the <seealso cref="SwaggerOptions"/>.</param>
	public static IServiceCollection AddSwagger(this IServiceCollection services, Action<SwaggerOptions> action)
	{
		// Create the options
		var options = new SwaggerOptions
		{
			Title = null,
			Version = null,
			Endpoint = null,
			Security = null
		};

		// Configure the options
		action?.Invoke(options);

		// Register the service
		return services.AddSwagger(options);
	}

	/// <summary>
	/// Registers the <see cref="Swashbuckle"/> middleware in the pipeline of the specified <seealso cref="IApplicationBuilder"/>.
	/// Uses the specified <seealso cref="SwaggerOptions"/>
	/// </summary>
	///
	/// <param name="builder">The application builder.</param>
	/// <param name="options">The options.</param>
	public static IApplicationBuilder UseSwagger(this IApplicationBuilder builder, SwaggerOptions options)
	{
		// Validate the options
		ValidateOptions(options);

		// Configure the swashbuckle ui
		builder.UseSwaggerUI((SwaggerUIOptions swashbuckleOptions) =>
		{
			// Configuration
			swashbuckleOptions.SwaggerEndpoint(options.Endpoint, $"{options.Title} {options.Version}");

			// Security (OpenIdConnect)
			if (options.Security?.OpenIdConnect is not null)
			{
				UseOpenIdConnectSecurity(options.Security.OpenIdConnect, swashbuckleOptions);
			}
			// Security (OAuth)
			else if (options.Security?.OAuth is not null)
			{
				UseOAuthSecurity(options.Security.OAuth, swashbuckleOptions);
			}
		});

		// Configure the swashbuckle middleware
		builder.UseSwagger();

		return builder;
	}

	/// <summary>
	/// Registers the <see cref="Swashbuckle"/> in the pipeline of the specified <seealso cref="IApplicationBuilder"/>.
	/// Configures the options using specified <seealso cref="Action{SwaggerOptions}"/>
	/// </summary>
	///
	/// <param name="builder">The application builder.</param>
	/// <param name="action">The action that configures the <seealso cref="SwaggerOptions"/>.</param>
	public static IApplicationBuilder UseSwagger(this IApplicationBuilder builder, Action<SwaggerOptions> action)
	{
		// Create the options
		var options = new SwaggerOptions
		{
			Title = null,
			Version = null,
			Endpoint = null,
			Security = null
		};

		// Configure the options
		action?.Invoke(options);

		// Register the service
		return builder.UseSwagger(options);
	}
	#endregion

	#region [Helpers]
	/// <summary>
	/// Validates the options.
	/// </summary>
	///
	/// <param name="options">The options.</param>
	private static void ValidateOptions(SwaggerOptions options)
	{
		// Validate the options
		if (options is null)
		{
			throw new ArgumentException($"The {nameof(options)} are invalid.");
		}

		// Validate the title
		if (string.IsNullOrWhiteSpace(options.Title))
		{
			throw new ArgumentException($"The {nameof(options.Title)} parameter is invalid.");
		}

		// Validate the version
		if (string.IsNullOrWhiteSpace(options.Version))
		{
			throw new ArgumentException($"The {nameof(options.Version)} parameter is invalid.");
		}

		// Validate the endpoint
		if (string.IsNullOrWhiteSpace(options.Endpoint))
		{
			throw new ArgumentException($"The {nameof(options.Endpoint)} parameter is invalid.");
		}

		// Validate the openid connect options
		if (options.Security?.OpenIdConnect is not null)
		{
			// Validate the bearer audience
			if (string.IsNullOrWhiteSpace(options.Security.OpenIdConnect.Audience))
			{
				throw new ArgumentException($" The {nameof(options.Security)}:{nameof(options.Security.OpenIdConnect)}:{nameof(options.Security.OpenIdConnect.Authority)} parameter is invalid.");
			}

			// Validate the bearer authority
			if (string.IsNullOrWhiteSpace(options.Security.OpenIdConnect.Authority))
			{
				throw new ArgumentException($" The {nameof(options.Security)}:{nameof(options.Security.OpenIdConnect)}:{nameof(options.Security.OpenIdConnect.Authority)} parameter is invalid.");
			}

			// Validate the bearer discovery url
			if (string.IsNullOrWhiteSpace(options.Security.OpenIdConnect.DiscoveryUrl))
			{
				throw new ArgumentException($" The {nameof(options.Security)}:{nameof(options.Security.OpenIdConnect)}:{nameof(options.Security.OpenIdConnect.DiscoveryUrl)} parameter is invalid.");
			}

			// Validate the bearer client identifier
			if (string.IsNullOrWhiteSpace(options.Security.OpenIdConnect.ClientId))
			{
				throw new ArgumentException($" The {nameof(options.Security)}:{nameof(options.Security.OpenIdConnect)}:{nameof(options.Security.OpenIdConnect.ClientId)} parameter is invalid.");
			}

			// Validate the bearer client secret
			if (string.IsNullOrWhiteSpace(options.Security.OpenIdConnect.ClientSecret))
			{
				throw new ArgumentException($" The {nameof(options.Security)}:{nameof(options.Security.OpenIdConnect)}:{nameof(options.Security.OpenIdConnect.ClientSecret)} parameter is invalid.");
			}

			// Validate the bearer scopes
			if (options.Security.OpenIdConnect.Scopes == null)
			{
				throw new ArgumentException($" The {nameof(options.Security)}:{nameof(options.Security.OpenIdConnect)}:{nameof(options.Security.OpenIdConnect.Scopes)} parameter is invalid.");
			}
		}

		// Validate the oauth options
		if (options.Security?.OAuth is not null)
		{
			// Validate the bearer audience
			if (string.IsNullOrWhiteSpace(options.Security.OpenIdConnect.Audience))
			{
				throw new ArgumentException($" The {nameof(options.Security)}:{nameof(options.Security.OpenIdConnect)}:{nameof(options.Security.OpenIdConnect.Authority)} parameter is invalid.");
			}

			// Validate the bearer authority
			if (string.IsNullOrWhiteSpace(options.Security.OpenIdConnect.Authority))
			{
				throw new ArgumentException($" The {nameof(options.Security)}:{nameof(options.Security.OpenIdConnect)}:{nameof(options.Security.OpenIdConnect.Authority)} parameter is invalid.");
			}

			// Validate the bearer authorizatio url
			if (string.IsNullOrWhiteSpace(options.Security.OAuth.AuthorizationUrl))
			{
				throw new ArgumentException($" The {nameof(options.Security)}:{nameof(options.Security.OAuth)}:{nameof(options.Security.OAuth.AuthorizationUrl)} parameter is invalid.");
			}

			// Validate the bearer token url
			if (string.IsNullOrWhiteSpace(options.Security.OAuth.TokenUrl))
			{
				throw new ArgumentException($" The {nameof(options.Security)}:{nameof(options.Security.OAuth)}:{nameof(options.Security.OAuth.TokenUrl)} parameter is invalid.");
			}

			// Validate the bearer client identifier
			if (string.IsNullOrWhiteSpace(options.Security.OAuth.ClientId))
			{
				throw new ArgumentException($" The {nameof(options.Security)}:{nameof(options.Security.OAuth)}:{nameof(options.Security.OAuth.ClientId)} parameter is invalid.");
			}

			// Validate the bearer client secret
			if (string.IsNullOrWhiteSpace(options.Security.OAuth.ClientSecret))
			{
				throw new ArgumentException($" The {nameof(options.Security)}:{nameof(options.Security.OAuth)}:{nameof(options.Security.OAuth.ClientSecret)} parameter is invalid.");
			}

			// Validate the bearer scopes
			if (options.Security.OAuth.Scopes == null)
			{
				throw new ArgumentException($" The {nameof(options.Security)}:{nameof(options.Security.OAuth)}:{nameof(options.Security.OAuth.Scopes)} parameter is invalid.");
			}
		}
	}

	/// <summary>
	/// Includes the swagger documentation files.
	/// </summary>
	///
	/// <param name="swaggerOptions">The options.</param>
	private static void SwaggerDocFiles(this SwaggerGenOptions swaggerOptions)
	{
		foreach (var documentationFile in GetAssemblyDocumentationFiles())
		{
			swaggerOptions.IncludeXmlComments(documentationFile);
		}
	}

	/// <summary>
	/// Gets the loaded assemblies.
	/// </summary>
	private static Assembly[] GetAssemblies()
	{
		var assemblies = AppDomain.CurrentDomain
			.GetAssemblies()
			.Where(assembly => assembly.GetTypes().Any(type => type.FullName?.StartsWith(nameof(Memento), StringComparison.OrdinalIgnoreCase) == true))
			.ToArray();

		return assemblies;
	}

	/// <summary>
	/// Gets the documentation files for the loaded assemblies.
	/// </summary>
	private static string[] GetAssemblyDocumentationFiles()
	{
		var assemblyDocumentationFiles = AppDomain.CurrentDomain
			.GetAssemblies()
			.Where(assembly => assembly.GetTypes().Any(type => type.FullName?.StartsWith(nameof(Memento), StringComparison.OrdinalIgnoreCase) == true))
			.Where(assembly => File.Exists(assembly.Location.Replace("dll", "xml")))
			.Select(assembly => assembly.Location.Replace("dll", "xml"))
			.ToArray();

		return assemblyDocumentationFiles;
	}

	/// <summary>
	/// Adds the OpenIdConnect security configuration to the Swagger middleware.
	/// </summary>
	///
	/// <param name="swaggerSecurityOptions">The swagger security options.</param>
	/// <param name="swashbuckleOptions">The swashbuckle options.</param>
	private static void AddOpenIdConnectSecurity(SwaggerSecurityOpenIdConnectOptions swaggerSecurityOptions, SwaggerGenOptions swashbuckleOptions)
	{
		var securityScheme = new OpenApiSecurityScheme
		{
			In = ParameterLocation.Header,
			Type = SecuritySchemeType.OpenIdConnect,
			Scheme = JwtBearerDefaults.AuthenticationScheme,
			OpenIdConnectUrl = new Uri(swaggerSecurityOptions.DiscoveryUrl)
		};

		swashbuckleOptions.AddSecurityDefinition(nameof(SecuritySchemeType.OpenIdConnect), securityScheme);
	}

	/// <summary>
	/// Adds the OAuth security configuration to the Swagger middleware.
	/// </summary>
	///
	/// <param name="swaggerSecurityOptions">The swagger security options.</param>
	/// <param name="swashbuckleOptions">The swashbuckle options.</param>
	private static void AddOAuthSecurity(SwaggerSecurityOAuthOptions swaggerSecurityOptions, SwaggerGenOptions swashbuckleOptions)
	{
		var securityScheme = new OpenApiSecurityScheme
		{
			In = ParameterLocation.Header,
			Type = SecuritySchemeType.OAuth2,
			Scheme = JwtBearerDefaults.AuthenticationScheme,
			Flows = new OpenApiOAuthFlows
			{
				AuthorizationCode = new OpenApiOAuthFlow
				{
					AuthorizationUrl = new Uri(swaggerSecurityOptions.AuthorizationUrl),
					TokenUrl = new Uri(swaggerSecurityOptions.TokenUrl),
					Scopes = swaggerSecurityOptions.Scopes.ToDictionary
					(
						(scope) => scope.Name,
						(scope) => scope.Description
					)
				},
				Implicit = new OpenApiOAuthFlow
				{
					AuthorizationUrl = new Uri(swaggerSecurityOptions.AuthorizationUrl),
					Scopes = swaggerSecurityOptions.Scopes.ToDictionary
					(
						(scope) => scope.Name,
						(scope) => scope.Description
					)
				}
			}
		};

		swashbuckleOptions.AddSecurityDefinition(nameof(SecuritySchemeType.OAuth2), securityScheme);
	}

	/// <summary>
	/// Uses the OpenIdConnect security configuration in the Swagger middleware.
	/// </summary>
	///
	/// <param name="swaggerSecurityOptions">The swagger options.</param>
	/// <param name="swashbuckleOptions">The swashbuckle options.</param>
	private static void UseOpenIdConnectSecurity(SwaggerSecurityOpenIdConnectOptions swaggerSecurityOptions, SwaggerUIOptions swashbuckleOptions)
	{
		var clientId = swaggerSecurityOptions.ClientId;
		var clientSecret = swaggerSecurityOptions.ClientSecret;
		var additionalQueryParameters = new Dictionary<string, string>
		{
			{ "audience", swaggerSecurityOptions.Audience }
		};

		swashbuckleOptions.OAuthClientId(clientId);
		swashbuckleOptions.OAuthClientSecret(clientSecret);
		swashbuckleOptions.OAuthAdditionalQueryStringParams(additionalQueryParameters);
	}

	/// <summary>
	/// Uses the OAuth security configuration in the Swagger middleware.
	/// </summary>
	///
	/// <param name="swaggerSecurityOptions">The swagger options.</param>
	/// <param name="swashbuckleOptions">The swashbuckle options.</param>
	private static void UseOAuthSecurity(SwaggerSecurityOAuthOptions swaggerSecurityOptions, SwaggerUIOptions swashbuckleOptions)
	{
		var clientId = swaggerSecurityOptions.ClientId;
		var clientSecret = swaggerSecurityOptions.ClientSecret;
		var additionalQueryParameters = new Dictionary<string, string>
		{
			{ "audience", swaggerSecurityOptions.Audience }
		};

		swashbuckleOptions.OAuthClientId(clientId);
		swashbuckleOptions.OAuthClientSecret(clientSecret);
		swashbuckleOptions.OAuthAdditionalQueryStringParams(additionalQueryParameters);
	}
	#endregion
}
