using Microsoft.EntityFrameworkCore.Migrations;

namespace Storage.Migrations.Migrations
{
    public partial class ChangeCabinetToString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "cabinet_number",
                table: "subjects",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "cabinet_number",
                table: "subjects",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
