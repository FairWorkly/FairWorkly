using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FairWorkly.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NormalizeTableNamesAndConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_award_level_award_award_id",
                table: "award_level");

            migrationBuilder.DropForeignKey(
                name: "fk_document_employees_employee_id",
                table: "document");

            migrationBuilder.DropForeignKey(
                name: "fk_document_organizations_organization_id",
                table: "document");

            migrationBuilder.DropForeignKey(
                name: "fk_document_users_created_by_user_id",
                table: "document");

            migrationBuilder.DropForeignKey(
                name: "fk_document_users_updated_by_user_id",
                table: "document");

            migrationBuilder.DropForeignKey(
                name: "fk_employees_users_created_by_user_id",
                table: "employees");

            migrationBuilder.DropForeignKey(
                name: "fk_employees_users_updated_by_user_id",
                table: "employees");

            migrationBuilder.DropForeignKey(
                name: "fk_organization_user_created_by_user_id",
                table: "organization");

            migrationBuilder.DropForeignKey(
                name: "fk_organization_user_updated_by_user_id",
                table: "organization");

            migrationBuilder.DropForeignKey(
                name: "fk_organization_award_organizations_organization_id",
                table: "organization_award");

            migrationBuilder.DropForeignKey(
                name: "fk_roster_issues_employees_employee_id",
                table: "roster_issues");

            migrationBuilder.DropForeignKey(
                name: "fk_roster_issues_organizations_organization_id",
                table: "roster_issues");

            migrationBuilder.DropForeignKey(
                name: "fk_roster_issues_users_resolved_by_user_id",
                table: "roster_issues");

            migrationBuilder.DropForeignKey(
                name: "fk_roster_issues_users_waived_by_user_id",
                table: "roster_issues");

            migrationBuilder.DropForeignKey(
                name: "fk_roster_validations_users_created_by_user_id",
                table: "roster_validations");

            migrationBuilder.DropForeignKey(
                name: "fk_roster_validations_users_updated_by_user_id",
                table: "roster_validations");

            migrationBuilder.DropForeignKey(
                name: "fk_shifts_employees_employee_id",
                table: "shifts");

            migrationBuilder.DropForeignKey(
                name: "fk_shifts_organizations_organization_id",
                table: "shifts");

            migrationBuilder.DropForeignKey(
                name: "fk_user_employees_employee_id",
                table: "user");

            migrationBuilder.DropForeignKey(
                name: "fk_user_organizations_organization_id",
                table: "user");

            migrationBuilder.DropForeignKey(
                name: "fk_user_user_created_by_user_id",
                table: "user");

            migrationBuilder.DropForeignKey(
                name: "fk_user_user_updated_by_user_id",
                table: "user");

            migrationBuilder.DropPrimaryKey(
                name: "pk_organization_award",
                table: "organization_award");

            migrationBuilder.DropPrimaryKey(
                name: "pk_document",
                table: "document");

            migrationBuilder.DropPrimaryKey(
                name: "pk_award_level",
                table: "award_level");

            migrationBuilder.DropPrimaryKey(
                name: "pk_award",
                table: "award");

            migrationBuilder.RenameTable(
                name: "organization_award",
                newName: "organization_awards");

            migrationBuilder.RenameTable(
                name: "document",
                newName: "documents");

            migrationBuilder.RenameTable(
                name: "award_level",
                newName: "award_levels");

            migrationBuilder.RenameTable(
                name: "award",
                newName: "awards");

            migrationBuilder.RenameIndex(
                name: "ix_organization_award_organization_id_is_primary",
                table: "organization_awards",
                newName: "ix_organization_awards_organization_id_is_primary");

            migrationBuilder.RenameIndex(
                name: "ix_organization_award_organization_id_award_type",
                table: "organization_awards",
                newName: "ix_organization_awards_organization_id_award_type");

            migrationBuilder.RenameIndex(
                name: "ix_document_updated_by_user_id",
                table: "documents",
                newName: "ix_documents_updated_by_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_document_organization_id_document_type",
                table: "documents",
                newName: "ix_documents_organization_id_document_type");

            migrationBuilder.RenameIndex(
                name: "ix_document_is_provided",
                table: "documents",
                newName: "ix_documents_is_provided");

            migrationBuilder.RenameIndex(
                name: "ix_document_employee_id_document_type",
                table: "documents",
                newName: "ix_documents_employee_id_document_type");

            migrationBuilder.RenameIndex(
                name: "ix_document_created_by_user_id",
                table: "documents",
                newName: "ix_documents_created_by_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_award_level_award_id_level_number_is_active",
                table: "award_levels",
                newName: "ix_award_levels_award_id_level_number_is_active");

            migrationBuilder.RenameIndex(
                name: "ix_award_level_award_id_level_number_effective_from",
                table: "award_levels",
                newName: "ix_award_levels_award_id_level_number_effective_from");

            migrationBuilder.RenameIndex(
                name: "ix_award_award_type",
                table: "awards",
                newName: "ix_awards_award_type");

            migrationBuilder.RenameIndex(
                name: "ix_award_award_code",
                table: "awards",
                newName: "ix_awards_award_code");

            migrationBuilder.AddPrimaryKey(
                name: "pk_organization_awards",
                table: "organization_awards",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_documents",
                table: "documents",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_award_levels",
                table: "award_levels",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_awards",
                table: "awards",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_award_levels_awards_award_id",
                table: "award_levels",
                column: "award_id",
                principalTable: "awards",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_documents_employees_employee_id",
                table: "documents",
                column: "employee_id",
                principalTable: "employees",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_documents_organizations_organization_id",
                table: "documents",
                column: "organization_id",
                principalTable: "organization",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_documents_users_created_by_user_id",
                table: "documents",
                column: "created_by_user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_documents_users_updated_by_user_id",
                table: "documents",
                column: "updated_by_user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_employees_user_created_by_user_id",
                table: "employees",
                column: "created_by_user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_employees_user_updated_by_user_id",
                table: "employees",
                column: "updated_by_user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_organization_users_created_by_user_id",
                table: "organization",
                column: "created_by_user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_organization_users_updated_by_user_id",
                table: "organization",
                column: "updated_by_user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_organization_awards_organizations_organization_id",
                table: "organization_awards",
                column: "organization_id",
                principalTable: "organization",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_roster_issues_employees_employee_id",
                table: "roster_issues",
                column: "employee_id",
                principalTable: "employees",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_roster_issues_organizations_organization_id",
                table: "roster_issues",
                column: "organization_id",
                principalTable: "organization",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_roster_issues_users_resolved_by_user_id",
                table: "roster_issues",
                column: "resolved_by_user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_roster_issues_users_waived_by_user_id",
                table: "roster_issues",
                column: "waived_by_user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_roster_validations_users_created_by_user_id",
                table: "roster_validations",
                column: "created_by_user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_roster_validations_users_updated_by_user_id",
                table: "roster_validations",
                column: "updated_by_user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_shifts_employees_employee_id",
                table: "shifts",
                column: "employee_id",
                principalTable: "employees",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_shifts_organizations_organization_id",
                table: "shifts",
                column: "organization_id",
                principalTable: "organization",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_user_employees_employee_id",
                table: "user",
                column: "employee_id",
                principalTable: "employees",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_user_organization_organization_id",
                table: "user",
                column: "organization_id",
                principalTable: "organization",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_user_user_created_by_user_id",
                table: "user",
                column: "created_by_user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_user_user_updated_by_user_id",
                table: "user",
                column: "updated_by_user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_award_levels_awards_award_id",
                table: "award_levels");

            migrationBuilder.DropForeignKey(
                name: "fk_documents_employees_employee_id",
                table: "documents");

            migrationBuilder.DropForeignKey(
                name: "fk_documents_organizations_organization_id",
                table: "documents");

            migrationBuilder.DropForeignKey(
                name: "fk_documents_users_created_by_user_id",
                table: "documents");

            migrationBuilder.DropForeignKey(
                name: "fk_documents_users_updated_by_user_id",
                table: "documents");

            migrationBuilder.DropForeignKey(
                name: "fk_employees_user_created_by_user_id",
                table: "employees");

            migrationBuilder.DropForeignKey(
                name: "fk_employees_user_updated_by_user_id",
                table: "employees");

            migrationBuilder.DropForeignKey(
                name: "fk_organization_users_created_by_user_id",
                table: "organization");

            migrationBuilder.DropForeignKey(
                name: "fk_organization_users_updated_by_user_id",
                table: "organization");

            migrationBuilder.DropForeignKey(
                name: "fk_organization_awards_organizations_organization_id",
                table: "organization_awards");

            migrationBuilder.DropForeignKey(
                name: "fk_roster_issues_employees_employee_id",
                table: "roster_issues");

            migrationBuilder.DropForeignKey(
                name: "fk_roster_issues_organizations_organization_id",
                table: "roster_issues");

            migrationBuilder.DropForeignKey(
                name: "fk_roster_issues_users_resolved_by_user_id",
                table: "roster_issues");

            migrationBuilder.DropForeignKey(
                name: "fk_roster_issues_users_waived_by_user_id",
                table: "roster_issues");

            migrationBuilder.DropForeignKey(
                name: "fk_roster_validations_users_created_by_user_id",
                table: "roster_validations");

            migrationBuilder.DropForeignKey(
                name: "fk_roster_validations_users_updated_by_user_id",
                table: "roster_validations");

            migrationBuilder.DropForeignKey(
                name: "fk_shifts_employees_employee_id",
                table: "shifts");

            migrationBuilder.DropForeignKey(
                name: "fk_shifts_organizations_organization_id",
                table: "shifts");

            migrationBuilder.DropForeignKey(
                name: "fk_user_employees_employee_id",
                table: "user");

            migrationBuilder.DropForeignKey(
                name: "fk_user_organization_organization_id",
                table: "user");

            migrationBuilder.DropForeignKey(
                name: "fk_user_user_created_by_user_id",
                table: "user");

            migrationBuilder.DropForeignKey(
                name: "fk_user_user_updated_by_user_id",
                table: "user");

            migrationBuilder.DropPrimaryKey(
                name: "pk_organization_awards",
                table: "organization_awards");

            migrationBuilder.DropPrimaryKey(
                name: "pk_documents",
                table: "documents");

            migrationBuilder.DropPrimaryKey(
                name: "pk_awards",
                table: "awards");

            migrationBuilder.DropPrimaryKey(
                name: "pk_award_levels",
                table: "award_levels");

            migrationBuilder.RenameTable(
                name: "organization_awards",
                newName: "organization_award");

            migrationBuilder.RenameTable(
                name: "documents",
                newName: "document");

            migrationBuilder.RenameTable(
                name: "awards",
                newName: "award");

            migrationBuilder.RenameTable(
                name: "award_levels",
                newName: "award_level");

            migrationBuilder.RenameIndex(
                name: "ix_organization_awards_organization_id_is_primary",
                table: "organization_award",
                newName: "ix_organization_award_organization_id_is_primary");

            migrationBuilder.RenameIndex(
                name: "ix_organization_awards_organization_id_award_type",
                table: "organization_award",
                newName: "ix_organization_award_organization_id_award_type");

            migrationBuilder.RenameIndex(
                name: "ix_documents_updated_by_user_id",
                table: "document",
                newName: "ix_document_updated_by_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_documents_organization_id_document_type",
                table: "document",
                newName: "ix_document_organization_id_document_type");

            migrationBuilder.RenameIndex(
                name: "ix_documents_is_provided",
                table: "document",
                newName: "ix_document_is_provided");

            migrationBuilder.RenameIndex(
                name: "ix_documents_employee_id_document_type",
                table: "document",
                newName: "ix_document_employee_id_document_type");

            migrationBuilder.RenameIndex(
                name: "ix_documents_created_by_user_id",
                table: "document",
                newName: "ix_document_created_by_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_awards_award_type",
                table: "award",
                newName: "ix_award_award_type");

            migrationBuilder.RenameIndex(
                name: "ix_awards_award_code",
                table: "award",
                newName: "ix_award_award_code");

            migrationBuilder.RenameIndex(
                name: "ix_award_levels_award_id_level_number_is_active",
                table: "award_level",
                newName: "ix_award_level_award_id_level_number_is_active");

            migrationBuilder.RenameIndex(
                name: "ix_award_levels_award_id_level_number_effective_from",
                table: "award_level",
                newName: "ix_award_level_award_id_level_number_effective_from");

            migrationBuilder.AddPrimaryKey(
                name: "pk_organization_award",
                table: "organization_award",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_document",
                table: "document",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_award",
                table: "award",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_award_level",
                table: "award_level",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_award_level_award_award_id",
                table: "award_level",
                column: "award_id",
                principalTable: "award",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_document_employees_employee_id",
                table: "document",
                column: "employee_id",
                principalTable: "employees",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_document_organizations_organization_id",
                table: "document",
                column: "organization_id",
                principalTable: "organization",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_document_users_created_by_user_id",
                table: "document",
                column: "created_by_user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_document_users_updated_by_user_id",
                table: "document",
                column: "updated_by_user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_employees_users_created_by_user_id",
                table: "employees",
                column: "created_by_user_id",
                principalTable: "user",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_employees_users_updated_by_user_id",
                table: "employees",
                column: "updated_by_user_id",
                principalTable: "user",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_organization_user_created_by_user_id",
                table: "organization",
                column: "created_by_user_id",
                principalTable: "user",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_organization_user_updated_by_user_id",
                table: "organization",
                column: "updated_by_user_id",
                principalTable: "user",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_organization_award_organizations_organization_id",
                table: "organization_award",
                column: "organization_id",
                principalTable: "organization",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_roster_issues_employees_employee_id",
                table: "roster_issues",
                column: "employee_id",
                principalTable: "employees",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_roster_issues_organizations_organization_id",
                table: "roster_issues",
                column: "organization_id",
                principalTable: "organization",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_roster_issues_users_resolved_by_user_id",
                table: "roster_issues",
                column: "resolved_by_user_id",
                principalTable: "user",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_roster_issues_users_waived_by_user_id",
                table: "roster_issues",
                column: "waived_by_user_id",
                principalTable: "user",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_roster_validations_users_created_by_user_id",
                table: "roster_validations",
                column: "created_by_user_id",
                principalTable: "user",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_roster_validations_users_updated_by_user_id",
                table: "roster_validations",
                column: "updated_by_user_id",
                principalTable: "user",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_shifts_employees_employee_id",
                table: "shifts",
                column: "employee_id",
                principalTable: "employees",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_shifts_organizations_organization_id",
                table: "shifts",
                column: "organization_id",
                principalTable: "organization",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_user_employees_employee_id",
                table: "user",
                column: "employee_id",
                principalTable: "employees",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_user_organizations_organization_id",
                table: "user",
                column: "organization_id",
                principalTable: "organization",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_user_user_created_by_user_id",
                table: "user",
                column: "created_by_user_id",
                principalTable: "user",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_user_user_updated_by_user_id",
                table: "user",
                column: "updated_by_user_id",
                principalTable: "user",
                principalColumn: "id");
        }
    }
}
