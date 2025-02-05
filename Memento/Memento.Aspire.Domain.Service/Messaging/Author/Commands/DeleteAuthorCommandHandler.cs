namespace Memento.Aspire.Domain.Service.Messaging.Author.Commands;

using Memento.Aspire.Domain.Service.Messaging.Author.Events;
using Memento.Aspire.Domain.Service.Persistence.Entities.Author;
using Memento.Aspire.Shared.Exceptions;
using Memento.Aspire.Shared.Messaging;
using Memento.Aspire.Shared.Messaging.RequestResponse;
using System.Threading;

/// <summary>
/// Implements the interface for the delete author command handler.
/// </summary>
///
/// <seealso cref="CommandHandler{T, T}" />
public sealed class DeleteAuthorCommandHandler : CommandHandler<DeleteAuthorCommand, DeleteAuthorCommandResult>
{
	#region [Properties]
	/// <summary>
	/// The message bus.
	/// </summary>
	private readonly IMessageBus MessageBus;

	/// <summary>
	/// The repository.
	/// </summary>
	private readonly IAuthorRepository Repository;
	#endregion

	#region [Constructors]
	/// <summary>
	/// Initializes a new instance of the <see cref="DeleteAuthorCommandHandler"/> class.
	/// </summary>
	///
	/// <param name="logger">The logger.</param>
	/// <param name="messageBus">The message bus.</param>
	/// <param name="repository">The repository.</param>
	public DeleteAuthorCommandHandler(ILogger<DeleteAuthorCommandHandler> logger, IMessageBus messageBus, IAuthorRepository repository) : base(logger)
	{
		this.MessageBus = messageBus;
		this.Repository = repository;
	}
	#endregion

	#region [Methods]
	/// <inheritdoc />
	protected override async Task<DeleteAuthorCommandResult> HandleMessageAsync(DeleteAuthorCommand command, CancellationToken cancellationToken = default)
	{
		// Delete the author
		await this.Repository.DeleteAsync(command.AuthorId, cancellationToken);

		// Create the event
		var createdEvent = new AuthorDeletedEvent
		{
			AuthorId = command.AuthorId,
			CorrelationId = command.CorrelationId,
			Timestamp = DateTimeOffset.UtcNow
		};

		// Publish the event
		await this.MessageBus.FireAndForgetViaBusAsync(createdEvent, cancellationToken);

		// Build the result
		return new DeleteAuthorCommandResult
		{
			CorrelationId = command.CorrelationId,
			UserId = command.UserId,
			Success = true,
			Exception = null
		};
	}

	/// <inheritdoc />
	protected override Task<DeleteAuthorCommandResult> HandleExceptionAsync(DeleteAuthorCommand command, StandardException exception, CancellationToken cancellationToken = default)
	{
		// Build the result
		return Task.FromResult(new DeleteAuthorCommandResult
		{
			CorrelationId = command.CorrelationId,
			UserId = command.UserId,
			Success = false,
			Exception = exception
		});
	}
	#endregion
}
