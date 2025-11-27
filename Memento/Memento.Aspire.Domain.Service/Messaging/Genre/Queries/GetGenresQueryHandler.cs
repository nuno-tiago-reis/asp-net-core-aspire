namespace Memento.Aspire.Domain.Service.Messaging.Genre.Queries;

using AutoMapper;
using Memento.Aspire.Domain.Service.Contracts.Genre;
using Memento.Aspire.Domain.Service.Persistence.Entities.Genre;
using Memento.Aspire.Core.Exceptions;
using Memento.Aspire.Core.Messaging.Messages;
using Memento.Aspire.Core.Pagination;
using System.Threading;

/// <summary>
/// Implements the interface for the get genres message handler.
/// </summary>
///
/// <seealso cref="MessageHandler{T, T}" />
public sealed class GetGenresQueryHandler : MessageHandler<GetGenresQuery, GetGenresQueryResult>
{
	#region [Properties]
	/// <summary>
	/// The mapper.
	/// </summary>
	private readonly IMapper Mapper;

	/// <summary>
	/// The repository.
	/// </summary>
	private readonly IGenreRepository Repository;
	#endregion

	#region [Constructors]
	/// <summary>
	/// Initializes a new instance of the <see cref="GetGenresQueryHandler"/> class.
	/// </summary>
	///
	/// <param name="logger">The logger.</param>
	/// <param name="mapper">The mapper.</param>
	/// <param name="repository">The repository.</param>
	public GetGenresQueryHandler(ILogger<GetGenresQueryHandler> logger, IMapper mapper, IGenreRepository repository) : base(logger)
	{
		this.Mapper = mapper;
		this.Repository = repository;
	}
	#endregion

	#region [Methods]
	/// <inheritdoc />
	protected override async Task<GetGenresQueryResult> HandleMessageAsync(GetGenresQuery query, CancellationToken cancellationToken = default)
	{
		// Map the filter
		var filter = this.Mapper.Map<GenreFilter>(query.GenreFilterContract);

		// Get the genres
		var genres = await this.Repository.GetAllAsync(filter, cancellationToken);

		// Build the result
		return new GetGenresQueryResult
		{
			CorrelationId = query.CorrelationId,
			UserId = query.UserId,
			Success = true,
			Exception = null,
			GenreContracts = this.Mapper.Map<Page<GenreSummaryContract>>(genres)
		};
	}

	/// <inheritdoc />
	protected override Task<GetGenresQueryResult> HandleExceptionAsync(GetGenresQuery query, StandardException exception, CancellationToken cancellationToken = default)
	{
		// Build the result
		return Task.FromResult(new GetGenresQueryResult
		{
			CorrelationId = query.CorrelationId,
			UserId = query.UserId,
			Success = false,
			Exception = exception,
			GenreContracts = null

		});
	}
	#endregion
}
