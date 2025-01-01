namespace Memento.Aspire.Domain.Service.Migrations;

using Microsoft.EntityFrameworkCore.Migrations;

/// <inheritdoc />
public partial class InitialMigration : Migration
{
	/// <inheritdoc />
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.CreateTable
		(
			name: "Authors",
			columns: (table) => new
			{
				Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
				Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
				BirthDate = table.Column<DateOnly>(type: "date", nullable: false),
				CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
				CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
				UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
				UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
			},
			constraints: (table) =>
			{
				table.PrimaryKey("PK_Authors", (author) => author.Id);
			}
		);

		migrationBuilder.CreateTable
		(
			name: "Genres",
			columns: (table) => new
			{
				Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
				Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
				CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
				CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
				UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
				UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
			},
			constraints: (table) =>
			{
				table.PrimaryKey("PK_Genres", (genre) => genre.Id);
			}
		);

		migrationBuilder.CreateTable
		(
			name: "Books",
			columns: (table) => new
			{
				Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
				Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
				ReleaseDate = table.Column<DateOnly>(type: "date", nullable: false),
				AuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
				GenreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
				CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
				CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
				UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
				UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
			},
			constraints: (table) =>
			{
				table.PrimaryKey("PK_Books", (book) => book.Id);
				table.ForeignKey(
					name: "FK_Books_Authors_AuthorId",
					column: (book) => book.AuthorId,
					principalTable: "Authors",
					principalColumn: "Id",
					onDelete: ReferentialAction.Cascade);
				table.ForeignKey(
					name: "FK_Books_Genres_GenreId",
					column: (book) => book.GenreId,
					principalTable: "Genres",
					principalColumn: "Id",
					onDelete: ReferentialAction.Cascade);
			}
		);

		migrationBuilder.CreateIndex
		(
			name: "IX_Authors_BirthDate",
			table: "Authors",
			column: "BirthDate"
		);

		migrationBuilder.CreateIndex
		(
			name: "IX_Authors_Name",
			table: "Authors",
			column: "Name"
		);

		#pragma warning disable CA1861 // Avoid constant arrays as arguments
		migrationBuilder.CreateIndex
		(
			name: "IX_Authors_Name_BirthDate",
			table: "Authors",
			columns: new[] { "Name", "BirthDate" },
			unique: true
		);
		#pragma warning restore CA1861 // Avoid constant arrays as arguments

		migrationBuilder.CreateIndex
		(
			name: "IX_Books_AuthorId",
			table: "Books",
			column: "AuthorId"
		);

		migrationBuilder.CreateIndex
		(
			name: "IX_Books_GenreId",
			table: "Books",
			column: "GenreId"
		);

		migrationBuilder.CreateIndex
		(
			name: "IX_Books_Name",
			table: "Books",
			column: "Name"
		);

		#pragma warning disable CA1861 // Avoid constant arrays as arguments
		migrationBuilder.CreateIndex
		(
			name: "IX_Books_Name_ReleaseDate",
			table: "Books",
			columns: new[] { "Name", "ReleaseDate" },
			unique: true
		);
		#pragma warning restore CA1861 // Avoid constant arrays as arguments

		migrationBuilder.CreateIndex
		(
			name: "IX_Books_ReleaseDate",
			table: "Books",
			column: "ReleaseDate"
		);

		migrationBuilder.CreateIndex
		(
			name: "IX_Genres_Name",
			table: "Genres",
			column: "Name",
			unique: true
		);
	}

	/// <inheritdoc />
	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropTable
		(
			name: "Books"
		);

		migrationBuilder.DropTable
		(
			name: "Authors"
		);

		migrationBuilder.DropTable
		(
			name: "Genres"
		);
	}
}
