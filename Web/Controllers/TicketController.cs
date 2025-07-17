#region

using System.Collections;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Domain;
using iDevCL;
using LyTex;
using LyTex.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PagarmeApiSDK.Standard;
using PagarmeApiSDK.Standard.Models;
using QRCoder;
using Repository;
using SendPulse;
using Web.Filter;
using Web.Models;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

#endregion

namespace Web.Controllers;

public class TicketController : ApplicationController<Ticket>
{
    private static string userId;
    private static string secret;
    private static Service _services;

    private static readonly HttpClient client = new();
    private readonly IMemoryCache _cache;
    private readonly string _certificado;
    private readonly HttpClient _client;
    private readonly string _clienteId;
    private readonly string _clienteSecret;
    private readonly IGenericDataRepository<Ticket> _dataRepository;
    private readonly IGenericDataRepository<Configuracao> _dataRepositoryConfiguracao;
    private readonly IGenericDataRepository<Interacao> _dataRepositoryInteracao;
    private readonly IGenericDataRepository<Pagador> _dataRepositoryPagador;
    private readonly IGenericDataRepository<Pedido> _dataRepositoryPedido;
    private readonly IGenericDataRepository<Visita> _dataRepositoryVisita;
    private readonly IWebHostEnvironment _hostingEnvironment;
    private readonly IOptions<Mensagens> _mensagens;
    private readonly bool _sandBox;
    private readonly string _url;

    public TicketController(IHttpContextAccessor httpContextAccessor, IConfiguration? configuration,
        IWebHostEnvironment environment, IMemoryCache cache,
        IGenericDataRepository<Ticket> dataRepository, IGenericDataRepository<Interacao> dataRepositoryInteracao,
        IGenericDataRepository<Pedido> dataRepositoryPedido, IGenericDataRepository<Pagador> dataRepositoryPagador,
        IGenericDataRepository<Visita> dataRepositoryVisita,
        IGenericDataRepository<Configuracao> dataRepositoryConfiguracao,
        IOptions<Mensagens> mensagens) : base(httpContextAccessor, configuration)
    {
        if (httpContextAccessor.HttpContext != null)
            NomeController = httpContextAccessor.HttpContext.Request.RouteValues["controller"]?.ToString();
        _dataRepository = dataRepository;
        _dataRepositoryInteracao = dataRepositoryInteracao;
        _dataRepositoryPedido = dataRepositoryPedido;
        _dataRepositoryPagador = dataRepositoryPagador;
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
                             "SELECT Id, Nome, Tipo, PixQrCode, Celular, Email FROM Ticket WHERE StatusPagamento = 'Aguardando' AND Status = 'Em aberto' AND Enviado is null AND DataCadastro >= DATEADD(MINUTE, -2, GETDATE());",
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
                        var pixQrCode = reader["PixQrCode"].ToString();
                        var id = reader["Id"].ToString();

                        var mensagemEmail =
                            $"<p>Olá, {nome} notamos que você preencheu um de nossos formulários, mas ainda não finalizou a solicitação. Estamos aqui para te ajudar a dar o próximo passo e finalizar o seu pedido.</p>" +
                            $"<p>O serviço escolhido foi: {tipo}</p>" +
                            $"<p>Para receber a certidão no seu e-mail, basta finalizar o pagamento pelo Código PIX Copia e Cola abaixo: <strong>{pixQrCode}</strong></p>" +
                            $"<p><strong>Prazo de entrega:</strong> a maioria das certidões são entregues em até 2 horas. Certidões Estaduais podem variar de acordo com a comarca.</p>" +
                            $"<p>Se você tiver alguma dúvida ou precisar de mais informações, não hesite em nos contatar. Nossa equipe está pronta para oferecer todo o suporte necessário.</p>" +
                            $"<p>Atenciosamente,<br>Certidão Negativa";

                        var mensagemSms =
                            $"Olá {nome}, notamos que você ainda não finalizou a solicitação da sua certidão. Para receber a certidão no seu e-mail, basta finalizar o pagamento pelo Código PIX Copia e Cola: {pixQrCode}";

                        var updateCommand = new SqlCommand($"UPDATE Ticket SET Enviado = GETDATE() WHERE Id = {id}",
                            connection);
                        updateCommand.ExecuteNonQuery();

                        SmtpSendMail(sp, "Certidão Negativa", remetente, nome, email, mensagemEmail, mensagemEmail, "Sua Certidão", new Dictionary<string, string>());
                        await SendSmsAsync(celular.RemoveCaracteres(new[] { "(", ")", "-", " " }), mensagemSms);
                    }
                }
            }

            connection.Close();
        }

        return Content("OK");
    }

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

    [HttpGet("/sair")]
    public ActionResult LogOff()
    {
        HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Ticket");
    }

    [HttpGet("/sucesso")]
    public IActionResult Sucesso()
    {
        return View();
    }

    [HttpGet("/acompanhar")]
    [RequiredPermission]
    public IActionResult Acompanhar()
    {
        var idUsuario = GetDadosUsuario<int>("IdUsuario");
        var usuario = _dataRepositoryPagador.Find(a => a.Id == idUsuario);
        var pediddos = _dataRepositoryPedido.GetAll(a => a.IdPagador == idUsuario);
        var tickets = _dataRepository.GetAll(a => a.IdPagador == idUsuario);
        var interacoes = _dataRepositoryInteracao.GetAll(a =>
            tickets.Select(c => c.Id).Contains(a.IdTicket.Value) && !string.IsNullOrWhiteSpace(a.Anexo));
        return View(
            new Tuple<Pagador, IList<Pedido>, IList<Ticket>, IList<Interacao>>(usuario, pediddos, tickets, interacoes));
    }

    [HttpGet("/entrar")]
    public IActionResult Entrar()
    {
        return View();
    }

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

    private static void SmtpSendMail(Sendpulse sp, string fromName, string fromEmail, string nameTo, string emailTo,
        string html, string text, string subject, Dictionary<string, string> attachments)
    {
        try
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
        catch (Exception e)
        {

        }
    }

    public async Task<JsonResult> GeraQrCode(string cpf, string nome, string telefone, string email, string valor,
        int idPagador, string senha)
    {
        var model = new Pagador();
        try
        {
            if (!User.Identity.IsAuthenticated)
            {
                if (idPagador > 0)
                {
                    model = _dataRepositoryPagador.Find(a => a.Id == idPagador);
                    if (!PasswordHash.ValidatePassword(senha, model.Senha))
                        return Json(new { cssClass = "warning", mensagem = "Senha inválida" });

                    LoginAsync(model);
                }
                else
                {
                    model = new Pagador
                    {
                        Nome = nome,
                        Cpf = cpf,
                        Telefone = telefone,
                        Email = email,
                        Senha = PasswordHash.CreateHash(senha),
                        Data = DateTime.Now
                    };
                    if (_dataRepositoryPagador.Add(model)) LoginAsync(model);
                }
            }
            else
            {
                model.Id = GetDadosUsuario<int>("IdUsuario");
            }
            //var configuracao = _dataRepositoryConfiguracao.Find(a => a.Id == 1);
            //if (configuracao.Gateway == "lytex")
            //{
            //    configuracao.Gateway = "pagarme";
            //    _dataRepositoryConfiguracao.Update(configuracao);

            //    if (!Equals(_configuration, null))
            //    {
            //await _services.GerarToken(_configuration.GetSection("LyTex.ClientId").Value,
            //    _configuration.GetSection("LyTex.ClientSecret").Value,
            //    new[]
            //    {
            //                "invoice", "client", "subscription", "paymentLink", "installment", "product", "billingRule"
            //    });

            //var consultaCliente =
            //    await _services.ConsultarCliente(cpf.Replace(".", "").Replace("-", "").Replace("/", ""));
            //var idCliente = "";
            //if (consultaCliente!.Results.Count == 0)
            //{
            //    var cliente = await _services.CadastarCliente(cpf, nome, email, telefone);
            //    if (Equals(cliente, null))
            //        return Json(new
            //        {
            //            cssClass = "warning",
            //            dados = "",
            //            mensagem = "Erro na geração do pix tente novamente em poucos minutos!"
            //        });

            //    idCliente = cliente.Id;
            //}
            //else
            //{
            //    idCliente = consultaCliente.Results[0].Id;
            //}

            //var conbranca = await _services.GerarCobranca(idCliente,
            //    new Item[]
            //    {
            //                new()
            //                {
            //                    Name = "Certidão Negativa", Quantity = 1,
            //                    Value = long.Parse(_configuration.GetSection("Valor").Value)
            //                }
            //    });

            //if (Equals(conbranca, null))
            //    return Json(new
            //    {
            //        cssClass = "warning",
            //        dados = "",
            //        mensagem = "Erro na geração do pix tente novamente em poucos minutos!"
            //    });

            //var qrGenerator = new QRCodeGenerator();
            //var copiaCola = conbranca.PaymentMethods.Pix.Qrcode;
            //var qrCodeData = qrGenerator.CreateQrCode(copiaCola, QRCodeGenerator.ECCLevel.Q);
            //var qrCode = new PngByteQRCode(qrCodeData);
            //var svg = Convert.ToBase64String(qrCode.GetGraphic(20));
            //return Json(new
            //{
            //    cssClass = "success",
            //    imagem = svg,
            //    txid = conbranca.Id,
            //    chave = conbranca.PaymentMethods.Pix.Qrcode
            //});
            //    }
            //}
            //else
            //{
            //configuracao.Gateway = "lytex";
            //_dataRepositoryConfiguracao.Update(configuracao);

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
                        Amount = int.Parse(valor.Replace(",", "")),
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


            var result = await client.OrdersController.CreateOrderAsync(body);
            var trasacao = (GetPixTransactionResponse)result.Charges[0].LastTransaction;
            //var tdi = "00020126570014";
            //var trasacao =
            //    new
            //    {
            //        QrCode =
            //            "00020126570014br.gov.bcb.pix0111811177100250220testede envio de pix52040000530398654041.235802BR5914testechave cpf6008saopaulo62070503***6304E067"
            //    };
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(trasacao.QrCode, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(qrCodeData);
            var svg = Convert.ToBase64String(qrCode.GetGraphic(20));


            return Json(new
            {
                cssClass = "success",
                imagem = svg,
                txid = result.Id,
                //txid = tdi,
                idPagador = model.Id,
                chave = trasacao.QrCode,
                valor
            });
            //}
        }
        catch (Exception e)
        {
            GeraQrCode(cpf, nome, telefone, email, valor, idPagador, senha);
        }

        return Json(new
        {
            cssClass = "warning",
            imagem = "",
            txid = "",
            chave = "",
            idPagador = model.Id,
            mensagem = "Não foi possível gerar o pix, tente novamente em poucos minutos!"
        });
    }

    private void LoginAsync(Pagador model)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, model.Nome!),
            new("IdUsuario", model.Id.ToString()),
            new("NomeUsuario", model.Nome!),
            new("Cpf", model.Cpf!),
            new("Celular", model.Telefone!),
            new("Email", model.Email!),
            new(ClaimTypes.Name, model.Nome!)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties();
        HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity), authProperties);
        var principal1 = new ClaimsPrincipal(claimsIdentity);
        Thread.CurrentPrincipal = new ClaimsPrincipal(principal1);
    }

    //[HttpPost]
    //public async Task<JsonResult> SalvarTemp(Ticket model)
    //{
    //    try
    //    {
    //        var url = "";
    //        model.StatusPagamento = "Aguardando";
    //        model.DataCadastro = DateTime.Now;
    //        model.UsuarioCadastro = model.Nome;
    //        var existe = _dataRepository.FindObject(
    //            a => a.TidTransacao == model.TidTransacao, a => a.Id);
    //        if (Equals(existe, null))
    //            _dataRepository.Add(model);
    //        return Json(new { cssClass = "success" });
    //    }
    //    catch (Exception e)
    //    {
    //        return Json(new { cssClass = "warning" });
    //    }

    //}

    [HttpPost]
    public async Task<JsonResult> SalvarListTemp([FromBody] object[] solicitacoes, int idPagador, string valor,
        string pixCopiaCola, string txid)
    {
        try
        {
            var existe = _dataRepositoryPedido.FindObject(a => a.TidTransacao == txid, a => a.Id);
            if (Equals(existe, null))
            {
                var pedido = new Pedido
                {
                    IdPagador = idPagador,
                    Valor = valor,
                    PixCopiaCola = pixCopiaCola,
                    TidTransacao = txid,
                    StatusPagamento = "Processando"
                };
                _dataRepositoryPedido.Add(pedido);
                foreach (var item in solicitacoes)
                {
                    var model1 = JsonConvert.DeserializeObject<dynamic>(item.ToString());
                    var model = JsonConvert.DeserializeObject<Ticket>(model1.ToString());

                    var email = _dataRepositoryPagador.FindObject(a => a.Id == idPagador, a => a.Email).ToString();

                    var url = "";
                    model.Email = email;
                    model.IdPagador = idPagador;
                    model.IdPedido = pedido.Id;
                    model.StatusPagamento = "Aguardando";
                    model.DataCadastro = DateTime.Now;
                    model.UsuarioCadastro = model.Nome;

                    _dataRepository.Add(model);
                }

                return Json(new { cssClass = "success" });
            }

            return Json(new { cssClass = "warning" });
        }
        catch (Exception e)
        {
            return Json(new { cssClass = "warning" });
        }
    }

    //public async Task<JsonResult> ConsultaPix(string txid)
    //{
    //    var rsposeQrCode = await _client.GetAsync($"{_url}/v3/payments/{txid}/status");
    //    var res = await rsposeQrCode.Content.ReadAsStringAsync();
    //    var jsonResponseQrCode = JObject.Parse(res);
    //    var status = jsonResponseQrCode["status"].ToString();
    //    return Json(status);
    //}

    public async Task<JsonResult> ConsultaPagamento(string txid)
    {
        var client = new PagarmeApiSDKClient.Builder()
            .BasicAuthCredentials(
                _configuration.GetSection("PargarMe.Chave").Value, "")
            .ServiceRefererName("ServiceRefererName")
            .Build();

        var consultaCliente = await client.OrdersController.GetOrderAsync(txid);
        if (consultaCliente.Status == "paid")
        {
            await using var connection = new SqlConnection(_configuration.GetConnectionString("Conn"));
            connection.Open();
            var selectCommand =
                new SqlCommand($"SELECT TOP(1) Id FROM Pedido WHERE TidTransacao = '{txid}';",
                    connection);
            var readerData = selectCommand.ExecuteScalar();
            if (!Equals(readerData, null))
            {
                var updateCommand =
                    new SqlCommand(
                        $"UPDATE Ticket SET StatusPagamento = 'Pago' WHERE IdPedido = {readerData};UPDATE Pedido SET StatusPagamento = 'Pago' WHERE Id = {readerData}",
                        connection);
                updateCommand.ExecuteNonQuery();
            }

            connection.Close();
        }

        return Json(consultaCliente.Status);
    }

    [HttpPost("/recebe-notificacao")]
    public async Task RecebeNotificacao()
    {
        using var reader = new StreamReader(Request.Body, Encoding.UTF8);
        var requestBody = await reader.ReadToEndAsync();
        System.IO.File.WriteAllText(
            Path.Combine(_hostingEnvironment.WebRootPath, $"temp\\postRetorno{DateTime.Now.ToFileTimeUtc()}.txt"),
            requestBody);
        var tidTransacao = "";

        var dados = JsonConvert.DeserializeObject<WebHookReturn>(requestBody);
        if (dados is { Data.Status: "paid" }) tidTransacao = dados.Data.InvoiceId;

        if (!string.IsNullOrWhiteSpace(tidTransacao))
        {
            await using var connection = new SqlConnection(_configuration.GetConnectionString("Conn"));
            connection.Open();
            var selectCommand =
                new SqlCommand($"SELECT TOP(1) Id FROM Pedido WHERE TidTransacao = '{dados.Data.InvoiceId}';",
                    connection);
            var readerData = selectCommand.ExecuteScalar();
            if (!Equals(readerData, null))
            {
                var updateCommand =
                    new SqlCommand(
                        $"UPDATE Ticket SET StatusPagamento = 'Pago' WHERE IdPedido = {readerData};UPDATE Pedido SET StatusPagamento = 'Pago' WHERE Id = {readerData}",
                        connection);
                updateCommand.ExecuteNonQuery();
                var ticket = _dataRepository.Find(a => a.TidTransacao == tidTransacao);
                await EnviaMensagem(ticket);
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

        ViewData["tipo"] = tipo;
        return View(new Ticket { Tipo = tipo, Valor = _configuration.GetSection("ValorSite").Value.Replace(".", ",") });
    }

    [HttpPost]
    public async Task<JsonResult> Salvar(Ticket model)
    {
        var url = "";
        if (ModelStateInvalido(out var json)) return json;
        if (!ModelState.IsValid) return Json(new { cssClass = "warning", mensagem = _mensagens.Value.ErroSalvar });


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
        sb.AppendFormat($"Valor do pedido: {_configuration.GetSection("Valor").Value}<br><br>");
        sb.AppendFormat("Aguarde a chegada do protocolo de agendamento do certidao negativa em seu email.");
        sb.AppendFormat("</p>");
        sb.AppendFormat("<p>Dados para acompanhamento: ");
        sb.AppendFormat($"E-mail: {model.Email}<br>");
        sb.AppendFormat(
            $"Use a opção recuperar senha para gerar uma senha de acesso: <br><a href='{_configuration.GetSection("Valor").Value}/entrar'>Recuperar senha</a>");
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
            //if (string.IsNullOrWhiteSpace(recaptcha)) return Json(new { cssClass = "error", mensagem = "Resolva o captcha!" });
            //if (!VerificaCaptcha(recaptcha)) return Json(new { cssClass = "error", mensagem = "Falha na verificação captcha!" });

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

    [HttpPost]
    [RequiredPermission]
    public async Task<JsonResult> UpdatePerfil(string nome, string cpf, string email, string senhaAtual,
        string novaSenha, string confirmarSenha)
    {
        try
        {
            var pagador = _dataRepositoryPagador.Find(a => a.Id == GetDadosUsuario<int>("IdUsuario"));

            if (!string.IsNullOrWhiteSpace(senhaAtual) && !string.IsNullOrWhiteSpace(novaSenha))
            {
                if (string.IsNullOrWhiteSpace(confirmarSenha))
                    return Json(new { cssClass = "error", mensagem = "Repita a senha!" });
                if (confirmarSenha != novaSenha)
                    return Json(new { cssClass = "error", mensagem = "As novas senha não confererem!" });
                if (!PasswordHash.ValidatePassword(senhaAtual, pagador.Senha))
                    return Json(new { cssClass = "error", mensagem = "Senha atual não confere!" });
                pagador.Senha = PasswordHash.CreateHash(novaSenha);
            }

            var existeEmail =
                _dataRepositoryPagador.Find(a => a.Id != GetDadosUsuario<int>("IdUsuario") && a.Email == email);
            if (!Equals(existeEmail, null))
                return Json(new
                { cssClass = "error", mensagem = "E-mail já cadastrado, tente informar outro e-mail!" });

            var existeCpf = _dataRepositoryPagador.Find(a => a.Id != GetDadosUsuario<int>("IdUsuario") && a.Cpf == cpf);
            if (!Equals(existeCpf, null))
                return Json(new { cssClass = "error", mensagem = "CPF já cadastrado, tente informar outro CPF!" });

            pagador.Nome = nome;
            pagador.Cpf = cpf;
            pagador.Email = email;

            if (_dataRepositoryPagador.Update(pagador))
                return Json(new { cssClass = "success", mensagem = "Dados atualizados com sucesso!" });

            return Json(new { cssClass = "erro", mensagem = "Ocorreu um erro, tente novamente em poucos minutos!" });
        }
        catch (Exception e)
        {
            return Json(new { cssClass = "erro", mensagem = "Ocorreu um erro, tente novamente em poucos minutos!" });
        }
    }

    [HttpPost]
    public async Task<JsonResult> ValidarDados(string email, string senha)
    {
        try
        {
            var model = _dataRepositoryPagador.Find(a => a.Email == email);
            if (Equals(model, null)) return Json(new { cssClass = "error", mensagem = "E-mail ou senha inválidos!" });
            if (!PasswordHash.ValidatePassword(senha, model.Senha))
                return Json(new { cssClass = "error", mensagem = "E-mail ou senha inválidos!" });
            LoginAsync(model);
            return Json(new { cssClass = "success", mensagem = "" });
        }
        catch (Exception e)
        {
            return Json(new { cssClass = "erro", mensagem = "Ocorreu um erro, tente novamente em poucos minutos!" });
        }
    }

    [HttpPost]
    public async Task<JsonResult> RecuperarSenha(string email)
    {
        try
        {
            var model = _dataRepositoryPagador.Find(a => a.Email == email);
            if (Equals(model, null)) return Json(new { cssClass = "error", mensagem = "E-mail não encontrado!" });
            var novaSenha = Guid.NewGuid().ToString().Split('-')[0];
            model.Senha = PasswordHash.CreateHash(novaSenha);
            _dataRepositoryPagador.Update(model);

            var sb = new StringBuilder();
            sb.AppendFormat("<p> Por sua solicitação, segue abaixo uma nova senha de acesso:<br><br>");
            sb.AppendFormat("<strong>E-mail:</strong> {0}<br>", email);
            sb.AppendFormat("<strong>Nova senha:</strong> {0}", novaSenha);
            sb.AppendFormat("</p>");
            sb.AppendFormat("<p>Ao efetuar login, modifique sua senha</p>");
            var sTemplate = await System.IO.File.ReadAllTextAsync(Path.Combine(_hostingEnvironment.WebRootPath,
                "templates\\template.html"));
            sTemplate = sTemplate.Replace("{Conteudo}", sb.ToString());
            sTemplate = sTemplate.Replace("{Titulo}", "Recuperação de senha");
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

            var sp = new Sendpulse(userId, secret);
            SmtpSendMail(sp, _configuration.GetSection("Sistema").Value, _configuration.GetSection("Remetente").Value,
                model.Nome,
                model.Email, sTemplate, sTemplate, "Recuperação de senha", new Dictionary<string, string>());
            return Json(new
            {
                cssClass = "success",
                mensagem = "Uma nova senha foi enviada ao seu e-mail, acesse seu e-mail e verifique!"
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

    public JsonResult GetPagador(string email)
    {
        var pagador = _dataRepositoryPagador.FindObject(a => !string.IsNullOrWhiteSpace(a.Email) && a.Email == email,
            a => new { a.Telefone, a.Nome, a.Id });
        return Json(pagador);
    }
}