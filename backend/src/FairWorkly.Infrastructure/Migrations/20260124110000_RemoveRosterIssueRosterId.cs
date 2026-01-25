using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FairWorkly.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRosterIssueRosterId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the composite index on (RosterId, EmployeeId)
            migrationBuilder.DropIndex(
                name: "ix_roster_issues_roster_id_employee_id",
                table: "roster_issues");

            // Drop the FK constraint from RosterIssue to Roster
            migrationBuilder.DropForeignKey(
                name: "fk_roster_issues_rosters_roster_id",
                table: "roster_issues");

            // Drop the RosterId column
            migrationBuilder.DropColumn(
                name: "roster_id",
                table: "roster_issues");

            // Create new index on (RosterValidationId, EmployeeId) to replace the dropped one
            migrationBuilder.CreateIndex(
                name: "ix_roster_issues_roster_validation_id_employee_id",
                table: "roster_issues",
                columns: new[] { "roster_validation_id", "employee_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the new index
            migrationBuilder.DropIndex(
                name: "ix_roster_issues_roster_validation_id_employee_id",
                table: "roster_issues");

            // Add back the RosterId column
            migrationBuilder.AddColumn<Guid>(
                name: "roster_id",
                table: "roster_issues",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            // Recreate the FK constraint
            migrationBuilder.AddForeignKey(
                name: "fk_roster_issues_rosters_roster_id",
                table: "roster_issues",
                column: "roster_id",
                principalTable: "rosters",
                principalColumn: "id",
                onDelete: ReferentialAction.NoAction);

            // Recreate the composite index on (RosterId, EmployeeId)
            migrationBuilder.CreateIndex(
                name: "ix_roster_issues_roster_id_employee_id",
                table: "roster_issues",
                columns: new[] { "roster_id", "employee_id" });
        }
    }
}
