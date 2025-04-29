using Ong.Models;
using Microsoft.EntityFrameworkCore;

namespace Ong.Data;

public class DbOng : DbContext
{
    public DbOng(DbContextOptions options) : base(options) {}     

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>().ToTable("Usuario");
        modelBuilder.Entity<Voluntario>().ToTable("Voluntario");
        modelBuilder.Entity<Doador>().ToTable("Doador");
        modelBuilder.Entity<Ong.Models.Ong>().ToTable("Ong");

        modelBuilder.Entity<VoluntarioEvento>()
            .HasKey(ve => new { ve.VoluntarioId, ve.EventoId }); // <- chave composta
            
        modelBuilder.Entity<Doacao>()
            .HasOne(d => d.Ong)
            .WithMany()
            .HasForeignKey(d => d.OngId)
            .OnDelete(DeleteBehavior.NoAction); 

        modelBuilder.Entity<Doacao>()
            .HasOne(d => d.Doador)
            .WithMany(d => d.Doacoes)
            .HasForeignKey(d => d.DoadorId)
            .OnDelete(DeleteBehavior.NoAction);
            
        modelBuilder.Entity<Evento>()
            .HasOne(e => e.Ong)
            .WithMany(o => o.Eventos)
            .HasForeignKey(e => e.OngId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<VoluntarioEvento>()
            .HasOne(ve => ve.Voluntario)
            .WithMany(v => v.VoluntarioEventos)
            .HasForeignKey(ve => ve.VoluntarioId)
            .OnDelete(DeleteBehavior.NoAction);
            
        modelBuilder.Entity<VoluntarioEvento>()
            .HasOne(ve => ve.Evento)
            .WithMany(e => e.VoluntarioEventos)
            .HasForeignKey(ve => ve.EventoId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
