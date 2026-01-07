
using Google;
using Google.Api;
using WebAppSystems.Data;
using WebAppSystems.Models;
using WebAppSystemsObra.Services;

namespace WebAppSystemsObra.Repository
{
    public class ServicoRepository : IServicoRepository
    {
        private readonly WebAppSystemsContext _context;

        public ServicoRepository(WebAppSystemsContext context)
        {
            _context = context;
        }


        public void Adicionar(Servico servico)
        {
            _context.Servicos.Add(servico);
            _context.SaveChanges();
        }

        public Servico BuscarPorId(int id)
        {
            return _context.Servicos.Find(id);
        }

        public IEnumerable<Servico> ListarTodas()
        {
            return _context.Servicos.ToList();
        }

        public void Atualizar(Servico servico)
        {
            _context.Servicos.Update(servico);
            _context.SaveChanges();
        }

        public void Remover(int id)
        {
            var servico = _context.Servicos.Find(id);
            if (servico != null)
            {
                _context.Servicos.Remove(servico);
                _context.SaveChanges();
            }
        }

        // Novo método implementado para buscar serviços por obra
        public IEnumerable<Servico> ListarPorObra(int obraId)
        {
            return _context.Servicos.Where(s => s.ObraId == obraId).ToList();
        }
    }

}
