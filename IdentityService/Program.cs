using Amazon.S3;
using IdentityService.Data;
using IdentityService.Models.ConfigModels;
using IdentityService.Services.Implementation;
using IdentityService.Services.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


builder.Configuration.AddEnvironmentVariables();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonS3>();

//var awsAccessKey = builder.Configuration["AWS:AccessKeyId"];
//var awsSecretKey = builder.Configuration["AWS:SecretAccessKey"];
//var region = RegionEndpoint.GetBySystemName(builder.Configuration["AWS:Region"]);

//builder.Services.AddSingleton<IAmazonS3>(sp =>
//    new AmazonS3Client(awsAccessKey, awsSecretKey, region));

var jwtSecretKey = builder.Configuration["JwtSettings:SecretKey"] ?? "placeholder-secret";
var keyBytes = Encoding.UTF8.GetBytes(jwtSecretKey);
var jwtSettings = builder.Configuration
    .GetSection("JwtSettings")
    .Get<JwtSettingsConfigModel>();

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

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
