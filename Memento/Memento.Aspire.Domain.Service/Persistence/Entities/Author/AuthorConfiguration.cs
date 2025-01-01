namespace Memento.Aspire.Domain.Service.Persistence.Entities.Author;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

/// <summary>
/// Implements the 'Author' configuration.
/// </summary>
///
/// <seealso cref="Author" />
public sealed class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
	#region [Constants]
	/// <summary>
	/// The maximum length for the name column.
	/// </summary>
	public const int NAME_MAXIMUM_LENGTH = 200;
	#endregion

	#region [Methods]
	/// <inheritdoc />
	public void Configure(EntityTypeBuilder<Author> builder)
	{
		// Keys
		builder.HasKey((author) => author.Id);

		// Indices (Account)
		builder.HasIndex((author) => author.Name).IsUnique(false);
		builder.HasIndex((author) => author.BirthDate).IsUnique(false);

		// Indices (Compound)
		builder.HasIndex((author) => new { author.Name, author.BirthDate }).IsUnique();

		// Properties (Account)
		builder.Property((author) => author.Name).IsRequired(true).HasMaxLength(NAME_MAXIMUM_LENGTH);
		builder.Property((author) => author.BirthDate).IsRequired(true);

		// Properties (Entity)
		builder.Property((author) => author.CreatedBy).IsRequired(true);
		builder.Property((author) => author.CreatedAt).IsRequired(true);
		builder.Property((author) => author.UpdatedBy).IsRequired(false);
		builder.Property((author) => author.UpdatedAt).IsRequired(false);
	}
	#endregion
}
