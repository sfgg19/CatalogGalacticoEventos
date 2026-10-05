using CatalogoGalactico.Data;
using CatalogoGalactico.Models;
using CatalogoGalactico.Services;

namespace CatalogoGalactico.Endpoints
{
    public static class EventoEndpoints
    {
        public static void MapEventoEndpoints(this RouteGroupBuilder api)
        {
            var group = api.MapGroup("/eventos").WithTags("Eventos");

            group.MapGet("/", () =>
            {
                var data = GalacticContext.Eventos;
                return Results.Ok(new ApiResponse<List<Evento>>(data, new PaginacionMeta(1, data.Count, data.Count)));
            })
            .WithName("GetEventos")
            .WithSummary("Lista todos los eventos históricos")
            .Produces<ApiResponse<List<Evento>>>(StatusCodes.Status200OK);

            group.MapGet("/{id:int}", (int id) =>
            {
                var evento = GalacticContext.Eventos.FirstOrDefault(e => e.Id == id);
                return evento is null
                    ? Results.NotFound(new ApiErrorResponse(new ApiErrorBody("NOT_FOUND", "Evento no encontrado")))
                    : Results.Ok(new ApiResponse<Evento>(evento));
            })
            .WithName("GetEventoById")
            .WithSummary("Obtiene los detalles y participantes de un evento")
            .Produces<ApiResponse<Evento>>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound);

            group.MapPost("/", (Evento input) =>
            {
                var nuevo = input with { Id = GalacticContext.GenerarId(GalacticContext.Eventos, e => e.Id) };
                GalacticContext.Eventos.Add(nuevo);
                return Results.Created($"/api/eventos/{nuevo.Id}", new ApiResponse<Evento>(nuevo));
            })
            .WithName("CreateEvento")
            .WithSummary("Registra un nuevo evento histórico")
            .Produces<ApiResponse<Evento>>(StatusCodes.Status201Created)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest);

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

            group.MapGet("/{id:int}/mvp", (int id) =>
            {
                var evento = GalacticContext.Eventos.FirstOrDefault(e => e.Id == id);
                if (evento == null) return Results.NotFound(new ApiErrorResponse(new ApiErrorBody("NOT_FOUND", "Evento no encontrado")));

                var cartasEvento = GalacticContext.Cartas.Where(c => evento.Participantes.Contains(c.PersonajeId)).ToList();
                if (!cartasEvento.Any()) return Results.NotFound(new ApiErrorResponse(new ApiErrorBody("NOT_FOUND", "Participantes sin cartas")));

                var mvpCard = cartasEvento.OrderByDescending(c => c.Poder).First();
                var mvpPersonaje = GalacticContext.Personajes.First(p => p.Id == mvpCard.PersonajeId);

                return Results.Ok(new ApiResponse<object>(new { Personaje = mvpPersonaje.Nombre, Poder = mvpCard.Poder }));
            })
            .WithName("GetEventoMVP")
            .WithSummary("Calcula y devuelve el participante con mayor poder (MVP) del evento")
            .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound);

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
        }
    }
}
