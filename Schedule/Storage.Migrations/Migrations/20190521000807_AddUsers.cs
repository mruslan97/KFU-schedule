using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Storage.Migrations.Migrations
{
    public partial class AddUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "vk_users",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    created = table.Column<DateTime>(nullable: true, defaultValueSql: "NOW()"),
                    updated = table.Column<DateTime>(nullable: true),
                    deleted = table.Column<DateTime>(nullable: true),
                    is_deleted = table.Column<bool>(nullable: false, defaultValue: false),
                    user_id = table.Column<long>(nullable: false),
                    first_name = table.Column<string>(nullable: true),
                    last_name = table.Column<string>(nullable: true),
                    group_id = table.Column<long>(nullable: true),
                    chat_state = table.Column<int>(nullable: false),
                    schedule_type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vk_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_vk_users_groups_group_id",
                        column: x => x.group_id,
                        principalTable: "groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_vk_users_group_id",
                table: "vk_users",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "ix_vk_users_id",
                table: "vk_users",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_vk_users_is_deleted",
                table: "vk_users",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "ix_vk_users_user_id",
                table: "vk_users",
                column: "user_id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "vk_users");
        }
    }
}
