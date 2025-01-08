using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTestResult : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TestResultId",
                table: "Answers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TestResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FilmId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestResults_Films_FilmId",
                        column: x => x.FilmId,
                        principalTable: "Films",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Answers_TestResultId",
                table: "Answers",
                column: "TestResultId");

            migrationBuilder.CreateIndex(
                name: "IX_TestResults_FilmId",
                table: "TestResults",
                column: "FilmId");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_TestResults_TestResultId",
                table: "Answers",
                column: "TestResultId",
                principalTable: "TestResults",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_TestResults_TestResultId",
                table: "Answers");

            migrationBuilder.DropTable(
                name: "TestResults");

            migrationBuilder.DropIndex(
                name: "IX_Answers_TestResultId",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "TestResultId",
                table: "Answers");
        }
    }
}
