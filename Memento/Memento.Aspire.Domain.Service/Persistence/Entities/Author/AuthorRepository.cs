namespace Memento.Aspire.Domain.Service.Persistence.Entities.Author;

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
	protected override void NormalizeEntity(Author sourceAuthor)
	{
		// Intentionally Empty.
	}

	/// <inheritdoc />
	protected override void ValidateEntity(Author sourceAuthor)
	{
		var errorMessages = new List<string>();

		// Required fields
		if (string.IsNullOrWhiteSpace(sourceAuthor.Name))
		{
			errorMessages.Add(this.GetEntityHasInvalidFieldMessage((author) => author.Name));
		}
		if (sourceAuthor.BirthDate == default)
		{
			errorMessages.Add(this.GetEntityHasInvalidFieldMessage((author) => author.BirthDate));
		}

		// Duplicate fields
		if (this.Entities.Any((author) => author.Id != sourceAuthor.Id && author.Name.Equals(sourceAuthor.Name) && author.BirthDate == sourceAuthor.BirthDate))
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

	/// <inheritdoc />
	protected override void UpdateEntityRelations(Author sourceAuthor, Author targetAuthor)
	{
		// Nothing to do here.
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
	protected override IQueryable<Author> FilterQueryable(IQueryable<Author> authorQueryable, AuthorFilter authorFilter)
	{
		// Apply the filter
		if (string.IsNullOrWhiteSpace(authorFilter.Name) == false)
		{
			var name = authorFilter.Name;

			authorQueryable = authorQueryable.Where((author) => EF.Functions.Like(author.Name, $"%{name}%"));
		}

		if (authorFilter.BornAfter is not null)
		{
			var birthDate = authorFilter.BornAfter.Value;

			authorQueryable = authorQueryable.Where((author) => author.BirthDate >= birthDate);
		}

		if (authorFilter.BornBefore is not null)
		{
			var birthDate = authorFilter.BornBefore.Value;

			authorQueryable = authorQueryable.Where((author) => author.BirthDate <= birthDate);
		}

		// Apply the order
		switch (authorFilter.OrderBy)
		{
			case AuthorOrderBy.Id:
			{
				authorQueryable = OrderQueryable(authorQueryable, authorFilter, (author) => author.Id);
				break;
			}
			case AuthorOrderBy.CreatedAt:
			{
				authorQueryable = OrderQueryable(authorQueryable, authorFilter, (author) => author.CreatedAt);
				break;
			}
			case AuthorOrderBy.UpdatedAt:
			{
				authorQueryable = OrderQueryable(authorQueryable, authorFilter, (author) => author.UpdatedAt);
				break;
			}
			case AuthorOrderBy.Name:
			{
				authorQueryable = OrderQueryable(authorQueryable, authorFilter, (author) => author.Name);
				break;
			}
			case AuthorOrderBy.BirthDate:
			{
				authorQueryable = OrderQueryable(authorQueryable, authorFilter, (author) => author.BirthDate);
				break;
			}
			default:
			{
				throw new InvalidEnumArgumentException(nameof(authorFilter.OrderBy));
			}
		}

		return authorQueryable;
	}

	/// <summary>
	/// Returns an ordered queryable according to the filters OrderDirection and expressions Property.
	/// </summary>
	///
	/// <typeparam name="TProperty">The property's type.</typeparam>
	///
	/// <param name="authorQueryable">The author queryable.</param>
	/// <param name="authorFilter">The author filter.</param>
	/// <param name="authorExpression">The author expression</param>
	private static IQueryable<Author> OrderQueryable<TProperty>(IQueryable<Author> authorQueryable, AuthorFilter authorFilter, Expression<Func<Author, TProperty>> authorExpression)
	{
		switch (authorFilter.OrderDirection)
		{
			case AuthorOrderDirection.Ascending:
			{
				return authorQueryable.OrderBy(authorExpression);
			}
			case AuthorOrderDirection.Descending:
			{
				return authorQueryable.OrderByDescending(authorExpression);
			}
			default:
			{
				throw new InvalidEnumArgumentException(nameof(authorFilter.OrderDirection));
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
