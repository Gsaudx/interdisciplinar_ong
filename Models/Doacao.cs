namespace Ong.Models;

public class Doacao
{
    public int DoacaoId { get; set; }

    public int DoadorId { get; set; }
    public Doador Doador { get; set; }

    public int OngId { get; set; }
    public Ong Ong { get; set; }

    public int? PedidoDoacaoId { get; set; }
    public PedidoDoacao? PedidoDoacao { get; set; }

    public string Categoria { get; set; }
    public string Descricao { get; set; }
    public DateTime DataDoacao { get; set; }
}
