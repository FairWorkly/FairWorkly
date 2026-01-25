using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FairWorkly.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixRosterEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_roster_issues_rosters_roster_id",
                table: "roster_issues");

            migrationBuilder.DropForeignKey(
                name: "fk_rosters_users_created_by_user_id",
                table: "rosters");

            migrationBuilder.DropForeignKey(
                name: "fk_rosters_users_updated_by_user_id",
                table: "rosters");

            migrationBuilder.DropIndex(
                name: "ix_rosters_organization_id_year_week_number",
                table: "rosters");

            migrationBuilder.CreateIndex(
                name: "ix_rosters_organization_id_year_week_number",
                table: "rosters",
                columns: new[] { "organization_id", "year", "week_number" });

            migrationBuilder.AddForeignKey(
                name: "fk_roster_issues_rosters_roster_id",
                table: "roster_issues",
                column: "roster_id",
                principalTable: "rosters",
                principalColumn: "id",
                onDelete: ReferentialAction.NoAction);

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_roster_issues_rosters_roster_id",
                table: "roster_issues");

            migrationBuilder.DropForeignKey(
                name: "fk_rosters_users_created_by_user_id",
                table: "rosters");

            migrationBuilder.DropForeignKey(
                name: "fk_rosters_users_updated_by_user_id",
                table: "rosters");

            migrationBuilder.DropIndex(
                name: "ix_rosters_organization_id_year_week_number",
                table: "rosters");

            migrationBuilder.CreateIndex(
                name: "ix_rosters_organization_id_year_week_number",
                table: "rosters",
                columns: new[] { "organization_id", "year", "week_number" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_roster_issues_rosters_roster_id",
                table: "roster_issues",
                column: "roster_id",
                principalTable: "rosters",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

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
        }
    }
}
