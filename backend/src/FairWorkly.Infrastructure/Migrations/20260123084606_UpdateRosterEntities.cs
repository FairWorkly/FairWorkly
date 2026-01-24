using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FairWorkly.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRosterEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_roster_issues_roster_validations_roster_validation_id",
                table: "roster_issues");

            migrationBuilder.DropForeignKey(
                name: "fk_roster_issues_rosters_roster_id",
                table: "roster_issues");

            migrationBuilder.DropForeignKey(
                name: "fk_shifts_employees_employee_id1",
                table: "shifts");

            migrationBuilder.DropForeignKey(
                name: "fk_shifts_rosters_roster_id",
                table: "shifts");

            migrationBuilder.DropIndex(
                name: "ix_user_email",
                table: "user");

            migrationBuilder.DropIndex(
                name: "ix_shifts_employee_id1",
                table: "shifts");

            migrationBuilder.DropColumn(
                name: "employee_id1",
                table: "shifts");

            migrationBuilder.DropColumn(
                name: "roster_validation_id",
                table: "rosters");

            migrationBuilder.AddForeignKey(
                name: "fk_roster_issues_roster_validations_roster_validation_id",
                table: "roster_issues",
                column: "roster_validation_id",
                principalTable: "roster_validations",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_roster_issues_rosters_roster_id",
                table: "roster_issues",
                column: "roster_id",
                principalTable: "rosters",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_shifts_rosters_roster_id",
                table: "shifts",
                column: "roster_id",
                principalTable: "rosters",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_roster_issues_roster_validations_roster_validation_id",
                table: "roster_issues");

            migrationBuilder.DropForeignKey(
                name: "fk_roster_issues_rosters_roster_id",
                table: "roster_issues");

            migrationBuilder.DropForeignKey(
                name: "fk_shifts_rosters_roster_id",
                table: "shifts");

            migrationBuilder.AddColumn<Guid>(
                name: "employee_id1",
                table: "shifts",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "roster_validation_id",
                table: "rosters",
                type: "uuid",
                nullable: true);

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
                name: "fk_shifts_employees_employee_id1",
                table: "shifts",
                column: "employee_id1",
                principalTable: "employees",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_shifts_rosters_roster_id",
                table: "shifts",
                column: "roster_id",
                principalTable: "rosters",
                principalColumn: "id");
        }
    }
}
