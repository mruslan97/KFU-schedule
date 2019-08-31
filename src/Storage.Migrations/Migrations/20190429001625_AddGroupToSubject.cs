using Microsoft.EntityFrameworkCore.Migrations;

namespace Storage.Migrations.Migrations
{
    public partial class AddGroupToSubject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "group",
                table: "subjects",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "group",
                table: "subjects");
        }
    }
}
