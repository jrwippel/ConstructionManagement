using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppSystems.Data;
using WebAppSystems.Models;
using WebAppSystemsObra.Services;
using WebAppSystems.Models.ViewModels;
using Azure.Storage.Blobs;
using WebAppSystemsObra.Models.ViewModels;


namespace WebAppSystems.Controllers
{
    public class ObrasController : Controller
    {
        private readonly ObraService _obraService;

        private readonly ClienteService _clienteService;

        private readonly EtapaService _etapaService;

        private readonly ExecucaoService _execucaoService;

        private readonly ServicoService _servicoService;

        private readonly RelatorioService _relatorioService;
        private readonly PdfService _pdfService;

        private readonly ImagemEtapaService _imageEtapaService;

        public ObrasController(ObraService obraService, ClienteService clienteService, EtapaService etapaService, ExecucaoService execucaoService, ServicoService servicoService, 
               RelatorioService relatorioService, ImagemEtapaService imageEtapaService, PdfService pdfService)
        {
            _obraService = obraService;
            _clienteService = clienteService;
            _etapaService = etapaService;
            _execucaoService = execucaoService;
            _servicoService = servicoService;
            _relatorioService = relatorioService;
            _imageEtapaService = imageEtapaService;
            _pdfService = pdfService;   
        }

        /*

        [HttpPost]
        public async Task<IActionResult> UploadImagemEtapa(int etapaId, IFormFile arquivo)
        {
            if (arquivo == null || arquivo.Length == 0)
                return BadRequest("Nenhuma imagem foi enviada.");

            var urlArquivo = await _blobStorageService.UploadFileAsync(arquivo.FileName, arquivo.OpenReadStream());

            await _imageEtapaService.AdicionarImagem(etapaId, arquivo.FileName, urlArquivo); // 🔗 Passa só a URL e não o Stream

            return Ok(new { mensagem = "Imagem enviada com sucesso!", url = urlArquivo });
        }

        */

        public IActionResult GerarRelatorioPLS(int obraId, int etapa)
        {
            var dados = _relatorioService.MontarPLSDaObra(obraId, etapa);

            var pdfBytes = _pdfService.GerarRelatorioPLS(dados);

            return File(pdfBytes, "application/pdf", $"Relatorio_PLS_Obra_{obraId}_Etapa_{etapa}.pdf");
        }

        [HttpGet]
        public IActionResult ListarFotosPorEtapa(int etapaId, int numeroEtapaId)
        {
            var imagens = _imageEtapaService
                .ListarPorEtapa(etapaId)
                .Where(i => i.NumeroEtapaId == numeroEtapaId);

            var viewModel = imagens.Select(img => new FotoEtapaViewModel
            {
                Id = img.Id,
                Url = img.UrlImagem
            });

            return Json(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> ExcluirFoto(int fotoId)
        {
            try
            {
                await _imageEtapaService.RemoverImagem(fotoId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpPost]
        public async Task<IActionResult> UploadImagemEtapa(int etapaId, int numeroEtapaId, IFormFile arquivo, string descricao)
        {
            if (arquivo == null || arquivo.Length == 0)
                return BadRequest("Nenhuma imagem foi enviada.");

            var caminhoLocal = Path.Combine("wwwroot/uploads", arquivo.FileName);

            using (var stream = new FileStream(caminhoLocal, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }

            var urlArquivo = $"/uploads/{arquivo.FileName}"; // 🔁 URL simulada da imagem

            await _imageEtapaService.AdicionarImagem(etapaId, numeroEtapaId, arquivo.FileName, urlArquivo, descricao);

            return Ok(new { mensagem = "Imagem enviada com sucesso!", url = urlArquivo });
        }


        public IActionResult ListarServicos(int obraId)
        {
            var obra = _obraService.BuscarPorId(obraId);
            if (obra == null) return NotFound();

            return PartialView("_TabelaServicos", obra.Servico);
        }


        [HttpGet]
        public IActionResult ObterExecucoesPorEtapa(int obraId, int etapa)
        {
            var execucoesAtuais = _execucaoService.ListarPorObraEEtapa(obraId, etapa);
            var execucoesAnteriores = new List<Execucao>();

            if (!execucoesAtuais.Any() && etapa > 1)
            {
                execucoesAnteriores = _execucaoService.ListarPorObraEEtapa(obraId, etapa - 1);
            }

            return Json(new { execucoesAtuais, execucoesAnteriores });
        }

        [HttpGet]
        public IActionResult VerificarExecucoes(int servicoId)
        {
            var temExecucoes = _execucaoService.ExisteExecucaoParaServico(servicoId);
            return Json(new { temExecucoes });
        }




        [HttpPost]
        public IActionResult SalvarExecucoes([FromBody] List<Execucao> execucoes)
        {
            if (execucoes == null || !execucoes.Any())
                return BadRequest("Nenhuma execução informada.");

            var obraId = execucoes.First().ObraId;

            var servicosDaObra = _servicoService.ListarPorObra(obraId);
            var incidenciaTotal = servicosDaObra.Sum(s => s.PercentualIncidencia);

            if (Math.Round(incidenciaTotal, 2) != 100.0m)
            {
                return BadRequest($"A incidência total dos serviços não soma 100%. Soma atual: {incidenciaTotal:F2}%");
            }

            var etapaAtualId = execucoes.First().EtapaId; // Obtém a etapa que está sendo salva

            // Busca os serviços da etapa anterior
            var execucoesAnteriores = _execucaoService.ListarPorObraEtapa(obraId, etapaAtualId - 1);

            if (execucoesAnteriores.Any())
            {
                foreach (var execucaoAtual in execucoes)
                {
                    var execucaoAnterior = execucoesAnteriores.FirstOrDefault(e => e.ServicoId == execucaoAtual.ServicoId);
                    if (execucaoAnterior != null && execucaoAtual.PercentualExecucao < execucaoAnterior.PercentualExecucao)
                    {
                        return BadRequest($"O serviço {execucaoAtual.ServicoId} tem um percentual de execução ({execucaoAtual.PercentualExecucao:F2}%) menor que na etapa anterior ({execucaoAnterior.PercentualExecucao:F2}%).");
                    }
                }
            }

            foreach (var exec in execucoes)
            {
                _execucaoService.SalvarOuAtualizar(exec);
            }

            return Ok();
        }


        /*
        [HttpPost]
        public IActionResult SalvarExecucoes([FromBody] List<Execucao> execucoes)
        {
            if (execucoes == null || !execucoes.Any())
                return BadRequest("Nenhuma execução informada.");

            var obraId = execucoes.First().ObraId;

            var servicosDaObra = _servicoService.ListarPorObra(obraId);
            var incidenciaTotal = servicosDaObra.Sum(s => s.PercentualIncidencia);

            if (Math.Round(incidenciaTotal, 2) != 100.0m)
            {
                return BadRequest($"A incidência total dos serviços não soma 100%. Soma atual: {incidenciaTotal:F2}%");
            }

            // Salvar execuções normalmente
            foreach (var execucao in execucoes)
            {
                _execucaoService.Salvar(execucao);
            }

            return Ok();
        }

        */
        public IActionResult Dashboard(int id)
        {
            var obra = _obraService.BuscarPorId(id);
            if (obra == null)
            {
                return NotFound();
            }

            var percentualConclusao = _obraService.CalcularPercentualConclusao(id);
            ViewBag.PercentualConclusao = percentualConclusao;
            ViewBag.Obra = obra;

            // Buscar a etapa da obra
            var etapaObra = _etapaService.BuscarPorObraId(id);

            if (etapaObra == null || etapaObra.qtde == 0)
            {
                ViewBag.Etapas = new SelectList(new List<SelectListItem>
        {
            new SelectListItem { Value = "", Text = "Nenhuma etapa disponível" }
        }, "Value", "Text");
                ViewBag.EtapaId = null;
            }
            else
            {
                var etapasList = new List<SelectListItem>
        {
            new SelectListItem { Value = "", Text = "Selecionar...", Selected = true }
        };

                etapasList.AddRange(
                    Enumerable.Range(1, etapaObra.qtde)
                        .Select(numero => new SelectListItem
                        {
                            Value = numero.ToString(),            // número da subetapa
                            Text = $"Etapa: {numero}"             // label para o usuário
                        })
                );

                ViewBag.Etapas = new SelectList(etapasList, "Value", "Text");
                ViewBag.EtapaId = etapaObra.Id;                    // Id real da entidade Etapa
            }

            // Buscar execuções já cadastradas
            var execucoesCadastradas = _execucaoService.ListarExecucoes()
                .Where(e => e.ObraId == id && e.PercentualExecucao > 0)
                .ToList();

            ViewBag.Execucoes = execucoesCadastradas;

            // Obter a última etapa com execução registrada
            var ultimaEtapa = execucoesCadastradas
                .OrderByDescending(e => e.EtapaId)
                .Select(e => e.EtapaId)
                .FirstOrDefault();

            ViewBag.EtapaSelecionada = ultimaEtapa;

            return View();
        }


        public IActionResult Index()
        {
            var obras = _obraService.ListarObras();

            var viewModel = obras.Select(obra => new ObraComExecucaoViewModel
            {
                Obra = obra,
                PercentualConclusao = _execucaoService.CalcularPercentualExecucaoTotal(obra.Id)
            }).ToList();

            return View(viewModel);
        }


        public IActionResult Details(int? id)
        {
            if (!id.HasValue) // Verifica se o id é null
            {
                return NotFound();
            }

            var obra = _obraService.BuscarPorId(id.Value); // Converte para int antes de passar para o método
            if (obra == null)
            {
                return NotFound();
            }

            return View(obra);
        }
        public IActionResult Create()
        {
            ViewBag.ClienteId = new SelectList(_clienteService.ListarClientes(), "Id", "Nome");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Descricao,ClienteId")] Obra obra, int quantidadeEtapas)
        {
            if (ModelState.IsValid)
            {
                _obraService.AdicionarObra(obra);

                var etapa = new Etapa
                {
                    Descricao = $"Etapas da obra: {obra.Descricao}",
                    ObraId = obra.Id,
                    qtde = quantidadeEtapas
                };

                _etapaService.AdicionarObra(etapa);

                return RedirectToAction(nameof(Index));
            }

            return View(obra);
        }



        public IActionResult Edit(int? id)
        {
            if (!id.HasValue) // Verifica se o ID não é null
            {
                return NotFound();
            }           

            var obra = _obraService.BuscarPorId(id.Value); // Converte de int? para int
            if (obra == null)
            {
                return NotFound();
            }        
            return View(obra);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Descricao,ClienteId")] Obra obra)
        {
            if (id != obra.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _obraService.AtualizarObra(obra);
                return RedirectToAction(nameof(Index));
            }

            return View(obra);
        }

        public IActionResult Delete(int? id)
        {
            if (!id.HasValue) // Verifica se o ID está preenchido
            {
                return NotFound();
            }

            var obra = _obraService.BuscarPorId(id.Value); // Converte de int? para int
            if (obra == null)
            {
                return NotFound();
            }

            return View(obra);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _obraService.Remover(id);
            return RedirectToAction(nameof(Index));
        }   
    }


}
