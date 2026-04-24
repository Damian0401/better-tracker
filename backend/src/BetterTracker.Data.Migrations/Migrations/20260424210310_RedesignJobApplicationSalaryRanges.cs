using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BetterTracker.Data.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class RedesignJobApplicationSalaryRanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SalaryPost",
                schema: "Default",
                table: "JobApplicationSalaries",
                newName: "OfferFrom");

            migrationBuilder.RenameColumn(
                name: "SalaryCandidate",
                schema: "Default",
                table: "JobApplicationSalaries",
                newName: "ExpectedFrom");

            migrationBuilder.AddColumn<decimal>(
                name: "OfferTo",
                schema: "Default",
                table: "JobApplicationSalaries",
                type: "TEXT",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ExpectedTo",
                schema: "Default",
                table: "JobApplicationSalaries",
                type: "TEXT",
                precision: 18,
                scale: 2,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OfferTo",
                schema: "Default",
                table: "JobApplicationSalaries");

            migrationBuilder.DropColumn(
                name: "ExpectedTo",
                schema: "Default",
                table: "JobApplicationSalaries");

            migrationBuilder.RenameColumn(
                name: "OfferFrom",
                schema: "Default",
                table: "JobApplicationSalaries",
                newName: "SalaryPost");

            migrationBuilder.RenameColumn(
                name: "ExpectedFrom",
                schema: "Default",
                table: "JobApplicationSalaries",
                newName: "SalaryCandidate");
        }
    }
}
