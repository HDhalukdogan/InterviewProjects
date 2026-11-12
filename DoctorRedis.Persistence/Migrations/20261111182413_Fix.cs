using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoctorRedis.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationLogs_Doctors_DoctorId",
                table: "NotificationLogs");

            migrationBuilder.DropIndex(
                name: "IX_NotificationLogs_DoctorId",
                table: "NotificationLogs");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_SlotId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "DoctorId",
                table: "NotificationLogs");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_SlotId",
                table: "Appointments",
                column: "SlotId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Appointments_SlotId",
                table: "Appointments");

            migrationBuilder.AddColumn<int>(
                name: "DoctorId",
                table: "NotificationLogs",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NotificationLogs_DoctorId",
                table: "NotificationLogs",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_SlotId",
                table: "Appointments",
                column: "SlotId");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationLogs_Doctors_DoctorId",
                table: "NotificationLogs",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id");
        }
    }
}
