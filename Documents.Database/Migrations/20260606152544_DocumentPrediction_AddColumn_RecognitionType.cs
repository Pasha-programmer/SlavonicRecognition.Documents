using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Documents.Database.Migrations
{
    /// <inheritdoc />
    public partial class DocumentPrediction_AddColumn_RecognitionType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RecognitionType",
                table: "DocumentPredictions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 2);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecognitionType",
                table: "DocumentPredictions");
        }
    }
}
