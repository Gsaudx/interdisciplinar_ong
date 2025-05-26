namespace Ong.Models;

public class Evento
{
    public int EventoId { get; set; }
    public required string Nome { get; set; }
    public required string Descricao { get; set; }
    public DateTime Data { get; set; }
    public int DuracaoMinutos { get; set; } = 120;
    public required string Localizacao { get; set; }

    public int OngId { get; set; }
    public Ong? Ong { get; set; }

    public ICollection<VoluntarioEvento>? VoluntarioEventos { get; set; }
}
