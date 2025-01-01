namespace Memento.Aspire.Shared.Messaging.RequestResponse;

using MassTransit;
using Memento.Aspire.Shared.Exceptions;
using Microsoft.Extensions.Logging;
using System.Threading;

/// <summary>
/// Implements a generic message handler.
/// Provides methods to handle messages and message results.
/// </summary>
public abstract class MessageHandler<TMessage, TMessageResult> : IConsumer<TMessage>
	where TMessage : Message<TMessageResult>
	where TMessageResult : MessageResult
{
	#region [Properties]
	/// <summary>
	/// The logger.
	/// </summary>
	protected readonly ILogger Logger;
	#endregion

	#region [Constructors]
	/// <summary>
	/// Initializes a new instance of the <see cref="MessageHandler{TMessage, TMessageResult}"/> class.
	/// </summary>
	///
	/// <param name="logger">The logger.</param>
	protected MessageHandler(ILogger<MessageHandler<TMessage, TMessageResult>> logger)
	{
		this.Logger = logger;
	}
	#endregion

	#region [Methods]
	/// <summary>
	/// Consumes the message.
	/// </summary>
	///
	/// <param name="context">The context.</param>
	public async Task Consume(ConsumeContext<TMessage> context)
	{
		// Log the message
		this.Logger.LogInformation("Processing {Message}", context.Message);

		try
		{
			// Handle the message and obtain the result
			var messageResult = await this.HandleMessageAsync(context.Message, context.CancellationToken);

			// Forward the result
			await context.RespondAsync(messageResult);

			// Log the message
			this.Logger.LogInformation("Processed Successfully {Message} {MessageResult}", context.Message, messageResult);
		}
		catch (Exception exception)
		{
			// Check if it's an unhandled exception
			if (exception is not StandardException)
			{
				exception = new StandardException(exception.Message, exception, StandardExceptionType.InternalServerError);
			}

			// Handle the exception and obtain the result
			var messageResult = await this.HandleExceptionAsync(context.Message, (StandardException)exception, context.CancellationToken);

			// Forward the result
			await context.RespondAsync(messageResult);

			// Log the message
			this.Logger.LogError("Processed Unsuccessfully {Message} {MessageResult} {Exception}", context.Message, messageResult, exception);
		}
	}

	/// <summary>
	/// Handles the message while returning a message result.
	/// </summary>
	///
	/// <param name="message">The message.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	///
	/// <returns>A message result.</returns>
	protected abstract Task<TMessageResult> HandleMessageAsync(TMessage message, CancellationToken cancellationToken = default);

	/// <summary>
	/// Handles the exception while returning a message result.
	/// </summary>
	///
	/// <param name="message">The message.</param>
	/// <param name="exception">The exception.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	///
	/// <returns>A message result.</returns>
	protected abstract Task<TMessageResult> HandleExceptionAsync(TMessage message, StandardException exception, CancellationToken cancellationToken = default);
	#endregion
}
