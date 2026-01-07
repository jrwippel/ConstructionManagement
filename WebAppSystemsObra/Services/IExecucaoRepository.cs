using WebAppSystems.Models;

namespace WebAppSystemsObra.Services
{
    public interface IExecucaoRepository
    {
        void Adicionar(Execucao execucao);
        Execucao BuscarPorId(int id);
        IEnumerable<Execucao> ListarTodas();
        void Atualizar(Execucao execucao);
        void Remover(int id);

        // Novo método para filtrar execuções por obra e etapa
        IEnumerable<Execucao> ListarPorObraEtapa(int obraId, int etapaId);

        IEnumerable<Execucao> ListarPorObra(int obraId);


    }
}


