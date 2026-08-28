using System;
using System.Collections.Generic;

namespace TaskBarFisher.Progression.Save
{
    /// <summary>
    /// Schema versionado del save (ver skill save-systems: "el día que shippeás un patch, todo save
    /// sin version es una adivinanza"). SIEMPRE agregar campos nuevos como opcionales y sumar una
    /// migración en <see cref="SaveSystem"/> en vez de romper el shape existente.
    /// </summary>
    [Serializable]
    public class SaveDataV1
    {
        public const int CurrentVersion = 1;

        public int version = CurrentVersion;

        // Timestamp UTC en ticks del último guardado -- base para el cálculo de progreso AFK/offline
        // al reabrir (game_design_overview.md §9: "manejo cuidadoso de timestamps").
        public long lastSavedUtcTicks;

        public List<CurrencyEntry> currencies = new();
        public List<string> caughtFishSpeciesIds = new();
        public float fuelRemaining01 = 1f;

        [Serializable]
        public class CurrencyEntry
        {
            public string currencyId;
            public int balance;
        }
    }
}
