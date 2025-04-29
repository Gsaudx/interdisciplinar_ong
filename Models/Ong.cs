namespace Ong.Models;

public class Ong : Usuario
{
    public string Cnpj { get; set; }
    public string RazaoSocial { get; set; }
    public string NomeFantasia { get; set; }
    public DateTime DataFundacao { get; set; }
    public string Descricao { get; set; }

    public ICollection<Evento> Eventos { get; set; } = new List<Evento>();
    public ICollection<PedidoDoacao> PedidoDoacoes { get; set; } = new List<PedidoDoacao>();
}