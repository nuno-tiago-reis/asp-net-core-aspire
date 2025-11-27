namespace Memento.Aspire.Domain.Service.Messaging.Book.Queries;

using AutoMapper;
using Memento.Aspire.Domain.Service.Contracts.Book;
using Memento.Aspire.Domain.Service.Persistence.Entities.Book;
using Memento.Aspire.Core.Exceptions;
using Memento.Aspire.Core.Messaging.Messages;
using System.Threading;

/// <summary>
/// Implements the interface for the get book query handler.
/// </summary>
///
/// <seealso cref="QueryHandler{T, T}" />
public sealed class GetBookQueryHandler : QueryHandler<GetBookQuery, GetBookQueryResult>
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
	/// Initializes a new instance of the <see cref="GetBookQueryHandler"/> class.
	/// </summary>
	///
	/// <param name="logger">The logger.</param>
	/// <param name="mapper">The mapper.</param>
	/// <param name="repository">The repository.</param>
	public GetBookQueryHandler(ILogger<GetBookQueryHandler> logger, IMapper mapper, IBookRepository repository) : base(logger)
	{
		this.Mapper = mapper;
		this.Repository = repository;
	}
	#endregion

	#region [Methods]
	/// <inheritdoc />
	protected override async Task<GetBookQueryResult> HandleMessageAsync(GetBookQuery query, CancellationToken cancellationToken = default)
	{
		// Get the book
		var book = await this.Repository.GetAsync(query.BookId, cancellationToken);

		// Build the result
		return new GetBookQueryResult
		{
			CorrelationId = query.CorrelationId,
			UserId = query.UserId,
			Success = true,
			Exception = null,
			BookContract = this.Mapper.Map<BookDetailContract>(book)
		};
	}

	/// <inheritdoc />
	protected override Task<GetBookQueryResult> HandleExceptionAsync(GetBookQuery query, StandardException exception, CancellationToken cancellationToken = default)
	{
		// Build the result
		return Task.FromResult(new GetBookQueryResult
		{
			CorrelationId = query.CorrelationId,
			UserId = query.UserId,
			Success = false,
			Exception = exception,
			BookContract = null

		});
	}
	#endregion
}
