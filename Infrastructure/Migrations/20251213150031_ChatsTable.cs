using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChatsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chat_AspNetUsers_FirstUserId",
                table: "Chat");

            migrationBuilder.DropForeignKey(
                name: "FK_Chat_AspNetUsers_SecondUserId",
                table: "Chat");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_Chat_ChatId",
                table: "ChatMessages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Chat",
                table: "Chat");

            migrationBuilder.RenameTable(
                name: "Chat",
                newName: "Chats");

            migrationBuilder.RenameIndex(
                name: "IX_Chat_SecondUserId",
                table: "Chats",
                newName: "IX_Chats_SecondUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Chat_FirstUserId",
                table: "Chats",
                newName: "IX_Chats_FirstUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Chats",
                table: "Chats",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_Chats_ChatId",
                table: "ChatMessages",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_AspNetUsers_FirstUserId",
                table: "Chats",
                column: "FirstUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_AspNetUsers_SecondUserId",
                table: "Chats",
                column: "SecondUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_Chats_ChatId",
                table: "ChatMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_Chats_AspNetUsers_FirstUserId",
                table: "Chats");

            migrationBuilder.DropForeignKey(
                name: "FK_Chats_AspNetUsers_SecondUserId",
                table: "Chats");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Chats",
                table: "Chats");

            migrationBuilder.RenameTable(
                name: "Chats",
                newName: "Chat");

            migrationBuilder.RenameIndex(
                name: "IX_Chats_SecondUserId",
                table: "Chat",
                newName: "IX_Chat_SecondUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Chats_FirstUserId",
                table: "Chat",
                newName: "IX_Chat_FirstUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Chat",
                table: "Chat",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Chat_AspNetUsers_FirstUserId",
                table: "Chat",
                column: "FirstUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Chat_AspNetUsers_SecondUserId",
                table: "Chat",
                column: "SecondUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_Chat_ChatId",
                table: "ChatMessages",
                column: "ChatId",
                principalTable: "Chat",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
