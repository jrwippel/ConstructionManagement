using WebAppSystems.Models;
using WebAppSystemsObra.Repository;

namespace WebAppSystemsObra.Services
{
    public class ExecucaoService
    {
        private readonly IExecucaoRepository _execucaoRepository;
        private readonly IServicoRepository _servicoRepository;

        public ExecucaoService(IExecucaoRepository execucaoRepository, IServicoRepository servicoRepository)
        {
            _execucaoRepository = execucaoRepository;
            _servicoRepository = servicoRepository;
        }

        public void AdicionarObra(Execucao execucao)
        {
            _execucaoRepository.Adicionar(execucao);
        }

        public Execucao BuscarPorId(int id)
        {
            return _execucaoRepository.BuscarPorId(id);
        }

        public IEnumerable<Execucao> ListarExecucoes()
        {
            return _execucaoRepository.ListarTodas();
        }

        public void AtualizarObra(Execucao execucao)
        {
            _execucaoRepository.Atualizar(execucao);
        }

        public void RemoverExecucao(int id)
        {
            _execucaoRepository.Remover(id);
        }

        // Novo método para buscar execuções filtradas por obra e etapa
        public IEnumerable<Execucao> ListarPorObraEtapa(int obraId, int etapaId)
        {
            return _execucaoRepository.ListarPorObraEtapa(obraId, etapaId);
        }

        public List<Execucao> ListarPorObraEEtapa(int obraId, int etapa)
        {
            return _execucaoRepository
                .ListarTodas()
                .Where(e => e.ObraId == obraId && e.EtapaId == etapa)
                .ToList();
        }

        public void SalvarOuAtualizar(Execucao novaExecucao)
        {
            var execucaoExistente = _execucaoRepository
                .ListarTodas()
                .FirstOrDefault(e =>
                    e.ObraId == novaExecucao.ObraId &&
                    e.ServicoId == novaExecucao.ServicoId &&
                    e.EtapaId == novaExecucao.EtapaId
                );

            if (execucaoExistente != null)
            {
                execucaoExistente.PercentualExecucao = novaExecucao.PercentualExecucao;
                _execucaoRepository.Atualizar(execucaoExistente);
            }
            else
            {
                _execucaoRepository.Adicionar(novaExecucao);
            }
        }

        public decimal CalcularPercentualExecucaoTotal(int obraId)
        {
            // Todos os serviços da obra, com suas incidências
            var servicosDaObra = _servicoRepository
                .ListarTodas()
                .Where(s => s.ObraId == obraId)
                .ToList();

            if (!servicosDaObra.Any())
                return 0;

            // Execuções da obra com PercentualExecucao > 0
            var execucoes = _execucaoRepository
                .ListarTodas()
                .Where(e => e.ObraId == obraId && e.PercentualExecucao > 0)
                .ToList();

            if (!execucoes.Any())
                return 0;

            // Descobre a última etapa com execuções preenchidas
            var ultimaEtapaId = execucoes
                .OrderByDescending(e => e.EtapaId)
                .Select(e => e.EtapaId)
                .First();

            // Filtra execuções da última etapa
            var execucoesDaUltimaEtapa = execucoes
                .Where(e => e.EtapaId == ultimaEtapaId)
                .ToList();

            // Junta os serviços com suas execuções (ou 0 se não houver)
            decimal somaPonderada = 0;
            foreach (var servico in servicosDaObra)
            {
                var execucao = execucoesDaUltimaEtapa
                    .FirstOrDefault(e => e.ServicoId == servico.Id);

                var percentualExecutado = execucao != null ? execucao.PercentualExecucao / 100m : 0;

                somaPonderada += servico.PercentualIncidencia * percentualExecutado;
            }

            return Math.Round(somaPonderada, 2);
        }

        public bool ExisteExecucaoParaServico(int servicoId)
        {
            return _execucaoRepository.ListarTodas().Any(e => e.ServicoId == servicoId);
        }

        public decimal ObterMensuradoAcumulado(int obraId, int numeroEtapaId)
        {
            // Carrega todas as execuções da obra anteriores à etapa atual
            var execucoes = _execucaoRepository
                .ListarPorObra(obraId)
                .Where(e => e.EtapaId == numeroEtapaId)
                .ToList();

            // Carrega todos os serviços da obra com suas incidências
            var servicos = _servicoRepository
                .ListarTodas()
                .Where(s => s.ObraId == obraId)
                .ToDictionary(s => s.Id, s => s.PercentualIncidencia);

            decimal total = 0;

            foreach (var execucao in execucoes)
            {
                if (servicos.TryGetValue(execucao.ServicoId, out var incidencia))
                {
                    total += (execucao.PercentualExecucao * incidencia) / 100;
                }
            }

            return Math.Round(total, 2);
        }






    }
}
