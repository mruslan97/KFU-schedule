using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Storage.Migrations.Migrations
{
    public partial class RemoveIsDeletedFromSubject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_subjects_is_deleted",
                table: "subjects");

            migrationBuilder.DropColumn(
                name: "deleted",
                table: "subjects");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "subjects");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "deleted",
                table: "subjects",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "subjects",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "ix_subjects_is_deleted",
                table: "subjects",
                column: "is_deleted");
        }
    }
}
