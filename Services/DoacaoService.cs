using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ong.Data;
using Ong.Models;

namespace Ong.Services
{
    public class DoacaoService
    {
        private readonly DbOng _context;

        public DoacaoService(DbOng context)
        {
            _context = context;
        }

        public async Task<Doacao> RealizarDoacao(int doadorId, int ongId, int? pedidoDoacaoId, string categoria, string descricao)
        {
            var doador = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.UsuarioId == doadorId && u.Tipo == TipoUsuario.Doador);
            
            if (doador == null)
            {
                throw new Exception("Doador não encontrado ou usuário não é um doador.");
            }

            var ong = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.UsuarioId == ongId && u.Tipo == TipoUsuario.Organizacao);
            
            if (ong == null)
            {
                throw new Exception("ONG não encontrada ou usuário não é uma organização.");
            }

            if (pedidoDoacaoId.HasValue)
            {
                var pedidoDoacao = await _context.PedidoDoacoes
                    .FirstOrDefaultAsync(p => p.PedidoDoacaoId == pedidoDoacaoId && p.OngId == ongId);
                
                if (pedidoDoacao == null)
                {
                    throw new Exception("Pedido de doação não encontrado ou não pertence à ONG especificada.");
                }
            }

            var doacao = new Doacao
            {
                DoadorId = doadorId,
                OngId = ongId,
                PedidoDoacaoId = pedidoDoacaoId,
                Categoria = categoria,
                Descricao = descricao,
                DataDoacao = DateTime.Now
            };

            await _context.Doacoes.AddAsync(doacao);
            await _context.SaveChangesAsync();

            return doacao;
        }

        public async Task<List<Doacao>> ObterDoacoesPorDoador(int doadorId)
        {
            return await _context.Doacoes
                .Where(d => d.DoadorId == doadorId)
                .Include(d => d.Ong)
                .Include(d => d.PedidoDoacao)
                .ToListAsync();
        }

        public async Task<List<Doacao>> ObterDoacoesPorOng(int ongId)
        {
            return await _context.Doacoes
                .Where(d => d.OngId == ongId)
                .Include(d => d.Doador)
                .Include(d => d.PedidoDoacao)
                .ToListAsync();
        }

        public async Task<List<Doacao>> ObterDoacoesPorPedido(int pedidoDoacaoId)
        {
            return await _context.Doacoes
                .Where(d => d.PedidoDoacaoId == pedidoDoacaoId)
                .Include(d => d.Doador)
                .Include(d => d.Ong)
                .ToListAsync();
        }
    }
}