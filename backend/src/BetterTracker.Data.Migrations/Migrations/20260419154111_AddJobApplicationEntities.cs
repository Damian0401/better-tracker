using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BetterTracker.Data.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddJobApplicationEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobApplications",
                schema: "Default",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    JobTitle = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    CompanyName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Requirements = table.Column<string>(type: "TEXT", nullable: true),
                    Benefits = table.Column<string>(type: "TEXT", nullable: true),
                    Link = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Technologies = table.Column<string>(type: "TEXT", nullable: true),
                    Experience = table.Column<string>(type: "TEXT", nullable: true),
                    WorkType = table.Column<int>(type: "INTEGER", nullable: false),
                    CurrentStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<long>(type: "INTEGER", nullable: false),
                    UpdatedAt = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobApplications_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Default",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                schema: "Default",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<long>(type: "INTEGER", nullable: false),
                    UpdatedAt = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tags_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Default",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobApplicationComments",
                schema: "Default",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    JobApplicationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Content = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<long>(type: "INTEGER", nullable: false),
                    UpdatedAt = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobApplicationComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobApplicationComments_JobApplications_JobApplicationId",
                        column: x => x.JobApplicationId,
                        principalSchema: "Default",
                        principalTable: "JobApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobApplicationSalaries",
                schema: "Default",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    JobApplicationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SalaryType = table.Column<int>(type: "INTEGER", nullable: false),
                    SalaryPost = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: true),
                    SalaryCandidate = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: true),
                    Currency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: true),
                    CreatedAt = table.Column<long>(type: "INTEGER", nullable: false),
                    UpdatedAt = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobApplicationSalaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobApplicationSalaries_JobApplications_JobApplicationId",
                        column: x => x.JobApplicationId,
                        principalSchema: "Default",
                        principalTable: "JobApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobApplicationStatusHistory",
                schema: "Default",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    JobApplicationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PreviousStatus = table.Column<int>(type: "INTEGER", nullable: true),
                    NewStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<long>(type: "INTEGER", nullable: false),
                    UpdatedAt = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobApplicationStatusHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobApplicationStatusHistory_JobApplications_JobApplicationId",
                        column: x => x.JobApplicationId,
                        principalSchema: "Default",
                        principalTable: "JobApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobApplicationTags",
                schema: "Default",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    JobApplicationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TagId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<long>(type: "INTEGER", nullable: false),
                    UpdatedAt = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobApplicationTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobApplicationTags_JobApplications_JobApplicationId",
                        column: x => x.JobApplicationId,
                        principalSchema: "Default",
                        principalTable: "JobApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobApplicationTags_Tags_TagId",
                        column: x => x.TagId,
                        principalSchema: "Default",
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobApplicationComments_JobApplicationId",
                schema: "Default",
                table: "JobApplicationComments",
                column: "JobApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_JobApplications_UserId_CreatedAt",
                schema: "Default",
                table: "JobApplications",
                columns: new[] { "UserId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_JobApplications_UserId_CurrentStatus",
                schema: "Default",
                table: "JobApplications",
                columns: new[] { "UserId", "CurrentStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_JobApplicationSalaries_JobApplicationId_SalaryType",
                schema: "Default",
                table: "JobApplicationSalaries",
                columns: new[] { "JobApplicationId", "SalaryType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobApplicationStatusHistory_JobApplicationId_CreatedAt",
                schema: "Default",
                table: "JobApplicationStatusHistory",
                columns: new[] { "JobApplicationId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_JobApplicationTags_JobApplicationId_TagId",
                schema: "Default",
                table: "JobApplicationTags",
                columns: new[] { "JobApplicationId", "TagId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobApplicationTags_TagId",
                schema: "Default",
                table: "JobApplicationTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_UserId",
                schema: "Default",
                table: "Tags",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_UserId_Name",
                schema: "Default",
                table: "Tags",
                columns: new[] { "UserId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobApplicationComments",
                schema: "Default");

            migrationBuilder.DropTable(
                name: "JobApplicationSalaries",
                schema: "Default");

            migrationBuilder.DropTable(
                name: "JobApplicationStatusHistory",
                schema: "Default");

            migrationBuilder.DropTable(
                name: "JobApplicationTags",
                schema: "Default");

            migrationBuilder.DropTable(
                name: "JobApplications",
                schema: "Default");

            migrationBuilder.DropTable(
                name: "Tags",
                schema: "Default");
        }
    }
}
