using IdentityService.Models.ConfigModels;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using TransactionCore;
using TransactionCore.BackgroundServices;
using TransactionCore.Consumer;
using TransactionCore.Data;
using TransactionCore.Services.Implementation;
using TransactionCore.Services.Interface;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new Exception("Connection string 'DefaultConnection' is missing.");
}

//Services
builder.Services.AddHttpClient();

builder.Services.AddDbContext<TransactionCoreDbContext>(options =>
{
    options.UseSqlServer(connectionString)
                  .EnableSensitiveDataLogging()
                  .EnableDetailedErrors();
});

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICryptoService, CryptoService>();
builder.Services.AddScoped<IIdentityServiceClient, IdentityServiceClient>();
builder.Services.AddScoped<INetworkService, NetworkService>();
builder.Services.AddScoped<IPaymentMethodService, PaymentMethodService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IPromoService, PromoService>();
builder.Services.AddScoped<IReferralRoleRewardSerice, ReferralRoleRewardSerice>();
builder.Services.AddScoped<IReferralService, ReferralService>();
builder.Services.AddScoped<IRemainderInfoService, RemainderInfoService>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<ISubscriptionUsageService, SubscriptionUsageService>();
builder.Services.AddScoped<ITronWebhookService, TronWebhookService>();
builder.Services.AddScoped<IUserInfoService, UserInfoService>();
builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();

//builder.Services.AddSingleton(StorageClient.Create());
//

builder.Services.Configure<RabbitMqConfigModel>(builder.Configuration.GetSection("RabbitMQ"));

builder.Services.AddHostedService<PaymentMonitoringService>();
builder.Services.AddHostedService<PromoExpirationService>();
builder.Services.AddHostedService<SubscriptionCleanupJob>();
builder.Services.AddMessaging(
    builder.Configuration,
    typeof(CreateUserInfoConsumer),
    typeof(IncrementReferralVisitsConsumer));
var identitySettings = builder.Configuration.GetSection("IdentitySettings");
var authorityUrl = identitySettings["Authority"];
var audience = identitySettings["Audience"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = authorityUrl;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };
    });


builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");


builder.Services.AddControllers();
builder.Services.AddMemoryCache();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Transaction Core API",
        Version = "v1"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "Bearer",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});


//app

var app = builder.Build();

if (builder.Configuration.GetValue<bool>("ApplyMigrations"))
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<TransactionCoreDbContext>();
    if (dbContext.Database.GetPendingMigrations().Any())
    {
        dbContext.Database.Migrate();
    }
}



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Transaction Core API V1");
    });
}

app.UseResponseCaching();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
