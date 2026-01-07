using WebAppSystems.Models;

namespace WebAppSystemsObra.Services
{
    public interface IServicoRepository
    {
        void Adicionar(Servico servico);
        Servico BuscarPorId(int id);
        IEnumerable<Servico> ListarTodas();
        void Atualizar(Servico servico);
        void Remover(int id);
        IEnumerable<Servico> ListarPorObra(int obraId);
    }

}

