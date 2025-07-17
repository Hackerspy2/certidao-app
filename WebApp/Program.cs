using System.Globalization;
using iDevCL;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Repository;
using Rotativa.AspNetCore;
using Serilog;
using WebApp.Models;

try
{
    var builder = WebApplication.CreateBuilder(args);

    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Error()
        .Enrich.FromLogContext()
        .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
        .CreateBootstrapLogger();

    builder.Host.UseSerilog();
    
    builder.Services.AddControllersWithViews();
    builder.Services.Configure<Mensagens>(builder.Configuration.GetSection("Mensagens"));
    var connString = builder.Configuration.GetConnectionString("Conn");
    builder.Services.AddDbContext<Contexto>(options =>
    {
        options.UseSqlServer(connString);
        options.EnableSensitiveDataLogging();
    });
    
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

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    // ============= NOVO CÓDIGO DE WARMUP =============
    app.Use(async (context, next) => {
        if (context.Request.Path == "/warmup") {
            // Pré-carrega dependências críticas
            await context.Response.WriteAsync("Warmup completed");
            return;
        }
        await next();
    });
    // =================================================

    app.UseCookiePolicy(cookiePolicyOptions);
    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();
    RotativaConfiguration.Setup(builder.Environment.WebRootPath);
    
    // Rotas MVC
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=DashBoard}/{id?}");

    // Health Checks
    app.MapGet("/health", () => Results.Ok("Online"));
    app.MapGet("/dbhealth", async (Contexto db) => 
        await db.Database.CanConnectAsync() 
            ? Results.Ok("DB Online") 
            : Results.Problem("DB Offline"));

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
