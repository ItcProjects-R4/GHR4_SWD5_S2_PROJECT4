using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddPublicIdFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FilePublicId",
                table: "SubmissionFile",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailPublicId",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideoPublicId",
                table: "Contents",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePublicId",
                table: "SubmissionFile");

            migrationBuilder.DropColumn(
                name: "ThumbnailPublicId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "VideoPublicId",
                table: "Contents");
        }
    }
}
