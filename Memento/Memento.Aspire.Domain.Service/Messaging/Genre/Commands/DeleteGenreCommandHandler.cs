namespace Memento.Aspire.Domain.Service.Messaging.Genre.Commands;

using AutoMapper;
using Memento.Aspire.Domain.Service.Contracts.Genre;
using Memento.Aspire.Domain.Service.Messaging.Genre.Events;
using Memento.Aspire.Domain.Service.Persistence.Entities.Genre;
using Memento.Aspire.Shared.Exceptions;
using Memento.Aspire.Shared.Messaging;
using Memento.Aspire.Shared.Messaging.Messages;
using System.Threading;

/// <summary>
/// Implements the interface for the delete deletedGenre command handler.
/// </summary>
///
/// <seealso cref="CommandHandler{T, T}" />
public sealed class DeleteGenreCommandHandler : CommandHandler<DeleteGenreCommand, DeleteGenreCommandResult>
{
	#region [Properties]
	/// <summary>
	/// The mapper.
	/// </summary>
	private readonly IMapper Mapper;

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
	/// <param name="mapper">The mapper.</param>
	/// <param name="messageBus">The message bus.</param>
	/// <param name="repository">The repository.</param>
	public DeleteGenreCommandHandler(ILogger<DeleteGenreCommandHandler> logger, IMapper mapper, IMessageBus messageBus, IGenreRepository repository) : base(logger)
	{
		this.Mapper = mapper;
		this.MessageBus = messageBus;
		this.Repository = repository;
	}
	#endregion

	#region [Methods]
	/// <inheritdoc />
	protected override async Task<DeleteGenreCommandResult> HandleMessageAsync(DeleteGenreCommand command, CancellationToken cancellationToken = default)
	{
		// Delete the genre
		var deletedGenre = await this.Repository.DeleteAsync(command.GenreId, cancellationToken);

		// Create the event
		var deletedEvent = new GenreDeletedEvent
		{
			Genre = this.Mapper.Map<GenreDetailContract>(deletedGenre),
			CorrelationId = command.CorrelationId,
			Timestamp = DateTimeOffset.UtcNow
		};

		// Publish the event
		await this.MessageBus.DispatchEventViaBusAsync(deletedEvent, cancellationToken);

		// Build the result
		return new DeleteGenreCommandResult
		{
			CorrelationId = command.CorrelationId,
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
			UserId = command.UserId,
			Success = false,
			Exception = exception
		});
	}
	#endregion
}
