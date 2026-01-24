using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FairWorkly.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRosterEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_document_organization_organization_id",
                table: "document");

            migrationBuilder.DropForeignKey(
                name: "fk_document_user_created_by_user_id",
                table: "document");

            migrationBuilder.DropForeignKey(
                name: "fk_document_user_updated_by_user_id",
                table: "document");

            migrationBuilder.DropForeignKey(
                name: "fk_employees_organization_organization_id",
                table: "employees");

            migrationBuilder.DropForeignKey(
                name: "fk_employees_user_created_by_user_id",
                table: "employees");

            migrationBuilder.DropForeignKey(
                name: "fk_employees_user_updated_by_user_id",
                table: "employees");

            migrationBuilder.DropForeignKey(
                name: "fk_organization_user_created_by_user_id",
                table: "organization");

            migrationBuilder.DropForeignKey(
                name: "fk_organization_user_updated_by_user_id",
                table: "organization");

            migrationBuilder.DropForeignKey(
                name: "fk_organization_award_organization_organization_id",
                table: "organization_award");

            migrationBuilder.DropForeignKey(
                name: "fk_payroll_issues_organization_organization_id",
                table: "payroll_issues");

            migrationBuilder.DropForeignKey(
                name: "fk_payroll_issues_user_resolved_by_user_id",
                table: "payroll_issues");

            migrationBuilder.DropForeignKey(
                name: "fk_payroll_validations_organization_organization_id",
                table: "payroll_validations");

            migrationBuilder.DropForeignKey(
                name: "fk_payroll_validations_user_created_by_user_id",
                table: "payroll_validations");

            migrationBuilder.DropForeignKey(
                name: "fk_payroll_validations_user_updated_by_user_id",
                table: "payroll_validations");

            migrationBuilder.DropForeignKey(
                name: "fk_payslips_organization_organization_id",
                table: "payslips");

            migrationBuilder.DropForeignKey(
                name: "fk_payslips_user_created_by_user_id",
                table: "payslips");

            migrationBuilder.DropForeignKey(
                name: "fk_payslips_user_updated_by_user_id",
                table: "payslips");

            migrationBuilder.DropForeignKey(
                name: "fk_roster_organization_organization_id",
                table: "roster");

            migrationBuilder.DropForeignKey(
                name: "fk_roster_user_created_by_user_id",
                table: "roster");

            migrationBuilder.DropForeignKey(
                name: "fk_roster_user_updated_by_user_id",
                table: "roster");

            migrationBuilder.DropForeignKey(
                name: "fk_roster_issue_employees_employee_id",
                table: "roster_issue");

            migrationBuilder.DropForeignKey(
                name: "fk_roster_issue_organization_organization_id",
                table: "roster_issue");

            migrationBuilder.DropForeignKey(
                name: "fk_roster_issue_roster_roster_id",
                table: "roster_issue");

            migrationBuilder.DropForeignKey(
                name: "fk_roster_issue_roster_validation_roster_validation_id",
                table: "roster_issue");

            migrationBuilder.DropForeignKey(
                name: "fk_roster_issue_shift_shift_id",
                table: "roster_issue");

            migrationBuilder.DropForeignKey(
                name: "fk_roster_issue_user_resolved_by_user_id",
                table: "roster_issue");

            migrationBuilder.DropForeignKey(
                name: "fk_roster_issue_user_waived_by_user_id",
                table: "roster_issue");

            migrationBuilder.DropForeignKey(
                name: "fk_roster_validation_organization_organization_id",
                table: "roster_validation");

            migrationBuilder.DropForeignKey(
                name: "fk_roster_validation_roster_roster_id",
                table: "roster_validation");

            migrationBuilder.DropForeignKey(
                name: "fk_roster_validation_user_created_by_user_id",
                table: "roster_validation");

            migrationBuilder.DropForeignKey(
                name: "fk_roster_validation_user_updated_by_user_id",
                table: "roster_validation");

            migrationBuilder.DropForeignKey(
                name: "fk_shift_employees_employee_id",
                table: "shift");

            migrationBuilder.DropForeignKey(
                name: "fk_shift_organization_organization_id",
                table: "shift");

            migrationBuilder.DropForeignKey(
                name: "fk_shift_roster_roster_id",
                table: "shift");

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
                name: "pk_shift",
                table: "shift");

            migrationBuilder.DropPrimaryKey(
                name: "pk_roster_validation",
                table: "roster_validation");

            migrationBuilder.DropPrimaryKey(
                name: "pk_roster_issue",
                table: "roster_issue");

            migrationBuilder.DropPrimaryKey(
                name: "pk_roster",
                table: "roster");

            migrationBuilder.RenameTable(
                name: "shift",
                newName: "shifts");

            migrationBuilder.RenameTable(
                name: "roster_validation",
                newName: "roster_validations");

            migrationBuilder.RenameTable(
                name: "roster_issue",
                newName: "roster_issues");

            migrationBuilder.RenameTable(
                name: "roster",
                newName: "rosters");

            migrationBuilder.RenameIndex(
                name: "ix_shift_roster_id_employee_id_date",
                table: "shifts",
                newName: "ix_shifts_roster_id_employee_id_date");

            migrationBuilder.RenameIndex(
                name: "ix_shift_organization_id_date",
                table: "shifts",
                newName: "ix_shifts_organization_id_date");

            migrationBuilder.RenameIndex(
                name: "ix_shift_employee_id_date",
                table: "shifts",
                newName: "ix_shifts_employee_id_date");

            migrationBuilder.RenameIndex(
                name: "ix_roster_validation_updated_by_user_id",
                table: "roster_validations",
                newName: "ix_roster_validations_updated_by_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_roster_validation_roster_id",
                table: "roster_validations",
                newName: "ix_roster_validations_roster_id");

            migrationBuilder.RenameIndex(
                name: "ix_roster_validation_organization_id_status",
                table: "roster_validations",
                newName: "ix_roster_validations_organization_id_status");

            migrationBuilder.RenameIndex(
                name: "ix_roster_validation_created_by_user_id",
                table: "roster_validations",
                newName: "ix_roster_validations_created_by_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_roster_issue_waived_by_user_id",
                table: "roster_issues",
                newName: "ix_roster_issues_waived_by_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_roster_issue_shift_id",
                table: "roster_issues",
                newName: "ix_roster_issues_shift_id");

            migrationBuilder.RenameIndex(
                name: "ix_roster_issue_roster_validation_id_severity",
                table: "roster_issues",
                newName: "ix_roster_issues_roster_validation_id_severity");

            migrationBuilder.RenameIndex(
                name: "ix_roster_issue_roster_id_employee_id",
                table: "roster_issues",
                newName: "ix_roster_issues_roster_id_employee_id");

            migrationBuilder.RenameIndex(
                name: "ix_roster_issue_resolved_by_user_id",
                table: "roster_issues",
                newName: "ix_roster_issues_resolved_by_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_roster_issue_organization_id",
                table: "roster_issues",
                newName: "ix_roster_issues_organization_id");

            migrationBuilder.RenameIndex(
                name: "ix_roster_issue_employee_id",
                table: "roster_issues",
                newName: "ix_roster_issues_employee_id");

            migrationBuilder.RenameIndex(
                name: "ix_roster_issue_check_type",
                table: "roster_issues",
                newName: "ix_roster_issues_check_type");

            migrationBuilder.RenameIndex(
                name: "ix_roster_updated_by_user_id",
                table: "rosters",
                newName: "ix_rosters_updated_by_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_roster_organization_id_year_week_number",
                table: "rosters",
                newName: "ix_rosters_organization_id_year_week_number");

            migrationBuilder.RenameIndex(
                name: "ix_roster_organization_id_week_start_date",
                table: "rosters",
                newName: "ix_rosters_organization_id_week_start_date");

            migrationBuilder.RenameIndex(
                name: "ix_roster_created_by_user_id",
                table: "rosters",
                newName: "ix_rosters_created_by_user_id");

            migrationBuilder.AddColumn<Guid>(
                name: "employee_id1",
                table: "shifts",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "pk_shifts",
                table: "shifts",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_roster_validations",
                table: "roster_validations",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_roster_issues",
                table: "roster_issues",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_rosters",
                table: "rosters",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_user_email",
                table: "user",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_shifts_employee_id1",
                table: "shifts",
                column: "employee_id1");

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
                name: "fk_employees_organizations_organization_id",
                table: "employees",
                column: "organization_id",
                principalTable: "organization",
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
                name: "fk_payroll_issues_organizations_organization_id",
                table: "payroll_issues",
                column: "organization_id",
                principalTable: "organization",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_payroll_issues_users_resolved_by_user_id",
                table: "payroll_issues",
                column: "resolved_by_user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_payroll_validations_organizations_organization_id",
                table: "payroll_validations",
                column: "organization_id",
                principalTable: "organization",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_payroll_validations_users_created_by_user_id",
                table: "payroll_validations",
                column: "created_by_user_id",
                principalTable: "user",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_payroll_validations_users_updated_by_user_id",
                table: "payroll_validations",
                column: "updated_by_user_id",
                principalTable: "user",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_payslips_organizations_organization_id",
                table: "payslips",
                column: "organization_id",
                principalTable: "organization",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_payslips_users_created_by_user_id",
                table: "payslips",
                column: "created_by_user_id",
                principalTable: "user",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_payslips_users_updated_by_user_id",
                table: "payslips",
                column: "updated_by_user_id",
                principalTable: "user",
                principalColumn: "id");

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
                name: "fk_roster_issues_roster_validations_roster_validation_id",
                table: "roster_issues",
                column: "roster_validation_id",
                principalTable: "roster_validations",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_roster_issues_rosters_roster_id",
                table: "roster_issues",
                column: "roster_id",
                principalTable: "rosters",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_roster_issues_shifts_shift_id",
                table: "roster_issues",
                column: "shift_id",
                principalTable: "shifts",
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
                name: "fk_roster_validations_organizations_organization_id",
                table: "roster_validations",
                column: "organization_id",
                principalTable: "organization",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_roster_validations_rosters_roster_id",
                table: "roster_validations",
                column: "roster_id",
                principalTable: "rosters",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

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
                name: "fk_rosters_organizations_organization_id",
                table: "rosters",
                column: "organization_id",
                principalTable: "organization",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_rosters_users_created_by_user_id",
                table: "rosters",
                column: "created_by_user_id",
                principalTable: "user",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_rosters_users_updated_by_user_id",
                table: "rosters",
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
                name: "fk_shifts_employees_employee_id1",
                table: "shifts",
                column: "employee_id1",
                principalTable: "employees",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_shifts_organizations_organization_id",
                table: "shifts",
                column: "organization_id",
                principalTable: "organization",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_shifts_rosters_roster_id",
                table: "shifts",
                column: "roster_id",
                principalTable: "rosters",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_user_employees_employee_id",
                table: "user",
                column: "employee_id",
                principalTable: "employees",
                principalColumn: "id");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                name: "fk_employees_organizations_organization_id",
                table: "employees");

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
                name: "fk_payroll_issues_organizations_organization_id",
                table: "payroll_issues");

            migrationBuilder.DropForeignKey(
                name: "fk_payroll_issues_users_resolved_by_user_id",
                table: "payroll_issues");

            migrationBuilder.DropForeignKey(
                name: "fk_payroll_validations_organizations_organization_id",
                table: "payroll_validations");

            migrationBuilder.DropForeignKey(
                name: "fk_payroll_validations_users_created_by_user_id",
                table: "payroll_validations");

            migrationBuilder.DropForeignKey(
                name: "fk_payroll_validations_users_updated_by_user_id",
                table: "payroll_validations");

            migrationBuilder.DropForeignKey(
                name: "fk_payslips_organizations_organization_id",
                table: "payslips");

            migrationBuilder.DropForeignKey(
                name: "fk_payslips_users_created_by_user_id",
                table: "payslips");

            migrationBuilder.DropForeignKey(
                name: "fk_payslips_users_updated_by_user_id",
                table: "payslips");

            migrationBuilder.DropForeignKey(
                name: "fk_roster_issues_employees_employee_id",
                table: "roster_issues");

            migrationBuilder.DropForeignKey(
                name: "fk_roster_issues_organizations_organization_id",
                table: "roster_issues");

            migrationBuilder.DropForeignKey(
                name: "fk_roster_issues_roster_validations_roster_validation_id",
                table: "roster_issues");

            migrationBuilder.DropForeignKey(
                name: "fk_roster_issues_rosters_roster_id",
                table: "roster_issues");

            migrationBuilder.DropForeignKey(
                name: "fk_roster_issues_shifts_shift_id",
                table: "roster_issues");

            migrationBuilder.DropForeignKey(
                name: "fk_roster_issues_users_resolved_by_user_id",
                table: "roster_issues");

            migrationBuilder.DropForeignKey(
                name: "fk_roster_issues_users_waived_by_user_id",
                table: "roster_issues");

            migrationBuilder.DropForeignKey(
                name: "fk_roster_validations_organizations_organization_id",
                table: "roster_validations");

            migrationBuilder.DropForeignKey(
                name: "fk_roster_validations_rosters_roster_id",
                table: "roster_validations");

            migrationBuilder.DropForeignKey(
                name: "fk_roster_validations_users_created_by_user_id",
                table: "roster_validations");

            migrationBuilder.DropForeignKey(
                name: "fk_roster_validations_users_updated_by_user_id",
                table: "roster_validations");

            migrationBuilder.DropForeignKey(
                name: "fk_rosters_organizations_organization_id",
                table: "rosters");

            migrationBuilder.DropForeignKey(
                name: "fk_rosters_users_created_by_user_id",
                table: "rosters");

            migrationBuilder.DropForeignKey(
                name: "fk_rosters_users_updated_by_user_id",
                table: "rosters");

            migrationBuilder.DropForeignKey(
                name: "fk_shifts_employees_employee_id",
                table: "shifts");

            migrationBuilder.DropForeignKey(
                name: "fk_shifts_employees_employee_id1",
                table: "shifts");

            migrationBuilder.DropForeignKey(
                name: "fk_shifts_organizations_organization_id",
                table: "shifts");

            migrationBuilder.DropForeignKey(
                name: "fk_shifts_rosters_roster_id",
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

            migrationBuilder.DropIndex(
                name: "ix_user_email",
                table: "user");

            migrationBuilder.DropPrimaryKey(
                name: "pk_shifts",
                table: "shifts");

            migrationBuilder.DropIndex(
                name: "ix_shifts_employee_id1",
                table: "shifts");

            migrationBuilder.DropPrimaryKey(
                name: "pk_rosters",
                table: "rosters");

            migrationBuilder.DropPrimaryKey(
                name: "pk_roster_validations",
                table: "roster_validations");

            migrationBuilder.DropPrimaryKey(
                name: "pk_roster_issues",
                table: "roster_issues");

            migrationBuilder.DropColumn(
                name: "employee_id1",
                table: "shifts");

            migrationBuilder.RenameTable(
                name: "shifts",
                newName: "shift");

            migrationBuilder.RenameTable(
                name: "rosters",
                newName: "roster");

            migrationBuilder.RenameTable(
                name: "roster_validations",
                newName: "roster_validation");

            migrationBuilder.RenameTable(
                name: "roster_issues",
                newName: "roster_issue");

            migrationBuilder.RenameIndex(
                name: "ix_shifts_roster_id_employee_id_date",
                table: "shift",
                newName: "ix_shift_roster_id_employee_id_date");

            migrationBuilder.RenameIndex(
                name: "ix_shifts_organization_id_date",
                table: "shift",
                newName: "ix_shift_organization_id_date");

            migrationBuilder.RenameIndex(
                name: "ix_shifts_employee_id_date",
                table: "shift",
                newName: "ix_shift_employee_id_date");

            migrationBuilder.RenameIndex(
                name: "ix_rosters_updated_by_user_id",
                table: "roster",
                newName: "ix_roster_updated_by_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_rosters_organization_id_year_week_number",
                table: "roster",
                newName: "ix_roster_organization_id_year_week_number");

            migrationBuilder.RenameIndex(
                name: "ix_rosters_organization_id_week_start_date",
                table: "roster",
                newName: "ix_roster_organization_id_week_start_date");

            migrationBuilder.RenameIndex(
                name: "ix_rosters_created_by_user_id",
                table: "roster",
                newName: "ix_roster_created_by_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_roster_validations_updated_by_user_id",
                table: "roster_validation",
                newName: "ix_roster_validation_updated_by_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_roster_validations_roster_id",
                table: "roster_validation",
                newName: "ix_roster_validation_roster_id");

            migrationBuilder.RenameIndex(
                name: "ix_roster_validations_organization_id_status",
                table: "roster_validation",
                newName: "ix_roster_validation_organization_id_status");

            migrationBuilder.RenameIndex(
                name: "ix_roster_validations_created_by_user_id",
                table: "roster_validation",
                newName: "ix_roster_validation_created_by_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_roster_issues_waived_by_user_id",
                table: "roster_issue",
                newName: "ix_roster_issue_waived_by_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_roster_issues_shift_id",
                table: "roster_issue",
                newName: "ix_roster_issue_shift_id");

            migrationBuilder.RenameIndex(
                name: "ix_roster_issues_roster_validation_id_severity",
                table: "roster_issue",
                newName: "ix_roster_issue_roster_validation_id_severity");

            migrationBuilder.RenameIndex(
                name: "ix_roster_issues_roster_id_employee_id",
                table: "roster_issue",
                newName: "ix_roster_issue_roster_id_employee_id");

            migrationBuilder.RenameIndex(
                name: "ix_roster_issues_resolved_by_user_id",
                table: "roster_issue",
                newName: "ix_roster_issue_resolved_by_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_roster_issues_organization_id",
                table: "roster_issue",
                newName: "ix_roster_issue_organization_id");

            migrationBuilder.RenameIndex(
                name: "ix_roster_issues_employee_id",
                table: "roster_issue",
                newName: "ix_roster_issue_employee_id");

            migrationBuilder.RenameIndex(
                name: "ix_roster_issues_check_type",
                table: "roster_issue",
                newName: "ix_roster_issue_check_type");

            migrationBuilder.AddPrimaryKey(
                name: "pk_shift",
                table: "shift",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_roster",
                table: "roster",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_roster_validation",
                table: "roster_validation",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_roster_issue",
                table: "roster_issue",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_document_organization_organization_id",
                table: "document",
                column: "organization_id",
                principalTable: "organization",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_document_user_created_by_user_id",
                table: "document",
                column: "created_by_user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_document_user_updated_by_user_id",
                table: "document",
                column: "updated_by_user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_employees_organization_organization_id",
                table: "employees",
                column: "organization_id",
                principalTable: "organization",
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
                name: "fk_organization_user_created_by_user_id",
                table: "organization",
                column: "created_by_user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_organization_user_updated_by_user_id",
                table: "organization",
                column: "updated_by_user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_organization_award_organization_organization_id",
                table: "organization_award",
                column: "organization_id",
                principalTable: "organization",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_payroll_issues_organization_organization_id",
                table: "payroll_issues",
                column: "organization_id",
                principalTable: "organization",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_payroll_issues_user_resolved_by_user_id",
                table: "payroll_issues",
                column: "resolved_by_user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_payroll_validations_organization_organization_id",
                table: "payroll_validations",
                column: "organization_id",
                principalTable: "organization",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_payroll_validations_user_created_by_user_id",
                table: "payroll_validations",
                column: "created_by_user_id",
                principalTable: "user",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_payroll_validations_user_updated_by_user_id",
                table: "payroll_validations",
                column: "updated_by_user_id",
                principalTable: "user",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_payslips_organization_organization_id",
                table: "payslips",
                column: "organization_id",
                principalTable: "organization",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_payslips_user_created_by_user_id",
                table: "payslips",
                column: "created_by_user_id",
                principalTable: "user",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_payslips_user_updated_by_user_id",
                table: "payslips",
                column: "updated_by_user_id",
                principalTable: "user",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_roster_organization_organization_id",
                table: "roster",
                column: "organization_id",
                principalTable: "organization",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_roster_user_created_by_user_id",
                table: "roster",
                column: "created_by_user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_roster_user_updated_by_user_id",
                table: "roster",
                column: "updated_by_user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_roster_issue_employees_employee_id",
                table: "roster_issue",
                column: "employee_id",
                principalTable: "employees",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_roster_issue_organization_organization_id",
                table: "roster_issue",
                column: "organization_id",
                principalTable: "organization",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_roster_issue_roster_roster_id",
                table: "roster_issue",
                column: "roster_id",
                principalTable: "roster",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_roster_issue_roster_validation_roster_validation_id",
                table: "roster_issue",
                column: "roster_validation_id",
                principalTable: "roster_validation",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_roster_issue_shift_shift_id",
                table: "roster_issue",
                column: "shift_id",
                principalTable: "shift",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_roster_issue_user_resolved_by_user_id",
                table: "roster_issue",
                column: "resolved_by_user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_roster_issue_user_waived_by_user_id",
                table: "roster_issue",
                column: "waived_by_user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_roster_validation_organization_organization_id",
                table: "roster_validation",
                column: "organization_id",
                principalTable: "organization",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_roster_validation_roster_roster_id",
                table: "roster_validation",
                column: "roster_id",
                principalTable: "roster",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_roster_validation_user_created_by_user_id",
                table: "roster_validation",
                column: "created_by_user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_roster_validation_user_updated_by_user_id",
                table: "roster_validation",
                column: "updated_by_user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_shift_employees_employee_id",
                table: "shift",
                column: "employee_id",
                principalTable: "employees",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_shift_organization_organization_id",
                table: "shift",
                column: "organization_id",
                principalTable: "organization",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_shift_roster_roster_id",
                table: "shift",
                column: "roster_id",
                principalTable: "roster",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

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
    }
}
