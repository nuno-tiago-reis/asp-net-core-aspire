namespace Memento.Aspire.Domain.Service.Messaging.Author.Queries;

using AutoMapper;
using Memento.Aspire.Domain.Service.Contracts.Author;
using Memento.Aspire.Domain.Service.Persistence.Entities.Author;
using Memento.Aspire.Shared.Exceptions;
using Memento.Aspire.Shared.Messaging.Messages;
using System.Threading;

/// <summary>
/// Implements the interface for the get author query handler.
/// </summary>
///
/// <seealso cref="QueryHandler{T, T}" />
public sealed class GetAuthorQueryHandler : QueryHandler<GetAuthorQuery, GetAuthorQueryResult>
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
	/// Initializes a new instance of the <see cref="GetAuthorQueryHandler"/> class.
	/// </summary>
	///
	/// <param name="logger">The logger.</param>
	/// <param name="mapper">The mapper.</param>
	/// <param name="repository">The repository.</param>
	public GetAuthorQueryHandler(ILogger<GetAuthorQueryHandler> logger, IMapper mapper, IAuthorRepository repository) : base(logger)
	{
		this.Mapper = mapper;
		this.Repository = repository;
	}
	#endregion

	#region [Methods]
	/// <inheritdoc />
	protected override async Task<GetAuthorQueryResult> HandleMessageAsync(GetAuthorQuery query, CancellationToken cancellationToken = default)
	{
		// Get the author
		var author = await this.Repository.GetAsync(query.AuthorId, cancellationToken);

		// Build the result
		return new GetAuthorQueryResult
		{
			CorrelationId = query.CorrelationId,
			UserId = query.UserId,
			Success = true,
			Exception = null,
			AuthorContract = this.Mapper.Map<AuthorDetailContract>(author)
		};
	}

	/// <inheritdoc />
	protected override Task<GetAuthorQueryResult> HandleExceptionAsync(GetAuthorQuery query, StandardException exception, CancellationToken cancellationToken = default)
	{
		// Build the result
		return Task.FromResult(new GetAuthorQueryResult
		{
			CorrelationId = query.CorrelationId,
			UserId = query.UserId,
			Success = false,
			Exception = exception,
			AuthorContract = null

		});
	}
	#endregion
}
