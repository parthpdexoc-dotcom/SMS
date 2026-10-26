using FirebaseAdmin;
using FluentValidation;
using FluentValidation.AspNetCore;
using Google.Apis.Auth.OAuth2;
using log4net.Config;
using Serilog;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Models.Filter;
using SMS.Generic.Services.Interfaces;
using SMS.Generic.Services.Services;
using SMS.Infrastructure.Extensions;

// Configure log4net (ISMSLogger / SMSLogger) so all custom logger output goes to file
var log4netConfigPath = Path.Combine(AppContext.BaseDirectory, "log4net.config");
if (File.Exists(log4netConfigPath))
    XmlConfigurator.Configure(new FileInfo(log4netConfigPath));

var builder = WebApplication.CreateBuilder(args);

// Initialize Firebase Admin SDK (when credential file is configured)
var firebaseCredentialPath = builder.Configuration["Firebase:CredentialPath"];
if (!string.IsNullOrWhiteSpace(firebaseCredentialPath))
{
    var resolvedPath = Path.IsPathRooted(firebaseCredentialPath)
        ? firebaseCredentialPath
        : Path.Combine(AppContext.BaseDirectory, firebaseCredentialPath.Trim().Replace('\\', Path.DirectorySeparatorChar));
    if (File.Exists(resolvedPath))
    {
        if (FirebaseApp.DefaultInstance == null)
            FirebaseApp.Create(new AppOptions { Credential = GoogleCredential.FromFile(resolvedPath) });
    }
}

// Serilog is the sole ILogger provider; file and console sinks accept Error/Critical/Fatal only.
builder.Host.UseSerilog((context, _, config) => config
    .ReadFrom.Configuration(context.Configuration)
    .MinimumLevel.Error()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Error)
    .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Error)
    .MinimumLevel.Override("Microsoft.Hosting", Serilog.Events.LogEventLevel.Error)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", Serilog.Events.LogEventLevel.Error)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Error)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", Serilog.Events.LogEventLevel.Error)
    .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Error)
    .Enrich.FromLogContext()
    .WriteTo.Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error)
    .WriteTo.File(
        path: "Logs/sms-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}"));

// ================= Controllers & Filters =================
builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(CustomExceptionFilter));
})
.AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    options.SerializerSettings.Converters.Add(new SMS.Generic.Dtos.JsonConverters.NewtonsoftTimeOnlyConverter());
    options.SerializerSettings.Converters.Add(new SMS.Generic.Dtos.JsonConverters.NewtonsoftNullableTimeOnlyConverter());
});

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// ================= Swagger =================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerWithJwt();

// ================= Database =================
builder.Services.AddSmsDb(builder.Configuration);
builder.Services.AddSmsAudit(builder.Configuration);

// ================= Authentication =================
builder.Services.AddJwtAuth(builder.Configuration);

// ================= Authorization Policies =================
builder.Services.AddRolePolicies();

// ================= Repositories & Services =================
builder.Services.AddRepositories();
builder.Services.AddAppServices();
builder.Services.AddAppAutoMapper();
builder.Services.AddEmailSender(builder.Configuration);
builder.Services.AddTwilioSms(builder.Configuration);
builder.Services.AddVehicleApprovalLinkBuilder(builder.Configuration);
builder.Services.AddFirebasePushNotification(builder.Configuration);

// ================= HttpContext Accessor =================
builder.Services.AddHttpContextAccessor();

// ================= Token Parser =================
builder.Services.AddScoped<ITokenParser, TokenParser>();

// ================= Configurations =================
builder.Services.AddSingleton<Configurations>();

// ================= Scheduled (Cron) Jobs =================
builder.Services.AddScheduledJobs(builder.Configuration);

// ================= CORS =================
var allowedOrigins = builder.Configuration
    .GetSection("CorsSettings:AllowedOrigins")
    .Get<string[]>()
    ?? new[] { "http://localhost:5173", "https://localhost:5173", "http://localhost:50591" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .WithExposedHeaders("X-New-Access-Token", "X-New-Access-Token-Expires-At")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ================= Build App =================
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
