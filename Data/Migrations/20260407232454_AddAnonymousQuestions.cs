using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebGiaoDucGioiTinh.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAnonymousQuestions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnonymousQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", maxLength: 4000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsAnswered = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnonymousQuestions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnonymousQuestions_CreatedAt",
                table: "AnonymousQuestions",
                column: "CreatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnonymousQuestions");
        }
    }
}
