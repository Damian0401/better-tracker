using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BetterTracker.Data.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class MigrateToPostgres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (this.ActiveProvider == "Npgsql.EntityFrameworkCore.PostgreSQL")
            {
                migrationBuilder.Sql(
                    "ALTER TABLE \"Default\".\"Notes\" DROP CONSTRAINT IF EXISTS \"FK_Notes_Users_UserId\";" +
                    "ALTER TABLE \"Default\".\"Tags\" DROP CONSTRAINT IF EXISTS \"FK_Tags_Users_UserId\";" +
                    "ALTER TABLE \"Default\".\"JobApplications\" DROP CONSTRAINT IF EXISTS \"FK_JobApplications_Users_UserId\";" +
                    "ALTER TABLE \"Default\".\"JobApplicationComments\" DROP CONSTRAINT IF EXISTS \"FK_JobApplicationComments_JobApplications_JobApplicationId\";" +
                    "ALTER TABLE \"Default\".\"JobApplicationSalaries\" DROP CONSTRAINT IF EXISTS \"FK_JobApplicationSalaries_JobApplications_JobApplicationId\";" +
                    "ALTER TABLE \"Default\".\"JobApplicationStatusHistory\" DROP CONSTRAINT IF EXISTS \"FK_JobApplicationStatusHistory_JobApplications_JobApplicationId\";" +
                    "ALTER TABLE \"Default\".\"JobApplicationTags\" DROP CONSTRAINT IF EXISTS \"FK_JobApplicationTags_JobApplications_JobApplicationId\";" +
                    "ALTER TABLE \"Default\".\"JobApplicationTags\" DROP CONSTRAINT IF EXISTS \"FK_JobApplicationTags_Tags_TagId\";" +
                    "ALTER TABLE \"Default\".\"Users\" ALTER COLUMN \"UserName\" TYPE character varying(100);" +
                    "ALTER TABLE \"Default\".\"Users\" ALTER COLUMN \"UpdatedAt\" TYPE bigint USING \"UpdatedAt\"::bigint;" +
                    "ALTER TABLE \"Default\".\"Users\" ALTER COLUMN \"PasswordHash\" TYPE character varying(500);" +
                    "ALTER TABLE \"Default\".\"Users\" ALTER COLUMN \"Login\" TYPE character varying(100);" +
                    "ALTER TABLE \"Default\".\"Users\" ALTER COLUMN \"CreatedAt\" TYPE bigint USING \"CreatedAt\"::bigint;" +
                    "ALTER TABLE \"Default\".\"Users\" ALTER COLUMN \"Id\" TYPE uuid USING \"Id\"::uuid;" +
                    "ALTER TABLE \"Default\".\"Tags\" ALTER COLUMN \"UserId\" TYPE uuid USING \"UserId\"::uuid;" +
                    "ALTER TABLE \"Default\".\"Tags\" ALTER COLUMN \"UpdatedAt\" TYPE bigint USING \"UpdatedAt\"::bigint;" +
                    "ALTER TABLE \"Default\".\"Tags\" ALTER COLUMN \"Name\" TYPE character varying(50);" +
                    "ALTER TABLE \"Default\".\"Tags\" ALTER COLUMN \"CreatedAt\" TYPE bigint USING \"CreatedAt\"::bigint;" +
                    "ALTER TABLE \"Default\".\"Tags\" ALTER COLUMN \"Id\" TYPE uuid USING \"Id\"::uuid;" +
                    "ALTER TABLE \"Default\".\"Notes\" ALTER COLUMN \"UserId\" DROP DEFAULT;" +
                    "ALTER TABLE \"Default\".\"Notes\" ALTER COLUMN \"UserId\" TYPE uuid USING \"UserId\"::uuid;" +
                    "ALTER TABLE \"Default\".\"Notes\" ALTER COLUMN \"UpdatedAt\" TYPE bigint USING \"UpdatedAt\"::bigint;" +
                    "ALTER TABLE \"Default\".\"Notes\" ALTER COLUMN \"Title\" TYPE character varying(100);" +
                    "ALTER TABLE \"Default\".\"Notes\" ALTER COLUMN \"CreatedAt\" TYPE bigint USING \"CreatedAt\"::bigint;" +
                    "ALTER TABLE \"Default\".\"Notes\" ALTER COLUMN \"Content\" TYPE character varying(500);" +
                    "ALTER TABLE \"Default\".\"Notes\" ALTER COLUMN \"Id\" TYPE uuid USING \"Id\"::uuid;" +
                    "ALTER TABLE \"Default\".\"JobApplicationTags\" ALTER COLUMN \"UpdatedAt\" TYPE bigint USING \"UpdatedAt\"::bigint;" +
                    "ALTER TABLE \"Default\".\"JobApplicationTags\" ALTER COLUMN \"TagId\" TYPE uuid USING \"TagId\"::uuid;" +
                    "ALTER TABLE \"Default\".\"JobApplicationTags\" ALTER COLUMN \"JobApplicationId\" TYPE uuid USING \"JobApplicationId\"::uuid;" +
                    "ALTER TABLE \"Default\".\"JobApplicationTags\" ALTER COLUMN \"CreatedAt\" TYPE bigint USING \"CreatedAt\"::bigint;" +
                    "ALTER TABLE \"Default\".\"JobApplicationTags\" ALTER COLUMN \"Id\" TYPE uuid USING \"Id\"::uuid;" +
                    "ALTER TABLE \"Default\".\"JobApplicationStatusHistory\" ALTER COLUMN \"UpdatedAt\" TYPE bigint USING \"UpdatedAt\"::bigint;" +
                    "ALTER TABLE \"Default\".\"JobApplicationStatusHistory\" ALTER COLUMN \"JobApplicationId\" TYPE uuid USING \"JobApplicationId\"::uuid;" +
                    "ALTER TABLE \"Default\".\"JobApplicationStatusHistory\" ALTER COLUMN \"CreatedAt\" TYPE bigint USING \"CreatedAt\"::bigint;" +
                    "ALTER TABLE \"Default\".\"JobApplicationStatusHistory\" ALTER COLUMN \"Id\" TYPE uuid USING \"Id\"::uuid;" +
                    "ALTER TABLE \"Default\".\"JobApplicationSalaries\" ALTER COLUMN \"UpdatedAt\" TYPE bigint USING \"UpdatedAt\"::bigint;" +
                    "ALTER TABLE \"Default\".\"JobApplicationSalaries\" ALTER COLUMN \"OfferTo\" TYPE numeric(18,2) USING \"OfferTo\"::numeric(18,2);" +
                    "ALTER TABLE \"Default\".\"JobApplicationSalaries\" ALTER COLUMN \"OfferFrom\" TYPE numeric(18,2) USING \"OfferFrom\"::numeric(18,2);" +
                    "ALTER TABLE \"Default\".\"JobApplicationSalaries\" ALTER COLUMN \"JobApplicationId\" TYPE uuid USING \"JobApplicationId\"::uuid;" +
                    "ALTER TABLE \"Default\".\"JobApplicationSalaries\" ALTER COLUMN \"ExpectedTo\" TYPE numeric(18,2) USING \"ExpectedTo\"::numeric(18,2);" +
                    "ALTER TABLE \"Default\".\"JobApplicationSalaries\" ALTER COLUMN \"ExpectedFrom\" TYPE numeric(18,2) USING \"ExpectedFrom\"::numeric(18,2);" +
                    "ALTER TABLE \"Default\".\"JobApplicationSalaries\" ALTER COLUMN \"Currency\" TYPE character varying(3);" +
                    "ALTER TABLE \"Default\".\"JobApplicationSalaries\" ALTER COLUMN \"CreatedAt\" TYPE bigint USING \"CreatedAt\"::bigint;" +
                    "ALTER TABLE \"Default\".\"JobApplicationSalaries\" ALTER COLUMN \"Id\" TYPE uuid USING \"Id\"::uuid;" +
                    "ALTER TABLE \"Default\".\"JobApplications\" ALTER COLUMN \"UserId\" TYPE uuid USING \"UserId\"::uuid;" +
                    "ALTER TABLE \"Default\".\"JobApplications\" ALTER COLUMN \"UpdatedAt\" TYPE bigint USING \"UpdatedAt\"::bigint;" +
                    "ALTER TABLE \"Default\".\"JobApplications\" ALTER COLUMN \"Link\" TYPE character varying(500);" +
                    "ALTER TABLE \"Default\".\"JobApplications\" ALTER COLUMN \"JobTitle\" TYPE character varying(200);" +
                    "ALTER TABLE \"Default\".\"JobApplications\" ALTER COLUMN \"IsArchived\" DROP DEFAULT;" +
                    "ALTER TABLE \"Default\".\"JobApplications\" ALTER COLUMN \"IsArchived\" TYPE boolean USING CASE WHEN \"IsArchived\" = 1 THEN true ELSE false END;" +
                    "ALTER TABLE \"Default\".\"JobApplications\" ALTER COLUMN \"IsArchived\" SET DEFAULT false;" +
                    "ALTER TABLE \"Default\".\"JobApplications\" ALTER COLUMN \"CreatedAt\" TYPE bigint USING \"CreatedAt\"::bigint;" +
                    "ALTER TABLE \"Default\".\"JobApplications\" ALTER COLUMN \"CompanyName\" TYPE character varying(200);" +
                    "ALTER TABLE \"Default\".\"JobApplications\" ALTER COLUMN \"Id\" TYPE uuid USING \"Id\"::uuid;" +
                    "ALTER TABLE \"Default\".\"JobApplicationComments\" ALTER COLUMN \"UpdatedAt\" TYPE bigint USING \"UpdatedAt\"::bigint;" +
                    "ALTER TABLE \"Default\".\"JobApplicationComments\" ALTER COLUMN \"JobApplicationId\" TYPE uuid USING \"JobApplicationId\"::uuid;" +
                    "ALTER TABLE \"Default\".\"JobApplicationComments\" ALTER COLUMN \"CreatedAt\" TYPE bigint USING \"CreatedAt\"::bigint;" +
                    "ALTER TABLE \"Default\".\"JobApplicationComments\" ALTER COLUMN \"Content\" TYPE character varying(2000);" +
                    "ALTER TABLE \"Default\".\"JobApplicationComments\" ALTER COLUMN \"Id\" TYPE uuid USING \"Id\"::uuid;" +
                    "ALTER TABLE \"Default\".\"Notes\" ADD CONSTRAINT \"FK_Notes_Users_UserId\" FOREIGN KEY (\"UserId\") REFERENCES \"Default\".\"Users\" (\"Id\") ON DELETE CASCADE;" +
                    "ALTER TABLE \"Default\".\"Tags\" ADD CONSTRAINT \"FK_Tags_Users_UserId\" FOREIGN KEY (\"UserId\") REFERENCES \"Default\".\"Users\" (\"Id\") ON DELETE CASCADE;" +
                    "ALTER TABLE \"Default\".\"JobApplications\" ADD CONSTRAINT \"FK_JobApplications_Users_UserId\" FOREIGN KEY (\"UserId\") REFERENCES \"Default\".\"Users\" (\"Id\") ON DELETE CASCADE;" +
                    "ALTER TABLE \"Default\".\"JobApplicationComments\" ADD CONSTRAINT \"FK_JobApplicationComments_JobApplications_JobApplicationId\" FOREIGN KEY (\"JobApplicationId\") REFERENCES \"Default\".\"JobApplications\" (\"Id\") ON DELETE CASCADE;" +
                    "ALTER TABLE \"Default\".\"JobApplicationSalaries\" ADD CONSTRAINT \"FK_JobApplicationSalaries_JobApplications_JobApplicationId\" FOREIGN KEY (\"JobApplicationId\") REFERENCES \"Default\".\"JobApplications\" (\"Id\") ON DELETE CASCADE;" +
                    "ALTER TABLE \"Default\".\"JobApplicationStatusHistory\" ADD CONSTRAINT \"FK_JobApplicationStatusHistory_JobApplications_JobApplicationId\" FOREIGN KEY (\"JobApplicationId\") REFERENCES \"Default\".\"JobApplications\" (\"Id\") ON DELETE CASCADE;" +
                    "ALTER TABLE \"Default\".\"JobApplicationTags\" ADD CONSTRAINT \"FK_JobApplicationTags_JobApplications_JobApplicationId\" FOREIGN KEY (\"JobApplicationId\") REFERENCES \"Default\".\"JobApplications\" (\"Id\") ON DELETE CASCADE;" +
                    "ALTER TABLE \"Default\".\"JobApplicationTags\" ADD CONSTRAINT \"FK_JobApplicationTags_Tags_TagId\" FOREIGN KEY (\"TagId\") REFERENCES \"Default\".\"Tags\" (\"Id\") ON DELETE CASCADE;");
                return;
            }

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                schema: "Default",
                table: "Users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<long>(
                name: "UpdatedAt",
                schema: "Default",
                table: "Users",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                schema: "Default",
                table: "Users",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Login",
                schema: "Default",
                table: "Users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<long>(
                name: "CreatedAt",
                schema: "Default",
                table: "Users",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "Default",
                table: "Users",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                schema: "Default",
                table: "Tags",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<long>(
                name: "UpdatedAt",
                schema: "Default",
                table: "Tags",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "Default",
                table: "Tags",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<long>(
                name: "CreatedAt",
                schema: "Default",
                table: "Tags",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "Default",
                table: "Tags",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                schema: "Default",
                table: "Notes",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<long>(
                name: "UpdatedAt",
                schema: "Default",
                table: "Notes",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                schema: "Default",
                table: "Notes",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<long>(
                name: "CreatedAt",
                schema: "Default",
                table: "Notes",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                schema: "Default",
                table: "Notes",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "Default",
                table: "Notes",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<long>(
                name: "UpdatedAt",
                schema: "Default",
                table: "JobApplicationTags",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<Guid>(
                name: "TagId",
                schema: "Default",
                table: "JobApplicationTags",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "JobApplicationId",
                schema: "Default",
                table: "JobApplicationTags",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<long>(
                name: "CreatedAt",
                schema: "Default",
                table: "JobApplicationTags",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "Default",
                table: "JobApplicationTags",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<long>(
                name: "UpdatedAt",
                schema: "Default",
                table: "JobApplicationStatusHistory",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "PreviousStatus",
                schema: "Default",
                table: "JobApplicationStatusHistory",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "NewStatus",
                schema: "Default",
                table: "JobApplicationStatusHistory",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<Guid>(
                name: "JobApplicationId",
                schema: "Default",
                table: "JobApplicationStatusHistory",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<long>(
                name: "CreatedAt",
                schema: "Default",
                table: "JobApplicationStatusHistory",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "Default",
                table: "JobApplicationStatusHistory",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<long>(
                name: "UpdatedAt",
                schema: "Default",
                table: "JobApplicationSalaries",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "SalaryType",
                schema: "Default",
                table: "JobApplicationSalaries",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<decimal>(
                name: "OfferTo",
                schema: "Default",
                table: "JobApplicationSalaries",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "OfferFrom",
                schema: "Default",
                table: "JobApplicationSalaries",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "JobApplicationId",
                schema: "Default",
                table: "JobApplicationSalaries",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<decimal>(
                name: "ExpectedTo",
                schema: "Default",
                table: "JobApplicationSalaries",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ExpectedFrom",
                schema: "Default",
                table: "JobApplicationSalaries",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Currency",
                schema: "Default",
                table: "JobApplicationSalaries",
                type: "character varying(3)",
                maxLength: 3,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 3,
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "CreatedAt",
                schema: "Default",
                table: "JobApplicationSalaries",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "Default",
                table: "JobApplicationSalaries",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "WorkType",
                schema: "Default",
                table: "JobApplications",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                schema: "Default",
                table: "JobApplications",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<long>(
                name: "UpdatedAt",
                schema: "Default",
                table: "JobApplications",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Technologies",
                schema: "Default",
                table: "JobApplications",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Requirements",
                schema: "Default",
                table: "JobApplications",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Link",
                schema: "Default",
                table: "JobApplications",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "JobTitle",
                schema: "Default",
                table: "JobApplications",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<bool>(
                name: "IsArchived",
                schema: "Default",
                table: "JobApplications",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Experience",
                schema: "Default",
                table: "JobApplications",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "Default",
                table: "JobApplications",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CurrentStatus",
                schema: "Default",
                table: "JobApplications",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<long>(
                name: "CreatedAt",
                schema: "Default",
                table: "JobApplications",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "CompanyName",
                schema: "Default",
                table: "JobApplications",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Benefits",
                schema: "Default",
                table: "JobApplications",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "Default",
                table: "JobApplications",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<long>(
                name: "UpdatedAt",
                schema: "Default",
                table: "JobApplicationComments",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<Guid>(
                name: "JobApplicationId",
                schema: "Default",
                table: "JobApplicationComments",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<long>(
                name: "CreatedAt",
                schema: "Default",
                table: "JobApplicationComments",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                schema: "Default",
                table: "JobApplicationComments",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 2000);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "Default",
                table: "JobApplicationComments",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            if (this.ActiveProvider == "Npgsql.EntityFrameworkCore.PostgreSQL")
            {
                migrationBuilder.Sql(
                    "ALTER TABLE \"Default\".\"Notes\" DROP CONSTRAINT IF EXISTS \"FK_Notes_Users_UserId\";" +
                    "ALTER TABLE \"Default\".\"Tags\" DROP CONSTRAINT IF EXISTS \"FK_Tags_Users_UserId\";" +
                    "ALTER TABLE \"Default\".\"JobApplications\" DROP CONSTRAINT IF EXISTS \"FK_JobApplications_Users_UserId\";" +
                    "ALTER TABLE \"Default\".\"JobApplicationComments\" DROP CONSTRAINT IF EXISTS \"FK_JobApplicationComments_JobApplications_JobApplicationId\";" +
                    "ALTER TABLE \"Default\".\"JobApplicationSalaries\" DROP CONSTRAINT IF EXISTS \"FK_JobApplicationSalaries_JobApplications_JobApplicationId\";" +
                    "ALTER TABLE \"Default\".\"JobApplicationStatusHistory\" DROP CONSTRAINT IF EXISTS \"FK_JobApplicationStatusHistory_JobApplications_JobApplicationId\";" +
                    "ALTER TABLE \"Default\".\"JobApplicationTags\" DROP CONSTRAINT IF EXISTS \"FK_JobApplicationTags_JobApplications_JobApplicationId\";" +
                    "ALTER TABLE \"Default\".\"JobApplicationTags\" DROP CONSTRAINT IF EXISTS \"FK_JobApplicationTags_Tags_TagId\";" +
                    "ALTER TABLE \"Default\".\"Users\" ALTER COLUMN \"UserName\" TYPE text;" +
                    "ALTER TABLE \"Default\".\"Users\" ALTER COLUMN \"UpdatedAt\" TYPE integer USING \"UpdatedAt\"::integer;" +
                    "ALTER TABLE \"Default\".\"Users\" ALTER COLUMN \"PasswordHash\" TYPE text;" +
                    "ALTER TABLE \"Default\".\"Users\" ALTER COLUMN \"Login\" TYPE text;" +
                    "ALTER TABLE \"Default\".\"Users\" ALTER COLUMN \"CreatedAt\" TYPE integer USING \"CreatedAt\"::integer;" +
                    "ALTER TABLE \"Default\".\"Users\" ALTER COLUMN \"Id\" TYPE text USING \"Id\"::text;" +
                    "ALTER TABLE \"Default\".\"Tags\" ALTER COLUMN \"UserId\" TYPE text USING \"UserId\"::text;" +
                    "ALTER TABLE \"Default\".\"Tags\" ALTER COLUMN \"UpdatedAt\" TYPE integer USING \"UpdatedAt\"::integer;" +
                    "ALTER TABLE \"Default\".\"Tags\" ALTER COLUMN \"Name\" TYPE text;" +
                    "ALTER TABLE \"Default\".\"Tags\" ALTER COLUMN \"CreatedAt\" TYPE integer USING \"CreatedAt\"::integer;" +
                    "ALTER TABLE \"Default\".\"Tags\" ALTER COLUMN \"Id\" TYPE text USING \"Id\"::text;" +
                    "ALTER TABLE \"Default\".\"Notes\" ALTER COLUMN \"UserId\" TYPE text USING \"UserId\"::text;" +
                    "ALTER TABLE \"Default\".\"Notes\" ALTER COLUMN \"UpdatedAt\" TYPE integer USING \"UpdatedAt\"::integer;" +
                    "ALTER TABLE \"Default\".\"Notes\" ALTER COLUMN \"Title\" TYPE text;" +
                    "ALTER TABLE \"Default\".\"Notes\" ALTER COLUMN \"CreatedAt\" TYPE integer USING \"CreatedAt\"::integer;" +
                    "ALTER TABLE \"Default\".\"Notes\" ALTER COLUMN \"Content\" TYPE text;" +
                    "ALTER TABLE \"Default\".\"Notes\" ALTER COLUMN \"Id\" TYPE text USING \"Id\"::text;" +
                    "ALTER TABLE \"Default\".\"JobApplicationTags\" ALTER COLUMN \"UpdatedAt\" TYPE integer USING \"UpdatedAt\"::integer;" +
                    "ALTER TABLE \"Default\".\"JobApplicationTags\" ALTER COLUMN \"TagId\" TYPE text USING \"TagId\"::text;" +
                    "ALTER TABLE \"Default\".\"JobApplicationTags\" ALTER COLUMN \"JobApplicationId\" TYPE text USING \"JobApplicationId\"::text;" +
                    "ALTER TABLE \"Default\".\"JobApplicationTags\" ALTER COLUMN \"CreatedAt\" TYPE integer USING \"CreatedAt\"::integer;" +
                    "ALTER TABLE \"Default\".\"JobApplicationTags\" ALTER COLUMN \"Id\" TYPE text USING \"Id\"::text;" +
                    "ALTER TABLE \"Default\".\"JobApplicationStatusHistory\" ALTER COLUMN \"UpdatedAt\" TYPE integer USING \"UpdatedAt\"::integer;" +
                    "ALTER TABLE \"Default\".\"JobApplicationStatusHistory\" ALTER COLUMN \"JobApplicationId\" TYPE text USING \"JobApplicationId\"::text;" +
                    "ALTER TABLE \"Default\".\"JobApplicationStatusHistory\" ALTER COLUMN \"CreatedAt\" TYPE integer USING \"CreatedAt\"::integer;" +
                    "ALTER TABLE \"Default\".\"JobApplicationStatusHistory\" ALTER COLUMN \"Id\" TYPE text USING \"Id\"::text;" +
                    "ALTER TABLE \"Default\".\"JobApplicationSalaries\" ALTER COLUMN \"UpdatedAt\" TYPE integer USING \"UpdatedAt\"::integer;" +
                    "ALTER TABLE \"Default\".\"JobApplicationSalaries\" ALTER COLUMN \"OfferTo\" TYPE text USING \"OfferTo\"::text;" +
                    "ALTER TABLE \"Default\".\"JobApplicationSalaries\" ALTER COLUMN \"OfferFrom\" TYPE text USING \"OfferFrom\"::text;" +
                    "ALTER TABLE \"Default\".\"JobApplicationSalaries\" ALTER COLUMN \"JobApplicationId\" TYPE text USING \"JobApplicationId\"::text;" +
                    "ALTER TABLE \"Default\".\"JobApplicationSalaries\" ALTER COLUMN \"ExpectedTo\" TYPE text USING \"ExpectedTo\"::text;" +
                    "ALTER TABLE \"Default\".\"JobApplicationSalaries\" ALTER COLUMN \"ExpectedFrom\" TYPE text USING \"ExpectedFrom\"::text;" +
                    "ALTER TABLE \"Default\".\"JobApplicationSalaries\" ALTER COLUMN \"Currency\" TYPE text;" +
                    "ALTER TABLE \"Default\".\"JobApplicationSalaries\" ALTER COLUMN \"CreatedAt\" TYPE integer USING \"CreatedAt\"::integer;" +
                    "ALTER TABLE \"Default\".\"JobApplicationSalaries\" ALTER COLUMN \"Id\" TYPE text USING \"Id\"::text;" +
                    "ALTER TABLE \"Default\".\"JobApplications\" ALTER COLUMN \"UserId\" TYPE text USING \"UserId\"::text;" +
                    "ALTER TABLE \"Default\".\"JobApplications\" ALTER COLUMN \"UpdatedAt\" TYPE integer USING \"UpdatedAt\"::integer;" +
                    "ALTER TABLE \"Default\".\"JobApplications\" ALTER COLUMN \"Link\" TYPE text;" +
                    "ALTER TABLE \"Default\".\"JobApplications\" ALTER COLUMN \"JobTitle\" TYPE text;" +
                    "ALTER TABLE \"Default\".\"JobApplications\" ALTER COLUMN \"IsArchived\" TYPE integer USING CASE WHEN \"IsArchived\" THEN 1 ELSE 0 END;" +
                    "ALTER TABLE \"Default\".\"JobApplications\" ALTER COLUMN \"CreatedAt\" TYPE integer USING \"CreatedAt\"::integer;" +
                    "ALTER TABLE \"Default\".\"JobApplications\" ALTER COLUMN \"CompanyName\" TYPE text;" +
                    "ALTER TABLE \"Default\".\"JobApplications\" ALTER COLUMN \"Id\" TYPE text USING \"Id\"::text;" +
                    "ALTER TABLE \"Default\".\"JobApplicationComments\" ALTER COLUMN \"UpdatedAt\" TYPE integer USING \"UpdatedAt\"::integer;" +
                    "ALTER TABLE \"Default\".\"JobApplicationComments\" ALTER COLUMN \"JobApplicationId\" TYPE text USING \"JobApplicationId\"::text;" +
                    "ALTER TABLE \"Default\".\"JobApplicationComments\" ALTER COLUMN \"CreatedAt\" TYPE integer USING \"CreatedAt\"::integer;" +
                    "ALTER TABLE \"Default\".\"JobApplicationComments\" ALTER COLUMN \"Content\" TYPE text;" +
                    "ALTER TABLE \"Default\".\"JobApplicationComments\" ALTER COLUMN \"Id\" TYPE text USING \"Id\"::text;" +
                    "ALTER TABLE \"Default\".\"Notes\" ADD CONSTRAINT \"FK_Notes_Users_UserId\" FOREIGN KEY (\"UserId\") REFERENCES \"Default\".\"Users\" (\"Id\") ON DELETE CASCADE;" +
                    "ALTER TABLE \"Default\".\"Tags\" ADD CONSTRAINT \"FK_Tags_Users_UserId\" FOREIGN KEY (\"UserId\") REFERENCES \"Default\".\"Users\" (\"Id\") ON DELETE CASCADE;" +
                    "ALTER TABLE \"Default\".\"JobApplications\" ADD CONSTRAINT \"FK_JobApplications_Users_UserId\" FOREIGN KEY (\"UserId\") REFERENCES \"Default\".\"Users\" (\"Id\") ON DELETE CASCADE;" +
                    "ALTER TABLE \"Default\".\"JobApplicationComments\" ADD CONSTRAINT \"FK_JobApplicationComments_JobApplications_JobApplicationId\" FOREIGN KEY (\"JobApplicationId\") REFERENCES \"Default\".\"JobApplications\" (\"Id\") ON DELETE CASCADE;" +
                    "ALTER TABLE \"Default\".\"JobApplicationSalaries\" ADD CONSTRAINT \"FK_JobApplicationSalaries_JobApplications_JobApplicationId\" FOREIGN KEY (\"JobApplicationId\") REFERENCES \"Default\".\"JobApplications\" (\"Id\") ON DELETE CASCADE;" +
                    "ALTER TABLE \"Default\".\"JobApplicationStatusHistory\" ADD CONSTRAINT \"FK_JobApplicationStatusHistory_JobApplications_JobApplicationId\" FOREIGN KEY (\"JobApplicationId\") REFERENCES \"Default\".\"JobApplications\" (\"Id\") ON DELETE CASCADE;" +
                    "ALTER TABLE \"Default\".\"JobApplicationTags\" ADD CONSTRAINT \"FK_JobApplicationTags_JobApplications_JobApplicationId\" FOREIGN KEY (\"JobApplicationId\") REFERENCES \"Default\".\"JobApplications\" (\"Id\") ON DELETE CASCADE;" +
                    "ALTER TABLE \"Default\".\"JobApplicationTags\" ADD CONSTRAINT \"FK_JobApplicationTags_Tags_TagId\" FOREIGN KEY (\"TagId\") REFERENCES \"Default\".\"Tags\" (\"Id\") ON DELETE CASCADE;");
                return;
            }

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                schema: "Default",
                table: "Users",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<int>(
                name: "UpdatedAt",
                schema: "Default",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                schema: "Default",
                table: "Users",
                type: "TEXT",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Login",
                schema: "Default",
                table: "Users",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<int>(
                name: "CreatedAt",
                schema: "Default",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                schema: "Default",
                table: "Users",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                schema: "Default",
                table: "Tags",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<int>(
                name: "UpdatedAt",
                schema: "Default",
                table: "Tags",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "Default",
                table: "Tags",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "CreatedAt",
                schema: "Default",
                table: "Tags",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                schema: "Default",
                table: "Tags",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                schema: "Default",
                table: "Notes",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<int>(
                name: "UpdatedAt",
                schema: "Default",
                table: "Notes",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                schema: "Default",
                table: "Notes",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<int>(
                name: "CreatedAt",
                schema: "Default",
                table: "Notes",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                schema: "Default",
                table: "Notes",
                type: "TEXT",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                schema: "Default",
                table: "Notes",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<int>(
                name: "UpdatedAt",
                schema: "Default",
                table: "JobApplicationTags",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "TagId",
                schema: "Default",
                table: "JobApplicationTags",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "JobApplicationId",
                schema: "Default",
                table: "JobApplicationTags",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<int>(
                name: "CreatedAt",
                schema: "Default",
                table: "JobApplicationTags",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                schema: "Default",
                table: "JobApplicationTags",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<int>(
                name: "UpdatedAt",
                schema: "Default",
                table: "JobApplicationStatusHistory",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "PreviousStatus",
                schema: "Default",
                table: "JobApplicationStatusHistory",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "NewStatus",
                schema: "Default",
                table: "JobApplicationStatusHistory",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "JobApplicationId",
                schema: "Default",
                table: "JobApplicationStatusHistory",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<int>(
                name: "CreatedAt",
                schema: "Default",
                table: "JobApplicationStatusHistory",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                schema: "Default",
                table: "JobApplicationStatusHistory",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<int>(
                name: "UpdatedAt",
                schema: "Default",
                table: "JobApplicationSalaries",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "SalaryType",
                schema: "Default",
                table: "JobApplicationSalaries",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "OfferTo",
                schema: "Default",
                table: "JobApplicationSalaries",
                type: "TEXT",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OfferFrom",
                schema: "Default",
                table: "JobApplicationSalaries",
                type: "TEXT",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "JobApplicationId",
                schema: "Default",
                table: "JobApplicationSalaries",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "ExpectedTo",
                schema: "Default",
                table: "JobApplicationSalaries",
                type: "TEXT",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ExpectedFrom",
                schema: "Default",
                table: "JobApplicationSalaries",
                type: "TEXT",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Currency",
                schema: "Default",
                table: "JobApplicationSalaries",
                type: "TEXT",
                maxLength: 3,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CreatedAt",
                schema: "Default",
                table: "JobApplicationSalaries",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                schema: "Default",
                table: "JobApplicationSalaries",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<int>(
                name: "WorkType",
                schema: "Default",
                table: "JobApplications",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                schema: "Default",
                table: "JobApplications",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<int>(
                name: "UpdatedAt",
                schema: "Default",
                table: "JobApplications",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "Technologies",
                schema: "Default",
                table: "JobApplications",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Requirements",
                schema: "Default",
                table: "JobApplications",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Link",
                schema: "Default",
                table: "JobApplications",
                type: "TEXT",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "JobTitle",
                schema: "Default",
                table: "JobApplications",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<int>(
                name: "IsArchived",
                schema: "Default",
                table: "JobApplications",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Experience",
                schema: "Default",
                table: "JobApplications",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "Default",
                table: "JobApplications",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CurrentStatus",
                schema: "Default",
                table: "JobApplications",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "CreatedAt",
                schema: "Default",
                table: "JobApplications",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "CompanyName",
                schema: "Default",
                table: "JobApplications",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Benefits",
                schema: "Default",
                table: "JobApplications",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                schema: "Default",
                table: "JobApplications",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<int>(
                name: "UpdatedAt",
                schema: "Default",
                table: "JobApplicationComments",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "JobApplicationId",
                schema: "Default",
                table: "JobApplicationComments",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<int>(
                name: "CreatedAt",
                schema: "Default",
                table: "JobApplicationComments",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                schema: "Default",
                table: "JobApplicationComments",
                type: "TEXT",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                schema: "Default",
                table: "JobApplicationComments",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");
        }
    }
}
