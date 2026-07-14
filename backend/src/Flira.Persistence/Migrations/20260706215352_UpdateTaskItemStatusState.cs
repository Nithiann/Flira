using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flira.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTaskItemStatusState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "TaskItems",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9f42de2c-4974-4b5b-a7e8-b7fb3f8b8a01",
                column: "ConcurrencyStamp",
                value: "d961b1eb-536a-4999-b0ea-5cfd232d91b1");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9f42de2c-4974-4b5b-a7e8-b7fb3f8b8a02",
                column: "ConcurrencyStamp",
                value: "3de374e8-bb28-4012-b718-caeacd3739e4");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9f42de2c-4974-4b5b-a7e8-b7fb3f8b8a03",
                column: "ConcurrencyStamp",
                value: "3db1020a-33cb-494c-bd70-7156c3fb5df2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "TaskItems",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9f42de2c-4974-4b5b-a7e8-b7fb3f8b8a01",
                column: "ConcurrencyStamp",
                value: "5c787055-3638-488e-912a-6b74293e5424");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9f42de2c-4974-4b5b-a7e8-b7fb3f8b8a02",
                column: "ConcurrencyStamp",
                value: "944fe30d-a7e2-48c3-8c86-4d098297af2e");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9f42de2c-4974-4b5b-a7e8-b7fb3f8b8a03",
                column: "ConcurrencyStamp",
                value: "154bdbd7-3b58-499c-8d0a-1ead377cb8ef");
        }
    }
}
