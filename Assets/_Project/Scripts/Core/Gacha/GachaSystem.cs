using System;
using TaskBarFisher.Core.Data;
using TaskBarFisher.Shared.Contracts;

namespace TaskBarFisher.Core.Gacha
{
    /// <summary>Resultado puro de una tirada, sin publicar todavía al event bus (ver <see cref="GachaSystem"/>).</summary>
    public readonly struct CatchResult
    {
        public readonly string SpeciesId;
        public readonly Rarity Rarity;
        public readonly float Weight;
        public readonly float Size;
        public readonly bool IsShiny;

        public CatchResult(string speciesId, Rarity rarity, float weight, float size, bool isShiny)
        {
            SpeciesId = speciesId;
            Rarity = rarity;
            Weight = weight;
            Size = size;
            IsShiny = isShiny;
        }
    }

    /// <summary>
    /// Sistema de rareza/gacha (game_design_overview.md §4 y §9). Clase POCO pura -- sin
    /// MonoBehaviour, sin tocar UnityEngine.Random para que sea determinista y testeable con un
    /// <see cref="Random"/> inyectado. Deliberadamente NO publica <see cref="FishCaughtEvent"/>: eso
    /// lo hace el controller de gameplay (Fase 1, Módulo A) que decide cuándo se pesca -- la mecánica
    /// de captura en sí todavía no está definida (game_design_overview.md §4, "por definir").
    /// </summary>
    public class GachaSystem
    {
        readonly Random _random;

        public GachaSystem(Random random = null) => _random = random ?? new Random();

        /// <summary>Tira una captura contra la tabla dada. Lanza si la tabla está vacía o mal formada.</summary>
        public CatchResult Roll(GachaTableSO table)
        {
            if (table == null || table.entries == null || table.entries.Length == 0)
                throw new ArgumentException("[GachaSystem] La tabla no tiene entradas.", nameof(table));

            var totalWeight = 0f;
            foreach (var entry in table.entries) totalWeight += entry.weight;
            if (totalWeight <= 0f)
                throw new ArgumentException("[GachaSystem] El peso total de la tabla es 0.", nameof(table));

            var roll = (float)(_random.NextDouble() * totalWeight);
            var accumulated = 0f;
            var species = table.entries[^1].species; // fallback: última entrada por redondeo de floats

            foreach (var entry in table.entries)
            {
                accumulated += entry.weight;
                if (roll <= accumulated)
                {
                    species = entry.species;
                    break;
                }
            }

            var weight = Lerp(_random, species.minWeightKg, species.maxWeightKg);
            var size = Lerp(_random, species.minSizeCm, species.maxSizeCm);
            var isShiny = species.canBeShiny && _random.NextDouble() < table.baseShinyChance;

            return new CatchResult(species.speciesId, species.rarity, weight, size, isShiny);
        }

        static float Lerp(Random random, float min, float max) => min + (float)random.NextDouble() * (max - min);
    }
}
