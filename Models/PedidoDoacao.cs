namespace Ong.Models;

public class PedidoDoacao
{
    public int PedidoDoacaoId { get; set; }

    public int OngId { get; set; }
    public Ong Ong { get; set; }

    public string Status { get; set; }
    public string Categoria { get; set; }
    public string Descricao { get; set; }
}
