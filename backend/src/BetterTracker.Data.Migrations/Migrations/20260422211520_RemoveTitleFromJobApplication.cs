using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BetterTracker.Data.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTitleFromJobApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                schema: "Default",
                table: "JobApplications");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                schema: "Default",
                table: "JobApplications",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }
    }
}
