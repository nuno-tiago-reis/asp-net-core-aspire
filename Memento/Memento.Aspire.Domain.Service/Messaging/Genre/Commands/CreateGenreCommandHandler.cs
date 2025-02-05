namespace Memento.Aspire.Domain.Service.Messaging.Genre.Commands;

using AutoMapper;
using Memento.Aspire.Domain.Service.Contracts.Genre;
using Memento.Aspire.Domain.Service.Messaging.Genre.Events;
using Memento.Aspire.Domain.Service.Persistence.Entities.Genre;
using Memento.Aspire.Shared.Exceptions;
using Memento.Aspire.Shared.Messaging;
using Memento.Aspire.Shared.Messaging.RequestResponse;
using System.Threading;

/// <summary>
/// Implements the interface for the create genre command handler.
/// </summary>
///
/// <seealso cref="CommandHandler{T, T}" />
public sealed class CreateGenreCommandHandler : CommandHandler<CreateGenreCommand, CreateGenreCommandResult>
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
	/// Initializes a new instance of the <see cref="CreateGenreCommandHandler"/> class.
	/// </summary>
	///
	/// <param name="logger">The logger.</param>
	/// <param name="mapper">The mapper.</param>
	/// <param name="messageBus">The message bus.</param>
	/// <param name="repository">The repository.</param>
	public CreateGenreCommandHandler(ILogger<CreateGenreCommandHandler> logger, IMapper mapper, IMessageBus messageBus, IGenreRepository repository) : base(logger)
	{
		this.Mapper = mapper;
		this.MessageBus = messageBus;
		this.Repository = repository;
	}
	#endregion

	#region [Methods]
	/// <inheritdoc />
	protected override async Task<CreateGenreCommandResult> HandleMessageAsync(CreateGenreCommand command, CancellationToken cancellationToken = default)
	{
		// Map the genre
		var genre = this.Mapper.Map<Genre>(command.GenreContract);
		genre.CreatedBy = command.UserId;

		// Create the genre
		var createdGenre = await this.Repository.CreateAsync(genre, cancellationToken);

		// Create the event
		var createdEvent = new GenreCreatedEvent
		{
			Genre = this.Mapper.Map<GenreDetailContract>(createdGenre),
			CorrelationId = command.CorrelationId,
			Timestamp = DateTimeOffset.UtcNow
		};

		// Publish the event
		await this.MessageBus.FireAndForgetViaBusAsync(createdEvent, cancellationToken);

		// Build the result
		return new CreateGenreCommandResult
		{
			CorrelationId = command.CorrelationId,
			UserId = command.UserId,
			Success = true,
			Exception = null,
			GenreContract = this.Mapper.Map<GenreDetailContract>(createdGenre)
		};
	}

	/// <inheritdoc />
	protected override Task<CreateGenreCommandResult> HandleExceptionAsync(CreateGenreCommand command, StandardException exception, CancellationToken cancellationToken = default)
	{
		// Build the result
		return Task.FromResult(new CreateGenreCommandResult
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
