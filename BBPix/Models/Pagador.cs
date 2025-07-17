using System.Text.Json;

namespace BBPix.Models;

public class Pagador
{
    /// <summary>
    ///     Constructor for Pagador class.
    /// </summary>
    public Pagador(string nome, string cpf, string cnpj)
    {
        Nome = nome;
        Cpf = cpf;
        Cnpj = cnpj;
    }

    /// <summary>
    ///     The name of the payer.
    /// </summary>
    public string Nome { get; set; }

    /// <summary>
    ///     The CPF of the payer. Can be null if identified by CNPJ.
    /// </summary>
    public string Cpf { get; set; }

    /// <summary>
    ///     The CNPJ of the payer. Can be null if identified by CPF.
    /// </summary>
    public string Cnpj { get; set; }

    /// <summary>
    ///     Serializes the Pagador object to a JSON string.
    /// </summary>
    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }

    /// <summary>
    ///     A factory method for creating a Pagador object from a JSON string.
    /// </summary>
    public static Pagador FromJson(string json)
    {
        return JsonSerializer.Deserialize<Pagador>(json);
    }
}