using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FairWorkly.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixCredentialConstraintForSoftDeletedUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // AllowDeletedUsersWithoutCredentials was supposed to add is_deleted = TRUE to this
            // constraint, but it may not have been applied cleanly on all environments.
            // Idempotent re-apply: drop and recreate with the correct exemption so that
            // soft-deleted invited users (no password, no google_id, no token) are never blocked.
            migrationBuilder.Sql(
                "ALTER TABLE users DROP CONSTRAINT IF EXISTS chk_user_has_auth_credential;"
            );

            migrationBuilder.Sql(
                @"ALTER TABLE users ADD CONSTRAINT chk_user_has_auth_credential
                    CHECK (
                        is_deleted = TRUE
                        OR (password_hash IS NOT NULL AND LENGTH(TRIM(password_hash)) > 0)
                        OR (google_id IS NOT NULL AND LENGTH(TRIM(google_id)) > 0)
                        OR (invitation_token IS NOT NULL AND LENGTH(TRIM(invitation_token)) > 0)
                    );"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Restore the state from AllowDeletedUsersWithoutCredentials, which already included
            // is_deleted = TRUE.  This Down() is therefore a no-op from the constraint perspective:
            // rolling back this migration leaves the exemption intact (it was added by the prior
            // migration and must stay to avoid violating the constraint with existing soft-deleted
            // users).
        }
    }
}
