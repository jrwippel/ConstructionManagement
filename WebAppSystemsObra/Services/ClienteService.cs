using WebAppSystems.Models;
using WebAppSystemsObra.Repository;

namespace WebAppSystemsObra.Services
{
    public class ClienteService
    {
        private readonly IClienteRepository _clienteRepository;

        public ClienteService(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public void AdicionarCliente(Cliente cliente)
        {
            _clienteRepository.Adicionar(cliente);
        }

        public Cliente BuscarPorId(int id)
        {
            return _clienteRepository.BuscarPorId(id);
        }

        public IEnumerable<Cliente> ListarClientes()
        {
            return _clienteRepository.ListarTodas();
        }

        public void AtualizarCliente(Cliente cliente)
        {
            _clienteRepository.Atualizar(cliente);
        }

        public void RemoverCliente(int id)
        {
            _clienteRepository.Remover(id);
        }

    }

}
