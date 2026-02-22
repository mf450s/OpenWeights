using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Weights.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExerciseLaterality : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Laterality",
                table: "Exercises",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Bilateral");

            migrationBuilder.AddColumn<string>(
                name: "Side",
                table: "SetHistories",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Laterality",
                table: "Exercises");

            migrationBuilder.DropColumn(
                name: "Side",
                table: "SetHistories");
        }
    }
}
