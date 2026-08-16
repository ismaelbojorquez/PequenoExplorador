# ExecPlan — discovery persistente e idempotente

- Fase/Gate: Prompt 18 / Gate B
- Estado: Complete
- Creado/actualizado: 2026-08-16 15:53 America/Mexico_City
- Owner: Game Systems Engineer — collection/progression

## Propósito y alcance

Implementar discovery nuevo/repetido con idempotency key, queries de progreso por mundo/categoría y persistencia schema v4, conectado únicamente al fixture animal Development. El resultado será consumible por Economy/Album/UI sin conocer estrellas, audio o vistas concretas. Se excluyen cámara, álbum final, economía completa, contenido factual/final y nuevas dependencias.

## Contexto y orientación

HEAD inicial `f6a38be097b6827f4b6a319fc363d264b9be8310`, `main`, limpio y `ahead 9`. `IContentCatalog` ya resuelve el discovery neutral Draft; `InteractionCoordinator` invoca `IInteractable`; `ISaveService.Current` expone un `PlayerProgress` inmutable. Save real es v3 con `discoveryIds` planos y migraciones v0→v1→v2→v3. Prompt 18 debe introducir DTO v4 y migración v3→v4, conservando v1–v3 sin reinterpretarlos.

## Progreso

- [x] 2026-08-16 15:36 — preflight, inventario, schema/migration plan y baseline integral completados.
- [x] 2026-08-16 — modelo Domain y casos de uso/queries Application implementados.
- [x] 2026-08-16 — save v4, migración v3→v4, autosave Latest y composición implementados.
- [x] 2026-08-16 — fixture animal Development, feedback ES/EN y contador diagnóstico conectados por datos.
- [x] 2026-08-16 — EditMode 103/103, PlayMode 18/18, pipeline Android y revisión final ejecutados.

## Hallazgos

- `PlayerProgress.DiscoveryIds` es una lista técnica sin count/idempotency/date; no puede editarse in-place ni satisfacer repetición/reward-once.
- El discovery neutral está Draft y placeholder: Development puede usarlo; Release ya lo bloquea por `DATA025`.
- Arquitectura conserva una frase residual de Prompt 16 que niega la interacción implementada; debe corregirse.

## Decisiones

- 2026-08-16 — crear schema v4 con records explícitos y processed grant keys; v3→v4 transforma cada ID histórico en count 1 sin fecha/idempotency inventados.
- 2026-08-16 — la fecha será un día local agregado opcional (`yyyy-MM-dd`) calculado desde `IClock.UtcNow` + offset inyectado; no se guarda hora, zona ni identificador personal.
- 2026-08-16 — `DiscoverUseCase` devuelve outcome explícito y progreso actualizado; un adapter/repository sobre `PlayerProgress` solicita el checkpoint, pero Economy/Album permanecen consumidores futuros.

## Plan de implementación

1. Crear `DiscoveryProgress`/idempotency value y operaciones inmutables en Domain.
2. Crear repository, `DiscoverUseCase`, outcomes y queries por catálogo en Application.
3. Añadir DTO/mapping v4, migración pura v3→v4 y registrar el paso en Bootstrap/tests.
4. Adaptar el fixture animal mediante un handler explícito compuesto en Bootstrap y mostrar feedback/contador Development localizado.
5. Cubrir first/repeat/idempotency/missing/unapproved/removed/progress/migration y persistencia PlayMode.
6. Actualizar fuentes canónicas, ejecutar pipeline, revisar staged diff y crear el commit solicitado.

## Comandos y validación

- `scripts/validate` — baseline `PASS`, código `0`, `1:17.19`; EditMode 99/99, PlayMode 17/17 y APK.
- `scripts/compile` — PASS tras corregir comparación de value object.
- `scripts/test-editmode` — PASS 103/103 tras corregir asserts de colección observados.
- `scripts/test-playmode` — PASS 18/18 tras preservar copy infantil existente al regenerar tablas.
- `scripts/validate` — PASS final repetido tras limpiar YAML, código 0, 2:12.27; APK 105,942,573 bytes/hash registrado en STATUS/CHANGELOG.
- `git diff --check`/repository checks — PASS; staged check y commit forman el cierre Git.

## Recovery y seguridad

No se cambia serializer, paquete, filesystem ni filenames. El paso v3→v4 se añade sin editar migraciones históricas. Los saves future-version siguen read-only. Si una prueba destructiva falla, conservar primary/backup, corregir DTO/mapping y repetir sobre stores in-memory/temporales; no editar `Library`, artefactos o saves reales para forzar éxito.

## Resultados y retrospectiva

Implementación y evidencia local completadas: schema v4/migración, first/repeat/idempotencia, persistencia tras reload, denominadores Approved, Draft ReleaseBlocked y APK Development. Android físico fue `NOT RUN` porque `adb devices` no listó hardware. Prompt 19 queda bloqueado por su precondición de animal Approved; no se promueve el fixture neutral por conveniencia.
