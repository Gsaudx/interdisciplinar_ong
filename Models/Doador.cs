namespace Ong.Models;

public class Doador : Usuario
{
    public string Cpf { get; set; }
    public DateTime? DataNascimento { get; set; }

    public ICollection<Doacao> Doacoes { get; set; } = new List<Doacao>();
}