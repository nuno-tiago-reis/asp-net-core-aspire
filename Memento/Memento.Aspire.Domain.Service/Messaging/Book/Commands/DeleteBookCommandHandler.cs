namespace Memento.Aspire.Domain.Service.Messaging.Book.Commands;

using Memento.Aspire.Domain.Service.Messaging.Book.Events;
using Memento.Aspire.Domain.Service.Persistence.Entities.Book;
using Memento.Aspire.Shared.Exceptions;
using Memento.Aspire.Shared.Messaging;
using Memento.Aspire.Shared.Messaging.RequestResponse;
using System.Threading;

/// <summary>
/// Implements the interface for the delete book command handler.
/// </summary>
///
/// <seealso cref="CommandHandler{T, T}" />
public sealed class DeleteBookCommandHandler : CommandHandler<DeleteBookCommand, DeleteBookCommandResult>
{
	#region [Properties]
	/// <summary>
	/// The message bus.
	/// </summary>
	private readonly IMessageBus MessageBus;

	/// <summary>
	/// The repository.
	/// </summary>
	private readonly IBookRepository Repository;
	#endregion

	#region [Constructors]
	/// <summary>
	/// Initializes a new instance of the <see cref="DeleteBookCommandHandler"/> class.
	/// </summary>
	///
	/// <param name="logger">The logger.</param>
	/// <param name="messageBus">The message bus.</param>
	/// <param name="repository">The repository.</param>
	public DeleteBookCommandHandler(ILogger<DeleteBookCommandHandler> logger, IMessageBus messageBus, IBookRepository repository) : base(logger)
	{
		this.MessageBus = messageBus;
		this.Repository = repository;
	}
	#endregion

	#region [Methods]
	/// <inheritdoc />
	protected override async Task<DeleteBookCommandResult> HandleMessageAsync(DeleteBookCommand command, CancellationToken cancellationToken = default)
	{
		// Delete the book
		await this.Repository.DeleteAsync(command.BookId, cancellationToken);

		// Create the event
		var createdEvent = new BookDeletedEvent
		{
			BookId = command.BookId,
			CorrelationId = command.CorrelationId,
			Timestamp = DateTimeOffset.UtcNow
		};

		// Publish the event
		await this.MessageBus.FireAndForgetViaBusAsync(createdEvent, cancellationToken);

		// Build the result
		return new DeleteBookCommandResult
		{
			CorrelationId = command.CorrelationId,
			IdempotencyId = command.IdempotencyId,
			UserId = command.UserId,
			Success = true,
			Exception = null
		};
	}

	/// <inheritdoc />
	protected override Task<DeleteBookCommandResult> HandleExceptionAsync(DeleteBookCommand command, StandardException exception, CancellationToken cancellationToken = default)
	{
		// Build the result
		return Task.FromResult(new DeleteBookCommandResult
		{
			CorrelationId = command.CorrelationId,
			IdempotencyId = command.IdempotencyId,
			UserId = command.UserId,
			Success = false,
			Exception = exception
		});
	}
	#endregion
}
