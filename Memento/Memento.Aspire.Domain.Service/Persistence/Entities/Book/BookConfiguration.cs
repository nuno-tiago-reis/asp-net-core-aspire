namespace Memento.Aspire.Domain.Service.Persistence.Entities.Book;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

/// <summary>
/// Implements the 'Book' configuration.
/// </summary>
///
/// <seealso cref="Book" />
internal sealed class BookConfiguration : IEntityTypeConfiguration<Book>
{
	#region [Constants]
	/// <summary>
	/// The maximum length for the name column.
	/// </summary>
	public const int NAME_MAXIMUM_LENGTH = 200;
	#endregion

	#region [Methods]
	/// <inheritdoc />
	public void Configure(EntityTypeBuilder<Book> builder)
	{
		// Keys
		builder.HasKey((book) => book.Id);

		// Indices (Book)
		builder.HasIndex((book) => book.Name).IsUnique(false);
		builder.HasIndex((book) => book.ReleaseDate).IsUnique(false);
		builder.HasIndex((book) => book.AuthorId).IsUnique(false);
		builder.HasIndex((book) => book.GenreId).IsUnique(false);

		// Indices (Compound)
		builder.HasIndex((book) => new { book.Name, book.ReleaseDate }).IsUnique();

		// Properties (Book)
		builder.Property((book) => book.Name).IsRequired(true).HasMaxLength(NAME_MAXIMUM_LENGTH);
		builder.Property((book) => book.ReleaseDate).IsRequired(true);
		builder.Property((book) => book.AuthorId).IsRequired(true);
		builder.Property((book) => book.GenreId).IsRequired(true);

		// Properties (Entity)
		builder.Property((book) => book.CreatedBy).IsRequired(true);
		builder.Property((book) => book.CreatedAt).IsRequired(true);
		builder.Property((book) => book.UpdatedBy).IsRequired(false);
		builder.Property((book) => book.UpdatedAt).IsRequired(false);
	}
	#endregion
}
