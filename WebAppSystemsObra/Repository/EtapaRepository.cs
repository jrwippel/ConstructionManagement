
using Azure;
using Google;
using WebAppSystems.Data;
using WebAppSystems.Models;
using WebAppSystemsObra.Services;

namespace WebAppSystemsObra.Repository
{
    public class EtapaRepository : IEtapaRepository
    {
        private readonly WebAppSystemsContext _context;

        public EtapaRepository(WebAppSystemsContext context)
        {
            _context = context;
        }


        public void Adicionar(Etapa etapa)
        {
            _context.Etapas.Add(etapa);
            _context.SaveChanges();
        }

        public Etapa BuscarPorId(int id)
        {
            return _context.Etapas.Find(id);
        }

        public IEnumerable<Etapa> ListarTodas()
        {
            return _context.Etapas.ToList();
        }

        public void Atualizar(Etapa etapa)
        {
            _context.Etapas.Update(etapa);
            _context.SaveChanges();
        }

        public void Remover(int id)
        {
            var etapa = _context.Etapas.Find(id);
            if (etapa != null)
            {
                _context.Etapas.Remove(etapa);
                _context.SaveChanges();
            }
        }

        public IEnumerable<Etapa> ListarPorObra(int obraId)
        {
            return _context.Etapas
                .Where(e => e.ObraId == obraId)
                .ToList(); // ToList ainda é válido, pois converte para IEnumerable
        }


    }

}
