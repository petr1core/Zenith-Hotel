using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class RenameGuesttoUserAddNewFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Guests_guestId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Guests_guestId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Guests_guestId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomRatings_Guests_guestId",
                table: "RoomRatings");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceOrders_Guests_guestId",
                table: "ServiceOrders");

            migrationBuilder.DropTable(
                name: "Guests");

            migrationBuilder.DropIndex(
                name: "IX_Payments_guestId",
                table: "Payments");

            migrationBuilder.RenameColumn(
                name: "Role",
                table: "Users",
                newName: "role");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Users",
                newName: "userId");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "Users",
                newName: "userPhoneNumber");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Users",
                newName: "userPasswordHash");

            migrationBuilder.RenameColumn(
                name: "guestId",
                table: "ServiceOrders",
                newName: "userId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceOrders_guestId",
                table: "ServiceOrders",
                newName: "IX_ServiceOrders_userId");

            migrationBuilder.RenameColumn(
                name: "guestId",
                table: "RoomRatings",
                newName: "userId");

            migrationBuilder.RenameIndex(
                name: "IX_RoomRatings_guestId",
                table: "RoomRatings",
                newName: "IX_RoomRatings_userId");

            migrationBuilder.RenameColumn(
                name: "guestId",
                table: "Reviews",
                newName: "userId");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_guestId",
                table: "Reviews",
                newName: "IX_Reviews_userId");

            migrationBuilder.RenameColumn(
                name: "guestId",
                table: "Bookings",
                newName: "userId");

            migrationBuilder.RenameIndex(
                name: "IX_Bookings_guestId",
                table: "Bookings",
                newName: "IX_Bookings_userId");

            migrationBuilder.AlterColumn<Guid>(
                name: "userId",
                table: "Users",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v4()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<DateTime>(
                name: "lastLoginDate",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "registrationDate",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "userEmail",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "userFirstname",
                table: "Users",
                type: "varchar(128)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "userLastname",
                table: "Users",
                type: "varchar(128)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "roomType",
                table: "Rooms",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Payments",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AdminProfiles",
                columns: table => new
                {
                    adminProfileId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    adminId = table.Column<Guid>(type: "uuid", nullable: false),
                    AvatarURL = table.Column<string>(type: "text", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminProfiles", x => x.adminProfileId);
                });

            migrationBuilder.CreateTable(
                name: "GuestProfiles",
                columns: table => new
                {
                    guestProfileId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    userId = table.Column<Guid>(type: "uuid", nullable: false),
                    AvatarURL = table.Column<string>(type: "text", nullable: false),
                    ReviewCount = table.Column<int>(type: "integer", nullable: true),
                    LastReviewDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    isVerified = table.Column<bool>(type: "boolean", nullable: false),
                    isBanned = table.Column<bool>(type: "boolean", nullable: false),
                    banReason = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuestProfiles", x => x.guestProfileId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_UserId",
                table: "Payments",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Users_userId",
                table: "Bookings",
                column: "userId",
                principalTable: "Users",
                principalColumn: "userId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Users_UserId",
                table: "Payments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Users_userId",
                table: "Reviews",
                column: "userId",
                principalTable: "Users",
                principalColumn: "userId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomRatings_Users_userId",
                table: "RoomRatings",
                column: "userId",
                principalTable: "Users",
                principalColumn: "userId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceOrders_Users_userId",
                table: "ServiceOrders",
                column: "userId",
                principalTable: "Users",
                principalColumn: "userId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Users_userId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Users_UserId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Users_userId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomRatings_Users_userId",
                table: "RoomRatings");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceOrders_Users_userId",
                table: "ServiceOrders");

            migrationBuilder.DropTable(
                name: "AdminProfiles");

            migrationBuilder.DropTable(
                name: "GuestProfiles");

            migrationBuilder.DropIndex(
                name: "IX_Payments_UserId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "lastLoginDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "registrationDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "userEmail",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "userFirstname",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "userLastname",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Payments");

            migrationBuilder.RenameColumn(
                name: "role",
                table: "Users",
                newName: "Role");

            migrationBuilder.RenameColumn(
                name: "userId",
                table: "Users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "userPhoneNumber",
                table: "Users",
                newName: "PasswordHash");

            migrationBuilder.RenameColumn(
                name: "userPasswordHash",
                table: "Users",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "userId",
                table: "ServiceOrders",
                newName: "guestId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceOrders_userId",
                table: "ServiceOrders",
                newName: "IX_ServiceOrders_guestId");

            migrationBuilder.RenameColumn(
                name: "userId",
                table: "RoomRatings",
                newName: "guestId");

            migrationBuilder.RenameIndex(
                name: "IX_RoomRatings_userId",
                table: "RoomRatings",
                newName: "IX_RoomRatings_guestId");

            migrationBuilder.RenameColumn(
                name: "userId",
                table: "Reviews",
                newName: "guestId");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_userId",
                table: "Reviews",
                newName: "IX_Reviews_guestId");

            migrationBuilder.RenameColumn(
                name: "userId",
                table: "Bookings",
                newName: "guestId");

            migrationBuilder.RenameIndex(
                name: "IX_Bookings_userId",
                table: "Bookings",
                newName: "IX_Bookings_guestId");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Users",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuid_generate_v4()");

            migrationBuilder.AlterColumn<string>(
                name: "roomType",
                table: "Rooms",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateTable(
                name: "Guests",
                columns: table => new
                {
                    guestId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    guestEmail = table.Column<string>(type: "text", nullable: false),
                    guestFirstname = table.Column<string>(type: "varchar(128)", nullable: false),
                    guestLastname = table.Column<string>(type: "varchar(128)", nullable: false),
                    guestPhoneNumber = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guests", x => x.guestId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_guestId",
                table: "Payments",
                column: "guestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Guests_guestId",
                table: "Bookings",
                column: "guestId",
                principalTable: "Guests",
                principalColumn: "guestId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Guests_guestId",
                table: "Payments",
                column: "guestId",
                principalTable: "Guests",
                principalColumn: "guestId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Guests_guestId",
                table: "Reviews",
                column: "guestId",
                principalTable: "Guests",
                principalColumn: "guestId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomRatings_Guests_guestId",
                table: "RoomRatings",
                column: "guestId",
                principalTable: "Guests",
                principalColumn: "guestId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceOrders_Guests_guestId",
                table: "ServiceOrders",
                column: "guestId",
                principalTable: "Guests",
                principalColumn: "guestId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
