using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Ong.Models;

namespace Ong.Services
{
    public class SessaoService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Ong.Data.DbOng _context;

        public SessaoService(IHttpContextAccessor httpContextAccessor, Ong.Data.DbOng context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }        public async Task LogarUsuario(Usuario usuario)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Email, usuario.EmailPrincipal),
                new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString()),
                new Claim(ClaimTypes.Role, usuario.Tipo.ToString())
            };

            var identidade = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identidade);

            if (_httpContextAccessor.HttpContext != null)
            {
                await _httpContextAccessor.HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTime.UtcNow.AddDays(7)
                    });
                    
                _httpContextAccessor.HttpContext.Session.SetInt32("UsuarioId", usuario.UsuarioId);
                _httpContextAccessor.HttpContext.Session.SetInt32("TipoUsuario", (int)usuario.Tipo);
            }
        }        public async Task Logout()
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                
                _httpContextAccessor.HttpContext.Session.Remove("UsuarioId");
                _httpContextAccessor.HttpContext.Session.Remove("TipoUsuario");
            }
        }

        public bool EstaAutenticado()
        {
            return _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true;
        }

        public int ObterUsuarioId()
        {
            if (!EstaAutenticado() || _httpContextAccessor.HttpContext == null)
                return 0;

            var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }        public TipoUsuario ObterTipoUsuario()
        {
            if (!EstaAutenticado() || _httpContextAccessor.HttpContext == null)
                return 0;

            var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role);
            return claim != null ? Enum.Parse<TipoUsuario>(claim.Value) : 0;
        }

        public async Task<Usuario?> ObterUsuarioAtual()
        {
            var usuarioId = ObterUsuarioId();
            if (usuarioId == 0)
                return null;

            return await _context.Usuarios.FindAsync(usuarioId);
        }
    }
}