namespace Memento.Aspire.Domain.Service.Messaging.Book.Queries;

using AutoMapper;
using Memento.Aspire.Domain.Service.Contracts.Book;
using Memento.Aspire.Domain.Service.Persistence.Entities.Book;
using Memento.Aspire.Shared.Exceptions;
using Memento.Aspire.Shared.Messaging.RequestResponse;
using Memento.Aspire.Shared.Pagination;
using System.Threading;

/// <summary>
/// Implements the interface for the get books message handler.
/// </summary>
///
/// <seealso cref="MessageHandler{T, T}" />
public sealed class GetBooksQueryHandler : MessageHandler<GetBooksQuery, GetBooksQueryResult>
{
	#region [Properties]
	/// <summary>
	/// The mapper.
	/// </summary>
	private readonly IMapper Mapper;

	/// <summary>
	/// The repository.
	/// </summary>
	private readonly IBookRepository Repository;
	#endregion

	#region [Constructors]
	/// <summary>
	/// Initializes a new instance of the <see cref="GetBooksQueryHandler"/> class.
	/// </summary>
	///
	/// <param name="logger">The logger.</param>
	/// <param name="mapper">The mapper.</param>
	/// <param name="repository">The repository.</param>
	public GetBooksQueryHandler(ILogger<GetBooksQueryHandler> logger, IMapper mapper, IBookRepository repository) : base(logger)
	{
		this.Mapper = mapper;
		this.Repository = repository;
	}
	#endregion

	#region [Methods]
	/// <inheritdoc />
	protected override async Task<GetBooksQueryResult> HandleMessageAsync(GetBooksQuery query, CancellationToken cancellationToken = default)
	{
		// Map the filter
		var filter = this.Mapper.Map<BookFilter>(query.BookFilterContract);

		// Get the books
		var books = await this.Repository.GetAllAsync(filter, cancellationToken);

		// Build the result
		return new GetBooksQueryResult
		{
			CorrelationId = query.CorrelationId,
			UserId = query.UserId,
			Success = true,
			Exception = null,
			BookContracts = this.Mapper.Map<Page<BookSummaryContract>>(books)
		};
	}

	/// <inheritdoc />
	protected override Task<GetBooksQueryResult> HandleExceptionAsync(GetBooksQuery query, StandardException exception, CancellationToken cancellationToken = default)
	{
		// Build the result
		return Task.FromResult(new GetBooksQueryResult
		{
			CorrelationId = query.CorrelationId,
			UserId = query.UserId,
			Success = false,
			Exception = exception,
			BookContracts = null

		});
	}
	#endregion
}
