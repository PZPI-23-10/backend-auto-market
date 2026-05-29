using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MessagesReadFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE "ChatMessages"
                DROP COLUMN IF EXISTS "IsRead";
                """);

            migrationBuilder.Sql("""
                CREATE TABLE IF NOT EXISTS "ChatMessageReads" (
                    "MessageId" integer NOT NULL,
                    "UserId" integer NOT NULL,
                    "ReadAt" timestamp with time zone NOT NULL,
                    CONSTRAINT "PK_ChatMessageReads" PRIMARY KEY ("MessageId", "UserId"),
                    CONSTRAINT "FK_ChatMessageReads_ChatMessages_MessageId" FOREIGN KEY ("MessageId") REFERENCES "ChatMessages" ("Id") ON DELETE CASCADE
                );
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DROP TABLE IF EXISTS "ChatMessageReads";
                """);

            migrationBuilder.Sql("""
                ALTER TABLE "ChatMessages"
                ADD COLUMN IF NOT EXISTS "IsRead" boolean NOT NULL DEFAULT FALSE;
                """);
        }
    }
}
