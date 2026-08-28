using UnityEngine;

namespace TaskBarFisher.Core.Data
{
    /// <summary>Caña equipable (game_design_overview.md §5, ítem de mejora / potencial de mercado a futuro).</summary>
    [CreateAssetMenu(fileName = "NewRod", menuName = "TaskBarFisher/Rod", order = 2)]
    public class RodSO : ScriptableObject
    {
        public string rodId;
        public string displayName;
        public Sprite icon;

        [Header("Placeholder -- balance real en Fase 3")]
        [Tooltip("Multiplicador sobre la probabilidad base de rareza alta.")]
        public float rareChanceMultiplier = 1f;
        public float catchSpeedMultiplier = 1f;
    }
}
