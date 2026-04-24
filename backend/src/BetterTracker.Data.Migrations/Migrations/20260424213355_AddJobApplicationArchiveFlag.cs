using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BetterTracker.Data.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddJobApplicationArchiveFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JobApplications_UserId_CreatedAt",
                schema: "Default",
                table: "JobApplications");

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                schema: "Default",
                table: "JobApplications",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_JobApplications_UserId_IsArchived_CreatedAt",
                schema: "Default",
                table: "JobApplications",
                columns: new[] { "UserId", "IsArchived", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JobApplications_UserId_IsArchived_CreatedAt",
                schema: "Default",
                table: "JobApplications");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                schema: "Default",
                table: "JobApplications");

            migrationBuilder.CreateIndex(
                name: "IX_JobApplications_UserId_CreatedAt",
                schema: "Default",
                table: "JobApplications",
                columns: new[] { "UserId", "CreatedAt" });
        }
    }
}
