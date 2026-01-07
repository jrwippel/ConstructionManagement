
using WebAppSystems.Models;
using WebAppSystemsObra.Repository;

namespace WebAppSystemsObra.Services
{
    public class EtapaService
    {
        private readonly IEtapaRepository _etapaRepository;

        public EtapaService(IEtapaRepository etapaRepository)
        {
            _etapaRepository = etapaRepository;
        }

        public void AdicionarObra(Etapa etapa)
        {
            _etapaRepository.Adicionar(etapa);
        }

        public Etapa BuscarPorId(int id)
        {
            return _etapaRepository.BuscarPorId(id);
        }
        public Etapa BuscarPorObraId(int obraId)
        {
            return _etapaRepository.ListarPorObra(obraId).FirstOrDefault();
        }


        public IEnumerable<Etapa> ListarEtapas() 
        {
            return _etapaRepository.ListarTodas();
        }


        public void AtualizarObra(Etapa etapa)
        {
            _etapaRepository.Atualizar(etapa);
        }
        public IEnumerable<Etapa> ListarPorObra(int obraId)
        {
            return _etapaRepository.ListarTodas()
                .Where(e => e.ObraId == obraId);
        }

        public void Remover(int id)
        {
            _etapaRepository.Remover(id);
        }


    }

}
