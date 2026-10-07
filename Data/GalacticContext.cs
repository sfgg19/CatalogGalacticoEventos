
using CatalogoGalactico.Models;

namespace CatalogoGalactico.Data
{
    public static class GalacticContext
    {
        public static List<Personaje> Personajes = new()
    {
        new(1, "Luke Skywalker", "Humano", Faccion.Rebelde, "Jedi", Estado.Vivo, true, "https://disney.images.edge.bamgrid.com/ripcut-delivery/v2/variant/disney/2BB6630FA1F41C1DF4BE34E84893CE2496534215BD92C291BC5228D85BA001E9/compose?format=webp&width=1920"),
        new(2, "Darth Vader", "Humano/Cyborg", Faccion.Imperio, "Sith", Estado.Vivo, true, "https://disney.images.edge.bamgrid.com/ripcut-delivery/v2/variant/disney/0C68420F47A554086E69B94C1E731DEABEBD41A47387F514DDF7454E345EEA73/compose?format=webp&width=1920"),
        new(3, "Han Solo", "Humano", Faccion.Rebelde, "Contrabandista", Estado.Vivo, false, "https://disney.images.edge.bamgrid.com/ripcut-delivery/v2/variant/disney/718F2ADDC24AFEE31402B43BC1BE064BE80E054FF4EB6AC42E913CC281F81FBA/compose?format=webp&width=1920"),
        new(4, "Boba Fett", "Clon", Faccion.Neutral, "Cazarrecompensas", Estado.Vivo, false, "https://disney.images.edge.bamgrid.com/ripcut-delivery/v2/variant/disney/D79E56C65C09B7C30D484801A2F4245DA84AF3B3C48C6385ACE123E950E2F608/compose?format=webp&width=1920"),
        new(5, "Emperador Palpatine", "Humano", Faccion.Imperio, "Sith", Estado.Vivo, true, "https://disney.images.edge.bamgrid.com/ripcut-delivery/v2/variant/disney/8AEA2921639F4101E99D2A4C826E87AB9357B4C55FC2E36FD65FB0EFE8902254/compose?format=webp&width=1920"),
        new(6, "Obi-Wan Kenobi", "Humano", Faccion.Rebelde, "Jedi", Estado.Muerto, true, "https://disney.images.edge.bamgrid.com/ripcut-delivery/v2/variant/disney/85880178035A8ED309FDC63CFA783887462CC89E7E1DA90212722225664E9495/compose?format=webp&width=1920")
    };

        public static List<CardPersonaje> Cartas = new()
    {
        new(1, 1, 90, "Concentración en la Fuerza", "Sable de luz verde", 9, "url_luke"),
        new(2, 2, 95, "Estrangulamiento", "Sable de luz rojo", 10, "url_vader"),
        new(3, 3, 75, "Disparo rápido", "Blaster pesado", 7, "url_han"),
        new(4, 4, 80, "Vuelo táctico", "Rifle Blaster y Jetpack", 8, "url_boba"),
        new(5, 5, 100, "Rayos de la Fuerza", "Poder Oscuro", 10, "url_palpatine"),
        new(6, 6, 88, "Defensa absoluta", "Sable de luz azul", 9, "url_obi")
    };

        public static List<Evento> Eventos = new()
    {
        new(1, "Batalla de Yavin", 0, "Yavin 4", "Destrucción Estrella de la Muerte", new List<int>{1, 2, 3}, "Rebelde"),
        new(2, "Batalla de Endor", 4, "Luna de Endor", "Caída del Imperio", new List<int>{1, 2, 3, 5}, "Rebelde"),
        new(3, "Duelo en Mustafar", -19, "Mustafar", "Nacimiento de Vader", new List<int>{2, 6}, "Imperio"),
        new(4, "Captura en Bespin", 3, "Ciudad de las Nubes", "Han Solo congelado", new List<int>{2, 3, 4}, "Imperio")
    };

        public static int GenerarId<T>(IEnumerable<T> lista, Func<T, int> selector) =>
            lista.Any() ? lista.Max(selector) + 1 : 1;
    }
}

