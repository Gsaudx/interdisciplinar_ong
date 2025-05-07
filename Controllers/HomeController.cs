using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Ong.Models;
using Ong.Services;

namespace Ong.Controllers
{
    public class HomeController : Controller
    {
        private readonly UsuarioService _usuarioService;
        private readonly EventoService _eventoService;
        private readonly PedidoDoacaoService _pedidoDoacaoService;
        private readonly IConfiguration _configuration;

        public HomeController(
            UsuarioService usuarioService, 
            EventoService eventoService, 
            PedidoDoacaoService pedidoDoacaoService,
            IConfiguration configuration)
        {
            _usuarioService = usuarioService;
            _eventoService = eventoService;
            _pedidoDoacaoService = pedidoDoacaoService;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            // Carregar ONGs para o mapa
            ViewBag.Ongs = await _usuarioService.ObterUsuariosPorTipo(TipoUsuario.Organizacao);
            
            // Carregar próximos eventos
            ViewBag.ProximosEventos = await _eventoService.ObterEventosFuturos();
            
            // Carregar pedidos de doação abertos
            ViewBag.PedidosDoacaoAbertos = await _pedidoDoacaoService.ObterPedidosDoacaoPorStatus("Aberto");
            
            // Passar a API key do Google Maps para a view
            ViewBag.GoogleMapsApiKey = _configuration["GoogleMaps:ApiKey"];
            
            return View();
        }

        public IActionResult Sobre()
        {
            return View();
        }

        public IActionResult Contato()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class ErrorViewModel
    {
        public string RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}