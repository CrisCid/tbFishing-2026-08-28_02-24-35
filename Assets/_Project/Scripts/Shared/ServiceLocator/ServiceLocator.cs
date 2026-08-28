using System;
using System.Collections.Generic;

namespace TaskBarFisher.Shared
{
    /// <summary>
    /// Resolución liviana de sistemas core (Economía, Gacha, Colección, Progresión/AFK, Guardado,
    /// Bounties, el bus de eventos, el overlay nativo) sin recurrir a singletons de MonoBehaviour
    /// desordenados ni a un framework de DI pesado (decisión registrada en game_design_overview.md §9:
    /// no se justifica Zenject/VContainer mientras el equipo sea chico).
    ///
    /// Se registra un único composition root al arrancar el juego (ver Fase 1: el módulo que arme la
    /// escena de bootstrap). Cada módulo (Core/Progression/Platform/UI) SOLO conoce las interfaces que
    /// resuelve aquí, nunca las clases concretas de otro módulo.
    /// </summary>
    public static class ServiceLocator
    {
        static readonly Dictionary<Type, object> _services = new();

        public static void Register<TService>(TService instance)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            _services[typeof(TService)] = instance;
        }

        public static TService Resolve<TService>()
        {
            if (_services.TryGetValue(typeof(TService), out var service))
                return (TService)service;

            throw new InvalidOperationException(
                $"[ServiceLocator] No hay ningún servicio registrado para {typeof(TService).Name}. " +
                "¿Falta registrarlo en el bootstrap de la escena?");
        }

        public static bool TryResolve<TService>(out TService service)
        {
            if (_services.TryGetValue(typeof(TService), out var found))
            {
                service = (TService)found;
                return true;
            }

            service = default;
            return false;
        }

        /// <summary>Limpia todos los registros. Usar en tests (setup/teardown por caso) y en domain reload.</summary>
        public static void Clear() => _services.Clear();
    }
}
