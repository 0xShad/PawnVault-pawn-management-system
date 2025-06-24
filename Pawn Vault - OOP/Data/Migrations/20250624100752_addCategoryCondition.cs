using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawn_Vault___OOP.Migrations
{
    /// <inheritdoc />
    public partial class addCategoryCondition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Loans",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Condition",
                table: "Loans",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "Condition",
                table: "Loans");
        }
    }
}
