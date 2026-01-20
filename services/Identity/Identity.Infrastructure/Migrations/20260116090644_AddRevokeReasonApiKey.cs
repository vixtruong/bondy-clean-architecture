using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRevokeReasonApiKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "revoke_reason",
                table: "api_keys",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "idx_api_keys_revoke_reason",
                table: "api_keys",
                column: "revoke_reason");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_api_keys_revoke_reason",
                table: "api_keys");

            migrationBuilder.DropColumn(
                name: "revoke_reason",
                table: "api_keys");
        }
    }
}
