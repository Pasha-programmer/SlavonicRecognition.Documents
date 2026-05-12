using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Documents.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddModelType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ModelType",
                table: "DocumentPredictions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModelType",
                table: "DocumentPredictions");
        }
    }
}
