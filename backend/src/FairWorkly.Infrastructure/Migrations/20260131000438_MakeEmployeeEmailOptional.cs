using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FairWorkly.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeEmployeeEmailOptional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_employees_organization_id_email",
                table: "employees");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "employees",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.CreateIndex(
                name: "ix_employees_organization_id_email",
                table: "employees",
                columns: new[] { "organization_id", "email" },
                unique: true,
                filter: "email IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_employees_organization_id_email",
                table: "employees");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "employees",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_employees_organization_id_email",
                table: "employees",
                columns: new[] { "organization_id", "email" },
                unique: true);
        }
    }
}
