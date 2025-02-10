namespace Memento.Aspire.Shared.Messaging.Events;

using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading;

/// <summary>
/// Implements a generic event handler.
/// Provides methods to handle events.
/// </summary>
public abstract class EventHandler<TEvent> : IConsumer<TEvent>
	where TEvent : Event
{
	#region [Properties]
	/// <summary>
	/// The logger.
	/// </summary>
	protected ILogger Logger { get; init; }
	#endregion

	#region [Constructors]
	/// <summary>
	/// Initializes a new instance of the <see cref="EventHandler{TEvent}"/> class.
	/// </summary>
	///
	/// <param name="logger">The logger.</param>
	public EventHandler(ILogger<EventHandler<TEvent>> logger)
	{
		this.Logger = logger;
	}
	#endregion

	#region [Methods]
	/// <summary>
	/// Consumes the event.
	/// </summary>
	///
	/// <param name="context">The context.</param>
	public async Task Consume(ConsumeContext<TEvent> context)
	{
		// Create a logging scope
		using var _ = this.Logger.BeginScope("{CorrelationId}", context.Message.CorrelationId);

		// Log the event
		this.Logger.LogInformation("Handling {Message}", context.Message);

		try
		{
			// Handle the event and obtain the result
			await this.HandleEventAsync(context.Message, context.CancellationToken);

			// Log the event
			this.Logger.LogInformation("Handled Successfully {Message}", context.Message);
		}
		catch (Exception exception)
		{
			// Log the event
			this.Logger.LogError("Handled Unsuccessfully {Message} {Exception}", context.Message, exception);
		}
	}

	/// <summary>
	/// Handles the event.
	/// </summary>
	///
	/// <param name="event">The event.</param>
	/// <param name="cancellationToken"></param>
	protected abstract Task HandleEventAsync(TEvent @event, CancellationToken cancellationToken = default);
	#endregion
}
