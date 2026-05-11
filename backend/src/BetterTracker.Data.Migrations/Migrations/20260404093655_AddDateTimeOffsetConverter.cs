using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BetterTracker.Data.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddDateTimeOffsetConverter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (this.ActiveProvider == "Npgsql.EntityFrameworkCore.PostgreSQL")
            {
                migrationBuilder.Sql(
                    "ALTER TABLE \"Default\".\"Notes\" ALTER COLUMN \"UpdatedAt\" TYPE bigint USING \"UpdatedAt\"::bigint;" +
                    "ALTER TABLE \"Default\".\"Notes\" ALTER COLUMN \"CreatedAt\" TYPE bigint USING \"CreatedAt\"::bigint;");
                return;
            }

            migrationBuilder.AlterColumn<long>(
                name: "UpdatedAt",
                schema: "Default",
                table: "Notes",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<long>(
                name: "CreatedAt",
                schema: "Default",
                table: "Notes",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "TEXT");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            if (this.ActiveProvider == "Npgsql.EntityFrameworkCore.PostgreSQL")
            {
                migrationBuilder.Sql(
                    "ALTER TABLE \"Default\".\"Notes\" ALTER COLUMN \"UpdatedAt\" TYPE text USING \"UpdatedAt\"::text;" +
                    "ALTER TABLE \"Default\".\"Notes\" ALTER COLUMN \"CreatedAt\" TYPE text USING \"CreatedAt\"::text;");
                return;
            }

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedAt",
                schema: "Default",
                table: "Notes",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                schema: "Default",
                table: "Notes",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "INTEGER");
        }
    }
}
