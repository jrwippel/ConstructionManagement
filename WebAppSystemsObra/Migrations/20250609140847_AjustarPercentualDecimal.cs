using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAppSystemsObra.Migrations
{
    public partial class AjustarPercentualDecimal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "PercentualIncidencia",
                table: "Servicos",
                type: "decimal(18,2)",
                nullable: false);
        }


        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
