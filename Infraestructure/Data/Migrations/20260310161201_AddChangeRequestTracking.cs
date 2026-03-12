using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infraestructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddChangeRequestTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AvatarUrl",
                table: "CommitteeMembers",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RequestType",
                table: "ChangeRequests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TicketNumber",
                table: "ChangeRequests",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ChangeRequestAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChangeRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    SizeInBytes = table.Column<long>(type: "bigint", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChangeRequestAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChangeRequestAttachments_ChangeRequests_ChangeRequestId",
                        column: x => x.ChangeRequestId,
                        principalTable: "ChangeRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChangeRequestStages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChangeRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    StageName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChangeRequestStages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChangeRequestStages_ChangeRequests_ChangeRequestId",
                        column: x => x.ChangeRequestId,
                        principalTable: "ChangeRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequests_ApplicantId",
                table: "ChangeRequests",
                column: "ApplicantId");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequests_ProjectId",
                table: "ChangeRequests",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequests_TicketNumber",
                table: "ChangeRequests",
                column: "TicketNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequestAttachments_ChangeRequestId",
                table: "ChangeRequestAttachments",
                column: "ChangeRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequestStages_ChangeRequestId_Sequence",
                table: "ChangeRequestStages",
                columns: new[] { "ChangeRequestId", "Sequence" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ChangeRequests_Applicants_ApplicantId",
                table: "ChangeRequests",
                column: "ApplicantId",
                principalTable: "Applicants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChangeRequests_Projects_ProjectId",
                table: "ChangeRequests",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChangeRequests_Applicants_ApplicantId",
                table: "ChangeRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ChangeRequests_Projects_ProjectId",
                table: "ChangeRequests");

            migrationBuilder.DropTable(
                name: "ChangeRequestAttachments");

            migrationBuilder.DropTable(
                name: "ChangeRequestStages");

            migrationBuilder.DropIndex(
                name: "IX_ChangeRequests_ApplicantId",
                table: "ChangeRequests");

            migrationBuilder.DropIndex(
                name: "IX_ChangeRequests_ProjectId",
                table: "ChangeRequests");

            migrationBuilder.DropIndex(
                name: "IX_ChangeRequests_TicketNumber",
                table: "ChangeRequests");

            migrationBuilder.DropColumn(
                name: "AvatarUrl",
                table: "CommitteeMembers");

            migrationBuilder.DropColumn(
                name: "TicketNumber",
                table: "ChangeRequests");

            migrationBuilder.AlterColumn<string>(
                name: "RequestType",
                table: "ChangeRequests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);
        }
    }
}
