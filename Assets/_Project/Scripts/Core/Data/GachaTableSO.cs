using System;
using UnityEngine;

namespace TaskBarFisher.Core.Data
{
    /// <summary>
    /// Tabla de probabilidades ponderadas por especie, consumida por <see cref="Gacha.GachaSystem"/>.
    /// Un asset por "zona" de pesca (o uno global para el MVP). Los pesos de ejemplo son estructura,
    /// no balance final -- ese ajuste es Fase 3 (game_design_overview.md §10, riesgo de balance).
    /// </summary>
    [CreateAssetMenu(fileName = "NewGachaTable", menuName = "TaskBarFisher/Gacha Table", order = 1)]
    public class GachaTableSO : ScriptableObject
    {
        [Serializable]
        public struct Entry
        {
            public FishSpeciesSO species;
            [Min(0f)] public float weight;
        }

        public Entry[] entries = Array.Empty<Entry>();

        [Header("Placeholder -- balance real en Fase 3")]
        [Range(0f, 1f)] public float baseShinyChance = 0.005f;
    }
}
