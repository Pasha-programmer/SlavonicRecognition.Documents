using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Documents.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDocumentPredictions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Prob",
                table: "DocumentPredictions",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Prob",
                table: "DocumentPredictions");
        }
    }
}
