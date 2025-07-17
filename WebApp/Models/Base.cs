#region

using System.Security.Claims;
using System.Text.RegularExpressions;

#endregion

namespace WebApp.Models;

public static class Base
{
    public static string GetName<T>(this T item) where T : class
    {
        if (item == null) return string.Empty;

        return typeof(T).GetProperties()[0].Name;
    }

    public static bool IsMobile(HttpContext contexto)
    {
        var userAgent = contexto.Request.Headers["User-Agent"].ToString();
        if (string.IsNullOrEmpty(userAgent))
            return false;
        //tablet
        if (Regex.IsMatch(userAgent, "(tablet|ipad|playbook|silk)|(android(?!.*mobile))", RegexOptions.IgnoreCase))
            return true;
        //mobile
        const string mobileRegex =
            "blackberry|iphone|mobile|windows ce|opera mini|htc|sony|palm|symbianos|ipad|ipod|blackberry|bada|kindle|symbian|sonyericsson|android|samsung|nokia|wap|motor";

        if (Regex.IsMatch(userAgent, mobileRegex, RegexOptions.IgnoreCase)) return true;
        //not mobile 
        return false;
    }

    public static DateTime GetDataUm()
    {
        return new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
    }
    
    public static DateTime GetDataUltimoDia()
    {
        var data = DateTime.Today;
        return new DateTime(data.Year, data.Month, DateTime.DaysInMonth(data.Year, data.Month));
    }

    public static TT GetDadosUsuario<TT>(ClaimsPrincipal user, string campo) where TT : IConvertible
    {
        var claims = user.Claims;
        if (claims != null)
        {
            var item = claims.FirstOrDefault(c => c.Type == campo)?.Value;
            return (TT)Convert.ChangeType(item, typeof(TT));
        }

        return (TT)Convert.ChangeType(null, typeof(TT));
    }

    public static bool ExibirParaCargos(ClaimsPrincipal user, string cargos)
    {
        return cargos.Split(',').Contains(GetDadosUsuario<string>(user, "IdCargo"));
    }

    public static bool VerificaPermissao(ClaimsPrincipal user, string listPaginas)
    {
        var podeAcessar = false;
        var paginas = listPaginas.Split(',');

        // se for usuario suporte, pode acessar qualquer página, sempre retorna TRUE para o acesso
        if (GetDadosUsuario<bool>(user, "IsSuporte"))
        {
            podeAcessar = true;
        }
        else
        {
            var acesso = GetDadosUsuario<string>(user, "Permissoes");
            if (!string.IsNullOrWhiteSpace(acesso))
                foreach (var a in acesso.Split(','))
                    if (paginas.Any(c => c == a))
                        podeAcessar = true;
        }

        return podeAcessar;
    }

    public static string ShowHideVerificaPermissao(string listPaginas)
    {
        var podeAcessar = "dec-hide";
        var paginas = listPaginas.Split(',');

        // se for usuario suporte, pode acessar qualquer página, sempre retorna TRUE para o acesso
        //if (GetDadosUsuario<bool>("IsSuporte"))
        //{
        //    podeAcessar = "";
        //}
        //else
        //{
        //    var acesso = GetDadosUsuario<string>("Permissoes");
        //    if (!string.IsNullOrWhiteSpace(acesso))
        //        if (paginas.Intersect(acesso.Split(',')).Any())
        //            podeAcessar = "";
        //}

        return podeAcessar;
    }

    private static string GetMensagem(string itm)
    {
        if (itm.Contains("Add")) return "Cadastrar " + itm.Replace("Add", "");
        if (itm.Contains("Del")) return "Deletar " + itm.Replace("Del", "");
        if (itm.Contains("Details")) return "Visualizar " + itm.Replace("Details", "");
        if (itm.Contains("Edit")) return "Alterar " + itm.Replace("Edit", "");
        if (itm.Contains("Lista")) return "Visualizar lista de " + itm.Replace("Lista", "");
        return "";
    }
}