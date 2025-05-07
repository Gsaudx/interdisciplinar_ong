// filepath: c:\Programming\interdisciplinar_ong\Controllers\UsuarioController.cs
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Ong.Models;
using Ong.Services;

namespace Ong.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly UsuarioService _usuarioService;
        private readonly SessaoService _sessaoService;
        private readonly IConfiguration _configuration;

        public UsuarioController(
            UsuarioService usuarioService, 
            SessaoService sessaoService,
            IConfiguration configuration)
        {
            _usuarioService = usuarioService;
            _sessaoService = sessaoService;
            _configuration = configuration;
        }

        // GET: Usuario/Cadastro
        public IActionResult Cadastro()
        {
            ViewBag.GoogleMapsApiKey = _configuration["GoogleMaps:ApiKey"];
            return View();
        }

        // POST: Usuario/Cadastro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cadastro(Usuario usuario, string senha, string confirmacaoSenha)
        {
            // Retornar API key para a view em caso de erro
            ViewBag.GoogleMapsApiKey = _configuration["GoogleMaps:ApiKey"];
            
            if (senha != confirmacaoSenha)
            {
                ModelState.AddModelError("ConfirmacaoSenha", "As senhas não coincidem.");
                return View(usuario);
            }

            try
            {
                // Tratar dados específicos com base no tipo de usuário
                if (usuario.Tipo == TipoUsuario.Doador)
                {
                    // Criar um doador
                    var doador = new Doador
                    {
                        Nome = usuario.Nome,
                        EmailPrincipal = usuario.EmailPrincipal,
                        Telefone = usuario.Telefone,
                        Cep = usuario.Cep,
                        Logradouro = usuario.Logradouro,
                        Numero = usuario.Numero,
                        Complemento = usuario.Complemento,
                        Bairro = usuario.Bairro,
                        Cidade = usuario.Cidade,
                        Estado = usuario.Estado,
                        Latitude = usuario.Latitude,
                        Longitude = usuario.Longitude,
                        Tipo = TipoUsuario.Doador,
                        Cpf = Request.Form.ContainsKey("Cpf") ? Request.Form["Cpf"].ToString() : string.Empty,
                        DataNascimento = Request.Form.ContainsKey("DataNascimento") && !string.IsNullOrEmpty(Request.Form["DataNascimento"]) ? 
                            DateTime.Parse(Request.Form["DataNascimento"]) : null
                    };
                    var novoUsuario = await _usuarioService.CriarUsuario(doador, senha);
                    await _sessaoService.LogarUsuario(novoUsuario);
                    return RedirectToAction("Index", "Home");
                }
                else if (usuario.Tipo == TipoUsuario.Voluntario)
                {
                    // Criar um voluntário
                    var voluntario = new Voluntario
                    {
                        Nome = usuario.Nome,
                        EmailPrincipal = usuario.EmailPrincipal,
                        Telefone = usuario.Telefone,
                        Cep = usuario.Cep,
                        Logradouro = usuario.Logradouro,
                        Numero = usuario.Numero,
                        Complemento = usuario.Complemento,
                        Bairro = usuario.Bairro,
                        Cidade = usuario.Cidade,
                        Estado = usuario.Estado,
                        Latitude = usuario.Latitude,
                        Longitude = usuario.Longitude,
                        Tipo = TipoUsuario.Voluntario,
                        Cpf = Request.Form.ContainsKey("Cpf") ? Request.Form["Cpf"].ToString() : string.Empty,
                        DataNascimento = Request.Form.ContainsKey("DataNascimento") && !string.IsNullOrEmpty(Request.Form["DataNascimento"]) ? 
                            DateTime.Parse(Request.Form["DataNascimento"]) : null,
                        Profissao = Request.Form.ContainsKey("Profissao") ? Request.Form["Profissao"].ToString() : string.Empty,
                        Disponibilidade = Request.Form.ContainsKey("Disponibilidade") ? Request.Form["Disponibilidade"].ToString() : string.Empty
                    };
                    var novoUsuario = await _usuarioService.CriarUsuario(voluntario, senha);
                    await _sessaoService.LogarUsuario(novoUsuario);
                    return RedirectToAction("Index", "Home");
                }
                else if (usuario.Tipo == TipoUsuario.Organizacao)
                {
                    // Criar uma ONG
                    var ong = new Models.Ong
                    {
                        Nome = usuario.Nome,
                        EmailPrincipal = usuario.EmailPrincipal,
                        Telefone = usuario.Telefone,
                        Cep = usuario.Cep,
                        Logradouro = usuario.Logradouro,
                        Numero = usuario.Numero,
                        Complemento = usuario.Complemento,
                        Bairro = usuario.Bairro,
                        Cidade = usuario.Cidade,
                        Estado = usuario.Estado,
                        Latitude = usuario.Latitude,
                        Longitude = usuario.Longitude,
                        Tipo = TipoUsuario.Organizacao,
                        Cnpj = Request.Form.ContainsKey("Cnpj") ? Request.Form["Cnpj"].ToString() : string.Empty,
                        DataFundacao = Request.Form.ContainsKey("DataFundacao") && !string.IsNullOrEmpty(Request.Form["DataFundacao"]) ? 
                            DateTime.Parse(Request.Form["DataFundacao"]) : null,
                        RazaoSocial = Request.Form.ContainsKey("RazaoSocial") ? Request.Form["RazaoSocial"].ToString() : string.Empty,
                        NomeFantasia = Request.Form.ContainsKey("NomeFantasia") ? Request.Form["NomeFantasia"].ToString() : string.Empty,
                        Descricao = Request.Form.ContainsKey("Descricao") ? Request.Form["Descricao"].ToString() : string.Empty
                    };
                    var novoUsuario = await _usuarioService.CriarUsuario(ong, senha);
                    await _sessaoService.LogarUsuario(novoUsuario);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Criar um usuário genérico se dados específicos não forem fornecidos
                    var novoUsuario = await _usuarioService.CriarUsuario(usuario, senha);
                    await _sessaoService.LogarUsuario(novoUsuario);
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar usuário: {ex.Message}");
                ModelState.AddModelError("", $"Erro ao criar usuário: {ex.Message}");
                return View(usuario);
            }
        }

        // GET: Usuario/Login
        public IActionResult Login(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: Usuario/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string senha, string returnUrl = null)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha))
            {
                ModelState.AddModelError("", "Email e senha são obrigatórios.");
                return View();
            }

            var usuario = await _usuarioService.AutenticarUsuario(email, senha);

            if (usuario == null)
            {
                ModelState.AddModelError("", "Email ou senha inválidos.");
                return View();
            }

            await _sessaoService.LogarUsuario(usuario);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        // GET: Usuario/Logout
        public async Task<IActionResult> Logout()
        {
            await _sessaoService.Logout();
            return RedirectToAction("Index", "Home");
        }

        // GET: Usuario/Perfil
        [Authorize]
        public async Task<IActionResult> Perfil()
        {
            var usuarioId = _sessaoService.ObterUsuarioId();
            var usuario = await _usuarioService.ObterUsuarioPorId(usuarioId);
            
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuario/Editar
        [Authorize]
        public async Task<IActionResult> Editar()
        {
            var usuarioId = _sessaoService.ObterUsuarioId();
            var usuario = await _usuarioService.ObterUsuarioPorId(usuarioId);
            
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuario/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Editar(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                // Implementar lógica para atualizar o usuário
                return RedirectToAction("Perfil");
            }

            return View(usuario);
        }

        // GET: Usuario/AcessoNegado
        public IActionResult AcessoNegado()
        {
            return View();
        }

        // GET: Usuario/BuscarOngsProximas
        public async Task<IActionResult> BuscarOngsProximas(double latitude, double longitude, double raioKm = 10)
        {
            var ongs = await _usuarioService.ObterONGsProximas(latitude, longitude, raioKm);
            return View(ongs);
        }
    }
}