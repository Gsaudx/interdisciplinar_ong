// filepath: c:\Programming\interdisciplinar_ong\Controllers\PedidoDoacaoController.cs
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ong.Models;
using Ong.Services;

namespace Ong.Controllers
{
    public class PedidoDoacaoController : Controller
    {
        private readonly PedidoDoacaoService _pedidoDoacaoService;
        private readonly DoacaoService _doacaoService;

        public PedidoDoacaoController(
            PedidoDoacaoService pedidoDoacaoService,
            DoacaoService doacaoService)
        {
            _pedidoDoacaoService = pedidoDoacaoService;
            _doacaoService = doacaoService;
        }

        // GET: PedidoDoacao
        public async Task<IActionResult> Index()
        {
            var pedidos = await _pedidoDoacaoService.ObterTodosPedidosDoacao();
            return View(pedidos);
        }

        // GET: PedidoDoacao/Criar
        public IActionResult Criar()
        {
            return View();
        }

        // POST: PedidoDoacao/Criar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Criar(int ongId, string categoria, string descricao)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _pedidoDoacaoService.CriarPedidoDoacao(ongId, categoria, descricao);
                    return RedirectToAction("MeusPedidos", new { id = ongId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            
            return View();
        }

        // GET: PedidoDoacao/MeusPedidos
        public async Task<IActionResult> MeusPedidos(int ongId)
        {
            var pedidos = await _pedidoDoacaoService.ObterPedidosDoacaoPorOng(ongId);
            return View(pedidos);
        }

        // GET: PedidoDoacao/Detalhes
        public async Task<IActionResult> Detalhes(int id)
        {
            var pedido = await _pedidoDoacaoService.ObterPedidosDoacaoPorStatus("Aberto");
            
            if (pedido == null)
            {
                return NotFound();
            }

            ViewBag.Doacoes = await _doacaoService.ObterDoacoesPorPedido(id);
            
            return View(pedido);
        }

        // GET: PedidoDoacao/AtualizarStatus
        public async Task<IActionResult> AtualizarStatus(int id, string novoStatus)
        {
            try
            {
                await _pedidoDoacaoService.AtualizarStatusPedidoDoacao(id, novoStatus);
                return RedirectToAction("Detalhes", new { id });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}