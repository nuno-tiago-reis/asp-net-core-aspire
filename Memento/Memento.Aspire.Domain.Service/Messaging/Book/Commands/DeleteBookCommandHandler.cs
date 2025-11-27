namespace Memento.Aspire.Domain.Service.Messaging.Book.Commands;

using AutoMapper;
using Memento.Aspire.Domain.Service.Contracts.Book;
using Memento.Aspire.Domain.Service.Messaging.Book.Events;
using Memento.Aspire.Domain.Service.Persistence.Entities.Book;
using Memento.Aspire.Core.Exceptions;
using Memento.Aspire.Core.Messaging;
using Memento.Aspire.Core.Messaging.Messages;
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
	private readonly IBookRepository Repository;
	#endregion

	#region [Constructors]
	/// <summary>
	/// Initializes a new instance of the <see cref="DeleteBookCommandHandler"/> class.
	/// </summary>
	///
	/// <param name="logger">The logger.</param>
	/// <param name="mapper">The mapper.</param>
	/// <param name="messageBus">The message bus.</param>
	/// <param name="repository">The repository.</param>
	public DeleteBookCommandHandler(ILogger<DeleteBookCommandHandler> logger, IMapper mapper, IMessageBus messageBus, IBookRepository repository) : base(logger)
	{
		this.Mapper = mapper;
		this.MessageBus = messageBus;
		this.Repository = repository;
	}
	#endregion

	#region [Methods]
	/// <inheritdoc />
	protected override async Task<DeleteBookCommandResult> HandleMessageAsync(DeleteBookCommand command, CancellationToken cancellationToken = default)
	{
		// Delete the book
		var deletedBook = await this.Repository.DeleteAsync(command.BookId, cancellationToken);

		// Create the event
		var createdEvent = new BookDeletedEvent
		{
			Book = this.Mapper.Map<BookDetailContract>(deletedBook),
			CorrelationId = command.CorrelationId,
			Timestamp = DateTimeOffset.UtcNow
		};

		// Publish the event
		await this.MessageBus.DispatchEventViaBusAsync(createdEvent, cancellationToken);

		// Build the result
		return new DeleteBookCommandResult
		{
			CorrelationId = command.CorrelationId,
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
			UserId = command.UserId,
			Success = false,
			Exception = exception
		});
	}
	#endregion
}
