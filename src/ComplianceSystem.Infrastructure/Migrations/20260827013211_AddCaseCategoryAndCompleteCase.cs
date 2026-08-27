using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComplianceSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCaseCategoryAndCompleteCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AssignedAnalystId",
                table: "Cases",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "Cases",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "DueAt",
                table: "Cases",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "EscalatedAt",
                table: "Cases",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEscalated",
                table: "Cases",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ResolutionExplanation",
                table: "Cases",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResolutionOutcome",
                table: "Cases",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResolvedAt",
                table: "Cases",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CaseCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseCategories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cases_AssignedAnalystId",
                table: "Cases",
                column: "AssignedAnalystId");

            migrationBuilder.CreateIndex(
                name: "IX_Cases_CategoryId",
                table: "Cases",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Cases_CreatedByUserId",
                table: "Cases",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseCategories_Code",
                table: "CaseCategories",
                column: "Code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Cases_AspNetUsers_AssignedAnalystId",
                table: "Cases",
                column: "AssignedAnalystId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Cases_AspNetUsers_CreatedByUserId",
                table: "Cases",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Cases_CaseCategories_CategoryId",
                table: "Cases",
                column: "CategoryId",
                principalTable: "CaseCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cases_AspNetUsers_AssignedAnalystId",
                table: "Cases");

            migrationBuilder.DropForeignKey(
                name: "FK_Cases_AspNetUsers_CreatedByUserId",
                table: "Cases");

            migrationBuilder.DropForeignKey(
                name: "FK_Cases_CaseCategories_CategoryId",
                table: "Cases");

            migrationBuilder.DropTable(
                name: "CaseCategories");

            migrationBuilder.DropIndex(
                name: "IX_Cases_AssignedAnalystId",
                table: "Cases");

            migrationBuilder.DropIndex(
                name: "IX_Cases_CategoryId",
                table: "Cases");

            migrationBuilder.DropIndex(
                name: "IX_Cases_CreatedByUserId",
                table: "Cases");

            migrationBuilder.DropColumn(
                name: "AssignedAnalystId",
                table: "Cases");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Cases");

            migrationBuilder.DropColumn(
                name: "DueAt",
                table: "Cases");

            migrationBuilder.DropColumn(
                name: "EscalatedAt",
                table: "Cases");

            migrationBuilder.DropColumn(
                name: "IsEscalated",
                table: "Cases");

            migrationBuilder.DropColumn(
                name: "ResolutionExplanation",
                table: "Cases");

            migrationBuilder.DropColumn(
                name: "ResolutionOutcome",
                table: "Cases");

            migrationBuilder.DropColumn(
                name: "ResolvedAt",
                table: "Cases");
        }
    }
}
