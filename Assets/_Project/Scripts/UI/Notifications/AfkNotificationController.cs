using TaskBarFisher.Shared.Contracts;
using TaskBarFisher.Shared.EventBus;
using UnityEngine;

namespace TaskBarFisher.UI.Notifications
{
    /// <summary>
    /// Notificación de "mientras no estabas" al reabrir el juego (game_design_overview.md §4).
    /// Igual que <c>CollectionScreenController</c>: solo escucha el bus, nunca lee
    /// <c>Progression.Idle.IdleProgressSystem</c> directamente.
    /// </summary>
    public class AfkNotificationController : MonoBehaviour
    {
        IGameEventBus _eventBus;

        void Start()
        {
            if (!ServiceLocatorAccess.TryGetEventBus(out _eventBus)) return;
            _eventBus.Subscribe<AfkProgressUpdatedEvent>(OnAfkProgressUpdated);
        }

        void OnDestroy() => _eventBus?.Unsubscribe<AfkProgressUpdatedEvent>(OnAfkProgressUpdated);

        void OnAfkProgressUpdated(AfkProgressUpdatedEvent evt)
        {
            // TODO Fase 1 (Módulo D): mostrar el panel real de resumen AFK.
            Debug.Log($"[AfkNotificationController] Estuviste afuera {evt.TimeAway}. Combustible: {evt.FuelRemaining01:P0}. Capturas: {evt.CatchesWhileAway}.");
        }
    }
}
