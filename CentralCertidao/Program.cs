using System.Globalization;
using iDevCL;
using LyTex;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Repository;
using Rotativa.AspNetCore;
using Serilog;
using Transfera;
using Web.Models;


try
{
    var builder = WebApplication.CreateBuilder(args);

    DateTime eastern = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "America/Sao_Paulo");


    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Error()
        .Enrich.FromLogContext()
        .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
        //.WriteTo.Console(LogEventLevel.Debug)
        .CreateBootstrapLogger();

    //static IConfigurationBuilder Configuration(IWebHostEnvironment env, IConfigurationBuilder config)
    //{
    //    var currentDirectoryPath = Directory.GetCurrentDirectory();
    //    //var filename = $"appsettings.{env.EnvironmentName}.json";
    //    var filename = $"appsettings.json";
    //    //Console.WriteLine(File.Exists(Path.Combine(env.ContentRootPath, filename)));
    //    //Console.WriteLine($"EnvironmentName: {env.EnvironmentName}");
    //    config.SetBasePath(env.ContentRootPath)
    //        .AddJsonFile(filename, true, true) //?? "Development" 
    //        .AddEnvironmentVariables();
    //    return config;
    //}

    //var configuration = Configuration(builder.Environment, new ConfigurationBuilder()).Build();

    //builder.WebHost.UseKestrel(o => { o.Limits.MaxRequestBodySize = long.MaxValue; }).UseContentRoot(Directory.GetCurrentDirectory()).UseIISIntegration();
    // Add services to the container.
    builder.Host.UseSerilog();
    builder.Services.AddControllersWithViews();
    builder.Services.Configure<Mensagens>(builder.Configuration.GetSection("Mensagens"));
    var connString = builder.Configuration.GetConnectionString("Conn");
    builder.Services.AddDbContext<Contexto>(options =>
    {
        options.UseSqlServer(connString);
        options.EnableSensitiveDataLogging();
    });
    //builder.Services.AddSingleton(configuration);
    //builder.Services.AddDbContext<Contexto>(options => options.UseSqlServer(connString));
    builder.Services.AddScoped<ITransferaApi, TransferaApi>();
    builder.Services.AddScoped<ITransferaApiAutenticar, TransferaApiAutenticar>();
    builder.Services.AddScoped(typeof(IGenericDataRepository<>), typeof(GenericDataRepository<>));
    builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    builder.Services.Configure<IISServerOptions>(o => { o.MaxRequestBodySize = long.MaxValue; });
    builder.Services.Configure<FormOptions>(o => { o.MultipartBodyLengthLimit = long.MaxValue; });
    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(240);
        options.LoginPath = "/AccessControl";
        options.AccessDeniedPath = "/AccessControl/AccessDenied";
        options.SlidingExpiration = true;
    });
    builder.Services.AddMemoryCache();
    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromSeconds(10);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });
    builder.Services.AddMvc(config => config.ModelBinderProviders.Insert(0, new DataTableModelBinderProvider()));
    var cookiePolicyOptions = new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Strict };

    var cultureInfo = new CultureInfo("pt-BR");
    CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
    CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

    var app = builder.Build();
    var supportedCultures = new[] { new CultureInfo("pt-BR") };
    app.UseRequestLocalization(new RequestLocalizationOptions
    {
        DefaultRequestCulture = new RequestCulture("pt-BR", "pt-BR"),
        SupportedCultures = supportedCultures,
        SupportedUICultures = supportedCultures
    });

    using var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
    using var context = serviceScope.ServiceProvider.GetService<Contexto>();
    context?.Database.Migrate();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }
    //RotativaConfiguration.Setup(builder.Environment.WebRootPath);
    app.UseCookiePolicy(cookiePolicyOptions);
    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();
        RotativaConfiguration.Setup(builder.Environment.WebRootPath);
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Ticket}/{action=Index}/{id?}");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}