using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebGiaoDucGioiTinh.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSecretCodeFinish : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SecretCode",
                table: "AnonymousQuestions",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SecretCode",
                table: "AnonymousQuestions");
        }
    }
}
