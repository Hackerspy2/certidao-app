#region

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#endregion

namespace Domain;

[Table("Ticket")]
public class Ticket : INotifyPropertyChanged
{
    public Ticket()
    {
        Status = "Em aberto";
        StatusPagamento = "Aguardando";
        DataCadastro = DateTimeOffset.Now;
    }

    [Key] public int Id { get; set; }
    public int? IdPagador { get; set; }
    public int? IdPedido { get; set; }
    public string? Dominio { get; set; }

    public DateTimeOffset DataCadastro { get; set; }
    public DateTimeOffset? DataAtribuicao { get; set; }
    public DateTimeOffset? DataFinalizado { get; set; }
    public DateTimeOffset DataAlteracao { get; set; }
    public string? UsuarioAlteracao { get; set; }
    public string? UsuarioCadastro { get; set; }
    public int? IdPessoa { get; set; }
    public string NomePessoa => Equals(Pessoa, null) ? "" : Pessoa.Nome;
    [ForeignKey("IdPessoa")] public Pessoa? Pessoa { get; set; }
    [StringLength(100)] public string? NomePagador { get; set; }
    [StringLength(100)] public string? Nome { get; set; }
    [StringLength(100)] public string? RazaoSocial { get; set; }
    [StringLength(100)] public string? NomePai { get; set; }
    [StringLength(100)] public string? NomeMae { get; set; }
    [StringLength(100)] public string? Nacionalidade { get; set; }
    [StringLength(200)]  public string? Pais { get; set; }
    [StringLength(200)]  public string? Cpf { get; set; }
    [NotMapped]  public string? cpfCnpj { get; set; }
    [NotMapped]  public string? codigo { get; set; }
    [StringLength(200)]  public string? Cnpj { get; set; }
    [StringLength(30)] public string? Rg { get; set; }
    [StringLength(150)] public string? RgData { get; set; }
    public string? Finalidade { get; set; }
    public string? FinalidadeComplemento { get; set; }
    public string? Profissao { get; set; }
    [StringLength(150)] public string? Nirf { get; set; }
    [StringLength(150)] public string? Emissor { get; set; }
    [StringLength(150)] public string? EmissorOutro { get; set; }
    [StringLength(150)] public string? EmissorEstado { get; set; }
    [StringLength(150)] public string? Passaporte { get; set; }
    [StringLength(150)] public string? PassaporteSerie { get; set; }
    [StringLength(150)] public string? DataNascimento { get; set; }
    [StringLength(200)]  public string? Cep { get; set; }
    [StringLength(200)] public string? Logradouro { get; set; }
    [StringLength(200)]  public string? Numero { get; set; }
    [StringLength(200)]  public string? Complemento { get; set; }
    [StringLength(100)] public string? Bairro { get; set; }
    [StringLength(200)] public string? Cidade { get; set; }
    [StringLength(200)] public string? CidadeNasceu { get; set; }
    [StringLength(200)]  public string? EstadoCivil { get; set; }
    [StringLength(200)]  public string? Instancia { get; set; }
    [StringLength(400)] public string? FiltroAcao { get; set; }
    [StringLength(200)] public string? TipoCnd { get; set; }
    [StringLength(200)]  public string? Estado { get; set; }
    [StringLength(200)]  public string? EstadoNasceu { get; set; }
    [StringLength(200)]  public string? Telefone { get; set; }
    [StringLength(200)]  public string? Celular { get; set; }
    [StringLength(200)] public string? Email { get; set; }
    [StringLength(50)] public string? Status { get; set; }
    [StringLength(50)] public string? StatusPagamento { get; set; }
    [StringLength(50)] public string? StatusPagamentoOperador { get; set; }
    [StringLength(50)] public string? Sexo { get; set; }
    [StringLength(50)] public string? EstadoEmissao { get; set; }
    [StringLength(50)] public string? CidadeEmissao { get; set; }
    public string? Observacao { get; set; }
    public string? Valor { get; set; }
    public string? StatusTransacao { get; set; }
    public string? TidTransacao { get; set; }
    public string? PixCopiaCola { get; set; }
    public string? PixQrCode { get; set; }
    public ICollection<Interacao> Interacoes { get; set; } = new List<Interacao>();
    [StringLength(200)]public string Tipo { get; set; }
    [StringLength(200)]public string? SubTipo { get; set; }
    [StringLength(20)]public string? Enviado { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;
}