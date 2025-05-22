using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FamilyPhotos.Migrations
{
    /// <inheritdoc />
    public partial class AddFolderPeopleNavigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_folder_people_folder_id",
                table: "folder_people");

            migrationBuilder.AddPrimaryKey(
                name: "PK_folder_people",
                table: "folder_people",
                columns: new[] { "folder_id", "person_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_folder_people",
                table: "folder_people");

            migrationBuilder.CreateIndex(
                name: "IX_folder_people_folder_id",
                table: "folder_people",
                column: "folder_id");
        }
    }
}
