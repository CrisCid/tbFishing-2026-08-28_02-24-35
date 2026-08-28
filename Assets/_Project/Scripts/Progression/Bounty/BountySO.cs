using TaskBarFisher.Core.Data;
using UnityEngine;

namespace TaskBarFisher.Progression.Bounty
{
    /// <summary>
    /// Definición de una bounty (game_design_overview.md §4: "pez payaso entre 2.1 y 2.5 kg").
    /// Para el MVP solo hace falta el tablón diario simple, sin escalado por nivel (game_design_overview.md §6).
    /// </summary>
    [CreateAssetMenu(fileName = "NewBounty", menuName = "TaskBarFisher/Bounty", order = 4)]
    public class BountySO : ScriptableObject
    {
        public string bountyId;
        public string description;

        public FishSpeciesSO requiredSpecies;
        public float minWeightKg;
        public float maxWeightKg;

        public string rewardCurrencyId = "soft";
        public int rewardAmount = 100;
    }
}
