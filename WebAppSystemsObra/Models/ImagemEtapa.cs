using System.ComponentModel.DataAnnotations.Schema;
using WebAppSystems.Models;
namespace WebAppSystems.Models
{
    public class ImagemEtapa
    {
        public int Id { get; set; }
        public int EtapaId { get; set; }  // 🔗 Relacionamento com a etapa
        public int NumeroEtapaId { get; set; }  
        public string NomeArquivo { get; set; }  // Nome do arquivo
        public string UrlImagem { get; set; }  // URL no Azure Blob Storage
        public DateTime DataUpload { get; set; } = DateTime.Now;
        public string Descricao { get; set; }


        [ForeignKey("EtapaId")]
        public Etapa Etapa { get; set; }

    }


}
