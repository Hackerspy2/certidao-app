using Repository.Util;
using System.Text;
namespace WebApp.Models;

public class HtmlHelper
{
    private static IConfiguration _configuration;
    public HtmlHelper()
    {

    }

    public static string MontaHtml(string titulo, string controller, string action, string modal,
        string headerColunas, string tituloBtn = null)
    {
        var sb = new StringBuilder();
        sb.Append("<div class='row page-titles'>");
        sb.Append("<div class='col-md-5'>");

        if (!string.IsNullOrWhiteSpace(action))
            //if (Base.VerificaPermissao($"{controller}{action}"))
            if (!string.IsNullOrWhiteSpace(tituloBtn))
            {
                sb.AppendFormat(
                    action.Contains("?")
                        ? "<a href='/{0}/{1}&acao=add' class='{2} waves-effect waves-light btn-inverse btn btn-sm pull-right m-l-10' data-toggle='tooltip' title='{3}' data-placement='right'>{4}</a>"
                        : "<a href='/{0}/{1}/?acao=add' class='{2} waves-effect waves-light btn-inverse btn btn-sm pull-right m-l-10' data-toggle='tooltip' title='{3}' data-placement='right'>{4}</a>",
                    controller, action, modal, !string.IsNullOrWhiteSpace(tituloBtn) ? tituloBtn : "Cadastrar",
                    tituloBtn);
            }

        sb.Append("</div>");
        sb.Append("<div class='col-md-7 align-self-center'>");
        sb.Append("<ol class='breadcrumb'>");
        sb.Append("<li class='breadcrumb-item'><a href='/'>Dashboard</a></li>");
        sb.AppendFormat("<li class='breadcrumb-item active'>{0}</li>", titulo);
        sb.Append("</ol></div><div></div>");
        sb.Append(
            "</div><div class='animated fadeInUp'><div class='row'><div class='col-12'><div class='card'><div class='card-body'><div class='table-responsive'>");
        sb.Append(
            "<table id='dynamicTableDesc' class='display table table-hover table-striped table-bordered' cellspacing='0' width='100%'>");
        sb.Append("<thead><tr>");
        foreach (var c in headerColunas.Split('|')) sb.AppendFormat("<th>{0}</th>", c);
        sb.Append("<th style='width: 15px;'></th>");
        sb.Append("</tr></thead><tbody></tbody></table></div></div></div></div></div></div>");
        return sb.ToString();
    }

    public static string MontaHtmlNoAction(string titulo, string controller, string action, string modal,
        string headerColunas, string tituloBtn = null)
    {
        var sb = new StringBuilder();
        sb.Append("<div class='row page-titles'>");
        sb.Append("<div class='col-md-5'>");

        if (!string.IsNullOrWhiteSpace(action))
            //if (Base.VerificaPermissao($"{controller}{action}"))
            if (!string.IsNullOrWhiteSpace(tituloBtn))
                sb.AppendFormat(
                    "<a href='/{0}/{1}' class='{2} waves-effect waves-light btn-inverse btn btn-sm pull-right m-l-10' data-toggle='tooltip' title='{3}' data-placement='right'>{4}</a>",
                    controller, action, modal, !string.IsNullOrWhiteSpace(tituloBtn) ? tituloBtn : "Cadastrar",
                    tituloBtn);
        sb.Append("</div>");
        sb.Append("<div class='col-md-7 align-self-center'>");
        sb.Append("<ol class='breadcrumb'>");
        sb.Append("<li class='breadcrumb-item'><a href='/'>Dashboard</a></li>");
        sb.AppendFormat("<li class='breadcrumb-item active'>{0}</li>", titulo);
        sb.Append("</ol></div><div></div>");
        sb.Append(
            "</div><div class='animated fadeInUp'><div class='row'><div class='col-12'><div class='card'><div class='card-body'><div class='table-responsive'>");
        sb.Append(
            "<table id='dynamicTableDesc' class='display table table-hover table-striped table-bordered'>");
        sb.Append("<thead><tr>");
        foreach (var c in headerColunas.Split('|')) sb.AppendFormat("<th>{0}</th>", c);
        sb.Append("</tr></thead><tbody></tbody></table></div></div></div></div></div></div>");
        return sb.ToString();
    }

    public static string MontaTabelaHtml(string idTabela, string url, string rotulo, string classAdd,
        string headerColunas, string classPaiModal, string inputUpload = null)
    {
        var sb = new StringBuilder();
        if (!string.IsNullOrWhiteSpace(inputUpload))
        {
            sb.AppendFormat("<p>");
            sb.AppendFormat(
                "<label for='upload' class='btn btn-xs btn-info'>{0}</label><input type='file' multiple id='upload' name='upload' style='display: none' />",
                rotulo);
            sb.Append("</p>");
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(rotulo))
            {
                sb.AppendFormat("<p class='{0}'>", classAdd);
                sb.AppendFormat(
                    "<a href='{0}' class='cadastrar{1} btn btn-xs btn-inverse' title='{2}' data-original-title='{2}'>{2}</a>",
                    url, classPaiModal, rotulo);
                sb.Append("</p>");
            }
        }

        sb.Append("<div class='table-responsive'>");
        sb.AppendFormat(
            "<table id='{0}' class='display table table-hover table-striped table-bordered'>",
            idTabela);
        sb.Append("<thead><tr>");
        foreach (var c in headerColunas.Split('|')) sb.AppendFormat("<th>{0}</th>", c);
        sb.Append("<th style='width: 15px;'></th></tr></thead><tbody></tbody></table></div>");
        return sb.ToString();
    }

    public static string MontaTabelaHtmlNoAction(string idTabela, string url, string rotulo, string classAdd,
        string headerColunas, string classPaiModal)
    {
        var sb = new StringBuilder();
        if (!string.IsNullOrWhiteSpace(rotulo))
        {
            sb.AppendFormat("<p class='{0}'>", classAdd);
            sb.AppendFormat(
                "<a href='{0}' class='cadastrar{1} btn btn-xs btn-info' title='{2}' data-original-title='{2}'>{2}</a>",
                url, classPaiModal, rotulo);
            sb.Append("</p>");
        }

        sb.Append("<div class='table-responsive'>");
        sb.AppendFormat(
            "<table id='{0}' class='display table table-hover table-striped table-bordered' cellspacing='0' width='100%'>",
            idTabela);
        sb.Append("<thead><tr>");
        foreach (var c in headerColunas.Split('|')) sb.AppendFormat("<th>{0}</th>", c);
        sb.Append("</tr></thead><tbody></tbody></table></div>");
        return sb.ToString();
    }

    public static string MontaDiv(string coluns, string label, object? valor)
    {
        return
            $"<div class='col-lg-{coluns} col-sm-{coluns} col-md-{coluns} col-12'><p><span class='rotulo'>{label}: </span><br /> <strong>{valor}</strong></p></div>";
    }


    public static string MontaHeader(string titulo, string controller, string action, string modal,
        string tituloBtn)
    {
        var sb = new StringBuilder();
        sb.Append("<div class='row page-titles'>");
        sb.Append("<div class='col-md-5'>");
        if (!string.IsNullOrWhiteSpace(action))
            //if (Base.VerificaPermissao($"{controller}{action}"))
            if (!string.IsNullOrWhiteSpace(tituloBtn))
                sb.AppendFormat(
                    "<a href='/{0}/{1}' class='{2} waves-effect waves-light btn-inverse btn btn-sm pull-right m-l-10' data-toggle='tooltip' title='{3}' data-placement='right'>{3}</a>",
                    controller, action, modal, !string.IsNullOrWhiteSpace(tituloBtn) ? tituloBtn : "Cadastrar");
        sb.Append(
            "</div><div class='col-md-7 align-self-center'><ol class='breadcrumb'><li class='breadcrumb-item'><a href='/'> Dashboard </a></li>");
        sb.AppendFormat("<li class='breadcrumb-item active'>{0}</li>", titulo);
        sb.Append("</ol></div><div></div></div>");
        return sb.ToString();
    }

    public static string MontaInput(InputTypes tipo, string nome, object? valor, string label, string requerido,
        string coluns, string opcoes = null, string classe = null, string soLeitura = null, string help = null,
        string max = null)
    {
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        _configuration = builder.Build();
        var sb = new StringBuilder();
        sb.AppendFormat("<div class='col-lg-{0} col-sm-{0} col-md-{0} col-12' id='div{1}' {2}>", coluns,
            nome.Replace('.', '_'), tipo == InputTypes.File ? "style='margin-bottom: 15px'" : "");
        sb.Append("<div class='form-group'>");
        var somenteLeitura = soLeitura == "readonly" ? "readonly=readonly" : "";
        var maxLength = !string.IsNullOrWhiteSpace(max) ? $"maxlength={max}" : "maxlength=300";
        var classeInt = classe ?? "";
        switch (tipo)
        {
            case InputTypes.Typeahead:
                sb.Append(
                    $"<div id='ty{nome}'>");
                sb.AppendFormat(
                    "<input type='text' name='{0}' id='{1}' value='{2}' autocomplete='off' class='form-control typeahead {3}' {4} /> ",
                    nome, nome.Replace('.', '_'), valor, classeInt, somenteLeitura);
                sb.Append("</div>");
                break;
            case InputTypes.Checkbox:
                var checado = valor.ToString() == "True" ? "checked=checked" : "";
                sb.AppendFormat(
                    "<div class='form-check' style='padding-left: 0'><input type='checkbox' name='{0}' id='{1}' value='{2}' class='form-check-input {3}' {4} {6} /><label for='{1}' style='margin: 0 !important;'></label> {5}</div>",
                    nome,
                    nome.Replace('.', '_'), valor, classeInt, somenteLeitura, label, checado);
                break;
            case InputTypes.Radio:
                sb.AppendFormat(
                    "<input type='radio' name='{0}' id='{1}' value='{2}' class='filled-in {3}' {4} />", nome,
                    nome.Replace('.', '_'), valor, classeInt, somenteLeitura);
                break;
            case InputTypes.Text:
                sb.AppendFormat(
                    "<input type='text' name='{0}' id='{1}' value='{2}' class='form-control {3}' {4} {5} />",
                    nome,
                    nome.Replace('.', '_'), valor, classeInt, somenteLeitura, maxLength);
                break;
            case InputTypes.Password:
                sb.AppendFormat(
                    "<input type='password' name='{0}' id='{1}' value='{2}' class='form-control {3}' {4} />", nome,
                    nome.Replace('.', '_'), valor, classeInt, somenteLeitura);
                break;
            case InputTypes.Email:
                sb.AppendFormat(
                    "<input type='email' name='{0}' id='{1}' value='{2}' class='form-control {3}' {4} />", nome,
                    nome.Replace('.', '_'), valor, classeInt, somenteLeitura);
                break;
            case InputTypes.Textarea:
                sb.AppendFormat(
                    "<textarea name='{0}' id='{1}' rows='5' cols='7' class='form-control {2}' {3}>{4}</textarea>",
                    nome, nome.Replace('.', '_'), classeInt, somenteLeitura, valor);
                break;
            case InputTypes.File:
                sb.AppendFormat(
                    "<label for='{0}' class='btn btn-warning btn-xs' style='color: #fff'><i class='fa fa-plus-circle'></i> {1}</label>",
                    nome, string.IsNullOrWhiteSpace(label) ? "Selecione um arquivo" : label);
                if (!string.IsNullOrWhiteSpace(classeInt))
                    sb.AppendFormat(
                        "<input type = 'file' class='{0}' name='{1}' id='{2}' value='{3}' style='display: none' />",
                        classeInt, nome, nome.Replace('.', '_'), valor);
                else
                    sb.AppendFormat(
                        "<input type = 'file' class='addFile' name='{0}' id='{1}' value='{2}' style='display: none' />",
                        nome, nome.Replace('.', '_'), valor);

                break;
            case InputTypes.Select:
                if (string.IsNullOrWhiteSpace(opcoes))
                {
                    sb.AppendFormat(
                        "<select name='{0}' id='{1}' value='{2}' class='form-control {3}' aria-invalid='false' {4} style='height: auto;'><option value=''></ option ></select>",
                        nome, nome.Replace('.', '_'), valor, classeInt, somenteLeitura);
                }
                else
                {
                    //var niveis = ConfigurationManager.AppSettings[opcoes].Split('|').ToList();
                    var niveis = _configuration.GetSection(opcoes).Value.Split('|').OrderBy(a => a).ToList();
                    sb.AppendFormat(
                        "<select name='{0}' id='{1}' value='{2}' class='form-control {3}' aria-invalid='false' {4} style='height: auto;'>",
                        nome, nome.Replace('.', '_'), valor, classeInt, somenteLeitura);
                    sb.Append("<option value=''></option>");
                    if (niveis.Any(a => a.Contains(';')))
                        foreach (var item in niveis.OrderBy(a => a))
                        {
                            var it = item.Split(';');
                            var select = valor == null ? "" : it[0] == valor.ToString() ? "selected" : "";
                            sb.AppendFormat("<option value='{0}' {1}>{2}</option>", it[0], select, it[1]);
                        }
                    else
                        foreach (var item in niveis)
                        {
                            var select = valor == null ? "" :
                                item != null && item == valor.ToString() ? "selected" : "";
                            sb.AppendFormat("<option value='{0}' {1}>{0}</option>", item, select);
                        }

                    sb.Append("</select>");
                }

                break;
            case InputTypes.MutipleSelect:
                var dados = _configuration.GetSection(opcoes).Value.Split('|').ToList();
                sb.AppendFormat(
                    "<select name='{0}' id='{1}' value='{2}' class='form-control {3}' aria-invalid='false' {4} style='height: auto; margin-top: 15px' multiple>",
                    nome, nome.Replace('.', '_'), valor, classeInt, somenteLeitura);
                foreach (var item in dados)
                {
                    var select = valor == null ? "" :
                        item != null && valor.ToString().Split('|').Contains(item) ? "selected" : "";
                    sb.AppendFormat("<option value='{0}' {1}>{0}</option>", item, select);
                }

                sb.Append("</select>");
                break;
        }

        if (tipo == InputTypes.MutipleSelect)
        {
            sb.Append("<span class='bar'></span>");
            sb.AppendFormat("<label for='{0}' style='top: -23px;'><span id='label{1}'>{2}</span> {3}:</label>",
                nome.Replace('.', '_'),
                nome, label, requerido);
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(label) && tipo != InputTypes.File)
            {
                sb.Append("<span class='bar'></span>");
                sb.AppendFormat("<label for='{0}' class='inputValidation'><span id='label{1}'>{2}</span> {3}</label>",
                    nome.Replace('.', '_'),
                    nome, (tipo == InputTypes.Checkbox || tipo == InputTypes.Radio ? "" : label + ":"), requerido);
            }
        }


        sb.AppendFormat("<p class='help-block'>{0}</p></div></div>", help);
        return sb.ToString();
    }

    public static string MontaInputNoCollum(InputTypes tipo, string nome, object valor, string label,
        string requerido, string opcoes = null, string classe = null, string soLeitura = null)
    {
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        _configuration = builder.Build();
        var sb = new StringBuilder();
        sb.AppendFormat("<div class='col-12' id='div{0}' style='{1}'>", nome.Replace('.', '_'),
            tipo == InputTypes.File ? "margin-top: -25px;" : "");
        sb.Append("<div class='form-group'>");
        var somenteLeitura = soLeitura == "readonly" ? "readonly=readonly" : "";
        var classeInt = classe ?? "";
        switch (tipo)
        {
            case InputTypes.Typeahead:
                sb.Append(
                    "<div class='typeahead__container'><div class='typeahead__field'><span class='typeahead__query'>");
                sb.AppendFormat(
                    "<input type = 'search' name='@nome' id='{0}' value='{1}' autocomplete='off' class='form-control js-typeahead {2}' {3} /> ",
                    nome.Replace('.', '_'), valor, classeInt, somenteLeitura);
                sb.Append("</span></div></div>");
                break;
            case InputTypes.Checkbox:
                sb.AppendFormat(
                    "<input type = 'checkbox' name='{0}' id='{1}' value='{2}' class='filled-in {3}' {4} />", nome,
                    nome.Replace('.', '_'), valor, classeInt, somenteLeitura);
                break;
            case InputTypes.Text:
                sb.AppendFormat("<input type = 'text' name='{0}' id='{1}' value='{2}' class='filled-in {3}' {4} />",
                    nome, nome.Replace('.', '_'), valor, classeInt, somenteLeitura);
                break;
            case InputTypes.Password:
                sb.AppendFormat(
                    "<input type = 'password' name='{0}' id='{1}' value='{2}' class='filled-in {3}' {4} />", nome,
                    nome.Replace('.', '_'), valor, classeInt, somenteLeitura);
                break;
            case InputTypes.Email:
                sb.AppendFormat(
                    "<input type = 'email' name='{0}' id='{1}' value='{2}' class='filled-in {3}' {4} />", nome,
                    nome.Replace('.', '_'), valor, classeInt, somenteLeitura);
                break;
            case InputTypes.Textarea:
                sb.AppendFormat(
                    "<textarea nname='{0}' id='{1}' rows='5' cols='7'class='filled-in {2}' {3}>{4}</textarea>",
                    nome, nome.Replace('.', '_'), classeInt, somenteLeitura, valor);
                break;
            case InputTypes.File:
                sb.AppendFormat(
                    "<label for='{0}' class='btn btn-warning btn-xs' style='color: #fff'><i class='fa fa-plus-circle'></i> {1})</label>",
                    nome, string.IsNullOrWhiteSpace(label) ? "Selecione um arquivo" : label);
                sb.AppendFormat(
                    "<input type = 'file' class='addFile' name='{0}' id='{1}' value='{2}' style='display: none' />",
                    nome, nome.Replace('.', '_'), valor);
                break;
            case InputTypes.Select:
                if (string.IsNullOrWhiteSpace(opcoes))
                {
                    sb.AppendFormat(
                        "<select name='{0}' id='{1}' value='{2}' class='filled-in {3}' aria-invalid='false' {4}><option value=''></ option ></select>",
                        nome, nome.Replace('.', '_'), valor, classeInt, somenteLeitura);
                }
                else
                {
                    var niveis = _configuration.GetSection(opcoes).Value.Split('|').ToList();
                    sb.AppendFormat(
                        "<select name='{0}' id='{1}' value='{2}' class='filled-in {3}' aria-invalid='false' {4}>",
                        nome, nome.Replace('.', '_'), valor, classeInt, somenteLeitura);
                    sb.Append("<option value = '' ></option>");
                    if (niveis.Any(a => a.Contains(';')))
                        foreach (var item in niveis)
                        {
                            var it = item.Split(';');
                            var select = valor == null ? "" : it[0] == valor.ToString() ? "selected" : "";
                            sb.AppendFormat("<option value='{0}' {1}>{2}</option>", it[0], select, it[1]);
                        }
                    else
                        foreach (var item in niveis)
                        {
                            var select = valor == null ? "" :
                                item != null && item == valor.ToString() ? "selected" : "";
                            sb.AppendFormat("<option value='{0}' {1}>{0}</option>", item, select);
                        }

                    sb.Append("</select>");
                }

                break;
        }

        if (!string.IsNullOrWhiteSpace(label) && tipo != InputTypes.File)
        {
            sb.Append("<span class='bar'></span>");
            sb.AppendFormat("<label for='{0}'><span id='label{1}'>{2}</span> {3}:</label>", nome.Replace('.', '_'),
                nome, label, requerido);
        }

        sb.Append("<p class='help-block'></p></div></div>");
        return sb.ToString();
    }

    public static string MontaInputOnly(InputTypes tipo, string nome, object valor,
        string requerido, string opcoes = null, string classe = null, string soLeitura = null,
        string primeiroItem = null)
    {
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        _configuration = builder.Build();
        var sb = new StringBuilder();
        var somenteLeitura = soLeitura == "readonly" ? "readonly=readonly" : "";
        var classeInt = classe ?? "";
        switch (tipo)
        {
            case InputTypes.Typeahead:
                sb.Append(
                    "<div class='typeahead__container'><div class='typeahead__field'><span class='typeahead__query'>");
                sb.AppendFormat(
                    "<input type = 'search' name='@nome' id='{0}' value='{1}' autocomplete='off' class='form-control js-typeahead {2}' {3} /> ",
                    nome.Replace('.', '_'), valor, classeInt, somenteLeitura);
                sb.Append("</span></div></div>");
                break;
            case InputTypes.Checkbox:
                sb.AppendFormat(
                    "<input type = 'checkbox' name='{0}' id='{1}' value='{2}' class='{3}' {4} />", nome,
                    nome.Replace('.', '_'), valor, classeInt, somenteLeitura);
                break;
            case InputTypes.Text:
                sb.AppendFormat(
                    "<input type = 'text' name='{0}' id='{1}' value='{2}' class='form-control {3}' {4} />",
                    nome, nome.Replace('.', '_'), valor, classeInt, somenteLeitura);
                break;
            case InputTypes.Password:
                sb.AppendFormat(
                    "<input type = 'password' name='{0}' id='{1}' value='{2}' class='form-control {3}' {4} />",
                    nome,
                    nome.Replace('.', '_'), valor, classeInt, somenteLeitura);
                break;
            case InputTypes.Email:
                sb.AppendFormat(
                    "<input type = 'email' name='{0}' id='{1}' value='{2}' class='form-control {3}' {4} />", nome,
                    nome.Replace('.', '_'), valor, classeInt, somenteLeitura);
                break;
            case InputTypes.Textarea:
                sb.AppendFormat(
                    "<textarea nname='{0}' id='{1}' rows='5' cols='7'class='form-control {2}' {3}>{4}</textarea>",
                    nome, nome.Replace('.', '_'), classeInt, somenteLeitura, valor);
                break;
            case InputTypes.File:
                sb.AppendFormat(
                    "<label for='{0}' class='btn btn-warning btn-xs' style='color: #fff'><i class='fa fa-plus-circle'></i> {1})</label>",
                    nome, "Selecione um arquivo");
                sb.AppendFormat(
                    "<input type = 'file' class='addFile' name='{0}' id='{1}' value='{2}' style='display: none' />",
                    nome, nome.Replace('.', '_'), valor);
                break;
            case InputTypes.Select:
                if (string.IsNullOrWhiteSpace(opcoes))
                {
                    sb.AppendFormat(
                        "<select name='{0}' id='{1}' value='{2}' class='form-control {3}' aria-invalid='false' {4}><option value=''></ option ></select>",
                        nome, nome.Replace('.', '_'), valor, classeInt, somenteLeitura);
                }
                else
                {
                    var niveis = _configuration.GetSection(opcoes).Value.Split('|').OrderBy(a => a).ToList();
                    sb.AppendFormat(
                        "<select name='{0}' id='{1}' value='{2}' class='form-control {3}' aria-invalid='false' {4}>",
                        nome, nome.Replace('.', '_'), valor, classeInt, somenteLeitura);
                    if (string.IsNullOrWhiteSpace(primeiroItem)) sb.Append("<option value = '' ></option>");
                    else sb.AppendFormat("<option value=''>{0}</option>", primeiroItem);
                    if (niveis.Any(a => a.Contains(';')))
                        foreach (var item in niveis)
                        {
                            var it = item.Split(';');
                            var select = valor == null ? "" : it[0] == valor.ToString() ? "selected" : "";
                            sb.AppendFormat("<option value='{0}' {1}>{2}</option>", it[0], select, it[1]);
                        }
                    else
                        foreach (var item in niveis)
                        {
                            var select = valor == null ? "" :
                                item != null && item == valor.ToString() ? "selected" : "";
                            sb.AppendFormat("<option value='{0}' {1}>{0}</option>", item, select);
                        }

                    sb.Append("</select>");
                }

                break;
        }

        return sb.ToString();
    }

    public static string MontaInputSite(InputTypes tipo, string nome, object valor, string label, string requerido,
        string coluns, string opcoes = null, string classe = null, string soLeitura = null, string help = null,
        string placeholder = null)
    {
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        _configuration = builder.Build();
        var sb = new StringBuilder();
        sb.AppendFormat("<div class='col-lg-{0} col-sm-{0} col-md-{0} col-12' id='div{1}' {2}>", coluns,
            nome.Replace('.', '_'), tipo == InputTypes.File ? "style='margin-top: -25px;'" : "");
        sb.Append("<div class='form-group'>");
        if (!string.IsNullOrWhiteSpace(label) && tipo != InputTypes.File)
        {
            sb.Append("<span class='bar'></span>");
            sb.AppendFormat("<label for='{0}'><span id='label{1}'>{2}</span> {3}:</label>", nome.Replace('.', '_'),
                nome, label, requerido);
        }

        var somenteLeitura = soLeitura == "readonly" ? "readonly=readonly" : "";
        var classeInt = classe ?? "";
        switch (tipo)
        {
            case InputTypes.Typeahead:
                sb.Append(
                    "<div class='typeahead__container'><div class='typeahead__field'><span class='typeahead__query'>");
                sb.AppendFormat(
                    "<input type='search' name='{0}' id='{1}' value='{2}' autocomplete='off' class='form-control js-typeahead {3}' {4} placeholder='{5}'/> ",
                    nome, nome.Replace('.', '_'), valor, classeInt, somenteLeitura, placeholder);
                sb.Append("</span></div></div>");
                break;
            case InputTypes.Checkbox:
                sb.AppendFormat(
                    "<input type='checkbox' name='{0}' id='{1}' value='{2}' class='filled-in {3}' {4} />", nome,
                    nome.Replace('.', '_'), valor, classeInt, somenteLeitura);
                break;
            case InputTypes.Text:
                sb.AppendFormat(
                    "<input type='text' name='{0}' id='{1}' value='{2}' class='form-control {3}' {4} placeholder='{5}' />",
                    nome,
                    nome.Replace('.', '_'), valor, classeInt, somenteLeitura, placeholder);
                break;
            case InputTypes.Password:
                sb.AppendFormat(
                    "<input type='password' name='{0}' id='{1}' value='{2}' class='form-control {3}' {4} />", nome,
                    nome.Replace('.', '_'), valor, classeInt, somenteLeitura);
                break;
            case InputTypes.Email:
                sb.AppendFormat(
                    "<input type='email' name='{0}' id='{1}' value='{2}' class='form-control {3}' {4} placeholder='{5}' />",
                    nome,
                    nome.Replace('.', '_'), valor, classeInt, somenteLeitura, placeholder);
                break;
            case InputTypes.Textarea:
                sb.AppendFormat(
                    "<textarea name='{0}' id='{1}' rows='5' cols='7' class='form-control {2}' {3} placeholder='{5}'>{4}</textarea>",
                    nome, nome.Replace('.', '_'), classeInt, somenteLeitura, valor, placeholder);
                break;
            case InputTypes.File:
                sb.AppendFormat(
                    "<label for='{0}' class='btn btn-warning btn-xs' style='color: #fff'><i class='fa fa-plus-circle'></i> {1})</label>",
                    nome, string.IsNullOrWhiteSpace(label) ? "Selecione um arquivo" : label);
                sb.AppendFormat(
                    "<input type = 'file' class='addFile' name='{0}' id='{1}' value='{2}' style='display: none' />",
                    nome, nome.Replace('.', '_'), valor);
                break;
            case InputTypes.Select:
                if (string.IsNullOrWhiteSpace(opcoes))
                {
                    sb.AppendFormat(
                        "<select name='{0}' id='{1}' value='{2}' class='form-control {3}' aria-invalid='false' {4} style='height: auto;'><option value=''></ option ></select>",
                        nome, nome.Replace('.', '_'), valor, classeInt, somenteLeitura);
                }
                else
                {
                    var niveis = _configuration.GetSection(opcoes).Value.Split('|').OrderBy(a => a).ToList();
                    sb.AppendFormat(
                        "<select name='{0}' id='{1}' value='{2}' class='form-control {3}' aria-invalid='false' {4} style='height: auto;'>",
                        nome, nome.Replace('.', '_'), valor, classeInt, somenteLeitura);
                    sb.Append("<option value=''></option>");
                    if (niveis.Any(a => a.Contains(';')))
                        foreach (var item in niveis)
                        {
                            var it = item.Split(';');
                            var select = valor == null ? "" : it[0] == valor.ToString() ? "selected" : "";
                            sb.AppendFormat("<option value='{0}' {1}>{2}</option>", it[0], select, it[1]);
                        }
                    else
                        foreach (var item in niveis)
                        {
                            var select = valor == null ? "" :
                                item != null && item == valor.ToString() ? "selected" : "";
                            sb.AppendFormat("<option value='{0}' {1}>{0}</option>", item, select);
                        }

                    sb.Append("</select>");
                }

                break;
        }

        sb.AppendFormat("<p class='help-block mt-1'><small>{0}</small></p></div></div>", help);
        return sb.ToString();
    }
}