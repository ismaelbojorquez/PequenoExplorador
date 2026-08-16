# ExecPlan — input táctil accesible y adaptación de dispositivo

- Fase/Gate: Prompt 13 / Gate B
- Estado: Complete
- Creado/actualizado: 2026-08-16 12:28 America/Mexico_City
- Owner: Mobile Input Engineer / Accessibility Engineer

## Propósito y alcance

Entregar acciones semánticas UI/Explorer/Photography/Parents/Debug, reconocimiento acotado de tap/hold/drag/pinch, Back seguro, safe area central, haptics no-op y harness de ratios. Incluye wiring, validadores y tests; excluye movimiento final, joystick, UI final, Device Simulator como dependencia y vibración invasiva.

## Contexto y orientación

La baseline observada parte del commit `ea023e34786119b7a9f4ee664075184eef928170`, rama `main`, árbol limpio. Unity es `6000.3.22f1`; Input System `1.20.0` está fijado en manifest/lock. La única integración previa es `InputSystemUIInputModule` en Bootstrap; el inventario no encontró `UnityEngine.Input`, `Touchscreen.current`, `Keyboard.current` ni `Mouse.current` en runtime.

## Progreso

- [x] 2026-08-16 12:08 — preflight, inventario y baseline `scripts/validate`: PASS; EditMode 70/70, PlayMode 7/7, APK Development.
- [x] 2026-08-16 12:17 — contratos, adapter, contenido, safe area, Back y diagnóstico Development implementados; EditMode 77/77 y PlayMode 10/10.
- [x] 2026-08-16 12:25 — pipeline completo PASS y APK Development generado.
- [x] 2026-08-16 12:28 — documentación canónica actualizada; pendiente solo revisión final/commit de esta misma sesión.

## Hallazgos

- La accesibilidad de interacción vive canónicamente en `docs/14_UI_UX.md`; no existe documento independiente que deba duplicarse.
- Input System ya está aprobado/fijado; esta fase no añade ni actualiza paquetes.
- Device Simulator no figura en el manifest y no se instalará: el harness automatizado usa modelos de viewport/safe area; la inspección física sigue requerida antes de Gate C.

## Decisiones

- 2026-08-16 — un único adapter consulta Input System y emite intenciones; el clasificador puro no depende de Unity y usa storage fijo para evitar allocations por frame.
- 2026-08-16 — tap-to-move queda como intención candidata, no como movimiento implementado; no se añade joystick.
- 2026-08-16 — haptics queda detrás de puerto con implementación no-op/desactivada; no se invoca vibración de plataforma en esta fase.

## Plan de implementación

1. Añadir contratos Input/SafeArea/Haptics y reconocedor puro en Application.
2. Añadir asset de acciones y thresholds de authoring en Content.
3. Implementar adapter Input System, safe area y driver en Infrastructure; componerlos solo en Bootstrap.
4. Añadir fitter, pausa Back y overlays Development en Presentation/Bootstrap.
5. Añadir setup/validator CLI, pruebas EditMode/PlayMode y documentación canónica.

## Comandos y validación

- `scripts/validate` — baseline PASS en 1:15; EditMode 70/70, PlayMode 7/7, APK Development PASS.
- `scripts/compile` — PASS; cinco mapas, safe area, legacy=0, haptics=noop.
- `scripts/test-editmode` — PASS `77/77`.
- `scripts/test-playmode` — PASS `10/10`, incluido `InputTestFixture`.
- `scripts/validate` — PASS final, código 0, 1:18.93; APK 66,067,652 bytes y SHA-256 `c19c68eacf50dfb61916c1eaa09c1c787bf452fd12f7e15784206fe898600d26`.
- `scripts/build-android-release` — `BLOCKED` esperado, código 3, signing externo ausente.
- `adb devices` — `NOT RUN` en hardware: lista vacía.

## Recovery y seguridad

Todos los cambios son versionables. No tocar `Library`, artifacts, signing, permisos, remote config ni paquetes. Si Unity falla al generar escenas/assets, conservar logs, corregir fuente/setup y reejecutar; no editar cachés. No incorporar cambios ajenos si aparece un estado sucio inesperado.

## Resultados y retrospectiva

Foundation móvil semántica implementada sin gameplay ni dependencia nueva. Automatización local PASS; Device Simulator visual y Android físico siguen `NOT RUN`. La detección de setup no idempotente en Localization se corrigió y los assets conservan solo las dos keys nuevas. Siguiente: Prompt 14 data-driven.
