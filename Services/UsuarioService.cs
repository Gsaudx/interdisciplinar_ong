// filepath: c:\Programming\interdisciplinar_ong\Services\UsuarioService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ong.Data;
using Ong.Models;

namespace Ong.Services
{
    public class UsuarioService
    {
        private readonly DbOng _context;

        public UsuarioService(DbOng context)
        {
            _context = context;
        }

        public async Task<Usuario> CriarUsuario(Usuario usuario, string senha)
        {
            // Verificar se o email já existe
            if (await _context.Usuarios.AnyAsync(u => u.EmailPrincipal == usuario.EmailPrincipal))
            {
                throw new Exception("Este email já está sendo utilizado.");
            }

            // Hash da senha
            usuario.Senha = PasswordHelper.HashPassword(senha);
            
            // Definir data de cadastro
            usuario.DataCadastro = DateTime.Now;

            // Adicionar usuário ao contexto
            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();

            return usuario;
        }

        public async Task<Usuario> AutenticarUsuario(string email, string senha)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.EmailPrincipal == email);

            if (usuario == null)
            {
                return null;
            }

            bool senhaCorreta = PasswordHelper.VerifyPassword(senha, usuario.Senha);

            return senhaCorreta ? usuario : null;
        }

        public async Task<Usuario> ObterUsuarioPorId(int id)
        {
            return await _context.Usuarios.FindAsync(id);
        }

        public async Task<List<Usuario>> ObterUsuariosPorTipo(TipoUsuario tipo)
        {
            return await _context.Usuarios
                .Where(u => u.Tipo == tipo)
                .ToListAsync();
        }

        public async Task<List<Usuario>> ObterONGsProximas(double latitude, double longitude, double raioKm)
        {
            // Implementação simples de cálculo de distância euclidiana
            // Para uma implementação mais precisa, considere usar fórmulas de distância geográfica como Haversine
            var ongs = await _context.Usuarios
                .Where(u => u.Tipo == TipoUsuario.Organizacao && u.Latitude.HasValue && u.Longitude.HasValue)
                .ToListAsync();

            return ongs.Where(o => CalcularDistancia(latitude, longitude, o.Latitude.Value, o.Longitude.Value) <= raioKm).ToList();
        }

        private double CalcularDistancia(double lat1, double lon1, double lat2, double lon2)
        {
            // Implementação simples usando a fórmula de Haversine
            // Raio da Terra em km
            const double R = 6371;
            
            var dLat = Deg2Rad(lat2 - lat1);
            var dLon = Deg2Rad(lon2 - lon1);
            
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(Deg2Rad(lat1)) * Math.Cos(Deg2Rad(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var d = R * c; // Distância em km
            
            return d;
        }

        private double Deg2Rad(double deg)
        {
            return deg * (Math.PI / 180);
        }
    }
}