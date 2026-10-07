using System.Text.Json;
using CatalogoGalactico.Endpoints;
using CatalogoGalactico.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

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

// 1. CORS DEBE IR PRIMERO QUE TODO
app.UseCors();

// Configuración de Swagger para pruebas
app.UseSwagger();
app.UseSwaggerUI();

// 2. COMENTADO: Evita que la redirección a HTTPS rompa las cabeceras CORS del fetch local
// app.UseHttpsRedirection();

// Conexión de todos los endpoints en la raíz "" para acoplarse al frontend del docente
var api = app.MapGroup("/");

api.RequireCors();

api.MapPersonajeEndpoints();
api.MapCartaEndpoints();
api.MapEventoEndpoints();

app.Run();





/*
# 1. Crear la rama 'defensa' y cambiarte a ella
git checkout -b defensa

# 2. Agregar los cambios
git add .

# 3. Hacer el commit
git commit -m "Defensa: agrega campo image a personajes y endpoint /con-card para ficha de combate"

# 4. Subir la rama por primera vez a GitHub
git push -u origin defensa
 
 */