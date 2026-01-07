using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using WebAppSystems.Models;
using WebAppSystemsObra.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace WebAppSystems.Controllers
{
    public class EtapasController : Controller
    {
        private readonly EtapaService _etapaService;
        private readonly ObraService _obraService;
        private readonly ImagemEtapaService _imageEtapaService;

        public EtapasController(EtapaService etapaService, ObraService obraService, ImagemEtapaService imageEtapaService)
        {
            _etapaService = etapaService;
            _obraService = obraService;
            _imageEtapaService = imageEtapaService;   
        }      


        // GET: Etapas
        public async Task<IActionResult> Index()
        {
            var etapas = await Task.Run(() => _etapaService.ListarEtapas());
            return View(etapas);
        }

        // GET: Etapas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            var etapa = await Task.Run(() => _etapaService.BuscarPorId(id.Value));
            if (etapa == null)
                return NotFound();

            return View(etapa);
        }

        // GET: Etapas/Create
        public IActionResult Create()
        {
            ViewBag.ObraId = new SelectList(_obraService.ListarObras(), "Id", "Descricao");
            return View();
        }

        // POST: Etapas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Descricao,ObraId,qtde")] Etapa etapa)
        {
            if (ModelState.IsValid)
            {
                await Task.Run(() => _etapaService.AdicionarObra(etapa));
                return RedirectToAction(nameof(Index));
            }
            return View(etapa);
        }

        // GET: Etapas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            ViewBag.ObraId = new SelectList(_obraService.ListarObras(), "Id", "Descricao");

            var etapa = await Task.Run(() => _etapaService.BuscarPorId(id.Value));
            if (etapa == null)
                return NotFound();

            return View(etapa);
        }

        // POST: Etapas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Descricao,ObraId,qtde")] Etapa etapa)
        {
            if (id != etapa.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await Task.Run(() => _etapaService.AtualizarObra(etapa));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (_etapaService.BuscarPorId(etapa.Id) == null)
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(etapa);
        }

        // GET: Etapas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            var etapa = await Task.Run(() => _etapaService.BuscarPorId(id.Value));
            if (etapa == null)
                return NotFound();

            return View(etapa);
        }

        // POST: Etapas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await Task.Run(() => _etapaService.Remover(id));
            return RedirectToAction(nameof(Index));
        }
    }
}
