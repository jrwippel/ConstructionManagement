namespace WebAppSystems.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public List<Obra> Obra { get; set; } = new List<Obra>();
    }
}
