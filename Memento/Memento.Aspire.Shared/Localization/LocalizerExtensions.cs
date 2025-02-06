namespace Memento.Aspire.Shared.Localization;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

/// <summary>
/// Implements the necessary methods to add an implementation of <see cref="ILocalizer" /> to the ASP.NET Core dependencies.
/// </summary>
public static class LocalizerExtensions
{
	#region [Extensions]
	/// <summary>
	/// Adds an instance of <see cref="ILocalizer{T}"/> to the specified <seealso cref="IServiceCollection"/>.
	/// Uses the specified <seealso cref="LocalizerOptions"/>
	/// </summary>
	///
	/// <typeparam name="T">The shared resources type.</typeparam>
	///
	/// <param name="services">The service collection.</param>
	/// <param name="options">The options.</param>
	public static IServiceCollection AddLocalizer<T>(this IServiceCollection services, LocalizerOptions options) where T : class
	{
		// Validate the options
		ValidateOptions(options);

		// Register the options
		services.AddSingleton(options);

		// Register the service
		services.AddScoped<ILocalizer, Localizer<T>>();

		// Register the microsoft service
		services.AddLocalization();

		// Configure the microsoft options
		services.Configure<RequestLocalizationOptions>((localizationOptions) =>
		{
			var defaultCulture = new RequestCulture(options.DefaultCulture);
			var supportedCultures = options.SupportedCultures.Select(culture => new CultureInfo(culture)).ToList();

			localizationOptions.DefaultRequestCulture = defaultCulture;
			localizationOptions.SupportedCultures = supportedCultures;
			localizationOptions.SupportedUICultures = supportedCultures;
		});

		return services;
	}

	/// <summary>
	/// Adds an instance of <see cref="ILocalizer{T}"/> to the specified <seealso cref="IServiceCollection"/>.
	/// Configures the options using specified <seealso cref="Action{LocalizerOptions}"/>
	/// </summary>
	///
	/// <typeparam name="T">The shared resources type.</typeparam>
	///
	/// <param name="services">The service collection.</param>
	/// <param name="action">The action that configures the <seealso cref="LocalizerOptions"/>.</param>
	public static IServiceCollection AddLocalizer<T>(this IServiceCollection services, Action<LocalizerOptions> action) where T : class
	{
		// Create the options
		var options = new LocalizerOptions
		{
			DefaultCulture = LocalizerOptions.DefaultDefaulCulture,
			SupportedCultures = LocalizerOptions.DefaultSupportedCultures
		};

		// Configure the options
		action?.Invoke(options);

		// Register the service
		services.AddLocalizer<T>(options);

		return services;
	}

	/// <summary>
	/// Registers the <see cref="ILocalizer{T}"/> in the pipeline of the specified <seealso cref="IApplicationBuilder"/>.
	/// Uses the specified <seealso cref="LocalizerOptions"/>
	/// </summary>
	///
	/// <param name="builder">The application builder.</param>
	/// <param name="options">The options.</param>
	public static IApplicationBuilder UseLocalizer(this IApplicationBuilder builder, LocalizerOptions options)
	{
		// Validate the options
		ValidateOptions(options);

		// Configure the localization options
		builder.UseRequestLocalization((localizationOptions) =>
		{
			var defaultCulture = new RequestCulture(options.DefaultCulture);
			var supportedCultures = options.SupportedCultures.Select(culture => new CultureInfo(culture)).ToList();

			localizationOptions.DefaultRequestCulture = defaultCulture;
			localizationOptions.SupportedCultures = supportedCultures;
		});

		return builder;
	}

	/// <summary>
	/// Registers the <see cref="ILocalizer{T}"/> in the pipeline of the specified <seealso cref="IApplicationBuilder"/>.
	/// Configures the options using specified <seealso cref="Action{LocalizerOptions}"/>
	/// </summary>
	///
	/// <param name="builder">The application builder.</param>
	/// <param name="action">The action that configures the <seealso cref="LocalizerOptions"/>.</param>
	public static IApplicationBuilder UseLocalizer(this IApplicationBuilder builder, Action<LocalizerOptions> action)
	{
		// Create the options
		var options = new LocalizerOptions
		{
			DefaultCulture = LocalizerOptions.DefaultDefaulCulture,
			SupportedCultures = LocalizerOptions.DefaultSupportedCultures
		};

		// Configure the options
		action?.Invoke(options);

		// Register the service
		builder.UseLocalizer(options);

		return builder;
	}
	#endregion

	#region [Helpers]
	/// <summary>
	/// Validates the options.
	/// </summary>
	///
	/// <param name="options">The options.</param>
	private static void ValidateOptions(LocalizerOptions options)
	{
		// Validate the options
		if (options is null)
		{
			throw new ArgumentException($"The {nameof(options)} are invalid.");
		}

		// Validate the default culture
		if (string.IsNullOrWhiteSpace(options.DefaultCulture))
		{
			throw new ArgumentException($"The {nameof(options.DefaultCulture)} parameter is invalid.");
		}

		// Validate the supported cultures
		if (options.SupportedCultures is null || options.SupportedCultures.Length == 0)
		{
			throw new ArgumentException($"The {nameof(options.SupportedCultures)} parameter is invalid.");
		}
	}
	#endregion
}
