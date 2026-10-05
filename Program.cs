using System.Text.Json;
using CatalogoGalactico.Endpoints;
using CatalogoGalactico.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuración estricta para cumplir con el formato JSON exigido (camelCase)
builder.Services.ConfigureHttpJsonOptions(options => {
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registro del servicio para simular batallas
builder.Services.AddScoped<IBatallaService, BatallaService>();

var app = builder.Build();

// Configuración de Swagger para pruebas
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Conexión de todos los endpoints
var api = app.MapGroup("/api");
api.MapPersonajeEndpoints();
api.MapCartaEndpoints();
api.MapEventoEndpoints();

app.Run();