using System;
using TaskBarFisher.Shared.Contracts;
using TaskBarFisher.Shared.EventBus;

namespace TaskBarFisher.Progression.Idle
{
    /// <summary>
    /// Combustible / progreso AFK (game_design_overview.md §4). Clase POCO, sin acoplarse a
    /// UnityEngine.Time para que el cálculo "cuánto pasó mientras no estabas" sea testeable con
    /// timestamps controlados -- ese cálculo es justo el que game_design_overview.md §9 marca como
    /// caso límite delicado (cambios de hora del sistema, cierres abruptos).
    ///
    /// Fase 0 = esqueleto de la mecánica de recarga; CUÁNTAS capturas AFK caen y con qué rareza
    /// depende de GachaSystem/GachaTableSO y es trabajo de implementación de Fase 1 (Módulo B),
    /// no de esta clase.
    /// </summary>
    public class IdleProgressSystem
    {
        readonly IGameEventBus _eventBus;

        // 10%/hora -- placeholder de estructura, el balance real es trabajo de Fase 3.
        public float RechargeRatePerHour { get; set; } = 0.1f;

        float _fuelRemaining01;

        public IdleProgressSystem(IGameEventBus eventBus, float initialFuel01 = 1f)
        {
            _eventBus = eventBus;
            _fuelRemaining01 = Clamp01(initialFuel01);
        }

        public float FuelRemaining01 => _fuelRemaining01;

        /// <summary>
        /// Recalcula el combustible recargado durante `timeAway` y publica el resultado. Se llama
        /// al reabrir el juego (con el timestamp del último save) y opcionalmente en cada tick
        /// mientras está abierto.
        /// </summary>
        public void ApplyElapsedTime(TimeSpan timeAway)
        {
            var recharged = (float)(timeAway.TotalHours * RechargeRatePerHour);
            _fuelRemaining01 = Clamp01(_fuelRemaining01 + recharged);

            // El número de capturas AFK resueltas mientras el jugador estaba afuera es contenido de
            // gameplay (depende de GachaSystem) -- acá solo se resuelve el combustible. El
            // controller de Fase 1 que orquesta esto es quien completa CatchesWhileAway antes de
            // publicar, o publica un evento separado por cada captura resuelta.
            _eventBus.Publish(new AfkProgressUpdatedEvent(_fuelRemaining01, catchesWhileAway: 0, timeAway));
        }

        public bool TrySpendFuel(float amount01)
        {
            if (amount01 > _fuelRemaining01) return false;
            _fuelRemaining01 = Clamp01(_fuelRemaining01 - amount01);
            return true;
        }

        static float Clamp01(float v) => v < 0f ? 0f : v > 1f ? 1f : v;
    }
}
