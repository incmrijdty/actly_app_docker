using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Actly.API.Migrations
{
    /// <inheritdoc />
    public partial class FinalChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Feedback",
                table: "Participations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Feedback",
                table: "Participations",
                type: "text",
                nullable: true);
        }
    }
}
