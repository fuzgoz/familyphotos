using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FamilyPhotos.Migrations
{
    /// <inheritdoc />
    public partial class LinkIdentityUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "user_id",
                table: "photos",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "user_id",
                table: "folders",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_photos_user_id",
                table: "photos",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_folders_user_id",
                table: "folders",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_folders_Users_user_id",
                table: "folders",
                column: "user_id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_photos_Users_user_id",
                table: "photos",
                column: "user_id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_folders_Users_user_id",
                table: "folders");

            migrationBuilder.DropForeignKey(
                name: "FK_photos_Users_user_id",
                table: "photos");

            migrationBuilder.DropIndex(
                name: "IX_photos_user_id",
                table: "photos");

            migrationBuilder.DropIndex(
                name: "IX_folders_user_id",
                table: "folders");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Users");

            migrationBuilder.AlterColumn<int>(
                name: "user_id",
                table: "photos",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "user_id",
                table: "folders",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    is_admin = table.Column<bool>(type: "boolean", nullable: true),
                    password = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("users_pkey", x => x.id);
                });
        }
    }
}
