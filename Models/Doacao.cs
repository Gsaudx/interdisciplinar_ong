namespace Ong.Models;

public class Doacao
{
    public int DoacaoId { get; set; }

    public int DoadorId { get; set; }
    public required Doador Doador { get; set; }

    public int OngId { get; set; }
    public required Ong Ong { get; set; }

    public int? PedidoDoacaoId { get; set; }
    public PedidoDoacao? PedidoDoacao { get; set; }

    public required string Categoria { get; set; }
    public required string Descricao { get; set; }
    public DateTime DataDoacao { get; set; }
    public StatusDoacao Status { get; set; } = StatusDoacao.Criada;
}
