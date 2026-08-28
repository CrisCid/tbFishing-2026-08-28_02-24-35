using TaskBarFisher.Shared.Contracts;
using TaskBarFisher.Shared.EventBus;

namespace TaskBarFisher.Progression.Bounty
{
    /// <summary>
    /// Tablón de bounties, versión MVP: una activa por slot, sin escalado por nivel (eso es Fase 3,
    /// game_design_overview.md §4 "más espacios según nivel"). Se suscribe a
    /// <see cref="FishCaughtEvent"/> y publica <see cref="BountyCompletedEvent"/> cuando una captura
    /// cumple la condición -- nunca consulta directamente a Collection/Gacha.
    /// </summary>
    public class BountySystem
    {
        readonly IGameEventBus _eventBus;
        BountySO _activeBounty;

        public BountySystem(IGameEventBus eventBus) => _eventBus = eventBus;

        public void SetActiveBounty(BountySO bounty) => _activeBounty = bounty;

        public void HandleFishCaught(FishCaughtEvent evt)
        {
            if (_activeBounty == null) return;
            if (_activeBounty.requiredSpecies == null) return;
            if (evt.SpeciesId != _activeBounty.requiredSpecies.speciesId) return;
            if (evt.Weight < _activeBounty.minWeightKg || evt.Weight > _activeBounty.maxWeightKg) return;

            _eventBus.Publish(new BountyCompletedEvent(_activeBounty.bountyId, _activeBounty.rewardAmount));
            _activeBounty = null;
        }
    }
}
