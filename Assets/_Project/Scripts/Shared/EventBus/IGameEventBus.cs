using System;

namespace TaskBarFisher.Shared.EventBus
{
    /// <summary>
    /// Contrato del bus de eventos que desacopla los sistemas core (Economía, Gacha, Colección,
    /// Progresión/AFK, Guardado, Bounties) entre sí y de la UI. Ningún sistema referencia a otro
    /// directamente: publica/escucha eventos definidos en <see cref="TaskBarFisher.Shared.Contracts"/>.
    ///
    /// Es una interfaz (no una clase estática) a propósito: cada sistema recibe su instancia por
    /// <see cref="ServiceLocator.ServiceLocator"/>, así los tests pueden inyectar un bus limpio por
    /// caso sin estado global compartido entre ellos.
    /// </summary>
    public interface IGameEventBus
    {
        void Subscribe<TEvent>(Action<TEvent> handler);
        void Unsubscribe<TEvent>(Action<TEvent> handler);
        void Publish<TEvent>(TEvent evt);
    }
}
