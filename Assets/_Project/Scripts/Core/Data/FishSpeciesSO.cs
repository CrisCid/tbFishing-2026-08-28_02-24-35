using TaskBarFisher.Shared.Contracts;
using UnityEngine;

namespace TaskBarFisher.Core.Data
{
    /// <summary>
    /// Definición de una especie de pez (game_design_overview.md §4). Datos de diseño editables sin
    /// tocar código -- ver skill unity-scriptableobjects. Los valores de peso/tamaño acá son
    /// placeholders de estructura; el balance real es trabajo de Fase 3, no de Fase 0.
    /// </summary>
    [CreateAssetMenu(fileName = "NewFishSpecies", menuName = "TaskBarFisher/Fish Species", order = 0)]
    public class FishSpeciesSO : ScriptableObject
    {
        [Tooltip("Id estable usado en eventos/guardado. No renombrar una vez usado en un save real.")]
        public string speciesId;

        public string displayName;
        public Rarity rarity;
        public Sprite icon;

        [Header("Rango (placeholder -- balance real en Fase 3)")]
        public float minWeightKg = 0.1f;
        public float maxWeightKg = 1f;
        public float minSizeCm = 5f;
        public float maxSizeCm = 30f;

        [Tooltip("Si esta especie puede salir en variante shiny (game_design_overview.md §4).")]
        public bool canBeShiny = true;
    }
}
