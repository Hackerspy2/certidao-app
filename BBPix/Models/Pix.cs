using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BBPix.Models
{
    public class Pix
    {
        public string? EndToEndId { get; set; }
        public string Valor { get; set; }
        public string Horario { get; set; }
        public Pagador Pagador { get; set; }
        public string InfoPagador { get; set; }
        public string? Txid { get; set; }

        public Pix(string? endToEndId, string valor, string horario, Pagador pagador, string infoPagador, string? txid)
        {
            EndToEndId = endToEndId;
            Valor = valor;
            Horario = horario;
            Pagador = pagador;
            InfoPagador = infoPagador;
            Txid = txid;
        }

        public Dictionary<string, dynamic> ToDictionary()
        {
            return new Dictionary<string, dynamic>
        {
            { "endToEndId", EndToEndId },
            { "valor", Valor },
            { "horario", Horario },
            { "pagador", Pagador.ToJson() },
            { "infoPagador", InfoPagador },
            { "txid", Txid }
        };
        }

        public static Pix FromDictionary(Dictionary<string, dynamic> map)
        {
            return new Pix(
                map.ContainsKey("endToEndId") ? map["endToEndId"] as string : null,
                map["valor"] as string,
                map["horario"] as string,
               map["pagador"],
                map["infoPagador"] as string,
                map.ContainsKey("txid") ? map["txid"] as string : null
            );
        }

        public string ToJson()
        {
            return JsonSerializer.Serialize(ToDictionary());
        }

        public static Pix FromJson(string source)
        {
            return FromDictionary(JsonSerializer.Deserialize<Dictionary<string, dynamic>>(source));
        }
    }

    //public class Pagador
    //{
    //    // properties of Pagador class
    //    // ...

    //    public Dictionary<string, dynamic> ToDictionary()
    //    {
    //        return new Dictionary<string, dynamic>
    //        {
    //            // properties of Pagador class
    //            // ...
    //        };
    //    }

    //    public static Pagador FromDictionary(Dictionary<string, dynamic> map)
    //    {
    //        return new Pagador
    //        {
    //            // properties of Pagador class
    //            // ...
    //        };
    //    }
    //}
}
