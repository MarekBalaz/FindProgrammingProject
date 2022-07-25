using Elasticsearch.Net;
using FindProgrammingProject.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

var configuration = System.Configuration.ConfigurationManager.AppSettings;
Serilog.Log.Logger = new LoggerConfiguration().MinimumLevel.Warning().WriteTo.Elasticsearch(new ElasticsearchSinkOptions()).CreateLogger();
// Add services to the container. 
builder.Services.AddControllersWithViews();
builder.Services.AddLogging();
builder.Logging.AddSerilog();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(x =>
{
    x.LoginPath = "Signing/SignIn";
    x.LogoutPath = "Signing/SignOut";
    x.Cookie.Name = "AuthenticationCookie_FindProgrammingProject";
    x.Cookie.Expiration = TimeSpan.FromMinutes(1);
})//.AddGoogle(x =>
//{
//    x.ClientId = "";
//    x.ClientSecret = "";
//}).AddTwitter(x =>
//{
//    x.ConsumerKey = "";
//    x.ConsumerSecret = "";
//}).AddGitHub(x =>
//{
//    x.ClientId = "";
//    x.ClientSecret = "";
//})
;
builder.Services.AddDbContext<UserContext>(x =>
{
    x.UseSqlServer(configuration["MSSQLConnectionString"]);
});
builder.Services.AddIdentity<User, IdentityRole>(x =>
 {
     x.SignIn.RequireConfirmedEmail = true;
     x.User.RequireUniqueEmail = true;

 }).AddEntityFrameworkStores<UserContext>();
builder.Services.Configure<DataProtectionTokenProviderOptions>(x =>
{
    x.TokenLifespan = TimeSpan.FromMinutes(5);
});
var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Signing}/{action=SignIn}");

app.Run();
public partial class Program{ };
