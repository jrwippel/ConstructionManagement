
using Google;
using Google.Api;
using WebAppSystems.Data;
using WebAppSystems.Models;
using WebAppSystemsObra.Services;

namespace WebAppSystemsObra.Repository
{
    public class ExecucaoRepository : IExecucaoRepository
    {
        private readonly WebAppSystemsContext _context;

        public ExecucaoRepository(WebAppSystemsContext context)
        {
            _context = context;
        }


        public void Adicionar(Execucao execucao)
        {
            _context.Execucaos.Add(execucao);
            _context.SaveChanges();
        }

        public Execucao BuscarPorId(int id)
        {
            return _context.Execucaos.Find(id);
        }

        public IEnumerable<Execucao> ListarTodas()
        {
            return _context.Execucaos.ToList();
        }

        public void Atualizar(Execucao execucao)
        {
            _context.Execucaos.Update(execucao);
            _context.SaveChanges();
        }

        public void Remover(int id)
        {
            var execucao = _context.Execucaos.Find(id);
            if (execucao != null)
            {
                _context.Execucaos.Remove(execucao);
                _context.SaveChanges();
            }
        }
        public IEnumerable<Execucao> ListarPorObraEtapa(int obraId, int etapaId)
        {
            return _context.Execucaos.Where(e => e.ObraId == obraId && e.EtapaId == etapaId).ToList();
        }
        public IEnumerable<Execucao> ListarPorObra(int obraId)
        {
            return _context.Execucaos
                .Where(e => e.ObraId == obraId)
                .ToList();
        }




    }

}
