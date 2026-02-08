using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FairWorkly.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SoftDeleteAndIndexHardening : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop CHECK constraint before dropping column (added in previous migration)
            migrationBuilder.Sql(
                "ALTER TABLE organizations DROP CONSTRAINT IF EXISTS chk_organization_current_employee_count_non_negative;");

            migrationBuilder.DropForeignKey(
                name: "fk_documents_employees_employee_id",
                table: "documents");

            migrationBuilder.DropIndex(
                name: "ix_users_google_id",
                table: "users");

            migrationBuilder.DropIndex(
                name: "ix_users_organization_id_email",
                table: "users");

            migrationBuilder.DropIndex(
                name: "ix_roster_validations_roster_id",
                table: "roster_validations");

            migrationBuilder.DropIndex(
                name: "ix_organizations_abn",
                table: "organizations");

            migrationBuilder.DropIndex(
                name: "ix_organization_awards_organization_id_award_type",
                table: "organization_awards");

            migrationBuilder.DropIndex(
                name: "ix_employees_organization_id_email",
                table: "employees");

            migrationBuilder.DropIndex(
                name: "ix_employees_organization_id_employee_number",
                table: "employees");

            migrationBuilder.DropIndex(
                name: "ix_awards_award_code",
                table: "awards");

            migrationBuilder.DropIndex(
                name: "ix_awards_award_type",
                table: "awards");

            migrationBuilder.DropColumn(
                name: "current_employee_count",
                table: "organizations");

            migrationBuilder.CreateIndex(
                name: "ix_users_google_id",
                table: "users",
                column: "google_id",
                unique: true,
                filter: "google_id IS NOT NULL AND is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_users_organization_id_email",
                table: "users",
                columns: new[] { "organization_id", "email" },
                unique: true,
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_users_refresh_token",
                table: "users",
                column: "refresh_token",
                filter: "refresh_token IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_roster_validations_roster_id",
                table: "roster_validations",
                column: "roster_id",
                unique: true,
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_organizations_abn",
                table: "organizations",
                column: "abn",
                unique: true,
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_organization_awards_organization_id_award_type",
                table: "organization_awards",
                columns: new[] { "organization_id", "award_type" },
                unique: true,
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_employees_organization_id_email",
                table: "employees",
                columns: new[] { "organization_id", "email" },
                unique: true,
                filter: "email IS NOT NULL AND is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_employees_organization_id_employee_number",
                table: "employees",
                columns: new[] { "organization_id", "employee_number" },
                unique: true,
                filter: "employee_number IS NOT NULL AND is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_awards_award_code",
                table: "awards",
                column: "award_code",
                unique: true,
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_awards_award_type",
                table: "awards",
                column: "award_type",
                unique: true,
                filter: "is_deleted = false");

            migrationBuilder.AddForeignKey(
                name: "fk_documents_employees_employee_id",
                table: "documents",
                column: "employee_id",
                principalTable: "employees",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_documents_employees_employee_id",
                table: "documents");

            migrationBuilder.DropIndex(
                name: "ix_users_google_id",
                table: "users");

            migrationBuilder.DropIndex(
                name: "ix_users_organization_id_email",
                table: "users");

            migrationBuilder.DropIndex(
                name: "ix_users_refresh_token",
                table: "users");

            migrationBuilder.DropIndex(
                name: "ix_roster_validations_roster_id",
                table: "roster_validations");

            migrationBuilder.DropIndex(
                name: "ix_organizations_abn",
                table: "organizations");

            migrationBuilder.DropIndex(
                name: "ix_organization_awards_organization_id_award_type",
                table: "organization_awards");

            migrationBuilder.DropIndex(
                name: "ix_employees_organization_id_email",
                table: "employees");

            migrationBuilder.DropIndex(
                name: "ix_employees_organization_id_employee_number",
                table: "employees");

            migrationBuilder.DropIndex(
                name: "ix_awards_award_code",
                table: "awards");

            migrationBuilder.DropIndex(
                name: "ix_awards_award_type",
                table: "awards");

            migrationBuilder.AddColumn<int>(
                name: "current_employee_count",
                table: "organizations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(
                "ALTER TABLE organizations ADD CONSTRAINT chk_organization_current_employee_count_non_negative CHECK (current_employee_count >= 0);");

            migrationBuilder.CreateIndex(
                name: "ix_users_google_id",
                table: "users",
                column: "google_id",
                unique: true,
                filter: "google_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_users_organization_id_email",
                table: "users",
                columns: new[] { "organization_id", "email" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_roster_validations_roster_id",
                table: "roster_validations",
                column: "roster_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_organizations_abn",
                table: "organizations",
                column: "abn",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_organization_awards_organization_id_award_type",
                table: "organization_awards",
                columns: new[] { "organization_id", "award_type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_employees_organization_id_email",
                table: "employees",
                columns: new[] { "organization_id", "email" },
                unique: true,
                filter: "email IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_employees_organization_id_employee_number",
                table: "employees",
                columns: new[] { "organization_id", "employee_number" },
                unique: true,
                filter: "employee_number IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_awards_award_code",
                table: "awards",
                column: "award_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_awards_award_type",
                table: "awards",
                column: "award_type",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_documents_employees_employee_id",
                table: "documents",
                column: "employee_id",
                principalTable: "employees",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
