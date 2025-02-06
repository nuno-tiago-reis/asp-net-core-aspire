namespace Memento.Aspire.Domain.Service.Persistence;

using Memento.Aspire.Domain.Service.Persistence.Entities.Author;
using Memento.Aspire.Domain.Service.Persistence.Entities.Book;
using Memento.Aspire.Domain.Service.Persistence.Entities.Genre;
using Memento.Aspire.Shared.Persistence;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Implements the domain entity context.
/// </summary>
///
/// <seealso cref="EntityContext"/>
internal sealed class DomainContext : EntityContext
{
	#region [Properties]
		/// <summary>
	/// Gets or sets the 'Author' database set.
	/// </summary>
	public DbSet<Author> Authors { get; init; }

	/// <summary>
	/// Gets or sets the 'Book' database set.
	/// </summary>
	public DbSet<Book> Books { get; init; }

	/// <summary>
	/// Gets or sets the 'Genre' database set.
	/// </summary>
	public DbSet<Genre> Genres { get; init; }
	#endregion

	#region [Constructors]
	/// <summary>
	/// Initializes a new instance of the <see cref="DomainContext"/> class.
	/// </summary>
	///
	/// <param name="options">The options.</param>
	public DomainContext(DbContextOptions<DomainContext> options) : base(options)
	{
		// Intentionally Empty.
	}
	#endregion

	#region [Methods]
	/// <inheritdoc />
	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		// Configurations (Models)
		builder.ApplyConfiguration(new AuthorConfiguration());
		builder.ApplyConfiguration(new BookConfiguration());
		builder.ApplyConfiguration(new GenreConfiguration());
	}
	#endregion
}
