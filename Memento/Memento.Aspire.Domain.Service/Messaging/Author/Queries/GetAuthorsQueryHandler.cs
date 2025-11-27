namespace Memento.Aspire.Domain.Service.Messaging.Author.Queries;

using AutoMapper;
using Memento.Aspire.Domain.Service.Contracts.Author;
using Memento.Aspire.Domain.Service.Persistence.Entities.Author;
using Memento.Aspire.Core.Exceptions;
using Memento.Aspire.Core.Messaging.Messages;
using Memento.Aspire.Core.Pagination;
using System.Threading;

/// <summary>
/// Implements the interface for the get authors message handler.
/// </summary>
///
/// <seealso cref="MessageHandler{T, T}" />
public sealed class GetAuthorsQueryHandler : MessageHandler<GetAuthorsQuery, GetAuthorsQueryResult>
{
	#region [Properties]
	/// <summary>
	/// The mapper.
	/// </summary>
	private readonly IMapper Mapper;

	/// <summary>
	/// The repository.
	/// </summary>
	private readonly IAuthorRepository Repository;
	#endregion

	#region [Constructors]
	/// <summary>
	/// Initializes a new instance of the <see cref="GetAuthorsQueryHandler"/> class.
	/// </summary>
	///
	/// <param name="logger">The logger.</param>
	/// <param name="mapper">The mapper.</param>
	/// <param name="repository">The repository.</param>
	public GetAuthorsQueryHandler(ILogger<GetAuthorsQueryHandler> logger, IMapper mapper, IAuthorRepository repository) : base(logger)
	{
		this.Mapper = mapper;
		this.Repository = repository;
	}
	#endregion

	#region [Methods]
	/// <inheritdoc />
	protected override async Task<GetAuthorsQueryResult> HandleMessageAsync(GetAuthorsQuery query, CancellationToken cancellationToken = default)
	{
		// Map the filter
		var filter = this.Mapper.Map<AuthorFilter>(query.AuthorFilterContract);

		// Get the authors
		var authors = await this.Repository.GetAllAsync(filter, cancellationToken);

		// Build the result
		return new GetAuthorsQueryResult
		{
			CorrelationId = query.CorrelationId,
			UserId = query.UserId,
			Success = true,
			Exception = null,
			AuthorContracts = this.Mapper.Map<Page<AuthorSummaryContract>>(authors)
		};
	}

	/// <inheritdoc />
	protected override Task<GetAuthorsQueryResult> HandleExceptionAsync(GetAuthorsQuery query, StandardException exception, CancellationToken cancellationToken = default)
	{
		// Build the result
		return Task.FromResult(new GetAuthorsQueryResult
		{
			CorrelationId = query.CorrelationId,
			UserId = query.UserId,
			Success = false,
			Exception = exception,
			AuthorContracts = null

		});
	}
	#endregion
}
