
using CatalogoGalactico.Data;
using CatalogoGalactico.Models;

namespace CatalogoGalactico.Endpoints
{
    public static class PersonajeEndpoints
    {
        public static void MapPersonajeEndpoints(this RouteGroupBuilder api)
        {
            var group = api.MapGroup("/personajes").WithTags("Personajes");

            group.MapGet("/", () =>
            {
                var data = GalacticContext.Personajes;
                return Results.Ok(new ApiResponse<List<Personaje>>(data, new PaginacionMeta(1, data.Count, data.Count)));
            })
            .WithName("GetPersonajes")
            .WithSummary("Lista todos los personajes registrados")
            .Produces<ApiResponse<List<Personaje>>>(StatusCodes.Status200OK);

            group.MapGet("/{id:int}", (int id) =>
            {
                var personaje = GalacticContext.Personajes.FirstOrDefault(p => p.Id == id);
                return personaje is null
                    ? Results.NotFound(new ApiErrorResponse(new ApiErrorBody("NOT_FOUND", "Personaje no encontrado")))
                    : Results.Ok(new ApiResponse<Personaje>(personaje));
            })
            .WithName("GetPersonajeById")
            .WithSummary("Obtiene los detalles de un personaje específico por su ID")
            .Produces<ApiResponse<Personaje>>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound);

            group.MapPost("/", (Personaje input) =>
            {
                var nuevo = input with { Id = GalacticContext.GenerarId(GalacticContext.Personajes, p => p.Id) };
                GalacticContext.Personajes.Add(nuevo);
                return Results.Created($"/api/personajes/{nuevo.Id}", new ApiResponse<Personaje>(nuevo));
            })
            .WithName("CreatePersonaje")
            .WithSummary("Crea un nuevo personaje y lo añade al catálogo")
            .Produces<ApiResponse<Personaje>>(StatusCodes.Status201Created)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest);

            group.MapPut("/{id:int}", (int id, Personaje input) =>
            {
                var index = GalacticContext.Personajes.FindIndex(p => p.Id == id);
                if (index == -1) return Results.NotFound(new ApiErrorResponse(new ApiErrorBody("NOT_FOUND", "Personaje no encontrado")));

                var actualizado = input with { Id = id };
                GalacticContext.Personajes[index] = actualizado;
                return Results.Ok(new ApiResponse<Personaje>(actualizado));
            })
            .WithName("UpdatePersonaje")
            .WithSummary("Actualiza todos los datos de un personaje existente")
            .Produces<ApiResponse<Personaje>>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound);

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
        }
    }
}
