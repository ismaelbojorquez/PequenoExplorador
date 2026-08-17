# ExecPlan — fotografía ficticia asistida del Vertical Slice

- Fase/Gate: Prompt 19 / Gate B permanece FAIL hasta Fases 19–29 y reauditoría
- Estado: Complete
- Creado/actualizado: 2026-08-17 00:07 America/Mexico_City
- Owner: Gameplay Camera Engineer / Rendering Engineer / Child UX

## Propósito y alcance

Implementar el modo cámara ficticia para `discovery.jungle.keel-billed-toucan`: entrar desde la interacción aprobada, suspender locomoción, evaluar un encuadre tolerante, capturar una miniatura acotada, registrar discovery idempotente y conservar el progreso aunque falle el almacenamiento. Incluye UI temporal localizada, file store de fotografías, save de metadatos, validadores y pruebas. Excluye cámara física, galería, compartir, arte/audio final, álbum, economía y permisos sensibles.

Aceptación observable: flujo válido/inválido recuperable, un solo shutter en vuelo, thumbnail mejor por discovery, cero RenderTexture temporal viva al terminar, save migrable y APK Development sin permiso `CAMERA`.

## Contexto y orientación

- HEAD inicial: `3f3a6dfb7dd40b4a95927614e47abf274881bb4e`; `main`, árbol limpio, ahead de origin por 18 commits.
- Unity `6000.3.22f1`; nueve asmdefs; sin dependencia nueva.
- Catálogo/interaction runtime Approved: `discovery.jungle.keel-billed-toucan` y `interaction.jungle.keel-billed-toucan`.
- Save actual v5; la fotografía requiere v6 porque el JSON solo conservará metadata/referencia y nunca bytes.
- Input ya posee mapa `Photography`; locomoción e interacción se suspenden al cambiar de mapa.
- El prefab aprobado incluye `VS_PhotoAnchor` y `CandidatePhotoBounds` revisados.
- Baseline: `scripts/compile` PASS, EditMode `107/107`, PlayMode `18/18`; `adb devices` vacío. El manifest previo no contiene cámara/micrófono/AD_ID.

## Progreso

- [x] 2026-08-16 23:31 — preflight, lectura canónica, inventario, baseline y permisos previos contrastados.
- [x] 2026-08-17 00:07 — contratos/evaluator/capture use case y repositories testeables.
- [x] 2026-08-17 00:07 — RenderTexture renderer, PhotoStore local y simulación Development.
- [x] 2026-08-17 00:07 — target/UI/scene/Bootstrap integrados; discovery directo reemplazado por cámara.
- [x] 2026-08-17 00:07 — save v6, migración, validator y suites EditMode/PlayMode.
- [x] 2026-08-17 00:07 — profiling Editor, pipeline completo, APK/manifest y revisión Git.

## Hallazgos

- `AGENTS.md` aún decía Save v3 y el encabezado de arquitectura describía un estado anterior; implementación/documento save/status prueban v5. Se corregirán junto con la documentación de fase.
- No hay documento separado de cámara/performance/privacy; sus contratos están repartidos entre discovery, UI/UX, input, save, arquitectura, testing y riesgos. Se creará el documento canónico de fotografía sin duplicar esas fuentes.
- El discovery runtime se concede hoy directamente desde la interacción. Prompt 19 debe reemplazar ese wiring por una solicitud explícita de fotografía; conservar la clase previa solo para compatibilidad/test no autoriza usarla como atajo runtime.
- No hay dispositivo Android conectado; cualquier afirmación táctil/memoria física será `NOT RUN`.

## Decisiones

- 2026-08-16 — evaluación pura con coverage, distancia, línea de visión, centrado y orientación. Thresholds iniciales: coverage `0.08`, distancia `10 m`, offset centro `0.36`, alignment `0.35`; son tolerantes y provisionales hasta hardware/playtest.
- 2026-08-16 — thumbnail `384×216`, PNG, RenderTexture ARGB32 temporal, máximo `512 KiB` por archivo, uno por discovery y presupuesto local `32 MiB`/64 entradas. No `ScreenCapture`.
- 2026-08-16 — save v6 añade solo metadata/referencia relativa de la mejor foto; binarios/manifiesto técnico viven bajo `persistentDataPath/Photos` mediante `IPhotoStore`.
- 2026-08-16 — discovery se confirma antes de render/storage: un fallo de miniatura nunca revierte progreso. La misma capture key alimenta `grant.photo.*` y retry no duplica.
- 2026-08-16 — flash es suave y cancelable; reduce-motion lo omite. Se reutilizan cues genéricos `PH_` ya bloqueados para Release, sin fingir audio final.

## Plan de implementación

1. Añadir `Application.Photography`: targets/samples/evaluation/settings, puertos renderer/store/repository, acción de entrada y use case serializado/idempotente.
2. Añadir `Domain.PhotoProgress`; DTO v6, migración v5→v6 y mapping determinista.
3. Añadir `Infrastructure.Photography.LocalPhotoStore` atómico con nombres seguros, manifiesto, límites, cleanup y fallo inyectable Development.
4. Añadir `Presentation.Photography`: target authoring, muestreo sin allocations, renderer RenderTexture, scene root y viewfinder uGUI localizado.
5. Extender Bootstrap/tooling/scene/prefab/localización y validator CLI, sin asmdef/dependencia/permiso nuevos.
6. Cubrir evaluator, límites, score, filenames, best policy, idempotencia, storage failure, pause/unload/reduce-motion y leak counters; ejecutar pipeline/build y actualizar docs.

## Comandos y validación

- `git status --short --branch`, `git branch --show-current`, `git log -1 --format=fuller` — preflight PASS.
- `scripts/compile` — baseline PASS.
- `scripts/test-editmode` — baseline PASS `107/107`.
- `scripts/test-playmode` — baseline PASS `18/18`.
- `adb devices` — ejecutado; cero dispositivos, hardware `NOT RUN`.
- `scripts/validate` — PASS en `3:21.55`; EditMode `112/112`, PlayMode `19/19`, Addressables local y APK Development.
- APK — `81,127,439` bytes, SHA-256 `37d54e90759f9fe13365e5137d063f31e58e82362ba400d902dd67d52c2cf61c`; API 26/36, IL2CPP/ARM64.
- Manifest — solo `INTERNET` heredado y permiso interno receiver; `CAMERA`/micrófono/ubicación/contactos/`AD_ID` ausentes.
- Profiling Editor — estimated peak `582,182` bytes, delta global orientativo `289,481`, recursos temporales `0`; dispositivo físico `NOT RUN`.

## Recovery y seguridad

No instalar dependencias, cambiar asmdefs ni solicitar permisos. Assets/escenas se generan con tooling idempotente y se revisa su diff; no editar `Library` ni artefactos. Si una migración falla, conservar v5 y retirar solo cambios propios no commiteados con parches explícitos, nunca Git destructivo. Un fallo de PhotoStore se degrada a imagen canónica y discovery persistente. No incluir rutas personales, imágenes capturadas de usuarios, cámara física, red o datos identificables.

## Resultados y retrospectiva

Completado. El flujo normal abre cámara desde el tucán, evalúa con guía tolerante, captura discovery y mejor thumbnail, conserva progreso ante fallo y limpia recursos al salir/unload. Save v6 migra sin inventar fotos y separa PNG del JSON. `scripts/validate` y APK Development pasan; no hay permiso CAMERA. Android físico, playtest/tuning, UI/audio final y presión de almacenamiento siguen pendientes. Siguiente incremento permitido: Prompt 20 (álbum), mientras Gate B permanece FAIL.
