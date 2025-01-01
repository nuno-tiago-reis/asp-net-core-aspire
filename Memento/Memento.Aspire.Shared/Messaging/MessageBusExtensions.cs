namespace Memento.Aspire.Shared.Messaging;

using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Reflection;
using System.Text.Json;
using System.Text;
using System.Net.Mime;

/// <summary>
/// Implements the necessary methods to add an implementation of <see cref="IMessageBus" /> to the ASP.NET Core dependencies.
/// </summary>
public static class MessageBusExtensions
{
	#region [Extensions]
	/// <summary>
	/// Adds an instance of <see cref="IMessageBus"/> to the specified <seealso cref="IServiceCollection"/>.
	/// Uses the specified <seealso cref="MessageBusOptions"/>
	/// </summary>
	///
	/// <param name="services">The service collection.</param>
	/// <param name="options">The options.</param>
	public static IServiceCollection AddMessageBus(this IServiceCollection services, MessageBusOptions options)
	{
		// Validate the options
		ValidateOptions(options);

		// Register the options
		services.AddSingleton(options);

		// Register the service
		services.AddScoped<IMessageBus, MessageBus>();

		// Register the mass transit service
		services.AddMassTransit((registrationConfigurator) =>
		{
			registrationConfigurator.UsingRabbitMq((context, factoryConfigurator) =>
			{
				factoryConfigurator.Host(options.ConnectionString);
				factoryConfigurator.ConfigureEndpoints(context);
			});

			registrationConfigurator.SetRabbitMqReplyToRequestClientFactory();
			registrationConfigurator.AddConsumers(Assembly.GetEntryAssembly());
		});

		// Register the mass transit mediator
		services.AddMediator((registrationConfigurator) =>
		{
			registrationConfigurator.AddConsumers(Assembly.GetEntryAssembly());
		});

		return services;
	}

	/// <summary>
	/// Adds an instance of <see cref="IMessageBus"/> to the specified <seealso cref="IServiceCollection"/>.
	/// Configures the options using specified <seealso cref="Action{MessageBusOptions}"/>
	/// </summary>
	///
	/// <param name="services">The service collection.</param>
	/// <param name="action">The action that configures the <seealso cref="MessageBusOptions"/>.</param>
	public static IServiceCollection AddMessageBus(this IServiceCollection services, Action<MessageBusOptions> action)
	{
		// Create the options
		var options = new MessageBusOptions
		{
			ConnectionString = null
		};

		// Configure the options
		action?.Invoke(options);

		// Register the service
		services.AddMessageBus(options);

		return services;
	}

	/// <summary>
	/// Registers the <see cref="IMessageBus"/> in the pipeline of the specified <seealso cref="IApplicationBuilder"/>.
	/// Uses the specified <seealso cref="MessageBusOptions"/>
	/// </summary>
	///
	/// <param name="builder">The endpoint route builder.</param>
	/// <param name="options">The options.</param>
	public static IEndpointRouteBuilder UseMessageBus(this IEndpointRouteBuilder builder, MessageBusOptions options)
	{
		// Validate the options
		ValidateOptions(options);

		// Configure the healthcheck options
		builder.MapHealthChecks("/health/masstransit", new HealthCheckOptions
		{
			Predicate = (check) => check.Tags.Contains("masstransit"),
			ResponseWriter = WriteResponse
		});

		return builder;
	}

	/// <summary>
	/// Registers the <see cref="IMessageBus"/> in the pipeline of the specified <seealso cref="IApplicationBuilder"/>.
	/// Configures the options using specified <seealso cref="Action{MessageBusOptions}"/>
	/// </summary>
	///
	/// <param name="builder">The endpoint route builder.</param>
	/// <param name="action">The action that configures the <seealso cref="MessageBusOptions"/>.</param>
	public static IEndpointRouteBuilder UseMessageBus(this IEndpointRouteBuilder builder, Action<MessageBusOptions> action)
	{
		// Create the options
		var options = new MessageBusOptions
		{
			ConnectionString = null
		};

		// Configure the options
		action?.Invoke(options);

		// Register the service
		builder.UseMessageBus(options);

		return builder;
	}
	#endregion

	#region [Helpers]
	/// <summary>
	/// Validates the options.
	/// </summary>
	///
	/// <param name="options">The options.</param>
	private static void ValidateOptions(MessageBusOptions options)
	{
		// Validate the options
		if (options is null)
		{
			throw new ArgumentException($"The {nameof(options)} are invalid.");
		}

		// Validate the host
		if (string.IsNullOrWhiteSpace(options.ConnectionString))
		{
			throw new ArgumentException($"The {nameof(options.ConnectionString)} parameter is invalid.");
		}
	}

	/// <summary>
	/// Writes the healthcheck report as a JSON object.
	/// </summary>
	///
	/// <param name="context">The context.</param>
	/// <param name="healthReport">The health report.</param>
	private static Task WriteResponse(HttpContext context, HealthReport healthReport)
	{
		var options = new JsonWriterOptions
		{
			Indented = true
		};

		using var memoryStream = new MemoryStream();
		using var jsonWriter = new Utf8JsonWriter(memoryStream, options);

		jsonWriter.WriteStartObject();
		jsonWriter.WriteString("status", healthReport.Status.ToString());
		jsonWriter.WriteStartObject("results");

		foreach (var healthReportEntry in healthReport.Entries)
		{
			jsonWriter.WriteStartObject(healthReportEntry.Key);
			jsonWriter.WriteString("status",
				healthReportEntry.Value.Status.ToString());
			jsonWriter.WriteString("description",
				healthReportEntry.Value.Description);
			jsonWriter.WriteStartObject("data");

			foreach (var item in healthReportEntry.Value.Data)
			{
				jsonWriter.WritePropertyName(item.Key);

				JsonSerializer.Serialize(jsonWriter, item.Value,
					item.Value?.GetType() ?? typeof(object));
			}

			jsonWriter.WriteEndObject();
			jsonWriter.WriteEndObject();
		}

		jsonWriter.WriteEndObject();
		jsonWriter.WriteEndObject();

		context.Response.ContentType = MediaTypeNames.Application.Json;

		return context.Response.WriteAsync(
			Encoding.UTF8.GetString(memoryStream.ToArray()));
	}
	#endregion
}
