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
        private readonly SessaoService _sessaoService;
        private readonly IConfiguration _configuration;

        public HomeController(
            UsuarioService usuarioService, 
            EventoService eventoService, 
            PedidoDoacaoService pedidoDoacaoService,
            SessaoService sessaoService,
            IConfiguration configuration)
        {
            _usuarioService = usuarioService;
            _eventoService = eventoService;
            _pedidoDoacaoService = pedidoDoacaoService;
            _sessaoService = sessaoService;
            _configuration = configuration;
        }        public async Task<IActionResult> Index()
        {
            ViewBag.Ongs = await _usuarioService.ObterUsuariosPorTipo(TipoUsuario.Organizacao);
            ViewBag.ProximosEventos = await _eventoService.ObterEventosFuturos();
            ViewBag.PedidosDoacaoAbertos = await _pedidoDoacaoService.ObterPedidosDoacaoPorStatus("Aberto");
            ViewBag.GoogleMapsApiKey = _configuration["GoogleMaps:ApiKey"];
            
            if (_sessaoService.EstaAutenticado())
            {
                var usuarioAtual = await _sessaoService.ObterUsuarioAtual();
                if (usuarioAtual != null && usuarioAtual.Latitude.HasValue && usuarioAtual.Longitude.HasValue)
                {
                    ViewBag.UsuarioLatitude = usuarioAtual.Latitude;
                    ViewBag.UsuarioLongitude = usuarioAtual.Longitude;
                    
                    // Buscar ONGs próximas (raio de 10km)
                    ViewBag.OngsProximas = await _usuarioService.ObterONGsProximas(
                        usuarioAtual.Latitude.Value, 
                        usuarioAtual.Longitude.Value, 
                        10
                    );
                }
            }
            
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
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}