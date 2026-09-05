# TaskBar Fisher — Documento de Contexto General

*Documento de síntesis del brainstorm del equipo — agosto 2026*
*Actualizado el 28/08/2026 con las decisiones técnicas definidas en sesión de arquitectura (ver §10).*
*Actualizado el 05/09/2026 con la primera ronda de decisiones tomadas en el tablero de Trello "💡 Lluvia de ideas Taskbar Fishing" (lista "Terminado Irán en el Juego") y con el esquema de datos de los Drive colaborativos de Ítems, Cofres, Zonas y Peces. Ver §11.*

## 1. Concepto

Un juego de pesca cuyo verdadero núcleo no es la pesca en sí, sino la colección y la progresión pasiva. La pesca es el tema/vehículo; el loop real es coleccionar peces con distintos niveles de rareza, mejorar el equipo, y volver periódicamente a revisar avances — con la posibilidad de vivir tanto como app de escritorio (task-bar) como integrado a la terminal, pensado para que developers puedan jugarlo sin soltar el teclado.

## 2. Género

Se define como un **idle/incremental game con mecánicas de collectathon (gacha)**, ambientado en pesca. No es un simulador de pesca tipo Fishing Planet — no hay skill real de lanzar el anzuelo como eje del juego.

- **Idle/incremental**: el core es dejar que algo pase con el tiempo (combustible que se recarga, pesca AFK) y volver a recoger resultados y mejorar equipo.
- **Gacha/collectathon**: la motivación central es coleccionar todas las variantes de peces por rareza (R, UR, SSR), la misma lógica de retención de los gacha mobile aplicada a especies de peces.
- **MMO-lite (opcional)**: si se suman mercado, ranking y bounties compartidas, gana una capa social/económica.
- **Utility/companion app (si se hace versión terminal)**: categoría cercana a un "tamagotchi de escritorio", pensado para vivir en segundo plano mientras se trabaja.

Tags sugeridos para un listing de Steam: **Idle / Casual / Simulation**, con **Fishing** y **Collectathon** como tags secundarios.

## 3. Público objetivo

Punto no resuelto en el equipo, con dos caminos posibles:

- **Enfoque gamer** (task-bar, más visual): mayor potencial de monetización, público más amplio.
- **Enfoque developer** (terminal, comandos tipo `/cast`): nicho más específico (80% de los devs vive en la terminal), da renombre al estudio pero es más difícil de monetizar directamente.

Ambos enfoques no son excluyentes: se puede compartir el mismo backend/lógica de juego y construir dos interfaces distintas sobre él.

> **Decisión técnica (28/08/2026):** el desarrollo arranca por el **enfoque gamer/task-bar**, como app de escritorio flotante al estilo *Rusty's Retirement*, *With You* (Yumi) y los "desktop fishing/pet" games — ventana transparente, siempre encima, sin barra de título, con zonas click-through. Ver §9 para el detalle técnico de cómo se implementa esto en Unity.
>
> **Actualización (05/09/2026):** la interfaz de terminal/CLI queda **descartada**, no solo pospuesta — el equipo la movió a la columna "Basura" del Trello de lluvia de ideas. El juego es exclusivamente overlay de escritorio task-bar.

## 4. Mecánicas centrales (ideas recopiladas)

### Pesca y modos de juego
- Modo manual vs. modo automático (AFK); el manual da más beneficios/rendimiento.
- Mecánica de captura por definir: desde un simple click/comando (`/cast`), pasando por un mini-juego activo tipo Stardew Valley (entretenido pero exige concentración), hasta un sistema de combinaciones de teclas tipo Black Desert.
- En AFK, posibilidad de elegir qué rareza pescar, con tiempos de espera más largos (referencia: Black Desert).

### Rareza y colección
> **Decisión (05/09/2026):** la escala de rareza original (R/UR/SSR) queda **reemplazada** por una escala de 9 tiers, confirmada en Trello:
>
> 1. Common
> 2. Uncommon
> 3. Rare
> 4. Epic
> 5. Mythic
> 6. Legendary
> 7. Unique
> 8. Infernal (opcional)
> 9. VOID (opcional)
>
> Esta es la escala que debe usar `RaritySO`/`GachaTableSO` — actualiza el placeholder de Fase 0 que asumía 3 tiers.
- Peces "Shiny" en zonas profundas, que aumenten su valor — **sigue en discusión** (columna "Por Revisar" del Trello), no confirmado todavía.
- Inventario/bag para ver peces capturados: rareza, peso, tamaño — sin duplicados exactos. **Confirmado (05/09/2026):** grilla de **5×5**, ampliable comprando hojas/páginas adicionales.
- Al pescar se puede obtener uno de **3 tipos de resultado**: **Peces**, **Basura**, o **Cofres** (ítems) — confirmado en la tarjeta "Core: Progresión pasiva y colección".
- Nombres y diseños con referencias culturales/humor (ej. un pez inspirado visualmente en una figura pop sin nombrarla directamente, un tiburón con forma de duende), procurando que funcionen en español e inglés. Punto de desacuerdo interno: hay quienes creen que esto vende más que nombres realistas de nicho, y quienes difieren. **Confirmado (05/09/2026)** como parte del juego, con la reserva legal ya señalada en §10.

### Combustible / AFK
- Sistema de combustible que se recarga con el tiempo, más rápido si se juega de forma activa.
- En modo manual, el combustible no se gasta o dura 2-3 veces más.
- **Confirmado (05/09/2026):** el combustible se puede comprar desde la Shop, y su capacidad máxima escala con la mejora del bote (progresivo) — ver Shop en §5.

### Progresión y retención
- Tablón de bounties (diarias y/o semanales): condiciones específicas tipo "pez payaso entre 2.1 y 2.5 kg" con recompensa asociada. Identificado por el equipo como el gancho de retención más fuerte. **Confirmado (05/09/2026)** para el MVP, sin escalado por nivel todavía (eso queda para Fase 3).
- Más espacios de bounties se desbloquean según nivel o alcance de profundidad (1-2 bounties por nivel).
- Pecera de buffs pasivos: ítem donde el jugador guarda peces; mantener peces raros otorga bonificaciones pasivas permanentes (ej. % extra de éxito), referencia: mecánica de recursos de Warframe. **Confirmado (05/09/2026):** capacidad inicial de **4**, mejorable hasta **6-8**.
- **Nuevo (confirmado 05/09/2026) — Sección Gacha / mejoras de tier:** sistema tipo "Taskbar Hero" para subir de tier los ítems — ej. combinar 5 ítems de tier 1 para obtener 1 de tier 2. Falta definir aún cómo se presenta visualmente.
- Sistema de ranking (leaderboards de capturas y bounties, MMO-lite) — **sigue en discusión** (columna "Por Revisar"), no confirmado.

### Interfaz de terminal — descartada (05/09/2026)
- La interfaz CLI/terminal (comandos tipo `/cast`, `/upgrade rod`, addon al PATH del sistema) **se descarta**. Se mantiene aquí solo como registro histórico de la idea original.

### Otras ideas en discusión ("Por Revisar" en Trello, no confirmadas)
- Peces "Shiny" en zonas profundas.
- Sistema de ranking / leaderboards.
- Función de "listen" del teclado, para incentivar a mantener el juego corriendo en segundo plano mientras se trabaja.

## 5. Monetización

- Ítems de mejora raros (bajo % de drop), no disponibles en tienda normal, vendibles en el **mercado de Steam** — identificado como el principal gancho de monetización. **Confirmado (05/09/2026)**.
- Ítems de mercado: mejoras de caña, cebos que aseguran cierta categoría de pez, permisos de pesca temporales — pensados para generar fluctuación de mercado.
- Accesorios de barco (velas, etc.) que aumenten la probabilidad de captura rara, manteniendo probabilidades bajas (ej. base 0,5%, con accesorio hasta 5%) para no romper el valor de la rareza.
- **Nuevo (confirmado 05/09/2026) — SHOP interna del juego** (no confundir con el Mercado de Steam, que es entre jugadores): el jugador puede comprar directamente:
  - **Combustible** (recarga para el modo AFK).
  - **Comidas** (dan buffs temporales).
  - **Botes** (mejoran capacidad de combustible y, potencialmente, probabilidad de captura — a definir junto con los accesorios de barco de arriba).

## 6. MVP propuesto

Para validar el concepto antes de invertir en las partes más costosas, el MVP recomendado incluye:

1. Mecánica de pesca simple (1 click o `/cast` con delay), sin combinaciones de teclas complejas.
2. Sistema de rareza de peces (R/UR/SSR) con lista fija de especies, peso y tamaño variables.
3. Inventario/bag para ver lo pescado.
4. Modo AFK básico con combustible que se recarga con el tiempo.
5. Tablón de bounties simple (aunque sea solo diario, sin escalado por nivel todavía).

**Fuera del MVP** (fase 2 en adelante): mercado de Steam y venta de ítems (requiere Steamworks SDK, aprobación de Valve, balance de economía), accesorios de barco, sistema de ranking.

> **Nota de alcance (05/09/2026):** en la lluvia de ideas de Trello el equipo confirmó (columna "Terminado Irán en el Juego") varios sistemas que en esta lista original estaban fuera del MVP o no existían: **Cofres/ítems como resultado de pesca, Sección Gacha de mejora de tier, SHOP interna, Pecera de buffs pasivos**. Son decisiones de **contenido** confirmadas (van a existir en el juego), pero **no implica automáticamente que entren en el MVP de Fase 1** — esa es una decisión de secuenciación que el equipo debe cerrar explícitamente. Ver ROADMAP.md, sección "Actualización de alcance (05/09/2026)", para la propuesta de dónde ubicar cada sistema nuevo por fase.

La razón: rareza + bag + AFK + bounties ya arman un loop completo de "pescar → coleccionar → razón para volver mañana", suficiente para probar si el concepto engancha antes de asumir el riesgo técnico y legal de la integración con Steam.

## 7. Herramientas de desarrollo sugeridas

| Área | Opciones sugeridas |
|---|---|
| Motor (versión task-bar) | ~~Godot~~ / **Unity 6 (URP) — decidido (28/08/2026)**. Se prioriza sobre Godot por soporte más maduro de plugins nativos de ventana en Windows y familiaridad del equipo con C#. |
| Plataforma inicial | **Solo Windows** por ahora (decidido 28/08/2026) — no se abstrae multiplataforma todavía; se revisita si se agrega macOS/Linux más adelante. |
| Cliente de terminal | ~~Node.js/TypeScript, Python, o Rust~~ — **descartado (05/09/2026)**, ver §3 y §4 |
| Lógica de juego compartida | Librería/servicio backend independiente, consumido por ambas interfaces |
| Backend / persistencia | PostgreSQL + Node/Express o Go; alternativa rápida: Supabase/Firebase |
| Integración de mercado | Steamworks SDK (fase 2) |
| Arte | Aseprite (pixel art) o Photoshop/Procreate; placeholders con IA solo para prototipo, no para arte final vendible |
| Gestión de proyecto | Notion o Linear + GitHub/GitLab |

## 8. Alcances y posibilidades

- Doble interfaz (task-bar + terminal) compartiendo el mismo core, ampliando el público sin duplicar trabajo de lógica.
- Economía de mercado dentro de Steam como modelo de monetización recurrente, no solo venta inicial del juego.
- Sistema de nombres/diseños con referencias culturales como diferenciador de marketing frente a simuladores de pesca "serios".
- Escalabilidad de contenido: nuevas zonas de profundidad, nuevas bounties y nuevos peces se pueden ir agregando post-lanzamiento sin rediseñar el sistema base.

## 9. Decisiones técnicas (motor y arquitectura)

*Definido en sesión de arquitectura con Claude — 28/08/2026. Complementa la tabla de herramientas del §7.*

### Motor y plataforma
- **Unity 6 + Universal Render Pipeline (URP)**, apuntando **exclusivamente a Windows** en esta primera etapa.
- El juego corre como **overlay de escritorio** (ventana transparente, siempre encima, sin barra de título, con zonas click-through), replicando el patrón técnico usado por *Rusty's Retirement*, *With You* y los juegos tipo "desktop pet/fishing garden".
- **Confirmado (05/09/2026) — formato de ventana:** cuadrado de **400×400 px** como tamaño base, con **dimensiones ajustables** por el jugador; posicionable en una esquina o movible por **drag and drop**; **ajuste de transparencia** configurable; opción de **"Pin"** para mantenerlo superpuesto sobre otras ventanas. Hetzas ya tiene un avance exploratorio de este comportamiento (basado en un mod de farming) — coordinar con él antes de rehacer esta parte desde cero.
- Mecánica de captura **confirmada (05/09/2026):** estilo **tira y afloja** (tug-of-war), con parámetros de dificultad que escalan según la rareza del pez — reemplaza la opción de "un solo click" mencionada como alternativa más simple en §4/§6. Esto afecta el diseño de `Platform`/`UI` (Input System) y de `Core` (resolución del intento de captura).
- Esto se logra con una **capa de plataforma nativa aislada** (`Platform/DesktopOverlay`), usando P/Invoke a `user32.dll` (`SetWindowLong`, `SetWindowPos` con `HWND_TOPMOST`) y `dwmapi.dll` (`DwmExtendFrameIntoClientArea`) para la transparencia real, más flags `WS_EX_LAYERED | WS_EX_TRANSPARENT` para el click-through selectivo. Todo el código nativo queda envuelto en `#if UNITY_STANDALONE_WIN && !UNITY_EDITOR`, de modo que el resto del juego no depende de que esté corriendo en modo overlay (funciona igual en el Editor para desarrollo normal).

### Arquitectura de software
Dado que el género real del juego es simulación económica/idle (no gameplay de reflejos), la arquitectura prioriza un modelo de datos ordenado y una simulación desacoplada de la presentación, por sobre patrones pensados para juegos de acción (se descarta ECS/DOTS: no hay volumen de entidades que lo justifique).

- **Capas + eventos**: MonoBehaviours delgados (solo Unity/presentación), lógica de negocio en clases C# puras (POCO) testeables sin correr el Editor, comunicadas por un **event bus** para que los sistemas no se conozcan entre sí directamente.
- **Datos dirigidos por ScriptableObjects**: especies de peces, tablas de rareza/gacha (R/UR/SSR), cañas, cebos, curvas de costo de mejoras, bounties — todo editable por diseño sin tocar código.
- **Sistemas core** (clases puras, independientes de MonoBehaviour): Economía, Gacha/Rareza, Colección/Bag, Guardado (persistencia), Progreso Idle/AFK (incluye cálculo de progreso offline al reabrir el juego, con manejo cuidadoso de timestamps).
- **Guardado robusto desde el día uno**: crítico para el loop idle/AFK — el cálculo de "mientras no estabas" depende de timestamps correctos y debe contemplar casos límite (cambios de hora del sistema, cierres abruptos).
- **Service locator / DI liviano** para que los sistemas se resuelvan entre sí sin singletons desordenados repartidos por el proyecto; no se justifica un framework de DI pesado (tipo Zenject) mientras el equipo sea chico.
- **UI desacoplada**: pantallas de colección, tienda, banners de gacha y notificaciones de progreso AFK se subscriben a eventos de los sistemas core, nunca leen su estado interno directamente.

### Próximos pasos técnicos
1. Conectar la carpeta local del repositorio (`tbFishing`) para generar la estructura de carpetas, Assembly Definitions y el módulo `Platform/DesktopOverlay`.
2. Crear un `CLAUDE.md` en el repositorio con estas convenciones, para que todo el equipo trabaje de forma consistente al usar Claude sobre el proyecto.
3. Implementar los sistemas core listados arriba como esqueleto funcional (sin balance final, solo estructura).

## 10. Limitaciones y riesgos

- **Público de nicho**: "pescadores gamers" es un segmento pequeño; el equipo mismo reconoce que el enfoque puramente realista tendría poco alcance.
- ~~**Versión developer/terminal sin monetización clara**~~ — **resuelto (05/09/2026):** la interfaz de terminal fue descartada por el equipo, este riesgo ya no aplica.
- **Riesgo de sobrecarga de sistemas**: el propio equipo advirtió que sumar demasiadas mecánicas (combustible, buffs, accesorios, ranking, bounties escalonadas) sin filtrar puede terminar en un diseño tan complejo como Black Desert, perdiendo la simpleza que hace atractivo al formato idle/casual.
- **Dependencia de Steamworks**: el mercado de ítems requiere aprobación de Valve, cumplimiento de sus políticas, y balance económico cuidadoso — no es una integración trivial ni rápida.
- **Balance de probabilidades**: las mecánicas de rareza y drop rate (0,5% base, shiny, buffs) necesitan tuning cuidadoso para no devaluar el mercado ni frustrar a los jugadores.
- **Arte con referencias culturales**: el uso de nombres/diseños "que se note que hacen referencia" a figuras o marcas reales conlleva riesgo legal si se acercan demasiado al original; deben quedar claramente como parodia/inspiración, no como copia reconocible.
- **Decisión de género pendiente de validar con mercado**: la categorización como idle/gacha es una hipótesis de diseño; falta testear si el público de Steam responde a esa combinación específica con temática de pesca. (Nota: esto es independiente de la decisión técnica de motor/arquitectura del §9, que ya está tomada — lo pendiente es la validación de mercado, no la implementación.)
- **Riesgo técnico del overlay nativo**: el modo "ventana flotante siempre encima" depende de APIs de Windows (`user32.dll`/`dwmapi.dll`) fuera del sandbox estándar de Unity; hay antecedentes de bugs de transparencia en juegos similares (ej. *Rusty's Retirement*). Se debe aislar bien esta capa (`Platform/DesktopOverlay`) para que un problema ahí no bloquee el desarrollo del resto del juego, y probar temprano en distintas versiones de Windows/configuraciones de escalado de pantalla.

---

*Documento generado a partir del brainstorm del equipo (Popo, Crixs, Pocento, Hetzas) — 27/08/2026. Sección 9 y notas técnicas incorporadas tras sesión de arquitectura — 28/08/2026.*


## 11. Esquema de datos vivo (Drive colaborativo — Ítems, Cofres, Zonas, Peces)

*Agregado el 05/09/2026. El equipo mantiene 4 planillas colaborativas en Drive donde se irá cargando
contenido real (peces, ítems, cofres, zonas). Al 05/09/2026 son plantillas con la estructura de
columnas ya definida y solo 1-2 filas de ejemplo — **no hay balance de contenido todavía**, esto es
el contrato de datos, no el contenido final. Sirve para diseñar `FishSpeciesSO`, `GachaTableSO` y
las nuevas SOs de ítems/cofres/zonas.

### Peces
Columnas: `Tier` (usa la escala de 9 tiers de §4), `Nombre`, `Peso (Rango Peso) KG`, `Posibilidad de
Captura`, `Zona`, `Categoria diseño` (ejemplo visto: `Meme` / `Serio` — confirma la mezcla de peces
con referencia cultural/humor y peces "serios" mencionada en §4).

### Cofres
Columnas: `Tier`, `Nombre`, `Zona`. Ejemplo cargado: `Cofre Normal` (Common), `Cofre Rarete`
(Uncommon). Los cofres son uno de los 3 resultados posibles de pescar (junto a Peces y Basura, ver §4).

### Ítems
Columnas: `Tier`, `Nombre`, `Categoria` (ejemplo: `Anzuelo`), `Posibilidad de Aparicion en Cofre`,
`Foreing Key Cofre` (a qué cofre pertenece — ej. un ítem con Foreing Key "Cofre Normal" solo puede
salir de ese cofre). Esto define la relación Cofre → Ítems que puede contener.

### Zonas
Columnas: `Numero`, `Nombre`, `Tematica`, `Clima`, `Zona`, `Rareza Barco` (probable relación con
las mejoras de bote de la Shop, §5, y con la probabilidad de captura por zona de Peces arriba).
Al 05/09/2026 la planilla de Zonas está vacía (solo encabezados) — sin zonas cargadas todavía.

**Implicancia técnica:** el modelo de `FishSpeciesSO` de Fase 0 necesita sumar campo de Zona y
Categoría de diseño; hace falta crear `ChestSO`, `ItemSO` (con categoría tipo Anzuelo/Cebo/etc.) y
`ZoneSO` como nuevas ScriptableObjects — no estaban en el esqueleto de Fase 0 porque el sistema de
cofres/zonas no existía todavía cuando se armó `CLAUDE.md`.
