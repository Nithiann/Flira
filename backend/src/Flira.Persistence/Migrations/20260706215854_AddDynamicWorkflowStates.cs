using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flira.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDynamicWorkflowStates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectTaskStates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    AllowedTransitionsJson = table.Column<string>(type: "text", nullable: false),
                    IsInitial = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTaskStates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectTaskStates_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9f42de2c-4974-4b5b-a7e8-b7fb3f8b8a01",
                column: "ConcurrencyStamp",
                value: "b46889fc-4af0-48b8-abc5-e362fb7a1002");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9f42de2c-4974-4b5b-a7e8-b7fb3f8b8a02",
                column: "ConcurrencyStamp",
                value: "212d6420-6f93-4bd0-9170-e1c6ae0adaec");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9f42de2c-4974-4b5b-a7e8-b7fb3f8b8a03",
                column: "ConcurrencyStamp",
                value: "d946138c-d6a7-4e05-9678-1506fb832042");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTaskStates_ProjectId_Name",
                table: "ProjectTaskStates",
                columns: new[] { "ProjectId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectTaskStates");

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
    }
}
