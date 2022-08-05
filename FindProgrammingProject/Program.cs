using Elasticsearch.Net;
using FindProgrammingProject.FunctionalClasses.SigningLogic;
using FindProgrammingProject.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Nest;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System.Configuration;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var reader = new AppSettingsReader();
var appSettings = System.Configuration.ConfigurationManager.AppSettings;
var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
Serilog.Log.Logger = new LoggerConfiguration()
    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri((string)reader.GetValue("ElasticSearchUri",typeof(string)))
    {
        //set options for elasticsearch
    }
    ))
    .ReadFrom.Configuration(configuration)
    .CreateLogger();
// Add services to the container. 
//builder.Services.AddControllersWithViews();
builder.Services.AddControllers();
builder.Services.AddLogging();
builder.Logging.AddSerilog();
builder.Services.AddAuthentication(x =>

{

    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(o =>
{

    var Key = Encoding.UTF8.GetBytes("a8d1fe1a-9523-4a2a-a72c-3b0fedd75bd5");

    o.SaveToken = true;

    o.TokenValidationParameters = new TokenValidationParameters

    {

        ValidateIssuer = true,

        ValidateAudience = true,

        ValidIssuer = "https://localhost:7168",

        ValidAudience = "https://localhost:7168",

        ValidateLifetime = true,

        ValidateIssuerSigningKey = true,

        IssuerSigningKey = new SymmetricSecurityKey(Key),

    };

}) //.AddGoogle(x =>
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
    x.UseSqlServer((string)reader.GetValue("MSSQLConnectionString", typeof(string)));
});
builder.Services.AddIdentity<User, IdentityRole>(x =>
 {
     //x.SignIn.RequireConfirmedEmail = true;
     x.User.RequireUniqueEmail = true;

 }).AddEntityFrameworkStores<UserContext>();
builder.Services.AddSingleton<ICreation>(x => new Creation((UserManager<User>)x.GetService(typeof(UserManager<User>))));
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
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
public partial class Program{ };
