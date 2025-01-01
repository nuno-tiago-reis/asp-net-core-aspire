namespace Memento.Aspire.Domain.Service.Persistence.Entities.Genre;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

/// <summary>
/// Implements the 'Genre' configuration.
/// </summary>
///
/// <seealso cref="Genre" />
public sealed class GenreConfiguration : IEntityTypeConfiguration<Genre>
{
	#region [Constants]
	/// <summary>
	/// The maximum length for the name column.
	/// </summary>
	public const int NAME_MAXIMUM_LENGTH = 200;
	#endregion

	#region [Methods]
	/// <inheritdoc />
	public void Configure(EntityTypeBuilder<Genre> builder)
	{
		// Keys
		builder.HasKey((genre) => genre.Id);

		// Indices (Account)
		builder.HasIndex((genre) => genre.Name).IsUnique(true);

		// Properties (Account)
		builder.Property((genre) => genre.Name).IsRequired(true).HasMaxLength(NAME_MAXIMUM_LENGTH);

		// Properties (Entity)
		builder.Property((genre) => genre.CreatedBy).IsRequired(true);
		builder.Property((genre) => genre.CreatedAt).IsRequired(true);
		builder.Property((genre) => genre.UpdatedBy).IsRequired(false);
		builder.Property((genre) => genre.UpdatedAt).IsRequired(false);
	}
	#endregion
}
