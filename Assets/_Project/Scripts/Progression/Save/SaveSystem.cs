using System;
using System.IO;
using TaskBarFisher.Shared.Contracts;
using TaskBarFisher.Shared.EventBus;
using UnityEngine;

namespace TaskBarFisher.Progression.Save
{
    /// <summary>
    /// Guardado robusto desde el día uno (game_design_overview.md §9, marcado explícitamente como
    /// crítico por depender de él el cálculo de progreso AFK). Escritura atómica: serializa a un
    /// archivo temporal, y solo si terminó bien reemplaza el archivo real -- un crash a mitad de
    /// guardado deja el save viejo intacto, nunca uno corrupto a medias (ver skill save-systems).
    /// </summary>
    public class SaveSystem
    {
        readonly IGameEventBus _eventBus;
        readonly string _savePath;
        readonly string _backupPath;

        public SaveSystem(IGameEventBus eventBus, string savePath = null)
        {
            _eventBus = eventBus;
            _savePath = savePath ?? Path.Combine(Application.persistentDataPath, "save.json");
            _backupPath = _savePath + ".bak";
        }

        public void Save(SaveDataV1 data)
        {
            data.version = SaveDataV1.CurrentVersion;
            data.lastSavedUtcTicks = DateTime.UtcNow.Ticks;

            var json = JsonUtility.ToJson(data, prettyPrint: true);
            var tempPath = _savePath + ".tmp";

            File.WriteAllText(tempPath, json);

            // Windows no garantiza que un rename sobre un archivo existente sea atómico como en
            // POSIX -- por eso el .bak: si el reemplazo falla a mitad de camino, igual queda una
            // copia buena de la que recuperarse (skill save-systems, "Atomic, crash-safe write").
            if (File.Exists(_savePath))
                File.Copy(_savePath, _backupPath, overwrite: true);

            File.Delete(_savePath);
            File.Move(tempPath, _savePath);

            _eventBus.Publish(new SaveCompletedEvent(data.version));
        }

        /// <summary>Carga defensivamente: intenta el save principal, cae al backup si está corrupto, y si ambos fallan devuelve un save nuevo.</summary>
        public SaveDataV1 Load()
        {
            if (TryLoadFrom(_savePath, out var data)) return Migrate(data);
            if (TryLoadFrom(_backupPath, out data)) return Migrate(data);

            Debug.LogWarning("[SaveSystem] No se encontró un save válido ni backup. Arrancando estado nuevo.");
            return new SaveDataV1();
        }

        static bool TryLoadFrom(string path, out SaveDataV1 data)
        {
            data = null;
            if (!File.Exists(path)) return false;

            try
            {
                var json = File.ReadAllText(path);
                data = JsonUtility.FromJson<SaveDataV1>(json);
                return data != null;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveSystem] Save en '{path}' corrupto o ilegible: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Punto de extensión para migraciones futuras (skill save-systems: cadena v -> v+1). Con
        /// CurrentVersion = 1 no hay nada que migrar todavía; agregar un case por versión nueva.
        /// </summary>
        static SaveDataV1 Migrate(SaveDataV1 data)
        {
            if (data.version > SaveDataV1.CurrentVersion)
                throw new InvalidOperationException(
                    $"[SaveSystem] El save es de una versión ({data.version}) más nueva que este build ({SaveDataV1.CurrentVersion}).");

            // if (data.version < 2) data = MigrateV1ToV2(data);
            return data;
        }
    }
}
