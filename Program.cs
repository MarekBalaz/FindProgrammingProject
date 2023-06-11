using FindProgrammingProject.FunctionalClasses.SigningLogic;
using FindProgrammingProject.Models.DbContexts;
using FindProgrammingProject.Models.ObjectModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
string fingerprint = builder.Configuration.GetValue<string>("ElasticCAFingerprint");
string username = builder.Configuration.GetValue<string>("ElasticSearchAuthUserName");
string password = builder.Configuration.GetValue<string>("ElasticSearchAuthPassword");
string url = builder.Configuration.GetValue<string>("ElasticSearchUrl");
var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
Serilog.Log.Logger = new LoggerConfiguration()
    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(url))
    {
        IndexFormat = $"FindProgrammingProject-AuthenticationServer-Logs-{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}",
        AutoRegisterTemplate = true,
        ModifyConnectionSettings = x => x.CertificateFingerprint(fingerprint).BasicAuthentication(username, password),
        NumberOfReplicas = 1,
        NumberOfShards = 3,
        OverwriteTemplate = true,
        TemplateName = "LogsForFPP",
        AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
        TypeName = null,
        BatchAction = ElasticOpType.Create
    }
    )
    .ReadFrom.Configuration(configuration)
    .CreateLogger();
// Add services to the container. 
builder.Services.AddControllers();
builder.Services.AddLogging();
builder.Logging.AddSerilog();
builder.Services.AddAuthentication(x =>

{

    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(o =>
{

    var Key = Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("IssuerSigningKey"));

    o.TokenValidationParameters = new TokenValidationParameters

    {

        ValidateIssuer = true,

        ValidateAudience = true,

        ValidIssuer = builder.Configuration.GetValue<string>("ValidJwtIssuer"),

        ValidAudience = $"{builder.Configuration.GetValue<string>("ValidJwtAudience")}",

        ValidateLifetime = true,

        ValidateIssuerSigningKey = true,

        IssuerSigningKey = new SymmetricSecurityKey(Key),

    };

});

builder.Services.AddDbContext<Context>(x =>
{
    x.UseSqlServer(builder.Configuration.GetValue<string>("MSSQLConnectionString"));
});

builder.Services.AddIdentity<User, IdentityRole>(x =>
 {
     x.User.RequireUniqueEmail = true;
     x.Password.RequireNonAlphanumeric = false;
     x.Password.RequiredLength = 8;
     x.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
     x.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
     x.Lockout.MaxFailedAccessAttempts = 5;
     x.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

 }).AddEntityFrameworkStores<Context>().AddDefaultTokenProviders();
builder.Services.AddSingleton<IJwtTokenGenerator>(x => new JwtTokenGenerator());
builder.Services.Configure<DataProtectionTokenProviderOptions>(x =>
{
    x.TokenLifespan = TimeSpan.FromMinutes(15);
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
public partial class Program{ };
