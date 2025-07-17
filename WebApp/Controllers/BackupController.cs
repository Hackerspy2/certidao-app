using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Repository.Util;
using WebApp.Filter;

namespace WebApp.Controllers;

public class BackupController : Controller
{
    private readonly string _controller = "";
    private readonly IWebHostEnvironment _hostingEnvironment;
    private readonly ILogger<BackUp> _logger;
    private readonly string _pasta = "";
    private SqlConnection _sqlConexao;
    private static IConfiguration? _configuration;

    public BackupController(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment environment,
        ILogger<BackUp> logger, IConfiguration? configuration)
    {
        _configuration = configuration;
        _controller = httpContextAccessor.HttpContext.Request.RouteValues["controller"].ToString();
        _hostingEnvironment = environment;
        _logger = logger;
        Conexao = _configuration.GetConnectionString("Conn");
        if (_controller != null)
            _pasta = Path.Combine(environment.WebRootPath, $"{_controller.ToLower()}");
    }

    public string Conexao { get; set; }

    protected void CriarZip(string arquivoZip, IEnumerable<string> arquivos)
    {
        try
        {
            var crc = new Crc32();
            var s = new ZipOutputStream(System.IO.File.Create(arquivoZip));

            s.SetLevel(6);

            foreach (var arquivo in arquivos)
                if (arquivo != null)
                {
                    var fs = System.IO.File.OpenRead(arquivo);

                    var buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    var entry = new ZipEntry(new FileInfo(arquivo).Name)
                        { DateTime = DateTime.Now, Size = fs.Length };
                    fs.Close();
                    crc.Reset();
                    crc.Update(buffer);
                    entry.Crc = crc.Value;
                    s.PutNextEntry(entry);
                    s.Write(buffer, 0, buffer.Length);
                    s.Finish();
                    s.Close();
                    System.IO.File.Delete(arquivo);
                }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }

    public string GerarBackup()
    {
        string nome = null;
        try
        {
            _sqlConexao = new SqlConnection(Conexao);
            var dt = DateTime.Now;
            var file = string.Format("{6}-{0}-{1}-{2}-{3}-{4}-{5}", dt.Day, dt.Month, dt.Year, dt.Hour, dt.Minute,
                dt.Second, _sqlConexao.Database);
            var cmd = $@"BACKUP DATABASE [{_sqlConexao.Database}] TO DISK = '{_pasta}\\{file}.bak' WITH COPY_ONLY";
            var command = new SqlCommand(cmd, _sqlConexao);

            _sqlConexao.Open();
            command.ExecuteNonQuery();
            _sqlConexao.Close();

            var arquivo = new string[1];
            arquivo.SetValue($@"{_pasta}\{file}.bak", 0);
            CriarZip($@"{_pasta}\{file}.rar", arquivo);

            nome = $"{file}.rar";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }

        return nome;
    }

    [RequiredPermission("gerente")]
    public IActionResult Lista()
    {
        var files = new DirectoryInfo(_pasta).GetFiles();
        var model = files
            .Select(fileInfo => new BackUp
                { DataCriado = fileInfo.CreationTime, Nome = fileInfo.Name, Tamanho = fileInfo.Length / 1024 })
            .OrderByDescending(a => a.DataCriado).ToList();
        return View(model);
    }

    [RequiredPermission("gerente")]
    public IActionResult Gerar()
    {
        GerarBackup();
        return RedirectToAction("Lista");
    }

    [RequiredPermission]
    public JsonResult Del(string id)
    {
        try
        {
            System.IO.File.Delete($@"{_pasta}\{id}");
            return Json(new { cssClass = "success", mensagem = "Deletado com sucesso!" });
        }
        catch (Exception e)
        {
            return Json(new { cssClass = "warning", mensagem = "Ocorreu um erro ao deletar o arquivo!" });
        }
    }
}