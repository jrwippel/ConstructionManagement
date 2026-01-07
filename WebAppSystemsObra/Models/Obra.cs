using WebAppSystems.Models;

namespace WebAppSystems.Models
{
    public class Obra
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public int ClienteId { get; set; }        
        public List<Etapa> Etapa { get; set; } = new List<Etapa>();
        public List<Servico> Servico { get; set; } = new List<Servico>();
        public List<Execucao> Execucao { get; set; } = new List<Execucao>();
    }
}
