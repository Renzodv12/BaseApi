using Serilog;
using WebApi.Config;
using Infra.Cache.Extension;
using Microsoft.Extensions.Configuration;
using Core.Interfaces;
using Infra.DBManager;
using Microsoft.EntityFrameworkCore;
using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Business.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Infra.Repository;
using Business.Application;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration.GetSection("Serilog"));
    config.Enrich.FromLogContext();
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "BaseApi", Version = "v1" });
});

builder.Services.UseRedisCache(builder.Configuration.GetSection("Cache"));

var x = builder.Services.AddDbContext<ApiDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("WebApi"))
   );

builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(jwt =>
{
    var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtConfig:Secret"]);

    jwt.SaveToken = true;
    jwt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        RequireExpirationTime = false,
        ValidateLifetime = true
    };
});

builder.Services.AddIdentityCore<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
 .AddEntityFrameworkStores<ApiDbContext>();


//agregamos los servicios 
builder.Services.AddScoped(typeof(ITokenHandler), typeof(Business.Services.TokenHandler));

//application, repository y dbcontext, se inyectan por convenci�n de nombres (Scrutor)
builder.Services.Scan(scan =>
    scan.FromAssemblies(Assembly.GetAssembly(typeof(Application<>)))
     .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Application")))
    .AsImplementedInterfaces()
    .WithTransientLifetime());

builder.Services.Scan(scan =>
    scan.FromAssemblies(Assembly.GetAssembly(typeof(Repository<>)))
     .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Repository")))
    .AsImplementedInterfaces()
    .WithTransientLifetime());

builder.Services.Scan(scan =>
   scan.FromAssemblies(Assembly.GetAssembly(typeof(DbContext<>)))
    .AddClasses(classes => classes.Where(type => type.Name.EndsWith("DbContext")))
   .AsImplementedInterfaces()
   .WithTransientLifetime());
//Add zipkin
builder.Services.AddOpenTelemetry()
                    .ConfigureResource(otelBuilder => otelBuilder
                        .AddService("OpenTelemetry"))
                    .WithTracing(otelBuilder => otelBuilder
                        .AddAspNetCoreInstrumentation()
                        .AddEntityFrameworkCoreInstrumentation()
                        .AddZipkinExporter()
                        .ConfigureServices(services =>
                        {
                            services.Configure<ZipkinExporterOptions>(builder.Configuration.GetSection("OpenTelemetrySettings:ZipkinSettings"));
                            services.Configure<EntityFrameworkInstrumentationOptions>(builder.Configuration.GetSection("OpenTelemetrySettings:SQLTracingSettings"));
                        }));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BaseApi v1"));
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseRouting();
app.UseHttpsRedirection();

app.UseAuthorization();
app.UseSerilogRequestLogging();

app.MapControllers();

app.Run();
