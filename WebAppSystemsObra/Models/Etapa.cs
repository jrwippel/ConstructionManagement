using WebAppSystems.Models;

namespace WebAppSystems.Models
{
    public class Etapa
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public int ObraId { get; set; }
        public int qtde { get; set; }
        //public List<Execucao> Execucao { get; set; } = new List<Execucao>();
    }
}
