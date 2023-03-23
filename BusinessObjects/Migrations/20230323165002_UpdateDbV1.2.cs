using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessObjects.Migrations
{
    public partial class UpdateDbV12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_File_Resource_ResourceId",
                table: "File");

            migrationBuilder.DropForeignKey(
                name: "FK_Resource_Courses_CourseId",
                table: "Resource");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Resource",
                table: "Resource");

            migrationBuilder.DropPrimaryKey(
                name: "PK_File",
                table: "File");

            migrationBuilder.RenameTable(
                name: "Resource",
                newName: "Resourses");

            migrationBuilder.RenameTable(
                name: "File",
                newName: "Files");

            migrationBuilder.RenameIndex(
                name: "IX_Resource_CourseId",
                table: "Resourses",
                newName: "IX_Resourses_CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_File_ResourceId",
                table: "Files",
                newName: "IX_Files_ResourceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Resourses",
                table: "Resourses",
                column: "ResourceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Files",
                table: "Files",
                column: "FileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Resourses_ResourceId",
                table: "Files",
                column: "ResourceId",
                principalTable: "Resourses",
                principalColumn: "ResourceId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Resourses_Courses_CourseId",
                table: "Resourses",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Resourses_ResourceId",
                table: "Files");

            migrationBuilder.DropForeignKey(
                name: "FK_Resourses_Courses_CourseId",
                table: "Resourses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Resourses",
                table: "Resourses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Files",
                table: "Files");

            migrationBuilder.RenameTable(
                name: "Resourses",
                newName: "Resource");

            migrationBuilder.RenameTable(
                name: "Files",
                newName: "File");

            migrationBuilder.RenameIndex(
                name: "IX_Resourses_CourseId",
                table: "Resource",
                newName: "IX_Resource_CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_Files_ResourceId",
                table: "File",
                newName: "IX_File_ResourceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Resource",
                table: "Resource",
                column: "ResourceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_File",
                table: "File",
                column: "FileId");

            migrationBuilder.AddForeignKey(
                name: "FK_File_Resource_ResourceId",
                table: "File",
                column: "ResourceId",
                principalTable: "Resource",
                principalColumn: "ResourceId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Resource_Courses_CourseId",
                table: "Resource",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId");
        }
    }
}
