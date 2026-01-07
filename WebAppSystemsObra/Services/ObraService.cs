
using WebAppSystems.Models;

namespace WebAppSystemsObra.Services
{
    public class ObraService
    {
        private readonly IObraRepository _obraRepository;

        public ObraService(IObraRepository obraRepository)
        {
            _obraRepository = obraRepository;
        }

        public void AdicionarObra(Obra obra)
        {
            _obraRepository.Adicionar(obra);
        }

        public Obra BuscarPorId(int id)
        {
            return _obraRepository.BuscarPorId(id);
        }

        public IEnumerable<Obra> ListarObras()
        {
            return _obraRepository.ListarTodas();
        }

        public void AtualizarObra(Obra obra)
        {
            _obraRepository.Atualizar(obra);
        }

        public void Remover(int id)
        {
            _obraRepository.Remover(id);
        }

        public decimal CalcularPercentualConclusao(int obraId)
        {
            var obra = _obraRepository.BuscarPorId(obraId);
            if (obra == null || obra.Servico.Count == 0) return 0;

            decimal percentualConclusaoTotal = 0;

            foreach (var servico in obra.Servico)
            {
                decimal percentualConclusaoServico = servico.Execucao.Sum(e => e.PercentualExecucao) * servico.PercentualIncidencia / 100;
                percentualConclusaoTotal += percentualConclusaoServico;
            }

            return percentualConclusaoTotal;
        }

    }

}
