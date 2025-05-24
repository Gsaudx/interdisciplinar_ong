// filepath: c:\Programming\interdisciplinar_ong\Services\EventoService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ong.Data;
using Ong.Models;

namespace Ong.Services
{
    public class EventoService
    {
        private readonly DbOng _context;

        public EventoService(DbOng context)
        {
            _context = context;
        }

        public async Task<Evento> CriarEvento(int ongId, string nome, string descricao, DateTime data, string localizacao)
        {
            // Verificar se a ONG existe
            var ong = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.UsuarioId == ongId && u.Tipo == TipoUsuario.Organizacao);
            
            if (ong == null)
            {
                throw new Exception("ONG não encontrada ou usuário não é uma organização.");
            }

            // Criar o evento
            var evento = new Evento
            {
                OngId = ongId,
                Nome = nome,
                Descricao = descricao,
                Data = data,
                Localizacao = localizacao
            };

            await _context.Eventos.AddAsync(evento);
            await _context.SaveChangesAsync();

            return evento;
        }

        public async Task<Evento> AtualizarEvento(int eventoId, string nome, string descricao, DateTime data, string localizacao)
        {
            var evento = await _context.Eventos.FindAsync(eventoId);
            
            if (evento == null)
            {
                throw new Exception("Evento não encontrado.");
            }

            evento.Nome = nome;
            evento.Descricao = descricao;
            evento.Data = data;
            evento.Localizacao = localizacao;

            await _context.SaveChangesAsync();

            return evento;
        }

        public async Task<Evento?> ObterEventoPorId(int eventoId)
        {
            return await _context.Eventos
                .Include(e => e.Ong)
                .FirstOrDefaultAsync(e => e.EventoId == eventoId);
        }

        public async Task<List<Evento>> ObterEventosPorOng(int ongId)
        {
            return await _context.Eventos
                .Where(e => e.OngId == ongId)
                .Include(e => e.Ong)
                .ToListAsync();
        }

        public async Task<List<Evento>> ObterEventosFuturos()
        {
            return await _context.Eventos
                .Where(e => e.Data > DateTime.Now)
                .Include(e => e.Ong)
                .OrderBy(e => e.Data)
                .ToListAsync();
        }

        public async Task<VoluntarioEvento> InscreverVoluntarioEmEvento(int voluntarioId, int eventoId)
        {
            // Verificar se o voluntário existe
            var voluntario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.UsuarioId == voluntarioId && u.Tipo == TipoUsuario.Voluntario);
            
            if (voluntario == null)
            {
                throw new Exception("Voluntário não encontrado ou usuário não é um voluntário.");
            }

            // Verificar se o evento existe
            var evento = await _context.Eventos.FindAsync(eventoId);
            
            if (evento == null)
            {
                throw new Exception("Evento não encontrado.");
            }

            // Verificar se o voluntário já está inscrito no evento
            var inscricaoExistente = await _context.VoluntarioEventos
                .FirstOrDefaultAsync(ve => ve.VoluntarioId == voluntarioId && ve.EventoId == eventoId);
            
            if (inscricaoExistente != null)
            {
                throw new Exception("Voluntário já está inscrito neste evento.");
            }

            // Criar a inscrição
            var voluntarioEvento = new VoluntarioEvento
            {
                VoluntarioId = voluntarioId,
                EventoId = eventoId,
                Status = "Inscrito",
                DataInscricao = DateTime.Now
            };

            await _context.VoluntarioEventos.AddAsync(voluntarioEvento);
            await _context.SaveChangesAsync();

            return voluntarioEvento;
        }

        public async Task<VoluntarioEvento> AtualizarStatusInscricao(int voluntarioId, int eventoId, string novoStatus)
        {
            var voluntarioEvento = await _context.VoluntarioEventos
                .FirstOrDefaultAsync(ve => ve.VoluntarioId == voluntarioId && ve.EventoId == eventoId);
            
            if (voluntarioEvento == null)
            {
                throw new Exception("Inscrição não encontrada.");
            }

            voluntarioEvento.Status = novoStatus;
            await _context.SaveChangesAsync();

            return voluntarioEvento;
        }

        public async Task<List<Evento>> ObterEventosPorVoluntario(int voluntarioId)
        {
            return await _context.VoluntarioEventos
                .Where(ve => ve.VoluntarioId == voluntarioId)
                .Include(ve => ve.Evento)
                .ThenInclude(e => e.Ong)
                .Select(ve => ve.Evento)
                .ToListAsync();
        }

        public async Task<List<Voluntario>> ObterVoluntariosPorEvento(int eventoId)
        {
            return await _context.VoluntarioEventos
                .Where(ve => ve.EventoId == eventoId)
                .Include(ve => ve.Voluntario)
                .Select(ve => ve.Voluntario)
                .ToListAsync();
        }
    }
}