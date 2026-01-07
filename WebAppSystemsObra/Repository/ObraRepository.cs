
using Google;
using Microsoft.EntityFrameworkCore;
using WebAppSystems.Data;
using WebAppSystems.Models;
using WebAppSystemsObra.Services;

namespace WebAppSystemsObra.Repository
{
    public class ObraRepository : IObraRepository
    {
        private readonly WebAppSystemsContext _context;

        public ObraRepository(WebAppSystemsContext context)
        {
            _context = context;
        }


        public void Adicionar(Obra obra)
        {
            _context.Obras.Add(obra);
            _context.SaveChanges();
        }

        public Obra BuscarPorId(int id)
        {
            return _context.Obras
                .Include(o => o.Servico) // Carrega os serviços da obra
                .ThenInclude(s => s.Execucao) // Carrega as execuções dos serviços
                .FirstOrDefault(o => o.Id == id);
        }


        public IEnumerable<Obra> ListarTodas()
        {
            return _context.Obras.ToList();
        }

        public void Atualizar(Obra obra)
        {
            _context.Obras.Update(obra);
            _context.SaveChanges();
        }

        public void Remover(int id)
        {
            var obra = _context.Obras.Find(id);
            if (obra != null)
            {
                _context.Obras.Remove(obra);
                _context.SaveChanges();
            }
        }
    }

}
