# TaskBar Fisher — Documento de Contexto General

*Documento de síntesis del brainstorm del equipo — agosto 2026*
*Actualizado el 28/08/2026 con las decisiones técnicas definidas en sesión de arquitectura (ver §10).*

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

> **Decisión técnica (28/08/2026):** el desarrollo arranca por el **enfoque gamer/task-bar**, como app de escritorio flotante al estilo *Rusty's Retirement*, *With You* (Yumi) y los "desktop fishing/pet" games — ventana transparente, siempre encima, sin barra de título, con zonas click-through. La interfaz de terminal queda como posible fase futura, no descartada, pero no forma parte del alcance inicial. Ver §9 para el detalle técnico de cómo se implementa esto en Unity.

## 4. Mecánicas centrales (ideas recopiladas)

### Pesca y modos de juego
- Modo manual vs. modo automático (AFK); el manual da más beneficios/rendimiento.
- Mecánica de captura por definir: desde un simple click/comando (`/cast`), pasando por un mini-juego activo tipo Stardew Valley (entretenido pero exige concentración), hasta un sistema de combinaciones de teclas tipo Black Desert.
- En AFK, posibilidad de elegir qué rareza pescar, con tiempos de espera más largos (referencia: Black Desert).

### Rareza y colección
- Categorías de peces tipo cartas: R, UR, SSR — peces de mayor profundidad, más raros.
- Posible mecánica de "shiny" en peces de zonas profundas, que aumente su valor.
- Inventario/bag (`/bag`, `/store`) para ver peces capturados: rareza, peso, tamaño — sin duplicados exactos.
- Nombres y diseños con referencias culturales/humor (ej. un pez inspirado visualmente en una figura pop sin nombrarla directamente, un tiburón con forma de duende), procurando que funcionen en español e inglés. Punto de desacuerdo interno: hay quienes creen que esto vende más que nombres realistas de nicho, y quienes difieren.

### Combustible / AFK
- Sistema de combustible que se recarga con el tiempo, más rápido si se juega de forma activa.
- En modo manual, el combustible no se gasta o dura 2-3 veces más.

### Progresión y retención
- Tablón de bounties (diarias y/o semanales): condiciones específicas tipo "pez payaso entre 2.1 y 2.5 kg" con recompensa asociada. Identificado por el equipo como el gancho de retención más fuerte.
- Más espacios de bounties se desbloquean según nivel o alcance de profundidad (1-2 bounties por nivel).
- Pecera/inventario con buffs pasivos: peces raros en posesión podrían otorgar bonos aleatorios de % de éxito (referencia: mecánica de recursos de Warframe).
- Sistema de ranking mencionado, posiblemente integrado al tablón de bounties.

### Interfaz de terminal (si se desarrolla)
- Consola interna en "modo trabajo", pensada para no ser distractiva — comandos tipo `/cast`, `/upgrade rod`.
- Addon opcional (gratuito) que agregue el comando al PATH del sistema, permitiendo jugar desde cualquier terminal.

## 5. Monetización

- Ítems de mejora raros (bajo % de drop), no disponibles en tienda normal, vendibles en el **mercado de Steam** — identificado como el principal gancho de monetización.
- Ítems de mercado: mejoras de caña, cebos que aseguran cierta categoría de pez, permisos de pesca temporales — pensados para generar fluctuación de mercado.
- Accesorios de barco (velas, etc.) que aumenten la probabilidad de captura rara, manteniendo probabilidades bajas (ej. base 0,5%, con accesorio hasta 5%) para no romper el valor de la rareza.

## 6. MVP propuesto

Para validar el concepto antes de invertir en las partes más costosas, el MVP recomendado incluye:

1. Mecánica de pesca simple (1 click o `/cast` con delay), sin combinaciones de teclas complejas.
2. Sistema de rareza de peces (R/UR/SSR) con lista fija de especies, peso y tamaño variables.
3. Inventario/bag para ver lo pescado.
4. Modo AFK básico con combustible que se recarga con el tiempo.
5. Tablón de bounties simple (aunque sea solo diario, sin escalado por nivel todavía).

**Fuera del MVP** (fase 2 en adelante): mercado de Steam y venta de ítems (requiere Steamworks SDK, aprobación de Valve, balance de economía), accesorios de barco, pecera con buffs pasivos, sistema de ranking, y el addon de integración al PATH del sistema.

La razón: rareza + bag + AFK + bounties ya arman un loop completo de "pescar → coleccionar → razón para volver mañana", suficiente para probar si el concepto engancha antes de asumir el riesgo técnico y legal de la integración con Steam.

## 7. Herramientas de desarrollo sugeridas

| Área | Opciones sugeridas |
|---|---|
| Motor (versión task-bar) | ~~Godot~~ / **Unity 6 (URP) — decidido (28/08/2026)**. Se prioriza sobre Godot por soporte más maduro de plugins nativos de ventana en Windows y familiaridad del equipo con C#. |
| Plataforma inicial | **Solo Windows** por ahora (decidido 28/08/2026) — no se abstrae multiplataforma todavía; se revisita si se agrega macOS/Linux más adelante. |
| Cliente de terminal | Node.js/TypeScript, Python, o Rust (footprint mínimo) — pendiente, fuera del alcance inicial (ver §3) |
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
- **Versión developer/terminal sin monetización clara**: da renombre al estudio pero no genera ingresos directos — es una decisión de marca, no de negocio.
- **Riesgo de sobrecarga de sistemas**: el propio equipo advirtió que sumar demasiadas mecánicas (combustible, buffs, accesorios, ranking, bounties escalonadas) sin filtrar puede terminar en un diseño tan complejo como Black Desert, perdiendo la simpleza que hace atractivo al formato idle/casual.
- **Dependencia de Steamworks**: el mercado de ítems requiere aprobación de Valve, cumplimiento de sus políticas, y balance económico cuidadoso — no es una integración trivial ni rápida.
- **Balance de probabilidades**: las mecánicas de rareza y drop rate (0,5% base, shiny, buffs) necesitan tuning cuidadoso para no devaluar el mercado ni frustrar a los jugadores.
- **Arte con referencias culturales**: el uso de nombres/diseños "que se note que hacen referencia" a figuras o marcas reales conlleva riesgo legal si se acercan demasiado al original; deben quedar claramente como parodia/inspiración, no como copia reconocible.
- **Decisión de género pendiente de validar con mercado**: la categorización como idle/gacha es una hipótesis de diseño; falta testear si el público de Steam responde a esa combinación específica con temática de pesca. (Nota: esto es independiente de la decisión técnica de motor/arquitectura del §9, que ya está tomada — lo pendiente es la validación de mercado, no la implementación.)
- **Riesgo técnico del overlay nativo**: el modo "ventana flotante siempre encima" depende de APIs de Windows (`user32.dll`/`dwmapi.dll`) fuera del sandbox estándar de Unity; hay antecedentes de bugs de transparencia en juegos similares (ej. *Rusty's Retirement*). Se debe aislar bien esta capa (`Platform/DesktopOverlay`) para que un problema ahí no bloquee el desarrollo del resto del juego, y probar temprano en distintas versiones de Windows/configuraciones de escalado de pantalla.

---

*Documento generado a partir del brainstorm del equipo (Popo, Crixs, Pocento, Hetzas) — 27/08/2026. Sección 9 y notas técnicas incorporadas tras sesión de arquitectura — 28/08/2026.*
