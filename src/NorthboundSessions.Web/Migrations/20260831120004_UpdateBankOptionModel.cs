using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NorthboundSessions.Web.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBankOptionModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankOptions_BankOptions_BankOptionId",
                table: "BankOptions");

            migrationBuilder.DropForeignKey(
                name: "FK_BankOptions_BankQuestions_BankQuestionId",
                table: "BankOptions");

            migrationBuilder.DropForeignKey(
                name: "FK_BankOptions_TopicBankItems_TopicBankItemId",
                table: "BankOptions");

            migrationBuilder.DropIndex(
                name: "IX_BankOptions_BankOptionId",
                table: "BankOptions");

            migrationBuilder.DropIndex(
                name: "IX_BankOptions_TopicBankItemId",
                table: "BankOptions");

            migrationBuilder.DropColumn(
                name: "BankOptionId",
                table: "BankOptions");

            migrationBuilder.DropColumn(
                name: "TopicBankItemId",
                table: "BankOptions");

            migrationBuilder.RenameColumn(
                name: "QuestionText",
                table: "BankOptions",
                newName: "OptionText");

            migrationBuilder.AlterColumn<int>(
                name: "BankQuestionId",
                table: "BankOptions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCorrect",
                table: "BankOptions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_BankOptions_BankQuestions_BankQuestionId",
                table: "BankOptions",
                column: "BankQuestionId",
                principalTable: "BankQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankOptions_BankQuestions_BankQuestionId",
                table: "BankOptions");

            migrationBuilder.DropColumn(
                name: "IsCorrect",
                table: "BankOptions");

            migrationBuilder.RenameColumn(
                name: "OptionText",
                table: "BankOptions",
                newName: "QuestionText");

            migrationBuilder.AlterColumn<int>(
                name: "BankQuestionId",
                table: "BankOptions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "BankOptionId",
                table: "BankOptions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TopicBankItemId",
                table: "BankOptions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BankOptions_BankOptionId",
                table: "BankOptions",
                column: "BankOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_BankOptions_TopicBankItemId",
                table: "BankOptions",
                column: "TopicBankItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_BankOptions_BankOptions_BankOptionId",
                table: "BankOptions",
                column: "BankOptionId",
                principalTable: "BankOptions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BankOptions_BankQuestions_BankQuestionId",
                table: "BankOptions",
                column: "BankQuestionId",
                principalTable: "BankQuestions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BankOptions_TopicBankItems_TopicBankItemId",
                table: "BankOptions",
                column: "TopicBankItemId",
                principalTable: "TopicBankItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
