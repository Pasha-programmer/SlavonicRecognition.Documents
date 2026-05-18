using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Documents.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddSelectedModelType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SelectedModelType",
                table: "Documents",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SelectedModelType",
                table: "Documents");
        }
    }
}
