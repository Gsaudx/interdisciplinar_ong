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

        public IActionResult Cadastro()
        {
            ViewBag.GoogleMapsApiKey = _configuration["GoogleMaps:ApiKey"];
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cadastro(Usuario usuario, string senha, string confirmacaoSenha)
        {
            ViewBag.GoogleMapsApiKey = _configuration["GoogleMaps:ApiKey"];

            if (senha != confirmacaoSenha) {
                ModelState.AddModelError("ConfirmacaoSenha", "As senhas não coincidem.");
                return View(usuario);
            }

            try
            {
                if (usuario.Tipo == TipoUsuario.Doador) {
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
                } else if (usuario.Tipo == TipoUsuario.Voluntario) {
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
                } else if (usuario.Tipo == TipoUsuario.Organizacao) {
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
                } else {
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

        public IActionResult Login(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

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

        public async Task<IActionResult> Logout()
        {
            await _sessaoService.Logout();
            return RedirectToAction("Index", "Home");
        }

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

        [Authorize]
        public async Task<IActionResult> Editar()
        {
            var usuarioId = _sessaoService.ObterUsuarioId();
            var usuario = await _usuarioService.ObterUsuarioPorId(usuarioId);

            if (usuario == null)
            {
                return NotFound();
            }

            ViewBag.GoogleMapsApiKey = _configuration["GoogleMaps:ApiKey"];
            return View(usuario);
        }        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Editar(Usuario usuario)
        {
            ViewBag.GoogleMapsApiKey = _configuration["GoogleMaps:ApiKey"];
            int usuarioId = _sessaoService.ObterUsuarioId();

            string senhaAtual = Request.Form["Senha"].ToString();
            if (string.IsNullOrEmpty(senhaAtual))
            {
                ModelState.AddModelError("", "A senha atual é obrigatória.");
                var userCompleto = await _usuarioService.ObterUsuarioPorId(usuarioId);
                return View(userCompleto);
            }

            bool senhaCorreta = await _usuarioService.VerificarSenha(usuarioId, senhaAtual);
            if (!senhaCorreta)
            {
                ModelState.AddModelError("", "Senha atual incorreta.");
                var userCompleto = await _usuarioService.ObterUsuarioPorId(usuarioId);
                return View(userCompleto);
            }

            if (ModelState.ContainsKey("Contatos"))
            {
                ModelState.Remove("Contatos");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    usuario.UsuarioId = _sessaoService.ObterUsuarioId();
                    usuario.Tipo = _sessaoService.ObterTipoUsuario();

                    if (usuario.Tipo == TipoUsuario.Doador)
                    {
                        var doador = new Doador
                        {
                            UsuarioId = usuario.UsuarioId,
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
                            Cpf = Request.Form.ContainsKey("Cpf") ? Request.Form["Cpf"].ToString() : string.Empty
                        };

                        if (Request.Form.ContainsKey("DataNascimento") && !string.IsNullOrEmpty(Request.Form["DataNascimento"]))
                        {
                            if (DateTime.TryParse(Request.Form["DataNascimento"], out DateTime dataNasc))
                            {
                                doador.DataNascimento = dataNasc;
                            }
                        }

                        await _usuarioService.AtualizarUsuario(doador);

                        var usuarioAtualizado = await _usuarioService.ObterUsuarioPorId(usuario.UsuarioId);
                        return RedirectToAction("Perfil");
                    }
                    else if (usuario.Tipo == TipoUsuario.Voluntario)
                    {
                        var voluntario = new Voluntario
                        {
                            UsuarioId = usuario.UsuarioId,
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
                            Profissao = Request.Form.ContainsKey("Profissao") ? Request.Form["Profissao"].ToString() : string.Empty,
                            Disponibilidade = Request.Form.ContainsKey("Disponibilidade") ? Request.Form["Disponibilidade"].ToString() : string.Empty
                        };

                        if (Request.Form.ContainsKey("DataNascimento") && !string.IsNullOrEmpty(Request.Form["DataNascimento"]))
                        {
                            if (DateTime.TryParse(Request.Form["DataNascimento"], out DateTime dataNasc))
                            {
                                voluntario.DataNascimento = dataNasc;
                            }
                        }

                        await _usuarioService.AtualizarUsuario(voluntario);

                        var usuarioAtualizado = await _usuarioService.ObterUsuarioPorId(usuario.UsuarioId);
                        return RedirectToAction("Perfil");
                    }
                    else if (usuario.Tipo == TipoUsuario.Organizacao)
                    {
                        var ong = new Models.Ong
                        {
                            UsuarioId = usuario.UsuarioId,
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
                            RazaoSocial = Request.Form.ContainsKey("RazaoSocial") ? Request.Form["RazaoSocial"].ToString() : string.Empty,
                            NomeFantasia = Request.Form.ContainsKey("NomeFantasia") ? Request.Form["NomeFantasia"].ToString() : string.Empty,
                            Descricao = Request.Form.ContainsKey("Descricao") ? Request.Form["Descricao"].ToString() : string.Empty
                        };

                        if (Request.Form.ContainsKey("DataFundacao") && !string.IsNullOrEmpty(Request.Form["DataFundacao"]))
                        {
                            if (DateTime.TryParse(Request.Form["DataFundacao"], out DateTime dataFund))
                            {
                                ong.DataFundacao = dataFund;
                            }
                        }

                        await _usuarioService.AtualizarUsuario(ong);

                        var usuarioAtualizado = await _usuarioService.ObterUsuarioPorId(usuario.UsuarioId);
                        return RedirectToAction("Perfil");
                    }
                    else
                    {
                        await _usuarioService.AtualizarUsuario(usuario);
                        return RedirectToAction("Perfil");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Erro ao atualizar usuário: {ex.Message}");
                }
            }

            var usuarioCompleto = await _usuarioService.ObterUsuarioPorId(usuario.UsuarioId);
            return View(usuarioCompleto);
        }

        public IActionResult AcessoNegado()
        {
            return View();
        }

        public async Task<IActionResult> BuscarOngsProximas(double latitude, double longitude, double raioKm = 10)
        {
            var ongs = await _usuarioService.ObterONGsProximas(latitude, longitude, raioKm);
            return View(ongs);
        }
    }
}