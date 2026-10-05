using CatalogoGalactico.Data;
using CatalogoGalactico.Models;

namespace CatalogoGalactico.Endpoints
{
    public static class CartaEndpoints
    {
        public static void MapCartaEndpoints(this RouteGroupBuilder api)
        {
            var group = api.MapGroup("/cartas").WithTags("Cartas");

            group.MapGet("/", () =>
            {
                var data = GalacticContext.Cartas;
                return Results.Ok(new ApiResponse<List<CardPersonaje>>(data, new PaginacionMeta(1, data.Count, data.Count)));
            })
            .WithName("GetCartas")
            .WithSummary("Lista todas las cartas coleccionables")
            .Produces<ApiResponse<List<CardPersonaje>>>(StatusCodes.Status200OK);

            group.MapGet("/{id:int}", (int id) =>
            {
                var carta = GalacticContext.Cartas.FirstOrDefault(c => c.Id == id);
                return carta is null
                    ? Results.NotFound(new ApiErrorResponse(new ApiErrorBody("NOT_FOUND", "Carta no encontrada")))
                    : Results.Ok(new ApiResponse<CardPersonaje>(carta));
            })
            .WithName("GetCartaById")
            .WithSummary("Obtiene una carta coleccionable por su ID")
            .Produces<ApiResponse<CardPersonaje>>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound);

            group.MapPost("/", (CardPersonaje input) =>
            {
                var nueva = input with { Id = GalacticContext.GenerarId(GalacticContext.Cartas, c => c.Id) };
                GalacticContext.Cartas.Add(nueva);
                return Results.Created($"/api/cartas/{nueva.Id}", new ApiResponse<CardPersonaje>(nueva));
            })
            .WithName("CreateCarta")
            .WithSummary("Crea una nueva carta y la asocia a un personaje")
            .Produces<ApiResponse<CardPersonaje>>(StatusCodes.Status201Created)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest);

            group.MapPut("/{id:int}", (int id, CardPersonaje input) =>
            {
                var index = GalacticContext.Cartas.FindIndex(c => c.Id == id);
                if (index == -1) return Results.NotFound(new ApiErrorResponse(new ApiErrorBody("NOT_FOUND", "Carta no encontrada")));

                var actualizada = input with { Id = id };
                GalacticContext.Cartas[index] = actualizada;
                return Results.Ok(new ApiResponse<CardPersonaje>(actualizada));
            })
            .WithName("UpdateCarta")
            .WithSummary("Actualiza los atributos de una carta existente")
            .Produces<ApiResponse<CardPersonaje>>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound);

            group.MapDelete("/{id:int}", (int id) =>
            {
                var eliminados = GalacticContext.Cartas.RemoveAll(c => c.Id == id);
                return eliminados > 0
                    ? Results.NoContent()
                    : Results.NotFound(new ApiErrorResponse(new ApiErrorBody("NOT_FOUND", "Carta no encontrada")));
            })
            .WithName("DeleteCarta")
            .WithSummary("Elimina una carta del sistema")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound);
        }
    }
}
