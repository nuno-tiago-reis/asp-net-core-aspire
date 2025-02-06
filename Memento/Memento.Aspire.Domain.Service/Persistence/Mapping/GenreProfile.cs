namespace Memento.Aspire.Domain.Service.Persistence.Mapping;

using AutoMapper;
using Memento.Aspire.Domain.Service.Contracts.Genre;
using Memento.Aspire.Domain.Service.Persistence.Entities.Genre;

/// <summary>
/// Implements the 'Genre' profile.
/// </summary>
///
/// <seealso cref="Profile" />
public sealed class GenreProfile : Profile
{
	#region [Constructor]
	/// <summary>
	/// Initializes a new instance of the <see cref="GenreProfile"/> class.
	/// </summary>
	public GenreProfile()
	{
		// Entity => Contract
		this.CreateMap<Genre, GenreDetailContract>();
		// Entity => Contract
		this.CreateMap<Genre, GenreFormContract>();
		// Entity => Contract
		this.CreateMap<Genre, GenreSummaryContract>();

		// Contract => Entity
		this.CreateMap<GenreFormContract, Genre>();

		// FilterContract => Filter
		this.CreateMap<GenreFilterContract, GenreFilter>();
		// Filter => FilterContract
		this.CreateMap<GenreFilter, GenreFilterContract>();
	}
	#endregion
}
