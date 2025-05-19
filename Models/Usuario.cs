namespace Ong.Models;

public class Usuario
{
    public int UsuarioId { get; set; }
    public string Nome { get; set; }
    public string EmailPrincipal { get; set; }
    public string Senha { get; set; }
    public string Telefone { get; set; }

    public string Logradouro { get; set; }
    public string Numero { get; set; }
    public string Complemento { get; set; }
    public string Bairro { get; set; }
    public string Cidade { get; set; }
    public string Estado { get; set; }
    public string Cep { get; set; }
    public TipoUsuario Tipo { get; set; }
    public DateTime DataCadastro { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    public ICollection<Contato>? Contatos { get; set; }
}