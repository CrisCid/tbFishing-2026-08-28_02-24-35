using System.Collections.Generic;
using TaskBarFisher.Shared.Contracts;
using TaskBarFisher.Shared.EventBus;

namespace TaskBarFisher.Core.Economy
{
    /// <summary>
    /// Balances de moneda del jugador. Clase POCO; publica <see cref="CurrencyChangedEvent"/> por
    /// cada cambio para que la UI reaccione sin leer el diccionario interno directamente
    /// (game_design_overview.md §9, "UI desacoplada"). No decide de dónde sale el dinero (eso lo
    /// disparan Gacha/Bounty/futuro mercado); solo lleva el balance y lo valida.
    /// </summary>
    public class EconomySystem
    {
        readonly IGameEventBus _eventBus;
        readonly Dictionary<string, int> _balances = new();

        public EconomySystem(IGameEventBus eventBus) => _eventBus = eventBus;

        public int GetBalance(string currencyId) => _balances.GetValueOrDefault(currencyId, 0);

        public void Add(string currencyId, int amount)
        {
            if (amount < 0) throw new System.ArgumentOutOfRangeException(nameof(amount), "Usar Spend() para restar.");
            SetBalance(currencyId, GetBalance(currencyId) + amount, amount);
        }

        /// <returns>false si no había saldo suficiente (no se descuenta nada en ese caso).</returns>
        public bool Spend(string currencyId, int amount)
        {
            if (amount < 0) throw new System.ArgumentOutOfRangeException(nameof(amount), "Debe ser positivo.");
            var current = GetBalance(currencyId);
            if (current < amount) return false;

            SetBalance(currencyId, current - amount, -amount);
            return true;
        }

        void SetBalance(string currencyId, int newBalance, int delta)
        {
            _balances[currencyId] = newBalance;
            _eventBus.Publish(new CurrencyChangedEvent(currencyId, newBalance, delta));
        }
    }
}
