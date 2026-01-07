using Microsoft.AspNetCore.Mvc;
using WebAppSystems.Models;
using WebAppSystemsObra.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebAppSystemsObra.Controllers
{
    public class ClientesController : Controller
    {
        private readonly ClienteService _clienteService;

        public ClientesController(ClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        // GET: Clientes
        public async Task<IActionResult> Index()
        {
            var clientes = await Task.Run(() => _clienteService.ListarClientes());
            return View(clientes);
        }

        // GET: Clientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            var cliente = await Task.Run(() => _clienteService.BuscarPorId(id.Value));
            if (cliente == null)
                return NotFound();

            return View(cliente);
        }

        // GET: Clientes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clientes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome")] Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                await Task.Run(() => _clienteService.AdicionarCliente(cliente));
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        // GET: Clientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            var cliente = await Task.Run(() => _clienteService.BuscarPorId(id.Value));
            if (cliente == null)
                return NotFound();

            return View(cliente);
        }

        // POST: Clientes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome")] Cliente cliente)
        {
            if (id != cliente.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await Task.Run(() => _clienteService.AtualizarCliente(cliente));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (_clienteService.BuscarPorId(cliente.Id) == null)
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        // GET: Clientes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            var cliente = await Task.Run(() => _clienteService.BuscarPorId(id.Value));
            if (cliente == null)
                return NotFound();

            return View(cliente);
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await Task.Run(() => _clienteService.RemoverCliente(id));
            return RedirectToAction(nameof(Index));
        }

        private bool ClienteExists(int id)
        {
            return _clienteService.BuscarPorId(id) != null;
        }
    }
}
