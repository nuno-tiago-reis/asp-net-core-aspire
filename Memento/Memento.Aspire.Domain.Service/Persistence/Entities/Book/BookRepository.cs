namespace Memento.Aspire.Domain.Service.Persistence.Entities.Book;

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
		if (this.Entities.Any((book) => book.Id != book.Id && book.Name.Equals(book.Name)))
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
	protected override IQueryable<Book> FilterQueryable(IQueryable<Book> bookQueryable, BookFilter bookFilter)
	{
		// Apply the filter
		if (string.IsNullOrWhiteSpace(bookFilter.Name) == false)
		{
			var name = bookFilter.Name;

			bookQueryable = bookQueryable.Where((book) => EF.Functions.Like(book.Name, $"%{name}%"));
		}

		if (string.IsNullOrWhiteSpace(bookFilter.Author) == false)
		{
			var authorName = bookFilter.Author;

			bookQueryable = bookQueryable.Where((book) => EF.Functions.Like(book.Author!.Name, $"%{authorName}%"));
		}

		if (string.IsNullOrWhiteSpace(bookFilter.Genre) == false)
		{
			var genreName = bookFilter.Genre;

			bookQueryable = bookQueryable.Where((book) => EF.Functions.Like(book.Genre!.Name, $"%{genreName}%"));
		}

		if (bookFilter.ReleasedAfter is not null)
		{
			var releaseDate = bookFilter.ReleasedAfter.Value;

			bookQueryable = bookQueryable.Where((book) => book.ReleaseDate >= releaseDate);
		}

		if (bookFilter.ReleasedBefore is not null)
		{
			var releaseDate = bookFilter.ReleasedBefore.Value;

			bookQueryable = bookQueryable.Where((book) => book.ReleaseDate <= releaseDate);
		}

		// Apply the order
		switch (bookFilter.OrderBy)
		{
			case BookOrderBy.Id:
			{
				bookQueryable = OrderQueryable(bookQueryable, bookFilter, (book) => book.Id);
				break;
			}
			case BookOrderBy.CreatedAt:
			{
				bookQueryable = OrderQueryable(bookQueryable, bookFilter, (book) => book.CreatedAt);
				break;
			}
			case BookOrderBy.UpdatedAt:
			{
				bookQueryable = OrderQueryable(bookQueryable, bookFilter, (book) => book.UpdatedAt);
				break;
			}
			case BookOrderBy.Name:
			{
				bookQueryable = OrderQueryable(bookQueryable, bookFilter, (book) => book.Name);
				break;
			}
			case BookOrderBy.ReleaseDate:
			{
				bookQueryable = OrderQueryable(bookQueryable, bookFilter, (book) => book.ReleaseDate);
				break;
			}
			case BookOrderBy.Author:
			{
				bookQueryable = OrderQueryable(bookQueryable, bookFilter, (book) => book.Author!.Name);
				break;
			}
			case BookOrderBy.Genre:
			{
				bookQueryable = OrderQueryable(bookQueryable, bookFilter, (book) => book.Genre!.Name);
				break;
			}
			default:
			{
				throw new InvalidEnumArgumentException(nameof(bookFilter.OrderBy));
			}
		}

		return bookQueryable;
	}

	/// <inheritdoc />
	private static IQueryable<Book> OrderQueryable<TProperty>(IQueryable<Book> bookQueryable, BookFilter bookFilter, Expression<Func<Book, TProperty>> bookExpression)
	{
		switch (bookFilter.OrderDirection)
		{
			case BookOrderDirection.Ascending:
			{
				return bookQueryable.OrderBy(bookExpression);
			}
			case BookOrderDirection.Descending:
			{
				return bookQueryable.OrderByDescending(bookExpression);
			}
			default:
			{
				throw new InvalidEnumArgumentException(nameof(bookFilter.OrderDirection));
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
