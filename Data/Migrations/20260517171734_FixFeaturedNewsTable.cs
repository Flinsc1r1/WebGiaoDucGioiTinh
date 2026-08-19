using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebGiaoDucGioiTinh.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixFeaturedNewsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FeaturedNews",
                table: "FeaturedNews");

            migrationBuilder.RenameTable(
                name: "FeaturedNews",
                newName: "FeaturedNewsList");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FeaturedNewsList",
                table: "FeaturedNewsList",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FeaturedNewsList",
                table: "FeaturedNewsList");

            migrationBuilder.RenameTable(
                name: "FeaturedNewsList",
                newName: "FeaturedNews");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FeaturedNews",
                table: "FeaturedNews",
                column: "Id");
        }
    }
}
