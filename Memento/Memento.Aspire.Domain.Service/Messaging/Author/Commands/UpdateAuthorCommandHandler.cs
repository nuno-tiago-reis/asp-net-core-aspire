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
/// Implements the interface for the update author command handler.
/// </summary>
///
/// <seealso cref="CommandHandler{T, T}" />
public sealed class UpdateAuthorCommandHandler : CommandHandler<UpdateAuthorCommand, UpdateAuthorCommandResult>
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
	/// Initializes a new instance of the <see cref="UpdateAuthorCommandHandler"/> class.
	/// </summary>
	///
	/// <param name="logger">The logger.</param>
	/// <param name="mapper">The mapper.</param>
	/// <param name="messageBus">The message bus.</param>
	/// <param name="repository">The repository.</param>
	public UpdateAuthorCommandHandler(ILogger<UpdateAuthorCommandHandler> logger, IMapper mapper, IMessageBus messageBus, IAuthorRepository repository) : base(logger)
	{
		this.Mapper = mapper;
		this.MessageBus = messageBus;
		this.Repository = repository;
	}
	#endregion

	#region [Methods]
	/// <inheritdoc />
	protected override async Task<UpdateAuthorCommandResult> HandleMessageAsync(UpdateAuthorCommand command, CancellationToken cancellationToken = default)
	{
		// Map the author
		var author = this.Mapper.Map<Author>(command.AuthorContract);
		author.Id = command.AuthorId;
		author.UpdatedBy = command.UserId;

		// Update the author
		var updatedAuthor = await this.Repository.UpdateAsync(author, cancellationToken);

		// Update the event
		var updatedEvent = new AuthorUpdatedEvent
		{
			Author = this.Mapper.Map<AuthorDetailContract>(updatedAuthor),
			CorrelationId = command.CorrelationId,
			Timestamp = DateTimeOffset.UtcNow
		};

		// Publish the event
		await this.MessageBus.FireAndForgetViaBusAsync(updatedEvent, cancellationToken);

		// Build the result
		return new UpdateAuthorCommandResult
		{
			CorrelationId = command.CorrelationId,
			UserId = command.UserId,
			Success = true,
			Exception = null,
			AuthorContract = this.Mapper.Map<AuthorDetailContract>(updatedAuthor)
		};
	}

	/// <inheritdoc />
	protected override Task<UpdateAuthorCommandResult> HandleExceptionAsync(UpdateAuthorCommand command, StandardException exception, CancellationToken cancellationToken = default)
	{
		// Build the result
		return Task.FromResult(new UpdateAuthorCommandResult
		{
			CorrelationId = command.CorrelationId,
			UserId = command.UserId,
			Success = false,
			Exception = exception,
			AuthorContract = null
		});
	}
	#endregion
}
