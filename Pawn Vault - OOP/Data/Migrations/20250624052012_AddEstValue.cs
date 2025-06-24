using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawn_Vault___OOP.Migrations
{
    /// <inheritdoc />
    public partial class AddEstValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "EstimatedValue",
                table: "Loans",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.DropColumn(
                name: "EstimatedValue",
                table: "Loans");
        }
    }
}
