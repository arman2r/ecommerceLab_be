using Amazon.S3;
using ecommerceLab.Models;
using ecommerceLab.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Amazon.Extensions.NETCore.Setup;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuración de AWS
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonS3>();
builder.Services.AddScoped<IProductoService, ProductoService>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Storelab API V1", Version = "v1" });
    // Otras configuraciones opcionales
});

builder.Services.AddScoped<IProductoService, ProductoService>(); // Registra servicio IProductoService
builder.Services.AddScoped<IAuthService, AuthService>();


// Registra ApplicationDbContext
builder.Services.AddDbContext<ApplicationDbContext>(options => { 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString)
           .LogTo(Console.WriteLine, LogLevel.Information);    
});


// Configuración de la autenticación JWT
var jwtSettings = builder.Configuration.GetSection("Jwt"); 
//var secretKey = !string.IsNullOrEmpty(secret) ? Encoding.ASCII.GetBytes(secret) : null;
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    string secret = builder.Configuration!.GetValue<string>("Jwt:secret")!;
    //string issuer = builder.Configuration!.GetValue<string>("Authorization:issuer")!;
    //string audience = builder.Configuration!.GetValue<string>("Authorization:audience")!;
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();
 

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Storelab API V1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

#region Conf CORS
app.UseCors(x =>
{
    if (builder.Environment.IsDevelopment())
    {
        x.AllowAnyHeader()
        .AllowAnyMethod()
        .SetIsOriginAllowed(origin => true) // allow any origin
        .WithMethods("GET", "POST", "PUT")
        .AllowCredentials();
    }
    else
    {
        x.AllowAnyHeader()
        .WithOrigins("*") //Specify url
        .AllowAnyMethod() // Permitir cualquier método
        .AllowCredentials();
    }
});
#endregion

app.Run();
