namespace Memento.Aspire.Domain.Service.Contracts.Author;

using Memento.Aspire.Core.Resources;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Implements the 'Author' form contract.
/// </summary>
public sealed record AuthorFormContract
{
	#region [Properties]
	/// <summary>
	/// The author's name.
	/// </summary>
	[Display(Name = nameof(SharedResources.AUTHOR_NAME), ResourceType = typeof(SharedResources))]
	public required string? Name { get; init; }

	/// <summary>
	/// The author's birth date.
	/// </summary>
	[Display(Name = nameof(SharedResources.AUTHOR_BIRTHDATE), ResourceType = typeof(SharedResources))]
	public required DateOnly? BirthDate { get; init; }
	#endregion
}
