using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class NavPropertyFixAll : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.CreateIndex(
                name: "IX_GuestProfiles_userId",
                table: "GuestProfiles",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_AdminProfiles_adminId",
                table: "AdminProfiles",
                column: "adminId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdminProfiles_Users_adminId",
                table: "AdminProfiles",
                column: "adminId",
                principalTable: "Users",
                principalColumn: "userId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GuestProfiles_Users_userId",
                table: "GuestProfiles",
                column: "userId",
                principalTable: "Users",
                principalColumn: "userId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdminProfiles_Users_adminId",
                table: "AdminProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_GuestProfiles_Users_userId",
                table: "GuestProfiles");

            migrationBuilder.DropIndex(
                name: "IX_GuestProfiles_userId",
                table: "GuestProfiles");

            migrationBuilder.DropIndex(
                name: "IX_AdminProfiles_adminId",
                table: "AdminProfiles");

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    paymentId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    guestId = table.Column<Guid>(type: "uuid", nullable: false),
                    paymentAmount = table.Column<decimal>(type: "numeric(9,2)", nullable: false),
                    paymentMethod = table.Column<string>(type: "varchar(128)", nullable: false),
                    timeWorked = table.Column<decimal>(type: "numeric(5,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.paymentId);
                    table.ForeignKey(
                        name: "FK_Payments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_UserId",
                table: "Payments",
                column: "UserId");
        }
    }
}
