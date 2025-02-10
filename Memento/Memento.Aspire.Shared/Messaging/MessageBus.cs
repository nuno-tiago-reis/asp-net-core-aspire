namespace Memento.Aspire.Shared.Messaging;

using MassTransit;
using MassTransit.Mediator;
using Memento.Aspire.Shared.Messaging.Messages;
using Microsoft.Extensions.DependencyInjection;
using System;

using Event = Events.Event;

/// <summary>
/// Implements the generic interface for a event bus.
/// Provides methods to send messages and receive event results.
/// </summary>
public sealed class MessageBus : IMessageBus
{
	#region [Properties]
	/// <summary>
	/// The bus.
	/// </summary>
	private readonly IBus Bus;
	/// <summary>
	/// The mediator.
	/// </summary>
	private readonly IMediator Mediator;

	/// <summary>
	/// The service scope factory.
	/// </summary>
	private readonly IServiceScopeFactory ServiceScopeFactory;
	#endregion

	#region [Constructors]
	/// <summary>
	/// Initializes a new instance of the <see cref="MessageBus"/> class.
	/// </summary>
	///
	/// <param name="bus">The bus.</param>
	/// <param name="mediator">The mediator.</param>
	/// <param name="serviceScopeFactory">The service scope factory.</param>
	public MessageBus(IBus bus, IMediator mediator, IServiceScopeFactory serviceScopeFactory)
	{
		this.Bus = bus;
		this.Mediator = mediator;
		this.ServiceScopeFactory = serviceScopeFactory;
	}
	#endregion

	#region [Methods]
	/// <summary>
	/// Dispatches an event using the event bus (fire and forget).
	/// </summary>
	///
	/// <param name="event">The event.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	public async Task DispatchEventViaBusAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
		where TEvent : Event
	{
		await this.Bus.Publish(@event, cancellationToken);
	}

	/// <summary>
	/// Dispatches an event using the mediator (fire and forget).
	/// </summary>
	///
	/// <param name="event">The event.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	public async Task DispatchEventViaMediatorAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
		where TEvent : Event
	{
		await this.Mediator.Publish(@event, cancellationToken);
	}

	/// <summary>
	/// Dispatches a message and returns a message result using the message bus (request response).
	/// </summary>
	///
	/// <param name="message">The message.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	///
	/// <returns>A message result</returns>
	public async Task<TMessageResult> DispatchMessageViaBusAsync<TMessage, TMessageResult>(TMessage message, CancellationToken cancellationToken = default)
		where TMessage : Message<TMessageResult>
		where TMessageResult : MessageResult
	{
		using var scope = this.ServiceScopeFactory.CreateScope();

		var clientType = typeof(IRequestClient<>).MakeGenericType(typeof(TMessage));
		var clientImplementation = scope.ServiceProvider.GetService(clientType);

		if (clientImplementation is not IRequestClient<TMessage> requestClient)
		{
			throw new InvalidOperationException($"Could not create a clientImplementation for [{clientType}]");
		}

		return (await requestClient.GetResponse<TMessageResult>(message, cancellationToken)).Message;
	}

	/// <summary>
	/// Dispatches a message and returns a message result using the message mediator (request response).
	/// </summary>
	///
	/// <param name="message">The message.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	///
	/// <returns>A message result</returns>
	public async Task<TMessageResult> DispatchMessageViaMediatorAsync<TMessage, TMessageResult>(TMessage message, CancellationToken cancellationToken = default)
		where TMessage : Message<TMessageResult>
		where TMessageResult : MessageResult
	{
		return await this.Mediator.SendRequest(message, cancellationToken);
	}
	#endregion
}
