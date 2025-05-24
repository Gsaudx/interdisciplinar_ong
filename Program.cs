using Ong.Data;
using Microsoft.EntityFrameworkCore;
using Ong.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Ong.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<DbOng>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("default")));

// Registrar serviços para injeção de dependência
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<DoacaoService>();
builder.Services.AddScoped<PedidoDoacaoService>();
builder.Services.AddScoped<EventoService>();
builder.Services.AddScoped<SessaoService>();

// Adicionar HttpContextAccessor para acesso ao HttpContext
builder.Services.AddHttpContextAccessor();

// Configurar autenticação com cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Usuario/Login";
        options.AccessDeniedPath = "/Usuario/AcessoNegado";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
    });

// Configurar sessão
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Adicionar middleware de autenticação e autorização
app.UseAuthentication();
app.UseAuthorization();

app.UseSession();
app.UseUserSession(); // Adiciona middleware para sincronizar sessão com autenticação

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();