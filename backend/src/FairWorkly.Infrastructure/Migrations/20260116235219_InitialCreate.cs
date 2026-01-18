using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FairWorkly.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "award",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    award_type = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    award_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    saturday_penalty_rate = table.Column<decimal>(type: "numeric(5,4)", precision: 5, scale: 4, nullable: false),
                    sunday_penalty_rate = table.Column<decimal>(type: "numeric(5,4)", precision: 5, scale: 4, nullable: false),
                    public_holiday_penalty_rate = table.Column<decimal>(type: "numeric(5,4)", precision: 5, scale: 4, nullable: false),
                    casual_loading_rate = table.Column<decimal>(type: "numeric(5,4)", precision: 5, scale: 4, nullable: false),
                    minimum_shift_hours = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    max_consecutive_days = table.Column<int>(type: "integer", nullable: false),
                    meal_break_threshold_hours = table.Column<int>(type: "integer", nullable: false),
                    meal_break_minutes = table.Column<int>(type: "integer", nullable: false),
                    minimum_rest_period_hours = table.Column<int>(type: "integer", nullable: false),
                    ordinary_weekly_hours = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_award", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "award_level",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    award_id = table.Column<Guid>(type: "uuid", nullable: false),
                    level_number = table.Column<int>(type: "integer", nullable: false),
                    level_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    full_time_hourly_rate = table.Column<decimal>(type: "numeric(10,4)", precision: 10, scale: 4, nullable: false),
                    part_time_hourly_rate = table.Column<decimal>(type: "numeric(10,4)", precision: 10, scale: 4, nullable: false),
                    casual_hourly_rate = table.Column<decimal>(type: "numeric(10,4)", precision: 10, scale: 4, nullable: false),
                    effective_from = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    effective_to = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_award_level", x => x.id);
                    table.ForeignKey(
                        name: "fk_award_level_award_award_id",
                        column: x => x.award_id,
                        principalTable: "award",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "document",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: true),
                    document_type = table.Column<int>(type: "integer", nullable: false),
                    is_provided = table.Column<bool>(type: "boolean", nullable: false),
                    provided_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    provided_method = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    is_legally_required = table.Column<bool>(type: "boolean", nullable: false),
                    compliance_deadline = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    uploaded_file_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    uploaded_file_path = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    uploaded_file_size = table.Column<long>(type: "bigint", nullable: true),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_document", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "employees",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    phone_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    date_of_birth = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    job_title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    department = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    employment_type = table.Column<int>(type: "integer", nullable: false),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    guaranteed_hours = table.Column<int>(type: "integer", nullable: true),
                    award_type = table.Column<int>(type: "integer", nullable: false),
                    award_level_number = table.Column<int>(type: "integer", nullable: false),
                    employee_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    tax_file_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    superannuation_fund = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    superannuation_member_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_employees", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "organization",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    logo_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    company_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    abn = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    industry_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    address_line1 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    address_line2 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    suburb = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    state = table.Column<int>(type: "integer", nullable: false),
                    postcode = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    contact_email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    phone_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    subscription_tier = table.Column<int>(type: "integer", nullable: false),
                    subscription_start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    subscription_end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_subscription_active = table.Column<bool>(type: "boolean", nullable: false),
                    current_employee_count = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_organization", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "organization_award",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    award_type = table.Column<int>(type: "integer", nullable: false),
                    added_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_primary = table.Column<bool>(type: "boolean", nullable: false),
                    employee_count = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_organization_award", x => x.id);
                    table.ForeignKey(
                        name: "fk_organization_award_organization_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organization",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    phone_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    role = table.Column<int>(type: "integer", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    last_login_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    refresh_token = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    refresh_token_expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    password_reset_token = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    password_reset_token_expiry = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    google_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_employees_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_user_organization_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organization",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_user_user_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_user_user_updated_by_user_id",
                        column: x => x.updated_by_user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "payroll_validations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    pay_period_start = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    pay_period_end = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    file_path = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    file_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    total_payslips = table.Column<int>(type: "integer", nullable: false),
                    passed_count = table.Column<int>(type: "integer", nullable: false),
                    failed_count = table.Column<int>(type: "integer", nullable: false),
                    total_issues_count = table.Column<int>(type: "integer", nullable: false),
                    critical_issues_count = table.Column<int>(type: "integer", nullable: false),
                    base_rate_check_performed = table.Column<bool>(type: "boolean", nullable: false),
                    penalty_rate_check_performed = table.Column<bool>(type: "boolean", nullable: false),
                    casual_loading_check_performed = table.Column<bool>(type: "boolean", nullable: false),
                    superannuation_check_performed = table.Column<bool>(type: "boolean", nullable: false),
                    stp_check_performed = table.Column<bool>(type: "boolean", nullable: false),
                    started_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_payroll_validations", x => x.id);
                    table.ForeignKey(
                        name: "fk_payroll_validations_organization_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organization",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_payroll_validations_user_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalTable: "user",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_payroll_validations_user_updated_by_user_id",
                        column: x => x.updated_by_user_id,
                        principalTable: "user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "roster",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    week_start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    week_end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    week_number = table.Column<int>(type: "integer", nullable: false),
                    year = table.Column<int>(type: "integer", nullable: false),
                    is_finalized = table.Column<bool>(type: "boolean", nullable: false),
                    finalized_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    total_shifts = table.Column<int>(type: "integer", nullable: false),
                    total_hours = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    total_employees = table.Column<int>(type: "integer", nullable: false),
                    roster_validation_id = table.Column<Guid>(type: "uuid", nullable: true),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roster", x => x.id);
                    table.ForeignKey(
                        name: "fk_roster_organization_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organization",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_roster_user_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_roster_user_updated_by_user_id",
                        column: x => x.updated_by_user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "payslips",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    payroll_validation_id = table.Column<Guid>(type: "uuid", nullable: true),
                    pay_period_start = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    pay_period_end = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    pay_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    employee_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    employee_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    employment_type = table.Column<int>(type: "integer", nullable: false),
                    award_type = table.Column<int>(type: "integer", nullable: false),
                    classification = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    hourly_rate = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    ordinary_hours = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    saturday_hours = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    sunday_hours = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    public_holiday_hours = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    overtime_hours = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    ordinary_pay = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    saturday_pay = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    sunday_pay = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    public_holiday_pay = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    overtime_pay = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    allowances = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    casual_loading_pay = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    gross_pay = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    tax = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    superannuation = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    other_deductions = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    net_pay = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    source_data = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    external_reference = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_payslips", x => x.id);
                    table.ForeignKey(
                        name: "fk_payslips_employees_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_payslips_organization_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organization",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_payslips_payroll_validations_payroll_validation_id",
                        column: x => x.payroll_validation_id,
                        principalTable: "payroll_validations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_payslips_user_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalTable: "user",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_payslips_user_updated_by_user_id",
                        column: x => x.updated_by_user_id,
                        principalTable: "user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "roster_validation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    roster_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    week_start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    week_end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    total_shifts = table.Column<int>(type: "integer", nullable: false),
                    passed_shifts = table.Column<int>(type: "integer", nullable: false),
                    failed_shifts = table.Column<int>(type: "integer", nullable: false),
                    total_issues_count = table.Column<int>(type: "integer", nullable: false),
                    critical_issues_count = table.Column<int>(type: "integer", nullable: false),
                    affected_employees = table.Column<int>(type: "integer", nullable: false),
                    minimum_shift_hours_check_performed = table.Column<bool>(type: "boolean", nullable: false),
                    max_consecutive_days_check_performed = table.Column<bool>(type: "boolean", nullable: false),
                    meal_break_check_performed = table.Column<bool>(type: "boolean", nullable: false),
                    rest_period_check_performed = table.Column<bool>(type: "boolean", nullable: false),
                    weekly_hours_check_performed = table.Column<bool>(type: "boolean", nullable: false),
                    started_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roster_validation", x => x.id);
                    table.ForeignKey(
                        name: "fk_roster_validation_organization_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organization",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_roster_validation_roster_roster_id",
                        column: x => x.roster_id,
                        principalTable: "roster",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_roster_validation_user_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_roster_validation_user_updated_by_user_id",
                        column: x => x.updated_by_user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "shift",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    roster_id = table.Column<Guid>(type: "uuid", nullable: false),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    start_time = table.Column<TimeSpan>(type: "interval", nullable: false),
                    end_time = table.Column<TimeSpan>(type: "interval", nullable: false),
                    has_meal_break = table.Column<bool>(type: "boolean", nullable: false),
                    meal_break_duration = table.Column<int>(type: "integer", nullable: true),
                    has_rest_breaks = table.Column<bool>(type: "boolean", nullable: false),
                    rest_breaks_duration = table.Column<int>(type: "integer", nullable: true),
                    is_public_holiday = table.Column<bool>(type: "boolean", nullable: false),
                    public_holiday_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    is_on_call = table.Column<bool>(type: "boolean", nullable: false),
                    location = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_shift", x => x.id);
                    table.ForeignKey(
                        name: "fk_shift_employees_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_shift_organization_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organization",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_shift_roster_roster_id",
                        column: x => x.roster_id,
                        principalTable: "roster",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "payroll_issues",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    payroll_validation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    payslip_id = table.Column<Guid>(type: "uuid", nullable: false),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    check_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    severity = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    detailed_explanation = table.Column<string>(type: "text", nullable: true),
                    recommendation = table.Column<string>(type: "text", nullable: true),
                    expected_value = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    actual_value = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    affected_units = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    unit_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    context_label = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    is_resolved = table.Column<bool>(type: "boolean", nullable: false),
                    resolved_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    resolved_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    resolution_notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_payroll_issues", x => x.id);
                    table.ForeignKey(
                        name: "fk_payroll_issues_employees_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_payroll_issues_organization_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organization",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_payroll_issues_payroll_validations_payroll_validation_id",
                        column: x => x.payroll_validation_id,
                        principalTable: "payroll_validations",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_payroll_issues_payslips_payslip_id",
                        column: x => x.payslip_id,
                        principalTable: "payslips",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_payroll_issues_user_resolved_by_user_id",
                        column: x => x.resolved_by_user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "roster_issue",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    roster_validation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    roster_id = table.Column<Guid>(type: "uuid", nullable: false),
                    shift_id = table.Column<Guid>(type: "uuid", nullable: true),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    check_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    severity = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    detailed_explanation = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    recommendation = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    expected_value = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    actual_value = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    affected_dates = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    affected_shifts_count = table.Column<int>(type: "integer", nullable: true),
                    is_resolved = table.Column<bool>(type: "boolean", nullable: false),
                    resolved_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    resolved_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    resolution_notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    is_waived = table.Column<bool>(type: "boolean", nullable: false),
                    waived_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    waived_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    waiver_reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roster_issue", x => x.id);
                    table.ForeignKey(
                        name: "fk_roster_issue_employees_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_roster_issue_organization_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organization",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_roster_issue_roster_roster_id",
                        column: x => x.roster_id,
                        principalTable: "roster",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_roster_issue_roster_validation_roster_validation_id",
                        column: x => x.roster_validation_id,
                        principalTable: "roster_validation",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_roster_issue_shift_shift_id",
                        column: x => x.shift_id,
                        principalTable: "shift",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_roster_issue_user_resolved_by_user_id",
                        column: x => x.resolved_by_user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_roster_issue_user_waived_by_user_id",
                        column: x => x.waived_by_user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_award_award_code",
                table: "award",
                column: "award_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_award_award_type",
                table: "award",
                column: "award_type",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_award_level_award_id_level_number_effective_from",
                table: "award_level",
                columns: new[] { "award_id", "level_number", "effective_from" });

            migrationBuilder.CreateIndex(
                name: "ix_award_level_award_id_level_number_is_active",
                table: "award_level",
                columns: new[] { "award_id", "level_number", "is_active" });

            migrationBuilder.CreateIndex(
                name: "ix_document_created_by_user_id",
                table: "document",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_document_employee_id_document_type",
                table: "document",
                columns: new[] { "employee_id", "document_type" });

            migrationBuilder.CreateIndex(
                name: "ix_document_is_provided",
                table: "document",
                column: "is_provided");

            migrationBuilder.CreateIndex(
                name: "ix_document_organization_id_document_type",
                table: "document",
                columns: new[] { "organization_id", "document_type" });

            migrationBuilder.CreateIndex(
                name: "ix_document_updated_by_user_id",
                table: "document",
                column: "updated_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_employees_created_by_user_id",
                table: "employees",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_employees_organization_id_email",
                table: "employees",
                columns: new[] { "organization_id", "email" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_employees_organization_id_employee_number",
                table: "employees",
                columns: new[] { "organization_id", "employee_number" },
                unique: true,
                filter: "employee_number IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_employees_updated_by_user_id",
                table: "employees",
                column: "updated_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_organization_abn",
                table: "organization",
                column: "abn",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_organization_contact_email",
                table: "organization",
                column: "contact_email");

            migrationBuilder.CreateIndex(
                name: "ix_organization_created_by_user_id",
                table: "organization",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_organization_updated_by_user_id",
                table: "organization",
                column: "updated_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_organization_award_organization_id_award_type",
                table: "organization_award",
                columns: new[] { "organization_id", "award_type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_organization_award_organization_id_is_primary",
                table: "organization_award",
                columns: new[] { "organization_id", "is_primary" });

            migrationBuilder.CreateIndex(
                name: "ix_payroll_issues_employee_id",
                table: "payroll_issues",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "ix_payroll_issues_organization_id",
                table: "payroll_issues",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_payroll_issues_payroll_validation_id",
                table: "payroll_issues",
                column: "payroll_validation_id");

            migrationBuilder.CreateIndex(
                name: "ix_payroll_issues_payslip_id",
                table: "payroll_issues",
                column: "payslip_id");

            migrationBuilder.CreateIndex(
                name: "ix_payroll_issues_resolved_by_user_id",
                table: "payroll_issues",
                column: "resolved_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_payroll_validations_created_by_user_id",
                table: "payroll_validations",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_payroll_validations_organization_id",
                table: "payroll_validations",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_payroll_validations_updated_by_user_id",
                table: "payroll_validations",
                column: "updated_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_payslips_created_by_user_id",
                table: "payslips",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_payslips_employee_id",
                table: "payslips",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "ix_payslips_organization_id",
                table: "payslips",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_payslips_payroll_validation_id",
                table: "payslips",
                column: "payroll_validation_id");

            migrationBuilder.CreateIndex(
                name: "ix_payslips_updated_by_user_id",
                table: "payslips",
                column: "updated_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_roster_created_by_user_id",
                table: "roster",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_roster_organization_id_week_start_date",
                table: "roster",
                columns: new[] { "organization_id", "week_start_date" });

            migrationBuilder.CreateIndex(
                name: "ix_roster_organization_id_year_week_number",
                table: "roster",
                columns: new[] { "organization_id", "year", "week_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_roster_updated_by_user_id",
                table: "roster",
                column: "updated_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_roster_issue_check_type",
                table: "roster_issue",
                column: "check_type");

            migrationBuilder.CreateIndex(
                name: "ix_roster_issue_employee_id",
                table: "roster_issue",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "ix_roster_issue_organization_id",
                table: "roster_issue",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_roster_issue_resolved_by_user_id",
                table: "roster_issue",
                column: "resolved_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_roster_issue_roster_id_employee_id",
                table: "roster_issue",
                columns: new[] { "roster_id", "employee_id" });

            migrationBuilder.CreateIndex(
                name: "ix_roster_issue_roster_validation_id_severity",
                table: "roster_issue",
                columns: new[] { "roster_validation_id", "severity" });

            migrationBuilder.CreateIndex(
                name: "ix_roster_issue_shift_id",
                table: "roster_issue",
                column: "shift_id");

            migrationBuilder.CreateIndex(
                name: "ix_roster_issue_waived_by_user_id",
                table: "roster_issue",
                column: "waived_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_roster_validation_created_by_user_id",
                table: "roster_validation",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_roster_validation_organization_id_status",
                table: "roster_validation",
                columns: new[] { "organization_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_roster_validation_roster_id",
                table: "roster_validation",
                column: "roster_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_roster_validation_updated_by_user_id",
                table: "roster_validation",
                column: "updated_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_shift_employee_id_date",
                table: "shift",
                columns: new[] { "employee_id", "date" });

            migrationBuilder.CreateIndex(
                name: "ix_shift_organization_id_date",
                table: "shift",
                columns: new[] { "organization_id", "date" });

            migrationBuilder.CreateIndex(
                name: "ix_shift_roster_id_employee_id_date",
                table: "shift",
                columns: new[] { "roster_id", "employee_id", "date" });

            migrationBuilder.CreateIndex(
                name: "ix_user_created_by_user_id",
                table: "user",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_employee_id",
                table: "user",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_google_id",
                table: "user",
                column: "google_id",
                unique: true,
                filter: "google_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_user_organization_id_email",
                table: "user",
                columns: new[] { "organization_id", "email" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_updated_by_user_id",
                table: "user",
                column: "updated_by_user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_document_employees_employee_id",
                table: "document",
                column: "employee_id",
                principalTable: "employees",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_user_employees_employee_id",
                table: "user");

            migrationBuilder.DropForeignKey(
                name: "fk_user_organization_organization_id",
                table: "user");

            migrationBuilder.DropTable(
                name: "award_level");

            migrationBuilder.DropTable(
                name: "document");

            migrationBuilder.DropTable(
                name: "organization_award");

            migrationBuilder.DropTable(
                name: "payroll_issues");

            migrationBuilder.DropTable(
                name: "roster_issue");

            migrationBuilder.DropTable(
                name: "award");

            migrationBuilder.DropTable(
                name: "payslips");

            migrationBuilder.DropTable(
                name: "roster_validation");

            migrationBuilder.DropTable(
                name: "shift");

            migrationBuilder.DropTable(
                name: "payroll_validations");

            migrationBuilder.DropTable(
                name: "roster");

            migrationBuilder.DropTable(
                name: "employees");

            migrationBuilder.DropTable(
                name: "organization");

            migrationBuilder.DropTable(
                name: "user");
        }
    }
}
