using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Passion_of_code.Models;
using System.Web;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);
var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();
// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<PassionOfCodeContext>();
//services.AddMvc().AddSessionStateTempDataProvider();
builder.Services.AddSession();


var app = builder.Build();
app.UseSession(); 

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();


app.MapControllerRoute(
    name: "default",    
    pattern: "{controller=Event}/{action=Year}/{year=2024}"
    // defaults: new { controller = "Home", action = "Index", year = 2024 }
    );

app.Run();
