using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NorthboundSessions.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddTopicBank : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TopicBankItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OutlineContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MarketSymbol = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TopicBankItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BankQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TopicBankItemId = table.Column<int>(type: "int", nullable: false),
                    QuestionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BankQuestions_TopicBankItems_TopicBankItemId",
                        column: x => x.TopicBankItemId,
                        principalTable: "TopicBankItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BankOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TopicBankItemId = table.Column<int>(type: "int", nullable: false),
                    QuestionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    BankOptionId = table.Column<int>(type: "int", nullable: true),
                    BankQuestionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BankOptions_BankOptions_BankOptionId",
                        column: x => x.BankOptionId,
                        principalTable: "BankOptions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BankOptions_BankQuestions_BankQuestionId",
                        column: x => x.BankQuestionId,
                        principalTable: "BankQuestions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BankOptions_TopicBankItems_TopicBankItemId",
                        column: x => x.TopicBankItemId,
                        principalTable: "TopicBankItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BankOptions_BankOptionId",
                table: "BankOptions",
                column: "BankOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_BankOptions_BankQuestionId",
                table: "BankOptions",
                column: "BankQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_BankOptions_TopicBankItemId",
                table: "BankOptions",
                column: "TopicBankItemId");

            migrationBuilder.CreateIndex(
                name: "IX_BankQuestions_TopicBankItemId",
                table: "BankQuestions",
                column: "TopicBankItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BankOptions");

            migrationBuilder.DropTable(
                name: "BankQuestions");

            migrationBuilder.DropTable(
                name: "TopicBankItems");
        }
    }
}
