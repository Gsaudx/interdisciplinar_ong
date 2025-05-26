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
            if (await _context.Usuarios.AnyAsync(u => u.EmailPrincipal == usuario.EmailPrincipal))
            {
                throw new Exception("Este email já está sendo utilizado.");
            }

            usuario.Senha = PasswordHelper.HashPassword(senha);
            
            usuario.DataCadastro = DateTime.Now;

            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();

            return usuario;
        }

        public async Task<Usuario> AtualizarUsuario(Usuario usuario)
        {
            var usuarioComMesmoEmail = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.EmailPrincipal == usuario.EmailPrincipal && u.UsuarioId != usuario.UsuarioId);

            if (usuarioComMesmoEmail != null)
            {
                throw new Exception("Este email já está sendo utilizado por outro usuário.");
            }

            var usuarioAtual = await _context.Usuarios.FindAsync(usuario.UsuarioId);

            if (usuarioAtual == null)
            {
                throw new Exception("Usuário não encontrado.");
            }

            usuarioAtual.Nome = usuario.Nome;
            usuarioAtual.EmailPrincipal = usuario.EmailPrincipal;
            usuarioAtual.Telefone = usuario.Telefone;
            usuarioAtual.Logradouro = usuario.Logradouro;
            usuarioAtual.Numero = usuario.Numero;
            usuarioAtual.Complemento = usuario.Complemento;
            usuarioAtual.Bairro = usuario.Bairro;
            usuarioAtual.Cidade = usuario.Cidade;
            usuarioAtual.Estado = usuario.Estado;
            usuarioAtual.Cep = usuario.Cep;
            usuarioAtual.Latitude = usuario.Latitude;
            usuarioAtual.Longitude = usuario.Longitude;

            if (usuarioAtual.Contatos != null && usuario.Contatos == null)
            {
                usuario.Contatos = usuarioAtual.Contatos;
            }

            try
            {
                if (usuarioAtual.Tipo == TipoUsuario.Doador)
                {
                    var doadorAtual = await _context.Doadores.FindAsync(usuario.UsuarioId);
                    if (doadorAtual != null)
                    {
                        if (usuario is Doador doador)
                        {
                            doadorAtual.Cpf = doador.Cpf;
                            doadorAtual.DataNascimento = doador.DataNascimento;
                        }
                        else
                        {
                            var cpf = usuario.GetType().GetProperty("Cpf")?.GetValue(usuario)?.ToString();
                            var dataNascimentoStr = usuario.GetType().GetProperty("DataNascimento")?.GetValue(usuario)?.ToString();

                            if (!string.IsNullOrEmpty(cpf))
                                doadorAtual.Cpf = cpf;

                            if (!string.IsNullOrEmpty(dataNascimentoStr) && DateTime.TryParse(dataNascimentoStr, out DateTime dataNascimento))
                                doadorAtual.DataNascimento = dataNascimento;
                        }
                    }
                }
                else if (usuarioAtual.Tipo == TipoUsuario.Voluntario)
                {
                    var voluntarioAtual = await _context.Voluntarios.FindAsync(usuario.UsuarioId);
                    if (voluntarioAtual != null)
                    {
                        if (usuario is Voluntario voluntario)
                        {
                            voluntarioAtual.Cpf = voluntario.Cpf;
                            voluntarioAtual.DataNascimento = voluntario.DataNascimento;
                            voluntarioAtual.Profissao = voluntario.Profissao;
                            voluntarioAtual.Disponibilidade = voluntario.Disponibilidade;
                        }
                        else
                        {
                            var cpf = usuario.GetType().GetProperty("Cpf")?.GetValue(usuario)?.ToString();
                            var dataNascimentoStr = usuario.GetType().GetProperty("DataNascimento")?.GetValue(usuario)?.ToString();
                            var profissao = usuario.GetType().GetProperty("Profissao")?.GetValue(usuario)?.ToString();
                            var disponibilidade = usuario.GetType().GetProperty("Disponibilidade")?.GetValue(usuario)?.ToString();

                            if (!string.IsNullOrEmpty(cpf))
                                voluntarioAtual.Cpf = cpf;

                            if (!string.IsNullOrEmpty(dataNascimentoStr) && DateTime.TryParse(dataNascimentoStr, out DateTime dataNascimento))
                                voluntarioAtual.DataNascimento = dataNascimento;

                            if (!string.IsNullOrEmpty(profissao))
                                voluntarioAtual.Profissao = profissao;

                            if (!string.IsNullOrEmpty(disponibilidade))
                                voluntarioAtual.Disponibilidade = disponibilidade;
                        }
                    }
                }
                else if (usuarioAtual.Tipo == TipoUsuario.Organizacao)
                {
                    var ongAtual = await _context.Ongs.FindAsync(usuario.UsuarioId);
                    if (ongAtual != null)
                    {
                        if (usuario is Models.Ong ong)
                        {
                            ongAtual.Cnpj = ong.Cnpj;
                            ongAtual.RazaoSocial = ong.RazaoSocial;
                            ongAtual.NomeFantasia = ong.NomeFantasia;
                            ongAtual.DataFundacao = ong.DataFundacao;
                            ongAtual.Descricao = ong.Descricao;
                        }
                        else
                        {
                            var cnpj = usuario.GetType().GetProperty("Cnpj")?.GetValue(usuario)?.ToString();
                            var razaoSocial = usuario.GetType().GetProperty("RazaoSocial")?.GetValue(usuario)?.ToString();
                            var nomeFantasia = usuario.GetType().GetProperty("NomeFantasia")?.GetValue(usuario)?.ToString();
                            var dataFundacaoStr = usuario.GetType().GetProperty("DataFundacao")?.GetValue(usuario)?.ToString();
                            var descricao = usuario.GetType().GetProperty("Descricao")?.GetValue(usuario)?.ToString();

                            if (!string.IsNullOrEmpty(cnpj))
                                ongAtual.Cnpj = cnpj;

                            if (!string.IsNullOrEmpty(razaoSocial))
                                ongAtual.RazaoSocial = razaoSocial;

                            if (!string.IsNullOrEmpty(nomeFantasia))
                                ongAtual.NomeFantasia = nomeFantasia;

                            if (!string.IsNullOrEmpty(dataFundacaoStr) && DateTime.TryParse(dataFundacaoStr, out DateTime dataFundacao))
                                ongAtual.DataFundacao = dataFundacao;

                            if (!string.IsNullOrEmpty(descricao))
                                ongAtual.Descricao = descricao;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar campos específicos: {ex.Message}");
            }

            await _context.SaveChangesAsync();

            return await ObterUsuarioPorId(usuarioAtual.UsuarioId);
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
            var usuario = await _context.Usuarios.FindAsync(id);
            
            if (usuario == null)
            {
                return null;
            }
            
            if (usuario.Tipo == TipoUsuario.Doador)
            {
                return await _context.Doadores.FindAsync(id);
            }
            else if (usuario.Tipo == TipoUsuario.Voluntario)
            {
                return await _context.Voluntarios.FindAsync(id);
            }
            else if (usuario.Tipo == TipoUsuario.Organizacao)
            {
                return await _context.Ongs.FindAsync(id);
            }
            
            return usuario;
        }

        public async Task<List<Usuario>> ObterUsuariosPorTipo(TipoUsuario tipo)
        {
            return await _context.Usuarios
                .Where(u => u.Tipo == tipo)
                .ToListAsync();
        }

        public async Task<List<Usuario>> ObterONGsProximas(double latitude, double longitude, double raioKm)
        {
            var ongs = await _context.Usuarios
                .Where(u => u.Tipo == TipoUsuario.Organizacao && u.Latitude.HasValue && u.Longitude.HasValue)
                .ToListAsync();

            return ongs.Where(o => CalcularDistancia(latitude, longitude, o.Latitude.Value, o.Longitude.Value) <= raioKm).ToList();
        }

        private double CalcularDistancia(double lat1, double lon1, double lat2, double lon2)
        {
            //Formula de Haversine para calcular a distância entre dois pontos na superfície da Terra
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

        public async Task<bool> VerificarSenha(int usuarioId, string senha)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            
            if (usuario == null)
            {
                return false;
            }
            
            return PasswordHelper.VerifyPassword(senha, usuario.Senha);
        }
    }
}