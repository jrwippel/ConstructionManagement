using ScottPlot;
using WebAppSystemsObra.Models.Dto;

namespace WebAppSystemsObra.Services
{
    public class RelatorioService
    {

        private readonly ObraService _obraService;

        private readonly ClienteService _clienteService;

        private readonly EtapaService _etapaService;

        private readonly ExecucaoService _execucaoService;

        private readonly ServicoService _servicoService;        

        private readonly ImagemEtapaService _imageEtapaService;

        private readonly IWebHostEnvironment _env;


        public RelatorioService(ObraService obraService, ClienteService clienteService, EtapaService etapaService, ExecucaoService execucaoService, ServicoService servicoService, ImagemEtapaService imageEtapaService,
            IWebHostEnvironment env)
        {
            _obraService = obraService;
            _clienteService = clienteService;
            _etapaService = etapaService;
            _execucaoService = execucaoService;
            _servicoService = servicoService;            
            _imageEtapaService = imageEtapaService;

            _env = env;
        }
        public RelatorioObraViewModel MontarPLSDaObra(int obraId, int numeroEtapaId)
        {
            var obra = _obraService.BuscarPorId(obraId);
            var etapa = _etapaService.BuscarPorObraId(obraId);
            var execucoes = _execucaoService.ListarExecucoes()
                .Where(e => e.ObraId == obraId && e.EtapaId == numeroEtapaId)
                .ToList();

            var imagens = _imageEtapaService.ListarPorEtapa(etapa.Id)
                .Where(i => i.NumeroEtapaId == numeroEtapaId)
                .ToList();

            var model = new RelatorioObraViewModel
            {
                DescricaoObra = obra.Descricao,
                //NomeProponente = obra.Proprietario?.Nome ?? "N/A", // ajuste se necessário
                //Endereco = $"{obra.Endereco}, {obra.Bairro}, {obra.Cidade} - {obra.UF}",
                NumeroEtapa = numeroEtapaId,
                Servicos = obra.Servico.Select(servico =>
                {
                    var execucao = execucoes.FirstOrDefault(e => e.ServicoId == servico.Id);
                    return new RelatorioServico
                    {
                        Descricao = servico.Descricao,
                        PercentualIncidencia = servico.PercentualIncidencia,
                        PercentualExecucao = execucao?.PercentualExecucao ?? 0
                    };
                }).ToList(),
                Fotos = imagens.Select(img => new RelatorioFoto
                {
                    Bytes = ObterImagemComoBytes(img.NomeArquivo), // Método auxiliar
                    Descricao = img.Descricao,
                }).ToList()
            };

            model.MensuradoAnterior = _execucaoService
          .ObterMensuradoAcumulado(obraId, numeroEtapaId - 1);

            var execucoess = _execucaoService.ListarExecucoes()
                .Where(e => e.ObraId == obraId)
                .ToList();

            var execucoesPorEtapa = execucoess
                .GroupBy(e => e.EtapaId)
                .OrderBy(g => g.Key); // Garante ordem crescente de etapas

            decimal acumulado = 0;
            decimal acumuladoAnterior = 0;

            foreach (var grupo in execucoesPorEtapa)
            {
                var etapaQtde = _etapaService.BuscarPorId(grupo.Key)?.qtde ?? 0;

                decimal totalEtapa = 0;

                foreach (var execucao in grupo)
                {
                    var incidencia = _servicoService.BuscarPorId(execucao.ServicoId)?.PercentualIncidencia ?? 0;
                    totalEtapa += (execucao.PercentualExecucao * incidencia) / 100;
                }

                acumulado += totalEtapa;
                var executadoNaEtapa = totalEtapa - acumuladoAnterior;

                model.ResumoPorEtapa.Add(new ResumoEtapaDto
                {
                    NumeroEtapa = grupo.Key,
                    DataReferencia = DateTime.MinValue,
                    PercentualExecutadoNaEtapa = Math.Round(executadoNaEtapa, 2),   // novo campo
                    PercentualAcumulado = Math.Round(totalEtapa, 2)
                });

                acumuladoAnterior = totalEtapa;
            }


            // Gerar gráfico de evolução
            var etapasResumo = model.ResumoPorEtapa.OrderBy(e => e.NumeroEtapa).ToList();

            double[] eixoX = etapasResumo.Select(e => (double)e.NumeroEtapa).ToArray();            
            double[] eixoY = etapasResumo.Select(e => (double)e.PercentualAcumulado).ToArray();


            var plot = new ScottPlot.Plot();
            plot.Title("Evolução por Etapa");
            plot.XLabel("Etapa");
            plot.YLabel("Execução (%)");

            var linha = plot.Add.Scatter(eixoX, eixoY);
            linha.LineWidth = 2;
            linha.MarkerSize = 5;
            linha.Color = Colors.Blue.WithAlpha(0.8);

            // Gera os bytes da imagem direto, sem MemoryStream
            model.GraficoEvolucaoEtapas = plot.GetImageBytes(600, 400);

            return model;
        }



        /*
        private byte[] ObterImagemComoBytes(string nomeArquivo)
        {
            //var caminho = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "etapas", nomeArquivo);
            var caminho = Path.Combine(AppContext.BaseDirectory, "wwwroot", "uploads", nomeArquivo);

            return System.IO.File.Exists(caminho)
                ? System.IO.File.ReadAllBytes(caminho)
                : Array.Empty<byte>(); // <-- evita erro, mas você trata isso no PDF agora
        }

        */

        private byte[] ObterImagemComoBytes(string nomeArquivo)
        {
            var caminho = Path.Combine(_env.WebRootPath, "uploads", nomeArquivo);

            return System.IO.File.Exists(caminho)
                ? System.IO.File.ReadAllBytes(caminho)
                : Array.Empty<byte>();
        }



    }

}
