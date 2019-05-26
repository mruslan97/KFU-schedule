using Microsoft.EntityFrameworkCore.Migrations;

namespace Storage.Migrations.Migrations
{
    public partial class AddGroupIdToSubject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "group",
                table: "subjects",
                newName: "group_name");

            migrationBuilder.AddColumn<long>(
                name: "group_id",
                table: "subjects",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "ix_subjects_group_id",
                table: "subjects",
                column: "group_id");

            migrationBuilder.AddForeignKey(
                name: "fk_subjects_groups_group_id",
                table: "subjects",
                column: "group_id",
                principalTable: "groups",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_subjects_groups_group_id",
                table: "subjects");

            migrationBuilder.DropIndex(
                name: "ix_subjects_group_id",
                table: "subjects");

            migrationBuilder.DropColumn(
                name: "group_id",
                table: "subjects");

            migrationBuilder.RenameColumn(
                name: "group_name",
                table: "subjects",
                newName: "group");
        }
    }
}
