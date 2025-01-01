namespace Memento.Aspire.Domain.Service.Messaging.Author.Commands;

using AutoMapper;
using Memento.Aspire.Domain.Service.Contracts.Author;
using Memento.Aspire.Domain.Service.Messaging.Author.Events;
using Memento.Aspire.Domain.Service.Persistence.Entities.Author;
using Memento.Aspire.Shared.Exceptions;
using Memento.Aspire.Shared.Messaging;
using Memento.Aspire.Shared.Messaging.RequestResponse;
using System.Threading;

/// <summary>
/// Implements the interface for the create author command handler.
/// </summary>
///
/// <seealso cref="CommandHandler{T, T}" />
public sealed class CreateAuthorCommandHandler : CommandHandler<CreateAuthorCommand, CreateAuthorCommandResult>
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
	private readonly IAuthorRepository Repository;
	#endregion

	#region [Constructors]
	/// <summary>
	/// Initializes a new instance of the <see cref="CreateAuthorCommandHandler"/> class.
	/// </summary>
	///
	/// <param name="logger">The logger.</param>
	/// <param name="mapper">The mapper.</param>
	/// <param name="messageBus">The message bus.</param>
	/// <param name="repository">The repository.</param>
	public CreateAuthorCommandHandler(ILogger<CreateAuthorCommandHandler> logger, IMapper mapper, IMessageBus messageBus, IAuthorRepository repository) : base(logger)
	{
		this.Mapper = mapper;
		this.MessageBus = messageBus;
		this.Repository = repository;
	}
	#endregion

	#region [Methods]
	/// <inheritdoc />
	protected override async Task<CreateAuthorCommandResult> HandleMessageAsync(CreateAuthorCommand command, CancellationToken cancellationToken = default)
	{
		// Map the author
		var author = this.Mapper.Map<Author>(command.AuthorContract);
		author.CreatedBy = command.UserId;

		// Create the author
		var createdAuthor = await this.Repository.CreateAsync(author, cancellationToken);

		// Create the event
		var createdEvent = new AuthorCreatedEvent
		{
			Author = this.Mapper.Map<AuthorDetailContract>(createdAuthor),
			CorrelationId = command.CorrelationId,
			Timestamp = DateTimeOffset.UtcNow
		};

		// Publish the event
		await this.MessageBus.FireAndForgetViaBusAsync(createdEvent, cancellationToken);

		// Build the result
		return new CreateAuthorCommandResult
		{
			CorrelationId = command.CorrelationId,
			IdempotencyId = command.IdempotencyId,
			UserId = command.UserId,
			Success = true,
			Exception = null,
			AuthorContract = this.Mapper.Map<AuthorDetailContract>(createdAuthor)
		};
	}

	/// <inheritdoc />
	protected override Task<CreateAuthorCommandResult> HandleExceptionAsync(CreateAuthorCommand command, StandardException exception, CancellationToken cancellationToken = default)
	{
		// Build the result
		return Task.FromResult(new CreateAuthorCommandResult
		{
			CorrelationId = command.CorrelationId,
			IdempotencyId = command.IdempotencyId,
			UserId = command.UserId,
			Success = false,
			Exception = exception,
			AuthorContract = null
		});
	}
	#endregion
}
