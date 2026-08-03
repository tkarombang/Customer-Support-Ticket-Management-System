using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTicketSequenceToYearly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TicketSequences",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "TicketSequences",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TicketSequences_Year",
                table: "TicketSequences",
                column: "Year",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TicketSequences_Year",
                table: "TicketSequences");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "TicketSequences");

            migrationBuilder.InsertData(
                table: "TicketSequences",
                columns: new[] { "Id", "LastSequence" },
                values: new object[] { 1, 0 });
        }
    }
}
