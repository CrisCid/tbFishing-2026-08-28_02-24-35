# CLAUDE.md — TaskBar Fisher

Convenciones del proyecto para cualquier sesión de Claude (o persona) que trabaje en este repo.
Leer esto antes de tocar código. El contexto de diseño completo vive en `game_design_overview.md`
y el plan de fases/equipo en `ROADMAP.md` — este archivo es solo la capa técnica de "cómo escribimos
código acá", no repite las decisiones de diseño.

## Qué es este proyecto

Unity 6 (URP 2D), overlay de escritorio para Windows, juego de pesca idle/gacha/collectathon con
mercado de Steam como monetización a futuro (fuera del MVP). Detalle completo en
`game_design_overview.md`.

## Skills del proyecto

Este repo trae skills instaladas en `.claude/skills/` (gestionadas con `npx skills`, ver
`.githooks/README.md`). Consultarlas *antes* de escribir el tipo de código que cubren:

| Skill | Cuándo leerla |
|---|---|
| `unity-scriptableobjects` | Antes de crear o tocar cualquier `*SO.cs` (datos, event channels). |
| `unity-csharp-scripting` | Antes de escribir cualquier `MonoBehaviour` nuevo (lifecycle, serialización). |
| `unity-package-management` | Antes de agregar/quitar un paquete de Unity. **Nunca** editar `Packages/manifest.json` a mano. |
| `save-systems` | Antes de tocar `Progression/Save/`. |
| `unity-cli` | Instalar/abrir el Editor, crear proyectos, builds headless — no hace falta para desarrollo normal día a día. |
| `new-unity-project` | Ya no aplica (el proyecto existe) — referencia si algún día se crea un proyecto hermano (ej. cliente de terminal). |

Si al abrir el Editor `.claude/skills/` no aparece cargado, correr `npx skills` o revisar
`.githooks/README.md` — no debería hacer falta, se commitean como copias reales.

## Arquitectura (resumen — el detalle de decisión está en `game_design_overview.md` §9)

Capas + eventos. **Nada** de un módulo llama directo a una clase concreta de otro módulo: todo pasa
por `IGameEventBus` (pub/sub tipado) o por `ServiceLocator` (resolución de sistemas). Los sistemas
core son clases C# puras (POCO) sin `MonoBehaviour`, para poder testearlos sin correr el Editor.

### Mapa de Assembly Definitions

```
TaskBarFisher.Shared        Assets/_Project/Scripts/Shared/
  ↳ EventBus (IGameEventBus, GameEventBus)
  ↳ ServiceLocator
  ↳ Contracts (Rarity, todos los payloads de evento — GameEvents.cs)
  Sin dependencias. noEngineReferences=true: no puede usar UnityEngine, a propósito.

TaskBarFisher.Core          Assets/_Project/Scripts/Core/     → depende de Shared
  ↳ Economy/EconomySystem
  ↳ Gacha/GachaSystem
  ↳ Collection/CollectionSystem
  ↳ Data/ (FishSpeciesSO, GachaTableSO, RodSO, BaitSO)

TaskBarFisher.Progression    Assets/_Project/Scripts/Progression/  → depende de Shared, Core
  ↳ Idle/IdleProgressSystem  (combustible / cálculo AFK-offline)
  ↳ Save/ (SaveSystem, SaveDataV1 — guardado atómico y versionado)
  ↳ Bounty/ (BountySystem, BountySO)

TaskBarFisher.Platform       Assets/_Project/Scripts/Platform/     → depende de Shared
  ↳ DesktopOverlay/ (NativeWindow: P/Invoke; DesktopOverlayController: MonoBehaviour)
  Todo dentro de #if UNITY_STANDALONE_WIN && !UNITY_EDITOR salvo el wrapper MonoBehaviour, que
  degrada a ventana normal en Editor/otras plataformas.

TaskBarFisher.UI             Assets/_Project/Scripts/UI/           → depende de Shared, Core, Progression*
  ↳ Collection/, Notifications/
  *La referencia a Core/Progression es SOLO para los tipos de datos que exponen los eventos
  (Rarity, etc.) vía Shared.Contracts — un controller de UI nunca debe llamar un método de
  EconomySystem/GachaSystem directamente, solo suscribirse a eventos.

TaskBarFisher.Bootstrap      Assets/_Project/Scripts/Bootstrap/    → depende de Shared, Core, Progression
  ↳ GameBootstrap: composition root. Instancia y registra todos los sistemas en el
    ServiceLocator, conecta los eventos entre sistemas que no se conocen entre sí. Es el único
    archivo que "conoce" todos los módulos a la vez — cambios acá, avisar en el sync de equipo.

TaskBarFisher.Tests.EditMode Assets/_Project/Tests/EditMode/       → depende de Shared, Core, Progression
```

### Contrato de eventos

`Assets/_Project/Scripts/Shared/Contracts/GameEvents.cs` es la única fuente de verdad de qué
eventos existen y qué forma tienen. Cubre solo el alcance del MVP (`game_design_overview.md` §6).
Cambiar la forma de un evento ahí afecta potencialmente a los 4 módulos — avisar en el sync antes
de romperlo, no cambiarlo por libre dentro de un solo módulo.

## Convenciones de código

- `[SerializeField] private` para tunables en el Inspector, nunca `public` (ver skill
  `unity-csharp-scripting`).
- Cachear `GetComponent` en `Awake`, nunca en `Update`.
- Física en `FixedUpdate`, no en `Update`.
- Todo dato de diseño (especies, cañas, cebos, bounties, tablas de gacha) es un `ScriptableObject`
  con `[CreateAssetMenu]`, nunca hardcodeado en una clase.
- Un sistema core es POCO: si necesita `UnityEngine`, es una señal de que esa lógica pertenece a un
  `MonoBehaviour` delgado que envuelve al sistema, no al sistema mismo.
- Los valores numéricos de balance (probabilidades, pesos, tiempos de recarga) que aparecen en el
  código de Fase 0 son **placeholders de estructura**, no números finales — el balance real es
  trabajo de Fase 3 (`ROADMAP.md`).

## Guardado

`Progression/Save/SaveSystem.cs` ya implementa escritura atómica (temp file + backup + reemplazo)
y un schema versionado (`SaveDataV1.version`). Cualquier cambio al shape del save necesita sumar
una migración ahí, nunca romper el shape existente — ver skill `save-systems`.

## Después de traer cambios nuevos (packages, `.cs`, asmdefs)

Abrir el proyecto una vez en el Editor de Unity para que genere los `.meta` de todo archivo nuevo,
y commitear cada `.cs`/asset **junto con su `.meta`**. `Library/`, `Temp/`, `obj/`, `Build/` nunca
se commitean (ya están en `.gitignore`).

## Git

- Rama por módulo (`feature/core-sim`, `feature/progression`, `feature/platform-overlay`,
  `feature/ui`), integradas contra `develop`. Detalle en `ROADMAP.md`.
- Evitar que dos personas toquen la misma escena/prefab en paralelo sin avisar — el merge de YAML
  de Unity (`unityyamlmerge`, configurado en `.gitattributes`) ayuda pero no elimina el problema.
- Habilitar los git hooks del repo una vez por clon: `git config core.hooksPath .githooks` (mantiene
  `.claude/skills/` y `.agents/skills/` sincronizados al commitear).
