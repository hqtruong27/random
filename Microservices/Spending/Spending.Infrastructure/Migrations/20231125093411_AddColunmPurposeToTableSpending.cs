using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spending.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnPurposeToTableSpending : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Purpose",
                table: "Spendings",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Purpose",
                table: "Spendings");
        }
    }
}
