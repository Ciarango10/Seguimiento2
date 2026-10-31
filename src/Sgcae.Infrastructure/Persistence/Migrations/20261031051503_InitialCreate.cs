using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sgcae.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "sgcae");

            migrationBuilder.CreateTable(
                name: "AccessPoints",
                schema: "sgcae",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Kind = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    RegisteredAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessPoints", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                schema: "sgcae",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FirstNames = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    LastNames = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    JobTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RegisteredAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DeactivatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Credentials",
                schema: "sgcae",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nchar(13)", fixedLength: true, maxLength: 13, nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssuedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    StatusChangedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    StatusReason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Credentials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Credentials_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "sgcae",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccessEvents",
                schema: "sgcae",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PresentedCredentialCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    PresentedAccessPointCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CredentialId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AccessPointId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Direction = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Result = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DenialReason = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    OccurredAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccessEvents_AccessPoints_AccessPointId",
                        column: x => x.AccessPointId,
                        principalSchema: "sgcae",
                        principalTable: "AccessPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccessEvents_Credentials_CredentialId",
                        column: x => x.CredentialId,
                        principalSchema: "sgcae",
                        principalTable: "Credentials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccessEvents_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "sgcae",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccessEvents_AccessPointId_OccurredAt",
                schema: "sgcae",
                table: "AccessEvents",
                columns: new[] { "AccessPointId", "OccurredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_AccessEvents_CredentialId",
                schema: "sgcae",
                table: "AccessEvents",
                column: "CredentialId");

            migrationBuilder.CreateIndex(
                name: "IX_AccessEvents_EmployeeId_OccurredAt",
                schema: "sgcae",
                table: "AccessEvents",
                columns: new[] { "EmployeeId", "OccurredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_AccessEvents_OccurredAt",
                schema: "sgcae",
                table: "AccessEvents",
                column: "OccurredAt");

            migrationBuilder.CreateIndex(
                name: "IX_AccessEvents_Result_OccurredAt",
                schema: "sgcae",
                table: "AccessEvents",
                columns: new[] { "Result", "OccurredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_AccessPoints_Code",
                schema: "sgcae",
                table: "AccessPoints",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccessPoints_IsEnabled",
                schema: "sgcae",
                table: "AccessPoints",
                column: "IsEnabled");

            migrationBuilder.CreateIndex(
                name: "IX_Credentials_Code",
                schema: "sgcae",
                table: "Credentials",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Credentials_EmployeeId_Current",
                schema: "sgcae",
                table: "Credentials",
                column: "EmployeeId",
                unique: true,
                filter: "[Status] <> 'Revoked'");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Department",
                schema: "sgcae",
                table: "Employees",
                column: "Department");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DocumentNumber",
                schema: "sgcae",
                table: "Employees",
                column: "DocumentNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Email",
                schema: "sgcae",
                table: "Employees",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Status",
                schema: "sgcae",
                table: "Employees",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessEvents",
                schema: "sgcae");

            migrationBuilder.DropTable(
                name: "AccessPoints",
                schema: "sgcae");

            migrationBuilder.DropTable(
                name: "Credentials",
                schema: "sgcae");

            migrationBuilder.DropTable(
                name: "Employees",
                schema: "sgcae");
        }
    }
}
