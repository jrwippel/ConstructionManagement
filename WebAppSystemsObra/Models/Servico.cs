using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebAppSystems.Models;

namespace WebAppSystems.Models
{
    public class Servico
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public int ObraId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public decimal PercentualIncidencia { get; set; }
        public List<Execucao> Execucao { get; set; } = new List<Execucao>();
    }
}
