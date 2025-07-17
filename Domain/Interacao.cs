#region

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#endregion

namespace Domain;

[Table("Interacao")]
public class Interacao : INotifyPropertyChanged
{
    public Interacao()
    {
        Enviar = "Não";
    }
    [Key] public int Id { get; set; }
    public DateTimeOffset  DataCadastro { get; set; }
    public int? IdTicket { get; set; }
    [ForeignKey("IdTicket")] public Ticket? Ticket { get; set; }
    [StringLength(1000)]public string? Mensagem { get; set; }
    [StringLength(50)]public string? Status { get; set; }
    [StringLength(50)]public string? Anexo { get; set; }
    [NotMapped]public string? Enviar { get; set; }
    
    public event PropertyChangedEventHandler? PropertyChanged;
}