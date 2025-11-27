namespace Memento.Aspire.Core.Logging;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System.Net.Mime;

/// <summary>
/// Implements the necessary methods to add the <see cref="Serilog"/> middleware to the ASP.NET Core dependencies.
/// </summary>
public static class LoggingExtensions
{
	#region [Properties]
	/// <summary>
	/// The default console theme.
	/// </summary>
	public static readonly AnsiConsoleTheme ConsoleTheme = new (new Dictionary<ConsoleThemeStyle, string>
	{
		[ConsoleThemeStyle.Text] = "\x001B[38;5;0015m",
		[ConsoleThemeStyle.SecondaryText] = "\x001B[38;5;0222m",
		[ConsoleThemeStyle.TertiaryText] = "\x001B[38;5;0015m",
		[ConsoleThemeStyle.Invalid] = "\x001B[38;5;0011m",
		[ConsoleThemeStyle.Null] = "\x001B[38;5;0027m",
		[ConsoleThemeStyle.Name] = "\x001B[38;5;0136m",
		[ConsoleThemeStyle.String] = "\x001B[38;5;0075m",
		[ConsoleThemeStyle.Number] = "\x001B[38;5;0208m",
		[ConsoleThemeStyle.Boolean] = "\x001B[38;5;0027m",
		[ConsoleThemeStyle.Scalar] = "\x001B[38;5;0085m",
		[ConsoleThemeStyle.LevelVerbose] = "\x001B[38;5;0231m",
		[ConsoleThemeStyle.LevelDebug] = "\x001B[38;5;0224m",
		[ConsoleThemeStyle.LevelInformation] = "\x001B[38;5;0076m",
		[ConsoleThemeStyle.LevelWarning] = "\x001B[38;5;0220m",
		[ConsoleThemeStyle.LevelError] = "\x001B[38;5;0015m\x001B[48;5;0160m",
		[ConsoleThemeStyle.LevelFatal] = "\x001B[38;5;0015m\x001B[48;5;0088m"
	});
	#endregion

	#region [Extensions]
	/// <summary>
	/// Creates the <see cref="Serilog"/> Logger.
	/// </summary>
	///
	/// <param name="builder">The host application builder.</param>
	public static ILogger CreateLogger(this IHostApplicationBuilder builder)
	{
		Serilog.Debugging.SelfLog.Enable(Console.Out);

		var loggerConfiguration = new LoggerConfiguration()
			.ReadFrom.Configuration(builder.Configuration);

		Log.Logger = loggerConfiguration
			.CreateLogger();

		var loggerLevel = Enum
			.GetValues<LogEventLevel>()
			.Cast<LogEventLevel>()
			.Where(Log.IsEnabled)
			.FirstOrDefault();

		Log.Logger
			.ForContext(typeof(LoggingExtensions))
			.Information("Running Serilog (Minimum Log Level: {LogLevel})", loggerLevel);

		return Log.Logger;
	}

	/// <summary>
	/// Adds the necessary <see cref="Serilog"/> dependencies to the specified <seealso cref="IServiceCollection"/>.
	/// Uses the specified <seealso cref="SwaggerOptions"/>
	/// </summary>
	///
	/// <param name="services">The service collection.</param>
	/// <param name="options">The options.</param>
	public static IServiceCollection AddLogging(this IServiceCollection services, LoggingOptions options)
	{
		// Validate the options
		ValidateOptions(options);

		// Register the options
		services.AddSingleton(options);

		// Register the http context accessor
		services.AddHttpContextAccessor();

		// Register the http logger
		services.AddHttpLogging((logging) =>
		{
			logging.LoggingFields = options.HttpLogging!.Fields!.Value;
			logging.MediaTypeOptions.AddText(MediaTypeNames.Application.Json);
			logging.RequestBodyLogLimit = int.MaxValue;
			logging.ResponseBodyLogLimit = int.MaxValue;
			logging.CombineLogs = false;
		});

		// Register the http logger interceptor
		services.AddHttpLoggingInterceptor<LoggingInterceptor>();

		// Register the serilog logger
		services.AddSerilog();

		return services;
	}

	/// <summary>
	/// Adds the necessary <see cref="Serilog"/> dependencies to the specified <seealso cref="IServiceCollection"/>.
	/// Configures the options using specified <seealso cref="Action{SerilogOptions}"/>
	/// </summary>
	///
	/// <param name="services">The service collection.</param>
	/// <param name="action">The action that configures the <seealso cref="LoggingOptions"/>.</param>
	public static IServiceCollection AddLogging(this IServiceCollection services, Action<LoggingOptions> action)
	{
		// Create the options
		var options = new LoggingOptions
		{
			HttpLogging = null
		};

		// Configure the options
		action?.Invoke(options);

		// Register the service
		return services.AddLogging(options);
	}

	/// <summary>
	/// Registers the <see cref="ILogging{T}"/> in the pipeline of the specified <seealso cref="IApplicationBuilder"/>.
	/// Uses the specified <seealso cref="LoggingOptions"/>
	/// </summary>
	///
	/// <param name="builder">The application builder.</param>
	/// <param name="options">The options.</param>
	public static IApplicationBuilder UseLogging(this IApplicationBuilder builder, LoggingOptions options)
	{
		// Validate the options
		ValidateOptions(options);

		// Configure the http logging
		builder.UseHttpLogging();

		return builder;
	}

	/// <summary>
	/// Registers the <see cref="ILogging{T}"/> in the pipeline of the specified <seealso cref="IApplicationBuilder"/>.
	/// Configures the options using specified <seealso cref="Action{LoggingOptions}"/>
	/// </summary>
	///
	/// <param name="builder">The application builder.</param>
	/// <param name="action">The action that configures the <seealso cref="LoggingOptions"/>.</param>
	public static IApplicationBuilder UseLogging(this IApplicationBuilder builder, Action<LoggingOptions> action)
	{
		// Create the options
		var options = new LoggingOptions
		{
			HttpLogging = null
		};

		// Configure the options
		action?.Invoke(options);

		// Register the service
		builder.UseLogging(options);

		return builder;
	}
	#endregion

	#region [Helpers]
	/// <summary>
	/// Validates the options.
	/// </summary>
	///
	/// <param name="options">The options.</param>
	private static void ValidateOptions(LoggingOptions options)
	{
		// Validate the options
		if (options is null)
		{
			throw new ArgumentException($"The {nameof(options)} are invalid.");
		}

		// Validate the http logging options
		if (options.HttpLogging is not null)
		{

			// Validate the fields
			if (options.HttpLogging.Fields is null)
			{
				throw new ArgumentException($"The {nameof(options.HttpLogging.Fields)} parameter is invalid.");
			}
		}
	}
	#endregion
}
