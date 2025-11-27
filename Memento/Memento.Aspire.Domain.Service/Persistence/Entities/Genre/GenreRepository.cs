namespace Memento.Aspire.Domain.Service.Persistence.Entities.Genre;

using Memento.Aspire.Core.Exceptions;
using Memento.Aspire.Core.Extensions;
using Memento.Aspire.Core.Localization;
using Memento.Aspire.Core.Persistence;
using Memento.Aspire.Core.Resources;
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
	protected override void ValidateEntity(Genre genre)
	{
		var errorMessages = new List<string>();

		// Required fields
		if (string.IsNullOrWhiteSpace(genre.Name))
		{
			errorMessages.Add(this.GetEntityHasInvalidFieldMessage((genre) => genre.Name));
		}

		// Duplicate fields
		if (this.Entities.Any((existingGenre) => existingGenre.Id != genre.Id && existingGenre.Name == genre.Name))
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
	protected override IQueryable<Genre> FilterQueryable(IQueryable<Genre> queryable, GenreFilter filter)
	{
		// Apply the filter
		if (string.IsNullOrWhiteSpace(filter.Name) == false)
		{
			var name = filter.Name;

			queryable = queryable.Where((genre) => EF.Functions.Like(genre.Name, $"%{name}%"));
		}

		// Apply the order
		switch (filter.OrderBy)
		{
			case GenreOrderBy.Id:
			{
				queryable = OrderQueryable(queryable, filter, (genre) => genre.Id);
				break;
			}
			case GenreOrderBy.CreatedAt:
			{
				queryable = OrderQueryable(queryable, filter, (genre) => genre.CreatedAt);
				break;
			}
			case GenreOrderBy.UpdatedAt:
			{
				queryable = OrderQueryable(queryable, filter, (genre) => genre.UpdatedAt);
				break;
			}
			case GenreOrderBy.Name:
			{
				queryable = OrderQueryable(queryable, filter, (genre) => genre.Name);
				break;
			}
			default:
			{
				throw new InvalidEnumArgumentException(nameof(filter.OrderBy));
			}
		}

		return queryable;
	}

	/// <inheritdoc />
	private static IQueryable<Genre> OrderQueryable<TProperty>(IQueryable<Genre> queryable, GenreFilter filter, Expression<Func<Genre, TProperty>> expression)
	{
		switch (filter.OrderDirection)
		{
			case GenreOrderDirection.Ascending:
			{
				return queryable.OrderBy(expression);
			}
			case GenreOrderDirection.Descending:
			{
				return queryable.OrderByDescending(expression);
			}
			default:
			{
				throw new InvalidEnumArgumentException(nameof(filter.OrderDirection));
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
