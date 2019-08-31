using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Storage.Migrations.Migrations
{
    public partial class Initialize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "subjects",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    created = table.Column<DateTime>(nullable: true, defaultValueSql: "NOW()"),
                    updated = table.Column<DateTime>(nullable: true),
                    deleted = table.Column<DateTime>(nullable: true),
                    is_deleted = table.Column<bool>(nullable: false, defaultValue: false),
                    name = table.Column<string>(nullable: true),
                    start_day = table.Column<DateTime>(nullable: true),
                    end_day = table.Column<DateTime>(nullable: true),
                    day_of_week = table.Column<int>(nullable: false),
                    week_type = table.Column<int>(nullable: false),
                    note = table.Column<string>(nullable: true),
                    total_time = table.Column<string>(nullable: true),
                    start_time = table.Column<TimeSpan>(nullable: true),
                    end_time = table.Column<TimeSpan>(nullable: true),
                    teacher_id = table.Column<long>(nullable: true),
                    teacher_lastname = table.Column<string>(nullable: true),
                    teacher_firstname = table.Column<string>(nullable: true),
                    teacher_middlename = table.Column<string>(nullable: true),
                    cabinet_number = table.Column<int>(nullable: false),
                    building_name = table.Column<string>(nullable: true),
                    building_id = table.Column<long>(nullable: false),
                    subject_kind_name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_subjects", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_subjects_id",
                table: "subjects",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_subjects_is_deleted",
                table: "subjects",
                column: "is_deleted");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "subjects");
        }
    }
}
