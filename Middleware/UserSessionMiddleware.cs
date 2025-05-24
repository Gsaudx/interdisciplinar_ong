using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace Ong.Middleware
{
    public class UserSessionMiddleware
    {
        private readonly RequestDelegate _next;

        public UserSessionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Se o usuário estiver autenticado, sincroniza os dados com a sessão
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var usuarioIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
                var tipoUsuarioClaim = context.User.FindFirst(ClaimTypes.Role);

                if (usuarioIdClaim != null && context.Session.GetInt32("UsuarioId") == null)
                {
                    context.Session.SetInt32("UsuarioId", int.Parse(usuarioIdClaim.Value));
                }

                if (tipoUsuarioClaim != null && context.Session.GetInt32("TipoUsuario") == null)
                {
                    if (Enum.TryParse<Ong.Models.TipoUsuario>(tipoUsuarioClaim.Value, out var tipoUsuario))
                    {
                        context.Session.SetInt32("TipoUsuario", (int)tipoUsuario);
                    }
                }
            }

            await _next(context);
        }
    }

    // Método de extensão para adicionar o middleware à pipeline
    public static class UserSessionMiddlewareExtensions
    {
        public static IApplicationBuilder UseUserSession(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UserSessionMiddleware>();
        }
    }
}
