namespace Memento.Aspire.Domain.Service.Messaging.Book.Commands;

using AutoMapper;
using Memento.Aspire.Domain.Service.Contracts.Book;
using Memento.Aspire.Domain.Service.Messaging.Book.Events;
using Memento.Aspire.Domain.Service.Persistence.Entities.Book;
using Memento.Aspire.Shared.Exceptions;
using Memento.Aspire.Shared.Messaging;
using Memento.Aspire.Shared.Messaging.RequestResponse;
using System.Threading;

/// <summary>
/// Implements the interface for the update book command handler.
/// </summary>
///
/// <seealso cref="CommandHandler{T, T}" />
public sealed class UpdateBookCommandHandler : CommandHandler<UpdateBookCommand, UpdateBookCommandResult>
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
	/// Initializes a new instance of the <see cref="UpdateBookCommandHandler"/> class.
	/// </summary>
	///
	/// <param name="logger">The logger.</param>
	/// <param name="mapper">The mapper.</param>
	/// <param name="messageBus">The message bus.</param>
	/// <param name="repository">The repository.</param>
	public UpdateBookCommandHandler(ILogger<UpdateBookCommandHandler> logger, IMapper mapper, IMessageBus messageBus, IBookRepository repository) : base(logger)
	{
		this.Mapper = mapper;
		this.MessageBus = messageBus;
		this.Repository = repository;
	}
	#endregion

	#region [Methods]
	/// <inheritdoc />
	protected override async Task<UpdateBookCommandResult> HandleMessageAsync(UpdateBookCommand command, CancellationToken cancellationToken = default)
	{
		// Map the book
		var book = this.Mapper.Map<Book>(command.BookContract);
		book.Id = command.BookId;
		book.UpdatedBy = command.UserId;

		// Update the book
		var updatedBook = await this.Repository.UpdateAsync(book, cancellationToken);

		// Update the event
		var updatedEvent = new BookUpdatedEvent
		{
			Book = this.Mapper.Map<BookDetailContract>(updatedBook),
			CorrelationId = command.CorrelationId,
			Timestamp = DateTimeOffset.UtcNow
		};

		// Publish the event
		await this.MessageBus.FireAndForgetViaBusAsync(updatedEvent, cancellationToken);

		// Build the result
		return new UpdateBookCommandResult
		{
			CorrelationId = command.CorrelationId,
			UserId = command.UserId,
			Success = true,
			Exception = null,
			BookContract = this.Mapper.Map<BookDetailContract>(updatedBook)
		};
	}

	/// <inheritdoc />
	protected override Task<UpdateBookCommandResult> HandleExceptionAsync(UpdateBookCommand command, StandardException exception, CancellationToken cancellationToken = default)
	{
		// Build the result
		return Task.FromResult(new UpdateBookCommandResult
		{
			CorrelationId = command.CorrelationId,
			UserId = command.UserId,
			Success = false,
			Exception = exception,
			BookContract = null
		});
	}
	#endregion
}
