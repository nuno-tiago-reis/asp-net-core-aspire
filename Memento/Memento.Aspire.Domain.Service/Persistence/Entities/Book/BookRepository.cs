namespace Memento.Aspire.Domain.Service.Persistence.Entities.Book;

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
/// Implements the interface for a 'Book' repository.
/// Provides methods to interact with the books (CRUD and more).
/// </summary>
///
/// <seealso cref="Book" />
/// <seealso cref="BookFilter" />
/// <seealso cref="BookOrderBy" />
/// <seealso cref="BookOrderDirection" />
public sealed class BookRepository : EntityRepository<Book, BookFilter, BookOrderBy, BookOrderDirection>, IBookRepository
{
	#region [Constructors]
	/// <summary>
	/// Initializes a new instance of the <see cref="BookRepository"/> class.
	/// </summary>
	///
	/// <param name="context">The context.</param>
	/// <param name="localizer">The localizer.</param>
	/// <param name="logger">The logger.</param>
	public BookRepository
	(
		DomainContext context,
		ILocalizer localizer,
		ILogger<BookRepository> logger
	)
	: base(context, localizer, logger)
	{
		// Nothing to do here.
	}
	#endregion

	#region [Methods] Entity
	/// <inheritdoc />
	protected override void ValidateEntity(Book book)
	{
		var errorMessages = new List<string>();

		// Required fields
		if (string.IsNullOrWhiteSpace(book.Name))
		{
			errorMessages.Add(this.GetEntityHasInvalidFieldMessage((book) => book.Name));
		}

		if (book.ReleaseDate == default)
		{
			errorMessages.Add(this.GetEntityHasInvalidFieldMessage((book) => book.ReleaseDate));
		}

		if (book.AuthorId == default)
		{
			errorMessages.Add(this.GetEntityHasInvalidFieldMessage((book) => book.AuthorId));
		}

		if (book.GenreId == default)
		{
			errorMessages.Add(this.GetEntityHasInvalidFieldMessage((book) => book.GenreId));
		}

		// Duplicate fields
		if (this.Entities.Any((existingBook) => existingBook.Id != book.Id && existingBook.Name == book.Name))
		{
			errorMessages.Add(this.GetEntityHasDuplicateFieldCombinationMessage((book) => book.Name, (book) => book.ReleaseDate));
		}

		if (errorMessages.Count > 0)
		{
			throw new StandardException(errorMessages, StandardExceptionType.BadRequest);
		}
	}

	/// <inheritdoc />
	protected override void UpdateEntity(Book sourceBook, Book targetBook)
	{
		// Book
		targetBook.Name = sourceBook.Name;
		targetBook.AuthorId = sourceBook.AuthorId;
		targetBook.GenreId = sourceBook.GenreId;

		// Entity
		targetBook.UpdatedBy = sourceBook.UpdatedBy;
	}
	#endregion

	#region [Methods] Queryable
	/// <inheritdoc />
	protected override IQueryable<Book> GetEntitySummaryQueryable()
	{
		return this.Entities;
	}

	/// <inheritdoc />
	protected override IQueryable<Book> GetEntityDetailQueryable()
	{
		return this.Entities
			.Include((book) => book.Author)
			.Include((book) => book.Genre);
	}

	/// <inheritdoc />
	protected override IQueryable<Book> GetEntityCountQueryable()
	{
		return this.Entities;
	}

	/// <inheritdoc />
	protected override IQueryable<Book> FilterQueryable(IQueryable<Book> queryable, BookFilter filter)
	{
		// Apply the filter
		if (string.IsNullOrWhiteSpace(filter.Name) == false)
		{
			var name = filter.Name;

			queryable = queryable.Where((book) => EF.Functions.Like(book.Name, $"%{name}%"));
		}

		if (string.IsNullOrWhiteSpace(filter.Author) == false)
		{
			var authorName = filter.Author;

			queryable = queryable.Where((book) => EF.Functions.Like(book.Author!.Name, $"%{authorName}%"));
		}

		if (string.IsNullOrWhiteSpace(filter.Genre) == false)
		{
			var genreName = filter.Genre;

			queryable = queryable.Where((book) => EF.Functions.Like(book.Genre!.Name, $"%{genreName}%"));
		}

		if (filter.ReleasedAfter is not null)
		{
			var releaseDate = filter.ReleasedAfter.Value;

			queryable = queryable.Where((book) => book.ReleaseDate >= releaseDate);
		}

		if (filter.ReleasedBefore is not null)
		{
			var releaseDate = filter.ReleasedBefore.Value;

			queryable = queryable.Where((book) => book.ReleaseDate <= releaseDate);
		}

		// Apply the order
		switch (filter.OrderBy)
		{
			case BookOrderBy.Id:
			{
				queryable = OrderQueryable(queryable, filter, (book) => book.Id);
				break;
			}
			case BookOrderBy.CreatedAt:
			{
				queryable = OrderQueryable(queryable, filter, (book) => book.CreatedAt);
				break;
			}
			case BookOrderBy.UpdatedAt:
			{
				queryable = OrderQueryable(queryable, filter, (book) => book.UpdatedAt);
				break;
			}
			case BookOrderBy.Name:
			{
				queryable = OrderQueryable(queryable, filter, (book) => book.Name);
				break;
			}
			case BookOrderBy.ReleaseDate:
			{
				queryable = OrderQueryable(queryable, filter, (book) => book.ReleaseDate);
				break;
			}
			case BookOrderBy.Author:
			{
				queryable = OrderQueryable(queryable, filter, (book) => book.Author!.Name);
				break;
			}
			case BookOrderBy.Genre:
			{
				queryable = OrderQueryable(queryable, filter, (book) => book.Genre!.Name);
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
	private static IQueryable<Book> OrderQueryable<TProperty>(IQueryable<Book> queryable, BookFilter filter, Expression<Func<Book, TProperty>> expression)
	{
		switch (filter.OrderDirection)
		{
			case BookOrderDirection.Ascending:
			{
				return queryable.OrderBy(expression);
			}
			case BookOrderDirection.Descending:
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
		var name = $"'{this.Localizer.GetString(SharedResources.BOOK)!}'";

		return this.Localizer.GetString(SharedResources.ERROR_NOT_FOUND, name);
	}

	/// <inheritdoc />
	protected override string GetEntityHasDuplicateFieldMessage<TProperty>(Expression<Func<Book, TProperty>> expression)
	{
		// Get the name
		var name = $"'{expression.GetDisplayName()!}'";

		return this.Localizer.GetString(SharedResources.ERROR_DUPLICATE_FIELD, name);
	}

	/// <inheritdoc />
	protected override string GetEntityHasDuplicateFieldCombinationMessage(params Expression<Func<Book, object>>[] expressions)
	{
		// Get the names
		var names = string.Join("', '", expressions.Select((expression) => expression.GetDisplayName()!).Take(expressions.Length - 1)) + $"' and '{expressions.Last().GetDisplayName()}'";

		return this.Localizer.GetString(SharedResources.ERROR_DUPLICATE_FIELD_COMBINATION, names);
	}

	/// <inheritdoc />
	protected override string GetEntityHasInvalidFieldMessage<TProperty>(Expression<Func<Book, TProperty>> expression)
	{
		// Get the name
		var name = $"'{expression.GetDisplayName()!}'";

		return this.Localizer.GetString(SharedResources.ERROR_INVALID_FIELD, name);
	}
	#endregion
}
