using TaskBarFisher.Shared.Contracts;
using TaskBarFisher.Shared.EventBus;
using UnityEngine;

namespace TaskBarFisher.UI.Collection
{
    /// <summary>
    /// Pantalla de colección/bag (game_design_overview.md §4). Se suscribe SOLO a eventos del bus
    /// -- nunca lee el estado interno de <c>Core.Collection.CollectionSystem</c> directamente
    /// (game_design_overview.md §9: "UI desacoplada"). Fase 0 = wiring de suscripción; el layout/
    /// visual real es contenido de Fase 1, Módulo D, y puede arrancar con datos mock antes de que
    /// Core esté terminado, precisamente porque el contrato de evento ya está congelado.
    /// </summary>
    public class CollectionScreenController : MonoBehaviour
    {
        IGameEventBus _eventBus;

        void Start()
        {
            if (!ServiceLocatorAccess.TryGetEventBus(out _eventBus)) return;
            _eventBus.Subscribe<FishCaughtEvent>(OnFishCaught);
        }

        void OnDestroy() => _eventBus?.Unsubscribe<FishCaughtEvent>(OnFishCaught);

        void OnFishCaught(FishCaughtEvent evt)
        {
            // TODO Fase 1 (Módulo D): instanciar la entrada visual del pez en el bag.
            Debug.Log($"[CollectionScreenController] Nueva captura: {evt.SpeciesId} ({evt.Rarity}){(evt.IsShiny ? " ✨shiny" : "")}");
        }
    }
}
