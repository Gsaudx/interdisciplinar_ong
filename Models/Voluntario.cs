namespace Ong.Models;

public class Voluntario : Usuario
{
    public string Cpf { get; set; }
    public DateTime? DataNascimento { get; set; }
    public string Profissao { get; set; }
    public string Disponibilidade { get; set; }

    public ICollection<VoluntarioEvento> VoluntarioEventos { get; set; } = new List<VoluntarioEvento>();
}