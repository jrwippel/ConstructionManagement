using Microsoft.AspNetCore.Mvc;
using WebAppSystems.Models;
using WebAppSystemsObra.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace WebAppSystemsObra.Controllers
{
    public class ServicoesController : Controller
    {
        private readonly ServicoService _servicoService;
        private readonly ObraService _obraService;
        private readonly ExecucaoService _execucaoService;

        public ServicoesController(ServicoService servicoService, ObraService obraService, ExecucaoService execucaoService)
        {
            _servicoService = servicoService;
            _obraService = obraService;
            _execucaoService = execucaoService; 
        }


        [HttpPost]
        public IActionResult SalvarServicos([FromBody] Servico servico)
        {
            if (servico == null)
                return BadRequest("Serviço inválido.");

            // Buscar todos os serviços da mesma obra
            var servicosExistentes = _servicoService.ListarPorObra(servico.ObraId);

            // Calcular a soma dos percentuais, excluindo o serviço atual se for uma atualização
            decimal somaPercentuais = servicosExistentes
                .Where(s => s.Id != servico.Id)
                .Sum(s => s.PercentualIncidencia);

            // Verificar se a soma ultrapassa 100%
            if (somaPercentuais + servico.PercentualIncidencia > 100)
            {
                return BadRequest("A soma dos percentuais de incidência dos serviços da obra não pode ultrapassar 100%.");
            }

            // Salvar ou atualizar
            if (servico.Id == 0)
                _servicoService.AdicionarObra(servico);
            else
                _servicoService.AtualizarServico(servico);

            return Ok();
        }


        [HttpPost]
        public IActionResult AtualizarServicos([FromBody] Servico servico)
        {
            if (servico == null || servico.Id == 0)
                return BadRequest("Serviço inválido.");

            _servicoService.AtualizarServico(servico);
            return Ok();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFromModal([Bind("Id,Descricao,ObraId,PercentualIncidencia")] Servico servico)
        {
            if (ModelState.IsValid)
            {
                string valorString = Request.Form["PercentualIncidencia"].ToString().Replace(",", ".");

                if (decimal.TryParse(valorString, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valorFormatado))
                {
                    servico.PercentualIncidencia = valorFormatado;
                }
                else
                {
                    return BadRequest("Formato inválido no campo PercentualIncidencia.");
                }

                await Task.Run(() => _servicoService.AdicionarObra(servico));

                return Json(new
                {
                    id = servico.Id,
                    descricao = servico.Descricao,
                    percentualIncidencia = servico.PercentualIncidencia.ToString("0.##", CultureInfo.InvariantCulture)
                });
            }

            return BadRequest("Dados inválidos. Verifique os campos obrigatórios.");
        }




        // GET: Servicoes
        public async Task<IActionResult> Index()
        {
            var servicos = await Task.Run(() => _servicoService.ListarTodas());
            return View(servicos);
        }

        // GET: Servicoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            var servico = await Task.Run(() => _servicoService.BuscarPorId(id.Value));
            if (servico == null)
                return NotFound();

            return View(servico);
        }

        // GET: Servicoes/Create
        public IActionResult Create()
        {
            ViewBag.ObraId = new SelectList(_obraService.ListarObras(), "Id", "Descricao");
            return View();
        }

        // POST: Servicoes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Descricao,ObraId,PercentualIncidencia")] Servico servico)
        {
            if (ModelState.IsValid)
            {
                string valorString = Request.Form["PercentualIncidencia"].ToString().Replace(",", ".");

                if (decimal.TryParse(valorString, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valorFormatado))
                {
                    servico.PercentualIncidencia = valorFormatado;
                }
                else
                {
                    ModelState.AddModelError("PercentualIncidencia", "Formato inválido. Use números decimais.");
                    return View(servico);
                }

                await Task.Run(() => _servicoService.AdicionarObra(servico));
                return RedirectToAction(nameof(Index));
            }
            return View(servico);
        }

        // GET: Servicoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
                return NotFound();
            ViewBag.ObraId = new SelectList(_obraService.ListarObras(), "Id", "Descricao");

            var servico = await Task.Run(() => _servicoService.BuscarPorId(id.Value));
            if (servico == null)
                return NotFound();

            return View(servico);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Descricao,ObraId,PercentualIncidencia")] Servico servico)
        {
            if (id != servico.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                // Faz a conversão segura de string para decimal
                string valorString = Request.Form["PercentualIncidencia"].ToString().Replace(",", ".");

                if (decimal.TryParse(valorString, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valorFormatado))
                {
                    servico.PercentualIncidencia = valorFormatado;
                }
                else
                {
                    ModelState.AddModelError("PercentualIncidencia", "Formato inválido. Use números decimais.");
                    return View(servico);
                }

                try
                {
                    await Task.Run(() => _servicoService.AtualizarServico(servico));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (_servicoService.BuscarPorId(servico.Id) == null)
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(servico);
        }

        // GET: Servicoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            var servico = await Task.Run(() => _servicoService.BuscarPorId(id.Value));
            if (servico == null)
                return NotFound();

            return View(servico);
        }

        // POST: Servicoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await Task.Run(() => _servicoService.RemoverServico(id));
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Remover(int servicoId) // 🔁 Altere para "servicoId" se o frontend envia assim
        {
            if (servicoId <= 0)
                return BadRequest("ID do serviço inválido.");

            _servicoService.RemoverServico(servicoId);
            return Ok();
        }

    }
}
