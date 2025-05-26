using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ong.Data;
using Ong.Models;

namespace Ong.Services
{
    public class PedidoDoacaoService
    {
        private readonly DbOng _context;

        public PedidoDoacaoService(DbOng context)
        {
            _context = context;
        }

        public async Task<PedidoDoacao> CriarPedidoDoacao(int ongId, string categoria, string descricao, string status = "Aberto")
        {
            var ong = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.UsuarioId == ongId && u.Tipo == TipoUsuario.Organizacao);
            
            if (ong == null)
            {
                throw new Exception("ONG não encontrada ou usuário não é uma organização.");
            }

            var pedidoDoacao = new PedidoDoacao
            {
                OngId = ongId,
                Categoria = categoria,
                Descricao = descricao,
                Status = status
            };

            await _context.PedidoDoacoes.AddAsync(pedidoDoacao);
            await _context.SaveChangesAsync();

            return pedidoDoacao;
        }

        public async Task<PedidoDoacao> AtualizarStatusPedidoDoacao(int pedidoDoacaoId, string novoStatus)
        {
            var pedidoDoacao = await _context.PedidoDoacoes.FindAsync(pedidoDoacaoId);
            
            if (pedidoDoacao == null)
            {
                throw new Exception("Pedido de doação não encontrado.");
            }

            pedidoDoacao.Status = novoStatus;
            await _context.SaveChangesAsync();

            return pedidoDoacao;
        }

        public async Task<PedidoDoacao?> ObterPedidoDoacaoPorId(int pedidoDoacaoId)
        {
            return await _context.PedidoDoacoes
                .Include(p => p.Ong)
                .FirstOrDefaultAsync(p => p.PedidoDoacaoId == pedidoDoacaoId);
        }

        public async Task<List<PedidoDoacao>> ObterPedidosDoacaoPorOng(int ongId)
        {
            return await _context.PedidoDoacoes
                .Where(p => p.OngId == ongId)
                .Include(p => p.Ong)
                .ToListAsync();
        }

        public async Task<List<PedidoDoacao>> ObterPedidosDoacaoPorStatus(string status)
        {
            return await _context.PedidoDoacoes
                .Where(p => p.Status == status)
                .Include(p => p.Ong)
                .ToListAsync();
        }

        public async Task<List<PedidoDoacao>> ObterTodosPedidosDoacao()
        {
            return await _context.PedidoDoacoes
                .Include(p => p.Ong)
                .ToListAsync();
        }
    }
}