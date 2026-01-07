using WebAppSystems.Data;
using WebAppSystems.Models;
using WebAppSystemsObra.Services;
using Microsoft.EntityFrameworkCore;


namespace WebAppSystemsObra.Repository
{
    public class ImagemEtapaRepository : IImagemEtapaRepository
    {
        private readonly WebAppSystemsContext _context;

        public ImagemEtapaRepository(WebAppSystemsContext context)
        {
            _context = context;
        }

        public void Adicionar(ImagemEtapa imagem)
        {
            _context.ImagensEtapa.Add(imagem);
            _context.SaveChanges();
        }

        public ImagemEtapa BuscarPorId(int id)
        {
            return _context.ImagensEtapa.Find(id);
        }

        public IEnumerable<ImagemEtapa> ListarTodas()
        {
            return _context.ImagensEtapa.ToList();
        }

        public void Remover(int id)
        {
            var imagem = _context.ImagensEtapa.Find(id);
            if (imagem != null)
            {
                _context.ImagensEtapa.Remove(imagem);
                _context.SaveChanges();
            }
        }

        public IEnumerable<ImagemEtapa> ListarPorEtapa(int etapaId)
        {
            return _context.ImagensEtapa.Where(i => i.EtapaId == etapaId).ToList();
        }

        public IQueryable<ImagemEtapa> ListarPorObra(int obraId)
        {
            return _context.ImagensEtapa.Include(i => i.Etapa).Where(i => i.Etapa.ObraId == obraId);
        }

    }
}
