using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreCodeCamp.Migrations
{
    public partial class CreatedValidations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Camps",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Moniker",
                table: "Camps",
                nullable: true,
                oldClrType: typeof(string));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Camps",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Moniker",
                table: "Camps",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
