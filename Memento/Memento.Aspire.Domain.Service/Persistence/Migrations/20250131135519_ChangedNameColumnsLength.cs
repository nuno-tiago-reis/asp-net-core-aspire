namespace Memento.Aspire.Domain.Service.Migrations;

using Microsoft.EntityFrameworkCore.Migrations;

/// <inheritdoc />
public partial class ChangedNameColumnsLength : Migration
{
	/// <inheritdoc />
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.AlterColumn<string>
		(
			name: "Name",
			table: "Genres",
			type: "nvarchar(200)",
			maxLength: 200,
			nullable: false,
			oldClrType: typeof(string),
			oldType: "nvarchar(50)",
			oldMaxLength: 50
		);

		migrationBuilder.AlterColumn<string>
		(
			name: "Name",
			table: "Books",
			type: "nvarchar(200)",
			maxLength: 200,
			nullable: false,
			oldClrType: typeof(string),
			oldType: "nvarchar(50)",
			oldMaxLength: 50
		);

		migrationBuilder.AlterColumn<string>
		(
			name: "Name",
			table: "Authors",
			type: "nvarchar(200)",
			maxLength: 200,
			nullable: false,
			oldClrType: typeof(string),
			oldType: "nvarchar(50)",
			oldMaxLength: 50
		);
	}

	/// <inheritdoc />
	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.AlterColumn<string>
		(
			name: "Name",
			table: "Genres",
			type: "nvarchar(50)",
			maxLength: 50,
			nullable: false,
			oldClrType: typeof(string),
			oldType: "nvarchar(200)",
			oldMaxLength: 200
		);

		migrationBuilder.AlterColumn<string>
		(
			name: "Name",
			table: "Books",
			type: "nvarchar(50)",
			maxLength: 50,
			nullable: false,
			oldClrType: typeof(string),
			oldType: "nvarchar(200)",
			oldMaxLength: 200
		);

		migrationBuilder.AlterColumn<string>
		(
			name: "Name",
			table: "Authors",
			type: "nvarchar(50)",
			maxLength: 50,
			nullable: false,
			oldClrType: typeof(string),
			oldType: "nvarchar(200)",
			oldMaxLength: 200
		);
	}
}
