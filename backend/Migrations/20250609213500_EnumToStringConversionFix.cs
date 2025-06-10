using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class EnumToStringConversionFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "moderationStatus",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "reviewText",
                table: "Reviews");

            migrationBuilder.RenameColumn(
                name: "reviewRate",
                table: "Reviews",
                newName: "rating");

            migrationBuilder.RenameColumn(
                name: "reviewDate",
                table: "Reviews",
                newName: "createdAt");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModeratedAt",
                table: "Reviews",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "comment",
                table: "Reviews",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "moderatorComment",
                table: "Reviews",
                type: "varchar(512)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "roomId",
                table: "Reviews",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "Reviews",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_roomId",
                table: "Reviews",
                column: "roomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Rooms_roomId",
                table: "Reviews",
                column: "roomId",
                principalTable: "Rooms",
                principalColumn: "roomId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Rooms_roomId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_roomId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "ModeratedAt",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "comment",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "moderatorComment",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "roomId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "status",
                table: "Reviews");

            migrationBuilder.RenameColumn(
                name: "rating",
                table: "Reviews",
                newName: "reviewRate");

            migrationBuilder.RenameColumn(
                name: "createdAt",
                table: "Reviews",
                newName: "reviewDate");

            migrationBuilder.AddColumn<string>(
                name: "moderationStatus",
                table: "Reviews",
                type: "varchar(20)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "reviewText",
                table: "Reviews",
                type: "varchar(512)",
                nullable: false,
                defaultValue: "");
        }
    }
}
