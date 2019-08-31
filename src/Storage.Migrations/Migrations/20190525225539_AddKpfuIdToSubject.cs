using Microsoft.EntityFrameworkCore.Migrations;

namespace Storage.Migrations.Migrations
{
    public partial class AddKpfuIdToSubject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "kpfu_id",
                table: "subjects",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "kpfu_id",
                table: "subjects");
        }
    }
}
