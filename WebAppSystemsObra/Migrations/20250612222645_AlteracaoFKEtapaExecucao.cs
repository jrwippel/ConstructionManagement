using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAppSystemsObra.Migrations
{
    public partial class AlteracaoFKEtapaExecucao : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Execucaos_Etapas_EtapaId",
                table: "Execucaos");

            migrationBuilder.DropIndex(
                name: "IX_Execucaos_EtapaId",
                table: "Execucaos");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Execucaos_EtapaId",
                table: "Execucaos",
                column: "EtapaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Execucaos_Etapas_EtapaId",
                table: "Execucaos",
                column: "EtapaId",
                principalTable: "Etapas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
