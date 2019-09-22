using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Storage.Migrations.Migrations
{
    public partial class RemoveIsDeletedFromGroupsAndTeachers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_teachers_is_deleted",
                table: "teachers");

            migrationBuilder.DropIndex(
                name: "ix_groups_is_deleted",
                table: "groups");

            migrationBuilder.DropColumn(
                name: "deleted",
                table: "teachers");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "teachers");

            migrationBuilder.DropColumn(
                name: "deleted",
                table: "groups");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "groups");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "deleted",
                table: "teachers",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "teachers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted",
                table: "groups",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "groups",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "ix_teachers_is_deleted",
                table: "teachers",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "ix_groups_is_deleted",
                table: "groups",
                column: "is_deleted");
        }
    }
}
