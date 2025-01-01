namespace Memento.Aspire.Shared.Messaging;

using Memento.Aspire.Shared.Messaging.RequestResponse;

/// <summary>
/// Defines a generic interface for a message bus.
/// Provides methods to send messages and receive message results.
/// </summary>
public interface IMessageBus
{
	#region [Methods]
	/// <summary>
	/// Dispatches a message and returns a message result using the message bus.
	/// </summary>
	///
	/// <param name="message">The message.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	Task FireAndForgetViaBusAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
		where TMessage : class;

	/// <summary>
	/// Dispatches a message and returns a message result using the message bus.
	/// </summary>
	///
	/// <param name="message">The message.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	///
	/// <returns>A message result</returns>
	Task<TMessageResult> RequestResponseViaBusAsync<TMessage, TMessageResult>(TMessage message, CancellationToken cancellationToken = default)
		where TMessage : Message<TMessageResult>
		where TMessageResult : MessageResult;

	/// <summary>
	/// Dispatches a message and returns a message result using the message mediator.
	/// </summary>
	///
	/// <param name="message">The message.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	Task FireAndForgetViaMediatorAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
		where TMessage : class;

	/// <summary>
	/// Dispatches a message and returns a message result using the message mediator.
	/// </summary>
	///
	/// <param name="message">The message.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	///
	/// <returns>A message result</returns>
	Task<TMessageResult> RequestResponseViaMediatorAsync<TMessage, TMessageResult>(TMessage message, CancellationToken cancellationToken = default)
		where TMessage : Message<TMessageResult>
		where TMessageResult : MessageResult;
	#endregion
}
