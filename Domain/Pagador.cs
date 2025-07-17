using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Domain
{
    [Table("Pagador")]
    public class Pagador : INotifyPropertyChanged
    {
        [Key] public int Id { get; set; }

        public DateTimeOffset Data { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string Cpf { get; set; }
        public string Senha { get; set; }
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
