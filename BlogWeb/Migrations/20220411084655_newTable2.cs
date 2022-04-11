using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogWeb.Migrations
{
    public partial class newTable2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Thumb",
                table: "Post",
                newName: "imageName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "imageName",
                table: "Post",
                newName: "Thumb");
        }
    }
}
