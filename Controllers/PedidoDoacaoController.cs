// filepath: c:\Programming\interdisciplinar_ong\Controllers\PedidoDoacaoController.cs
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Ong.Models;
using Ong.Services;

namespace Ong.Controllers
{
    public class PedidoDoacaoController : Controller
    {
        private readonly PedidoDoacaoService _pedidoDoacaoService;
        private readonly DoacaoService _doacaoService;
        private readonly SessaoService _sessaoService;

        public PedidoDoacaoController(
            PedidoDoacaoService pedidoDoacaoService,
            DoacaoService doacaoService,
            SessaoService sessaoService)
        {
            _pedidoDoacaoService = pedidoDoacaoService;
            _doacaoService = doacaoService;
            _sessaoService = sessaoService;
        }

        // GET: PedidoDoacao
        public async Task<IActionResult> Index()
        {
            var pedidos = await _pedidoDoacaoService.ObterTodosPedidosDoacao();
            return View(pedidos);
        }

        // GET: PedidoDoacao/Criar
        [Authorize]
        public IActionResult Criar()
        {
            var usuarioId = _sessaoService.ObterUsuarioId();
            var tipoUsuario = _sessaoService.ObterTipoUsuario();

            if (tipoUsuario != TipoUsuario.Organizacao)
            {
                return RedirectToAction("AcessoNegado", "Usuario");
            }

            ViewBag.OngId = usuarioId;
            return View();
        }

        // POST: PedidoDoacao/Criar
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Criar(int ongId, string categoria, string descricao)
        {
            var usuarioId = _sessaoService.ObterUsuarioId();
            var tipoUsuario = _sessaoService.ObterTipoUsuario();

            if (tipoUsuario != TipoUsuario.Organizacao || usuarioId != ongId)
            {
                return RedirectToAction("AcessoNegado", "Usuario");
            }

            ViewBag.OngId = ongId;

            if (ModelState.IsValid)
            {
                try
                {
                    await _pedidoDoacaoService.CriarPedidoDoacao(ongId, categoria, descricao);
                    TempData["SuccessMessage"] = "Pedido de doação criado com sucesso!";
                    return RedirectToAction("MeusPedidos");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    TempData["ErrorMessage"] = "Erro ao criar pedido: " + ex.Message;
                }
            }

            return View();
        }

        // GET: PedidoDoacao/MeusPedidos
        [Authorize]
        public async Task<IActionResult> MeusPedidos()
        {
            var usuarioId = _sessaoService.ObterUsuarioId();
            var tipoUsuario = _sessaoService.ObterTipoUsuario();

            if (tipoUsuario != TipoUsuario.Organizacao)
            {
                return RedirectToAction("AcessoNegado", "Usuario");
            }

            ViewBag.OngId = usuarioId;
            var pedidos = await _pedidoDoacaoService.ObterPedidosDoacaoPorOng(usuarioId);
            return View(pedidos);
        }

        // GET: PedidoDoacao/Detalhes
        [Authorize]
        public async Task<IActionResult> Detalhes(int id)
        {
            var pedido = await _pedidoDoacaoService.ObterPedidoDoacaoPorId(id);

            if (pedido == null)
            {
                return NotFound();
            }

            var usuarioId = _sessaoService.ObterUsuarioId();
            var tipoUsuario = _sessaoService.ObterTipoUsuario();

            // Verificar se o usuário tem permissão para ver os detalhes
            if (tipoUsuario == TipoUsuario.Organizacao && pedido.OngId != usuarioId)
            {
                return RedirectToAction("AcessoNegado", "Usuario");
            }

            ViewBag.Doacoes = await _doacaoService.ObterDoacoesPorPedido(id);

            return View(pedido);
        }

        // GET: PedidoDoacao/AtualizarStatus
        [Authorize]
        public async Task<IActionResult> AtualizarStatus(int id, string novoStatus)
        {
            var pedido = await _pedidoDoacaoService.ObterPedidoDoacaoPorId(id);

            if (pedido == null)
            {
                return NotFound();
            }

            var usuarioId = _sessaoService.ObterUsuarioId();
            var tipoUsuario = _sessaoService.ObterTipoUsuario();

            // Verificar se o usuário tem permissão para atualizar o status
            if (tipoUsuario != TipoUsuario.Organizacao || pedido.OngId != usuarioId)
            {
                return RedirectToAction("AcessoNegado", "Usuario");
            }
            try
            {
                await _pedidoDoacaoService.AtualizarStatusPedidoDoacao(id, novoStatus);
                TempData["SuccessMessage"] = $"Status do pedido alterado para {novoStatus} com sucesso!";
                return RedirectToAction("Detalhes", new { id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erro ao atualizar status: " + ex.Message;
                return RedirectToAction("Detalhes", new { id });
            }
        }
    }
}