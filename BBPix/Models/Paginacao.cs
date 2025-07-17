using System.Text.Json;

namespace BBPix.Models;

public class Paginacao
{
    /// <summary>
    ///     Constructor for Paginacao.
    /// </summary>
    public Paginacao(int paginaAtual, int itensPorPagina, int quantidadeDePaginas, int quantidadeTotalDeItens)
    {
        PaginaAtual = paginaAtual;
        ItensPorPagina = itensPorPagina;
        QuantidadeDePaginas = quantidadeDePaginas;
        QuantidadeTotalDeItens = quantidadeTotalDeItens;
    }

    /// <summary>
    ///     The current page number.
    /// </summary>
    public int PaginaAtual { get; set; }

    /// <summary>
    ///     The number of items per page.
    /// </summary>
    public int ItensPorPagina { get; set; }

    /// <summary>
    ///     The total number of pages.
    /// </summary>
    public int QuantidadeDePaginas { get; set; }

    /// <summary>
    ///     The total number of items.
    /// </summary>
    public int QuantidadeTotalDeItens { get; set; }

    /// <summary>
    ///     Serializes the Paginacao object to a JSON string.
    /// </summary>
    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }

    /// <summary>
    ///     A factory method for creating a Paginacao object from a JSON string.
    /// </summary>
    public static Paginacao FromJson(string json)
    {
        return JsonSerializer.Deserialize<Paginacao>(json);
    }
}