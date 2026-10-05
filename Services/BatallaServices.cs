using CatalogoGalactico.Data;
using CatalogoGalactico.Models;

namespace CatalogoGalactico.Services
{
    public interface IBatallaService
    {
        (string Ganador, int TotalRebelde, int TotalImperio, string Criterio) SimularBatalla(int eventoId);
        void RegistrarMuerte(int personajeId);
    }

    public class BatallaService : IBatallaService
    {
        public (string, int, int, string) SimularBatalla(int eventoId)
        {
            var evento = GalacticContext.Eventos.FirstOrDefault(e => e.Id == eventoId);
            if (evento == null || evento.Participantes.Count < 2)
                throw new InvalidOperationException("Faltan participantes");

            int poderRebelde = 0;
            int poderImperio = 0;

            foreach (var pId in evento.Participantes)
            {
                var personaje = GalacticContext.Personajes.FirstOrDefault(p => p.Id == pId);
                var carta = GalacticContext.Cartas.FirstOrDefault(c => c.PersonajeId == pId);

                if (personaje == null || carta == null || personaje.Estado == Estado.Muerto) continue;

                if (personaje.Faccion == Faccion.Rebelde) poderRebelde += carta.Poder;
                else if (personaje.Faccion == Faccion.Imperio) poderImperio += carta.Poder;
            }

            // Factor aleatorio acotado (-10% a +10%)
            var random = new Random();
            poderRebelde = (int)(poderRebelde * (1 + (random.NextDouble() * 0.2 - 0.1)));
            poderImperio = (int)(poderImperio * (1 + (random.NextDouble() * 0.2 - 0.1)));

            string ganador = poderRebelde >= poderImperio ? "Rebelde" : "Imperio";
            return (ganador, poderRebelde, poderImperio, "Suma de poder base + factor aleatorio 10%");
        }

        public void RegistrarMuerte(int personajeId)
        {
            var index = GalacticContext.Personajes.FindIndex(p => p.Id == personajeId);
            if (index != -1)
            {
                var p = GalacticContext.Personajes[index];
                // Reemplazo inmutable
                GalacticContext.Personajes[index] = p with { Estado = Estado.Muerto };
            }
        }
    }
}

