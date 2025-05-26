using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Ong.Models;
using Ong.Services;

namespace Ong.Controllers
{
    public class EventoController : Controller
    {
        private readonly EventoService _eventoService;
        private readonly UsuarioService _usuarioService;
        private readonly SessaoService _sessaoService;

        public EventoController(
            EventoService eventoService, 
            UsuarioService usuarioService,
            SessaoService sessaoService)
        {
            _eventoService = eventoService;
            _usuarioService = usuarioService;
            _sessaoService = sessaoService;
        }

        public async Task<IActionResult> Index()
        {
            var eventos = await _eventoService.ObterEventosFuturos();
            return View(eventos);
        }

        [Authorize]
        public IActionResult Criar()
        {
            var usuarioId = _sessaoService.ObterUsuarioId();
            var tipoUsuario = _sessaoService.ObterTipoUsuario();

            if (tipoUsuario != TipoUsuario.Organizacao) {
                return RedirectToAction("AcessoNegado", "Usuario");
            }

            ViewBag.OngId = usuarioId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Criar(int ongId, string nome, string descricao, DateTime data, string localizacao)
        {
            var usuarioId = _sessaoService.ObterUsuarioId();
            var tipoUsuario = _sessaoService.ObterTipoUsuario();

            if (tipoUsuario != TipoUsuario.Organizacao || usuarioId != ongId) {
                return RedirectToAction("AcessoNegado", "Usuario");
            }

            ViewBag.OngId = ongId;

            if (ModelState.IsValid) {
                try
                {
                    await _eventoService.CriarEvento(ongId, nome, descricao, data, localizacao);
                    TempData["SuccessMessage"] = "Evento criado com sucesso!";
                    return RedirectToAction("MeusEventos");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    TempData["ErrorMessage"] = "Erro ao criar evento: " + ex.Message;
                }
            }
            
            return View();
        }

        [Authorize]
        public async Task<IActionResult> MeusEventos()
        {
            var usuarioId = _sessaoService.ObterUsuarioId();
            var tipoUsuario = _sessaoService.ObterTipoUsuario();

            if (tipoUsuario != TipoUsuario.Organizacao) {
                return RedirectToAction("AcessoNegado", "Usuario");
            }

            ViewBag.OngId = usuarioId;
            var eventos = await _eventoService.ObterEventosPorOng(usuarioId);
            return View(eventos);
        }
        
        public async Task<IActionResult> Detalhes(int id)
        {
            var evento = await _eventoService.ObterEventoPorId(id);

            if (evento == null) {
                return NotFound();
            }

            ViewBag.UsuarioId = _sessaoService.ObterUsuarioId();
            ViewBag.TipoUsuario = _sessaoService.ObterTipoUsuario();

            ViewBag.Voluntarios = await _eventoService.ObterVoluntariosPorEvento(id);

            return View(evento);
        }

        [Authorize(Roles = "Voluntario")]
        public async Task<IActionResult> Inscrever(int eventoId, int voluntarioId)
        {
            var usuarioId = _sessaoService.ObterUsuarioId();
            var tipoUsuario = _sessaoService.ObterTipoUsuario();

            if (tipoUsuario != TipoUsuario.Voluntario || usuarioId != voluntarioId) {
                return RedirectToAction("AcessoNegado", "Usuario");
            }
            
            try
            {
                await _eventoService.InscreverVoluntarioEmEvento(voluntarioId, eventoId);
                TempData["SuccessMessage"] = "Inscrição realizada com sucesso!";
                return RedirectToAction("Detalhes", new { id = eventoId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erro ao realizar inscrição: " + ex.Message;
                return RedirectToAction("Detalhes", new { id = eventoId });
            }
        }

        [Authorize]
        public async Task<IActionResult> MeusEventosVoluntario()
        {
            var usuarioId = _sessaoService.ObterUsuarioId();
            var tipoUsuario = _sessaoService.ObterTipoUsuario();

            if (tipoUsuario != TipoUsuario.Voluntario) {
                return RedirectToAction("AcessoNegado", "Usuario");
            }

            var eventos = await _eventoService.ObterEventosPorVoluntario(usuarioId);
            return View(eventos);
        }

        [Authorize]
        public async Task<IActionResult> AtualizarStatusInscricao(int eventoId, int voluntarioId, string novoStatus)
        {
            var evento = await _eventoService.ObterEventoPorId(eventoId);
            
            if (evento == null) {
                return NotFound();
            }

            var usuarioId = _sessaoService.ObterUsuarioId();
            var tipoUsuario = _sessaoService.ObterTipoUsuario();

            if ((tipoUsuario != TipoUsuario.Organizacao || evento.OngId != usuarioId) && (tipoUsuario != TipoUsuario.Voluntario || usuarioId != voluntarioId)) {
                return RedirectToAction("AcessoNegado", "Usuario");
            }

            try
            {
                await _eventoService.AtualizarStatusInscricao(voluntarioId, eventoId, novoStatus);
                TempData["SuccessMessage"] = $"Status da inscrição atualizado para {novoStatus} com sucesso!";
                return RedirectToAction("Detalhes", new { id = eventoId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erro ao atualizar status da inscrição: " + ex.Message;
                return RedirectToAction("Detalhes", new { id = eventoId });
            }
        }

        [Authorize]
        public async Task<IActionResult> Editar(int id)
        {
            var evento = await _eventoService.ObterEventoPorId(id);
            
            if (evento == null) {
                return NotFound();
            }
            
            var usuarioId = _sessaoService.ObterUsuarioId();
            var tipoUsuario = _sessaoService.ObterTipoUsuario();

            if (tipoUsuario != TipoUsuario.Organizacao || evento.OngId != usuarioId) {
                return RedirectToAction("AcessoNegado", "Usuario");
            }
            
            return View(evento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Editar(int id, string nome, string descricao, DateTime data, string localizacao)
        {
            var evento = await _eventoService.ObterEventoPorId(id);
            
            if (evento == null) {
                return NotFound();
            }
            
            var usuarioId = _sessaoService.ObterUsuarioId();
            var tipoUsuario = _sessaoService.ObterTipoUsuario();

            if (tipoUsuario != TipoUsuario.Organizacao || evento.OngId != usuarioId) {
                return RedirectToAction("AcessoNegado", "Usuario");
            }

            if (ModelState.IsValid) {
                try
                {
                    await _eventoService.AtualizarEvento(id, nome, descricao, data, localizacao);
                    return RedirectToAction("Detalhes", new { id });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            
            return View(evento);
        }
    }
}