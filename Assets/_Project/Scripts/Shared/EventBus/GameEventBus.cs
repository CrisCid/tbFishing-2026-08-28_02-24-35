using System;
using System.Collections.Generic;

namespace TaskBarFisher.Shared.EventBus
{
    /// <summary>
    /// Implementación por defecto de <see cref="IGameEventBus"/>. Pub/sub simple, tipado por el
    /// tipo del evento (sin strings/IDs mágicos). No es MonoBehaviour ni singleton estático: se
    /// registra una única instancia en el <see cref="ServiceLocator.ServiceLocator"/> al arrancar
    /// el juego (o se instancia una nueva por test).
    /// </summary>
    public sealed class GameEventBus : IGameEventBus
    {
        readonly Dictionary<Type, Delegate> _handlers = new();

        public void Subscribe<TEvent>(Action<TEvent> handler)
        {
            var eventType = typeof(TEvent);
            _handlers[eventType] = _handlers.TryGetValue(eventType, out var existing)
                ? Delegate.Combine(existing, handler)
                : handler;
        }

        public void Unsubscribe<TEvent>(Action<TEvent> handler)
        {
            var eventType = typeof(TEvent);
            if (!_handlers.TryGetValue(eventType, out var existing)) return;

            var updated = Delegate.Remove(existing, handler);
            if (updated == null) _handlers.Remove(eventType);
            else _handlers[eventType] = updated;
        }

        public void Publish<TEvent>(TEvent evt)
        {
            if (_handlers.TryGetValue(typeof(TEvent), out var existing) && existing is Action<TEvent> handler)
                handler.Invoke(evt);
        }

        /// <summary>Quita todas las suscripciones. Llamar en shutdown/domain reload para no acumular listeners muertos.</summary>
        public void Clear() => _handlers.Clear();
    }
}
