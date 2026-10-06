using Microsoft.AspNetCore.Http;
using CatalogoGalactico.Models;
using CatalogoGalactico.Data;
using CatalogoGalactico.Services;

namespace CatalogoGalactico.Endpoints;

public static class EventoEndpoints
{
    public static void MapEventoEndpoints(this RouteGroupBuilder api)
    {
        var group = api.MapGroup("/eventos").WithTags("Eventos");

        // GET /eventos
        group.MapGet("/", () =>
        {
            var data = GalacticContext.Eventos;
            return Results.Ok(new ApiResponse<List<Evento>>(data, new PaginacionMeta(1, data.Count, data.Count)));
        })
        .WithName("GetEventos")
        .WithSummary("Lista todos los eventos históricos")
        .Produces<ApiResponse<List<Evento>>>(StatusCodes.Status200OK);

        // GET /eventos/{id}/mvp (Participante con mayor poder dentro del evento)
        group.MapGet("/{id:int}/mvp", (int id) =>
        {
            var evento = GalacticContext.Eventos.FirstOrDefault(e => e.Id == id);
            if (evento == null)
                return Results.NotFound(new ApiErrorResponse(new ApiErrorBody("NOT_FOUND", "Evento no encontrado")));

            if (evento.Participantes == null || !evento.Participantes.Any())
                return Results.BadRequest(new ApiErrorResponse(new ApiErrorBody("BAD_REQUEST", "El evento no tiene participantes registrados")));

            // Obtener cartas de los participantes
            var cartasEvento = GalacticContext.Cartas
                .Where(c => evento.Participantes.Contains(c.PersonajeId))
                .ToList();

            if (!cartasEvento.Any())
                return Results.NotFound(new ApiErrorResponse(new ApiErrorBody("NOT_FOUND", "Ninguno de los participantes tiene carta asignada")));

            var mvpCard = cartasEvento.OrderByDescending(c => c.Poder).First();
            var mvpPersonaje = GalacticContext.Personajes.FirstOrDefault(p => p.Id == mvpCard.PersonajeId);

            var resultado = new
            {
                Evento = evento.Nombre,
                PersonajeId = mvpPersonaje?.Id,
                Nombre = mvpPersonaje?.Nombre,
                Faccion = mvpPersonaje?.Faccion.ToString(),
                Poder = mvpCard.Poder,
                Habilidad = mvpCard.HabilidadEspecial
            };

            return Results.Ok(new ApiResponse<object>(resultado));
        })
        .WithName("GetEventoMVP")
        .WithSummary("Devuelve el participante con mayor poder dentro del evento")
        .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
        .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
        .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest);

        // POST /eventos/{id}/simular (Simulación de batalla)
        group.MapPost("/{id:int}/simular", (int id, IBatallaService batallaService) =>
        {
            try
            {
                var resultado = batallaService.SimularBatalla(id);
                return Results.Ok(new ApiResponse<object>(new
                {
                    Mensaje = "Simulación completada",
                    Ganador = resultado.Ganador,
                    PuntosRebelde = resultado.TotalRebelde,
                    PuntosImperio = resultado.TotalImperio,
                    Explicacion = resultado.Criterio
                }));
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new ApiErrorResponse(new ApiErrorBody("BAD_REQUEST", ex.Message)));
            }
        })
        .WithName("SimularBatallaEvento")
        .WithSummary("Simula el resultado de la batalla en base a las cartas de los participantes")
        .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
        .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest);

        // GET /eventos/{id}
        group.MapGet("/{id:int}", (int id) =>
        {
            var evento = GalacticContext.Eventos.FirstOrDefault(e => e.Id == id);
            return evento is null
                ? Results.NotFound(new ApiErrorResponse(new ApiErrorBody("NOT_FOUND", "Evento no encontrado")))
                : Results.Ok(new ApiResponse<Evento>(evento));
        })
        .WithName("GetEventoById")
        .WithSummary("Obtiene un evento por su ID")
        .Produces<ApiResponse<Evento>>(StatusCodes.Status200OK)
        .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound);

        // POST /eventos
        group.MapPost("/", (Evento input) =>
        {
            var nuevo = input with { Id = GalacticContext.GenerarId(GalacticContext.Eventos, e => e.Id) };
            GalacticContext.Eventos.Add(nuevo);
            return Results.Created($"/api/eventos/{nuevo.Id}", new ApiResponse<Evento>(nuevo));
        })
        .WithName("CreateEvento")
        .WithSummary("Registra un nuevo evento histórico")
        .Produces<ApiResponse<Evento>>(StatusCodes.Status201Created);

        // PUT /eventos/{id}
        group.MapPut("/{id:int}", (int id, Evento input) =>
        {
            var index = GalacticContext.Eventos.FindIndex(e => e.Id == id);
            if (index == -1) return Results.NotFound(new ApiErrorResponse(new ApiErrorBody("NOT_FOUND", "Evento no encontrado")));

            var actualizado = input with { Id = id };
            GalacticContext.Eventos[index] = actualizado;
            return Results.Ok(new ApiResponse<Evento>(actualizado));
        })
        .WithName("UpdateEvento")
        .WithSummary("Actualiza los datos de un evento histórico")
        .Produces<ApiResponse<Evento>>(StatusCodes.Status200OK)
        .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound);
    }
}