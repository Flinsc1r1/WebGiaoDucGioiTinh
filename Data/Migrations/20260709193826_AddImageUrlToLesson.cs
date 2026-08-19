using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebGiaoDucGioiTinh.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddImageUrlToLesson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Lessons",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Lessons");
        }
    }
}
