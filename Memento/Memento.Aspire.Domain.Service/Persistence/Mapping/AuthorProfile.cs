namespace Memento.Aspire.Domain.Service.Persistence.Mapping;

using AutoMapper;
using Memento.Aspire.Domain.Service.Contracts.Author;
using Memento.Aspire.Domain.Service.Persistence.Entities.Author;

/// <summary>
/// Implements the 'Author' profile.
/// </summary>
///
/// <seealso cref="Profile" />
public sealed class AuthorProfile : Profile
{
	#region [Constructor]
	/// <summary>
	/// Initializes a new instance of the <see cref="AuthorProfile"/> class.
	/// </summary>
	public AuthorProfile()
	{
		// Entity => Contract
		this.CreateMap<Author, AuthorDetailContract>();
		// Entity => Contract
		this.CreateMap<Author, AuthorFormContract>();
		// Entity => Contract
		this.CreateMap<Author, AuthorSummaryContract>();

		// Contract => Entity
		this.CreateMap<AuthorFormContract, Author>();

		// FilterContract => Filter
		this.CreateMap<AuthorFilterContract, AuthorFilter>();
		// Filter => FilterContract
		this.CreateMap<AuthorFilter, AuthorFilterContract>();
	}
	#endregion
}
