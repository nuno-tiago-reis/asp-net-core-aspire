namespace Memento.Aspire.Domain.Service.Messaging.Genre.Commands;

using Memento.Aspire.Domain.Service.Messaging.Genre.Events;
using Memento.Aspire.Domain.Service.Persistence.Entities.Genre;
using Memento.Aspire.Shared.Exceptions;
using Memento.Aspire.Shared.Messaging;
using Memento.Aspire.Shared.Messaging.RequestResponse;
using System.Threading;

/// <summary>
/// Implements the interface for the delete genre command handler.
/// </summary>
///
/// <seealso cref="CommandHandler{T, T}" />
public sealed class DeleteGenreCommandHandler : CommandHandler<DeleteGenreCommand, DeleteGenreCommandResult>
{
	#region [Properties]
	/// <summary>
	/// The message bus.
	/// </summary>
	private readonly IMessageBus MessageBus;

	/// <summary>
	/// The repository.
	/// </summary>
	private readonly IGenreRepository Repository;
	#endregion

	#region [Constructors]
	/// <summary>
	/// Initializes a new instance of the <see cref="DeleteGenreCommandHandler"/> class.
	/// </summary>
	///
	/// <param name="logger">The logger.</param>
	/// <param name="messageBus">The message bus.</param>
	/// <param name="repository">The repository.</param>
	public DeleteGenreCommandHandler(ILogger<DeleteGenreCommandHandler> logger, IMessageBus messageBus, IGenreRepository repository) : base(logger)
	{
		this.MessageBus = messageBus;
		this.Repository = repository;
	}
	#endregion

	#region [Methods]
	/// <inheritdoc />
	protected override async Task<DeleteGenreCommandResult> HandleMessageAsync(DeleteGenreCommand command, CancellationToken cancellationToken = default)
	{
		// Delete the genre
		await this.Repository.DeleteAsync(command.GenreId, cancellationToken);

		// Create the event
		var createdEvent = new GenreDeletedEvent
		{
			GenreId = command.GenreId,
			CorrelationId = command.CorrelationId,
			Timestamp = DateTimeOffset.UtcNow
		};

		// Publish the event
		await this.MessageBus.FireAndForgetViaBusAsync(createdEvent, cancellationToken);

		// Build the result
		return new DeleteGenreCommandResult
		{
			CorrelationId = command.CorrelationId,
			IdempotencyId = command.IdempotencyId,
			UserId = command.UserId,
			Success = true,
			Exception = null
		};
	}

	/// <inheritdoc />
	protected override Task<DeleteGenreCommandResult> HandleExceptionAsync(DeleteGenreCommand command, StandardException exception, CancellationToken cancellationToken = default)
	{
		// Build the result
		return Task.FromResult(new DeleteGenreCommandResult
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
