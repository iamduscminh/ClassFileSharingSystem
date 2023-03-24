using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessObjects.Migrations
{
    public partial class UpdateDbV15 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Resourses_Courses_CourseId",
                table: "Resourses");

            migrationBuilder.AlterColumn<int>(
                name: "CourseId",
                table: "Resourses",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Resourses_Courses_CourseId",
                table: "Resourses",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Resourses_Courses_CourseId",
                table: "Resourses");

            migrationBuilder.AlterColumn<int>(
                name: "CourseId",
                table: "Resourses",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Resourses_Courses_CourseId",
                table: "Resourses",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId");
        }
    }
}
