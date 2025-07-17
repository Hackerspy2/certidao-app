#region

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#endregion

namespace Domain;

[Table("Visita")]
public class Visita : INotifyPropertyChanged
{
    [Key] public int Id { get; set; }

    public DateTimeOffset Data { get; set; }
    public string Ip { get; set; }
    public string Dominio { get; set; }
    public event PropertyChangedEventHandler? PropertyChanged;
}