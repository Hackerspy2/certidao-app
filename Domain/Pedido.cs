using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Domain
{
    [Table("Pedido")]
    public class Pedido : INotifyPropertyChanged
    {
        public Pedido()
        {
            StatusPagamento = "Em aberto";
            Data = DateTime.Now;
        }
        [Key] public int Id { get; set; }

        public DateTimeOffset Data { get; set; }
        public int? IdPagador { get; set; }
        [StringLength(50)] public string? StatusPagamento { get; set; }
        public string? TidTransacao { get; set; }
        public string? Valor { get; set; }
        public string? PixCopiaCola { get; set; }
        public string? PixQrCode { get; set; }
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
