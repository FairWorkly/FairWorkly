using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FairWorkly.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AllowDeletedUsersWithoutCredentials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_users_invitation_token",
                table: "users",
                column: "invitation_token",
                filter: "invitation_token IS NOT NULL");

            // Update credential check: soft-deleted users are exempt
            migrationBuilder.Sql(
                "ALTER TABLE users DROP CONSTRAINT IF EXISTS chk_user_has_auth_credential;"
            );

            migrationBuilder.Sql(@"
                ALTER TABLE users ADD CONSTRAINT chk_user_has_auth_credential
                    CHECK (
                        is_deleted = TRUE
                        OR (password_hash IS NOT NULL AND LENGTH(TRIM(password_hash)) > 0)
                        OR (google_id IS NOT NULL AND LENGTH(TRIM(google_id)) > 0)
                        OR (invitation_token IS NOT NULL AND LENGTH(TRIM(invitation_token)) > 0)
                    );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Restore previous constraint (without is_deleted exemption)
            migrationBuilder.Sql(
                "ALTER TABLE users DROP CONSTRAINT IF EXISTS chk_user_has_auth_credential;"
            );

            migrationBuilder.Sql(@"
                ALTER TABLE users ADD CONSTRAINT chk_user_has_auth_credential
                    CHECK (
                        (password_hash IS NOT NULL AND LENGTH(TRIM(password_hash)) > 0)
                        OR (google_id IS NOT NULL AND LENGTH(TRIM(google_id)) > 0)
                        OR (invitation_token IS NOT NULL AND LENGTH(TRIM(invitation_token)) > 0)
                    );
            ");

            migrationBuilder.DropIndex(
                name: "ix_users_invitation_token",
                table: "users");
        }
    }
}
