#region

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#endregion

namespace Domain;

[Table("Pessoa")]
public class Pessoa : INotifyPropertyChanged
{
    public Pessoa()
    {
        DataCadastro = DateTime.Now;
        DataAlteracao = DateTime.Now;
    }

    [Key] public int Id { get; set; }
    public DateTimeOffset DataCadastro { get; set; }
    public DateTimeOffset DataAlteracao { get; set; }
    public string? UsuarioAlteracao { get; set; }
    public string? UsuarioCadastro { get; set; }
    [StringLength(200)] public string? Nome { get; set; }
    [StringLength(200)] public string? Email { get; set; }
    [StringLength(200)] public string? Senha { get; set; }
    [StringLength(20)] public string? Status { get; set; }
    [StringLength(200)] public string? Foto { get; set; }
    [StringLength(20)] public string? Cep { get; set; }
    [StringLength(200)] public string? Logradouro { get; set; }
    [StringLength(20)] public string? Numero { get; set; }
    [StringLength(20)] public string? Complemento { get; set; }
    [StringLength(100)] public string? Bairro { get; set; }
    [StringLength(200)] public string? Cidade { get; set; }
    [StringLength(20)] public string? Estado { get; set; }
    [StringLength(20)] public string? Telefone { get; set; }
    [StringLength(20)] public string? Celular { get; set; }
    [StringLength(200)] public string? Token { get; set; }
    public bool IsUsuario { get; set; }
    public bool IsSuporte { get; set; }
    public string? Perfil { get; set; }
    public string? Sexo { get; set; }
    public DateTime? DataNascimento { get; set; }
    [StringLength(200)] public string? Cpf { get; set; }
    [StringLength(200)] public string? Rg { get; set; }
    [StringLength(200)] public string? RgEmissor { get; set; }
    [StringLength(100)] public string? DeviceName { get; set; }
    [StringLength(100)] public string? DeviceVersion { get; set; }
    [StringLength(150)] public string? PushToken { get; set; }
    [StringLength(150)] public string? CodigoIndicacao { get; set; }
    [NotMapped] public DateTime? ExpiresToken { get; set; }

    public decimal? Taxa { get; set; }
    public string? Observacao { get; set; }
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    public event PropertyChangedEventHandler? PropertyChanged;
}