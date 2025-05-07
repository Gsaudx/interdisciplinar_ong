using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ong.Models;
using Ong.Services;

namespace Ong.Controllers
{
    public class EventoController : Controller
    {
        private readonly EventoService _eventoService;
        private readonly UsuarioService _usuarioService;

        public EventoController(EventoService eventoService, UsuarioService usuarioService)
        {
            _eventoService = eventoService;
            _usuarioService = usuarioService;
        }

        // GET: Evento
        public async Task<IActionResult> Index()
        {
            var eventos = await _eventoService.ObterEventosFuturos();
            return View(eventos);
        }

        // GET: Evento/Criar
        public IActionResult Criar()
        {
            return View();
        }

        // POST: Evento/Criar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Criar(int ongId, string nome, string descricao, DateTime data, string localizacao)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _eventoService.CriarEvento(ongId, nome, descricao, data, localizacao);
                    return RedirectToAction("MeusEventos", new { id = ongId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            
            return View();
        }

        // GET: Evento/MeusEventos
        public async Task<IActionResult> MeusEventos(int ongId)
        {
            var eventos = await _eventoService.ObterEventosPorOng(ongId);
            return View(eventos);
        }

        // GET: Evento/Detalhes
        public async Task<IActionResult> Detalhes(int id)
        {
            // Obter detalhes do evento
            var eventos = await _eventoService.ObterEventosFuturos();
            var evento = eventos.FirstOrDefault(e => e.EventoId == id);
            
            if (evento == null)
            {
                return NotFound();
            }

            // Obter voluntários inscritos no evento
            ViewBag.Voluntarios = await _eventoService.ObterVoluntariosPorEvento(id);
            
            return View(evento);
        }

        // GET: Evento/Inscrever
        public async Task<IActionResult> Inscrever(int eventoId, int voluntarioId)
        {
            try
            {
                await _eventoService.InscreverVoluntarioEmEvento(voluntarioId, eventoId);
                return RedirectToAction("Detalhes", new { id = eventoId });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: Evento/MeusEventosVoluntario
        public async Task<IActionResult> MeusEventosVoluntario(int voluntarioId)
        {
            var eventos = await _eventoService.ObterEventosPorVoluntario(voluntarioId);
            return View(eventos);
        }

        // GET: Evento/AtualizarStatusInscricao
        public async Task<IActionResult> AtualizarStatusInscricao(int eventoId, int voluntarioId, string novoStatus)
        {
            try
            {
                await _eventoService.AtualizarStatusInscricao(voluntarioId, eventoId, novoStatus);
                return RedirectToAction("Detalhes", new { id = eventoId });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: Evento/Editar
        public async Task<IActionResult> Editar(int id)
        {
            var eventos = await _eventoService.ObterEventosFuturos();
            var evento = eventos.FirstOrDefault(e => e.EventoId == id);
            
            if (evento == null)
            {
                return NotFound();
            }
            
            return View(evento);
        }

        // POST: Evento/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, string nome, string descricao, DateTime data, string localizacao)
        {
            if (ModelState.IsValid)
            {
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
            
            return View();
        }
    }
}