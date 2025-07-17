#region

using System.Data;
using Azure.Core;
using Dapper;
using Domain;
using iDevCL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Repository;
using SixLabors.ImageSharp;
using WebApp.Filter;
using WebApp.Models;

#endregion

namespace WebApp.Controllers;

public class TicketController : ApplicationController<Ticket>
{
    private static string userId;
    private static string secret;
    private readonly string _certificado;
    private readonly HttpClient _client;
    private readonly string _clienteId;
    private readonly string _clienteSecret;
    private readonly IOptions<Mensagens> _mensagens;
    private readonly bool _sandBox;
    private readonly string _url;
    private IGenericDataRepository<Ticket> _dataRepository;

    public TicketController(IHttpContextAccessor httpContextAccessor, IConfiguration? configuration,
        IGenericDataRepository<Ticket> dataRepository, IOptions<Mensagens> mensagens) : base(httpContextAccessor,
        configuration)
    {
        if (httpContextAccessor.HttpContext != null)
            NomeController = httpContextAccessor.HttpContext.Request.RouteValues["controller"]?.ToString();
        _dataRepository = dataRepository;

        _mensagens = mensagens;
        _sandBox = bool.Parse(_configuration.GetSection("Efi.Sandbox").Value);
        _url = _sandBox ? "https://sandbox.asaas.com/api" : "https://api.asaas.com";

        _client = new HttpClient();
        _client.DefaultRequestHeaders.Accept.Clear();
        _client.DefaultRequestHeaders.Add("access_token", _configuration.GetSection("Asaas.ApiKey").Value);
        _client.BaseAddress = new Uri(_url);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            if (_dataRepository != null)
            {
                _dataRepository.Dispose();
                _dataRepository = null;
            }

        base.Dispose(disposing);
    }


    #region Admin

    [RequiredPermission]
    public JsonResult ValidaCad(string dados, int id)
    {
        if (string.IsNullOrWhiteSpace(dados)) return Json(new { cssClass = "success", mensagem = "" });
        if (!IdevString.ValidaCpf(dados)) return Json(new { cssClass = "warning", mensagem = "CPF inválido" });
        var filtro = new FilterExpression<Ticket>().Start(a =>
            IdevString.SubstituiAcentos(a.Cpf).ToLower() == IdevString.SubstituiAcentos(dados).ToLower());
        filtro = filtro.And(a => a.Id != id, id > 0);
        var item = _dataRepository.Find(filtro.ResultExpression);
        return Json(!Equals(item, null)
            ? new { cssClass = "warning", mensagem = _mensagens.Value.ItemCadastrado }
            : new { cssClass = "success", mensagem = "" });
    }

    [RequiredPermission("operador", "financeiro")]
    public virtual ActionResult Lista(string status)
    {
        return View();
    }

    [RequiredPermission("operador", "financeiro")]
    public virtual ActionResult Concluidos(string status)
    {
        return View();
    }

    [RequiredPermission("financeiro")]
    public virtual JsonResult ListGeral(DataTablesParam param)
    {
        var admin = GetDadosUsuario<bool>("IsSuporte");
        var idPessoa = GetDadosUsuario<int>("IdUsuario");

        var filter = new FilterExpression<Ticket>().Start(a => a.StatusPagamento != "Pago");
        if (!admin) filter = filter.And(a => a.IdPessoa == idPessoa || !a.IdPessoa.HasValue);
        if (param.sSearch.Contains("operador"))
        {
            param.sSearch = "";
            filter = filter.And(a => !a.IdPessoa.HasValue);
        }

        Filtro = filter.ResultExpression;
        Selector = a => new object[]
        {
            a.Id, a.Cpf!, $"{a.Nome!}<br>{a.Celular!}", $"{a.DataCadastro:d}<br>{a.DataCadastro:t}",
            $"{a.DataAtribuicao:d}<br>{a.DataAtribuicao:t}", $"{a.DataFinalizado:d}<br>{a.DataFinalizado:t}", a.Status!,
            a.Id
        };
        ColumnNames = new[] { "Id", "Cpf", "Nome", "DataCadastro", "DataAtribuicao", "DataFinalizado", "Status", "Id" };
        DataTypes = new[]
        {
            DataType.tInt, DataType.tString, DataType.tString, DataType.tDate, DataType.tNullDate, DataType.tNullDate,
            DataType.tString, DataType.tInt
        };
        Includes = new[] { "Pessoa" };
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

    [RequiredPermission("financeiro")]
    public virtual ActionResult Geral()
    {
        return View();
    }

    [RequiredPermission]
    public virtual ActionResult Relatorio(string dataInicial, string dataFinal, int? idPessoa, string status)
    {
        ViewBag.dataInicial =
            string.IsNullOrWhiteSpace(dataInicial) ? Base.GetDataUm().ToShortDateString() : dataInicial;
        ViewBag.dataFinal = string.IsNullOrWhiteSpace(dataFinal)
            ? Base.GetDataUltimoDia().ToShortDateString()
            : dataFinal;
        ViewBag.idPessoa = idPessoa;
        ViewBag.status = status;
        IList<Ticket> model = new List<Ticket>();
        var filtro = new FilterExpression<Ticket>().Start(a => a.IdPessoa.HasValue && a.Status == "Concluído");
        if (!string.IsNullOrWhiteSpace(dataInicial) && !string.IsNullOrWhiteSpace(dataFinal))
        {
            filtro = filtro.And(a =>
                a.DataCadastro.Date >= DateTime.Parse(dataInicial).Date &&
                a.DataCadastro.Date <= DateTime.Parse(dataFinal).Date);
            filtro = filtro.And(a => a.IdPessoa == idPessoa, idPessoa.HasValue);
            if (!string.IsNullOrWhiteSpace(status))
            {
                if (status == "Pago") filtro = filtro.And(a => !string.IsNullOrWhiteSpace(a.StatusPagamentoOperador));
                else filtro.And(a => string.IsNullOrWhiteSpace(a.StatusPagamentoOperador));
            }

            model = _dataRepository.GetAllItens(filtro.ResultExpression, a => a.Pessoa);
        }

        return View(model);
    }

    [RequiredPermission]
    public virtual ActionResult Estatisticas(string data)
    {
        using var connection = new SqlConnection(_configuration.GetConnectionString("Conn"));
        connection.Open();
        
        data = string.IsNullOrWhiteSpace(data) ? DateTime.Today.ToShortDateString() : data;
        var dataParsed = DateTime.Parse(data);
        
        string queryPessoas = @"SELECT * FROM Pessoa";
        string queryModel = @"SELECT * FROM Ticket WHERE StatusPagamento = 'Pago'";
        string queryModelDia = @"SELECT * FROM Ticket WHERE CAST(DataCadastro AS DATE) = @DataCadastro AND Dominio IS NOT NULL AND Dominio <> ''";
        string queryNaoPagos = @"SELECT COUNT(Id) FROM Ticket WHERE StatusPagamento != 'Pago'";
        
        var pessoas = connection.Query<Pessoa>(queryPessoas).ToList();
        var model = connection.Query<Ticket>(queryModel).ToList();
        var modelDia = connection.Query<Ticket>(queryModelDia, new { DataCadastro = dataParsed }).ToList();
        var naoPagos = connection.ExecuteScalar<int>(queryNaoPagos);
        
        ViewBag.NaoPagos = naoPagos;
        ViewBag.data = data;
        
        return View(new Tuple<IList<Ticket>, IList<Ticket>, IList<Pessoa>>(model, modelDia, pessoas));
    }
    //public virtual ActionResult Estatisticas(string data)
    //{
    //    data = string.IsNullOrWhiteSpace(data) ? DateTime.Today.ToShortDateString() : data;
    //    IList<Ticket> model = new List<Ticket>();
    //    IList<Ticket> modelDia  = new List<Ticket>();
    //    var filtroDia = new FilterExpression<Ticket>().Start(a => a.DataCadastro.Date == DateTime.Parse(data) && !string.IsNullOrWhiteSpace(a.Dominio));
    //    var filtro = new FilterExpression<Ticket>().Start(a => a.StatusPagamento == "Pago");
    //    model = _dataRepository.GetAllItens(filtro.ResultExpression, a => a.Pessoa);
    //    modelDia = _dataRepository.GetAllItens(filtroDia.ResultExpression);
    //    ViewBag.NaoPagos = _dataRepository.GetAllObject(a => a.StatusPagamento != "Pago", a => a.Id).Count;
    //    ViewBag.data = data;
    //    return View(new Tuple< IList<Ticket> ,  IList<Ticket>>(model, modelDia));
    //}

    [RequiredPermission]
    public virtual JsonResult Baixar(string dataInicial, string dataFinal, int? idPessoa)
    {
        if (!string.IsNullOrWhiteSpace(dataInicial) && !string.IsNullOrWhiteSpace(dataFinal))
        {
            var filtro = new FilterExpression<Ticket>().Start(a =>
                a.IdPessoa.HasValue && string.IsNullOrWhiteSpace(a.StatusPagamentoOperador) &&
                a.Status == "Concluído" &&
                a.DataCadastro.Date >= DateTime.Parse(dataInicial).Date &&
                a.DataCadastro.Date <= DateTime.Parse(dataFinal).Date);
            filtro = filtro.And(a => a.IdPessoa == idPessoa, idPessoa.HasValue);
            var model = _dataRepository.GetAllItens(filtro.ResultExpression);
            foreach (var item in model)
            {
                item.StatusPagamentoOperador = DateTime.Today.ToString("dd/MM/yyyy");
                _dataRepository.Update(item);
            }
        }

        return Json(new { cssClass = "success" });
    }

    [RequiredPermission("operador", "financeiro")]
    public virtual JsonResult List(DataTablesParam param)
    {
        var admin = GetDadosUsuario<bool>("IsSuporte");
        var idPessoa = GetDadosUsuario<int>("IdUsuario");
        var operador = Base.GetDadosUsuario<string>(User, "operador");

        var filter = new FilterExpression<Ticket>().Start(a => a.StatusPagamento == "Pago" && a.Status != "Concluído");
        if (!admin && operador == "liberado") filter = filter.And(a => a.IdPessoa == idPessoa || !a.IdPessoa.HasValue);
        if (param.sSearch.Contains("operador"))
        {
            param.sSearch = "";
            filter = filter.And(a => !a.IdPessoa.HasValue);
        }

        Filtro = filter.ResultExpression;
        Selector = a => new object[]
        {
            a.Id, a.Cpf!, $"{a.Nome!}<br>{a.Celular!}", $"{a.DataCadastro:d}<br>{a.DataCadastro:t}",
            $"{a.DataAtribuicao:d}<br>{a.DataAtribuicao:t}", $"{a.DataFinalizado:d}<br>{a.DataFinalizado:t}", a.Status!,
            !Equals(a.Pessoa, null) ? a.Pessoa!.Nome! : "", a.IdPessoa, a.Id
        };
        ColumnNames = new[]
        {
            "Id", "Cpf", "Nome", "DataCadastro", "DataAtribuicao", "DataFinalizado", "Status", "Pessoa.Nome",
            "IdPessoa", "Id"
        };
        DataTypes = new[]
        {
            DataType.tInt, DataType.tString, DataType.tString, DataType.tDate, DataType.tNullDate, DataType.tNullDate,
            DataType.tString, DataType.tString, DataType.tNullInt, DataType.tInt
        };
        Includes = new[] { "Pessoa" };
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
    public virtual JsonResult ListConcluidos(DataTablesParam param)
    {
        var admin = GetDadosUsuario<bool>("IsSuporte");
        var idPessoa = GetDadosUsuario<int>("IdUsuario");
        var operador = GetDadosUsuario<string>("operador");

        var filter = new FilterExpression<Ticket>().Start(a => a.Status == "Concluído");
        if (!admin && operador is "liberado") filter = filter.And(a => a.IdPessoa == idPessoa || !a.IdPessoa.HasValue);
        if (param.sSearch.Contains("operador"))
        {
            param.sSearch = "";
            filter = filter.And(a => !a.IdPessoa.HasValue);
        }

        Filtro = filter.ResultExpression;
        Selector = a => new object[]
        {
            a.Id, a.Cpf!, $"{a.Nome!}<br>{a.Celular!}", $"{a.DataCadastro:d}<br>{a.DataCadastro:t}",
            $"{a.DataAtribuicao:d}<br>{a.DataAtribuicao:t}", $"{a.DataFinalizado:d}<br>{a.DataFinalizado:t}", a.Status!, a.NomePessoa!, a.IdPessoa!, a.Id
        };
        ColumnNames = new[] { "Id", "Cpf", "Nome", "DataCadastro", "DataAtribuicao", "DataFinalizado", "Status", "NomePessoa", "IdPessoa", "Id" };
        DataTypes = new[] { DataType.tInt, DataType.tString, DataType.tString, DataType.tDate, DataType.tNullDate, DataType.tNullDate, DataType.tString, DataType.tString, DataType.tNullInt, DataType.tInt };
        Includes = new[] { "Pessoa" };
        var lista = _dataRepository.GetAll(Filtro, Selector, param, out var totalRecords, out var totalRecordsDisplay, ColumnNames, DataTypes, Includes);
        var ret = new
        {
            iTotalRecords = totalRecords,
            iTotalDisplayRecords = totalRecordsDisplay,
            param.sEcho,
            aaData = lista
        };
        return Json(ret);
    }

    [RequiredPermission]
    public IActionResult Add(string id, string acao)
    {
        var model = new Ticket();
        ViewData["janela"] = "filha";
        ViewData["acao"] = acao;
        if (acao == "update")
            model = _dataRepository.Find(a => a.Id == Convert.ToInt32(id));
        return View(model);
    }

    [HttpPost]
    [RequiredPermission]
    public JsonResult Salvar(Ticket model)
    {
        var url = "";
        if (ModelStateInvalido(out var json)) return json;
        if (!ModelState.IsValid) return Json(new { cssClass = "warning", mensagem = _mensagens.Value.ErroSalvar });
        if (ValidaModel(model, out var salvar)) return salvar;
        if (Request.Form["acao"].ToString() == "update")
        {
            model.DataAlteracao = DateTime.Now;
            model.UsuarioAlteracao = GetDadosUsuario<string>("NomeUsuario");
            if (!model.IdPessoa.HasValue) model.DataAtribuicao = null;
            if (!_dataRepository.Update(model))
                return Json(new { cssClass = "warning", mensagem = _mensagens.Value.ErroSalvar });
        }
        else
        {
            model.DataCadastro = DateTime.Now;
            model.UsuarioCadastro = GetDadosUsuario<string>("NomeUsuario");
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

    [RequiredPermission("operador", "financeiro")]
    public ActionResult Details(int dados)
    {
        var model = _dataRepository.Find(a => a.Id == dados);
        var perfil = GetDadosUsuario<string>("financeiro");
        if (!GetDadosUsuario<bool>("IsSuporte") && perfil != "liberado" && !model.IdPessoa.HasValue)
            return RedirectToAction("Nao", "Home");
        return View(model);
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

    [AllPermission]
    public JsonResult Atribuir(int id)
    {
        var model = _dataRepository.Find(a => a.Id == id);
        if (Equals(model, null)) return Json(new { cssClass = "warning", mensagem = _mensagens.Value.NaoEncontrado });
        if (model.Status != "Em aberto")
            return Json(new { cssClass = "warning", mensagem = "Este ticket não está mais disponível!" });
        model.IdPessoa = GetDadosUsuario<int>("IdUsuario");
        model.Status = "Em atendimento";
        model.DataAtribuicao = DateTime.Now;
        var update = _dataRepository.Update(model);
        return Json(update
            ? new { cssClass = "success", mensagem = "Atribuido com sucesso" }
            : new { cssClass = "warning", mensagem = "Ocorreu um erro ao atribuir, tente novamente mais tarde!" });
    }

    [RequiredPermission]
    public JsonResult Disponibilizar(int id)
    {
        var model = _dataRepository.Find(a => a.Id == id);
        if (Equals(model, null)) return Json(new { cssClass = "warning", mensagem = _mensagens.Value.NaoEncontrado });

        model.StatusPagamento = "Pago";
        var update = _dataRepository.Update(model);
        return Json(update
            ? new { cssClass = "success", mensagem = "Realizado com sucesso" }
            : new { cssClass = "warning", mensagem = "Ocorreu um erro, tente novamente mais tarde!" });
    }

    [RequiredPermission]
    public async Task<JsonResult> ConsultarPagos()
    {
        var model = _dataRepository.GetAll(a => a.StatusPagamento != "Pago");
        var contador = 0;
        foreach (var m in model)
        {
            var rsposeQrCode = await _client.GetAsync($"{_url}/v3/payments/{m.TidTransacao}/status");
            var res = await rsposeQrCode.Content.ReadAsStringAsync();
            var jsonResponseQrCode = JObject.Parse(res);
            var status = jsonResponseQrCode["status"].ToString();
            if (status == "RECEIVED")
            {
                m.StatusPagamento = "Pago";
                contador++;
                _dataRepository.Update(m);
            }

            await Task.Delay(1000);
        }

        return Json(new { cssClass = "success", mensagem = $"{contador} atualizado(s) com sucesso" });
    }

    #endregion
}