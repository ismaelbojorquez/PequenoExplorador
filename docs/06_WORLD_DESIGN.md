# 06 — Diseño de mundo

La Selva es el único mundo del MVP. Cantidades: [`MVP_SCOPE.md`](MVP_SCOPE.md). Dirección visual: [`15_ART_DIRECTION.md`](15_ART_DIRECTION.md).

## Función del mundo

La Selva convierte los objetivos educativos en motivos para explorar. Cada espacio debe comunicar dónde se puede ir, qué reacciona y qué merece observarse, sin llenar la pantalla de marcadores.

## Topología

- **Campamento:** hub seguro con misión, álbum, personalización, asistencia, controles y acceso adulto.
- **Zona Vertical Slice:** claro compacto, recorrido legible, un encuentro animal candidato y retorno directo.
- **MVP:** una Selva conectada compuesta por sectores pequeños reutilizables. La cantidad final de sectores depende de que los descubrimientos canónicos quepan sin saturación; no se comprometen biomas adicionales.
- Atajos y reaperturas reducen recorrido repetitivo. No hay daño, muerte, persecución intensa ni pérdida de inventario.

## Reglas de colocación

- Un punto de interés principal visible o sugerido por encuadre/sonido a la vez.
- Interactivos distinguidos por forma, movimiento y audio, no solo color.
- Caminos válidos tienen señal consistente y retorno claro.
- Descubrimientos educativos aparecen en contexto plausible solo después de aprobación factual.
- Secretos recompensan observación, no precisión extrema, horario real o volver cada día.

## Campamento

El campamento representa progreso sin convertirse en city builder.

- Mejoras son deterministas, permanentes y visibles.
- Personalización cosmética usa opciones ganadas dentro del juego; sin rarezas, cajas o comparación social.
- La primera mejora del Vertical Slice es `Mesa de observación`, que habilita revisar el descubrimiento y evidencia persistencia.
- El MVP incluye el número canónico de mejoras y opciones definido en [`MVP_SCOPE.md`](MVP_SCOPE.md).

## Secretos y objetos especiales

- Un secreto es una observación opcional con pista justa y recompensa fija.
- Un objeto especial es contenido coleccionable contextual, no moneda premium.
- Ninguno bloquea la misión principal ni desaparece por tiempo.

## Contrato runtime de mundo

Cada mundo se descubre por un `WorldManifest` local, no por un switch central. El contrato incluye ID estable, versiones de manifest/contenido, nombre localizado, escena, labels, spawn, checkpoints, catálogos, música/ambiente, requisitos no comerciales y tamaño instalado estimado. `IWorldCatalog` enumera; `WorldLoadUseCase` distingue `Available`, `Locked` y `Missing`; `IWorldSession` posee el mundo activo y retry. Entitlements/precios no forman parte de disponibilidad técnica.

Baseline Selva: `world.jungle`, `scene/jungle`, `spawn.jungle.entry`, `checkpoint.jungle.entry`, labels `scene`/`world-jungle` y catálogo local actual. Sigue `Draft`/`PH_`, por lo que Development puede mostrarlo y Release lo bloquea. Un mundo retirado devuelve copy amable y no reescribe el progreso; la reconciliación de IDs publicados se diseñará cuando save guarde estado de mundo real.

Cada sector futuro define puntos de entrada/salida, descubrimientos disponibles, interacciones, requisitos de misión, feedback ambiental, navegación y fallback si un objetivo no carga. El mundo consulta progreso; no decide recompensas, hechos educativos ni persistencia física.

## Baseline runtime F07–Prompt 16

`Bootstrap` representa Boot y conserva servicios. Camp lista Selva desde el catálogo; `Camp` y `Jungle` son escenas placeholder aditivas, no contenido del Vertical Slice. Prueban `Boot → Camp ↔ Expedition`, error/retry y unload. Al volver a Camp se libera Jungle y sus bundles sin destruir servicios. Addresses, manifests, grupos y la prohibición `SharedLocal → JungleLocal` viven en [`CONTENT_PIPELINE.md`](CONTENT_PIPELINE.md).

Prompt 16 convierte Jungle en un claro técnico `16×14 m` con suelo walkable, límites, dos obstáculos, `NavMeshSurface`, spawn y explorador `PH_`. Es geometría de prueba, no un sector Vertical Slice ni arte final. La cámara asistida queda limitada al stub y salir libera agente/root junto con la escena.

La pantalla temporal, geometría, controlador y controles Development no fijan navegación final, layout ni input infantil aprobado. Siguen bloqueados para Release mediante metadata `PH_`; no cuentan para cantidades MVP ni autorizan producir sectores/animales.

## Post-MVP explícito

Otros biomas, ciclo día/noche, clima dinámico, encuentros procedurales y mundo conectado a servicios remotos son post-MVP sin cantidad comprometida. Dinosaurios/Océano/Espacio/Polar/Desierto son posibilidades documentales: no existen assets, catálogos, entitlements ni compromisos de producción para ellos.
