namespace Memento.Aspire.Domain.Service.Persistence.Entities.Author;

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
/// Implements the interface for a 'Author' repository.
/// Provides methods to interact with the authors (CRUD and more).
/// </summary>
///
/// <seealso cref="Author" />
/// <seealso cref="AuthorFilter" />
/// <seealso cref="AuthorOrderBy" />
/// <seealso cref="AuthorOrderDirection" />
public sealed class AuthorRepository : EntityRepository<Author, AuthorFilter, AuthorOrderBy, AuthorOrderDirection>, IAuthorRepository
{
	#region [Constructors]
	/// <summary>
	/// Initializes a new instance of the <see cref="AuthorRepository"/> class.
	/// </summary>
	///
	/// <param name="context">The context.</param>
	/// <param name="localizer">The localizer.</param>
	/// <param name="logger">The logger.</param>
	public AuthorRepository
	(
		DomainContext context,
		ILocalizer localizer,
		ILogger<AuthorRepository> logger
	)
	: base(context, localizer, logger)
	{
		// Nothing to do here.
	}
	#endregion

	#region [Methods] Entity
	/// <inheritdoc />
	protected override void ValidateEntity(Author author)
	{
		var errorMessages = new List<string>();

		// Required fields
		if (string.IsNullOrWhiteSpace(author.Name))
		{
			errorMessages.Add(this.GetEntityHasInvalidFieldMessage((author) => author.Name));
		}

		if (author.BirthDate == default)
		{
			errorMessages.Add(this.GetEntityHasInvalidFieldMessage((author) => author.BirthDate));
		}

		// Duplicate fields
		if (this.Entities.Any((existingAuthor) => existingAuthor.Id != author.Id && existingAuthor.Name == author.Name && existingAuthor.BirthDate == author.BirthDate))
		{
			errorMessages.Add(this.GetEntityHasDuplicateFieldCombinationMessage((author) => author.Name, (author) => author.BirthDate));
		}

		if (errorMessages.Count > 0)
		{
			throw new StandardException(errorMessages, StandardExceptionType.BadRequest);
		}
	}

	/// <inheritdoc />
	protected override void UpdateEntity(Author sourceAuthor, Author targetAuthor)
	{
		// Author
		targetAuthor.Name = sourceAuthor.Name;
		targetAuthor.BirthDate = sourceAuthor.BirthDate;

		// Entity
		targetAuthor.UpdatedBy = sourceAuthor.UpdatedBy;
	}
	#endregion

	#region [Methods] Queryable
	/// <inheritdoc />
	protected override IQueryable<Author> GetEntitySummaryQueryable()
	{
		return this.Entities;
	}

	/// <inheritdoc />
	protected override IQueryable<Author> GetEntityDetailQueryable()
	{
		return this.Entities
			.Include((author) => author.Books)
			.ThenInclude(book => book.Genre);
	}

	/// <inheritdoc />
	protected override IQueryable<Author> GetEntityCountQueryable()
	{
		return this.Entities;
	}

	/// <inheritdoc />
	protected override IQueryable<Author> FilterQueryable(IQueryable<Author> queryable, AuthorFilter filter)
	{
		// Apply the filter
		if (string.IsNullOrWhiteSpace(filter.Name) == false)
		{
			var name = filter.Name;

			queryable = queryable.Where((author) => EF.Functions.Like(author.Name, $"%{name}%"));
		}

		if (filter.BornAfter is not null)
		{
			var birthDate = filter.BornAfter.Value;

			queryable = queryable.Where((author) => author.BirthDate >= birthDate);
		}

		if (filter.BornBefore is not null)
		{
			var birthDate = filter.BornBefore.Value;

			queryable = queryable.Where((author) => author.BirthDate <= birthDate);
		}

		// Apply the order
		switch (filter.OrderBy)
		{
			case AuthorOrderBy.Id:
			{
				queryable = OrderQueryable(queryable, filter, (author) => author.Id);
				break;
			}
			case AuthorOrderBy.CreatedAt:
			{
				queryable = OrderQueryable(queryable, filter, (author) => author.CreatedAt);
				break;
			}
			case AuthorOrderBy.UpdatedAt:
			{
				queryable = OrderQueryable(queryable, filter, (author) => author.UpdatedAt);
				break;
			}
			case AuthorOrderBy.Name:
			{
				queryable = OrderQueryable(queryable, filter, (author) => author.Name);
				break;
			}
			case AuthorOrderBy.BirthDate:
			{
				queryable = OrderQueryable(queryable, filter, (author) => author.BirthDate);
				break;
			}
			default:
			{
				throw new InvalidEnumArgumentException(nameof(filter.OrderBy));
			}
		}

		return queryable;
	}

	/// <summary>
	/// Returns an ordered queryable according to the filters OrderDirection and expressions Property.
	/// </summary>
	///
	/// <typeparam name="TProperty">The property's type.</typeparam>
	///
	/// <param name="queryable">The author queryable.</param>
	/// <param name="filter">The author filter.</param>
	/// <param name="expression">The author expression</param>
	private static IQueryable<Author> OrderQueryable<TProperty>(IQueryable<Author> queryable, AuthorFilter filter, Expression<Func<Author, TProperty>> expression)
	{
		switch (filter.OrderDirection)
		{
			case AuthorOrderDirection.Ascending:
			{
				return queryable.OrderBy(expression);
			}
			case AuthorOrderDirection.Descending:
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
		var name = $"'{this.Localizer.GetString(SharedResources.AUTHOR)!}'";

		return this.Localizer.GetString(SharedResources.ERROR_NOT_FOUND, name);
	}

	/// <inheritdoc />
	protected override string GetEntityHasDuplicateFieldMessage<TProperty>(Expression<Func<Author, TProperty>> expression)
	{
		// Get the name
		var name = $"'{expression.GetDisplayName()!}'";

		return this.Localizer.GetString(SharedResources.ERROR_DUPLICATE_FIELD, name);
	}

	/// <inheritdoc />
	protected override string GetEntityHasDuplicateFieldCombinationMessage(params Expression<Func<Author, object>>[] expressions)
	{
		// Get the names
		var names = string.Join("', '", expressions.Select((expression) => expression.GetDisplayName()!).Take(expressions.Length - 1)) + $"' and '{expressions.Last().GetDisplayName()}'";

		return this.Localizer.GetString(SharedResources.ERROR_DUPLICATE_FIELD_COMBINATION, names);
	}

	/// <inheritdoc />
	protected override string GetEntityHasInvalidFieldMessage<TProperty>(Expression<Func<Author, TProperty>> expression)
	{
		// Get the name
		var name = $"'{expression.GetDisplayName()!}'";

		return this.Localizer.GetString(SharedResources.ERROR_INVALID_FIELD, name);
	}
	#endregion
}
