using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMarket.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNotificationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PersonalNotifications_SystemNotifications_SystemNotificatio~",
                table: "PersonalNotifications");

            migrationBuilder.AlterColumn<int>(
                name: "SystemNotificationId",
                table: "PersonalNotifications",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalNotifications_SystemNotifications_SystemNotificatio~",
                table: "PersonalNotifications",
                column: "SystemNotificationId",
                principalTable: "SystemNotifications",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PersonalNotifications_SystemNotifications_SystemNotificatio~",
                table: "PersonalNotifications");

            migrationBuilder.AlterColumn<int>(
                name: "SystemNotificationId",
                table: "PersonalNotifications",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalNotifications_SystemNotifications_SystemNotificatio~",
                table: "PersonalNotifications",
                column: "SystemNotificationId",
                principalTable: "SystemNotifications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
