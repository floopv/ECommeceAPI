using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommece_API.Migrations
{
    /// <inheritdoc />
    public partial class AddCanResetPasswordToModelApplicationUserOTP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanResetPassword",
                table: "ApplicationUserOTPs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanResetPassword",
                table: "ApplicationUserOTPs");
        }
    }
}
