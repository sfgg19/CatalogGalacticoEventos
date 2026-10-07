using Microsoft.AspNetCore.Http;
using CatalogoGalactico.Models;
using CatalogoGalactico.Data;

namespace CatalogoGalactico.Endpoints;

public static class PersonajeEndpoints
{
    public static void MapPersonajeEndpoints(this RouteGroupBuilder api)
    {
        var group = api.MapGroup("/personajes").WithTags("Personajes");

        // 1. GET /personajes?faccion=Imperio&fuerzaSensitivo=true (Filtros combinados)
        /*
        group.MapGet("/", (string? faccion, bool? fuerzaSensitivo) =>
        {
            var query = GalacticContext.Personajes.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(faccion) && Enum.TryParse<Faccion>(faccion, true, out var f))
            {
                query = query.Where(p => p.Faccion == f);
            }

            if (fuerzaSensitivo.HasValue)
            {
                query = query.Where(p => p.FuerzaSensitivo == fuerzaSensitivo.Value);
            }

            var data = query.ToList();
            return Results.Ok(new ApiResponse<List<Personaje>>(data, new PaginacionMeta(1, data.Count, data.Count)));
        })
        .WithName("GetPersonajes")
        .WithSummary("Lista personajes con filtros opcionales por facción y fuerzaSensitivo")
        .Produces<ApiResponse<List<Personaje>>>(StatusCodes.Status200OK);
        */

        group.MapGet("/", (string? faccion, bool? fuerzaSensitivo) =>
        {
            var query = GalacticContext.Personajes.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(faccion) && Enum.TryParse<Faccion>(faccion, true, out var f))
            {
                query = query.Where(p => p.Faccion == f);
            }

            if (fuerzaSensitivo.HasValue)
            {
                query = query.Where(p => p.FuerzaSensitivo == fuerzaSensitivo.Value);
            }

            var data = query.ToList();

            // SOLUCIÓN: Retornamos la lista 'data' directamente sin ApiResponse
            return Results.Ok(data);
        })
        .WithName("GetPersonajes")
        .WithSummary("Lista personajes con filtros opcionales por facción y fuerzaSensitivo")
        .Produces<List<Personaje>>(StatusCodes.Status200OK); // 💡 Ajustado el tipo esperado en Swagger


        // 2. GET /personajes/ranking?por=poder (Ranking de personajes según su carta)
        group.MapGet("/ranking", (string? por) =>
        {
            if (string.IsNullOrWhiteSpace(por) || !por.Equals("poder", StringComparison.OrdinalIgnoreCase))
            {
                return Results.BadRequest(new ApiErrorResponse(new ApiErrorBody(
                    "BAD_REQUEST",
                    "Criterio de ordenamiento no válido. Usa '?por=poder'",
                    new List<ErrorDetalle> { new("por", "Debe especificarse el valor 'poder'") }
                )));
            }

            var ranking = (from p in GalacticContext.Personajes
                           join c in GalacticContext.Cartas on p.Id equals c.PersonajeId
                           orderby c.Poder descending
                           select new
                           {
                               PersonajeId = p.Id,
                               Nombre = p.Nombre,
                               Faccion = p.Faccion.ToString(),
                               Poder = c.Poder,
                               CartaId = c.Id
                           }).ToList();

            return Results.Ok(new ApiResponse<object>(ranking, new PaginacionMeta(1, ranking.Count, ranking.Count)));
        })
        .WithName("GetRankingPersonajes")
        .WithSummary("Ordena personajes según el poder de su carta coleccionable")
        .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
        .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest);

        // 3. GET /personajes/{id}/eventos (Eventos relacionados con el personaje)
        group.MapGet("/{id:int}/eventos", (int id) =>
        {
            var personaje = GalacticContext.Personajes.FirstOrDefault(p => p.Id == id);
            if (personaje is null)
            {
                return Results.NotFound(new ApiErrorResponse(new ApiErrorBody("NOT_FOUND", "Personaje no encontrado")));
            }

            var eventos = GalacticContext.Eventos
                .Where(e => e.Participantes.Contains(id))
                .OrderBy(e => e.Fecha) // Orden cronológico ABY/BBY
                .ToList();

            return Results.Ok(new ApiResponse<List<Evento>>(eventos, new PaginacionMeta(1, eventos.Count, eventos.Count)));
        })
        .WithName("GetEventosByPersonaje")
        .WithSummary("Devuelve los eventos en los que participa o participó un personaje")
        .Produces<ApiResponse<List<Evento>>>(StatusCodes.Status200OK)
        .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound);

        // 4. GET /personajes/{id}
        group.MapGet("/{id:int}", (int id) =>
        {
            var personaje = GalacticContext.Personajes.FirstOrDefault(p => p.Id == id);
            return personaje is null
                ? Results.NotFound(new ApiErrorResponse(new ApiErrorBody("NOT_FOUND", "Personaje no encontrado")))
                : Results.Ok(new ApiResponse<Personaje>(personaje));
        })
        .WithName("GetPersonajeById")
        .WithSummary("Obtiene los detalles de un personaje por su ID")
        .Produces<ApiResponse<Personaje>>(StatusCodes.Status200OK)
        .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound);

        // 5. POST /personajes
        group.MapPost("/", (Personaje input) =>
        {
            var nuevo = input with { Id = GalacticContext.GenerarId(GalacticContext.Personajes, p => p.Id) };
            GalacticContext.Personajes.Add(nuevo);
            return Results.Created($"/api/personajes/{nuevo.Id}", new ApiResponse<Personaje>(nuevo));
        })
        .WithName("CreatePersonaje")
        .WithSummary("Crea un nuevo personaje")
        .Produces<ApiResponse<Personaje>>(StatusCodes.Status201Created);

        // 6. PUT /personajes/{id}
        group.MapPut("/{id:int}", (int id, Personaje input) =>
        {
            var index = GalacticContext.Personajes.FindIndex(p => p.Id == id);
            if (index == -1) return Results.NotFound(new ApiErrorResponse(new ApiErrorBody("NOT_FOUND", "Personaje no encontrado")));

            var actualizado = input with { Id = id };
            GalacticContext.Personajes[index] = actualizado;
            return Results.Ok(new ApiResponse<Personaje>(actualizado));
        })
        .WithName("UpdatePersonaje")
        .WithSummary("Actualiza los datos de un personaje")
        .Produces<ApiResponse<Personaje>>(StatusCodes.Status200OK)
        .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound);

        // 7. DELETE /personajes/{id}
        group.MapDelete("/{id:int}", (int id) =>
        {
            var eliminados = GalacticContext.Personajes.RemoveAll(p => p.Id == id);
            return eliminados > 0
                ? Results.NoContent()
                : Results.NotFound(new ApiErrorResponse(new ApiErrorBody("NOT_FOUND", "Personaje no encontrado")));
        })
        .WithName("DeletePersonaje")
        .WithSummary("Elimina un personaje del catálogo")
        .Produces(StatusCodes.Status204NoContent)
        .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound);

        // Tarea 2 - Ficha de combate: Combina Personaje con su Carta
        group.MapGet("/{id:int}/con-card", (int id) =>
        {
            var personaje = GalacticContext.Personajes.FirstOrDefault(p => p.Id == id);
            if (personaje is null)
            {
                return Results.NotFound(new { codigo = "NOT_FOUND", mensaje = "Personaje no encontrado" });
            }

            var card = GalacticContext.Cartas.FirstOrDefault(c => c.PersonajeId == id);

            var fichaCombate = new
            {
                Personaje = personaje,
                Carta = card,
                Card = card,   
                Stats = card   
            };

            return Results.Ok(fichaCombate);
        })
        .WithName("GetPersonajeConCard")
        .WithSummary("Obtiene la ficha de combate combinando datos del personaje y su carta")
        .Produces<object>(StatusCodes.Status200OK)
        .Produces<object>(StatusCodes.Status404NotFound);


    }
}
