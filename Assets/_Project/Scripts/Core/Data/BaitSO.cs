using TaskBarFisher.Shared.Contracts;
using UnityEngine;

namespace TaskBarFisher.Core.Data
{
    /// <summary>Cebo consumible (game_design_overview.md §5: "cebos que aseguran cierta categoría de pez").</summary>
    [CreateAssetMenu(fileName = "NewBait", menuName = "TaskBarFisher/Bait", order = 3)]
    public class BaitSO : ScriptableObject
    {
        public string baitId;
        public string displayName;
        public Sprite icon;

        [Tooltip("Si está seteado, garantiza esta rareza en la próxima captura al usarse.")]
        public bool guaranteesRarity;
        public Rarity guaranteedRarity;
    }
}
