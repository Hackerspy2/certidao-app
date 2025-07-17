#region

using System.Collections;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Domain;
using iDevCL;
using LyTex;
using LyTex.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PagarmeApiSDK.Standard;
using PagarmeApiSDK.Standard.Exceptions;
using PagarmeApiSDK.Standard.Models;
using QRCoder;
using Repository;
using SendPulse;
using Transfera;
using Web.Models;
using static Dapper.SqlMapper;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

#endregion

namespace Web.Controllers;

public class TicketController : ApplicationController<Ticket>
{
    private static string userId;
    private static string secret;
    private static Service _services;
    private readonly IMemoryCache _cache;
    private readonly string _certificado;
    private readonly HttpClient _client;
    private readonly string _clienteId;
    private readonly string _clienteSecret;
    private readonly IGenericDataRepository<Ticket> _dataRepository;
    private readonly IGenericDataRepository<Configuracao> _dataRepositoryConfiguracao;
    private readonly IGenericDataRepository<Visita> _dataRepositoryVisita;
    private readonly IWebHostEnvironment _hostingEnvironment;
    private readonly IOptions<Mensagens> _mensagens;
    private readonly bool _sandBox;
    private readonly string _url;

    public TicketController(IHttpContextAccessor httpContextAccessor, IConfiguration? configuration,
        IWebHostEnvironment environment, IMemoryCache cache,
        IGenericDataRepository<Ticket> dataRepository, IGenericDataRepository<Visita> dataRepositoryVisita,
        IGenericDataRepository<Configuracao> dataRepositoryConfiguracao,
        IOptions<Mensagens> mensagens) : base(httpContextAccessor, configuration)
    {
        if (httpContextAccessor.HttpContext != null)
            NomeController = httpContextAccessor.HttpContext.Request.RouteValues["controller"]?.ToString();
        _dataRepository = dataRepository;
        _cache = cache;
        _dataRepositoryVisita = dataRepositoryVisita;
        _dataRepositoryConfiguracao = dataRepositoryConfiguracao;
        _hostingEnvironment = environment;
        _mensagens = mensagens;
        _configuration = configuration;
        var sandBox = _configuration.GetSection("LyTex.Sandbox").Value;
        _services = new Service(bool.Parse(sandBox));
        //_sandBox = bool.Parse(_configuration.GetSection("Efi.Sandbox").Value);
        //_clienteId = _configuration.GetSection("Efi.ClientId").Value;
        //_clienteSecret = _configuration.GetSection("Efi.ClientSecret").Value;
        //_certificado = Path.GetFullPath("certificado/homologacao-132922-PixTeste.p12");
        //_url = _sandBox ? "https://sandbox.asaas.com/api" : "https://api.asaas.com";

        //_client = new HttpClient();
        //_client.DefaultRequestHeaders.Accept.Clear();
        //_client.DefaultRequestHeaders.Add("access_token", _configuration.GetSection("Asaas.ApiKey").Value);
        //_client.BaseAddress = new Uri(_url);

        userId = _configuration.GetSection("SendPulse.UserId").Value;
        secret = _configuration.GetSection("SendPulse.Secret").Value;


    }

    [HttpGet("/remarketing")]
    public async Task<IActionResult> Remarketing()
    {
        await using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn")))
        {
            var sp = new Sendpulse(userId, secret);
            var remetente = _configuration.GetSection("Remetente").Value;
            await connection.OpenAsync();
            await using (var command = new SqlCommand(
                             "SELECT Id, Nome, Tipo, PixCopiaCola, Celular, Email FROM Ticket WHERE StatusPagamento = 'Aguardando' AND Status = 'Em aberto' AND Enviado is null AND DataCadastro >= DATEADD(MINUTE, -2, GETDATE());",
                             connection))
            {
                await using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var celular = reader["Celular"].ToString();
                        var email = reader["Email"].ToString();
                        var nome = reader["Nome"].ToString();
                        var tipo = reader["Tipo"].ToString();
                        var pixQrCode = reader["PixCopiaCola"].ToString();
                        var id = reader["Id"].ToString();

                        var mensagemEmail =
                            $"<p>Olá, {nome} notamos que você preencheu um de nossos formulários, mas ainda não finalizou a solicitação.</p>" +
                            $"<p>O serviço escolhido foi: {tipo}</p>" +
                            $"<p>Para receber a certidão no seu e-mail, basta copiar todo o Código abaixo e efetuar o pagamento em seu banco de preferência utilizando a opção PIX Copia e Cola:\n:<br></p>" +
                            $"<p>- - - - - - - - - - - - - - - - - - - - - - - - - - - -</p>" +
                            $"<div style='background-color: #000; padding: 10px'><a href='#' style=' color: #fff; text-decoration: none; cursor: text;'>{pixQrCode}</a></div>" +
                            $"<p>- - - - - - - - - - - - - - - - - - - - - - - - - - - -</p>" +
                            $"<p>Em caso de dúvida, não hesite em nos contatar. Nossa equipe está pronta para oferecer todo o suporte necessário.</p>" +
                            $"<p>Atenciosamente,<br>Certidão Negativa";

                        var mensagemSms =
                            $"Olá {nome}, notamos que você ainda não finalizou a solicitação da sua certidão. Para receber a certidão no seu e-mail, basta finalizar o pagamento pelo Código PIX Copia e Cola: {pixQrCode}";

                        var updateCommand = new SqlCommand($"UPDATE Ticket SET Enviado = GETDATE() WHERE Id = {id}",
                            connection);
                        updateCommand.ExecuteNonQuery();

                        SmtpSendMail(sp, "Certidão Negativa", remetente, nome, email, mensagemEmail, mensagemEmail, "Sua Certidão", new Dictionary<string, string>());
                        //await SendSmsAsync(celular.RemoveCaracteres(new[] { "(", ")", "-", " " }), mensagemSms);
                    }
                }
            }

            connection.Close();
        }

        return Content("OK");
    }
    private static readonly HttpClient client = new();
    private static string Token { get; set; }
    public async Task SendSmsAsync(string phoneNumber, string message)
    {
        try
        {
            var url = "https://sms.comtele.com.br/api/v2/send";
            var requestData = new
            {
                Receivers = phoneNumber,
                Content = message
            };
            var json = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("auth-key", _configuration.GetSection("Comtele").Value);
            var response = await client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
        }
        catch (Exception e)
        {

        }

    }

    [HttpGet("/sucesso")]
    public IActionResult Sucesso()
    {
        return View();
    }

    //public async Task Teste()
    //{
    //    var item = new
    //    {
    //        calendario = new { expiracao = 3600 },
    //        devedor = new
    //        {
    //            cnpj = 12345678000195,
    //            nome = "Empresa de Serviços SA"
    //        },
    //        valor = new { original = 37.00 },
    //        chave = _configuration.GetSection("BB.ChavePix").Value,
    //        solcnpjitacaoPagador = "Serviço realizado"
    //    };
    //    var api = new PixBB(Ambiente.Homologacao,
    //        _configuration.GetSection("BB.Client_basic").Value,
    //        _configuration.GetSection("BB.Client_DeveloperAppKey").Value);
    //    var rest = await api.GeraPix("/cob", item);

    //    return;
    //}

    private static Dictionary<string, object> ObjectToDictionary(object obj)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj));

        var dictionary = new Dictionary<string, object>();

        foreach (var property in obj.GetType().GetProperties())
        {
            var key = property.Name;
            var value = property.GetValue(obj);

            dictionary.Add(key, value);
        }

        return dictionary;
    }

    //public async Task<JsonResult> GeraQrCodeBB(string cpf, string nome, string telefone)
    //{
    //    try
    //    {
    //        var item = new
    //        {
    //            calendario = new { expiracao = 3600 },
    //            devedor = new
    //            {
    //                cpf = cpf.Replace(".", "").Replace("-", ""),
    //                nome
    //            },
    //            valor = new { original = _configuration.GetSection("Valor").Value },
    //            chave = _configuration.GetSection("BB.ChavePix").Value,
    //            solicitacaoPagador = "Certidão Negativa"
    //        };
    //        var api = new PixBB(Ambiente.Homologacao,
    //            _configuration.GetSection("BB.Client_basic").Value,
    //            _configuration.GetSection("BB.Client_DeveloperAppKey").Value);
    //        var rest = await api.GeraPix("/cob", item);

    //        var qrGenerator = new QRCodeGenerator();
    //        var copiaCola = (string)rest.Item1["pixCopiaECola"];
    //        var qrCodeData = qrGenerator.CreateQrCode(copiaCola, QRCodeGenerator.ECCLevel.Q);
    //        var qrCode = new PngByteQRCode(qrCodeData);
    //        var svg = Convert.ToBase64String(qrCode.GetGraphic(20));

    //        return Json(new
    //        { cssClass = "success", chave = rest.Item1["pixCopiaECola"], imagem = svg, txid = rest.Item1["txid"] });
    //    }
    //    catch
    //    {
    //        return Json(new { cssClass = "error", mensagem = "Ocorreu um erro, tente novamente mais tarde!" });
    //    }
    //}

    private static void SmtpSendMail(Sendpulse sp, string fromName, string fromEmail, string nameTo, string emailTo,
        string html, string text, string subject, Dictionary<string, string> attachments)
    {
        var from = new Dictionary<string, object>
        {
            { "name", fromName },
            { "email", fromEmail }
        };
        var to = new ArrayList();
        var elementto = new Dictionary<string, object>
        {
            { "name", nameTo },
            { "email", emailTo }
        };
        to.Add(elementto);
        var emaildata = new Dictionary<string, object>
        {
            { "html", html },
            { "text", text },
            { "subject", subject },
            { "from", from },
            { "to", to }
        };
        if (attachments.Count > 0) emaildata.Add("attachments_binary", attachments);
        var result = sp.smtpSendMail(emaildata);
    }

    public TransferaApi CieloApi = new TransferaApi();
    public TransferaApiAutenticar CieloApiAutenticar = new TransferaApiAutenticar();

    public async Task<JsonResult> GeraQrCode(string cpf, string nome, string telefone, string email)
    {
        try
        {
            //var configuracao = _dataRepositoryConfiguracao.Find(a => a.Id == 1);
            //if (configuracao.Gateway == "transfeera")
            //{
            //    configuracao.Gateway = "pagarme";
            //    _dataRepositoryConfiguracao.Update(configuracao);
            //    await ValidaToken();

            //    var pix = await CieloApi.GerarPix(new PixPost()
            //    {
            //        Payer = new Payer() { Name = nome, Document = cpf.RemoveCaracteres(new[] { ".", "-" }) },
            //        PixKey = "5683e1a3-18d8-4f18-9696-da2c2d4ae067",
            //        OriginalValue = _configuration.GetSection("ValorSite").Value,
            //        Token = Token
            //    });

            //    return Json(new
            //    {
            //        cssClass = "success",
            //        imagem = pix.ImageBase64,
            //        txid = pix.Txid,
            //        chave = pix.EmvPayload
            //    });
            //}
            //else
            //{
            //    configuracao.Gateway = "transfeera";
            //    _dataRepositoryConfiguracao.Update(configuracao);

                var client = new PagarmeApiSDKClient.Builder()
                .BasicAuthCredentials(
                    _configuration.GetSection("PargarMe.Chave").Value, "")
                .ServiceRefererName("ServiceRefererName")
                .Build();

                telefone = telefone.RemoveCaracteres(new[] { "(", ")", "-", " " });
                var body = new CreateOrderRequest
                {
                    Items = new List<CreateOrderItemRequest>
                {
                    new()
                    {
                        Amount = int.Parse(_configuration.GetSection("Valor").Value),
                        Description = "Certidão Negativa",
                        Quantity = 1
                    }
                },
                    Customer = new CreateCustomerRequest
                    {
                        Name = nome,
                        Email = email,
                        Document = cpf.RemoveCaracteres(new[] { ".", "-" }),
                        Type = "individual",
                        Phones = new CreatePhonesRequest
                        {
                            MobilePhone = new CreatePhoneRequest("55", telefone.Substring(2, telefone.Length - 2),
                                telefone.Substring(0, 2))
                        }
                    },
                    Payments = new List<CreatePaymentRequest>
                {
                    new()
                    {
                        PaymentMethod = "pix",
                        Amount = int.Parse(_configuration.GetSection("Valor").Value),
                        Pix = new CreatePixPaymentRequest(DateTime.Now.AddDays(1))
                    }
                }
                };

                //var pey = JsonConvert.SerializeObject(body);
                //System.IO.File.WriteAllText(Path.Combine(_hostingEnvironment.WebRootPath, $"temp\\postpagPlay{DateTime.Now.ToFileTimeUtc()}.txt"), pey);
                var result = await client.OrdersController.CreateOrderAsync(body);

                var trasacao = (GetPixTransactionResponse)result.Charges[0].LastTransaction;
                var qrGenerator = new QRCodeGenerator();
                var qrCodeData = qrGenerator.CreateQrCode(trasacao.QrCode, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new PngByteQRCode(qrCodeData);
                var svg = Convert.ToBase64String(qrCode.GetGraphic(20));


                return Json(new
                {
                    cssClass = "success",
                    imagem = svg,
                    txid = result.Id,
                    chave = trasacao.QrCode
                });
            //}
        }
        catch (ApiException e)
        {
            //System.IO.File.WriteAllText(Path.Combine(_hostingEnvironment.WebRootPath, $"temp\\postpagErro{DateTime.Now.ToFileTimeUtc()}.txt"), JsonConvert.SerializeObject(e));
            //GeraQrCode(cpf, nome, telefone, email);
            return Json(new
            {
                cssClass = "warning",
                imagem = e.Message,
                txid = "",
                chave = "",
                mensagem = "Não foi possível gerar o pix, tente novamente em poucos minutos!"
            });
        }

        return Json(new
        {
            cssClass = "warning",
            imagem = "",
            txid = "",
            chave = "",
            mensagem = "Não foi possível gerar o pix, tente novamente em poucos minutos!"
        });
    }

    private async Task ValidaToken()
    {
        if (string.IsNullOrWhiteSpace(Token))
        {
            var autenticar = await CieloApiAutenticar.Autenticar(new AutentiacacaoPost()
            {
                Clientid = _configuration.GetSection("Transfera.ClienteId").Value,
                ClientSecret = _configuration.GetSection("Transfera.ClienteSecret").Value
            });
            if (autenticar.StatusCode == 0)
            {
                Token = autenticar.AccessToken;
            }
        }
    }

    [HttpPost]
    public async Task<JsonResult> SalvarTemp(Ticket model)
    {
        try
        {
            var url = "";
            model.Dominio = Request.Host.Host;
            model.StatusPagamento = "Aguardando";
            model.DataCadastro = DateTime.Now;
            model.UsuarioCadastro = model.Nome;
            var existe = _dataRepository.FindObject(
                a => a.TidTransacao == model.TidTransacao, a => a.Id);
            if (Equals(existe, null))
                _dataRepository.Add(model);
            return Json(new { cssClass = "success" });
        }
        catch (Exception e)
        {
            return Json(new { cssClass = "warning" });
        }
    }

    public async Task<JsonResult> ConsultaPix(string txid)
    {
        var rsposeQrCode = await _client.GetAsync($"{_url}/v3/payments/{txid}/status");
        var res = await rsposeQrCode.Content.ReadAsStringAsync();
        var jsonResponseQrCode = JObject.Parse(res);
        var status = jsonResponseQrCode["status"].ToString();
        return Json(status);
    }

    public async Task<JsonResult> ConsultaPagamento(string txid)
    {
        try
        {
            if (txid.Contains("or_"))
            {
                var client = new PagarmeApiSDKClient.Builder()
                    .BasicAuthCredentials(
                        _configuration.GetSection("PargarMe.Chave").Value, "")
                    .ServiceRefererName("ServiceRefererName")
                    .Build();

                var result = await client.OrdersController.GetOrderAsync(txid);
                return Json(result.Status);
            }

            await ValidaToken();
            var pix = await CieloApi.Consultar($"{Token}|{txid}");
            return Json(pix.Status);

        }
        catch (ApiException e)
        {
            return Json("Erro ao consultar no pagar-me");
        }
    }

    [HttpPost("/recebe-notificacao")]
    public async Task RecebeNotificacao()
    {
        using var reader = new StreamReader(Request.Body, Encoding.UTF8);
        var requestBody = await reader.ReadToEndAsync();
        //System.IO.File.WriteAllText(Path.Combine(_hostingEnvironment.WebRootPath, $"temp\\postRetorno{DateTime.Now.ToFileTimeUtc()}.txt"), requestBody);
        var tidTransacao = "";

        var dados = JsonConvert.DeserializeObject<WebHookReturn>(requestBody);
        if (dados is { Data.Status: "paid" }) tidTransacao = dados.Data.InvoiceId;
        //if (dados is { Object: "CashIn" }) tidTransacao = dados.Data.Txid;

        if (!string.IsNullOrWhiteSpace(tidTransacao))
        {
            await using var connection = new SqlConnection(_configuration.GetConnectionString("Conn"));
            connection.Open();
            var selectCommand =
                new SqlCommand($"SELECT TOP(1) Id FROM Ticket WHERE TidTransacao = '{dados.Data.InvoiceId}';",
                    connection);
            var readerData = selectCommand.ExecuteScalar();
            if (!Equals(readerData, null))
            {
                var updateCommand =
                    new SqlCommand($"UPDATE Ticket SET StatusPagamento = 'Pago' WHERE Id = {readerData}",
                        connection);
                updateCommand.ExecuteNonQuery();
            }

            connection.Close();
            //var item = _dataRepository.Find(a => a.TidTransacao == tidTransacao);
            //if (!Equals(item, null))
            //{
            //    item.StatusPagamento = "Pago";
            //    _dataRepository.Update(item);
            //}
        }
    }


    public ActionResult Index()
    {
        return View();
    }

    [HttpGet("/emitir/{tipo}")]
    public ActionResult Add(string tipo)
    {
        var ip = HttpContext.Connection.RemoteIpAddress.ToString();
        var lastVisitTime = _cache.Get<DateTime?>(ip);

        if (lastVisitTime == null || (DateTime.Now - lastVisitTime.Value).TotalMinutes >= 1)
        {
            _cache.Set(ip, DateTime.Now, TimeSpan.FromMinutes(1));
            var visit = new Visita { Data = DateTime.Now, Ip = ip, Dominio = HttpContext.Request.Host.Host };
            _dataRepositoryVisita.Add(visit);
        }

        ViewBag.Tipo = tipo;
        return View(new Ticket { Tipo = tipo, Valor = _configuration.GetSection("Valor").Value });
    }

    [HttpPost]
    public async Task<JsonResult> Salvar(Ticket model)
    {
        var url = "";
        if (ModelStateInvalido(out var json)) return json;
        if (!ModelState.IsValid) return Json(new { cssClass = "warning", mensagem = _mensagens.Value.ErroSalvar });


        model.Dominio = Request.Host.Host;
        model.DataCadastro = DateTime.Now;
        model.UsuarioCadastro = model.Nome;
        var existe = _dataRepository.Find(a => a.TidTransacao == model.TidTransacao);
        existe.StatusPagamento = "Pago";
        if (!_dataRepository.Update(existe))
            return Json(new { cssClass = "warning", mensagem = _mensagens.Value.ErroSalvar });
        await EnviaMensagem(model);

        return Json(new
        {
            janela = Request.Form["janela"].ToString(),
            cssClass = "success",
            mensagem = "Obrigado, logo logo entraremos em contato você você via What'sApp!",
            url
        });
    }

    private async Task EnviaMensagem(Ticket model)
    {
        var sTemplate = await System.IO.File.ReadAllTextAsync(Path.Combine(_hostingEnvironment.WebRootPath,
            "templates\\template.html"));

        var sb = new StringBuilder();
        sb.AppendFormat($"<p> Olá <strong>{model.Nome}</strong>,<br><br>");
        sb.AppendFormat("O pagamento do serviço abaixo foi <strong>APROVADO</strong><br><br>");
        sb.AppendFormat($"Serviço: {model.Tipo}<br>");
        sb.AppendFormat($"Nº do pedido: {model.Id}<br>");
        sb.AppendFormat($"Data do pagamento: {DateTime.Now:g}<br>");
        sb.AppendFormat("Meio de pagamento: PIX<br>");
        sb.AppendFormat($"Valor do pedido: {_configuration.GetSection("ValorSite").Value}<br><br>");
        sb.AppendFormat("Aguarde a chegada do protocolo de agendamento do certidao negativa em seu email.");
        sb.AppendFormat("</p>");

        sTemplate = sTemplate.Replace("{Conteudo}", sb.ToString());
        sTemplate = sTemplate.Replace("{Titulo}", "Obrigado ;)");
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

        var sp = new Sendpulse(userId, secret);
        SmtpSendMail(sp, "Certidão Negativa", _configuration.GetSection("Remetente").Value, model.Nome,
            model.Email, sTemplate, sTemplate, "Obrigado ;)", new Dictionary<string, string>());
    }

    private bool ValidaModel(Ticket model, out JsonResult salvar)
    {
        if (model.Emissor == "OUTROS" && string.IsNullOrWhiteSpace(model.EmissorOutro))
        {
            salvar = Json(new { cssClass = "warning", mensagem = "Informe o orgão emissor" });
            return true;
        }

        salvar = null!;
        return false;
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
        var secretKey = "6Lea3BQpAAAAAFRsGawosRehTT4vPmUIGPbyDwfw";
        var apiUrl = "https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}";
        var requestUri = string.Format(apiUrl, secretKey, captchaResponse);
        var request = (HttpWebRequest)WebRequest.Create(requestUri);

        using var response = request.GetResponse();
        using var stream = new StreamReader(response.GetResponseStream());
        var jResponse = JObject.Parse(stream.ReadToEnd());
        var isSuccess = jResponse.Value<bool>("success");
        result = isSuccess;
        return result;
    }

    [HttpPost]
    public async Task<JsonResult> ContatoEnviar(string nome, string celular, string email, string mensagem,
        string recaptcha)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(celular) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(mensagem))
                return Json(new { cssClass = "error", mensagem = "Preencha todos os campos!" });
            var assunto = "Sua Certidão";
            if (string.IsNullOrWhiteSpace(recaptcha)) return Json(new { cssClass = "error", mensagem = "Resolva o captcha!" });
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
            //string[] retEnvio;
            //ServicePointManager.SecurityProtocol =
            //    SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            //var sp = new Sendpulse(userId, secret);
            //Email.DisparaEmail(_configuration.GetSection("HostSmtp").Value,
            //    _configuration.GetSection("Remetente").Value, _configuration.GetSection("Destinatario").Value,
            //    "Contato", sTemplate, true, "", out var Res);

            var sp = new Sendpulse(userId, secret);
            SmtpSendMail(sp, nome, _configuration.GetSection("Remetente").Value, "Certidão",
                _configuration.GetSection("Destinatario").Value, sTemplate, sTemplate, "Contato",
                new Dictionary<string, string>());


            return Json(new
            {
                cssClass = "success",
                mensagem = "Obrigado pela mensagem<br>Iremos te responder em um prazo de até 24 horas."
            });
        }
        catch (Exception e)
        {
            return Json(new { cssClass = "erro", mensagem = "Ocorreu um erro, tente novamente em poucos minutos!" });
        }
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
                        var endereco = JsonConvert.DeserializeObject<Endereco>(json);
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