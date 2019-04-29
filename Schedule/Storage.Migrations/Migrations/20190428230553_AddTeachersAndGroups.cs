using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Storage.Migrations.Migrations
{
    public partial class AddTeachersAndGroups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "groups",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    created = table.Column<DateTime>(nullable: true, defaultValueSql: "NOW()"),
                    updated = table.Column<DateTime>(nullable: true),
                    deleted = table.Column<DateTime>(nullable: true),
                    is_deleted = table.Column<bool>(nullable: false, defaultValue: false),
                    kpfu_id = table.Column<long>(nullable: false),
                    group_name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_groups", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "teachers",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    created = table.Column<DateTime>(nullable: true, defaultValueSql: "NOW()"),
                    updated = table.Column<DateTime>(nullable: true),
                    deleted = table.Column<DateTime>(nullable: true),
                    is_deleted = table.Column<bool>(nullable: false, defaultValue: false),
                    kpfu_id = table.Column<long>(nullable: false),
                    lastname = table.Column<string>(nullable: true),
                    firstname = table.Column<string>(nullable: true),
                    middlename = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_teachers", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_groups_group_name",
                table: "groups",
                column: "group_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_groups_id",
                table: "groups",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_groups_is_deleted",
                table: "groups",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "ix_teachers_id",
                table: "teachers",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_teachers_is_deleted",
                table: "teachers",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "ix_teachers_kpfu_id",
                table: "teachers",
                column: "kpfu_id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "groups");

            migrationBuilder.DropTable(
                name: "teachers");
        }
    }
}
