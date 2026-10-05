namespace CatalogoGalactico.Models {
    public enum Faccion { Rebelde, Imperio, Neutral }
    public enum Estado { Vivo, Muerto, Desconocido }

    public record Personaje(int Id, string Nombre, string Especie, Faccion Faccion, string Afiliacion, Estado Estado, bool FuerzaSensitivo);

    public record CardPersonaje(int Id, int PersonajeId, int Poder, string HabilidadEspecial, string Arma, int NivelPeligrosidad, string ImagenUrl);

    public record Evento(int Id, string Nombre, int Fecha, string Ubicacion, string Descripcion, List<int> Participantes, string? ResultadoGanador);
}

