namespace Web.Models
{
    public class Parcela
    {
        public Parcela()
        {
            Parcelas = 1;
        }
        public string FormaPagamento { get; set; }
        public int? Parcelas { get; set; }
        public decimal? Valor { get; set; }
        public string? Observacoes { get; set; }
    }
}
