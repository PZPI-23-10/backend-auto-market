using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend_auto_market.Migrations
{
    /// <inheritdoc />
    public partial class VerificationCodesFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "EmailVerificationCodes");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "EmailVerificationCodes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_EmailVerificationCodes_UserId",
                table: "EmailVerificationCodes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailVerificationCodes_Users_UserId",
                table: "EmailVerificationCodes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailVerificationCodes_Users_UserId",
                table: "EmailVerificationCodes");

            migrationBuilder.DropIndex(
                name: "IX_EmailVerificationCodes_UserId",
                table: "EmailVerificationCodes");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "EmailVerificationCodes");

            migrationBuilder.AddColumn<string>(
                name: "RecoveryCode",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RecoveryCodeExpiresAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerificationCode",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerificationCodeExpiresAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "EmailVerificationCodes",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
