#region

using System.Collections;
using System.Net;
using System.Text;
using Domain;
using iDevCL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Repository;
using SendPulse;
using WebApp.Filter;
using WebApp.Models;

#endregion

namespace WebApp.Controllers;

public class InteracaoController : ApplicationController<Interacao>
{
    private static string userId;
    private static string secret;
    private IGenericDataRepository<Interacao> _dataRepository;
    private IGenericDataRepository<Pagador> _dataRepositoryPagador;
    private readonly IGenericDataRepository<Ticket> _dataRepositoryTicket;
    private readonly IWebHostEnvironment _hostingEnvironment;
    private readonly IOptions<Mensagens> _mensagens;

    public InteracaoController(IHttpContextAccessor httpContextAccessor, IConfiguration? configuration,
        IWebHostEnvironment environment,
        IGenericDataRepository<Interacao> dataRepository, IGenericDataRepository<Pagador> dataRepositoryPagador, IGenericDataRepository<Ticket> dataRepositoryTicket,
        IOptions<Mensagens> mensagens) : base(httpContextAccessor, configuration)
    {
        if (httpContextAccessor.HttpContext != null)
            NomeController = httpContextAccessor.HttpContext.Request.RouteValues["controller"]?.ToString();
        _dataRepository = dataRepository;
        _dataRepositoryPagador = dataRepositoryPagador;
        _dataRepositoryTicket = dataRepositoryTicket;
        _hostingEnvironment = environment;
        _mensagens = mensagens;
        userId = configuration.GetSection("SendPulse.UserId").Value;
        secret = configuration.GetSection("SendPulse.Secret").Value;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_dataRepository != null)
            {
                _dataRepository.Dispose();
                _dataRepositoryPagador.Dispose();
                _dataRepository = null;
            }
        }

        base.Dispose(disposing);
    }

    #region Admin

    [RequiredPermission("operador", "financeiro")]
    public virtual JsonResult ListByTicket(DataTablesParam param, int idTicket)
    {
        var admin = GetDadosUsuario<bool>("IsSuporte");
        var idPessoa = GetDadosUsuario<int>("IdUsuario");
        var perfil = GetDadosUsuario<string>("financeiro");

        var filter = new FilterExpression<Interacao>().Start(a => a.IdTicket == idTicket);
        filter = filter.And(a => a.Ticket!.IdPessoa == idPessoa, !admin && perfil != "liberado");
        Filtro = filter.ResultExpression;
        Selector = a => new object[] { $"{a.DataCadastro}", a.Mensagem!, a.Status!, a.Anexo, a.Id };
        ColumnNames = new[] { "DataCadastro", "Mensagem", "Status", "Anexo", "Id" };
        DataTypes = new[] { DataType.tDate, DataType.tString, DataType.tString, DataType.tString, DataType.tInt };
        Includes = new[] { "Ticket" };
        var lista = _dataRepository.GetAll(Filtro, Selector, param, out var totalRecords,
            out var totalRecordsDisplay, ColumnNames, DataTypes, Includes);
        var ret = new
        {
            iTotalRecords = totalRecords,
            iTotalDisplayRecords = totalRecordsDisplay,
            param.sEcho,
            aaData = lista
        };
        return Json(ret);
    }

    [RequiredPermission("operador", "financeiro")]
    public IActionResult Add(string id, string tipo, string dados, string janela)
    {
        var model = new Interacao { IdTicket = Convert.ToInt32(dados), Enviar = "Sim" };
        ViewData["tipo"] = tipo;
        ViewData["janela"] = janela;
        if (tipo == "update") model = _dataRepository.Find(a => a.Id == Convert.ToInt32(id));
        return View(model);
    }

    [HttpPost]
    [RequiredPermission("operador", "financeiro")]
    public async Task<JsonResult> Salvar(Interacao model, IFormFile? file = null)
    {
        var url = "";
        var ticket = _dataRepositoryTicket.Find(a => a.Id == model.IdTicket);
        if (ModelStateInvalido(out var json)) return json;
        if (!ModelState.IsValid) return Json(new { cssClass = "warning", mensagem = _mensagens.Value.ErroSalvar });

        if (Request.Form["tipo"].ToString() == "update")
        {
            if (!_dataRepository.Update(model))
                return Json(new { cssClass = "warning", mensagem = _mensagens.Value.ErroSalvar });
        }
        else
        {
            if (file != null)
            {
                var nome = $"{Guid.NewGuid().ToString().Replace("-", "")}.{Path.GetExtension(file.FileName)}";
                var uploadsRootFolder = Path.Combine(_hostingEnvironment.WebRootPath, "temp");
                if (!Directory.Exists(uploadsRootFolder)) Directory.CreateDirectory(uploadsRootFolder);
                await using var stream = System.IO.File.Create(Path.Combine(uploadsRootFolder, nome));
                await file.CopyToAsync(stream);
                model.Anexo = nome;
            }

            model.DataCadastro = DateTime.Now;
            if (!_dataRepository.Add(model))
                return Json(new { cssClass = "warning", mensagem = _mensagens.Value.ErroSalvar });

            switch (model.Status)
            {
                case "Preciso de resposta do cliente":
                    ticket.Status = "Aguardando resposta do cliente";
                    break;
                case "Encaminhar financeiro":
                    ticket.Status = "Financeiro";
                    break;
                case "Concluir":
                    ticket.Status = "Concluído";
                    ticket.DataFinalizado = DateTime.Now;
                    break;
            }

            _dataRepositoryTicket.Update(ticket);
        }

        if (model.Enviar == "Sim")
        {
            var assunto = "Solicitação de Certidão";

            var sTemplate = await System.IO.File.ReadAllTextAsync(Path.Combine(_hostingEnvironment.WebRootPath,
                "templates\\template.html"));

            var sb = new StringBuilder();
            sb.AppendFormat("<p>{0}</p>", model.Mensagem);

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
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 |
                                                   SecurityProtocolType.Tls;

            var sp = new Sendpulse(userId, secret);
            if (!string.IsNullOrWhiteSpace(model.Anexo))
            {
                var attachment = new Dictionary<string, string>();
                var contentBytes =
                    await System.IO.File.ReadAllBytesAsync(Path.Combine(_hostingEnvironment.WebRootPath, $"temp\\{model.Anexo}"));
                var base64FileContent = Convert.ToBase64String(contentBytes);
                attachment.Add(model.Anexo, base64FileContent);
                //How to attach files
                SmtpSendMail(sp, "Certidão Negativa", _configuration.GetSection("Remetente").Value, ticket.Nome,
                    ticket.Email, sTemplate, sTemplate, assunto, attachment);
            }
            else
            {
                SmtpSendMail(sp, "Certidão Negativa", _configuration.GetSection("Remetente").Value, ticket.Nome,
                    ticket.Email, sTemplate, sTemplate, assunto, new Dictionary<string, string>());
            }
        }

        return Json(new
        {
            janela = Request.Form["janela"].ToString(),
            cssClass = "success",
            mensagem = _mensagens.Value.Sucesso,
            url
        });
    }

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


    [RequiredPermission]
    public JsonResult Del(int id)
    {
        var model = _dataRepository.Find(a => a.Id == id);
        return Equals(model, null)
            ? Json(new { cssClass = "warning", mensagem = _mensagens.Value.NaoEncontrado })
            : Json(_dataRepository.Remove(model)
                ? new { cssClass = "success", mensagem = _mensagens.Value.SucessoDelete }
                : new { cssClass = "warning", mensagem = _mensagens.Value.ErroDelete });
    }

    #endregion
}