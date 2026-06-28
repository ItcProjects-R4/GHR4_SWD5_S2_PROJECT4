using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Addlessontracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop foreign key of the singular SubmissionFile table (currently links to Submissions plural)
            migrationBuilder.DropForeignKey(
                name: "FK_SubmissionFile_Submissions_SubmissionId",
                table: "SubmissionFile");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubmissionFile",
                table: "SubmissionFile");

            // Rename SubmissionFile to plural
            migrationBuilder.RenameTable(
                name: "SubmissionFile",
                newName: "SubmissionFiles");

            migrationBuilder.RenameIndex(
                name: "IX_SubmissionFile_SubmissionId",
                table: "SubmissionFiles",
                newName: "IX_SubmissionFiles_SubmissionId");

            // Add the new tracking columns
            migrationBuilder.AddColumn<int>(
                name: "CompletedLessonsCount",
                table: "Enrollments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalLessonCount",
                table: "Courses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubmissionFiles",
                table: "SubmissionFiles",
                column: "Id");

            // Re-add foreign key using the new plural tables
            migrationBuilder.AddForeignKey(
                name: "FK_SubmissionFiles_Submissions_SubmissionId",
                table: "SubmissionFiles",
                column: "SubmissionId",
                principalTable: "Submissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubmissionFiles_Submissions_SubmissionId",
                table: "SubmissionFiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubmissionFiles",
                table: "SubmissionFiles");

            migrationBuilder.DropColumn(
                name: "CompletedLessonsCount",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "TotalLessonCount",
                table: "Courses");

            migrationBuilder.RenameTable(
                name: "SubmissionFiles",
                newName: "SubmissionFile");

            migrationBuilder.RenameIndex(
                name: "IX_SubmissionFiles_SubmissionId",
                table: "SubmissionFile",
                newName: "IX_SubmissionFile_SubmissionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubmissionFile",
                table: "SubmissionFile",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubmissionFile_Submissions_SubmissionId",
                table: "SubmissionFile",
                column: "SubmissionId",
                principalTable: "Submissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
