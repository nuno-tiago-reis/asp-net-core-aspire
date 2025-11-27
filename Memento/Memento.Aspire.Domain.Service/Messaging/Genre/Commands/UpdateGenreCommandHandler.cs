namespace Memento.Aspire.Domain.Service.Messaging.Genre.Commands;

using AutoMapper;
using Memento.Aspire.Domain.Service.Contracts.Genre;
using Memento.Aspire.Domain.Service.Messaging.Genre.Events;
using Memento.Aspire.Domain.Service.Persistence.Entities.Genre;
using Memento.Aspire.Core.Exceptions;
using Memento.Aspire.Core.Messaging;
using Memento.Aspire.Core.Messaging.Messages;
using System.Threading;

/// <summary>
/// Implements the interface for the update genre command handler.
/// </summary>
///
/// <seealso cref="CommandHandler{T, T}" />
public sealed class UpdateGenreCommandHandler : CommandHandler<UpdateGenreCommand, UpdateGenreCommandResult>
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
	/// Initializes a new instance of the <see cref="UpdateGenreCommandHandler"/> class.
	/// </summary>
	///
	/// <param name="logger">The logger.</param>
	/// <param name="mapper">The mapper.</param>
	/// <param name="messageBus">The message bus.</param>
	/// <param name="repository">The repository.</param>
	public UpdateGenreCommandHandler(ILogger<UpdateGenreCommandHandler> logger, IMapper mapper, IMessageBus messageBus, IGenreRepository repository) : base(logger)
	{
		this.Mapper = mapper;
		this.MessageBus = messageBus;
		this.Repository = repository;
	}
	#endregion

	#region [Methods]
	/// <inheritdoc />
	protected override async Task<UpdateGenreCommandResult> HandleMessageAsync(UpdateGenreCommand command, CancellationToken cancellationToken = default)
	{
		// Map the genre
		var genre = this.Mapper.Map<Genre>(command.GenreContract);
		genre.Id = command.GenreId;
		genre.UpdatedBy = command.UserId;

		// Update the genre
		var updatedGenre = await this.Repository.UpdateAsync(genre, cancellationToken);

		// Update the event
		var updatedEvent = new GenreUpdatedEvent
		{
			Genre = this.Mapper.Map<GenreDetailContract>(updatedGenre),
			CorrelationId = command.CorrelationId,
			Timestamp = DateTimeOffset.UtcNow
		};

		// Publish the event
		await this.MessageBus.DispatchEventViaBusAsync(updatedEvent, cancellationToken);

		// Build the result
		return new UpdateGenreCommandResult
		{
			CorrelationId = command.CorrelationId,
			UserId = command.UserId,
			Success = true,
			Exception = null,
			GenreContract = this.Mapper.Map<GenreDetailContract>(updatedGenre)
		};
	}

	/// <inheritdoc />
	protected override Task<UpdateGenreCommandResult> HandleExceptionAsync(UpdateGenreCommand command, StandardException exception, CancellationToken cancellationToken = default)
	{
		// Build the result
		return Task.FromResult(new UpdateGenreCommandResult
		{
			CorrelationId = command.CorrelationId,
			UserId = command.UserId,
			Success = false,
			Exception = exception,
			GenreContract = null
		});
	}
	#endregion
}
