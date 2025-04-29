namespace Ong.Models;

public class VoluntarioEvento
{
    public int VoluntarioId { get; set; }
    public Voluntario Voluntario { get; set; }

    public int EventoId { get; set; }
    public Evento Evento { get; set; }

    public string Status { get; set; }
    public DateTime DataInscricao { get; set; }
}