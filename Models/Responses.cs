namespace CatalogoGalactico.Models
{
    public record PaginacionMeta(int Page, int Limit, int Total);

    public record ApiResponse<T>(T Data, PaginacionMeta? Meta = null);

    public record ErrorDetalle(string Campo, string Mensaje);

    public record ApiErrorBody(string Codigo, string Mensaje, List<ErrorDetalle>? Detalles = null);

    public record ApiErrorResponse(ApiErrorBody Error);
}

