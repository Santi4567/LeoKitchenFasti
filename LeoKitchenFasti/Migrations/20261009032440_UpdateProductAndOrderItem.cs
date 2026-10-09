using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeoKitchenFasti.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductAndOrderItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AreaId",
                table: "Tables",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Station",
                table: "Products",
                type: "longtext",
                nullable: false,
                collation: "utf8mb4_spanish_ci");

            migrationBuilder.AddColumn<decimal>(
                name: "UnitPrice",
                table: "OrderItems",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "Areas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_spanish_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Areas", x => x.Id);
                })
                .Annotation("Relational:Collation", "utf8mb4_spanish_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Tables_AreaId",
                table: "Tables",
                column: "AreaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tables_Areas_AreaId",
                table: "Tables",
                column: "AreaId",
                principalTable: "Areas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tables_Areas_AreaId",
                table: "Tables");

            migrationBuilder.DropTable(
                name: "Areas");

            migrationBuilder.DropIndex(
                name: "IX_Tables_AreaId",
                table: "Tables");

            migrationBuilder.DropColumn(
                name: "AreaId",
                table: "Tables");

            migrationBuilder.DropColumn(
                name: "Station",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                table: "OrderItems");
        }
    }
}
