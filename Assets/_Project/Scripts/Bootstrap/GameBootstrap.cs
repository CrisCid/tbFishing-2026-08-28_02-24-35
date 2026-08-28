using System;
using TaskBarFisher.Core.Collection;
using TaskBarFisher.Core.Economy;
using TaskBarFisher.Core.Gacha;
using TaskBarFisher.Progression.Bounty;
using TaskBarFisher.Progression.Idle;
using TaskBarFisher.Progression.Save;
using TaskBarFisher.Shared.Contracts;
using TaskBarFisher.Shared;
using TaskBarFisher.Shared.EventBus;
using UnityEngine;

namespace TaskBarFisher.Bootstrap
{
    /// <summary>
    /// Composition root: el ÚNICO lugar que instancia los sistemas core y los registra en el
    /// <see cref="ServiceLocator"/>, y el único que conecta eventos entre sistemas que no se
    /// conocen entre sí (ej. Gacha -> Collection, Gacha -> Bounty). Un objeto en la escena de
    /// bootstrap, ejecuta antes que cualquier otro script (Script Execution Order).
    ///
    /// Fase 0 = wiring de los sistemas ya definidos. A medida que cada módulo de Fase 1 sume
    /// sistemas nuevos, se registran acá -- este archivo es el que más previsiblemente van a tocar
    /// las 4 personas a la vez, así que los cambios acá conviene avisarlos en el sync (ver
    /// ROADMAP.md, "Forma de trabajar").
    /// </summary>
    public class GameBootstrap : MonoBehaviour
    {
        void Awake()
        {
            ServiceLocator.Clear();

            var eventBus = new GameEventBus();
            ServiceLocator.Register<IGameEventBus>(eventBus);

            var economy = new EconomySystem(eventBus);
            ServiceLocator.Register(economy);

            var gacha = new GachaSystem();
            ServiceLocator.Register(gacha);

            var collection = new CollectionSystem();
            ServiceLocator.Register(collection);
            eventBus.Subscribe<FishCaughtEvent>(collection.HandleFishCaught);

            var saveSystem = new SaveSystem(eventBus);
            ServiceLocator.Register(saveSystem);

            var loadedSave = saveSystem.Load();
            var idleProgress = new IdleProgressSystem(eventBus, loadedSave.fuelRemaining01);
            ServiceLocator.Register(idleProgress);

            var bounty = new BountySystem(eventBus);
            ServiceLocator.Register(bounty);
            eventBus.Subscribe<FishCaughtEvent>(bounty.HandleFishCaught);

            // Progreso mientras el juego estaba cerrado, calculado desde el último save.
            var lastSaved = new DateTime(loadedSave.lastSavedUtcTicks == 0
                ? DateTime.UtcNow.Ticks
                : loadedSave.lastSavedUtcTicks, DateTimeKind.Utc);
            idleProgress.ApplyElapsedTime(DateTime.UtcNow - lastSaved);

            Debug.Log("[GameBootstrap] Sistemas core registrados en ServiceLocator.");
        }

        void OnApplicationQuit()
        {
            if (!ServiceLocator.TryResolve<SaveSystem>(out var saveSystem)) return;
            if (!ServiceLocator.TryResolve<IdleProgressSystem>(out var idleProgress)) return;
            if (!ServiceLocator.TryResolve<EconomySystem>(out var economy)) return;
            if (!ServiceLocator.TryResolve<CollectionSystem>(out var collection)) return;

            var data = new SaveDataV1 { fuelRemaining01 = idleProgress.FuelRemaining01 };
            foreach (var fish in collection.All)
                data.caughtFishSpeciesIds.Add(fish.SpeciesId);

            // TODO Fase 1 (Módulo B): iterar las monedas realmente en juego en vez de una lista fija.
            data.currencies.Add(new SaveDataV1.CurrencyEntry { currencyId = "soft", balance = economy.GetBalance("soft") });

            saveSystem.Save(data);
        }
    }
}
