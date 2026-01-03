using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FairWorkly.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePayrollIssueSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "check_type",
                table: "payroll_issues");

            migrationBuilder.DropColumn(
                name: "description",
                table: "payroll_issues");

            migrationBuilder.AddColumn<string>(
                name: "category_type",
                table: "payroll_issues",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "impact_amount",
                table: "payroll_issues",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "warning_message",
                table: "payroll_issues",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "category_type",
                table: "payroll_issues");

            migrationBuilder.DropColumn(
                name: "impact_amount",
                table: "payroll_issues");

            migrationBuilder.DropColumn(
                name: "warning_message",
                table: "payroll_issues");

            migrationBuilder.AddColumn<string>(
                name: "check_type",
                table: "payroll_issues",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "payroll_issues",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }
    }
}
