using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheBoard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BoardItemCreatedDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "BoardItems",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "BoardItems");
        }
    }
}
