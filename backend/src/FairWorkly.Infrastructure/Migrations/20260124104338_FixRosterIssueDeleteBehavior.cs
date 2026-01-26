using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FairWorkly.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixRosterIssueDeleteBehavior : Migration
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
                name: "fk_payroll_issues_employees_employee_id",
                table: "payroll_issues");

            migrationBuilder.DropForeignKey(
                name: "fk_payroll_issues_organizations_organization_id",
                table: "payroll_issues");

            migrationBuilder.DropForeignKey(
                name: "fk_payroll_issues_payroll_validations_payroll_validation_id",
                table: "payroll_issues");

            migrationBuilder.DropForeignKey(
                name: "fk_payroll_issues_payslips_payslip_id",
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
                name: "fk_payslips_employees_employee_id",
                table: "payslips");

            migrationBuilder.DropForeignKey(
                name: "fk_payslips_organizations_organization_id",
                table: "payslips");

            migrationBuilder.DropForeignKey(
                name: "fk_payslips_payroll_validations_payroll_validation_id",
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

            migrationBuilder.DropPrimaryKey(
                name: "pk_user",
                table: "user");

            migrationBuilder.DropPrimaryKey(
                name: "pk_shifts",
                table: "shifts");

            migrationBuilder.DropPrimaryKey(
                name: "pk_rosters",
                table: "rosters");

            migrationBuilder.DropPrimaryKey(
                name: "pk_payslips",
                table: "payslips");

            migrationBuilder.DropPrimaryKey(
                name: "pk_organization",
                table: "organization");

            migrationBuilder.DropPrimaryKey(
                name: "pk_employees",
                table: "employees");

            migrationBuilder.DropPrimaryKey(
                name: "pk_roster_validations",
                table: "roster_validations");

            migrationBuilder.DropPrimaryKey(
                name: "pk_roster_issues",
                table: "roster_issues");

            migrationBuilder.DropPrimaryKey(
                name: "pk_payroll_validations",
                table: "payroll_validations");

            migrationBuilder.DropPrimaryKey(
                name: "pk_payroll_issues",
                table: "payroll_issues");

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
                name: "shifts",
                newName: "Shifts");

            migrationBuilder.RenameTable(
                name: "rosters",
                newName: "Rosters");

            migrationBuilder.RenameTable(
                name: "payslips",
                newName: "Payslips");

            migrationBuilder.RenameTable(
                name: "employees",
                newName: "Employees");

            migrationBuilder.RenameTable(
                name: "roster_validations",
                newName: "RosterValidations");

            migrationBuilder.RenameTable(
                name: "roster_issues",
                newName: "RosterIssues");

            migrationBuilder.RenameTable(
                name: "payroll_validations",
                newName: "PayrollValidations");

            migrationBuilder.RenameTable(
                name: "payroll_issues",
                newName: "PayrollIssues");

            migrationBuilder.RenameTable(
                name: "organization_award",
                newName: "OrganizationAwards");

            migrationBuilder.RenameTable(
                name: "document",
                newName: "Documents");

            migrationBuilder.RenameTable(
                name: "award_level",
                newName: "AwardLevels");

            migrationBuilder.RenameTable(
                name: "award",
                newName: "Awards");

            migrationBuilder.RenameColumn(
                name: "role",
                table: "user",
                newName: "Role");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "user",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "user",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_by_user_id",
                table: "user",
                newName: "UpdatedByUserId");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "user",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "refresh_token_expires_at",
                table: "user",
                newName: "RefreshTokenExpiresAt");

            migrationBuilder.RenameColumn(
                name: "refresh_token",
                table: "user",
                newName: "RefreshToken");

            migrationBuilder.RenameColumn(
                name: "phone_number",
                table: "user",
                newName: "PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "password_reset_token_expiry",
                table: "user",
                newName: "PasswordResetTokenExpiry");

            migrationBuilder.RenameColumn(
                name: "password_reset_token",
                table: "user",
                newName: "PasswordResetToken");

            migrationBuilder.RenameColumn(
                name: "password_hash",
                table: "user",
                newName: "PasswordHash");

            migrationBuilder.RenameColumn(
                name: "organization_id",
                table: "user",
                newName: "OrganizationId");

            migrationBuilder.RenameColumn(
                name: "last_name",
                table: "user",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "last_login_at",
                table: "user",
                newName: "LastLoginAt");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "user",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "user",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "google_id",
                table: "user",
                newName: "GoogleId");

            migrationBuilder.RenameColumn(
                name: "first_name",
                table: "user",
                newName: "FirstName");

            migrationBuilder.RenameColumn(
                name: "employee_id",
                table: "user",
                newName: "EmployeeId");

            migrationBuilder.RenameColumn(
                name: "created_by_user_id",
                table: "user",
                newName: "CreatedByUserId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "user",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "ix_user_updated_by_user_id",
                table: "user",
                newName: "IX_user_UpdatedByUserId");

            migrationBuilder.RenameIndex(
                name: "ix_user_organization_id_email",
                table: "user",
                newName: "IX_user_OrganizationId_Email");

            migrationBuilder.RenameIndex(
                name: "ix_user_google_id",
                table: "user",
                newName: "IX_user_GoogleId");

            migrationBuilder.RenameIndex(
                name: "ix_user_employee_id",
                table: "user",
                newName: "IX_user_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "ix_user_created_by_user_id",
                table: "user",
                newName: "IX_user_CreatedByUserId");

            migrationBuilder.RenameColumn(
                name: "notes",
                table: "Shifts",
                newName: "Notes");

            migrationBuilder.RenameColumn(
                name: "location",
                table: "Shifts",
                newName: "Location");

            migrationBuilder.RenameColumn(
                name: "date",
                table: "Shifts",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Shifts",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "start_time",
                table: "Shifts",
                newName: "StartTime");

            migrationBuilder.RenameColumn(
                name: "roster_id",
                table: "Shifts",
                newName: "RosterId");

            migrationBuilder.RenameColumn(
                name: "rest_breaks_duration",
                table: "Shifts",
                newName: "RestBreaksDuration");

            migrationBuilder.RenameColumn(
                name: "public_holiday_name",
                table: "Shifts",
                newName: "PublicHolidayName");

            migrationBuilder.RenameColumn(
                name: "organization_id",
                table: "Shifts",
                newName: "OrganizationId");

            migrationBuilder.RenameColumn(
                name: "meal_break_duration",
                table: "Shifts",
                newName: "MealBreakDuration");

            migrationBuilder.RenameColumn(
                name: "is_public_holiday",
                table: "Shifts",
                newName: "IsPublicHoliday");

            migrationBuilder.RenameColumn(
                name: "is_on_call",
                table: "Shifts",
                newName: "IsOnCall");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "Shifts",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "has_rest_breaks",
                table: "Shifts",
                newName: "HasRestBreaks");

            migrationBuilder.RenameColumn(
                name: "has_meal_break",
                table: "Shifts",
                newName: "HasMealBreak");

            migrationBuilder.RenameColumn(
                name: "end_time",
                table: "Shifts",
                newName: "EndTime");

            migrationBuilder.RenameColumn(
                name: "employee_id",
                table: "Shifts",
                newName: "EmployeeId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Shifts",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "ix_shifts_roster_id_employee_id_date",
                table: "Shifts",
                newName: "IX_Shifts_RosterId_EmployeeId_Date");

            migrationBuilder.RenameIndex(
                name: "ix_shifts_organization_id_date",
                table: "Shifts",
                newName: "IX_Shifts_OrganizationId_Date");

            migrationBuilder.RenameIndex(
                name: "ix_shifts_employee_id_date",
                table: "Shifts",
                newName: "IX_Shifts_EmployeeId_Date");

            migrationBuilder.RenameColumn(
                name: "year",
                table: "Rosters",
                newName: "Year");

            migrationBuilder.RenameColumn(
                name: "notes",
                table: "Rosters",
                newName: "Notes");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Rosters",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "week_start_date",
                table: "Rosters",
                newName: "WeekStartDate");

            migrationBuilder.RenameColumn(
                name: "week_number",
                table: "Rosters",
                newName: "WeekNumber");

            migrationBuilder.RenameColumn(
                name: "week_end_date",
                table: "Rosters",
                newName: "WeekEndDate");

            migrationBuilder.RenameColumn(
                name: "updated_by_user_id",
                table: "Rosters",
                newName: "UpdatedByUserId");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "Rosters",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "total_shifts",
                table: "Rosters",
                newName: "TotalShifts");

            migrationBuilder.RenameColumn(
                name: "total_hours",
                table: "Rosters",
                newName: "TotalHours");

            migrationBuilder.RenameColumn(
                name: "total_employees",
                table: "Rosters",
                newName: "TotalEmployees");

            migrationBuilder.RenameColumn(
                name: "organization_id",
                table: "Rosters",
                newName: "OrganizationId");

            migrationBuilder.RenameColumn(
                name: "is_finalized",
                table: "Rosters",
                newName: "IsFinalized");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "Rosters",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "finalized_at",
                table: "Rosters",
                newName: "FinalizedAt");

            migrationBuilder.RenameColumn(
                name: "created_by_user_id",
                table: "Rosters",
                newName: "CreatedByUserId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Rosters",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "ix_rosters_updated_by_user_id",
                table: "Rosters",
                newName: "IX_Rosters_UpdatedByUserId");

            migrationBuilder.RenameIndex(
                name: "ix_rosters_organization_id_year_week_number",
                table: "Rosters",
                newName: "IX_Rosters_OrganizationId_Year_WeekNumber");

            migrationBuilder.RenameIndex(
                name: "ix_rosters_organization_id_week_start_date",
                table: "Rosters",
                newName: "IX_Rosters_OrganizationId_WeekStartDate");

            migrationBuilder.RenameIndex(
                name: "ix_rosters_created_by_user_id",
                table: "Rosters",
                newName: "IX_Rosters_CreatedByUserId");

            migrationBuilder.RenameColumn(
                name: "tax",
                table: "Payslips",
                newName: "Tax");

            migrationBuilder.RenameColumn(
                name: "superannuation",
                table: "Payslips",
                newName: "Superannuation");

            migrationBuilder.RenameColumn(
                name: "classification",
                table: "Payslips",
                newName: "Classification");

            migrationBuilder.RenameColumn(
                name: "allowances",
                table: "Payslips",
                newName: "Allowances");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Payslips",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_by_user_id",
                table: "Payslips",
                newName: "UpdatedByUserId");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "Payslips",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "sunday_pay",
                table: "Payslips",
                newName: "SundayPay");

            migrationBuilder.RenameColumn(
                name: "sunday_hours",
                table: "Payslips",
                newName: "SundayHours");

            migrationBuilder.RenameColumn(
                name: "source_data",
                table: "Payslips",
                newName: "SourceData");

            migrationBuilder.RenameColumn(
                name: "saturday_pay",
                table: "Payslips",
                newName: "SaturdayPay");

            migrationBuilder.RenameColumn(
                name: "saturday_hours",
                table: "Payslips",
                newName: "SaturdayHours");

            migrationBuilder.RenameColumn(
                name: "public_holiday_pay",
                table: "Payslips",
                newName: "PublicHolidayPay");

            migrationBuilder.RenameColumn(
                name: "public_holiday_hours",
                table: "Payslips",
                newName: "PublicHolidayHours");

            migrationBuilder.RenameColumn(
                name: "payroll_validation_id",
                table: "Payslips",
                newName: "PayrollValidationId");

            migrationBuilder.RenameColumn(
                name: "pay_period_start",
                table: "Payslips",
                newName: "PayPeriodStart");

            migrationBuilder.RenameColumn(
                name: "pay_period_end",
                table: "Payslips",
                newName: "PayPeriodEnd");

            migrationBuilder.RenameColumn(
                name: "pay_date",
                table: "Payslips",
                newName: "PayDate");

            migrationBuilder.RenameColumn(
                name: "overtime_pay",
                table: "Payslips",
                newName: "OvertimePay");

            migrationBuilder.RenameColumn(
                name: "overtime_hours",
                table: "Payslips",
                newName: "OvertimeHours");

            migrationBuilder.RenameColumn(
                name: "other_deductions",
                table: "Payslips",
                newName: "OtherDeductions");

            migrationBuilder.RenameColumn(
                name: "organization_id",
                table: "Payslips",
                newName: "OrganizationId");

            migrationBuilder.RenameColumn(
                name: "ordinary_pay",
                table: "Payslips",
                newName: "OrdinaryPay");

            migrationBuilder.RenameColumn(
                name: "ordinary_hours",
                table: "Payslips",
                newName: "OrdinaryHours");

            migrationBuilder.RenameColumn(
                name: "net_pay",
                table: "Payslips",
                newName: "NetPay");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "Payslips",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "hourly_rate",
                table: "Payslips",
                newName: "HourlyRate");

            migrationBuilder.RenameColumn(
                name: "gross_pay",
                table: "Payslips",
                newName: "GrossPay");

            migrationBuilder.RenameColumn(
                name: "external_reference",
                table: "Payslips",
                newName: "ExternalReference");

            migrationBuilder.RenameColumn(
                name: "employment_type",
                table: "Payslips",
                newName: "EmploymentType");

            migrationBuilder.RenameColumn(
                name: "employee_number",
                table: "Payslips",
                newName: "EmployeeNumber");

            migrationBuilder.RenameColumn(
                name: "employee_name",
                table: "Payslips",
                newName: "EmployeeName");

            migrationBuilder.RenameColumn(
                name: "employee_id",
                table: "Payslips",
                newName: "EmployeeId");

            migrationBuilder.RenameColumn(
                name: "created_by_user_id",
                table: "Payslips",
                newName: "CreatedByUserId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Payslips",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "casual_loading_pay",
                table: "Payslips",
                newName: "CasualLoadingPay");

            migrationBuilder.RenameColumn(
                name: "award_type",
                table: "Payslips",
                newName: "AwardType");

            migrationBuilder.RenameIndex(
                name: "ix_payslips_updated_by_user_id",
                table: "Payslips",
                newName: "IX_Payslips_UpdatedByUserId");

            migrationBuilder.RenameIndex(
                name: "ix_payslips_payroll_validation_id",
                table: "Payslips",
                newName: "IX_Payslips_PayrollValidationId");

            migrationBuilder.RenameIndex(
                name: "ix_payslips_organization_id",
                table: "Payslips",
                newName: "IX_Payslips_OrganizationId");

            migrationBuilder.RenameIndex(
                name: "ix_payslips_employee_id",
                table: "Payslips",
                newName: "IX_Payslips_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "ix_payslips_created_by_user_id",
                table: "Payslips",
                newName: "IX_Payslips_CreatedByUserId");

            migrationBuilder.RenameColumn(
                name: "suburb",
                table: "organization",
                newName: "Suburb");

            migrationBuilder.RenameColumn(
                name: "state",
                table: "organization",
                newName: "State");

            migrationBuilder.RenameColumn(
                name: "postcode",
                table: "organization",
                newName: "Postcode");

            migrationBuilder.RenameColumn(
                name: "abn",
                table: "organization",
                newName: "ABN");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "organization",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_by_user_id",
                table: "organization",
                newName: "UpdatedByUserId");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "organization",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "subscription_tier",
                table: "organization",
                newName: "SubscriptionTier");

            migrationBuilder.RenameColumn(
                name: "subscription_start_date",
                table: "organization",
                newName: "SubscriptionStartDate");

            migrationBuilder.RenameColumn(
                name: "subscription_end_date",
                table: "organization",
                newName: "SubscriptionEndDate");

            migrationBuilder.RenameColumn(
                name: "phone_number",
                table: "organization",
                newName: "PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "logo_url",
                table: "organization",
                newName: "LogoUrl");

            migrationBuilder.RenameColumn(
                name: "is_subscription_active",
                table: "organization",
                newName: "IsSubscriptionActive");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "organization",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "industry_type",
                table: "organization",
                newName: "IndustryType");

            migrationBuilder.RenameColumn(
                name: "current_employee_count",
                table: "organization",
                newName: "CurrentEmployeeCount");

            migrationBuilder.RenameColumn(
                name: "created_by_user_id",
                table: "organization",
                newName: "CreatedByUserId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "organization",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "contact_email",
                table: "organization",
                newName: "ContactEmail");

            migrationBuilder.RenameColumn(
                name: "company_name",
                table: "organization",
                newName: "CompanyName");

            migrationBuilder.RenameColumn(
                name: "address_line2",
                table: "organization",
                newName: "AddressLine2");

            migrationBuilder.RenameColumn(
                name: "address_line1",
                table: "organization",
                newName: "AddressLine1");

            migrationBuilder.RenameIndex(
                name: "ix_organization_abn",
                table: "organization",
                newName: "IX_organization_ABN");

            migrationBuilder.RenameIndex(
                name: "ix_organization_updated_by_user_id",
                table: "organization",
                newName: "IX_organization_UpdatedByUserId");

            migrationBuilder.RenameIndex(
                name: "ix_organization_created_by_user_id",
                table: "organization",
                newName: "IX_organization_CreatedByUserId");

            migrationBuilder.RenameIndex(
                name: "ix_organization_contact_email",
                table: "organization",
                newName: "IX_organization_ContactEmail");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "Employees",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "department",
                table: "Employees",
                newName: "Department");

            migrationBuilder.RenameColumn(
                name: "address",
                table: "Employees",
                newName: "Address");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Employees",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_by_user_id",
                table: "Employees",
                newName: "UpdatedByUserId");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "Employees",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "tax_file_number",
                table: "Employees",
                newName: "TaxFileNumber");

            migrationBuilder.RenameColumn(
                name: "superannuation_member_number",
                table: "Employees",
                newName: "SuperannuationMemberNumber");

            migrationBuilder.RenameColumn(
                name: "superannuation_fund",
                table: "Employees",
                newName: "SuperannuationFund");

            migrationBuilder.RenameColumn(
                name: "start_date",
                table: "Employees",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "phone_number",
                table: "Employees",
                newName: "PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "organization_id",
                table: "Employees",
                newName: "OrganizationId");

            migrationBuilder.RenameColumn(
                name: "last_name",
                table: "Employees",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "job_title",
                table: "Employees",
                newName: "JobTitle");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "Employees",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "Employees",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "guaranteed_hours",
                table: "Employees",
                newName: "GuaranteedHours");

            migrationBuilder.RenameColumn(
                name: "first_name",
                table: "Employees",
                newName: "FirstName");

            migrationBuilder.RenameColumn(
                name: "end_date",
                table: "Employees",
                newName: "EndDate");

            migrationBuilder.RenameColumn(
                name: "employment_type",
                table: "Employees",
                newName: "EmploymentType");

            migrationBuilder.RenameColumn(
                name: "employee_number",
                table: "Employees",
                newName: "EmployeeNumber");

            migrationBuilder.RenameColumn(
                name: "date_of_birth",
                table: "Employees",
                newName: "DateOfBirth");

            migrationBuilder.RenameColumn(
                name: "created_by_user_id",
                table: "Employees",
                newName: "CreatedByUserId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Employees",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "award_type",
                table: "Employees",
                newName: "AwardType");

            migrationBuilder.RenameColumn(
                name: "award_level_number",
                table: "Employees",
                newName: "AwardLevelNumber");

            migrationBuilder.RenameIndex(
                name: "ix_employees_updated_by_user_id",
                table: "Employees",
                newName: "IX_Employees_UpdatedByUserId");

            migrationBuilder.RenameIndex(
                name: "ix_employees_organization_id_employee_number",
                table: "Employees",
                newName: "IX_Employees_OrganizationId_EmployeeNumber");

            migrationBuilder.RenameIndex(
                name: "ix_employees_organization_id_email",
                table: "Employees",
                newName: "IX_Employees_OrganizationId_Email");

            migrationBuilder.RenameIndex(
                name: "ix_employees_created_by_user_id",
                table: "Employees",
                newName: "IX_Employees_CreatedByUserId");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "RosterValidations",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "notes",
                table: "RosterValidations",
                newName: "Notes");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "RosterValidations",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "weekly_hours_check_performed",
                table: "RosterValidations",
                newName: "WeeklyHoursCheckPerformed");

            migrationBuilder.RenameColumn(
                name: "week_start_date",
                table: "RosterValidations",
                newName: "WeekStartDate");

            migrationBuilder.RenameColumn(
                name: "week_end_date",
                table: "RosterValidations",
                newName: "WeekEndDate");

            migrationBuilder.RenameColumn(
                name: "updated_by_user_id",
                table: "RosterValidations",
                newName: "UpdatedByUserId");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "RosterValidations",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "total_shifts",
                table: "RosterValidations",
                newName: "TotalShifts");

            migrationBuilder.RenameColumn(
                name: "total_issues_count",
                table: "RosterValidations",
                newName: "TotalIssuesCount");

            migrationBuilder.RenameColumn(
                name: "started_at",
                table: "RosterValidations",
                newName: "StartedAt");

            migrationBuilder.RenameColumn(
                name: "roster_id",
                table: "RosterValidations",
                newName: "RosterId");

            migrationBuilder.RenameColumn(
                name: "rest_period_check_performed",
                table: "RosterValidations",
                newName: "RestPeriodCheckPerformed");

            migrationBuilder.RenameColumn(
                name: "passed_shifts",
                table: "RosterValidations",
                newName: "PassedShifts");

            migrationBuilder.RenameColumn(
                name: "organization_id",
                table: "RosterValidations",
                newName: "OrganizationId");

            migrationBuilder.RenameColumn(
                name: "minimum_shift_hours_check_performed",
                table: "RosterValidations",
                newName: "MinimumShiftHoursCheckPerformed");

            migrationBuilder.RenameColumn(
                name: "meal_break_check_performed",
                table: "RosterValidations",
                newName: "MealBreakCheckPerformed");

            migrationBuilder.RenameColumn(
                name: "max_consecutive_days_check_performed",
                table: "RosterValidations",
                newName: "MaxConsecutiveDaysCheckPerformed");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "RosterValidations",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "failed_shifts",
                table: "RosterValidations",
                newName: "FailedShifts");

            migrationBuilder.RenameColumn(
                name: "critical_issues_count",
                table: "RosterValidations",
                newName: "CriticalIssuesCount");

            migrationBuilder.RenameColumn(
                name: "created_by_user_id",
                table: "RosterValidations",
                newName: "CreatedByUserId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "RosterValidations",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "completed_at",
                table: "RosterValidations",
                newName: "CompletedAt");

            migrationBuilder.RenameColumn(
                name: "affected_employees",
                table: "RosterValidations",
                newName: "AffectedEmployees");

            migrationBuilder.RenameIndex(
                name: "ix_roster_validations_updated_by_user_id",
                table: "RosterValidations",
                newName: "IX_RosterValidations_UpdatedByUserId");

            migrationBuilder.RenameIndex(
                name: "ix_roster_validations_roster_id",
                table: "RosterValidations",
                newName: "IX_RosterValidations_RosterId");

            migrationBuilder.RenameIndex(
                name: "ix_roster_validations_organization_id_status",
                table: "RosterValidations",
                newName: "IX_RosterValidations_OrganizationId_Status");

            migrationBuilder.RenameIndex(
                name: "ix_roster_validations_created_by_user_id",
                table: "RosterValidations",
                newName: "IX_RosterValidations_CreatedByUserId");

            migrationBuilder.RenameColumn(
                name: "severity",
                table: "RosterIssues",
                newName: "Severity");

            migrationBuilder.RenameColumn(
                name: "recommendation",
                table: "RosterIssues",
                newName: "Recommendation");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "RosterIssues",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "RosterIssues",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "waiver_reason",
                table: "RosterIssues",
                newName: "WaiverReason");

            migrationBuilder.RenameColumn(
                name: "waived_by_user_id",
                table: "RosterIssues",
                newName: "WaivedByUserId");

            migrationBuilder.RenameColumn(
                name: "waived_at",
                table: "RosterIssues",
                newName: "WaivedAt");

            migrationBuilder.RenameColumn(
                name: "shift_id",
                table: "RosterIssues",
                newName: "ShiftId");

            migrationBuilder.RenameColumn(
                name: "roster_validation_id",
                table: "RosterIssues",
                newName: "RosterValidationId");

            migrationBuilder.RenameColumn(
                name: "resolved_by_user_id",
                table: "RosterIssues",
                newName: "ResolvedByUserId");

            migrationBuilder.RenameColumn(
                name: "resolved_at",
                table: "RosterIssues",
                newName: "ResolvedAt");

            migrationBuilder.RenameColumn(
                name: "resolution_notes",
                table: "RosterIssues",
                newName: "ResolutionNotes");

            migrationBuilder.RenameColumn(
                name: "organization_id",
                table: "RosterIssues",
                newName: "OrganizationId");

            migrationBuilder.RenameColumn(
                name: "is_waived",
                table: "RosterIssues",
                newName: "IsWaived");

            migrationBuilder.RenameColumn(
                name: "is_resolved",
                table: "RosterIssues",
                newName: "IsResolved");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "RosterIssues",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "expected_value",
                table: "RosterIssues",
                newName: "ExpectedValue");

            migrationBuilder.RenameColumn(
                name: "employee_id",
                table: "RosterIssues",
                newName: "EmployeeId");

            migrationBuilder.RenameColumn(
                name: "detailed_explanation",
                table: "RosterIssues",
                newName: "DetailedExplanation");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "RosterIssues",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "check_type",
                table: "RosterIssues",
                newName: "CheckType");

            migrationBuilder.RenameColumn(
                name: "affected_shifts_count",
                table: "RosterIssues",
                newName: "AffectedShiftsCount");

            migrationBuilder.RenameColumn(
                name: "affected_dates",
                table: "RosterIssues",
                newName: "AffectedDates");

            migrationBuilder.RenameColumn(
                name: "actual_value",
                table: "RosterIssues",
                newName: "ActualValue");

            migrationBuilder.RenameIndex(
                name: "ix_roster_issues_waived_by_user_id",
                table: "RosterIssues",
                newName: "IX_RosterIssues_WaivedByUserId");

            migrationBuilder.RenameIndex(
                name: "ix_roster_issues_shift_id",
                table: "RosterIssues",
                newName: "IX_RosterIssues_ShiftId");

            migrationBuilder.RenameIndex(
                name: "ix_roster_issues_roster_validation_id_severity",
                table: "RosterIssues",
                newName: "IX_RosterIssues_RosterValidationId_Severity");

            migrationBuilder.RenameIndex(
                name: "ix_roster_issues_roster_validation_id_employee_id",
                table: "RosterIssues",
                newName: "IX_RosterIssues_RosterValidationId_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "ix_roster_issues_resolved_by_user_id",
                table: "RosterIssues",
                newName: "IX_RosterIssues_ResolvedByUserId");

            migrationBuilder.RenameIndex(
                name: "ix_roster_issues_organization_id",
                table: "RosterIssues",
                newName: "IX_RosterIssues_OrganizationId");

            migrationBuilder.RenameIndex(
                name: "ix_roster_issues_employee_id",
                table: "RosterIssues",
                newName: "IX_RosterIssues_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "ix_roster_issues_check_type",
                table: "RosterIssues",
                newName: "IX_RosterIssues_CheckType");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "PayrollValidations",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "notes",
                table: "PayrollValidations",
                newName: "Notes");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "PayrollValidations",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_by_user_id",
                table: "PayrollValidations",
                newName: "UpdatedByUserId");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "PayrollValidations",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "total_payslips",
                table: "PayrollValidations",
                newName: "TotalPayslips");

            migrationBuilder.RenameColumn(
                name: "total_issues_count",
                table: "PayrollValidations",
                newName: "TotalIssuesCount");

            migrationBuilder.RenameColumn(
                name: "superannuation_check_performed",
                table: "PayrollValidations",
                newName: "SuperannuationCheckPerformed");

            migrationBuilder.RenameColumn(
                name: "stp_check_performed",
                table: "PayrollValidations",
                newName: "STPCheckPerformed");

            migrationBuilder.RenameColumn(
                name: "started_at",
                table: "PayrollValidations",
                newName: "StartedAt");

            migrationBuilder.RenameColumn(
                name: "penalty_rate_check_performed",
                table: "PayrollValidations",
                newName: "PenaltyRateCheckPerformed");

            migrationBuilder.RenameColumn(
                name: "pay_period_start",
                table: "PayrollValidations",
                newName: "PayPeriodStart");

            migrationBuilder.RenameColumn(
                name: "pay_period_end",
                table: "PayrollValidations",
                newName: "PayPeriodEnd");

            migrationBuilder.RenameColumn(
                name: "passed_count",
                table: "PayrollValidations",
                newName: "PassedCount");

            migrationBuilder.RenameColumn(
                name: "organization_id",
                table: "PayrollValidations",
                newName: "OrganizationId");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "PayrollValidations",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "file_path",
                table: "PayrollValidations",
                newName: "FilePath");

            migrationBuilder.RenameColumn(
                name: "file_name",
                table: "PayrollValidations",
                newName: "FileName");

            migrationBuilder.RenameColumn(
                name: "failed_count",
                table: "PayrollValidations",
                newName: "FailedCount");

            migrationBuilder.RenameColumn(
                name: "critical_issues_count",
                table: "PayrollValidations",
                newName: "CriticalIssuesCount");

            migrationBuilder.RenameColumn(
                name: "created_by_user_id",
                table: "PayrollValidations",
                newName: "CreatedByUserId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "PayrollValidations",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "completed_at",
                table: "PayrollValidations",
                newName: "CompletedAt");

            migrationBuilder.RenameColumn(
                name: "casual_loading_check_performed",
                table: "PayrollValidations",
                newName: "CasualLoadingCheckPerformed");

            migrationBuilder.RenameColumn(
                name: "base_rate_check_performed",
                table: "PayrollValidations",
                newName: "BaseRateCheckPerformed");

            migrationBuilder.RenameIndex(
                name: "ix_payroll_validations_updated_by_user_id",
                table: "PayrollValidations",
                newName: "IX_PayrollValidations_UpdatedByUserId");

            migrationBuilder.RenameIndex(
                name: "ix_payroll_validations_organization_id",
                table: "PayrollValidations",
                newName: "IX_PayrollValidations_OrganizationId");

            migrationBuilder.RenameIndex(
                name: "ix_payroll_validations_created_by_user_id",
                table: "PayrollValidations",
                newName: "IX_PayrollValidations_CreatedByUserId");

            migrationBuilder.RenameColumn(
                name: "severity",
                table: "PayrollIssues",
                newName: "Severity");

            migrationBuilder.RenameColumn(
                name: "recommendation",
                table: "PayrollIssues",
                newName: "Recommendation");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "PayrollIssues",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "PayrollIssues",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "unit_type",
                table: "PayrollIssues",
                newName: "UnitType");

            migrationBuilder.RenameColumn(
                name: "resolved_by_user_id",
                table: "PayrollIssues",
                newName: "ResolvedByUserId");

            migrationBuilder.RenameColumn(
                name: "resolved_at",
                table: "PayrollIssues",
                newName: "ResolvedAt");

            migrationBuilder.RenameColumn(
                name: "resolution_notes",
                table: "PayrollIssues",
                newName: "ResolutionNotes");

            migrationBuilder.RenameColumn(
                name: "payslip_id",
                table: "PayrollIssues",
                newName: "PayslipId");

            migrationBuilder.RenameColumn(
                name: "payroll_validation_id",
                table: "PayrollIssues",
                newName: "PayrollValidationId");

            migrationBuilder.RenameColumn(
                name: "organization_id",
                table: "PayrollIssues",
                newName: "OrganizationId");

            migrationBuilder.RenameColumn(
                name: "is_resolved",
                table: "PayrollIssues",
                newName: "IsResolved");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "PayrollIssues",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "expected_value",
                table: "PayrollIssues",
                newName: "ExpectedValue");

            migrationBuilder.RenameColumn(
                name: "employee_id",
                table: "PayrollIssues",
                newName: "EmployeeId");

            migrationBuilder.RenameColumn(
                name: "detailed_explanation",
                table: "PayrollIssues",
                newName: "DetailedExplanation");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "PayrollIssues",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "context_label",
                table: "PayrollIssues",
                newName: "ContextLabel");

            migrationBuilder.RenameColumn(
                name: "check_type",
                table: "PayrollIssues",
                newName: "CheckType");

            migrationBuilder.RenameColumn(
                name: "affected_units",
                table: "PayrollIssues",
                newName: "AffectedUnits");

            migrationBuilder.RenameColumn(
                name: "actual_value",
                table: "PayrollIssues",
                newName: "ActualValue");

            migrationBuilder.RenameIndex(
                name: "ix_payroll_issues_resolved_by_user_id",
                table: "PayrollIssues",
                newName: "IX_PayrollIssues_ResolvedByUserId");

            migrationBuilder.RenameIndex(
                name: "ix_payroll_issues_payslip_id",
                table: "PayrollIssues",
                newName: "IX_PayrollIssues_PayslipId");

            migrationBuilder.RenameIndex(
                name: "ix_payroll_issues_payroll_validation_id",
                table: "PayrollIssues",
                newName: "IX_PayrollIssues_PayrollValidationId");

            migrationBuilder.RenameIndex(
                name: "ix_payroll_issues_organization_id",
                table: "PayrollIssues",
                newName: "IX_PayrollIssues_OrganizationId");

            migrationBuilder.RenameIndex(
                name: "ix_payroll_issues_employee_id",
                table: "PayrollIssues",
                newName: "IX_PayrollIssues_EmployeeId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "OrganizationAwards",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "organization_id",
                table: "OrganizationAwards",
                newName: "OrganizationId");

            migrationBuilder.RenameColumn(
                name: "is_primary",
                table: "OrganizationAwards",
                newName: "IsPrimary");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "OrganizationAwards",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "employee_count",
                table: "OrganizationAwards",
                newName: "EmployeeCount");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "OrganizationAwards",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "award_type",
                table: "OrganizationAwards",
                newName: "AwardType");

            migrationBuilder.RenameColumn(
                name: "added_at",
                table: "OrganizationAwards",
                newName: "AddedAt");

            migrationBuilder.RenameIndex(
                name: "ix_organization_award_organization_id_is_primary",
                table: "OrganizationAwards",
                newName: "IX_OrganizationAwards_OrganizationId_IsPrimary");

            migrationBuilder.RenameIndex(
                name: "ix_organization_award_organization_id_award_type",
                table: "OrganizationAwards",
                newName: "IX_OrganizationAwards_OrganizationId_AwardType");

            migrationBuilder.RenameColumn(
                name: "notes",
                table: "Documents",
                newName: "Notes");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Documents",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "uploaded_file_size",
                table: "Documents",
                newName: "UploadedFileSize");

            migrationBuilder.RenameColumn(
                name: "uploaded_file_path",
                table: "Documents",
                newName: "UploadedFilePath");

            migrationBuilder.RenameColumn(
                name: "uploaded_file_name",
                table: "Documents",
                newName: "UploadedFileName");

            migrationBuilder.RenameColumn(
                name: "updated_by_user_id",
                table: "Documents",
                newName: "UpdatedByUserId");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "Documents",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "provided_method",
                table: "Documents",
                newName: "ProvidedMethod");

            migrationBuilder.RenameColumn(
                name: "provided_at",
                table: "Documents",
                newName: "ProvidedAt");

            migrationBuilder.RenameColumn(
                name: "organization_id",
                table: "Documents",
                newName: "OrganizationId");

            migrationBuilder.RenameColumn(
                name: "is_provided",
                table: "Documents",
                newName: "IsProvided");

            migrationBuilder.RenameColumn(
                name: "is_legally_required",
                table: "Documents",
                newName: "IsLegallyRequired");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "Documents",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "employee_id",
                table: "Documents",
                newName: "EmployeeId");

            migrationBuilder.RenameColumn(
                name: "document_type",
                table: "Documents",
                newName: "DocumentType");

            migrationBuilder.RenameColumn(
                name: "created_by_user_id",
                table: "Documents",
                newName: "CreatedByUserId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Documents",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "compliance_deadline",
                table: "Documents",
                newName: "ComplianceDeadline");

            migrationBuilder.RenameIndex(
                name: "ix_document_updated_by_user_id",
                table: "Documents",
                newName: "IX_Documents_UpdatedByUserId");

            migrationBuilder.RenameIndex(
                name: "ix_document_organization_id_document_type",
                table: "Documents",
                newName: "IX_Documents_OrganizationId_DocumentType");

            migrationBuilder.RenameIndex(
                name: "ix_document_is_provided",
                table: "Documents",
                newName: "IX_Documents_IsProvided");

            migrationBuilder.RenameIndex(
                name: "ix_document_employee_id_document_type",
                table: "Documents",
                newName: "IX_Documents_EmployeeId_DocumentType");

            migrationBuilder.RenameIndex(
                name: "ix_document_created_by_user_id",
                table: "Documents",
                newName: "IX_Documents_CreatedByUserId");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "AwardLevels",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "AwardLevels",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "part_time_hourly_rate",
                table: "AwardLevels",
                newName: "PartTimeHourlyRate");

            migrationBuilder.RenameColumn(
                name: "level_number",
                table: "AwardLevels",
                newName: "LevelNumber");

            migrationBuilder.RenameColumn(
                name: "level_name",
                table: "AwardLevels",
                newName: "LevelName");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "AwardLevels",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "AwardLevels",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "full_time_hourly_rate",
                table: "AwardLevels",
                newName: "FullTimeHourlyRate");

            migrationBuilder.RenameColumn(
                name: "effective_to",
                table: "AwardLevels",
                newName: "EffectiveTo");

            migrationBuilder.RenameColumn(
                name: "effective_from",
                table: "AwardLevels",
                newName: "EffectiveFrom");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "AwardLevels",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "casual_hourly_rate",
                table: "AwardLevels",
                newName: "CasualHourlyRate");

            migrationBuilder.RenameColumn(
                name: "award_id",
                table: "AwardLevels",
                newName: "AwardId");

            migrationBuilder.RenameIndex(
                name: "ix_award_level_award_id_level_number_is_active",
                table: "AwardLevels",
                newName: "IX_AwardLevels_AwardId_LevelNumber_IsActive");

            migrationBuilder.RenameIndex(
                name: "ix_award_level_award_id_level_number_effective_from",
                table: "AwardLevels",
                newName: "IX_AwardLevels_AwardId_LevelNumber_EffectiveFrom");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Awards",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "Awards",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Awards",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "sunday_penalty_rate",
                table: "Awards",
                newName: "SundayPenaltyRate");

            migrationBuilder.RenameColumn(
                name: "saturday_penalty_rate",
                table: "Awards",
                newName: "SaturdayPenaltyRate");

            migrationBuilder.RenameColumn(
                name: "public_holiday_penalty_rate",
                table: "Awards",
                newName: "PublicHolidayPenaltyRate");

            migrationBuilder.RenameColumn(
                name: "ordinary_weekly_hours",
                table: "Awards",
                newName: "OrdinaryWeeklyHours");

            migrationBuilder.RenameColumn(
                name: "minimum_shift_hours",
                table: "Awards",
                newName: "MinimumShiftHours");

            migrationBuilder.RenameColumn(
                name: "minimum_rest_period_hours",
                table: "Awards",
                newName: "MinimumRestPeriodHours");

            migrationBuilder.RenameColumn(
                name: "meal_break_threshold_hours",
                table: "Awards",
                newName: "MealBreakThresholdHours");

            migrationBuilder.RenameColumn(
                name: "meal_break_minutes",
                table: "Awards",
                newName: "MealBreakMinutes");

            migrationBuilder.RenameColumn(
                name: "max_consecutive_days",
                table: "Awards",
                newName: "MaxConsecutiveDays");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "Awards",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Awards",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "casual_loading_rate",
                table: "Awards",
                newName: "CasualLoadingRate");

            migrationBuilder.RenameColumn(
                name: "award_type",
                table: "Awards",
                newName: "AwardType");

            migrationBuilder.RenameColumn(
                name: "award_code",
                table: "Awards",
                newName: "AwardCode");

            migrationBuilder.RenameIndex(
                name: "ix_award_award_type",
                table: "Awards",
                newName: "IX_Awards_AwardType");

            migrationBuilder.RenameIndex(
                name: "ix_award_award_code",
                table: "Awards",
                newName: "IX_Awards_AwardCode");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user",
                table: "user",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Shifts",
                table: "Shifts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rosters",
                table: "Rosters",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payslips",
                table: "Payslips",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_organization",
                table: "organization",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Employees",
                table: "Employees",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RosterValidations",
                table: "RosterValidations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RosterIssues",
                table: "RosterIssues",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PayrollValidations",
                table: "PayrollValidations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PayrollIssues",
                table: "PayrollIssues",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganizationAwards",
                table: "OrganizationAwards",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Documents",
                table: "Documents",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AwardLevels",
                table: "AwardLevels",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Awards",
                table: "Awards",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AwardLevels_Awards_AwardId",
                table: "AwardLevels",
                column: "AwardId",
                principalTable: "Awards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Employees_EmployeeId",
                table: "Documents",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_organization_OrganizationId",
                table: "Documents",
                column: "OrganizationId",
                principalTable: "organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_user_CreatedByUserId",
                table: "Documents",
                column: "CreatedByUserId",
                principalTable: "user",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_user_UpdatedByUserId",
                table: "Documents",
                column: "UpdatedByUserId",
                principalTable: "user",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_organization_OrganizationId",
                table: "Employees",
                column: "OrganizationId",
                principalTable: "organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_user_CreatedByUserId",
                table: "Employees",
                column: "CreatedByUserId",
                principalTable: "user",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_user_UpdatedByUserId",
                table: "Employees",
                column: "UpdatedByUserId",
                principalTable: "user",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_organization_user_CreatedByUserId",
                table: "organization",
                column: "CreatedByUserId",
                principalTable: "user",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_organization_user_UpdatedByUserId",
                table: "organization",
                column: "UpdatedByUserId",
                principalTable: "user",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationAwards_organization_OrganizationId",
                table: "OrganizationAwards",
                column: "OrganizationId",
                principalTable: "organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollIssues_Employees_EmployeeId",
                table: "PayrollIssues",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollIssues_PayrollValidations_PayrollValidationId",
                table: "PayrollIssues",
                column: "PayrollValidationId",
                principalTable: "PayrollValidations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollIssues_Payslips_PayslipId",
                table: "PayrollIssues",
                column: "PayslipId",
                principalTable: "Payslips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollIssues_organization_OrganizationId",
                table: "PayrollIssues",
                column: "OrganizationId",
                principalTable: "organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollIssues_user_ResolvedByUserId",
                table: "PayrollIssues",
                column: "ResolvedByUserId",
                principalTable: "user",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollValidations_organization_OrganizationId",
                table: "PayrollValidations",
                column: "OrganizationId",
                principalTable: "organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollValidations_user_CreatedByUserId",
                table: "PayrollValidations",
                column: "CreatedByUserId",
                principalTable: "user",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollValidations_user_UpdatedByUserId",
                table: "PayrollValidations",
                column: "UpdatedByUserId",
                principalTable: "user",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Payslips_Employees_EmployeeId",
                table: "Payslips",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payslips_PayrollValidations_PayrollValidationId",
                table: "Payslips",
                column: "PayrollValidationId",
                principalTable: "PayrollValidations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payslips_organization_OrganizationId",
                table: "Payslips",
                column: "OrganizationId",
                principalTable: "organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payslips_user_CreatedByUserId",
                table: "Payslips",
                column: "CreatedByUserId",
                principalTable: "user",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Payslips_user_UpdatedByUserId",
                table: "Payslips",
                column: "UpdatedByUserId",
                principalTable: "user",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RosterIssues_Employees_EmployeeId",
                table: "RosterIssues",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RosterIssues_RosterValidations_RosterValidationId",
                table: "RosterIssues",
                column: "RosterValidationId",
                principalTable: "RosterValidations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RosterIssues_Shifts_ShiftId",
                table: "RosterIssues",
                column: "ShiftId",
                principalTable: "Shifts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RosterIssues_organization_OrganizationId",
                table: "RosterIssues",
                column: "OrganizationId",
                principalTable: "organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RosterIssues_user_ResolvedByUserId",
                table: "RosterIssues",
                column: "ResolvedByUserId",
                principalTable: "user",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RosterIssues_user_WaivedByUserId",
                table: "RosterIssues",
                column: "WaivedByUserId",
                principalTable: "user",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Rosters_organization_OrganizationId",
                table: "Rosters",
                column: "OrganizationId",
                principalTable: "organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Rosters_user_CreatedByUserId",
                table: "Rosters",
                column: "CreatedByUserId",
                principalTable: "user",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Rosters_user_UpdatedByUserId",
                table: "Rosters",
                column: "UpdatedByUserId",
                principalTable: "user",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RosterValidations_Rosters_RosterId",
                table: "RosterValidations",
                column: "RosterId",
                principalTable: "Rosters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RosterValidations_organization_OrganizationId",
                table: "RosterValidations",
                column: "OrganizationId",
                principalTable: "organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RosterValidations_user_CreatedByUserId",
                table: "RosterValidations",
                column: "CreatedByUserId",
                principalTable: "user",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RosterValidations_user_UpdatedByUserId",
                table: "RosterValidations",
                column: "UpdatedByUserId",
                principalTable: "user",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Shifts_Employees_EmployeeId",
                table: "Shifts",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Shifts_Rosters_RosterId",
                table: "Shifts",
                column: "RosterId",
                principalTable: "Rosters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Shifts_organization_OrganizationId",
                table: "Shifts",
                column: "OrganizationId",
                principalTable: "organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_user_Employees_EmployeeId",
                table: "user",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_user_organization_OrganizationId",
                table: "user",
                column: "OrganizationId",
                principalTable: "organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_user_user_CreatedByUserId",
                table: "user",
                column: "CreatedByUserId",
                principalTable: "user",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_user_user_UpdatedByUserId",
                table: "user",
                column: "UpdatedByUserId",
                principalTable: "user",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AwardLevels_Awards_AwardId",
                table: "AwardLevels");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Employees_EmployeeId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_organization_OrganizationId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_user_CreatedByUserId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_user_UpdatedByUserId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_organization_OrganizationId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_user_CreatedByUserId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_user_UpdatedByUserId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_organization_user_CreatedByUserId",
                table: "organization");

            migrationBuilder.DropForeignKey(
                name: "FK_organization_user_UpdatedByUserId",
                table: "organization");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationAwards_organization_OrganizationId",
                table: "OrganizationAwards");

            migrationBuilder.DropForeignKey(
                name: "FK_PayrollIssues_Employees_EmployeeId",
                table: "PayrollIssues");

            migrationBuilder.DropForeignKey(
                name: "FK_PayrollIssues_PayrollValidations_PayrollValidationId",
                table: "PayrollIssues");

            migrationBuilder.DropForeignKey(
                name: "FK_PayrollIssues_Payslips_PayslipId",
                table: "PayrollIssues");

            migrationBuilder.DropForeignKey(
                name: "FK_PayrollIssues_organization_OrganizationId",
                table: "PayrollIssues");

            migrationBuilder.DropForeignKey(
                name: "FK_PayrollIssues_user_ResolvedByUserId",
                table: "PayrollIssues");

            migrationBuilder.DropForeignKey(
                name: "FK_PayrollValidations_organization_OrganizationId",
                table: "PayrollValidations");

            migrationBuilder.DropForeignKey(
                name: "FK_PayrollValidations_user_CreatedByUserId",
                table: "PayrollValidations");

            migrationBuilder.DropForeignKey(
                name: "FK_PayrollValidations_user_UpdatedByUserId",
                table: "PayrollValidations");

            migrationBuilder.DropForeignKey(
                name: "FK_Payslips_Employees_EmployeeId",
                table: "Payslips");

            migrationBuilder.DropForeignKey(
                name: "FK_Payslips_PayrollValidations_PayrollValidationId",
                table: "Payslips");

            migrationBuilder.DropForeignKey(
                name: "FK_Payslips_organization_OrganizationId",
                table: "Payslips");

            migrationBuilder.DropForeignKey(
                name: "FK_Payslips_user_CreatedByUserId",
                table: "Payslips");

            migrationBuilder.DropForeignKey(
                name: "FK_Payslips_user_UpdatedByUserId",
                table: "Payslips");

            migrationBuilder.DropForeignKey(
                name: "FK_RosterIssues_Employees_EmployeeId",
                table: "RosterIssues");

            migrationBuilder.DropForeignKey(
                name: "FK_RosterIssues_RosterValidations_RosterValidationId",
                table: "RosterIssues");

            migrationBuilder.DropForeignKey(
                name: "FK_RosterIssues_Shifts_ShiftId",
                table: "RosterIssues");

            migrationBuilder.DropForeignKey(
                name: "FK_RosterIssues_organization_OrganizationId",
                table: "RosterIssues");

            migrationBuilder.DropForeignKey(
                name: "FK_RosterIssues_user_ResolvedByUserId",
                table: "RosterIssues");

            migrationBuilder.DropForeignKey(
                name: "FK_RosterIssues_user_WaivedByUserId",
                table: "RosterIssues");

            migrationBuilder.DropForeignKey(
                name: "FK_Rosters_organization_OrganizationId",
                table: "Rosters");

            migrationBuilder.DropForeignKey(
                name: "FK_Rosters_user_CreatedByUserId",
                table: "Rosters");

            migrationBuilder.DropForeignKey(
                name: "FK_Rosters_user_UpdatedByUserId",
                table: "Rosters");

            migrationBuilder.DropForeignKey(
                name: "FK_RosterValidations_Rosters_RosterId",
                table: "RosterValidations");

            migrationBuilder.DropForeignKey(
                name: "FK_RosterValidations_organization_OrganizationId",
                table: "RosterValidations");

            migrationBuilder.DropForeignKey(
                name: "FK_RosterValidations_user_CreatedByUserId",
                table: "RosterValidations");

            migrationBuilder.DropForeignKey(
                name: "FK_RosterValidations_user_UpdatedByUserId",
                table: "RosterValidations");

            migrationBuilder.DropForeignKey(
                name: "FK_Shifts_Employees_EmployeeId",
                table: "Shifts");

            migrationBuilder.DropForeignKey(
                name: "FK_Shifts_Rosters_RosterId",
                table: "Shifts");

            migrationBuilder.DropForeignKey(
                name: "FK_Shifts_organization_OrganizationId",
                table: "Shifts");

            migrationBuilder.DropForeignKey(
                name: "FK_user_Employees_EmployeeId",
                table: "user");

            migrationBuilder.DropForeignKey(
                name: "FK_user_organization_OrganizationId",
                table: "user");

            migrationBuilder.DropForeignKey(
                name: "FK_user_user_CreatedByUserId",
                table: "user");

            migrationBuilder.DropForeignKey(
                name: "FK_user_user_UpdatedByUserId",
                table: "user");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user",
                table: "user");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Shifts",
                table: "Shifts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rosters",
                table: "Rosters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payslips",
                table: "Payslips");

            migrationBuilder.DropPrimaryKey(
                name: "PK_organization",
                table: "organization");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Employees",
                table: "Employees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RosterValidations",
                table: "RosterValidations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RosterIssues",
                table: "RosterIssues");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PayrollValidations",
                table: "PayrollValidations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PayrollIssues",
                table: "PayrollIssues");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganizationAwards",
                table: "OrganizationAwards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Documents",
                table: "Documents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Awards",
                table: "Awards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AwardLevels",
                table: "AwardLevels");

            migrationBuilder.RenameTable(
                name: "Shifts",
                newName: "shifts");

            migrationBuilder.RenameTable(
                name: "Rosters",
                newName: "rosters");

            migrationBuilder.RenameTable(
                name: "Payslips",
                newName: "payslips");

            migrationBuilder.RenameTable(
                name: "Employees",
                newName: "employees");

            migrationBuilder.RenameTable(
                name: "RosterValidations",
                newName: "roster_validations");

            migrationBuilder.RenameTable(
                name: "RosterIssues",
                newName: "roster_issues");

            migrationBuilder.RenameTable(
                name: "PayrollValidations",
                newName: "payroll_validations");

            migrationBuilder.RenameTable(
                name: "PayrollIssues",
                newName: "payroll_issues");

            migrationBuilder.RenameTable(
                name: "OrganizationAwards",
                newName: "organization_award");

            migrationBuilder.RenameTable(
                name: "Documents",
                newName: "document");

            migrationBuilder.RenameTable(
                name: "Awards",
                newName: "award");

            migrationBuilder.RenameTable(
                name: "AwardLevels",
                newName: "award_level");

            migrationBuilder.RenameColumn(
                name: "Role",
                table: "user",
                newName: "role");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "user",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "user",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedByUserId",
                table: "user",
                newName: "updated_by_user_id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "user",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "RefreshTokenExpiresAt",
                table: "user",
                newName: "refresh_token_expires_at");

            migrationBuilder.RenameColumn(
                name: "RefreshToken",
                table: "user",
                newName: "refresh_token");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "user",
                newName: "phone_number");

            migrationBuilder.RenameColumn(
                name: "PasswordResetTokenExpiry",
                table: "user",
                newName: "password_reset_token_expiry");

            migrationBuilder.RenameColumn(
                name: "PasswordResetToken",
                table: "user",
                newName: "password_reset_token");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "user",
                newName: "password_hash");

            migrationBuilder.RenameColumn(
                name: "OrganizationId",
                table: "user",
                newName: "organization_id");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "user",
                newName: "last_name");

            migrationBuilder.RenameColumn(
                name: "LastLoginAt",
                table: "user",
                newName: "last_login_at");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "user",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "user",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "GoogleId",
                table: "user",
                newName: "google_id");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "user",
                newName: "first_name");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                table: "user",
                newName: "employee_id");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "user",
                newName: "created_by_user_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "user",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_user_UpdatedByUserId",
                table: "user",
                newName: "ix_user_updated_by_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_user_OrganizationId_Email",
                table: "user",
                newName: "ix_user_organization_id_email");

            migrationBuilder.RenameIndex(
                name: "IX_user_GoogleId",
                table: "user",
                newName: "ix_user_google_id");

            migrationBuilder.RenameIndex(
                name: "IX_user_EmployeeId",
                table: "user",
                newName: "ix_user_employee_id");

            migrationBuilder.RenameIndex(
                name: "IX_user_CreatedByUserId",
                table: "user",
                newName: "ix_user_created_by_user_id");

            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "shifts",
                newName: "notes");

            migrationBuilder.RenameColumn(
                name: "Location",
                table: "shifts",
                newName: "location");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "shifts",
                newName: "date");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "shifts",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "shifts",
                newName: "start_time");

            migrationBuilder.RenameColumn(
                name: "RosterId",
                table: "shifts",
                newName: "roster_id");

            migrationBuilder.RenameColumn(
                name: "RestBreaksDuration",
                table: "shifts",
                newName: "rest_breaks_duration");

            migrationBuilder.RenameColumn(
                name: "PublicHolidayName",
                table: "shifts",
                newName: "public_holiday_name");

            migrationBuilder.RenameColumn(
                name: "OrganizationId",
                table: "shifts",
                newName: "organization_id");

            migrationBuilder.RenameColumn(
                name: "MealBreakDuration",
                table: "shifts",
                newName: "meal_break_duration");

            migrationBuilder.RenameColumn(
                name: "IsPublicHoliday",
                table: "shifts",
                newName: "is_public_holiday");

            migrationBuilder.RenameColumn(
                name: "IsOnCall",
                table: "shifts",
                newName: "is_on_call");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "shifts",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "HasRestBreaks",
                table: "shifts",
                newName: "has_rest_breaks");

            migrationBuilder.RenameColumn(
                name: "HasMealBreak",
                table: "shifts",
                newName: "has_meal_break");

            migrationBuilder.RenameColumn(
                name: "EndTime",
                table: "shifts",
                newName: "end_time");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                table: "shifts",
                newName: "employee_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "shifts",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_Shifts_RosterId_EmployeeId_Date",
                table: "shifts",
                newName: "ix_shifts_roster_id_employee_id_date");

            migrationBuilder.RenameIndex(
                name: "IX_Shifts_OrganizationId_Date",
                table: "shifts",
                newName: "ix_shifts_organization_id_date");

            migrationBuilder.RenameIndex(
                name: "IX_Shifts_EmployeeId_Date",
                table: "shifts",
                newName: "ix_shifts_employee_id_date");

            migrationBuilder.RenameColumn(
                name: "Year",
                table: "rosters",
                newName: "year");

            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "rosters",
                newName: "notes");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "rosters",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "WeekStartDate",
                table: "rosters",
                newName: "week_start_date");

            migrationBuilder.RenameColumn(
                name: "WeekNumber",
                table: "rosters",
                newName: "week_number");

            migrationBuilder.RenameColumn(
                name: "WeekEndDate",
                table: "rosters",
                newName: "week_end_date");

            migrationBuilder.RenameColumn(
                name: "UpdatedByUserId",
                table: "rosters",
                newName: "updated_by_user_id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "rosters",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "TotalShifts",
                table: "rosters",
                newName: "total_shifts");

            migrationBuilder.RenameColumn(
                name: "TotalHours",
                table: "rosters",
                newName: "total_hours");

            migrationBuilder.RenameColumn(
                name: "TotalEmployees",
                table: "rosters",
                newName: "total_employees");

            migrationBuilder.RenameColumn(
                name: "OrganizationId",
                table: "rosters",
                newName: "organization_id");

            migrationBuilder.RenameColumn(
                name: "IsFinalized",
                table: "rosters",
                newName: "is_finalized");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "rosters",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "FinalizedAt",
                table: "rosters",
                newName: "finalized_at");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "rosters",
                newName: "created_by_user_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "rosters",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_Rosters_UpdatedByUserId",
                table: "rosters",
                newName: "ix_rosters_updated_by_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_Rosters_OrganizationId_Year_WeekNumber",
                table: "rosters",
                newName: "ix_rosters_organization_id_year_week_number");

            migrationBuilder.RenameIndex(
                name: "IX_Rosters_OrganizationId_WeekStartDate",
                table: "rosters",
                newName: "ix_rosters_organization_id_week_start_date");

            migrationBuilder.RenameIndex(
                name: "IX_Rosters_CreatedByUserId",
                table: "rosters",
                newName: "ix_rosters_created_by_user_id");

            migrationBuilder.RenameColumn(
                name: "Tax",
                table: "payslips",
                newName: "tax");

            migrationBuilder.RenameColumn(
                name: "Superannuation",
                table: "payslips",
                newName: "superannuation");

            migrationBuilder.RenameColumn(
                name: "Classification",
                table: "payslips",
                newName: "classification");

            migrationBuilder.RenameColumn(
                name: "Allowances",
                table: "payslips",
                newName: "allowances");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "payslips",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedByUserId",
                table: "payslips",
                newName: "updated_by_user_id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "payslips",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "SundayPay",
                table: "payslips",
                newName: "sunday_pay");

            migrationBuilder.RenameColumn(
                name: "SundayHours",
                table: "payslips",
                newName: "sunday_hours");

            migrationBuilder.RenameColumn(
                name: "SourceData",
                table: "payslips",
                newName: "source_data");

            migrationBuilder.RenameColumn(
                name: "SaturdayPay",
                table: "payslips",
                newName: "saturday_pay");

            migrationBuilder.RenameColumn(
                name: "SaturdayHours",
                table: "payslips",
                newName: "saturday_hours");

            migrationBuilder.RenameColumn(
                name: "PublicHolidayPay",
                table: "payslips",
                newName: "public_holiday_pay");

            migrationBuilder.RenameColumn(
                name: "PublicHolidayHours",
                table: "payslips",
                newName: "public_holiday_hours");

            migrationBuilder.RenameColumn(
                name: "PayrollValidationId",
                table: "payslips",
                newName: "payroll_validation_id");

            migrationBuilder.RenameColumn(
                name: "PayPeriodStart",
                table: "payslips",
                newName: "pay_period_start");

            migrationBuilder.RenameColumn(
                name: "PayPeriodEnd",
                table: "payslips",
                newName: "pay_period_end");

            migrationBuilder.RenameColumn(
                name: "PayDate",
                table: "payslips",
                newName: "pay_date");

            migrationBuilder.RenameColumn(
                name: "OvertimePay",
                table: "payslips",
                newName: "overtime_pay");

            migrationBuilder.RenameColumn(
                name: "OvertimeHours",
                table: "payslips",
                newName: "overtime_hours");

            migrationBuilder.RenameColumn(
                name: "OtherDeductions",
                table: "payslips",
                newName: "other_deductions");

            migrationBuilder.RenameColumn(
                name: "OrganizationId",
                table: "payslips",
                newName: "organization_id");

            migrationBuilder.RenameColumn(
                name: "OrdinaryPay",
                table: "payslips",
                newName: "ordinary_pay");

            migrationBuilder.RenameColumn(
                name: "OrdinaryHours",
                table: "payslips",
                newName: "ordinary_hours");

            migrationBuilder.RenameColumn(
                name: "NetPay",
                table: "payslips",
                newName: "net_pay");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "payslips",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "HourlyRate",
                table: "payslips",
                newName: "hourly_rate");

            migrationBuilder.RenameColumn(
                name: "GrossPay",
                table: "payslips",
                newName: "gross_pay");

            migrationBuilder.RenameColumn(
                name: "ExternalReference",
                table: "payslips",
                newName: "external_reference");

            migrationBuilder.RenameColumn(
                name: "EmploymentType",
                table: "payslips",
                newName: "employment_type");

            migrationBuilder.RenameColumn(
                name: "EmployeeNumber",
                table: "payslips",
                newName: "employee_number");

            migrationBuilder.RenameColumn(
                name: "EmployeeName",
                table: "payslips",
                newName: "employee_name");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                table: "payslips",
                newName: "employee_id");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "payslips",
                newName: "created_by_user_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "payslips",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "CasualLoadingPay",
                table: "payslips",
                newName: "casual_loading_pay");

            migrationBuilder.RenameColumn(
                name: "AwardType",
                table: "payslips",
                newName: "award_type");

            migrationBuilder.RenameIndex(
                name: "IX_Payslips_UpdatedByUserId",
                table: "payslips",
                newName: "ix_payslips_updated_by_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_Payslips_PayrollValidationId",
                table: "payslips",
                newName: "ix_payslips_payroll_validation_id");

            migrationBuilder.RenameIndex(
                name: "IX_Payslips_OrganizationId",
                table: "payslips",
                newName: "ix_payslips_organization_id");

            migrationBuilder.RenameIndex(
                name: "IX_Payslips_EmployeeId",
                table: "payslips",
                newName: "ix_payslips_employee_id");

            migrationBuilder.RenameIndex(
                name: "IX_Payslips_CreatedByUserId",
                table: "payslips",
                newName: "ix_payslips_created_by_user_id");

            migrationBuilder.RenameColumn(
                name: "Suburb",
                table: "organization",
                newName: "suburb");

            migrationBuilder.RenameColumn(
                name: "State",
                table: "organization",
                newName: "state");

            migrationBuilder.RenameColumn(
                name: "Postcode",
                table: "organization",
                newName: "postcode");

            migrationBuilder.RenameColumn(
                name: "ABN",
                table: "organization",
                newName: "abn");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "organization",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedByUserId",
                table: "organization",
                newName: "updated_by_user_id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "organization",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "SubscriptionTier",
                table: "organization",
                newName: "subscription_tier");

            migrationBuilder.RenameColumn(
                name: "SubscriptionStartDate",
                table: "organization",
                newName: "subscription_start_date");

            migrationBuilder.RenameColumn(
                name: "SubscriptionEndDate",
                table: "organization",
                newName: "subscription_end_date");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "organization",
                newName: "phone_number");

            migrationBuilder.RenameColumn(
                name: "LogoUrl",
                table: "organization",
                newName: "logo_url");

            migrationBuilder.RenameColumn(
                name: "IsSubscriptionActive",
                table: "organization",
                newName: "is_subscription_active");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "organization",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "IndustryType",
                table: "organization",
                newName: "industry_type");

            migrationBuilder.RenameColumn(
                name: "CurrentEmployeeCount",
                table: "organization",
                newName: "current_employee_count");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "organization",
                newName: "created_by_user_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "organization",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "ContactEmail",
                table: "organization",
                newName: "contact_email");

            migrationBuilder.RenameColumn(
                name: "CompanyName",
                table: "organization",
                newName: "company_name");

            migrationBuilder.RenameColumn(
                name: "AddressLine2",
                table: "organization",
                newName: "address_line2");

            migrationBuilder.RenameColumn(
                name: "AddressLine1",
                table: "organization",
                newName: "address_line1");

            migrationBuilder.RenameIndex(
                name: "IX_organization_ABN",
                table: "organization",
                newName: "ix_organization_abn");

            migrationBuilder.RenameIndex(
                name: "IX_organization_UpdatedByUserId",
                table: "organization",
                newName: "ix_organization_updated_by_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_organization_CreatedByUserId",
                table: "organization",
                newName: "ix_organization_created_by_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_organization_ContactEmail",
                table: "organization",
                newName: "ix_organization_contact_email");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "employees",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Department",
                table: "employees",
                newName: "department");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "employees",
                newName: "address");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "employees",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedByUserId",
                table: "employees",
                newName: "updated_by_user_id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "employees",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "TaxFileNumber",
                table: "employees",
                newName: "tax_file_number");

            migrationBuilder.RenameColumn(
                name: "SuperannuationMemberNumber",
                table: "employees",
                newName: "superannuation_member_number");

            migrationBuilder.RenameColumn(
                name: "SuperannuationFund",
                table: "employees",
                newName: "superannuation_fund");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "employees",
                newName: "start_date");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "employees",
                newName: "phone_number");

            migrationBuilder.RenameColumn(
                name: "OrganizationId",
                table: "employees",
                newName: "organization_id");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "employees",
                newName: "last_name");

            migrationBuilder.RenameColumn(
                name: "JobTitle",
                table: "employees",
                newName: "job_title");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "employees",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "employees",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "GuaranteedHours",
                table: "employees",
                newName: "guaranteed_hours");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "employees",
                newName: "first_name");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "employees",
                newName: "end_date");

            migrationBuilder.RenameColumn(
                name: "EmploymentType",
                table: "employees",
                newName: "employment_type");

            migrationBuilder.RenameColumn(
                name: "EmployeeNumber",
                table: "employees",
                newName: "employee_number");

            migrationBuilder.RenameColumn(
                name: "DateOfBirth",
                table: "employees",
                newName: "date_of_birth");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "employees",
                newName: "created_by_user_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "employees",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "AwardType",
                table: "employees",
                newName: "award_type");

            migrationBuilder.RenameColumn(
                name: "AwardLevelNumber",
                table: "employees",
                newName: "award_level_number");

            migrationBuilder.RenameIndex(
                name: "IX_Employees_UpdatedByUserId",
                table: "employees",
                newName: "ix_employees_updated_by_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_Employees_OrganizationId_EmployeeNumber",
                table: "employees",
                newName: "ix_employees_organization_id_employee_number");

            migrationBuilder.RenameIndex(
                name: "IX_Employees_OrganizationId_Email",
                table: "employees",
                newName: "ix_employees_organization_id_email");

            migrationBuilder.RenameIndex(
                name: "IX_Employees_CreatedByUserId",
                table: "employees",
                newName: "ix_employees_created_by_user_id");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "roster_validations",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "roster_validations",
                newName: "notes");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "roster_validations",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "WeeklyHoursCheckPerformed",
                table: "roster_validations",
                newName: "weekly_hours_check_performed");

            migrationBuilder.RenameColumn(
                name: "WeekStartDate",
                table: "roster_validations",
                newName: "week_start_date");

            migrationBuilder.RenameColumn(
                name: "WeekEndDate",
                table: "roster_validations",
                newName: "week_end_date");

            migrationBuilder.RenameColumn(
                name: "UpdatedByUserId",
                table: "roster_validations",
                newName: "updated_by_user_id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "roster_validations",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "TotalShifts",
                table: "roster_validations",
                newName: "total_shifts");

            migrationBuilder.RenameColumn(
                name: "TotalIssuesCount",
                table: "roster_validations",
                newName: "total_issues_count");

            migrationBuilder.RenameColumn(
                name: "StartedAt",
                table: "roster_validations",
                newName: "started_at");

            migrationBuilder.RenameColumn(
                name: "RosterId",
                table: "roster_validations",
                newName: "roster_id");

            migrationBuilder.RenameColumn(
                name: "RestPeriodCheckPerformed",
                table: "roster_validations",
                newName: "rest_period_check_performed");

            migrationBuilder.RenameColumn(
                name: "PassedShifts",
                table: "roster_validations",
                newName: "passed_shifts");

            migrationBuilder.RenameColumn(
                name: "OrganizationId",
                table: "roster_validations",
                newName: "organization_id");

            migrationBuilder.RenameColumn(
                name: "MinimumShiftHoursCheckPerformed",
                table: "roster_validations",
                newName: "minimum_shift_hours_check_performed");

            migrationBuilder.RenameColumn(
                name: "MealBreakCheckPerformed",
                table: "roster_validations",
                newName: "meal_break_check_performed");

            migrationBuilder.RenameColumn(
                name: "MaxConsecutiveDaysCheckPerformed",
                table: "roster_validations",
                newName: "max_consecutive_days_check_performed");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "roster_validations",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "FailedShifts",
                table: "roster_validations",
                newName: "failed_shifts");

            migrationBuilder.RenameColumn(
                name: "CriticalIssuesCount",
                table: "roster_validations",
                newName: "critical_issues_count");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "roster_validations",
                newName: "created_by_user_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "roster_validations",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "CompletedAt",
                table: "roster_validations",
                newName: "completed_at");

            migrationBuilder.RenameColumn(
                name: "AffectedEmployees",
                table: "roster_validations",
                newName: "affected_employees");

            migrationBuilder.RenameIndex(
                name: "IX_RosterValidations_UpdatedByUserId",
                table: "roster_validations",
                newName: "ix_roster_validations_updated_by_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_RosterValidations_RosterId",
                table: "roster_validations",
                newName: "ix_roster_validations_roster_id");

            migrationBuilder.RenameIndex(
                name: "IX_RosterValidations_OrganizationId_Status",
                table: "roster_validations",
                newName: "ix_roster_validations_organization_id_status");

            migrationBuilder.RenameIndex(
                name: "IX_RosterValidations_CreatedByUserId",
                table: "roster_validations",
                newName: "ix_roster_validations_created_by_user_id");

            migrationBuilder.RenameColumn(
                name: "Severity",
                table: "roster_issues",
                newName: "severity");

            migrationBuilder.RenameColumn(
                name: "Recommendation",
                table: "roster_issues",
                newName: "recommendation");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "roster_issues",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "roster_issues",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "WaiverReason",
                table: "roster_issues",
                newName: "waiver_reason");

            migrationBuilder.RenameColumn(
                name: "WaivedByUserId",
                table: "roster_issues",
                newName: "waived_by_user_id");

            migrationBuilder.RenameColumn(
                name: "WaivedAt",
                table: "roster_issues",
                newName: "waived_at");

            migrationBuilder.RenameColumn(
                name: "ShiftId",
                table: "roster_issues",
                newName: "shift_id");

            migrationBuilder.RenameColumn(
                name: "RosterValidationId",
                table: "roster_issues",
                newName: "roster_validation_id");

            migrationBuilder.RenameColumn(
                name: "ResolvedByUserId",
                table: "roster_issues",
                newName: "resolved_by_user_id");

            migrationBuilder.RenameColumn(
                name: "ResolvedAt",
                table: "roster_issues",
                newName: "resolved_at");

            migrationBuilder.RenameColumn(
                name: "ResolutionNotes",
                table: "roster_issues",
                newName: "resolution_notes");

            migrationBuilder.RenameColumn(
                name: "OrganizationId",
                table: "roster_issues",
                newName: "organization_id");

            migrationBuilder.RenameColumn(
                name: "IsWaived",
                table: "roster_issues",
                newName: "is_waived");

            migrationBuilder.RenameColumn(
                name: "IsResolved",
                table: "roster_issues",
                newName: "is_resolved");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "roster_issues",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "ExpectedValue",
                table: "roster_issues",
                newName: "expected_value");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                table: "roster_issues",
                newName: "employee_id");

            migrationBuilder.RenameColumn(
                name: "DetailedExplanation",
                table: "roster_issues",
                newName: "detailed_explanation");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "roster_issues",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "CheckType",
                table: "roster_issues",
                newName: "check_type");

            migrationBuilder.RenameColumn(
                name: "AffectedShiftsCount",
                table: "roster_issues",
                newName: "affected_shifts_count");

            migrationBuilder.RenameColumn(
                name: "AffectedDates",
                table: "roster_issues",
                newName: "affected_dates");

            migrationBuilder.RenameColumn(
                name: "ActualValue",
                table: "roster_issues",
                newName: "actual_value");

            migrationBuilder.RenameIndex(
                name: "IX_RosterIssues_WaivedByUserId",
                table: "roster_issues",
                newName: "ix_roster_issues_waived_by_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_RosterIssues_ShiftId",
                table: "roster_issues",
                newName: "ix_roster_issues_shift_id");

            migrationBuilder.RenameIndex(
                name: "IX_RosterIssues_RosterValidationId_Severity",
                table: "roster_issues",
                newName: "ix_roster_issues_roster_validation_id_severity");

            migrationBuilder.RenameIndex(
                name: "IX_RosterIssues_RosterValidationId_EmployeeId",
                table: "roster_issues",
                newName: "ix_roster_issues_roster_validation_id_employee_id");

            migrationBuilder.RenameIndex(
                name: "IX_RosterIssues_ResolvedByUserId",
                table: "roster_issues",
                newName: "ix_roster_issues_resolved_by_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_RosterIssues_OrganizationId",
                table: "roster_issues",
                newName: "ix_roster_issues_organization_id");

            migrationBuilder.RenameIndex(
                name: "IX_RosterIssues_EmployeeId",
                table: "roster_issues",
                newName: "ix_roster_issues_employee_id");

            migrationBuilder.RenameIndex(
                name: "IX_RosterIssues_CheckType",
                table: "roster_issues",
                newName: "ix_roster_issues_check_type");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "payroll_validations",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "payroll_validations",
                newName: "notes");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "payroll_validations",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedByUserId",
                table: "payroll_validations",
                newName: "updated_by_user_id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "payroll_validations",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "TotalPayslips",
                table: "payroll_validations",
                newName: "total_payslips");

            migrationBuilder.RenameColumn(
                name: "TotalIssuesCount",
                table: "payroll_validations",
                newName: "total_issues_count");

            migrationBuilder.RenameColumn(
                name: "SuperannuationCheckPerformed",
                table: "payroll_validations",
                newName: "superannuation_check_performed");

            migrationBuilder.RenameColumn(
                name: "StartedAt",
                table: "payroll_validations",
                newName: "started_at");

            migrationBuilder.RenameColumn(
                name: "STPCheckPerformed",
                table: "payroll_validations",
                newName: "stp_check_performed");

            migrationBuilder.RenameColumn(
                name: "PenaltyRateCheckPerformed",
                table: "payroll_validations",
                newName: "penalty_rate_check_performed");

            migrationBuilder.RenameColumn(
                name: "PayPeriodStart",
                table: "payroll_validations",
                newName: "pay_period_start");

            migrationBuilder.RenameColumn(
                name: "PayPeriodEnd",
                table: "payroll_validations",
                newName: "pay_period_end");

            migrationBuilder.RenameColumn(
                name: "PassedCount",
                table: "payroll_validations",
                newName: "passed_count");

            migrationBuilder.RenameColumn(
                name: "OrganizationId",
                table: "payroll_validations",
                newName: "organization_id");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "payroll_validations",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "FilePath",
                table: "payroll_validations",
                newName: "file_path");

            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "payroll_validations",
                newName: "file_name");

            migrationBuilder.RenameColumn(
                name: "FailedCount",
                table: "payroll_validations",
                newName: "failed_count");

            migrationBuilder.RenameColumn(
                name: "CriticalIssuesCount",
                table: "payroll_validations",
                newName: "critical_issues_count");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "payroll_validations",
                newName: "created_by_user_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "payroll_validations",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "CompletedAt",
                table: "payroll_validations",
                newName: "completed_at");

            migrationBuilder.RenameColumn(
                name: "CasualLoadingCheckPerformed",
                table: "payroll_validations",
                newName: "casual_loading_check_performed");

            migrationBuilder.RenameColumn(
                name: "BaseRateCheckPerformed",
                table: "payroll_validations",
                newName: "base_rate_check_performed");

            migrationBuilder.RenameIndex(
                name: "IX_PayrollValidations_UpdatedByUserId",
                table: "payroll_validations",
                newName: "ix_payroll_validations_updated_by_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_PayrollValidations_OrganizationId",
                table: "payroll_validations",
                newName: "ix_payroll_validations_organization_id");

            migrationBuilder.RenameIndex(
                name: "IX_PayrollValidations_CreatedByUserId",
                table: "payroll_validations",
                newName: "ix_payroll_validations_created_by_user_id");

            migrationBuilder.RenameColumn(
                name: "Severity",
                table: "payroll_issues",
                newName: "severity");

            migrationBuilder.RenameColumn(
                name: "Recommendation",
                table: "payroll_issues",
                newName: "recommendation");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "payroll_issues",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "payroll_issues",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UnitType",
                table: "payroll_issues",
                newName: "unit_type");

            migrationBuilder.RenameColumn(
                name: "ResolvedByUserId",
                table: "payroll_issues",
                newName: "resolved_by_user_id");

            migrationBuilder.RenameColumn(
                name: "ResolvedAt",
                table: "payroll_issues",
                newName: "resolved_at");

            migrationBuilder.RenameColumn(
                name: "ResolutionNotes",
                table: "payroll_issues",
                newName: "resolution_notes");

            migrationBuilder.RenameColumn(
                name: "PayslipId",
                table: "payroll_issues",
                newName: "payslip_id");

            migrationBuilder.RenameColumn(
                name: "PayrollValidationId",
                table: "payroll_issues",
                newName: "payroll_validation_id");

            migrationBuilder.RenameColumn(
                name: "OrganizationId",
                table: "payroll_issues",
                newName: "organization_id");

            migrationBuilder.RenameColumn(
                name: "IsResolved",
                table: "payroll_issues",
                newName: "is_resolved");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "payroll_issues",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "ExpectedValue",
                table: "payroll_issues",
                newName: "expected_value");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                table: "payroll_issues",
                newName: "employee_id");

            migrationBuilder.RenameColumn(
                name: "DetailedExplanation",
                table: "payroll_issues",
                newName: "detailed_explanation");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "payroll_issues",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "ContextLabel",
                table: "payroll_issues",
                newName: "context_label");

            migrationBuilder.RenameColumn(
                name: "CheckType",
                table: "payroll_issues",
                newName: "check_type");

            migrationBuilder.RenameColumn(
                name: "AffectedUnits",
                table: "payroll_issues",
                newName: "affected_units");

            migrationBuilder.RenameColumn(
                name: "ActualValue",
                table: "payroll_issues",
                newName: "actual_value");

            migrationBuilder.RenameIndex(
                name: "IX_PayrollIssues_ResolvedByUserId",
                table: "payroll_issues",
                newName: "ix_payroll_issues_resolved_by_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_PayrollIssues_PayslipId",
                table: "payroll_issues",
                newName: "ix_payroll_issues_payslip_id");

            migrationBuilder.RenameIndex(
                name: "IX_PayrollIssues_PayrollValidationId",
                table: "payroll_issues",
                newName: "ix_payroll_issues_payroll_validation_id");

            migrationBuilder.RenameIndex(
                name: "IX_PayrollIssues_OrganizationId",
                table: "payroll_issues",
                newName: "ix_payroll_issues_organization_id");

            migrationBuilder.RenameIndex(
                name: "IX_PayrollIssues_EmployeeId",
                table: "payroll_issues",
                newName: "ix_payroll_issues_employee_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "organization_award",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "OrganizationId",
                table: "organization_award",
                newName: "organization_id");

            migrationBuilder.RenameColumn(
                name: "IsPrimary",
                table: "organization_award",
                newName: "is_primary");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "organization_award",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "EmployeeCount",
                table: "organization_award",
                newName: "employee_count");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "organization_award",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "AwardType",
                table: "organization_award",
                newName: "award_type");

            migrationBuilder.RenameColumn(
                name: "AddedAt",
                table: "organization_award",
                newName: "added_at");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationAwards_OrganizationId_IsPrimary",
                table: "organization_award",
                newName: "ix_organization_award_organization_id_is_primary");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationAwards_OrganizationId_AwardType",
                table: "organization_award",
                newName: "ix_organization_award_organization_id_award_type");

            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "document",
                newName: "notes");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "document",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UploadedFileSize",
                table: "document",
                newName: "uploaded_file_size");

            migrationBuilder.RenameColumn(
                name: "UploadedFilePath",
                table: "document",
                newName: "uploaded_file_path");

            migrationBuilder.RenameColumn(
                name: "UploadedFileName",
                table: "document",
                newName: "uploaded_file_name");

            migrationBuilder.RenameColumn(
                name: "UpdatedByUserId",
                table: "document",
                newName: "updated_by_user_id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "document",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "ProvidedMethod",
                table: "document",
                newName: "provided_method");

            migrationBuilder.RenameColumn(
                name: "ProvidedAt",
                table: "document",
                newName: "provided_at");

            migrationBuilder.RenameColumn(
                name: "OrganizationId",
                table: "document",
                newName: "organization_id");

            migrationBuilder.RenameColumn(
                name: "IsProvided",
                table: "document",
                newName: "is_provided");

            migrationBuilder.RenameColumn(
                name: "IsLegallyRequired",
                table: "document",
                newName: "is_legally_required");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "document",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                table: "document",
                newName: "employee_id");

            migrationBuilder.RenameColumn(
                name: "DocumentType",
                table: "document",
                newName: "document_type");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "document",
                newName: "created_by_user_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "document",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "ComplianceDeadline",
                table: "document",
                newName: "compliance_deadline");

            migrationBuilder.RenameIndex(
                name: "IX_Documents_UpdatedByUserId",
                table: "document",
                newName: "ix_document_updated_by_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_Documents_OrganizationId_DocumentType",
                table: "document",
                newName: "ix_document_organization_id_document_type");

            migrationBuilder.RenameIndex(
                name: "IX_Documents_IsProvided",
                table: "document",
                newName: "ix_document_is_provided");

            migrationBuilder.RenameIndex(
                name: "IX_Documents_EmployeeId_DocumentType",
                table: "document",
                newName: "ix_document_employee_id_document_type");

            migrationBuilder.RenameIndex(
                name: "IX_Documents_CreatedByUserId",
                table: "document",
                newName: "ix_document_created_by_user_id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "award",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "award",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "award",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "SundayPenaltyRate",
                table: "award",
                newName: "sunday_penalty_rate");

            migrationBuilder.RenameColumn(
                name: "SaturdayPenaltyRate",
                table: "award",
                newName: "saturday_penalty_rate");

            migrationBuilder.RenameColumn(
                name: "PublicHolidayPenaltyRate",
                table: "award",
                newName: "public_holiday_penalty_rate");

            migrationBuilder.RenameColumn(
                name: "OrdinaryWeeklyHours",
                table: "award",
                newName: "ordinary_weekly_hours");

            migrationBuilder.RenameColumn(
                name: "MinimumShiftHours",
                table: "award",
                newName: "minimum_shift_hours");

            migrationBuilder.RenameColumn(
                name: "MinimumRestPeriodHours",
                table: "award",
                newName: "minimum_rest_period_hours");

            migrationBuilder.RenameColumn(
                name: "MealBreakThresholdHours",
                table: "award",
                newName: "meal_break_threshold_hours");

            migrationBuilder.RenameColumn(
                name: "MealBreakMinutes",
                table: "award",
                newName: "meal_break_minutes");

            migrationBuilder.RenameColumn(
                name: "MaxConsecutiveDays",
                table: "award",
                newName: "max_consecutive_days");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "award",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "award",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "CasualLoadingRate",
                table: "award",
                newName: "casual_loading_rate");

            migrationBuilder.RenameColumn(
                name: "AwardType",
                table: "award",
                newName: "award_type");

            migrationBuilder.RenameColumn(
                name: "AwardCode",
                table: "award",
                newName: "award_code");

            migrationBuilder.RenameIndex(
                name: "IX_Awards_AwardType",
                table: "award",
                newName: "ix_award_award_type");

            migrationBuilder.RenameIndex(
                name: "IX_Awards_AwardCode",
                table: "award",
                newName: "ix_award_award_code");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "award_level",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "award_level",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "PartTimeHourlyRate",
                table: "award_level",
                newName: "part_time_hourly_rate");

            migrationBuilder.RenameColumn(
                name: "LevelNumber",
                table: "award_level",
                newName: "level_number");

            migrationBuilder.RenameColumn(
                name: "LevelName",
                table: "award_level",
                newName: "level_name");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "award_level",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "award_level",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "FullTimeHourlyRate",
                table: "award_level",
                newName: "full_time_hourly_rate");

            migrationBuilder.RenameColumn(
                name: "EffectiveTo",
                table: "award_level",
                newName: "effective_to");

            migrationBuilder.RenameColumn(
                name: "EffectiveFrom",
                table: "award_level",
                newName: "effective_from");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "award_level",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "CasualHourlyRate",
                table: "award_level",
                newName: "casual_hourly_rate");

            migrationBuilder.RenameColumn(
                name: "AwardId",
                table: "award_level",
                newName: "award_id");

            migrationBuilder.RenameIndex(
                name: "IX_AwardLevels_AwardId_LevelNumber_IsActive",
                table: "award_level",
                newName: "ix_award_level_award_id_level_number_is_active");

            migrationBuilder.RenameIndex(
                name: "IX_AwardLevels_AwardId_LevelNumber_EffectiveFrom",
                table: "award_level",
                newName: "ix_award_level_award_id_level_number_effective_from");

            migrationBuilder.AddPrimaryKey(
                name: "pk_user",
                table: "user",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_shifts",
                table: "shifts",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_rosters",
                table: "rosters",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_payslips",
                table: "payslips",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_organization",
                table: "organization",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_employees",
                table: "employees",
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
                name: "pk_payroll_validations",
                table: "payroll_validations",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_payroll_issues",
                table: "payroll_issues",
                column: "id");

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
                name: "fk_payroll_issues_employees_employee_id",
                table: "payroll_issues",
                column: "employee_id",
                principalTable: "employees",
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
                name: "fk_payroll_issues_payroll_validations_payroll_validation_id",
                table: "payroll_issues",
                column: "payroll_validation_id",
                principalTable: "payroll_validations",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_payroll_issues_payslips_payslip_id",
                table: "payroll_issues",
                column: "payslip_id",
                principalTable: "payslips",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

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
                name: "fk_payslips_employees_employee_id",
                table: "payslips",
                column: "employee_id",
                principalTable: "employees",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_payslips_organizations_organization_id",
                table: "payslips",
                column: "organization_id",
                principalTable: "organization",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_payslips_payroll_validations_payroll_validation_id",
                table: "payslips",
                column: "payroll_validation_id",
                principalTable: "payroll_validations",
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
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

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
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_rosters_users_updated_by_user_id",
                table: "rosters",
                column: "updated_by_user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

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
                name: "fk_shifts_rosters_roster_id",
                table: "shifts",
                column: "roster_id",
                principalTable: "rosters",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

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
    }
}
