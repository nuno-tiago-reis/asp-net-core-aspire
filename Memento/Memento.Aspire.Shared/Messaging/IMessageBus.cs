namespace Memento.Aspire.Shared.Messaging;

using Memento.Aspire.Shared.Messaging.Events;
using Memento.Aspire.Shared.Messaging.Messages;

/// <summary>
/// Defines a generic interface for a event bus.
/// Provides methods to send messages and receive event results.
/// </summary>
public interface IMessageBus
{
	#region [Methods]
	/// <summary>
	/// Dispatches an event using the event bus (fire and forget).
	/// </summary>
	///
	/// <param name="event">The event.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	Task DispatchEventViaBusAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
		where TEvent : Event;

	/// <summary>
	/// Dispatches an event using the mediator (fire and forget).
	/// </summary>
	///
	/// <param name="event">The event.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	Task DispatchEventViaMediatorAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
		where TEvent : Event;

	/// <summary>
	/// Dispatches a message and returns a message result using the message bus (request response).
	/// </summary>
	///
	/// <param name="message">The message.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	///
	/// <returns>A message result</returns>
	Task<TMessageResult> DispatchMessageViaBusAsync<TMessage, TMessageResult>(TMessage message, CancellationToken cancellationToken = default)
		where TMessage : Message<TMessageResult>
		where TMessageResult : MessageResult;

	/// <summary>
	/// Dispatches a message and returns a message result using the message mediator (request response).
	/// </summary>
	///
	/// <param name="message">The message.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	///
	/// <returns>A message result</returns>
	Task<TMessageResult> DispatchMessageViaMediatorAsync<TMessage, TMessageResult>(TMessage message, CancellationToken cancellationToken = default)
		where TMessage : Message<TMessageResult>
		where TMessageResult : MessageResult;
	#endregion
}
