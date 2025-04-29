namespace Ong.Models;

public class Evento
{
    public int EventoId { get; set; }
    public string Nome { get; set; }
    public string Descricao { get; set; }
    public DateTime Data { get; set; }
    public string Localizacao { get; set; }

    public int OngId { get; set; }
    public Ong Ong { get; set; }

    public ICollection<VoluntarioEvento> VoluntarioEventos { get; set; }
}
