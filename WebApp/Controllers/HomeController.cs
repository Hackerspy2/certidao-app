using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http.Headers;
using Domain;
using Newtonsoft.Json;
using Repository;
using WebApp.Filter;
using WebApp.Models;
using iDevCL;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace WebApp.Controllers
{
    public class HomeController :  ApplicationController<Ticket>
    {
        protected static IConfiguration? _configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private IGenericDataRepository<Ticket> _dataRepository;
        private readonly IGenericDataRepository<Pessoa> _dataRepositoryPessoa;
        public HomeController(IHttpContextAccessor httpContextAccessor, IConfiguration? configuration, IWebHostEnvironment environment, IGenericDataRepository<Ticket> dataRepository, IGenericDataRepository<Pessoa> dataRepositoryPessoa) : base(httpContextAccessor, configuration)
        {
            _configuration = configuration;
            _hostingEnvironment = environment;
            _dataRepository = dataRepository;
            _dataRepositoryPessoa = dataRepositoryPessoa;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_dataRepository != null)
                {
                    _dataRepository.Dispose();
                    _dataRepository = null;
                }
            }

            base.Dispose(disposing);
        }
        [HttpGet("/nao-autorizado")]
        public IActionResult Nao()
        {
            return View();
        }
        [HttpGet("/fale-conosco")]
        public IActionResult FaleConosco()
        {
            return View();
        }

        //me dê um código que calacula a diferença entre duas datas

        public bool VerificaCaptcha(string captchaResponse)
        {
            var result = false;
            var secretKey = "6LeEzn4oAAAAAPUn2iFC-7Ho0aOSCyOoTGLjyEnM";
            var apiUrl = "https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}";
            var requestUri = string.Format(apiUrl, secretKey, captchaResponse);
            var request = (HttpWebRequest)WebRequest.Create(requestUri);

            using var response = request.GetResponse();
            using StreamReader stream = new StreamReader(response.GetResponseStream());
            JObject jResponse = JObject.Parse(stream.ReadToEnd());
            var isSuccess = jResponse.Value<bool>("success");
            result = (isSuccess);
            return result;
        }

        [HttpPost]
        public async Task<JsonResult> ContatoEnviar(string nome, string celular, string email, string mensagem, string recaptcha)
        {
            try
            {
                var assunto = "Contato via Site";
                if (string.IsNullOrWhiteSpace(recaptcha)) return Json( new {cssClass = "error", mensagem = "Resolva o captcha!" });
                if (!VerificaCaptcha(recaptcha)) return Json(new { cssClass = "error", mensagem = "Falha na verificação captcha!" });

                if (!Email.ValidaEmail(email))
                    return Json(new { cssClass = "error", mensagem = @"E-mail inválido" });

                var sTemplate = await System.IO.File.ReadAllTextAsync(Path.Combine(_hostingEnvironment.WebRootPath,
                    "templates\\template.html"));

                var sb = new StringBuilder();
                sb.AppendFormat("<p> Contato via site:<br><br>");
                sb.AppendFormat("<strong>Nome:</strong> {0}<br>", nome);
                sb.AppendFormat("<strong>Celular:</strong> {0}<br>", celular);
                sb.AppendFormat("<strong>E-mail:</strong> {0}<br>", email);
                sb.AppendFormat("<strong>Mensagem:</strong> {0}", mensagem);
                sb.AppendFormat("</p>");

                sTemplate = sTemplate.Replace("{Conteudo}", sb.ToString());
                sTemplate = sTemplate.Replace("{Titulo}", assunto);
                sTemplate = sTemplate.Replace("{Url}", _configuration.GetSection("Url").Value);
                sTemplate = sTemplate.Replace("{UrlLimpo}",
                    _configuration.GetSection("Url").Value.RemoveCaracteres(new[] { "http://", "https://" }));
                sTemplate = sTemplate.Replace("{Destinatario}", _configuration.GetSection("Destinatario").Value);
                sTemplate = sTemplate.Replace("{UrlInstagram}", _configuration.GetSection("UrlInstagram").Value);
                sTemplate = sTemplate.Replace("{UrlFacebook}", _configuration.GetSection("UrlFacebook").Value);
                sTemplate = sTemplate.Replace("{Whats}", _configuration.GetSection("Whats").Value);
                sTemplate = sTemplate.Replace("{WhatsLimpo}",
                    _configuration.GetSection("Whats").Value.RemoveCaracteres(new[] { "(", ")", "-", " " })
                        .Trim());
                string[] retEnvio;
                ServicePointManager.SecurityProtocol =
                    SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                Email.DisparaEmail(_configuration.GetSection("HostSmtp").Value,
                _configuration.GetSection("Remetente").Value,
                _configuration.GetSection("Destinatario").Value, assunto, sTemplate, true, "", out retEnvio);


                return Json(new { cssClass = retEnvio[0] == "erro" ? "error" : "success", mensagem = retEnvio[1] });
            }
            catch (Exception e)
            {
                return Json(new { cssClass = "erro", mensagem = "Ocorreu um erro, tente novamente em poucos minutos!" });
            }
        }


        [AllPermission]
        public ActionResult DashBoard()
        {
            var admin = GetDadosUsuario<bool>("IsSuporte");
            var idPessoa = GetDadosUsuario<int>("IdUsuario");
            var filtro = new FilterExpression<Ticket>().Start(a => a.StatusPagamento == "Pago");
            filtro = filtro.And(a => a.IdPessoa == idPessoa, !admin);
            var dados = _dataRepository.GetAllObject(filtro.ResultExpression, a => new Ticket{ IdPessoa = a.IdPessoa, Status = a.Status}).Cast<Ticket>().ToList();
            ViewBag.taxa = admin ? 1.5m: (decimal)_dataRepositoryPessoa.FindObject(a => a.Id == idPessoa, a => a.Taxa);
            return View(dados);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public string GetEndereco(string cep)
        {
            if (cep != null)
                try
                {
                    var Http = new HttpClient();
                    var BaseUrl = $"https://viacep.com.br/ws/{cep.Replace("-", "")}/json/";
                    Http.DefaultRequestHeaders.Accept.Clear();
                    Http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var response = Http.GetAsync(BaseUrl).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var json = response.Content.ReadAsStringAsync().Result;
                        if (!string.IsNullOrEmpty(json))
                        {
                            var endereco = JsonConvert.DeserializeObject<Address>(json);
                            string[] res = { endereco.Logradouro, endereco.Bairro, endereco.Logradouro, endereco.Uf };
                            return JsonConvert.SerializeObject(res);
                        }

                        return "";
                    }

                    return "";
                }
                catch (Exception)
                {
                    return "";
                }

            return "";
        }
    }
}