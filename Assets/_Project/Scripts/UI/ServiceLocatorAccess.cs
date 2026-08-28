using TaskBarFisher.Shared.EventBus;
using UnityEngine;

namespace TaskBarFisher.UI
{
    /// <summary>
    /// Único punto donde la UI toca el ServiceLocator, para no repetir el try/resolve + log en cada
    /// controller. Los controllers de UI solo deberían pedir <see cref="IGameEventBus"/> por acá --
    /// nunca resolver sistemas de Core/Progression directamente (eso rompería el desacople).
    /// </summary>
    internal static class ServiceLocatorAccess
    {
        public static bool TryGetEventBus(out IGameEventBus eventBus)
        {
            if (Shared.ServiceLocator.TryResolve(out eventBus)) return true;

            Debug.LogWarning("[UI] IGameEventBus no está registrado todavía. ¿Falta el bootstrap de la escena?");
            return false;
        }
    }
}
