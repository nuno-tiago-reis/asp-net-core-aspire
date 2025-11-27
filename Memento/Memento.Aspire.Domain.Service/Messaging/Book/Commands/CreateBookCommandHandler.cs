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
/// Implements the interface for the create book command handler.
/// </summary>
///
/// <seealso cref="CommandHandler{T, T}" />
public sealed class CreateBookCommandHandler : CommandHandler<CreateBookCommand, CreateBookCommandResult>
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
	/// Initializes a new instance of the <see cref="CreateBookCommandHandler"/> class.
	/// </summary>
	///
	/// <param name="logger">The logger.</param>
	/// <param name="mapper">The mapper.</param>
	/// <param name="messageBus">The message bus.</param>
	/// <param name="repository">The repository.</param>
	public CreateBookCommandHandler(ILogger<CreateBookCommandHandler> logger, IMapper mapper, IMessageBus messageBus, IBookRepository repository) : base(logger)
	{
		this.Mapper = mapper;
		this.MessageBus = messageBus;
		this.Repository = repository;
	}
	#endregion

	#region [Methods]
	/// <inheritdoc />
	protected override async Task<CreateBookCommandResult> HandleMessageAsync(CreateBookCommand command, CancellationToken cancellationToken = default)
	{
		// Map the book
		var book = this.Mapper.Map<Book>(command.BookContract);
		book.CreatedBy = command.UserId;

		// Create the book
		var createdBook = await this.Repository.CreateAsync(book, cancellationToken);

		// Create the event
		var createdEvent = new BookCreatedEvent
		{
			Book = this.Mapper.Map<BookDetailContract>(createdBook),
			CorrelationId = command.CorrelationId,
			Timestamp = DateTimeOffset.UtcNow
		};

		// Publish the event
		await this.MessageBus.DispatchEventViaBusAsync(createdEvent, cancellationToken);

		// Build the result
		return new CreateBookCommandResult
		{
			CorrelationId = command.CorrelationId,
			UserId = command.UserId,
			Success = true,
			Exception = null,
			BookContract = this.Mapper.Map<BookDetailContract>(createdBook)
		};
	}

	/// <inheritdoc />
	protected override Task<CreateBookCommandResult> HandleExceptionAsync(CreateBookCommand command, StandardException exception, CancellationToken cancellationToken = default)
	{
		// Build the result
		return Task.FromResult(new CreateBookCommandResult
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
