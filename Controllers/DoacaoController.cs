using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ong.Models;
using Ong.Services;

namespace Ong.Controllers
{
    public class DoacaoController : Controller
    {
        private readonly DoacaoService _doacaoService;
        private readonly UsuarioService _usuarioService;
        private readonly PedidoDoacaoService _pedidoDoacaoService;

        public DoacaoController(
            DoacaoService doacaoService,
            UsuarioService usuarioService,
            PedidoDoacaoService pedidoDoacaoService)
        {
            _doacaoService = doacaoService;
            _usuarioService = usuarioService;
            _pedidoDoacaoService = pedidoDoacaoService;
        }

        // GET: Doacao/Realizar
        public async Task<IActionResult> Realizar(int? pedidoId)
        {
            ViewBag.ONGs = await _usuarioService.ObterUsuariosPorTipo(TipoUsuario.Organizacao);
            
            if (pedidoId.HasValue)
            {
                ViewBag.PedidoDoacao = await _pedidoDoacaoService.ObterPedidosDoacaoPorStatus("Aberto");
            }
            
            return View();
        }

        // POST: Doacao/Realizar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Realizar(int doadorId, int ongId, int? pedidoDoacaoId, string categoria, string descricao)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _doacaoService.RealizarDoacao(doadorId, ongId, pedidoDoacaoId, categoria, descricao);
                    return RedirectToAction("MinhasDoacoes", new { id = doadorId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            ViewBag.ONGs = await _usuarioService.ObterUsuariosPorTipo(TipoUsuario.Organizacao);
            
            if (pedidoDoacaoId.HasValue)
            {
                ViewBag.PedidoDoacao = await _pedidoDoacaoService.ObterPedidosDoacaoPorStatus("Aberto");
            }
            
            return View();
        }

        // GET: Doacao/MinhasDoacoes
        public async Task<IActionResult> MinhasDoacoes(int doadorId)
        {
            var doacoes = await _doacaoService.ObterDoacoesPorDoador(doadorId);
            return View(doacoes);
        }

        // GET: Doacao/DoacoesRecebidas
        public async Task<IActionResult> DoacoesRecebidas(int ongId)
        {
            var doacoes = await _doacaoService.ObterDoacoesPorOng(ongId);
            return View(doacoes);
        }

        // GET: Doacao/DoacoesPorPedido
        public async Task<IActionResult> DoacoesPorPedido(int pedidoId)
        {
            var doacoes = await _doacaoService.ObterDoacoesPorPedido(pedidoId);
            return View(doacoes);
        }
    }
}