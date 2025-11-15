using SistemaAcai_II.Repository;
using SistemaAcai_II.Libraries.Login;
using SistemaAcai_II.Repositories.Contracts;
using SistemaAcai_II.Repositories.Contract;
using SistemaAcai_II.Repository.Contract;
using AppQuinta6.Repository;
using SistemaAcai_II.Libraries.ExportarArquivo;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using System.Net.Mail;
using System.Net;
using SistemaAcai_II.Libraries.Email;
using SistemaAcai_II.Libraries.Filtro;
using SistemaAcai_II.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();



//Adicionado para manipular a Sessão
builder.Services.AddHttpContextAccessor();


builder.Services.AddScoped<SistemaAcai_II.Libraries.Sessao.Sessao>();
builder.Services.AddScoped<IColaboradorRepository, ColaboradorRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IFiliaisRepository, FiliaisRepository>();
builder.Services.AddScoped<IProdutoSimplesRepository, ProdutoSimplesRepository>();
builder.Services.AddScoped<IComandaRepository, ComandaRepository>();
builder.Services.AddScoped<IItensComandaRepository, ItemComandaRepository>();
builder.Services.AddScoped<IFormasPagamentoRepository, FormasPagamentoRepository>();
builder.Services.AddScoped<ICaixaRepository, CaixaRepository>();

builder.Services.AddScoped<LoginColaborador>();
builder.Services.AddScoped<LoginCliente>();
builder.Services.AddScoped<ExportaArquivo>();

builder.Services.AddScoped<SistemaAcai_II.Libraries.Cookie.Cookie>();
builder.Services.AddScoped<SistemaAcai_II.Libraries.PedidoCompra.CookiePedidoCompra>();
builder.Services.AddScoped<SistemaAcai_II.Libraries.PedidoCompra.CookieEditarPedidoCompra>();
builder.Services.AddScoped<CaixaAutorizacaoAttribute>();

/*
 * SMTP
 */
builder.Services.AddTransient<SmtpClient>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    return new SmtpClient
    {
        Host = config["Email:Host"],
        Port = int.Parse(config["Email:Port"]),
        Credentials = new NetworkCredential(
            config["Email:Username"],
            config["Email:Password"]),
        EnableSsl = true
    };
});
builder.Services.AddScoped<GerenciarEmail>();

// corrigir problema com TEMPDATA
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    // Set a short timeout for easy testing. 
    options.IdleTimeout = TimeSpan.FromSeconds(3600);
    options.Cookie.HttpOnly = true;
    // Make the session cookie essential 
    options.Cookie.IsEssential = true;
});
builder.Services.AddMvc().AddSessionStateTempDataProvider();

builder.Services.AddMemoryCache(); // Guardar os dados na memoria
builder.Services.AddSession(options =>
{

});

builder.Services.AddSingleton<SistemaAcai_II.Services.IScaleService, SistemaAcai_II.Services.ScaleService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseDefaultFiles();
app.UseCookiePolicy();
app.UseSession();
app.UseRouting();
app.UseAuthorization();

app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
