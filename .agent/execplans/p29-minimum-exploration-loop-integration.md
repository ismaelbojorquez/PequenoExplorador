# ExecPlan — integración del loop mínimo de exploración

- Fase/Gate: Prompt 29 / Gate B permanece FAIL hasta Prompt 30
- Estado: Complete
- Creado/actualizado: 2026-08-17 07:10 CST
- Owner: Technical Game Direction / Integration Engineering

## Propósito y alcance

Ensamblar y probar un journey continuo y offline con el único contenido Vertical Slice: Camp → Selva → movimiento → interacción/actividad amable → fotografía → discovery/fact → estrellas → misión → álbum → primera mejora → Camp → guardado. La primera sesión usa el FTUE versionado y la segunda no lo impone. Se integran contratos existentes, checkpoints y recuperación; se excluyen contenido adicional, SDKs, padres, monetización y arte/audio final.

## Contexto y orientación

HEAD inicial `2bdb62a00adf4c5a7a7d3e9eb2f8081671487621`, rama `main`, árbol limpio. `scripts/validate` inicial PASS el 2026-08-17: repository checks, compile, Addressables locales, EditMode `165/165`, PlayMode `28/28` y APK Development. Save es v12; el tucán `discovery.jungle.keel-billed-toucan` y la misión fotográfica están Approved, mientras la actividad integrada sigue Sourced/placeholder y por tanto bloqueada para Release. Gate B conserva `FAIL`: el audit de 2026-08-16 se ejecutó cuando HEAD era Prompt 18 y debe repetirse de forma independiente después de esta fase.

El composition root es `Assets/_Game/Bootstrap/DiagnosticBootstrap.cs`; la interacción se enlaza en `InteractionSceneRoot`, la actividad continúa por `LearningActivityView`, fotografía por `PhotographySceneRoot`, y Camp/álbum/misión por sus presenters. Los repositories de Discovery, Economy, Missions, Learning y Camp convergen en el mismo `AutosaveCoordinator.Latest`.

## Progreso

- [x] 2026-08-17 07:10 CST — preflight, documentos, inventario y baseline integral contrastados.
- [x] 2026-08-17 — integrado orden actividad→fotografía y activación normal/idempotente de misión sin atajos UI→Save.
- [x] 2026-08-17 — añadidos checkpoints observables y build marker `PE_VERTICAL_SLICE_P29`/journey v1.
- [x] 2026-08-17 — journey PlayMode E2E con tres repeticiones, pause, recovery, ES/EN, ratios y métricas.
- [x] 2026-08-17 — `scripts/validate` integral PASS y fuentes canónicas actualizadas.

## Hallazgos

- `InteractionSceneRoot.Bind` prioriza `HasDirectDiscovery` sobre `HasLearningActivity`; el tucán abre cámara y ofrece actividad después de capturar. El objetivo P29 requiere actividad antes de fotografía y `LearningActivityView` ya posee una continuación explícita hacia cámara al completar.
- `mission.vertical-slice.photograph-toucan` solo se activa desde `MissionView`; el journey normal no incluye ese paso y una captura previa no cuenta por diseño. Debe activarse al entrar por primera vez a Selva usando `MissionCoordinator`, con idempotencia existente.
- Los repositorios solicitan autosave tras mutaciones, pero P29 exige checkpoints explícitos en fronteras del journey. Bootstrap puede observar outcomes sin leer/escribir DTO o archivos.
- `adb devices -l` no lista hardware. El Android físico seguirá `NOT RUN`; no se presentará Editor batch como profiling de dispositivo.

## Decisiones

- 2026-08-17 — reutilizar `LearningInteractionAction` como acción primaria del tucán cuando exista actividad; al cerrar una actividad completada, la continuación explícita solicita fotografía. No se introduce bus ni coordinador paralelo.
- 2026-08-17 — activar la misión VS al entrar a `world.jungle`; `Activate` ya es idempotente y conserva la regla de que hechos previos no cuentan.
- 2026-08-17 — checkpoint de captura cubre discovery+rewards+misión ya reconciliados; compra Camp y retorno a Camp generan checkpoints adicionales. El flush solo se fuerza en pruebas/lifecycle, no en cada frame o tap.
- 2026-08-17 — Gate B no cambia en esta fase; Prompt 30 debe auditar sin confiar en este reporte.

## Plan de implementación

1. Ajustar el binding de interacción y la continuidad Learning→Photography sin cambiar reglas puras ni contenido.
2. Componer activación de misión y checkpoints de captura, compra Camp y retorno; añadir un marker/version técnico visible en reportes/diagnóstico Development.
3. Crear un journey PlayMode que recorra acciones normales, repita fotografía tres veces, compruebe idempotencia, pause/resume, ES/EN, ratios, flush/reload y segunda sesión sin FTUE obligatorio.
4. Cubrir save corrupto con las suites existentes y añadir validadores estáticos de integración/offline/debug exclusion donde falte.
5. Ejecutar comandos individuales y `scripts/validate`; actualizar arquitectura, loop, testing, riesgos, decisiones, changelog y status.

## Comandos y validación

- `scripts/validate` — baseline PASS: EditMode `165/165`, PlayMode `28/28`, Addressables `61` locations / `1,920,120` bytes, APK `67,440,962` bytes, SHA-256 `22f29e9b7e901a0aaa2bf9d39cf3cbf6918b5047e3ac93976ac806d4494899ef`.
- `adb devices -l` — ejecutado; ningún dispositivo listado, Android físico `NOT RUN`.
- `scripts/test-editmode` — PASS `167/167`.
- `scripts/test-playmode` — PASS `29/29`.
- `scripts/validate` — PASS: Addressables 61/1,920,120 bytes; APK 67,443,923 bytes, SHA-256 `770b2d855e485dfbf2cd23528002328ec3cae5199ec715c888eac434f0b2b08f`.

## Recovery y seguridad

No se cambian schemas, paquetes, permisos, Addressables remotos, servicios externos ni signing. Los cambios deben permanecer en contratos existentes y ser reversibles por archivo. Ante fallo, conservar `artifacts/`, corregir el primer error, no editar `Library`, no resetear Git ni incorporar cambios ajenos. Checkpoints no sustituyen la atomicidad del save y ningún test puede usar botones debug dentro del journey, salvo preparación aislada de fixture antes de comenzar.

## Resultados y retrospectiva

El journey normal quedó observable y determinista. La integración expuso dos races reales: preferencia persistida desde `ISaveService.Current` podía pisar progreso pending, y un write in-flight dejaba temporalmente de ser visible como Latest. El coordinador ahora conserva ownership pending/in-flight/current y locale/audio fusionan sobre él; las regresiones y el journey pasan. El test final midió 7.947 s, tres recapturas, count 4 y saldo 1 tras mejora; FPS/memoria son solo Editor batch.

Gate B permanece FAIL hasta auditoría independiente. Arte/audio final, actividad Release Approved, touch Android, FPS/memoria de dispositivo, cinco recorridos y playtest infantil siguen fuera de la evidencia automatizada.
