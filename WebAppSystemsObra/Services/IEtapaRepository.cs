using WebAppSystems.Models;

namespace WebAppSystemsObra.Services
{
    public interface IEtapaRepository
    {
        void Adicionar(Etapa etapa);
        Etapa BuscarPorId(int id);
        IEnumerable<Etapa> ListarTodas();
        void Atualizar(Etapa etapa);
        void Remover(int id);     
        IEnumerable<Etapa> ListarPorObra(int obraId); // Adicionado

    }

}

