using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NorthboundSessions.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddLessonAndTopicImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "ImageBytes",
                table: "TopicBankItems",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageBytes",
                table: "Lessons",
                type: "varbinary(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageBytes",
                table: "TopicBankItems");

            migrationBuilder.DropColumn(
                name: "ImageBytes",
                table: "Lessons");
        }
    }
}
