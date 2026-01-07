
using WebAppSystems.Data;
using WebAppSystems.Models;
using WebAppSystemsObra.Services;


namespace WebAppSystemsObra.Repository
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly WebAppSystemsContext _context;

        public ClienteRepository(WebAppSystemsContext context)
        {
            _context = context;
        }


        public void Adicionar(Cliente cliente)
        {
            _context.Clientes.Add(cliente);
            _context.SaveChanges();
        }

        public Cliente BuscarPorId(int id)
        {
            return _context.Clientes.Find(id);
        }

        public IEnumerable<Cliente> ListarTodas()
        {
            return _context.Clientes.ToList();
        }

        public void Atualizar(Cliente cliente)
        {
            _context.Clientes.Update(cliente);
            _context.SaveChanges();
        }

        public void Remover(int id)
        {
            var cliente = _context.Clientes.Find(id);
            if (cliente != null)
            {
                _context.Clientes.Remove(cliente);
                _context.SaveChanges();
            }
        }
    }

}
