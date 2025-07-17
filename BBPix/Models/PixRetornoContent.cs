using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBPix.Models
{
    internal class PixRetornoContent
    {
        [JsonProperty("pix")]
        public PixRetornoContent[] Pix { get; set; }
    }

    public partial class PixRetorno
    {
        [JsonProperty("chave")]
        public Guid Chave { get; set; }

        [JsonProperty("solicitacaoPagador")]
        public string SolicitacaoPagador { get; set; }
       

        [JsonProperty("txid")]
        public string Txid { get; set; }

        [JsonProperty("revisao")]
        public long Revisao { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("pixCopiaECola")]
        public string PixCopiaECola { get; set; }
    }
}
