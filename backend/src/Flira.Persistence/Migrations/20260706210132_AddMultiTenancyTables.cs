using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flira.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiTenancyTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrganizationUsers",
                columns: table => new
                {
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationUsers", x => new { x.OrganizationId, x.UserId });
                    table.ForeignKey(
                        name: "FK_OrganizationUsers_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeamUsers",
                columns: table => new
                {
                    TeamId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamUsers", x => new { x.TeamId, x.UserId });
                    table.ForeignKey(
                        name: "FK_TeamUsers_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganizationUsers");

            migrationBuilder.DropTable(
                name: "TeamUsers");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9f42de2c-4974-4b5b-a7e8-b7fb3f8b8a01",
                column: "ConcurrencyStamp",
                value: "1063b4c9-ca58-4aeb-a0de-359a8a115699");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9f42de2c-4974-4b5b-a7e8-b7fb3f8b8a02",
                column: "ConcurrencyStamp",
                value: "6855d72b-343d-4471-ab65-4873da605150");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9f42de2c-4974-4b5b-a7e8-b7fb3f8b8a03",
                column: "ConcurrencyStamp",
                value: "70d0447e-0b00-4453-99c5-f463325e9027");
        }
    }
}
