using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flira.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEpic6Fields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "TaskItems",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Labels",
                table: "TaskItems",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9f42de2c-4974-4b5b-a7e8-b7fb3f8b8a01",
                column: "ConcurrencyStamp",
                value: "59b1407b-f463-48e2-aea4-07d6a1ac0dc7");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9f42de2c-4974-4b5b-a7e8-b7fb3f8b8a02",
                column: "ConcurrencyStamp",
                value: "d05ea911-1694-41ba-a4bb-c7881823172e");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9f42de2c-4974-4b5b-a7e8-b7fb3f8b8a03",
                column: "ConcurrencyStamp",
                value: "add1cb63-d6d6-466b-b3cc-f87ad8d21a0e");

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_Description",
                table: "TaskItems",
                column: "Description");

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_Labels",
                table: "TaskItems",
                column: "Labels");

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_Title",
                table: "TaskItems",
                column: "Title");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TaskItems_Description",
                table: "TaskItems");

            migrationBuilder.DropIndex(
                name: "IX_TaskItems_Labels",
                table: "TaskItems");

            migrationBuilder.DropIndex(
                name: "IX_TaskItems_Title",
                table: "TaskItems");

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "TaskItems");

            migrationBuilder.DropColumn(
                name: "Labels",
                table: "TaskItems");

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
        }
    }
}
