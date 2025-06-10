using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    deptId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    deptName = table.Column<string>(type: "varchar(255)", nullable: false),
                    deptDesc = table.Column<string>(type: "varchar(255)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.deptId);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeSalaries",
                columns: table => new
                {
                    employeeSalaryId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    hourlyRate = table.Column<decimal>(type: "numeric(9,2)", nullable: false),
                    monthlyHours = table.Column<decimal>(type: "numeric(9,2)", nullable: false),
                    timeWorked = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    paymentAmount = table.Column<decimal>(type: "numeric(9,2)", nullable: false),
                    paymentDate = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeSalaries", x => x.employeeSalaryId);
                });

            migrationBuilder.CreateTable(
                name: "Guests",
                columns: table => new
                {
                    guestId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    guestFirstname = table.Column<string>(type: "varchar(128)", nullable: false),
                    guestLastname = table.Column<string>(type: "varchar(128)", nullable: false),
                    guestPhoneNumber = table.Column<string>(type: "text", nullable: false),
                    guestEmail = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guests", x => x.guestId);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    roomId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    roomNumber = table.Column<int>(type: "integer", nullable: false),
                    roomType = table.Column<string>(type: "text", nullable: false),
                    availability = table.Column<int>(type: "integer", nullable: false),
                    roomCharge = table.Column<decimal>(type: "numeric(9,2)", nullable: false),
                    capacity = table.Column<int>(type: "integer", nullable: false),
                    area = table.Column<decimal>(type: "numeric", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    cleaningStatus = table.Column<int>(type: "integer", nullable: false),
                    lastCleaned = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    floor = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.roomId);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    serviceId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    serviceName = table.Column<string>(type: "varchar(128)", nullable: false),
                    servicePrice = table.Column<decimal>(type: "numeric(9,2)", nullable: false),
                    serviceIsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.serviceId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    employeeId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    departmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    employeeSalaryId = table.Column<Guid>(type: "uuid", nullable: false),
                    emplFirstname = table.Column<string>(type: "varchar(128)", nullable: false),
                    emplLastname = table.Column<string>(type: "varchar(128)", nullable: false),
                    emplJobPosition = table.Column<string>(type: "varchar(128)", nullable: false),
                    emplStatus = table.Column<string>(type: "varchar(128)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.employeeId);
                    table.ForeignKey(
                        name: "FK_Employees_Departments_departmentId",
                        column: x => x.departmentId,
                        principalTable: "Departments",
                        principalColumn: "deptId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Employees_EmployeeSalaries_employeeSalaryId",
                        column: x => x.employeeSalaryId,
                        principalTable: "EmployeeSalaries",
                        principalColumn: "employeeSalaryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    paymentId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    guestId = table.Column<Guid>(type: "uuid", nullable: false),
                    timeWorked = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    paymentAmount = table.Column<decimal>(type: "numeric(9,2)", nullable: false),
                    paymentMethod = table.Column<string>(type: "varchar(128)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.paymentId);
                    table.ForeignKey(
                        name: "FK_Payments_Guests_guestId",
                        column: x => x.guestId,
                        principalTable: "Guests",
                        principalColumn: "guestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    bookingId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    guestId = table.Column<Guid>(type: "uuid", nullable: false),
                    roomId = table.Column<Guid>(type: "uuid", nullable: false),
                    bookingStatus = table.Column<string>(type: "varchar(128)", nullable: false),
                    checkInDate = table.Column<DateOnly>(type: "date", nullable: false),
                    checkOutDate = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.bookingId);
                    table.ForeignKey(
                        name: "FK_Bookings_Guests_guestId",
                        column: x => x.guestId,
                        principalTable: "Guests",
                        principalColumn: "guestId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bookings_Rooms_roomId",
                        column: x => x.roomId,
                        principalTable: "Rooms",
                        principalColumn: "roomId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoomPhotos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    PhotoUrl = table.Column<string>(type: "text", nullable: false),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    UploadDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false),
                    roomId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomPhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomPhotos_Rooms_roomId",
                        column: x => x.roomId,
                        principalTable: "Rooms",
                        principalColumn: "roomId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoomRatings",
                columns: table => new
                {
                    ratingId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    roomId = table.Column<Guid>(type: "uuid", nullable: false),
                    guestId = table.Column<Guid>(type: "uuid", nullable: false),
                    roomRate = table.Column<int>(type: "integer", nullable: false),
                    comment = table.Column<string>(type: "varchar(512)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomRatings", x => x.ratingId);
                    table.ForeignKey(
                        name: "FK_RoomRatings_Guests_guestId",
                        column: x => x.guestId,
                        principalTable: "Guests",
                        principalColumn: "guestId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoomRatings_Rooms_roomId",
                        column: x => x.roomId,
                        principalTable: "Rooms",
                        principalColumn: "roomId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceOrders",
                columns: table => new
                {
                    serviceOrderId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    serviceId = table.Column<Guid>(type: "uuid", nullable: false),
                    guestId = table.Column<Guid>(type: "uuid", nullable: false),
                    roomId = table.Column<Guid>(type: "uuid", nullable: false),
                    departmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    orderDate = table.Column<DateOnly>(type: "date", nullable: false),
                    orderStatus = table.Column<string>(type: "varchar(128)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceOrders", x => x.serviceOrderId);
                    table.ForeignKey(
                        name: "FK_ServiceOrders_Departments_departmentId",
                        column: x => x.departmentId,
                        principalTable: "Departments",
                        principalColumn: "deptId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceOrders_Guests_guestId",
                        column: x => x.guestId,
                        principalTable: "Guests",
                        principalColumn: "guestId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceOrders_Rooms_roomId",
                        column: x => x.roomId,
                        principalTable: "Rooms",
                        principalColumn: "roomId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceOrders_Services_serviceId",
                        column: x => x.serviceId,
                        principalTable: "Services",
                        principalColumn: "serviceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    reviewId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    guestId = table.Column<Guid>(type: "uuid", nullable: false),
                    bookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    reviewDate = table.Column<DateOnly>(type: "date", nullable: false),
                    reviewRate = table.Column<int>(type: "integer", nullable: false),
                    reviewText = table.Column<string>(type: "varchar(512)", nullable: false),
                    moderationStatus = table.Column<string>(type: "varchar(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.reviewId);
                    table.ForeignKey(
                        name: "FK_Reviews_Bookings_bookingId",
                        column: x => x.bookingId,
                        principalTable: "Bookings",
                        principalColumn: "bookingId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_Guests_guestId",
                        column: x => x.guestId,
                        principalTable: "Guests",
                        principalColumn: "guestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceEmployees",
                columns: table => new
                {
                    serviceOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    employeeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_ServiceEmployees_Employees_employeeId",
                        column: x => x.employeeId,
                        principalTable: "Employees",
                        principalColumn: "employeeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceEmployees_ServiceOrders_serviceOrderId",
                        column: x => x.serviceOrderId,
                        principalTable: "ServiceOrders",
                        principalColumn: "serviceOrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_guestId",
                table: "Bookings",
                column: "guestId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_roomId",
                table: "Bookings",
                column: "roomId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_departmentId",
                table: "Employees",
                column: "departmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_employeeSalaryId",
                table: "Employees",
                column: "employeeSalaryId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_guestId",
                table: "Payments",
                column: "guestId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_bookingId",
                table: "Reviews",
                column: "bookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_guestId",
                table: "Reviews",
                column: "guestId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomPhotos_roomId",
                table: "RoomPhotos",
                column: "roomId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomRatings_guestId",
                table: "RoomRatings",
                column: "guestId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomRatings_roomId",
                table: "RoomRatings",
                column: "roomId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceEmployees_employeeId",
                table: "ServiceEmployees",
                column: "employeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceEmployees_serviceOrderId",
                table: "ServiceEmployees",
                column: "serviceOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOrders_departmentId",
                table: "ServiceOrders",
                column: "departmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOrders_guestId",
                table: "ServiceOrders",
                column: "guestId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOrders_roomId",
                table: "ServiceOrders",
                column: "roomId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOrders_serviceId",
                table: "ServiceOrders",
                column: "serviceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "RoomPhotos");

            migrationBuilder.DropTable(
                name: "RoomRatings");

            migrationBuilder.DropTable(
                name: "ServiceEmployees");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "ServiceOrders");

            migrationBuilder.DropTable(
                name: "EmployeeSalaries");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Guests");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Services");
        }
    }
}
