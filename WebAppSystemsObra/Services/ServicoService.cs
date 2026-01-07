
using WebAppSystems.Models;

namespace WebAppSystemsObra.Services
{
    public class ServicoService
    {
        private readonly IServicoRepository _servicoRepository;

        public ServicoService(IServicoRepository servicoRepository)
        {
            _servicoRepository = servicoRepository;
        }

        public void AdicionarObra(Servico servico)
        {
            _servicoRepository.Adicionar(servico);
        }

        public Servico BuscarPorId(int id)
        {
            return _servicoRepository.BuscarPorId(id);
        }

        public IEnumerable<Servico> ListarTodas()
        {
            return _servicoRepository.ListarTodas();
        }

        public void AtualizarServico(Servico servico)
        {
            _servicoRepository.Atualizar(servico);
        }

        public IEnumerable<Servico> ListarPorObra(int obraId)
        {
            return _servicoRepository.ListarPorObra(obraId);
        }

        public void RemoverServico(int id)
        {
            _servicoRepository.Remover(id);
        }



    }

}
