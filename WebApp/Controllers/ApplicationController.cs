#region

using System.Net;
using System.Net.Mail;
using iDevCL;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace WebApp.Controllers;

public class ApplicationController<T> : Controller
{
    protected static IConfiguration? _configuration;
    protected string? NomeController;

    public ApplicationController(IHttpContextAccessor httpContextAccessor, IConfiguration? configuration)
    {
        var httpRequestFeature = httpContextAccessor.HttpContext.Features.Get<IHttpRequestFeature>();
        ViewBag.Url = httpRequestFeature.RawTarget;
        ViewBag.Edit = "";
        ViewBag.Del = "";
        _configuration = configuration;
        //if (GetDadosUsuario<bool>("IsSuporte", httpContextAccessor))
        //{
        //    ViewBag.Edit = "";
        //    ViewBag.Del = "";
        //}
        //else
        //{
        //    var controller = httpContextAccessor.HttpContext.Request.RouteValues["controller"];
        //    var acesso = GetDadosUsuario<string>("Permissoes", httpContextAccessor);
        //    if (!string.IsNullOrWhiteSpace(acesso))
        //    {
        //        if (acesso.Split(',').Any(a => a == $"{controller}Edit")) ViewBag.Edit = "";
        //        if (acesso.Split(',').Any(a => a == $"{controller}Del")) ViewBag.Del = "";
        //    }
        //}
    }

    protected Func<T, bool> Filtro { get; set; }
    protected Func<T, object> Selector { get; set; }
    protected string[] ColumnNames { get; set; }
    protected DataType[] DataTypes { get; set; }
    protected string[] Includes { get; set; }
    public Func<T, object> OrderByExpression { get; set; }

    public bool IsSuporte(out int idEmpresa)
    {
        var suporte = GetDadosUsuario<bool>("IsSuporte");
        if (suporte)
        {
            idEmpresa = 0;
            return true;
        }

        idEmpresa = GetDadosUsuario<int>("IdEmpresa");
        return false;
    }

    protected bool ModelStateInvalido(out JsonResult json)
    {
        if (!ModelState.IsValid)
        {
            var ret = "";
            foreach (var modelState in ViewData.ModelState.Values.Where(a => a.Errors.Count > 0))
            foreach (var error in modelState.Errors)
                ret += $"{error.ErrorMessage} | ";
            {
                json = Json(new { cssClass = "warning", mensagem = ret });
                return true;
            }
        }

        json = null;
        return false;
    }

    public async Task<bool> EnviarEmail(string nomeRemetente, string assunto, string emailResposta,
        string mensagem)
    {
        try
        {
            var dadosSmtp = _configuration?.GetSection("HostSmtp").Value?.Split(',').ToList();
            var mail = new MailMessage
            {
                From = new MailAddress(dadosSmtp[3], nomeRemetente)
            };

            mail.To.Add(emailResposta);
            //mail.ReplyToList.Add(Startup.Configuration.GetSection("Destinatario").Value);
            mail.Subject = assunto;
            mail.Body = mensagem;
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;

            using var smtp = new SmtpClient(dadosSmtp[0], int.Parse(dadosSmtp[1]))
            {
                Credentials = new NetworkCredential(dadosSmtp[3], dadosSmtp[4])
            };
            if (bool.Parse(dadosSmtp[2])) smtp.EnableSsl = true;
            await smtp.SendMailAsync(mail);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public TT GetDadosUsuario<TT>(string campo) where TT : IConvertible
    {
        var claims = User;
        if (claims == null) return (TT)Convert.ChangeType(null, typeof(TT));
        var item = claims.Claims.FirstOrDefault(c => c.Type == campo)?.Value;
        return (TT)Convert.ChangeType(item, typeof(TT));
    }
}