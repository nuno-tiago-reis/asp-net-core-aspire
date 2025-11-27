namespace Memento.Aspire.Domain.Api;

using Carter;
using FluentValidation;
using Memento.Aspire.Domain.Api.Configuration;
using Memento.Aspire.Domain.Api.Validators.Author;
using Memento.Aspire.Domain.Api.Validators.Book;
using Memento.Aspire.Domain.Api.Validators.Genre;
using Memento.Aspire.Domain.Service.Contracts.Author;
using Memento.Aspire.Domain.Service.Contracts.Book;
using Memento.Aspire.Domain.Service.Contracts.Genre;
using Memento.Aspire.Core.Api;
using Memento.Aspire.Core.Cache;
using Memento.Aspire.Core.Extensions;
using Memento.Aspire.Core.Localization;
using Memento.Aspire.Core.Logging;
using Memento.Aspire.Core.Messaging;
using Memento.Aspire.Core.Resources;
using Memento.Aspire.Core.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System;
using System.Security.Claims;

using HttpJsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;
using MvcJsonOptions = Microsoft.AspNetCore.Mvc.JsonOptions;

/// <summary>
/// Implements the hosting program.
/// </summary>
internal sealed class Program
{
	/// <summary>
	/// Bootstraps the application.
	/// </summary>
	///
	/// <param name="arguments">The arguments.</param>
	internal static void Main(string[] arguments)
	{
		#region [Builder]
		// Builder
		var builder = WebApplication.CreateBuilder(arguments);

		// Builder (ApplicationSettings)
		var applicationSettings = builder.Configuration.Get<ApplicationSettings>()!;

		// Builder (ASP.NET)
		builder.Services
			.AddAuthorization()
			.AddExceptionHandler<UnhandledExceptionHandler>()
			.AddCors();

		builder.Services
			.AddAuthentication((options) =>
			{
				options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer((options) =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					NameClaimType = ClaimTypes.NameIdentifier,
					ValidateAudience = true,
					ValidateIssuer = true,
					ValidateLifetime = false
				};

				if (applicationSettings.Swagger.Security?.OpenIdConnect is not null)
				{
					options.Audience = applicationSettings.Swagger.Security.OpenIdConnect.Audience;
					options.Authority = applicationSettings.Swagger.Security.OpenIdConnect.Authority;
					options.TokenValidationParameters.ValidAudience = applicationSettings.Swagger.Security.OpenIdConnect.Audience;
					options.TokenValidationParameters.ValidIssuer = applicationSettings.Swagger.Security.OpenIdConnect.Authority;
				}
				else if (applicationSettings.Swagger.Security?.OAuth is not null)
				{
					options.Audience = applicationSettings.Swagger.Security.OAuth.Audience;
					options.Authority = applicationSettings.Swagger.Security.OAuth.Authority;
					options.TokenValidationParameters.ValidAudience = applicationSettings.Swagger.Security.OAuth.Audience;
					options.TokenValidationParameters.ValidIssuer = applicationSettings.Swagger.Security.OAuth.Authority;
				}
			});

		builder.Services
			.Configure<ApplicationSettings>(builder.Configuration);
		builder.Services
			.Configure<HttpJsonOptions>((options) =>
			{
				options.SerializerOptions.ConfigureDefaultOptions();
			});
		builder.Services
			.Configure<MvcJsonOptions>((options) =>
			{
				options.JsonSerializerOptions.ConfigureDefaultOptions();
			});
		builder.Services
			.Configure<RouteOptions>((options) =>
			{
				options.AppendTrailingSlash = false;
				options.LowercaseUrls = true;
			});

		// Builder (Controllers)
		builder.Services
			.AddControllers((options) =>
			{
				options.Conventions.Add(new RouteTokenTransformerConvention(new SlashParameterTransformer()));
			})
			.AddModelValidationProcessing();

		// Builder (MinimalApis)
		builder.Services
			.AddCarter();

		// Builder (Caching)
		builder.Services
			.AddCache(applicationSettings.Cache);

		// Builder (Localization)
		builder.Services
			.AddLocalizer<SharedResources>(applicationSettings.Localizer);

		// Builder (Logging)
		builder.Services
			.AddLogging(applicationSettings.Logging);

		// Builder (Mapping)
		builder.Services
			.AddAutoMapper(AppDomain.CurrentDomain.GetMementoAssemblies());

		// Builder (Messaging)
		builder.Services
			.AddMessageBus(applicationSettings.MessageBus);

		// Builder (Swagger)
		builder.Services
			.AddSwagger(applicationSettings.Swagger);

		// Builder (Validation)
		builder.Services
			.AddSingleton<IValidator<AuthorFormContract>, AuthorFormContractValidator>()
			.AddSingleton<IValidator<BookFormContract>, BookFormContractValidator>()
			.AddSingleton<IValidator<GenreFormContract>, GenreFormContractValidator>();
		#endregion

		#region [Application]
		// Application
		var application = builder.Build();

		// Application (Logging)
		application
			.UseLogging(applicationSettings.Logging);

		// Application (ASP.NET)
		application
			.UseAuthentication();
		application
			.UseAuthorization();
		application
			.UseExceptionHandler((configurator) =>
			{
				// Intentionally Empty.
				// https://github.com/dotnet/aspnetcore/issues/51888
			});
		application
			.UseHttpsRedirection();
		application
			.UseCors();

		// Application (Controllers)
		application
			.MapControllers();

		// Application (MinimalApis)
		application
			.MapCarter();

		// Application (Localization)
		application
			.UseLocalizer(applicationSettings.Localizer);

		// Application (Messaging)
		application
			.UseMessageBus(applicationSettings.MessageBus);

		// Application (Swagger)
		application
			.UseSwagger(applicationSettings.Swagger);
		#endregion

		#region [Runner]
		try
		{
			// Logger
			builder.CreateLogger();

			// Runner
			application.Run();
		}
		catch (Exception exception)
		{
			Log.Fatal(exception, "The application terminated unexpectedly.");
		}
		finally
		{
			Log.CloseAndFlush();
		}
		#endregion
	}
}
