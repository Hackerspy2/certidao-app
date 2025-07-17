#region

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#endregion

namespace Domain;

[Table("Configuracao")]
public class Configuracao : INotifyPropertyChanged
{
    public Configuracao()
    {
        
    }
    [Key] public int Id { get; set; }

    public string? Gateway { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;
}