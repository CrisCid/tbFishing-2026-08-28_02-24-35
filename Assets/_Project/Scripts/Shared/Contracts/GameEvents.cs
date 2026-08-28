using System;

namespace TaskBarFisher.Shared.Contracts
{
    // -----------------------------------------------------------------------------------------
    // CONTRATO DE INTEGRACIÓN -- FASE 0 (congelado para arrancar la Fase 1 en paralelo)
    //
    // Este archivo es la única fuente de verdad de qué eventos existen y qué forma tienen.
    // Un cambio acá afecta potencialmente a los 4 módulos a la vez: revisarlo en el sync de equipo
    // (ver ROADMAP.md, "Forma de trabajar"), no cambiarlo por libre dentro de un solo módulo.
    //
    // Cubre únicamente el alcance del MVP (game_design_overview.md §6): pesca simple, rareza,
    // bag/colección, AFK con combustible, bounties diarias. Mercado de Steam y sistemas de Fase 3+
    // se agregan como contratos nuevos cuando toque esa fase, no se anticipan acá.
    // -----------------------------------------------------------------------------------------

    /// <summary>Publicado por Core.Gacha cuando se resuelve una captura (manual o AFK).</summary>
    public readonly struct FishCaughtEvent
    {
        public readonly string SpeciesId;
        public readonly Rarity Rarity;
        public readonly float Weight;
        public readonly float Size;
        public readonly bool IsShiny;
        public readonly bool WasAfk;

        public FishCaughtEvent(string speciesId, Rarity rarity, float weight, float size, bool isShiny, bool wasAfk)
        {
            SpeciesId = speciesId;
            Rarity = rarity;
            Weight = weight;
            Size = size;
            IsShiny = isShiny;
            WasAfk = wasAfk;
        }
    }

    /// <summary>Publicado por Core.Economy cada vez que cambia un balance de moneda (UI reacciona, no lee estado interno).</summary>
    public readonly struct CurrencyChangedEvent
    {
        public readonly string CurrencyId;
        public readonly int NewBalance;
        public readonly int Delta;

        public CurrencyChangedEvent(string currencyId, int newBalance, int delta)
        {
            CurrencyId = currencyId;
            NewBalance = newBalance;
            Delta = delta;
        }
    }

    /// <summary>
    /// Publicado por Progression.Idle cuando se recalcula el progreso AFK: al reabrir el juego
    /// (offline progress) o en cada tick de recarga de combustible mientras está abierto.
    /// </summary>
    public readonly struct AfkProgressUpdatedEvent
    {
        public readonly float FuelRemaining01;   // normalizado 0..1
        public readonly int CatchesWhileAway;
        public readonly TimeSpan TimeAway;

        public AfkProgressUpdatedEvent(float fuelRemaining01, int catchesWhileAway, TimeSpan timeAway)
        {
            FuelRemaining01 = fuelRemaining01;
            CatchesWhileAway = catchesWhileAway;
            TimeAway = timeAway;
        }
    }

    /// <summary>Publicado por Progression.Bounty cuando se cumple la condición de una bounty activa.</summary>
    public readonly struct BountyCompletedEvent
    {
        public readonly string BountyId;
        public readonly int RewardCurrencyAmount;

        public BountyCompletedEvent(string bountyId, int rewardCurrencyAmount)
        {
            BountyId = bountyId;
            RewardCurrencyAmount = rewardCurrencyAmount;
        }
    }

    /// <summary>Solicita a Progression.Save que persista el estado actual (ej. al cerrar, o en un autosave periódico).</summary>
    public readonly struct SaveRequestedEvent { }

    /// <summary>Publicado por Progression.Save al terminar un guardado exitoso.</summary>
    public readonly struct SaveCompletedEvent
    {
        public readonly int SchemaVersion;
        public SaveCompletedEvent(int schemaVersion) => SchemaVersion = schemaVersion;
    }
}
