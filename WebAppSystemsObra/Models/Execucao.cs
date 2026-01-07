using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebAppSystems.Models
{
    public class Execucao
    {
        public int Id { get; set; }
        public int ObraId { get; set; }
        public int EtapaId { get; set; }
        public int ServicoId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]  
        public decimal PercentualExecucao { get; set; }       

        // Propriedades de navegação
        //public Etapa Etapa { get; set; }  // Adicionado para garantir relacionamento correto
        public Servico Servico { get; set; }
    }
}
