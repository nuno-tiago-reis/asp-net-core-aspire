namespace Memento.Aspire.Domain.Service.Messaging.Genre.Queries;

using AutoMapper;
using Memento.Aspire.Domain.Service.Contracts.Genre;
using Memento.Aspire.Domain.Service.Persistence.Entities.Genre;
using Memento.Aspire.Core.Exceptions;
using Memento.Aspire.Core.Messaging.Messages;
using System.Threading;

/// <summary>
/// Implements the interface for the get genre query handler.
/// </summary>
///
/// <seealso cref="QueryHandler{T, T}" />
public sealed class GetGenreQueryHandler : QueryHandler<GetGenreQuery, GetGenreQueryResult>
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
	/// Initializes a new instance of the <see cref="GetGenreQueryHandler"/> class.
	/// </summary>
	///
	/// <param name="logger">The logger.</param>
	/// <param name="mapper">The mapper.</param>
	/// <param name="repository">The repository.</param>
	public GetGenreQueryHandler(ILogger<GetGenreQueryHandler> logger, IMapper mapper, IGenreRepository repository) : base(logger)
	{
		this.Mapper = mapper;
		this.Repository = repository;
	}
	#endregion

	#region [Methods]
	/// <inheritdoc />
	protected override async Task<GetGenreQueryResult> HandleMessageAsync(GetGenreQuery query, CancellationToken cancellationToken = default)
	{
		// Get the genre
		var genre = await this.Repository.GetAsync(query.GenreId, cancellationToken);

		// Build the result
		return new GetGenreQueryResult
		{
			CorrelationId = query.CorrelationId,
			UserId = query.UserId,
			Success = true,
			Exception = null,
			GenreContract = this.Mapper.Map<GenreDetailContract>(genre)
		};
	}

	/// <inheritdoc />
	protected override Task<GetGenreQueryResult> HandleExceptionAsync(GetGenreQuery query, StandardException exception, CancellationToken cancellationToken = default)
	{
		// Build the result
		return Task.FromResult(new GetGenreQueryResult
		{
			CorrelationId = query.CorrelationId,
			UserId = query.UserId,
			Success = false,
			Exception = exception,
			GenreContract = null

		});
	}
	#endregion
}
