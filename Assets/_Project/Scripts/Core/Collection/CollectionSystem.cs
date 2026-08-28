using System.Collections.Generic;
using TaskBarFisher.Shared.Contracts;

namespace TaskBarFisher.Core.Collection
{
    /// <summary>Un pez individual en el bag del jugador. Sin duplicados exactos por diseño (game_design_overview.md §4).</summary>
    public readonly struct CaughtFish
    {
        public readonly string SpeciesId;
        public readonly Rarity Rarity;
        public readonly float Weight;
        public readonly float Size;
        public readonly bool IsShiny;

        public CaughtFish(string speciesId, Rarity rarity, float weight, float size, bool isShiny)
        {
            SpeciesId = speciesId;
            Rarity = rarity;
            Weight = weight;
            Size = size;
            IsShiny = isShiny;
        }
    }

    /// <summary>
    /// Colección/bag del jugador (game_design_overview.md §4). Clase POCO; se suscribe a
    /// <see cref="FishCaughtEvent"/> vía el bus -- no se llama directamente desde Gacha, para que
    /// Colección y Gacha no se referencien entre sí (el controller de gameplay de Fase 1 es quien
    /// conecta ambos publicando el evento).
    /// </summary>
    public class CollectionSystem
    {
        readonly List<CaughtFish> _caught = new();

        public IReadOnlyList<CaughtFish> All => _caught;

        public void HandleFishCaught(FishCaughtEvent evt)
        {
            _caught.Add(new CaughtFish(evt.SpeciesId, evt.Rarity, evt.Weight, evt.Size, evt.IsShiny));
        }

        public bool HasCaughtSpecies(string speciesId)
        {
            foreach (var fish in _caught)
                if (fish.SpeciesId == speciesId) return true;
            return false;
        }
    }
}
