using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FairWorkly.Infrastructure.Migrations
{
    /// <summary>
    /// Migrates old execution-failure note prefix from "Validation failed: "
    /// to "ExecutionFailure: " so that ValidationFailureMarker can recognise
    /// retryable failures and allow re-runs.
    /// </summary>
    public partial class MigrateExecutionFailureNotePrefix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE roster_validations
                SET notes = 'ExecutionFailure: ' || SUBSTRING(notes FROM 20)
                WHERE notes LIKE 'Validation failed: %'
                  AND status = 4
                  AND NOT is_deleted;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE roster_validations
                SET notes = 'Validation failed: ' || SUBSTRING(notes FROM 19)
                WHERE notes LIKE 'ExecutionFailure: %'
                  AND status = 4
                  AND NOT is_deleted;
            ");
        }
    }
}
