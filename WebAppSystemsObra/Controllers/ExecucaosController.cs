using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppSystems.Data;
using WebAppSystems.Models;
using WebAppSystemsObra.Services;

namespace WebAppSystemsObra.Controllers
{
    public class ExecucaosController : Controller
    {

        private readonly ObraService _obraService;

        private readonly ClienteService _clienteService;

        private readonly EtapaService _etapaService;

        private readonly ExecucaoService _execucaoService;

        private readonly ServicoService _servicoService;


        public ExecucaosController(ObraService obraService, ClienteService clienteService, EtapaService etapaService, ExecucaoService execucaoService, ServicoService servicoService)
        {
            _obraService = obraService;
            _clienteService = clienteService;
            _etapaService = etapaService;
            _execucaoService = execucaoService;
            _servicoService = servicoService;
        }

        public JsonResult ObterExecucaoPorEtapa(int obraId, int etapaId)
        {
            var execucoes = _execucaoService.ListarPorObraEtapa(obraId, etapaId)
                .Select(e => new
                {
                    servicoId = e.ServicoId,
                    percentualExecucao = e.PercentualExecucao
                })
                .ToList();

            return Json(execucoes);
        }




        public JsonResult GetEtapasEServicos(int obraId)
        {
            var obra = _obraService.BuscarPorId(obraId);

            if (obra == null)
                return Json(new { etapas = new List<object>(), servicos = new List<object>() });

            var etapas = _etapaService.ListarPorObra(obraId)
                .Select(e => new { id = e.Id, descricao = e.Descricao })
                .ToList();

            var servicos = _servicoService.ListarPorObra(obraId)
                .Select(s => new { id = s.Id, descricao = s.Descricao })
                .ToList();

            return Json(new { etapas, servicos });
        }



        public async Task<IActionResult> Index()
        {
            var execucoes = await Task.Run(() => _execucaoService.ListarExecucoes());

            return View(execucoes);
        }


        // GET: Execucaos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            var execucao = await Task.Run(() => _execucaoService.BuscarPorId(id.Value));

            if (execucao == null)
                return NotFound();

            return View(execucao);
        }

        public IActionResult Create()
        {
            
            ViewData["EtapaId"] = new SelectList(_etapaService.ListarEtapas(), "Id", "Descricao");
            ViewData["ServicoId"] = new SelectList(_servicoService.ListarTodas(), "Id", "Descricao");
            ViewBag.ObraId = new SelectList(_obraService.ListarObras(), "Id", "Descricao");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ObraId,EtapaId,ServicoId,PercentualExecucao")] Execucao execucao)
        {
            ModelState.Remove("Etapa");
            ModelState.Remove("Servico");

            if (ModelState.IsValid)
            {
                // Ajustar formatação do percentual
                string valorString = Request.Form["PercentualExecucao"].ToString().Replace(",", ".");

                if (decimal.TryParse(valorString, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valorFormatado))
                {
                    execucao.PercentualExecucao = valorFormatado;
                }
                else
                {
                    ModelState.AddModelError("PercentualExecucao", "Formato inválido. Use números decimais (exemplo: 2,28).");
                    return View(execucao);
                }

                _execucaoService.AdicionarObra(execucao);
                return RedirectToAction(nameof(Index));
            }

            ViewData["EtapaId"] = new SelectList(_etapaService.ListarEtapas(), "Id", "Descricao", execucao.EtapaId);
            ViewData["ServicoId"] = new SelectList(_servicoService.ListarTodas(), "Id", "Descricao", execucao.ServicoId);
            ViewBag.ObraId = new SelectList(_obraService.ListarObras(), "Id", "Descricao");

            return View(execucao);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            var execucao = await Task.Run(() => _execucaoService.BuscarPorId(id.Value));

            if (execucao == null)
                return NotFound();

            ViewData["EtapaId"] = new SelectList(_etapaService.ListarEtapas(), "Id", "Descricao", execucao.EtapaId);
            ViewData["ServicoId"] = new SelectList(_servicoService.ListarTodas(), "Id", "Descricao", execucao.ServicoId);
            ViewBag.ObraId = new SelectList(_obraService.ListarObras(), "Id", "Descricao");

            return View(execucao);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ObraId,EtapaId,ServicoId,PercentualExecucao")] Execucao execucao)
        {
            ModelState.Remove("Etapa");
            ModelState.Remove("Servico");

            if (id != execucao.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _execucaoService.AtualizarObra(execucao);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (_execucaoService.BuscarPorId(execucao.Id) == null) // Verifica se é nulo corretamente
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["EtapaId"] = new SelectList(_etapaService.ListarEtapas(), "Id", "Descricao", execucao.EtapaId);
            ViewData["ServicoId"] = new SelectList(_servicoService.ListarTodas(), "Id", "Descricao", execucao.ServicoId);
            ViewBag.ObraId = new SelectList(_obraService.ListarObras(), "Id", "Descricao");

            return View(execucao);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            var execucao = await Task.Run(() => _execucaoService.BuscarPorId(id.Value));

            if (execucao == null)
                return NotFound();

            return View(execucao);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _execucaoService.RemoverExecucao(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
