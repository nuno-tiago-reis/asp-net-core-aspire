namespace Memento.Aspire.Domain.Service.Persistence.Entities.Genre;

using Memento.Aspire.Shared.Exceptions;
using Memento.Aspire.Shared.Extensions;
using Memento.Aspire.Shared.Localization;
using Memento.Aspire.Shared.Persistence;
using Memento.Aspire.Shared.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

/// <summary>
/// Implements the interface for a 'Genre' repository.
/// Provides methods to interact with the genres (CRUD and more).
/// </summary>
///
/// <seealso cref="Genre" />
/// <seealso cref="GenreFilter" />
/// <seealso cref="GenreOrderBy" />
/// <seealso cref="GenreOrderDirection" />
public sealed class GenreRepository : EntityRepository<Genre, GenreFilter, GenreOrderBy, GenreOrderDirection>, IGenreRepository
{
	#region [Constructors]
	/// <summary>
	/// Initializes a new instance of the <see cref="GenreRepository"/> class.
	/// </summary>
	///
	/// <param name="context">The context.</param>
	/// <param name="localizer">The localizer.</param>
	/// <param name="logger">The logger.</param>
	public GenreRepository
	(
		DomainContext context,
		ILocalizer localizer,
		ILogger<GenreRepository> logger
	)
	: base(context, localizer, logger)
	{
		// Nothing to do here.
	}
	#endregion

	#region [Methods] Entity
	/// <inheritdoc />
	protected override void NormalizeEntity(Genre genre)
	{
		// Intentionally Empty.
	}

	/// <inheritdoc />
	protected override void ValidateEntity(Genre genre)
	{
		var errorMessages = new List<string>();

		// Required fields
		if (string.IsNullOrWhiteSpace(genre.Name))
		{
			errorMessages.Add(this.GetEntityHasInvalidFieldMessage((genre) => genre.Name));
		}

		// Duplicate fields
		if (this.Entities.Any((genre) => genre.Id != genre.Id && genre.Name.Equals(genre.Name, StringComparison.OrdinalIgnoreCase)))
		{
			errorMessages.Add(this.GetEntityHasDuplicateFieldMessage((genre) => genre.Name));
		}

		if (errorMessages.Count > 0)
		{
			throw new StandardException(errorMessages, StandardExceptionType.BadRequest);
		}
	}

	/// <inheritdoc />
	protected override void UpdateEntity(Genre sourceGenre, Genre targetGenre)
	{
		// Genre
		targetGenre.Name = sourceGenre.Name;

		// Entity
		targetGenre.UpdatedBy = sourceGenre.UpdatedBy;
	}
	#endregion

	#region [Methods] Queryable
	/// <inheritdoc />
	protected override IQueryable<Genre> GetEntitySummaryQueryable()
	{
		return this.Entities;
	}

	/// <inheritdoc />
	protected override IQueryable<Genre> GetEntityDetailQueryable()
	{
		return this.Entities
			.Include((genre) => genre.Books)
			.ThenInclude(book => book.Author);
	}

	/// <inheritdoc />
	protected override IQueryable<Genre> GetEntityCountQueryable()
	{
		return this.Entities;
	}

	/// <inheritdoc />
	protected override IQueryable<Genre> FilterQueryable(IQueryable<Genre> genreQueryable, GenreFilter genreFilter)
	{
		// Apply the filter
		if (string.IsNullOrWhiteSpace(genreFilter.Name) == false)
		{
			var name = genreFilter.Name;

			genreQueryable = genreQueryable.Where((genre) => EF.Functions.Like(genre.Name, $"%{name}%"));
		}

		// Apply the order
		switch (genreFilter.OrderBy)
		{
			case GenreOrderBy.Id:
			{
				genreQueryable = OrderQueryable(genreQueryable, genreFilter, (genre) => genre.Id);
				break;
			}
			case GenreOrderBy.CreatedAt:
			{
				genreQueryable = OrderQueryable(genreQueryable, genreFilter, (genre) => genre.CreatedAt);
				break;
			}
			case GenreOrderBy.UpdatedAt:
			{
				genreQueryable = OrderQueryable(genreQueryable, genreFilter, (genre) => genre.UpdatedAt);
				break;
			}
			case GenreOrderBy.Name:
			{
				genreQueryable = OrderQueryable(genreQueryable, genreFilter, (genre) => genre.Name);
				break;
			}
			default:
			{
				throw new InvalidEnumArgumentException(nameof(genreFilter.OrderBy));
			}
		}

		return genreQueryable;
	}

	/// <inheritdoc />
	private static IQueryable<Genre> OrderQueryable<TProperty>(IQueryable<Genre> genreQueryable, GenreFilter genreFilter, Expression<Func<Genre, TProperty>> genreExpression)
	{
		switch (genreFilter.OrderDirection)
		{
			case GenreOrderDirection.Ascending:
			{
				return genreQueryable.OrderBy(genreExpression);
			}
			case GenreOrderDirection.Descending:
			{
				return genreQueryable.OrderByDescending(genreExpression);
			}
			default:
			{
				throw new InvalidEnumArgumentException(nameof(genreFilter.OrderDirection));
			}
		}
	}
	#endregion

	#region [Methods] Messages
	/// <inheritdoc />
	protected override string GetEntityDoesNotExistMessage()
	{
		// Get the name
		var name = $"'{this.Localizer.GetString(SharedResources.GENRE)!}'";

		return this.Localizer.GetString(SharedResources.ERROR_NOT_FOUND, name);
	}

	/// <inheritdoc />
	protected override string GetEntityHasDuplicateFieldMessage<TProperty>(Expression<Func<Genre, TProperty>> expression)
	{
		// Get the name
		var name = $"'{expression.GetDisplayName()!}'";

		return this.Localizer.GetString(SharedResources.ERROR_DUPLICATE_FIELD, name);
	}

	/// <inheritdoc />
	protected override string GetEntityHasDuplicateFieldCombinationMessage(params Expression<Func<Genre, object>>[] expressions)
	{
		// Get the names
		var names = string.Join("', '", expressions.Select((expression) => expression.GetDisplayName()!).Take(expressions.Length - 1)) + $"' and '{expressions.Last().GetDisplayName()}'";

		return this.Localizer.GetString(SharedResources.ERROR_DUPLICATE_FIELD_COMBINATION, names);
	}

	/// <inheritdoc />
	protected override string GetEntityHasInvalidFieldMessage<TProperty>(Expression<Func<Genre, TProperty>> expression)
	{
		// Get the name
		var name = $"'{expression.GetDisplayName()!}'";

		return this.Localizer.GetString(SharedResources.ERROR_INVALID_FIELD, name);
	}
	#endregion
}
