using Ong.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<DbOng>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("default")));

builder.Services.AddSession();

var app = builder.Build();

app.MapControllerRoute("default", "/{controller=Produto}/{action=Index}/{id?}");

app.UseSession();

app.UseStaticFiles();

app.Run();