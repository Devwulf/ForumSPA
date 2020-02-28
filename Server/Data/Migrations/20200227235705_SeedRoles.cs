using Microsoft.EntityFrameworkCore.Migrations;

namespace ForumSPA.Server.Data.Migrations
{
    public partial class SeedRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "dadf71a3-b0ac-42dc-835e-1d04d2e4ff05", "157439bc-11ae-4254-a197-49573871134f", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "3742475a-4c99-43eb-b719-7b497fec4e73", "f3aa0b73-fc66-47bc-b516-24b89ca85e22", "Moderator", "MODERATOR" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "38cee0d9-c8e4-40f3-b944-55514ba7c210", "729a35a3-cf56-4a86-85fa-fb6d24b8ec7e", "Administrator", "ADMINISTRATOR" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3742475a-4c99-43eb-b719-7b497fec4e73");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "38cee0d9-c8e4-40f3-b944-55514ba7c210");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "dadf71a3-b0ac-42dc-835e-1d04d2e4ff05");
        }
    }
}
