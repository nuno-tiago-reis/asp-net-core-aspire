namespace Memento.Aspire.Shared.ApplicationInsights;

using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Implements the necessary methods to add the <see cref="ApplicationInsights"/> middleware to the ASP.NET Core dependencies.
/// </summary>
public static class ApplicationInsightsExtensions
{
	#region [Extensions]
	/// <summary>
	/// Adds the necessary <see cref="ApplicationInsights"/> dependencies to the specified <seealso cref="IServiceCollection"/>.
	/// Uses the specified <seealso cref="ApplicationInsightsOptions"/>
	/// </summary>
	///
	/// <param name="services">The service collection.</param>
	/// <param name="options">The options.</param>
	public static IServiceCollection AddApplicationInsights(this IServiceCollection services, ApplicationInsightsOptions options)
	{
		// Validate the options
		ValidateOptions(options);

		// Register the options
		services.AddSingleton(options);

		// Register the application insights service
		services.AddApplicationInsightsTelemetry(options);

		return services;
	}

	/// <summary>
	/// Adds the necessary <see cref="ApplicationInsights"/> dependencies to the specified <seealso cref="IServiceCollection"/>.
	/// Configures the options using specified <seealso cref="Action{ApplicationInsightsOptions}"/>
	/// </summary>
	///
	/// <param name="services">The service collection.</param>
	/// <param name="action">The action that configures the <seealso cref="ApplicationInsightsOptions"/>.</param>
	public static IServiceCollection AddApplicationInsights(this IServiceCollection services, Action<ApplicationInsightsOptions> action)
	{
		// Create the options
		var options = new ApplicationInsightsOptions();

		// Configure the options
		action?.Invoke(options);

		// Register the service
		return services.AddApplicationInsights(options);
	}
	#endregion

	#region [Helpers]
	/// <summary>
	/// Validates the options.
	/// </summary>
	///
	/// <param name="options">The options.</param>
	private static void ValidateOptions(ApplicationInsightsOptions options)
	{
		// Validate the options
		if (options is null)
		{
			throw new ArgumentException($"The {nameof(options)} are invalid.");
		}

		// Validate the connection string
		if (string.IsNullOrWhiteSpace(options.ConnectionString))
		{
			throw new ArgumentException($"The {nameof(options.ConnectionString)} parameter is invalid.");
		}
	}
	#endregion
}
