using WebAppSystems.Models;


namespace WebAppSystemsObra.Services
{
    public interface IClienteRepository
    {
        void Adicionar(Cliente cliente);
        Cliente BuscarPorId(int id);
        IEnumerable<Cliente> ListarTodas();
        void Atualizar(Cliente cliente);
        void Remover(int id);
    }

}

