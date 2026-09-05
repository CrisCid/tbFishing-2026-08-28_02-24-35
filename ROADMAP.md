# TaskBar Fisher — Hoja de Ruta de Producción y Modularización

*Basado en `game_design_overview.md` (28/08/2026, actualizado 05/09/2026) y en el estado actual del repo: proyecto Unity 6.0.5.10f1 (URP 2D) recién creado, sin código ni contenido todavía — solo el scaffold por defecto de Unity.*

**Fase 0 — completada (05/09/2026):** la estructura de carpetas, los 6 asmdefs, el event bus, las SOs vacías, el service locator y el skeleton de `Platform/DesktopOverlay` ya están en el repo. Ver "Actualización de alcance (05/09/2026)" más abajo para lo que cambia de cara a la Fase 1 a partir de las decisiones de Trello.

Esta hoja de ruta asume el alcance ya decidido en el documento de contexto: **MVP task-bar/overlay en Unity 6, solo Windows**, mercado de Steam fuera del MVP, arquitectura por capas + eventos con datos en ScriptableObjects (ver §9 de `game_design_overview.md`).

## Fase 0 — Fundación técnica compartida (no paralelizable)

Antes de repartir trabajo, alguien tiene que construir el terreno común sobre el que los 4 van a pisar. Hacer esto en paralelo sin acuerdo previo es la forma más rápida de generar conflictos de fusión y trabajo duplicado.

- Estructura de carpetas de `Assets/` organizada por dominio (no por tipo de archivo).
- Un **Assembly Definition (asmdef) por módulo** (ver Fase 1) — no es solo organización: hace que el compilador imponga los límites entre módulos, no solo la buena voluntad del equipo.
- **Event Bus** base: el mecanismo por el que los sistemas se hablan sin conocerse.
- Clases base de **ScriptableObjects** vacías (`FishSpeciesSO`, `RaritySO`, `RodSO`, `BaitSO`, `BountySO`, etc.) — solo la forma de los datos, sin balance todavía.
- **Service locator / DI liviano** para resolver sistemas entre sí sin singletons desordenados.
- Skeleton (compilable, no necesariamente funcional) de `Platform/DesktopOverlay`, aislado con `#if UNITY_STANDALONE_WIN && !UNITY_EDITOR`.
- `CLAUDE.md` en la raíz del repo con estas convenciones, para que las 4 sesiones de Claude del equipo trabajen de forma consistente.
- **Congelar los contratos de integración**: nombres y payloads de cada evento del bus, y el schema de cada ScriptableObject. Esto es lo único que de verdad permite que los 4 módulos siguientes avancen sin pisarse — sin esto, la Fase 1 no debería empezar.

Recomendación: una persona (o dos, en una sesión corta conjunta) hace esta fase en pocos días. El resto del equipo puede revisar y opinar sobre los contratos, pero no debería escribir código de gameplay todavía.

## Fase 1 — MVP en paralelo (4 módulos, 1 por persona)

Con los contratos congelados, cada módulo vive en su propio asmdef y su propia rama. El límite entre módulos es exactamente el que ya definió el equipo en §9 del documento de contexto (capas + eventos, sistemas core como POCO independientes de MonoBehaviour).

**Módulo A — Simulación Core** (Economía + Gacha/Rareza + Colección)
- `EconomySystem`, `GachaSystem` (escala de 9 tiers confirmada 05/09/2026: Common → Uncommon → Rare → Epic → Mythic → Legendary → Unique → Infernal (opcional) → VOID (opcional); shiny sigue en discusión, no implementar todavía), `CollectionSystem`/Bag.
- SOs de especies de peces (lista fija del MVP), peso/tamaño variables, tabla de probabilidades, campo de Zona y Categoría de diseño (Meme/Serio) — ver `game_design_overview.md` §11.
- **Nuevo (confirmado 05/09/2026):** al pescar el resultado puede ser Pez, Basura o Cofre. Sumar `ChestSO` e `ItemSO` (con categoría tipo Anzuelo) y la relación Cofre→Ítems de §11.
- **Nuevo (confirmado 05/09/2026):** sección de Gacha de mejora de tier (combinar N ítems de un tier para obtener uno del siguiente) — falta definir el diseño visual, pero la lógica de combinación es Core/POCO igual que el resto.
- Clases POCO puras → testeables sin correr el Editor.
- No conoce UI ni Platform: solo emite y escucha eventos del bus.

**Módulo B — Progresión & Persistencia**
- `IdleProgressSystem` (combustible, cálculo de progreso AFK/offline con timestamps robustos ante cambios de hora del sistema o cierres abruptos). **Confirmado (05/09/2026):** el combustible es comprable en la Shop y su capacidad escala con la mejora de bote — coordinar el contrato de datos de bote con Módulo A (compras) y Módulo D (Shop UI).
- `SaveSystem`.
- `BountySystem` (tablón diario simple, sin escalado por nivel — eso es Fase 3).
- **Nuevo (confirmado 05/09/2026):** `AquariumSystem`/Pecera de buffs pasivos — capacidad inicial 4, mejorable a 6-8; otorga bonos pasivos por peces raros guardados. Encaja acá porque modifica probabilidades/velocidad del progreso idle, no el estado de colección en sí.
- Depende del *contrato* de eventos de Economía/Gacha (Módulo A), no de su implementación interna — por eso puede arrancar en paralelo desde el día 1.

**Módulo C — Platform & Shell**
- Overlay de escritorio: P/Invoke a `user32.dll`/`dwmapi.dll`, ventana transparente/topmost/click-through selectivo. **Confirmado (05/09/2026):** ventana base 400×400 redimensionable, drag and drop, slider de transparencia, y toggle de "Pin" (mantener superpuesta) — Hetzas ya avanzó algo de esto con un mod de farming, coordinar antes de reimplementar.
- Input System: mecánica de captura **confirmada (05/09/2026)** como tira y afloja (tug-of-war), con dificultad escalando por rareza — ya no es un solo click. Reemplaza la referencia a `/cast` en modo click de la versión original.
- Debe funcionar como ventana normal dentro del Editor (el `#if` ya lo aísla) para que el resto del equipo no dependa de tener el overlay funcionando para probar lo suyo.
- Es el módulo de mayor riesgo técnico (bugs de transparencia conocidos en juegos similares, ej. *Rusty's Retirement*) — conviene arrancarlo temprano y probarlo pronto en distintas configuraciones de escalado de pantalla de Windows.

**Módulo D — UI/UX & Presentación**
- Pantallas de colección/bag (grilla 5×5 confirmada, con ampliación por hojas compradas), banner de captura, notificación de progreso AFK al reabrir, tablón de bounties.
- **Nuevo (confirmado 05/09/2026):** UI del mini-juego de captura tira y afloja (Input System de Módulo C + resolución en Módulo A).
- **Nuevo (confirmado 05/09/2026):** UI de la Shop (comprar Combustible, Comidas/buffs, Botes) — consume eventos de Economía, nunca llama `EconomySystem` directo.
- Se suscribe *solo* a eventos del bus, nunca lee el estado interno de otros sistemas directamente.
- Puede arrancar con datos mock/fake antes de que A o B estén terminados, precisamente porque el contrato de eventos ya quedó definido en Fase 0.
- Estilo visual **confirmado (05/09/2026):** 2D pixel art estilo 16-bit — define el criterio de arte para todos los assets de UI y de peces/zonas.

## Fase 2 — Integración y vertical slice

- Las 4 ramas se integran contra una rama `develop` común.
- Alguien (rol rotativo, no fijo) hace de integrador: corre el juego completo y valida el loop end-to-end — pescar → coleccionar → AFK → bounty → volver al día siguiente.
- Los conflictos de escena/prefab se resuelven con `UnityYAMLMerge` (ya configurado vía `.gitattributes`), pero sigue siendo el punto de mayor fricción — evitar que dos personas toquen la misma escena/prefab al mismo tiempo sin avisar.
- Playtesting interno del equipo sobre el MVP completo.

## Fase 3 — Contenido & balance (post-MVP)

Mismo reparto de 4 módulos que en Fase 1, ahora agregando contenido en vez de sistemas nuevos:
- Más especies de peces, variantes shiny, curvas de mejora de cañas/cebos.
- Accesorios de barco, pecera con buffs pasivos, sistema de ranking.
- Tuning de probabilidades de rareza (base 0,5%, escalado por accesorios, etc.).

## Fase 4 — Steamworks & Mercado (aislar aparte, es el más riesgoso)

- Cuenta de **Steamworks partner** y App ID: esto es trámite y costo, no código. Conviene iniciarlo en paralelo a las Fases 1-3, no esperar a esta fase para arrancarlo — los tiempos de aprobación no dependen del equipo.
- Steam Inventory Service, definición de qué ítems son marketeables, y solicitud de aprobación a Valve para el mercado — **no está garantizada ni es rápida**, es una dependencia externa real, no solo técnica.
- Nuevo módulo (o extensión del Módulo A/Economía): integración del SDK, sincronización de inventario, ítems tradeable.
- Balance económico del mercado: fluctuación de precios sin romper la economía interna del juego.

## Fase 5 — Polish, QA y beta

- Bugs, rendimiento, distintas configuraciones de Windows y escalado de pantalla (crítico para el overlay).
- Beta cerrada con jugadores externos al equipo.
- Página de Steam, capturas, trailer.

## Fase 6 — Lanzamiento y contenido post-launch

- Pipeline de nuevas zonas de profundidad, peces y bounties sin rediseñar el sistema base — ya contemplado en la arquitectura de datos por ScriptableObjects.

---

## Forma de trabajar entre las 4 personas

- **Ramas por módulo**: `feature/core-sim`, `feature/progression`, `feature/platform-overlay`, `feature/ui`, integradas contra `develop`; `develop` → `main` solo tras validar en Fase 2.
- **Nadie edita la misma escena/prefab en paralelo** sin avisar al resto — aunque el merge de YAML de Unity ya está configurado, sigue siendo el mayor punto de fricción real.
- **Sync corto 2 veces por semana como mínimo** mientras dura la Fase 0/1, enfocado en dudas de integración y cambios a los contratos de eventos/SOs (cambiarlos a mitad de la Fase 1 rompe el paralelismo de los otros 3).
- **Definition of Done por módulo**: compila de forma aislada, tiene al menos un test o escena de prueba propia, no rompe el contrato de eventos acordado en Fase 0.
- **`CLAUDE.md`** como fuente única de verdad de convenciones — cada sesión de Claude de cada integrante debería leerlo antes de tocar código.

## Próximo paso concreto

Arrancar por la Fase 0 ahora: pedirme generar en el repo la estructura de carpetas, los asmdefs, el skeleton del event bus, los ScriptableObjects vacíos y el `CLAUDE.md` con las convenciones. Recién desde ahí se reparten los 4 módulos de la Fase 1.


## Actualización de alcance (05/09/2026) — decisiones de Trello

La lluvia de ideas del equipo (Trello "💡 Lluvia de ideas Taskbar Fishing") movió varios sistemas a
"Terminado Irán en el Juego" (confirmados como contenido) que en la versión original de este
roadmap estaban en Fase 3/4 o no existían: **Cofres/Ítems como resultado de pesca, Gacha de mejora
de tier, SHOP interna, Pecera de buffs pasivos**, además de confirmar Mercado de Steam y cambiar la
mecánica de captura a tira y afloja con dificultad por rareza (antes: un click simple). También se
descartó definitivamente la interfaz de terminal/CLI.

Esto es una decisión de **contenido** (van a existir), no todavía una decisión de **secuenciación**
de fases. Los módulos de la Fase 1 de arriba ya incorporan dónde encajaría cada sistema nuevo
(marcado inline como "Nuevo (confirmado 05/09/2026)"), pero el equipo debería confirmar
explícitamente en el próximo sync si:

1. Cofres/Ítems, Gacha de mejora, Pecera y Shop entran al **MVP de Fase 1**, o
2. Se mantiene el MVP original (pesca simple + rareza + bag + AFK + bounty) y estos sistemas pasan
   a una **Fase 1.5** inmediatamente después, antes de la Fase 2 de integración.

Mientras esa decisión no se cierre, cualquier persona que arranque el Módulo A o B debería tratar
estos sistemas nuevos como **siguiente prioridad después del MVP original**, no como bloqueantes del
vertical slice de Fase 2.

Sigue habiendo ideas en la columna "Por Revisar" de Trello que **no** están confirmadas y no deben
implementarse todavía: peces Shiny, sistema de ranking/leaderboards, función de "listen" de teclado
para incentivar dejar el juego corriendo en segundo plano.

Las planillas colaborativas de Drive (Ítems, Cofres, Zonas, Peces) definen el esquema de datos para
las SOs nuevas — ver `game_design_overview.md` §11. Al 05/09/2026 son solo estructura de columnas,
sin contenido balanceado.
