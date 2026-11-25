using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NorthboundSessions.Web.Migrations
{
    /// <inheritdoc />
    public partial class LessonProgression : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PassingScorePercent",
                table: "Quizzes",
                type: "integer",
                nullable: false,
                defaultValue: 70);

            migrationBuilder.AddColumn<bool>(
                name: "Passed",
                table: "QuizAttempts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "Lessons",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CompletedAt",
                table: "LessonProgresses",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "LessonProgresses",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql(@"
UPDATE ""Lessons""
SET ""SortOrder"" = ranked.""SortOrder""
FROM (
    SELECT ""Id"", ROW_NUMBER() OVER (ORDER BY ""ReleaseDate"", ""Id"") AS ""SortOrder""
    FROM ""Lessons""
) ranked
WHERE ""Lessons"".""Id"" = ranked.""Id"";

UPDATE ""QuizAttempts"" AS a
SET ""Passed"" = scores.""Passed""
FROM (
    SELECT att.""Id"",
           CASE
               WHEN COUNT(qq.""Id"") = 0 THEN FALSE
               ELSE att.""Score"" * 100.0 / COUNT(qq.""Id"") >= q.""PassingScorePercent""
           END AS ""Passed""
    FROM ""QuizAttempts"" AS att
    INNER JOIN ""Quizzes"" AS q ON q.""Id"" = att.""QuizId""
    LEFT JOIN ""QuizQuestions"" AS qq ON qq.""QuizId"" = att.""QuizId""
    GROUP BY att.""Id"", att.""Score"", q.""PassingScorePercent""
) AS scores
WHERE scores.""Id"" = a.""Id"";

UPDATE ""LessonProgresses"" AS lp
SET ""IsCompleted"" = true,
    ""CompletedAt"" = passed.""SubmittedAt""
FROM (
    SELECT q.""LessonId"", a.""StudentId"", MIN(a.""SubmittedAt"") AS ""SubmittedAt""
    FROM ""QuizAttempts"" AS a
    INNER JOIN ""Quizzes"" AS q ON q.""Id"" = a.""QuizId""
    WHERE a.""Passed"" = true
    GROUP BY q.""LessonId"", a.""StudentId""
) AS passed
WHERE passed.""LessonId"" = lp.""LessonId"" AND passed.""StudentId"" = lp.""StudentId"";
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PassingScorePercent",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "Passed",
                table: "QuizAttempts");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "LessonProgresses");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "LessonProgresses");
        }
    }
}
