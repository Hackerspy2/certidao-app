#region

using System.Security.Claims;
using System.Text;
using Domain;
using iDevCL;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Repository;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using WebApp.Filter;
using WebApp.Models;

#endregion

namespace WebApp.Controllers;

public class PessoaController : ApplicationController<Pessoa>
{
    private IGenericDataRepository<Pessoa> _dataRepository;
    private readonly IWebHostEnvironment _hostingEnvironment;
    private readonly IOptions<Mensagens> _mensagens;

    public PessoaController(IHttpContextAccessor httpContextAccessor, IConfiguration? configuration,
        IWebHostEnvironment environment, IGenericDataRepository<Pessoa> dataRepository, IOptions<Mensagens> mensagens) :
        base(httpContextAccessor, configuration)
    {
        _dataRepository = dataRepository;
        _mensagens = mensagens;
        _hostingEnvironment = environment;
        if (httpContextAccessor.HttpContext != null)
            NomeController = httpContextAccessor.HttpContext.Request.RouteValues["controller"]?.ToString();
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

    #region Logar

    [HttpPost("recuperar-senha")]
    public async Task<JsonResult> RecuperarSenha([FromForm(Name = "emailReq")] string emailReq)
    {
        if (!Email.ValidaEmail(emailReq)) return Json(new { cssClass = "warning", mensagem = "E-mail inválido" });

        var model = _dataRepository.Find(a =>
            !string.IsNullOrWhiteSpace(a.Email) && a.Email.ToLower() == emailReq.ToLower());
        if (Equals(model, null)) return Json(new { cssClass = "warning", mensagem = "E-mail não encontrado" });

        model.Token = Guid.NewGuid().ToString().ToLower();
        var dados = _dataRepository.Update(model);
        if (!dados) return Json(new { cssClass = "warning", mensagem = "Erro ao recuperar sua senha, tente novamente mais tarde" });

        var sTemplate = await System.IO.File.ReadAllTextAsync(Path.Combine(_hostingEnvironment.WebRootPath,
            "templates\\template.html"));
        var sb = new StringBuilder();
        sb.Append($"<p>Olá {model.Nome}, tudo bem?</p><br />");
        sb.Append(
            $"<p>Recebemos uma solicitação para redefinir sua senha do {_configuration?.GetSection("Sistema").Value}.<br /> Clique no link abaixo e crie uma nova senha:<br /><br /><a href='{_configuration?.GetSection("Url").Value}/criar-nova-senha/{model.Token}'>Criar uma nova senha</a></p>");
        sb.Append(
            "<br /><p>Caso você não tenha realizado está solicitação, ignore esta mensagem e sua senha permanece a mesma.</p>");
        sTemplate = sTemplate.Replace("{Conteudo}", sb.ToString());
        sTemplate = sTemplate.Replace("{Titulo}", "Recuperação de senha");
        sTemplate = sTemplate.Replace("{Url}", _configuration?.GetSection("Url").Value);
        sTemplate = sTemplate.Replace("{UrlLimpo}",
            _configuration?.GetSection("Url").Value.RemoveCaracteres(new[] { "http://", "https://" }));
        sTemplate = sTemplate.Replace("{Destinatario}", _configuration?.GetSection("Destinatario").Value);
        sTemplate = sTemplate.Replace("{UrlInstagram}", _configuration?.GetSection("UrlInstagram").Value);
        sTemplate = sTemplate.Replace("{UrlFacebook}", _configuration?.GetSection("UrlFacebook").Value);
        sTemplate = sTemplate.Replace("{Whats}", _configuration?.GetSection("Whats").Value);
        sTemplate = sTemplate.Replace("{WhatsLimpo}",
            _configuration?.GetSection("Whats").Value.RemoveCaracteres(new[] { "(", ")", "-", " " })
                .Trim());

        var retorno = await EnviarEmail(_configuration?.GetSection("Sistema").Value!,
            "Recuperação de Senha", emailReq, sTemplate);

        return Json(new
        {
            url = "",
            janela = "pai",
            cssClass = retorno ? "success" : "error",
            mensagem = retorno
                ? "Acesse seu e-mail para criar uma nova senha de acesso!"
                : "Erro ao envia sua mensagem tente novamente mais tarde!"
        });
    }
    
    [HttpGet("sair")]
    public ActionResult LogOff()
    {
        HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Logar", "Pessoa");
    }

    [HttpGet("/entrar")]
    public ActionResult Logar(string returnUrl)
    {
        ViewBag.returnUrl = returnUrl;
        return View();
    }

    [HttpGet("criar-nova-senha/{token}")]
    public ActionResult CriarSenha(string token)
    {
        var model = _dataRepository.Find(a => a.Token == token);
        ViewBag.Token = model.Status == null ? "" : token;
        return View();
    }

    [HttpPost("/salvar-senha")]
    public JsonResult SalvarSenha(string token, string senha, string senha1)
    {
        if (senha.Length < 8)
            return Json(new { cssClass = "warning", mensagem = "Informe uma senha com pelo menos 8 caracteres" });
        if (senha != senha1) return Json(new { cssClass = "warning", mensagem = "As duas senha devem ser iguais" });

        var model = _dataRepository.Find(a => !string.IsNullOrWhiteSpace(a.Token) && a.Token == token);
        if (model.Status == null)
            return Json(new { cssClass = "warning", mensagem = "Token inválido" });

        model.Senha = PasswordHash.CreateHash(senha);
        model.Token = "";
        var updated = _dataRepository.Update(model);
        if (updated)
            return Json(new
            {
                cssClass = "success",
                mensagem = "Senha alterada com sucesso, você será redirecionado!",
                janela = "pai",
                url = "/entrar"
            });
        return Json(new
            { cssClass = "warning", mensagem = "Erro ao alterar sua senha, tente novamente mais tarde" });
    }

    [HttpPost("valida-dados")]
    public JsonResult Validar(string email, string senha, string returnUrl)
    {
        if (!Email.ValidaEmail(email)) return Json(new { cssClass = "warning", mensagem = "E-mail inválido" });

        var model = _dataRepository.Find(a =>
            !string.IsNullOrWhiteSpace(a.Email) && a.Email.ToLower() == email.ToLower());
        if (Equals(model, null)) return Json(new { cssClass = "warning", mensagem = "E-mail ou senha inválidos" });

        if (!PasswordHash.ValidatePassword(senha, model.Senha))
            return Json(new { cssClass = "warning", mensagem = "E-mail ou senha inválidos" });

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, model.Email!),
            new("IdUsuario", model.Id.ToString()),
            new("NomeUsuario", model.Nome!),
            new("IsSuporte", model.IsSuporte.ToString()),
            new(ClaimTypes.Name, model.Nome!)
        };
        if (!model.IsSuporte)
        { 
            if(model.Perfil == "Operador") claims.Add(new Claim("operador", "liberado"));
            if(model.Perfil != "Operador") claims.Add(new Claim("financeiro", "liberado"));
        }

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties();

        HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity), authProperties);

        var principal1 = new ClaimsPrincipal(claimsIdentity);
        Thread.CurrentPrincipal = new ClaimsPrincipal(principal1);
        return Json(new
        {
            janela = "pai",
            cssClass = "success",
            mensagem = "Autenticado com sucesso, redirecionando!",
            url = !string.IsNullOrEmpty(returnUrl) ? returnUrl : Url.Action("DashBoard", "Home")
        });
    }

    #endregion

    #region Logado

    [RequiredPermission]
    public JsonResult GetAllDados(int id)
    {
        var dados = _dataRepository.Find(a => a.Id == id);
        return Json(new { data = dados });
    }

    [RequiredPermission]
    public JsonResult AutoComplete(string q)
    {
        var filtro =
            new FilterExpression<Pessoa>().Start(a => a.Nome!.ToLower().Contains(q.ToLower()),
                !string.IsNullOrEmpty(q));
        var dados = _dataRepository.GetAllOrder(filtro.ResultExpression,
            a => new { Id = a.Id.ToString(), a.Nome }, a => a.Nome, "asc");
        return Json(dados);
    }

    [RequiredPermission]
    public JsonResult GetDados()
    {
        var filtro = new FilterExpression<Pessoa>().Start(a => !string.IsNullOrWhiteSpace(a.Nome) && !a.IsSuporte);
        var dados = _dataRepository.GetAllOrder(filtro.ResultExpression,
            a => new { Value = a.Id.ToString(), Text = a.Nome }, a => a.Nome, "asc").ToList();
        dados.Insert(0, new { Value = "", Text = "" });
        return Json(new { data = dados });
    }

    public JsonResult ValidaEmail(string email, int id)
    {
        if (!Email.ValidaEmail(email)) return Json(new { cssClass = "warning", mensagem = "E-mail inválido" });

        var filtro = new FilterExpression<Pessoa>().Start(a =>
            !string.IsNullOrWhiteSpace(a.Email) && a.Email.ToLower() == email.ToLower());
        filtro = filtro.And(a => a.Id != id, id > 0);
        var model = _dataRepository.Find(filtro.ResultExpression);
        return Json(!Equals(model, null)
            ? new { cssClass = "warning", mensagem = "E-mail já cadastro" }
            : new { cssClass = "success", mensagem = "" });
    }

    public JsonResult ValidaCpf(string dados, int id)
    {
        if (!IdevString.ValidaCpf(dados)) return Json(new { cssClass = "warning", mensagem = "CPF inválido" });

        var filtro = new FilterExpression<Pessoa>().Start(a => !string.IsNullOrWhiteSpace(a.Cpf) && a.Cpf == dados);
        filtro = filtro.And(a => a.Id != id, id > 0);
        var model = _dataRepository.Find(filtro.ResultExpression);
        return !Equals(model, null)
            ? Json(new { cssClass = "warning", mensagem = "CPF já cadastro", model!.Id })
            : Json(new { cssClass = "success", mensagem = "" });
    }

    [RequiredPermission]
    public IActionResult Add(string dados, string id, string acao)
    {
        var model = new Pessoa
        {
            IsUsuario = true,
            IsSuporte = true,
            Nome = dados,
            Status = "Ativo"
        };

        if (acao == "update") model = _dataRepository.Find(a => a.Id == Convert.ToInt32(id));
        ViewData["acao"] = acao;
        ViewData["janela"] = "filha";
        return View(model);
    }

    [HttpPost]
    [RequiredPermission]
    public JsonResult Salvar(Pessoa model)
    {
        var url = "";
        if (ModelStateInvalido(out var json)) return json;
        if (!ModelState.IsValid) return Json(new { cssClass = "warning", mensagem = _mensagens.Value.Erro });
        if(!model.IsSuporte && string.IsNullOrWhiteSpace(model.Perfil)) return Json(new { cssClass = "warning", mensagem = "Informe o perfil de acesso" });
        if (Request.Form["acao"].ToString() == "update")
        {
            if (!string.IsNullOrWhiteSpace(model.Senha))
            {
                var segSenha = PasswordHash.SenhaForte(model.Senha);
                if(!segSenha) return Json(new { cssClass = "warning", mensagem = "Senha fraca, informe uma senha com pelo menos 8 caracteres, com letras maiúsculas, minúsculas, números e caracteres especiais" });
                model.Senha = PasswordHash.CreateHash(model.Senha);
            }
            else
            {
                var senhaAtual = _dataRepository.FindObject(a => a.Id == model.Id, a => a.Senha!).ToString();
                model.Senha = senhaAtual;
            }
            model.DataAlteracao = DateTime.Now;
            model.UsuarioAlteracao = GetDadosUsuario<string>("NomeUsuario");
            if (!_dataRepository.Update(model))
                return Json(new { cssClass = "warning", mensagem = _mensagens.Value.ErroSalvar });
        }
        else
        {
            var segSenha = PasswordHash.SenhaForte(model.Senha);
            if(!segSenha) return Json(new { cssClass = "warning", mensagem = "Senha fraca, informe uma senha com pelo menos 8 caracteres, com letras maiúsculas, minúsculas, números e caracteres especiais" });
            model.UsuarioCadastro = GetDadosUsuario<string>("NomeUsuario");
            if (!string.IsNullOrWhiteSpace(model.Senha)) model.Senha = PasswordHash.CreateHash(model.Senha);
            if (!_dataRepository.Add(model))
                return Json(new { cssClass = "warning", mensagem = _mensagens.Value.ErroSalvar });

            url = $"/{NomeController}/Details/?dados={model.Id}{Request.Form["rediret"]}";
        }

        return Json(new
        {
            janela = Request.Form["janela"].ToString(),
            cssClass = "success",
            mensagem = _mensagens.Value.Sucesso,
            url
        });

    }

    [RequiredPermission]
    public ViewResult Details(string dados)
    {
        var model = _dataRepository.Find(a => a.Id == int.Parse(dados));
        return View(model);
    }

    [RequiredPermission]
    public virtual ActionResult Lista()
    {
        return View();
    }

    [RequiredPermission]
    public virtual JsonResult List(DataTablesParam param)
    {
        var filter = new FilterExpression<Pessoa>().Start(a => true);
        Filtro = filter.ResultExpression;
        Selector = a => new object[] { a.Nome!, a.Email!, a.Cpf!, a.Celular!, a.Perfil, a.Status!, a.Id };
        ColumnNames = new[] { "Nome", "Email", "Cpf", "Celular", "Perfil", "Status", "Id" };
        DataTypes = new[]
            { DataType.tString, DataType.tString, DataType.tString, DataType.tString, DataType.tString,  DataType.tString, DataType.tInt };

        var lista = _dataRepository.GetAll(Filtro, Selector, param, out var totalRecords,
            out var totalRecordsDisplay,
            ColumnNames, DataTypes, Includes);
        var ret = new
        {
            iTotalRecords = totalRecords,
            iTotalDisplayRecords = totalRecordsDisplay,
            param.sEcho,
            aaData = lista
        };
        return Json(ret);
    }

    [AllPermission]
    public ViewResult Perfil()
    {
        var id = GetDadosUsuario<int>("IdUsuario");
        var data = _dataRepository.Find(a => a.Id == id);
        ViewData["tipo"] = "update";
        return View(data);
    }

    [HttpPost]
    [AllPermission]
    public JsonResult Perfil(Pessoa model)
    {
        if (!ModelState.IsValid) return Json(new { cssClass = "warning", mensagem = _mensagens.Value.Erro });
        if (!string.IsNullOrWhiteSpace(model.Senha))
        {
            var segSenha = PasswordHash.SenhaForte(model.Senha);
            if(!segSenha) return Json(new { cssClass = "warning", mensagem = "Senha fraca, informe uma senha com pelo menos 8 caracteres, com letras maiúsculas, minúsculas, números e caracteres especiais" });
            model.Senha = PasswordHash.CreateHash(model.Senha);
        }
        else
        {
            var senhaAtual = _dataRepository.FindObject(a => a.Id == model.Id, a => a.Senha!).ToString();
            model.Senha = senhaAtual;
        }
        if (_dataRepository.Update(model))
            return Json(new
            {
                janela = "pai",
                cssClass = "success",
                mensagem = "Atualizado com sucesso, recarregando!",
                url = "recarregar"
            });

        return Json(new { cssClass = "warning", mensagem = _mensagens.Value.Erro });
    }

    [RequiredPermission]
    public JsonResult Del(int id)
    {
        var model = _dataRepository.Find(a => a.Id == id);
        if (Equals(model, null)) return Json(new { cssClass = "warning", mensagem = _mensagens.Value.NaoEncontrado });

        return Json(_dataRepository.Remove(model)
            ? new { cssClass = "success", mensagem = _mensagens.Value.SucessoDelete }
            : new { cssClass = "warning", mensagem = _mensagens.Value.Erro });
    }

    [HttpPost]
    [AllPermission]
    public JsonResult Senha([FromForm(Name = "senhaAtual")] string senhaAtual,
        [FromForm(Name = "novaSenha")] string novaSenha, [FromForm(Name = "novaSenhaCopia")] string novaSenhaCopia)
    {
        var id = HttpContext.User.Claims.FirstOrDefault(f => f.Type == "IdUsuario");
        var model = _dataRepository.Find(a => id != null && a.Id == int.Parse(id.Value));
        if (!PasswordHash.ValidatePassword(senhaAtual, model.Senha))
            return Json(new { cssClass = "warning", mensagem = "Senha atual inválida" });
        if (novaSenha != novaSenhaCopia)
            return Json(new { cssClass = "warning", mensagem = "As duas novas senhas devem ser iguais" });

        model.Senha = PasswordHash.CreateHash(novaSenha);
        if (_dataRepository.Update(model))
            return Json(new
            {
                janela = Request.Form["janela"].ToString(),
                cssClass = "success",
                mensagem = "Senha alterada com sucesso",
                url = "",
                funcao = "CloseModalPai"
            });

        return Json(new { cssClass = "warning", mensagem = _mensagens.Value.Erro });
    }

    [RequiredPermission]
    public ViewResult AlterarFoto()
    {
        var id = HttpContext.User.Claims.FirstOrDefault(f => f.Type == "IdUsuario");
        var data = _dataRepository.Find(a => id != null && a.Id == int.Parse(id.Value));
        ViewData["janela"] = "filha";
        return View(data);
    }

    [RequiredPermission]
    [HttpPost]
    public ViewResult AlterarFoto([FromForm(Name = "imgX")] string imgX, [FromForm(Name = "imgY")] string imgY,
        [FromForm(Name = "imgW")] string imgW, [FromForm(Name = "imgH")] string imgH, IFormFile file)
    {
        var id = HttpContext.User.Claims.FirstOrDefault(f => f.Type == "IdUsuario");
        var model = _dataRepository.Find(a => id != null && a.Id == int.Parse(id.Value));

        var x = Convert.ToInt32(Math.Round(Convert.ToDouble(imgX.Replace(".", ",")), 0));
        var y = Convert.ToInt32(Math.Round(Convert.ToDouble(imgY.Replace(".", ",")), 0));
        var w = Convert.ToInt32(Math.Round(Convert.ToDouble(imgW.Replace(".", ",")), 0));
        var h = Convert.ToInt32(Math.Round(Convert.ToDouble(imgH.Replace(".", ",")), 0));
        var nome = $"{Guid.NewGuid().ToString().Replace("-", "")}.{Path.GetExtension(file.FileName)}";
        using var image = Image.Load(file.OpenReadStream());
        image.Mutate(it => it.Crop(new Rectangle(x, y, w, h)));
        var uploadsRootFolder = Path.Combine(_hostingEnvironment.WebRootPath, $"images/{NomeController!.ToLower()}");
        if (!Directory.Exists(uploadsRootFolder)) Directory.CreateDirectory(uploadsRootFolder);
        image.Save(Path.Combine(uploadsRootFolder, nome), new WebpEncoder());
        if (!string.IsNullOrWhiteSpace(model.Foto))
            System.IO.File.Delete(Path.Combine(_hostingEnvironment.WebRootPath,
                $"images/{NomeController.ToLower()}/{model.Foto}"));
        model.Foto = nome;
        _dataRepository.Update(model);
        ViewBag.Retorno = new[] { "success", "Foto atualizada com sucesso!" };
        model = _dataRepository.Find(a => id != null && a.Id == int.Parse(id.Value));
        ViewData["janela"] = "filha";
        return View(model);
    }

    #endregion
}