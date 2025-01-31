﻿namespace Memento.Aspire.Domain.Service.Contracts.Author;

using Memento.Aspire.Shared.Resources;
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
	public required string? Name { get; set; }

	/// <summary>
	/// The author's birth date.
	/// </summary>
	[Required]
	[Display(Name = nameof(SharedResources.AUTHOR_BIRTHDATE), ResourceType = typeof(SharedResources))]
	public required DateOnly? BirthDate { get; set; }
	#endregion
}
