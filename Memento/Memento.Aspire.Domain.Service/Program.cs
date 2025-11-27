namespace Memento.Aspire.Domain.Service;

using Memento.Aspire.Domain.Service.Configuration;
using Memento.Aspire.Domain.Service.Persistence;
using Memento.Aspire.Domain.Service.Persistence.Entities.Author;
using Memento.Aspire.Domain.Service.Persistence.Entities.Book;
using Memento.Aspire.Domain.Service.Persistence.Entities.Genre;
using Memento.Aspire.Core.Extensions;
using Memento.Aspire.Core.Localization;
using Memento.Aspire.Core.Logging;
using Memento.Aspire.Core.Messaging;
using Memento.Aspire.Core.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Reflection;

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

		// Builder (EntityFramework)
		builder.Services
			.AddDbContext<DomainContext>((databaseOptionsProvider) =>
			{
				databaseOptionsProvider.UseSqlServer(applicationSettings.Database.ConnectionString, (databaseOptionsBuilder) =>
				{
					databaseOptionsBuilder.MigrationsAssembly(Assembly.GetExecutingAssembly());
				});
			})
			.AddTransient<DomainSeeder>();

		builder.Services
			.AddTransient<IAuthorRepository, AuthorRepository>()
			.AddTransient<IBookRepository, BookRepository>()
			.AddTransient<IGenreRepository, GenreRepository>();

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
		#endregion

		#region [Application]
		// Application
		var application = builder.Build();

		// Application (ASP.NET)
		application
			.UseExceptionHandler((configurator) =>
			{
				// Intentionally Empty.
				// https://github.com/dotnet/aspnetcore/issues/51888
			});
		application
			.UseHttpsRedirection();

		// Application (EntityFramework)
		using (var scope = application.Services.CreateScope())
		{
			var context = scope.ServiceProvider.GetService<DomainContext>()!;
			context.Database.Migrate();

			var seeder = scope.ServiceProvider.GetService<DomainSeeder>()!;
			seeder.Seed();
		}

		// Application (Localization)
		application
			.UseLocalizer(applicationSettings.Localizer);

		// Application (Messaging)
		application
			.UseMessageBus(applicationSettings.MessageBus);
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
