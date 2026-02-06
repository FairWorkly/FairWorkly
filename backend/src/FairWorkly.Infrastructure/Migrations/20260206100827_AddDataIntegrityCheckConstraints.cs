using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FairWorkly.Infrastructure.Migrations
{
    /// <summary>
    /// Adds database-level CHECK constraints for data integrity:
    /// 1. Enum value validation (reject invalid/unset enum values)
    /// 2. Required string validation (reject empty strings)
    /// 3. Non-negative value validation (counts, hours, rates)
    /// 4. Date range validation (start &lt;= end)
    /// 5. Schema changes: password_hash nullable, severity int→string
    /// </summary>
    public partial class AddDataIntegrityCheckConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ═══════════════════════════════════════════════════════════════
            // SCHEMA CHANGES (detected by EF Core)
            // ═══════════════════════════════════════════════════════════════

            migrationBuilder.AlterColumn<string>(
                name: "password_hash",
                table: "users",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            // Convert NULL to empty string BEFORE changing to NOT NULL
            migrationBuilder.Sql("UPDATE roster_validations SET executed_check_types = '' WHERE executed_check_types IS NULL;");

            migrationBuilder.AlterColumn<string>(
                name: "executed_check_types",
                table: "roster_validations",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "severity",
                table: "roster_issues",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            // Convert existing int severity values to string names
            // Must happen AFTER AlterColumn but BEFORE CHECK constraint
            migrationBuilder.Sql(@"
                UPDATE roster_issues SET severity = CASE severity
                    WHEN '1' THEN 'Info'
                    WHEN '2' THEN 'Warning'
                    WHEN '3' THEN 'Error'
                    WHEN '4' THEN 'Critical'
                    ELSE severity
                END;
            ");

            // Convert NULL to empty string BEFORE changing to NOT NULL
            migrationBuilder.Sql("UPDATE roster_issues SET affected_dates = '' WHERE affected_dates IS NULL;");

            migrationBuilder.AlterColumn<string>(
                name: "affected_dates",
                table: "roster_issues",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            // ═══════════════════════════════════════════════════════════════
            // 1. ENUM CHECK CONSTRAINTS
            // Ensures enum columns only contain valid defined values
            // ═══════════════════════════════════════════════════════════════

            // UserRole: Admin=1, Manager=2
            migrationBuilder.Sql(@"
                ALTER TABLE users ADD CONSTRAINT chk_user_role_valid
                    CHECK (role IN (1, 2));
            ");

            // SubscriptionTier: Tier1=1, Tier2=2, Tier3=3
            migrationBuilder.Sql(@"
                ALTER TABLE organizations ADD CONSTRAINT chk_organization_subscription_tier_valid
                    CHECK (subscription_tier IN (1, 2, 3));
            ");

            // AustralianState: VIC=1, NSW=2, QLD=3, SA=4, WA=5, TAS=6, ACT=7, NT=8
            migrationBuilder.Sql(@"
                ALTER TABLE organizations ADD CONSTRAINT chk_organization_state_valid
                    CHECK (state IN (1, 2, 3, 4, 5, 6, 7, 8));
            ");

            // EmploymentType: FullTime=1, PartTime=2, Casual=3, FixedTerm=4
            migrationBuilder.Sql(@"
                ALTER TABLE employees ADD CONSTRAINT chk_employee_employment_type_valid
                    CHECK (employment_type IN (1, 2, 3, 4));
            ");

            // AwardType: Hospitality=1, Retail=2, Clerks=3
            migrationBuilder.Sql(@"
                ALTER TABLE employees ADD CONSTRAINT chk_employee_award_type_valid
                    CHECK (award_type IN (1, 2, 3));
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE awards ADD CONSTRAINT chk_award_award_type_valid
                    CHECK (award_type IN (1, 2, 3));
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE payslips ADD CONSTRAINT chk_payslip_award_type_valid
                    CHECK (award_type IN (1, 2, 3));
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE payslips ADD CONSTRAINT chk_payslip_employment_type_valid
                    CHECK (employment_type IN (1, 2, 3, 4));
            ");

            // ValidationStatus: Pending=1, InProgress=2, Passed=3, Failed=4
            migrationBuilder.Sql(@"
                ALTER TABLE roster_validations ADD CONSTRAINT chk_roster_validation_status_valid
                    CHECK (status IN (1, 2, 3, 4));
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE payroll_validations ADD CONSTRAINT chk_payroll_validation_status_valid
                    CHECK (status IN (1, 2, 3, 4));
            ");

            // IssueSeverity: stored as string (HasConversion<string>())
            migrationBuilder.Sql(@"
                ALTER TABLE roster_issues ADD CONSTRAINT chk_roster_issue_severity_valid
                    CHECK (severity IN ('Info', 'Warning', 'Error', 'Critical'));
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE payroll_issues ADD CONSTRAINT chk_payroll_issue_severity_valid
                    CHECK (severity IN ('Info', 'Warning', 'Error', 'Critical'));
            ");

            // RosterCheckType is stored as string (HasConversion<string>())
            migrationBuilder.Sql(@"
                ALTER TABLE roster_issues ADD CONSTRAINT chk_roster_issue_check_type_valid
                    CHECK (check_type IN ('DataQuality', 'MinimumShiftHours', 'MealBreak', 'RestPeriodBetweenShifts', 'WeeklyHoursLimit', 'MaximumConsecutiveDays'));
            ");

            // IssueCategory is stored as string (HasConversion<string>())
            migrationBuilder.Sql(@"
                ALTER TABLE payroll_issues ADD CONSTRAINT chk_payroll_issue_category_valid
                    CHECK (category_type IN ('PreValidation', 'BaseRate', 'PenaltyRate', 'CasualLoading', 'Superannuation', 'STPCompliance'));
            ");

            // DocumentType: FWIS=1, SeparationCert=2, CasualConversion=3, OfferLetter=4
            migrationBuilder.Sql(@"
                ALTER TABLE documents ADD CONSTRAINT chk_document_type_valid
                    CHECK (document_type IN (1, 2, 3, 4));
            ");

            // ═══════════════════════════════════════════════════════════════
            // 2. REQUIRED STRING CHECK CONSTRAINTS
            // Ensures required string columns are not empty
            // ═══════════════════════════════════════════════════════════════

            // Users
            migrationBuilder.Sql(@"
                ALTER TABLE users ADD CONSTRAINT chk_user_email_not_empty
                    CHECK (LENGTH(TRIM(email)) > 0);
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE users ADD CONSTRAINT chk_user_first_name_not_empty
                    CHECK (LENGTH(TRIM(first_name)) > 0);
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE users ADD CONSTRAINT chk_user_last_name_not_empty
                    CHECK (LENGTH(TRIM(last_name)) > 0);
            ");
            // At least one authentication credential must be present AND non-empty (Password or OAuth)
            migrationBuilder.Sql(@"
                ALTER TABLE users ADD CONSTRAINT chk_user_has_auth_credential
                    CHECK (
                        (password_hash IS NOT NULL AND LENGTH(TRIM(password_hash)) > 0)
                        OR (google_id IS NOT NULL AND LENGTH(TRIM(google_id)) > 0)
                    );
            ");

            // Organizations
            migrationBuilder.Sql(@"
                ALTER TABLE organizations ADD CONSTRAINT chk_organization_company_name_not_empty
                    CHECK (LENGTH(TRIM(company_name)) > 0);
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE organizations ADD CONSTRAINT chk_organization_abn_not_empty
                    CHECK (LENGTH(TRIM(abn)) > 0);
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE organizations ADD CONSTRAINT chk_organization_industry_type_not_empty
                    CHECK (LENGTH(TRIM(industry_type)) > 0);
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE organizations ADD CONSTRAINT chk_organization_address_line1_not_empty
                    CHECK (LENGTH(TRIM(address_line1)) > 0);
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE organizations ADD CONSTRAINT chk_organization_suburb_not_empty
                    CHECK (LENGTH(TRIM(suburb)) > 0);
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE organizations ADD CONSTRAINT chk_organization_postcode_not_empty
                    CHECK (LENGTH(TRIM(postcode)) > 0);
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE organizations ADD CONSTRAINT chk_organization_contact_email_not_empty
                    CHECK (LENGTH(TRIM(contact_email)) > 0);
            ");

            // Employees
            migrationBuilder.Sql(@"
                ALTER TABLE employees ADD CONSTRAINT chk_employee_first_name_not_empty
                    CHECK (LENGTH(TRIM(first_name)) > 0);
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE employees ADD CONSTRAINT chk_employee_last_name_not_empty
                    CHECK (LENGTH(TRIM(last_name)) > 0);
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE employees ADD CONSTRAINT chk_employee_job_title_not_empty
                    CHECK (LENGTH(TRIM(job_title)) > 0);
            ");

            // Awards
            migrationBuilder.Sql(@"
                ALTER TABLE awards ADD CONSTRAINT chk_award_name_not_empty
                    CHECK (LENGTH(TRIM(name)) > 0);
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE awards ADD CONSTRAINT chk_award_code_not_empty
                    CHECK (LENGTH(TRIM(award_code)) > 0);
            ");

            // AwardLevels
            migrationBuilder.Sql(@"
                ALTER TABLE award_levels ADD CONSTRAINT chk_award_level_name_not_empty
                    CHECK (LENGTH(TRIM(level_name)) > 0);
            ");

            // Payslips
            migrationBuilder.Sql(@"
                ALTER TABLE payslips ADD CONSTRAINT chk_payslip_employee_name_not_empty
                    CHECK (LENGTH(TRIM(employee_name)) > 0);
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE payslips ADD CONSTRAINT chk_payslip_employee_number_not_empty
                    CHECK (LENGTH(TRIM(employee_number)) > 0);
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE payslips ADD CONSTRAINT chk_payslip_classification_not_empty
                    CHECK (LENGTH(TRIM(classification)) > 0);
            ");

            // RosterIssues
            migrationBuilder.Sql(@"
                ALTER TABLE roster_issues ADD CONSTRAINT chk_roster_issue_description_not_empty
                    CHECK (LENGTH(TRIM(description)) > 0);
            ");

            // ═══════════════════════════════════════════════════════════════
            // 3. NON-NEGATIVE CHECK CONSTRAINTS
            // ═══════════════════════════════════════════════════════════════

            // Shifts - break durations
            migrationBuilder.Sql(@"
                ALTER TABLE shifts ADD CONSTRAINT chk_shift_meal_break_duration_non_negative
                    CHECK (meal_break_duration IS NULL OR meal_break_duration >= 0);
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE shifts ADD CONSTRAINT chk_shift_rest_breaks_duration_non_negative
                    CHECK (rest_breaks_duration IS NULL OR rest_breaks_duration >= 0);
            ");

            // Rosters - statistics
            migrationBuilder.Sql(@"
                ALTER TABLE rosters ADD CONSTRAINT chk_roster_total_shifts_non_negative
                    CHECK (total_shifts >= 0);
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE rosters ADD CONSTRAINT chk_roster_total_hours_non_negative
                    CHECK (total_hours >= 0);
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE rosters ADD CONSTRAINT chk_roster_total_employees_non_negative
                    CHECK (total_employees >= 0);
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE rosters ADD CONSTRAINT chk_roster_week_number_valid
                    CHECK (week_number >= 1 AND week_number <= 53);
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE rosters ADD CONSTRAINT chk_roster_year_valid
                    CHECK (year >= 2000);
            ");

            // RosterValidations - statistics
            migrationBuilder.Sql(@"
                ALTER TABLE roster_validations ADD CONSTRAINT chk_roster_validation_total_shifts_non_negative
                    CHECK (total_shifts >= 0);
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE roster_validations ADD CONSTRAINT chk_roster_validation_passed_shifts_non_negative
                    CHECK (passed_shifts >= 0);
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE roster_validations ADD CONSTRAINT chk_roster_validation_failed_shifts_non_negative
                    CHECK (failed_shifts >= 0);
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE roster_validations ADD CONSTRAINT chk_roster_validation_total_issues_non_negative
                    CHECK (total_issues_count >= 0);
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE roster_validations ADD CONSTRAINT chk_roster_validation_critical_issues_non_negative
                    CHECK (critical_issues_count >= 0);
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE roster_validations ADD CONSTRAINT chk_roster_validation_affected_employees_non_negative
                    CHECK (affected_employees >= 0);
            ");

            // PayrollValidations - statistics
            migrationBuilder.Sql(@"
                ALTER TABLE payroll_validations ADD CONSTRAINT chk_payroll_validation_total_payslips_non_negative
                    CHECK (total_payslips >= 0);
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE payroll_validations ADD CONSTRAINT chk_payroll_validation_passed_count_non_negative
                    CHECK (passed_count >= 0);
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE payroll_validations ADD CONSTRAINT chk_payroll_validation_failed_count_non_negative
                    CHECK (failed_count >= 0);
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE payroll_validations ADD CONSTRAINT chk_payroll_validation_total_issues_non_negative
                    CHECK (total_issues_count >= 0);
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE payroll_validations ADD CONSTRAINT chk_payroll_validation_critical_issues_non_negative
                    CHECK (critical_issues_count >= 0);
            ");

            // Awards - configuration values
            migrationBuilder.Sql(@"
                ALTER TABLE awards ADD CONSTRAINT chk_award_saturday_penalty_rate_non_negative
                    CHECK (saturday_penalty_rate >= 0);
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE awards ADD CONSTRAINT chk_award_sunday_penalty_rate_non_negative
                    CHECK (sunday_penalty_rate >= 0);
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE awards ADD CONSTRAINT chk_award_public_holiday_penalty_rate_non_negative
                    CHECK (public_holiday_penalty_rate >= 0);
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE awards ADD CONSTRAINT chk_award_casual_loading_rate_non_negative
                    CHECK (casual_loading_rate >= 0);
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE awards ADD CONSTRAINT chk_award_minimum_shift_hours_non_negative
                    CHECK (minimum_shift_hours >= 0);
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE awards ADD CONSTRAINT chk_award_max_consecutive_days_non_negative
                    CHECK (max_consecutive_days >= 0);
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE awards ADD CONSTRAINT chk_award_meal_break_threshold_hours_non_negative
                    CHECK (meal_break_threshold_hours >= 0);
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE awards ADD CONSTRAINT chk_award_meal_break_minutes_non_negative
                    CHECK (meal_break_minutes >= 0);
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE awards ADD CONSTRAINT chk_award_minimum_rest_period_hours_non_negative
                    CHECK (minimum_rest_period_hours >= 0);
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE awards ADD CONSTRAINT chk_award_ordinary_weekly_hours_non_negative
                    CHECK (ordinary_weekly_hours >= 0);
            ");

            // AwardLevels - rates and level
            migrationBuilder.Sql(@"
                ALTER TABLE award_levels ADD CONSTRAINT chk_award_level_number_positive
                    CHECK (level_number >= 1);
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE award_levels ADD CONSTRAINT chk_award_level_full_time_rate_non_negative
                    CHECK (full_time_hourly_rate >= 0);
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE award_levels ADD CONSTRAINT chk_award_level_part_time_rate_non_negative
                    CHECK (part_time_hourly_rate >= 0);
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE award_levels ADD CONSTRAINT chk_award_level_casual_rate_non_negative
                    CHECK (casual_hourly_rate >= 0);
            ");

            // Employees
            migrationBuilder.Sql(@"
                ALTER TABLE employees ADD CONSTRAINT chk_employee_guaranteed_hours_non_negative
                    CHECK (guaranteed_hours IS NULL OR guaranteed_hours >= 0);
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE employees ADD CONSTRAINT chk_employee_award_level_number_positive
                    CHECK (award_level_number >= 1);
            ");

            // Organizations
            migrationBuilder.Sql(@"
                ALTER TABLE organizations ADD CONSTRAINT chk_organization_current_employee_count_non_negative
                    CHECK (current_employee_count >= 0);
            ");

            // OrganizationAwards
            migrationBuilder.Sql(@"
                ALTER TABLE organization_awards ADD CONSTRAINT chk_organization_award_employee_count_non_negative
                    CHECK (employee_count >= 0);
            ");

            // Payslips - hours and rate (amounts can be negative for corrections)
            migrationBuilder.Sql(@"
                ALTER TABLE payslips ADD CONSTRAINT chk_payslip_hourly_rate_non_negative
                    CHECK (hourly_rate >= 0);
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE payslips ADD CONSTRAINT chk_payslip_ordinary_hours_non_negative
                    CHECK (ordinary_hours >= 0);
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE payslips ADD CONSTRAINT chk_payslip_saturday_hours_non_negative
                    CHECK (saturday_hours >= 0);
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE payslips ADD CONSTRAINT chk_payslip_sunday_hours_non_negative
                    CHECK (sunday_hours >= 0);
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE payslips ADD CONSTRAINT chk_payslip_public_holiday_hours_non_negative
                    CHECK (public_holiday_hours >= 0);
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE payslips ADD CONSTRAINT chk_payslip_overtime_hours_non_negative
                    CHECK (overtime_hours IS NULL OR overtime_hours >= 0);
            ");

            // ═══════════════════════════════════════════════════════════════
            // 4. DATE RANGE CHECK CONSTRAINTS
            // Ensures start dates are before or equal to end dates
            // ═══════════════════════════════════════════════════════════════

            // Rosters: WeekStartDate <= WeekEndDate
            migrationBuilder.Sql(@"
                ALTER TABLE rosters ADD CONSTRAINT chk_roster_week_date_range
                    CHECK (week_start_date <= week_end_date);
            ");

            // RosterValidations: WeekStartDate <= WeekEndDate
            migrationBuilder.Sql(@"
                ALTER TABLE roster_validations ADD CONSTRAINT chk_roster_validation_week_date_range
                    CHECK (week_start_date <= week_end_date);
            ");

            // Employees: StartDate <= EndDate (when EndDate is set)
            migrationBuilder.Sql(@"
                ALTER TABLE employees ADD CONSTRAINT chk_employee_employment_date_range
                    CHECK (end_date IS NULL OR start_date <= end_date);
            ");

            // Payslips: PayPeriodStart <= PayPeriodEnd
            migrationBuilder.Sql(@"
                ALTER TABLE payslips ADD CONSTRAINT chk_payslip_pay_period_range
                    CHECK (pay_period_start <= pay_period_end);
            ");

            // PayrollValidations: PayPeriodStart <= PayPeriodEnd
            migrationBuilder.Sql(@"
                ALTER TABLE payroll_validations ADD CONSTRAINT chk_payroll_validation_pay_period_range
                    CHECK (pay_period_start <= pay_period_end);
            ");

            // AwardLevels: EffectiveFrom <= EffectiveTo (when EffectiveTo is set)
            migrationBuilder.Sql(@"
                ALTER TABLE award_levels ADD CONSTRAINT chk_award_level_effective_date_range
                    CHECK (effective_to IS NULL OR effective_from <= effective_to);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // ═══════════════════════════════════════════════════════════════
            // Drop all constraints in reverse order
            // ═══════════════════════════════════════════════════════════════

            // 4. Date range constraints
            migrationBuilder.Sql("ALTER TABLE award_levels DROP CONSTRAINT IF EXISTS chk_award_level_effective_date_range;");
            migrationBuilder.Sql("ALTER TABLE payroll_validations DROP CONSTRAINT IF EXISTS chk_payroll_validation_pay_period_range;");
            migrationBuilder.Sql("ALTER TABLE payslips DROP CONSTRAINT IF EXISTS chk_payslip_pay_period_range;");
            migrationBuilder.Sql("ALTER TABLE employees DROP CONSTRAINT IF EXISTS chk_employee_employment_date_range;");
            migrationBuilder.Sql("ALTER TABLE roster_validations DROP CONSTRAINT IF EXISTS chk_roster_validation_week_date_range;");
            migrationBuilder.Sql("ALTER TABLE rosters DROP CONSTRAINT IF EXISTS chk_roster_week_date_range;");

            // 3. Non-negative constraints
            migrationBuilder.Sql("ALTER TABLE payslips DROP CONSTRAINT IF EXISTS chk_payslip_overtime_hours_non_negative;");
            migrationBuilder.Sql("ALTER TABLE payslips DROP CONSTRAINT IF EXISTS chk_payslip_public_holiday_hours_non_negative;");
            migrationBuilder.Sql("ALTER TABLE payslips DROP CONSTRAINT IF EXISTS chk_payslip_sunday_hours_non_negative;");
            migrationBuilder.Sql("ALTER TABLE payslips DROP CONSTRAINT IF EXISTS chk_payslip_saturday_hours_non_negative;");
            migrationBuilder.Sql("ALTER TABLE payslips DROP CONSTRAINT IF EXISTS chk_payslip_ordinary_hours_non_negative;");
            migrationBuilder.Sql("ALTER TABLE payslips DROP CONSTRAINT IF EXISTS chk_payslip_hourly_rate_non_negative;");
            migrationBuilder.Sql("ALTER TABLE organization_awards DROP CONSTRAINT IF EXISTS chk_organization_award_employee_count_non_negative;");
            migrationBuilder.Sql("ALTER TABLE organizations DROP CONSTRAINT IF EXISTS chk_organization_current_employee_count_non_negative;");
            migrationBuilder.Sql("ALTER TABLE employees DROP CONSTRAINT IF EXISTS chk_employee_award_level_number_positive;");
            migrationBuilder.Sql("ALTER TABLE employees DROP CONSTRAINT IF EXISTS chk_employee_guaranteed_hours_non_negative;");
            migrationBuilder.Sql("ALTER TABLE award_levels DROP CONSTRAINT IF EXISTS chk_award_level_casual_rate_non_negative;");
            migrationBuilder.Sql("ALTER TABLE award_levels DROP CONSTRAINT IF EXISTS chk_award_level_part_time_rate_non_negative;");
            migrationBuilder.Sql("ALTER TABLE award_levels DROP CONSTRAINT IF EXISTS chk_award_level_full_time_rate_non_negative;");
            migrationBuilder.Sql("ALTER TABLE award_levels DROP CONSTRAINT IF EXISTS chk_award_level_number_positive;");
            migrationBuilder.Sql("ALTER TABLE awards DROP CONSTRAINT IF EXISTS chk_award_ordinary_weekly_hours_non_negative;");
            migrationBuilder.Sql("ALTER TABLE awards DROP CONSTRAINT IF EXISTS chk_award_minimum_rest_period_hours_non_negative;");
            migrationBuilder.Sql("ALTER TABLE awards DROP CONSTRAINT IF EXISTS chk_award_meal_break_minutes_non_negative;");
            migrationBuilder.Sql("ALTER TABLE awards DROP CONSTRAINT IF EXISTS chk_award_meal_break_threshold_hours_non_negative;");
            migrationBuilder.Sql("ALTER TABLE awards DROP CONSTRAINT IF EXISTS chk_award_max_consecutive_days_non_negative;");
            migrationBuilder.Sql("ALTER TABLE awards DROP CONSTRAINT IF EXISTS chk_award_minimum_shift_hours_non_negative;");
            migrationBuilder.Sql("ALTER TABLE awards DROP CONSTRAINT IF EXISTS chk_award_casual_loading_rate_non_negative;");
            migrationBuilder.Sql("ALTER TABLE awards DROP CONSTRAINT IF EXISTS chk_award_public_holiday_penalty_rate_non_negative;");
            migrationBuilder.Sql("ALTER TABLE awards DROP CONSTRAINT IF EXISTS chk_award_sunday_penalty_rate_non_negative;");
            migrationBuilder.Sql("ALTER TABLE awards DROP CONSTRAINT IF EXISTS chk_award_saturday_penalty_rate_non_negative;");
            migrationBuilder.Sql("ALTER TABLE payroll_validations DROP CONSTRAINT IF EXISTS chk_payroll_validation_critical_issues_non_negative;");
            migrationBuilder.Sql("ALTER TABLE payroll_validations DROP CONSTRAINT IF EXISTS chk_payroll_validation_total_issues_non_negative;");
            migrationBuilder.Sql("ALTER TABLE payroll_validations DROP CONSTRAINT IF EXISTS chk_payroll_validation_failed_count_non_negative;");
            migrationBuilder.Sql("ALTER TABLE payroll_validations DROP CONSTRAINT IF EXISTS chk_payroll_validation_passed_count_non_negative;");
            migrationBuilder.Sql("ALTER TABLE payroll_validations DROP CONSTRAINT IF EXISTS chk_payroll_validation_total_payslips_non_negative;");
            migrationBuilder.Sql("ALTER TABLE roster_validations DROP CONSTRAINT IF EXISTS chk_roster_validation_affected_employees_non_negative;");
            migrationBuilder.Sql("ALTER TABLE roster_validations DROP CONSTRAINT IF EXISTS chk_roster_validation_critical_issues_non_negative;");
            migrationBuilder.Sql("ALTER TABLE roster_validations DROP CONSTRAINT IF EXISTS chk_roster_validation_total_issues_non_negative;");
            migrationBuilder.Sql("ALTER TABLE roster_validations DROP CONSTRAINT IF EXISTS chk_roster_validation_failed_shifts_non_negative;");
            migrationBuilder.Sql("ALTER TABLE roster_validations DROP CONSTRAINT IF EXISTS chk_roster_validation_passed_shifts_non_negative;");
            migrationBuilder.Sql("ALTER TABLE roster_validations DROP CONSTRAINT IF EXISTS chk_roster_validation_total_shifts_non_negative;");
            migrationBuilder.Sql("ALTER TABLE rosters DROP CONSTRAINT IF EXISTS chk_roster_year_valid;");
            migrationBuilder.Sql("ALTER TABLE rosters DROP CONSTRAINT IF EXISTS chk_roster_week_number_valid;");
            migrationBuilder.Sql("ALTER TABLE rosters DROP CONSTRAINT IF EXISTS chk_roster_total_employees_non_negative;");
            migrationBuilder.Sql("ALTER TABLE rosters DROP CONSTRAINT IF EXISTS chk_roster_total_hours_non_negative;");
            migrationBuilder.Sql("ALTER TABLE rosters DROP CONSTRAINT IF EXISTS chk_roster_total_shifts_non_negative;");
            migrationBuilder.Sql("ALTER TABLE shifts DROP CONSTRAINT IF EXISTS chk_shift_rest_breaks_duration_non_negative;");
            migrationBuilder.Sql("ALTER TABLE shifts DROP CONSTRAINT IF EXISTS chk_shift_meal_break_duration_non_negative;");

            // 2. Required string constraints
            migrationBuilder.Sql("ALTER TABLE roster_issues DROP CONSTRAINT IF EXISTS chk_roster_issue_description_not_empty;");
            migrationBuilder.Sql("ALTER TABLE payslips DROP CONSTRAINT IF EXISTS chk_payslip_classification_not_empty;");
            migrationBuilder.Sql("ALTER TABLE payslips DROP CONSTRAINT IF EXISTS chk_payslip_employee_number_not_empty;");
            migrationBuilder.Sql("ALTER TABLE payslips DROP CONSTRAINT IF EXISTS chk_payslip_employee_name_not_empty;");
            migrationBuilder.Sql("ALTER TABLE award_levels DROP CONSTRAINT IF EXISTS chk_award_level_name_not_empty;");
            migrationBuilder.Sql("ALTER TABLE awards DROP CONSTRAINT IF EXISTS chk_award_code_not_empty;");
            migrationBuilder.Sql("ALTER TABLE awards DROP CONSTRAINT IF EXISTS chk_award_name_not_empty;");
            migrationBuilder.Sql("ALTER TABLE employees DROP CONSTRAINT IF EXISTS chk_employee_job_title_not_empty;");
            migrationBuilder.Sql("ALTER TABLE employees DROP CONSTRAINT IF EXISTS chk_employee_last_name_not_empty;");
            migrationBuilder.Sql("ALTER TABLE employees DROP CONSTRAINT IF EXISTS chk_employee_first_name_not_empty;");
            migrationBuilder.Sql("ALTER TABLE organizations DROP CONSTRAINT IF EXISTS chk_organization_contact_email_not_empty;");
            migrationBuilder.Sql("ALTER TABLE organizations DROP CONSTRAINT IF EXISTS chk_organization_postcode_not_empty;");
            migrationBuilder.Sql("ALTER TABLE organizations DROP CONSTRAINT IF EXISTS chk_organization_suburb_not_empty;");
            migrationBuilder.Sql("ALTER TABLE organizations DROP CONSTRAINT IF EXISTS chk_organization_address_line1_not_empty;");
            migrationBuilder.Sql("ALTER TABLE organizations DROP CONSTRAINT IF EXISTS chk_organization_industry_type_not_empty;");
            migrationBuilder.Sql("ALTER TABLE organizations DROP CONSTRAINT IF EXISTS chk_organization_abn_not_empty;");
            migrationBuilder.Sql("ALTER TABLE organizations DROP CONSTRAINT IF EXISTS chk_organization_company_name_not_empty;");
            migrationBuilder.Sql("ALTER TABLE users DROP CONSTRAINT IF EXISTS chk_user_has_auth_credential;");
            migrationBuilder.Sql("ALTER TABLE users DROP CONSTRAINT IF EXISTS chk_user_last_name_not_empty;");
            migrationBuilder.Sql("ALTER TABLE users DROP CONSTRAINT IF EXISTS chk_user_first_name_not_empty;");
            migrationBuilder.Sql("ALTER TABLE users DROP CONSTRAINT IF EXISTS chk_user_email_not_empty;");

            // 1. Enum constraints
            migrationBuilder.Sql("ALTER TABLE documents DROP CONSTRAINT IF EXISTS chk_document_type_valid;");
            migrationBuilder.Sql("ALTER TABLE payroll_issues DROP CONSTRAINT IF EXISTS chk_payroll_issue_category_valid;");
            migrationBuilder.Sql("ALTER TABLE roster_issues DROP CONSTRAINT IF EXISTS chk_roster_issue_check_type_valid;");
            migrationBuilder.Sql("ALTER TABLE payroll_issues DROP CONSTRAINT IF EXISTS chk_payroll_issue_severity_valid;");
            migrationBuilder.Sql("ALTER TABLE roster_issues DROP CONSTRAINT IF EXISTS chk_roster_issue_severity_valid;");
            migrationBuilder.Sql("ALTER TABLE payroll_validations DROP CONSTRAINT IF EXISTS chk_payroll_validation_status_valid;");
            migrationBuilder.Sql("ALTER TABLE roster_validations DROP CONSTRAINT IF EXISTS chk_roster_validation_status_valid;");
            migrationBuilder.Sql("ALTER TABLE payslips DROP CONSTRAINT IF EXISTS chk_payslip_employment_type_valid;");
            migrationBuilder.Sql("ALTER TABLE payslips DROP CONSTRAINT IF EXISTS chk_payslip_award_type_valid;");
            migrationBuilder.Sql("ALTER TABLE awards DROP CONSTRAINT IF EXISTS chk_award_award_type_valid;");
            migrationBuilder.Sql("ALTER TABLE employees DROP CONSTRAINT IF EXISTS chk_employee_award_type_valid;");
            migrationBuilder.Sql("ALTER TABLE employees DROP CONSTRAINT IF EXISTS chk_employee_employment_type_valid;");
            migrationBuilder.Sql("ALTER TABLE organizations DROP CONSTRAINT IF EXISTS chk_organization_state_valid;");
            migrationBuilder.Sql("ALTER TABLE organizations DROP CONSTRAINT IF EXISTS chk_organization_subscription_tier_valid;");
            migrationBuilder.Sql("ALTER TABLE users DROP CONSTRAINT IF EXISTS chk_user_role_valid;");

            // ═══════════════════════════════════════════════════════════════
            // Revert schema changes
            // ═══════════════════════════════════════════════════════════════

            // Convert NULL to empty string BEFORE changing to NOT NULL
            migrationBuilder.Sql("UPDATE users SET password_hash = '' WHERE password_hash IS NULL;");

            migrationBuilder.AlterColumn<string>(
                name: "password_hash",
                table: "users",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "executed_check_types",
                table: "roster_validations",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            // Convert string severity values back to int BEFORE AlterColumn
            migrationBuilder.Sql(@"
                UPDATE roster_issues SET severity = CASE severity
                    WHEN 'Info' THEN '1'
                    WHEN 'Warning' THEN '2'
                    WHEN 'Error' THEN '3'
                    WHEN 'Critical' THEN '4'
                    ELSE severity
                END;
            ");

            migrationBuilder.AlterColumn<int>(
                name: "severity",
                table: "roster_issues",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "affected_dates",
                table: "roster_issues",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);
        }
    }
}
