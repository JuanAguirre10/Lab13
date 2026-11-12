using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Veterinaria.Datos;
using Veterinaria.Datos.Repositorios;
using Veterinaria.Negocio;
using Veterinaria.Negocio.Servicios;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.NumberHandling = JsonNumberHandling.Strict);

builder.Services.AddDbContext<VeterinariaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Veterinaria")));

builder.Services.AddScoped<IPropietarioRepositorio, PropietarioRepositorio>();
builder.Services.AddScoped<IMascotaRepositorio, MascotaRepositorio>();
builder.Services.AddScoped<IPropietarioServicio, PropietarioServicio>();
builder.Services.AddScoped<IMascotaServicio, MascotaServicio>();
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

builder.Services.AddCors(options =>
    options.AddPolicy("Front", policy => policy
        .WithOrigins("http://localhost:5173")
        .AllowAnyHeader()
        .AllowAnyMethod()));

var app = builder.Build();

app.MapOpenApi();
app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "v1"));

app.UseHttpsRedirection();
app.UseCors("Front");
app.UseAuthorization();
app.MapControllers();

app.Run();
