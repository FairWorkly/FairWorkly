using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FairWorkly.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeInvitationTokenIndexUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_users_invitation_token",
                table: "users");

            migrationBuilder.CreateIndex(
                name: "ix_users_invitation_token",
                table: "users",
                column: "invitation_token",
                unique: true,
                filter: "invitation_token IS NOT NULL AND is_deleted = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_users_invitation_token",
                table: "users");

            migrationBuilder.CreateIndex(
                name: "ix_users_invitation_token",
                table: "users",
                column: "invitation_token",
                filter: "invitation_token IS NOT NULL");
        }
    }
}
