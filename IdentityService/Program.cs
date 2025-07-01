using Google.Cloud.Storage.V1;
using IdentityService;
using IdentityService.Consumers;
using IdentityService.Data;
using IdentityService.IdentityServerConfig;
using IdentityService.Models.ConfigModels;
using IdentityService.Services.Implementation;
using IdentityService.Services.Interface;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


builder.Configuration.AddEnvironmentVariables();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.Configure<JwtSettingsConfigModel>(builder.Configuration.GetSection("JwtSettings"));


builder.Services.Configure<RabbitMqConfigModel>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddMessaging(builder.Configuration, typeof(UpdateUserRoleConsumer), typeof(GetUserRoleConsumer));

//builder.Services.AddSingleton(StorageClient.Create());

var jwtSecretKey = builder.Configuration["JwtSettings:SecretKey"] ?? "placeholder-secret";
var keyBytes = Encoding.UTF8.GetBytes(jwtSecretKey);
var signingKey = new SymmetricSecurityKey(keyBytes);
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettingsConfigModel>();
builder.Services.Configure<SmtpSettingsConfigModel>(builder.Configuration.GetSection("SmtpSettings"));

//builder.Services.AddIdentityServer()
//    .AddInMemoryClients(IdentityServerDefinitions.GetClients(builder.Configuration))
//    .AddInMemoryApiScopes(IdentityServerDefinitions.GetApiScopes())
//    .AddInMemoryApiResources(IdentityServerDefinitions.GetApiResources())
//    .AddInMemoryIdentityResources(IdentityServerDefinitions.GetIdentityResources())
//    .AddDeveloperSigningCredential();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOrMicroservice", policy =>
    {
        policy.RequireAssertion(context =>
            context.User.IsInRole("Admin") ||
            context.User.HasClaim("client_id", "TransactionCore")
        );
    });
});

builder.Services.AddControllers();
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
        Title = "Identity Service API",
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

var app = builder.Build();

if (builder.Configuration.GetValue<bool>("ApplyMigrations"))
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
    if (dbContext.Database.GetPendingMigrations().Any())
    {
        dbContext.Database.Migrate();
    }
    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    await DatabaseSeeder.SeedAsync(dbContext, configuration);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Identity Service API v1");
    });
}

app.UseHttpsRedirection();
//app.UseIdentityServer();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
