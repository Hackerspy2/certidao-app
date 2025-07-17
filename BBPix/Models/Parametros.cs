using System;
using System.Collections.Generic;
using System.Text.Json;

namespace BBPix.Models
{
    public class Parametros
    {
        public string inicio { get; set; }
        public string fim { get; set; }
        public Paginacao paginacao { get; set; }

        public Parametros(string inicio, string fim, Paginacao paginacao)
        {
            this.inicio = inicio;
            this.fim = fim;
            this.paginacao = paginacao;
        }

        public Dictionary<string, dynamic> ToDictionary()
        {
            return new Dictionary<string, dynamic>
            {
                { "inicio", inicio },
                { "fim", fim },
                { "paginacao", paginacao.ToJson() }
            };
        }

        public static Parametros FromDictionary(Dictionary<string, dynamic> map)
        {
            return new Parametros(
                map["inicio"],
                map["fim"],
                Paginacao.FromJson(map["paginacao"])
            );
        }

        public string ToJson()
        {
            return JsonSerializer.Serialize(ToDictionary());
        }

        public static Parametros FromJson(string source)
        {
            return FromDictionary(JsonSerializer.Deserialize<Dictionary<string, dynamic>>(source));
        }
    }
}
