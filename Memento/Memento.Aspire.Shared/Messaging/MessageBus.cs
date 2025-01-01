namespace Memento.Aspire.Shared.Messaging;

using MassTransit;
using MassTransit.Mediator;
using Memento.Aspire.Shared.Messaging.RequestResponse;
using Microsoft.Extensions.DependencyInjection;
using System;

/// <summary>
/// Implements the generic interface for a message bus.
/// Provides methods to send messages and receive message results.
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
	/// Dispatches a message and returns a message result.
	/// </summary>
	///
	/// <param name="message">The message.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	public async Task FireAndForgetViaBusAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
		where TMessage : class
	{
		await this.Bus.Publish(message, cancellationToken);
	}

	/// <summary>
	/// Dispatches a message and returns a message result.
	/// </summary>
	///
	/// <param name="message">The message.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	///
	/// <returns>A message result</returns>
	public async Task<TMessageResult> RequestResponseViaBusAsync<TMessage, TMessageResult>(TMessage message, CancellationToken cancellationToken = default)
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
	/// Dispatches a message and returns a message result.
	/// </summary>
	///
	/// <param name="message">The message.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	public async Task FireAndForgetViaMediatorAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
		where TMessage : class
	{
		await this.Mediator.Publish(message, cancellationToken);
	}

	/// <summary>
	/// Dispatches a message and returns a message result.
	/// </summary>
	///
	/// <param name="message">The message.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	///
	/// <returns>A message result</returns>
	public async Task<TMessageResult> RequestResponseViaMediatorAsync<TMessage, TMessageResult>(TMessage message, CancellationToken cancellationToken = default)
		where TMessage : Message<TMessageResult>
		where TMessageResult : MessageResult
	{
		return await this.Mediator.SendRequest(message, cancellationToken);
	}
	#endregion
}
