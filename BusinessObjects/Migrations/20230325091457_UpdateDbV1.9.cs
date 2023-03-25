using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessObjects.Migrations
{
    public partial class UpdateDbV19 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentCourses_AspNetUsers_StudentId1",
                table: "StudentCourses");

            migrationBuilder.DropIndex(
                name: "IX_StudentCourses_StudentId1",
                table: "StudentCourses");

            migrationBuilder.DropColumn(
                name: "StudentId1",
                table: "StudentCourses");

            migrationBuilder.AlterColumn<string>(
                name: "StudentId",
                table: "StudentCourses",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "CourseIdRef",
                table: "StudentCourses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StudentIdRef",
                table: "StudentCourses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_StudentCourses_StudentId",
                table: "StudentCourses",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentCourses_AspNetUsers_StudentId",
                table: "StudentCourses",
                column: "StudentId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentCourses_AspNetUsers_StudentId",
                table: "StudentCourses");

            migrationBuilder.DropIndex(
                name: "IX_StudentCourses_StudentId",
                table: "StudentCourses");

            migrationBuilder.DropColumn(
                name: "CourseIdRef",
                table: "StudentCourses");

            migrationBuilder.DropColumn(
                name: "StudentIdRef",
                table: "StudentCourses");

            migrationBuilder.AlterColumn<int>(
                name: "StudentId",
                table: "StudentCourses",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "StudentId1",
                table: "StudentCourses",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_StudentCourses_StudentId1",
                table: "StudentCourses",
                column: "StudentId1");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentCourses_AspNetUsers_StudentId1",
                table: "StudentCourses",
                column: "StudentId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
