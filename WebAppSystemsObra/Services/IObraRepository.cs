using WebAppSystems.Models;

namespace WebAppSystemsObra.Services
{
    public interface IObraRepository
    {
        void Adicionar(Obra obra);
        Obra BuscarPorId(int id);
        IEnumerable<Obra> ListarTodas();
        void Atualizar(Obra obra);
        void Remover(int id);
    }

}

