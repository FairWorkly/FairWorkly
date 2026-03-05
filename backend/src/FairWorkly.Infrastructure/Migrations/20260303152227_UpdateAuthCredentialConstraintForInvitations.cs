using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FairWorkly.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAuthCredentialConstraintForInvitations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the old constraint that only allowed password_hash or google_id.
            // Invited users have neither until they accept the invitation.
            migrationBuilder.Sql(
                "ALTER TABLE users DROP CONSTRAINT IF EXISTS chk_user_has_auth_credential;"
            );

            // Re-create with invitation_token as a third valid credential path.
            migrationBuilder.Sql(@"
                ALTER TABLE users ADD CONSTRAINT chk_user_has_auth_credential
                    CHECK (
                        (password_hash IS NOT NULL AND LENGTH(TRIM(password_hash)) > 0)
                        OR (google_id IS NOT NULL AND LENGTH(TRIM(google_id)) > 0)
                        OR (invitation_token IS NOT NULL AND LENGTH(TRIM(invitation_token)) > 0)
                    );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "ALTER TABLE users DROP CONSTRAINT IF EXISTS chk_user_has_auth_credential;"
            );

            // Restore original constraint (password or OAuth only)
            migrationBuilder.Sql(@"
                ALTER TABLE users ADD CONSTRAINT chk_user_has_auth_credential
                    CHECK (
                        (password_hash IS NOT NULL AND LENGTH(TRIM(password_hash)) > 0)
                        OR (google_id IS NOT NULL AND LENGTH(TRIM(google_id)) > 0)
                    );
            ");
        }
    }
}
