namespace TaskBarFisher.Shared.Contracts
{
    /// <summary>
    /// Rareza de captura (game_design_overview.md §4: "categorías de peces tipo cartas").
    /// Vive en Shared -- no en Core -- porque tanto Progression (bounties, buffs de pecera) como UI
    /// (colores/badges de rareza) necesitan el tipo sin depender del módulo Core completo.
    /// </summary>
    public enum Rarity
    {
        R = 0,
        UR = 1,
        SSR = 2,
    }
}
