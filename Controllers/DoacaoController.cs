using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Ong.Models;
using Ong.Services;

namespace Ong.Controllers
{
    public class DoacaoController : Controller
    {
        private readonly DoacaoService _doacaoService;
        private readonly UsuarioService _usuarioService;
        private readonly PedidoDoacaoService _pedidoDoacaoService;
        private readonly SessaoService _sessaoService;

        public DoacaoController(
            DoacaoService doacaoService,
            UsuarioService usuarioService,
            PedidoDoacaoService pedidoDoacaoService,
            SessaoService sessaoService)
        {
            _doacaoService = doacaoService;
            _usuarioService = usuarioService;
            _pedidoDoacaoService = pedidoDoacaoService;
            _sessaoService = sessaoService;
        }

        public async Task<IActionResult> Realizar(int? pedidoId)
        {
            var usuarioId = _sessaoService.ObterUsuarioId();
            var tipoUsuario = _sessaoService.ObterTipoUsuario();

            if (usuarioId == 0 || tipoUsuario != TipoUsuario.Doador) {
                TempData["ErrorMessage"] = "Você precisa estar logado como Doador para realizar uma doação.";
                return RedirectToAction("Login", "Usuario");
            }

            ViewBag.DoadorId = usuarioId;
            ViewBag.ONGs = await _usuarioService.ObterUsuariosPorTipo(TipoUsuario.Organizacao);

            if (pedidoId.HasValue) {
                ViewBag.PedidoDoacao = await _pedidoDoacaoService.ObterPedidoDoacaoPorId(pedidoId.Value);
            }

            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Realizar(int doadorId, int ongId, int? pedidoDoacaoId, string categoria, string descricao)
        {
            var usuarioId = _sessaoService.ObterUsuarioId();
            var tipoUsuario = _sessaoService.ObterTipoUsuario();

            if (usuarioId == 0 || tipoUsuario != TipoUsuario.Doador || usuarioId != doadorId) {
                TempData["ErrorMessage"] = "Você precisa estar logado como Doador para realizar uma doação.";
                return RedirectToAction("Login", "Usuario");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _doacaoService.RealizarDoacao(doadorId, ongId, pedidoDoacaoId, categoria, descricao);
                    TempData["SuccessMessage"] = "Doação realizada com sucesso! Obrigado por sua contribuição.";
                    return RedirectToAction("MinhasDoacoes", new { doadorId = doadorId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    TempData["ErrorMessage"] = "Erro ao realizar doação: " + ex.Message;
                }
            }

            ViewBag.DoadorId = doadorId;
            ViewBag.ONGs = await _usuarioService.ObterUsuariosPorTipo(TipoUsuario.Organizacao);
            
            if (pedidoDoacaoId.HasValue)
            {
                ViewBag.PedidoDoacao = await _pedidoDoacaoService.ObterPedidoDoacaoPorId(pedidoDoacaoId.Value);
            }
            
            return View();
        }
        
        public async Task<IActionResult> MinhasDoacoes(int? doadorId = null)
        {
            var usuarioId = _sessaoService.ObterUsuarioId();
            var tipoUsuario = _sessaoService.ObterTipoUsuario();

            if (usuarioId == 0 || tipoUsuario != TipoUsuario.Doador) {
                TempData["ErrorMessage"] = "Você precisa estar logado como Doador para ver suas doações.";
                return RedirectToAction("Login", "Usuario");
            }

            if (!doadorId.HasValue) {
                doadorId = usuarioId;
            } else if (doadorId.Value != usuarioId) {
                TempData["ErrorMessage"] = "Você só pode ver suas próprias doações.";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.DoadorId = doadorId.Value;
            var doacoes = await _doacaoService.ObterDoacoesPorDoador(doadorId.Value);
            return View(doacoes);
        }

        public async Task<IActionResult> DoacoesRecebidas(int? ongId = null)
        {
            var usuarioId = _sessaoService.ObterUsuarioId();
            var tipoUsuario = _sessaoService.ObterTipoUsuario();

            if (usuarioId == 0 || tipoUsuario != TipoUsuario.Organizacao) {
                TempData["ErrorMessage"] = "Você precisa estar logado como ONG para ver doações recebidas.";
                return RedirectToAction("Login", "Usuario");
            }

            if (!ongId.HasValue) {
                ongId = usuarioId;
            } else if (ongId.Value != usuarioId) {
                TempData["ErrorMessage"] = "Você só pode ver doações recebidas pela sua ONG.";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.OngId = ongId.Value;
            var doacoes = await _doacaoService.ObterDoacoesPorOng(ongId.Value);
            return View(doacoes);
        }

        public async Task<IActionResult> DoacoesPorPedido(int pedidoId)
        {
            var usuarioId = _sessaoService.ObterUsuarioId();
            var tipoUsuario = _sessaoService.ObterTipoUsuario();

            if (usuarioId == 0) {
                TempData["ErrorMessage"] = "Você precisa estar logado para ver doações por pedido.";
                return RedirectToAction("Login", "Usuario");
            }

            var pedido = await _pedidoDoacaoService.ObterPedidoDoacaoPorId(pedidoId);
            if (pedido == null) {
                TempData["ErrorMessage"] = "Pedido de doação não encontrado.";
                return RedirectToAction("Index", "Home");
            }

            if (tipoUsuario == TipoUsuario.Organizacao && pedido.OngId != usuarioId) {
                TempData["ErrorMessage"] = "Você só pode ver doações para pedidos da sua ONG.";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.PedidoId = pedidoId;
            ViewBag.Pedido = pedido;
            var doacoes = await _doacaoService.ObterDoacoesPorPedido(pedidoId);
            return View(doacoes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AtualizarStatus(int doacaoId, StatusDoacao novoStatus)
        {
            var usuarioId = _sessaoService.ObterUsuarioId();
            var tipoUsuario = _sessaoService.ObterTipoUsuario();

            if (usuarioId == 0 || tipoUsuario != TipoUsuario.Organizacao)
            {
                TempData["ErrorMessage"] = "Você precisa estar logado como ONG para alterar status de doações.";
                return RedirectToAction("Login", "Usuario");
            }

            try
            {
                await _doacaoService.AtualizarStatusDoacao(doacaoId, novoStatus, usuarioId);
                
                string statusTexto = novoStatus switch
                {
                    StatusDoacao.Entregue => "Entregue",
                    StatusDoacao.Cancelada => "Cancelada",
                    StatusDoacao.Criada => "Criada",
                    _ => "Atualizado"
                };

                TempData["SuccessMessage"] = $"Status da doação alterado para {statusTexto} com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erro ao atualizar status: " + ex.Message;
            }

            return RedirectToAction("DoacoesRecebidas");
        }
    }
}