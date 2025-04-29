namespace Ong.Models;

public class Contato
{
    public int ContatoId { get; set; }
    public string Tipo { get; set; }
    public string Descricao { get; set; }
    public string Valor { get; set; }

    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; }
}