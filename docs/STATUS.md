# Estado vivo del proyecto

Actualizado: 2026-08-16 14:43 (`America/Mexico_City`). Git, implementación y evidencia ejecutada prevalecen si contradicen este resumen.

## Fase y Gate

- **Fase actual:** Prompt 16 — explorador, cámara asistida y tap-to-move completado.
- **Estado:** `PASS` local; prototipo `PH_`, suites, Addressables y APK pasan. Tap-to-move sigue candidato hasta playtest; Release bloqueado por placeholders/signing.
- **Gate actual:** B — Vertical slice playable iniciado; Camp/Jungle todavía son placeholders sin gameplay.
- **Gate A:** [`audits/GATE_A_2026-08-15.md`](audits/GATE_A_2026-08-15.md) — `PASS`, sin Critical/Major abierto.
- **Siguiente fase:** Prompt 17 — interacción contextual accesible; requiere preflight nuevo y no hereda este PASS.
- **ExecPlan cerrado:** [`p16-explorer-tap-to-move.md`](../.agent/execplans/p16-explorer-tap-to-move.md).

## Capacidades verificadas

| Capacidad | Estado | Evidencia/limitación |
|---|---|---|
| Git/producto/contrato | Verificado | Fases 00–06 auditadas; visión/GDD/MVP Selva y AGENTS canónicos. Cada preflight debe volver a contrastarlos. |
| Unity y assemblies | `PASS` | Unity `6000.3.22f1`, URP, nueve asmdefs, grafo acíclico; Domain/Application sin engine. |
| Pipeline local | `PASS` | Prompt 16: `scripts/validate` código `0` en `4:23.73`; checks, compile/validators, Addressables, EditMode `94/94`, PlayMode `14/14` y APK. |
| Bootstrap/servicios | `PASS` | MessageBus→Input→SafeArea→Haptics→Save→Localization→Audio→Analytics→Ads→Purchases; shutdown inverso y perfiles fail-closed. |
| Scene flow / Addressables | `PASS` | `4.0.1`; Boot→Camp↔Selva por `WorldManifest`, tres ciclos, sesión/handles controlados, 41 locations/893,594 bytes; sin endpoint/catálogo remoto. |
| Save local | `PASS` automatizado; dispositivo `NOT RUN` | Schema v3, JSON builtin, SHA-256, atomicidad/backup, v0→v1→v2→v3, future read-only y cinco volúmenes/subtítulos; sin PII/red. |
| Config runtime | `PASS` | Development/Release tipados, cero flags Release, sin remote config ni secretos. |
| Localización | `PASS` local | Localization `1.5.12`; 31 keys, ES/EN completos, pseudo Development, cinco slots Voice y tres subtítulos/cues localizados. |
| Audio | `PASS` estructural; final/hardware `NOT RUN` | Audio builtin `1.0.0`; 5 buses, 7 cues, 7 sources, 10 WAV `PH_` mono/48 kHz, queue/cooldown/ducking/replay/pause. `releaseFinal=0`. |
| Input/adaptación | `PASS` automatizado; hardware `NOT RUN` | Input System `1.20.0`; 5 mapas, safe area central, Back+checkpoint, no-op haptics, ratios 4:3/16:9/20:9/16:10 y supresión multitouch. Android físico requerido antes de Gate C. |
| Locomoción candidata | `PASS` automatizado; UX/hardware pendiente | AI Navigation `2.0.9`; tap válido/inválido, spam, UI/Photography, cancelación, reduce-motion, camera bounds, unload y Selva x3. Sin joystick/root motion; P-006 sigue abierto. |
| Contenido data-driven | Development `PASS`; Release `BLOCKED` | Un discovery neutral Draft resuelve por `DiscoveryId` en catálogo O(1); IDs/referencias/aliases/editorial generan reports. Release devuelve `DATA025` por cinco assets Draft. |
| Mundos data-driven | Development `PASS`; Release `BLOCKED` | `world.jungle` compila desde manifest con escena/labels/spawn/checkpoint/catálogos/cues/version/tamaño. Fixture `world.test-ocean` prueba expansión sin switch; Release devuelve `WORLD018` por Draft/PH_. |
| Tests Unity | `PASS` | EditMode `94/94`, `1.719 s`; PlayMode `14/14`, `4.876 s`. Añade locomoción pura, taps/path, maps, reduce motion, FPS Editor y root/unload x3. |
| Android | Development `PASS`; Release `BLOCKED` | APK ES `80,860,082` bytes, SHA-256 final `1d04f40d81a34794d18949ccc562cb768ffcd6afdd69a5df2bfb898e909b7e79`, repetición staged `49.551 s` Unity/`68.07 s` wrapper, API 26/36, IL2CPP/ARM64, zipalign/7 ELF 16 KB. Release bloqueado por Draft antes de signing. |
| iOS/CI remota | `NOT RUN` | Sin Xcode/módulo iOS local. `origin` existe, pero runner/licencia/checks remotos no se ejecutaron; no hubo push. |
| Paquetes | Verificado | AI Navigation `2.0.9`, Audio builtin `1.0.0`, Localization `1.5.12`, AndroidJNI `1.0.0`, Addressables `4.0.1`; exactos, sin preview/SDK comercial. |
| Gameplay/assets finales | Solo locomoción `PH_` | No existen interacción/discovery/foto ni assets finales. Camp/Jungle/UI/audio/personaje son placeholders; prohibido escalar contenido antes del Vertical Slice. |

## Lectura para reanudar

1. [`../AGENTS.md`](../AGENTS.md), este archivo, [`README.md`](README.md), [`ROADMAP.md`](ROADMAP.md), [`DECISIONS.md`](DECISIONS.md) y [`RISK_REGISTER.md`](RISK_REGISTER.md).
2. [`02_TECHNICAL_ARCHITECTURE.md`](02_TECHNICAL_ARCHITECTURE.md), [`ENGINEERING_STANDARDS.md`](ENGINEERING_STANDARDS.md), [`VALIDATION_PLAYBOOK.md`](VALIDATION_PLAYBOOK.md) y [`.agent/PLANS.md`](../.agent/PLANS.md).
3. [`CONTENT_MODEL.md`](CONTENT_MODEL.md), [`CONTENT_PIPELINE.md`](CONTENT_PIPELINE.md), [`RUNTIME_CONFIGURATION.md`](RUNTIME_CONFIGURATION.md), [`10_SAVE_SYSTEM.md`](10_SAVE_SYSTEM.md), [`17_LOCALIZATION.md`](17_LOCALIZATION.md), [`16_AUDIO.md`](16_AUDIO.md) y [`18_TESTING.md`](18_TESTING.md).
4. Prompt 16 del [catálogo](prompts/00_MASTER_CODEX_PROMPTS.md), GDD, input/UI, world design, art direction, accessibility y arquitectura.

## Bloqueos y decisiones humanas

- Los diez clips `PH_` son tonos técnicos internos, no voz/música/SFX aprobados. Actor/compositor, licencias, pronunciación, mezcla, inteligibilidad ES/EN y prueba en altavoz/audífonos bloquean audio Release.
- `adb devices` no listó hardware: touch/rotación/notch/Back/haptics, reproducción/focus, instalación, pause/force-stop, filesystem lleno y kill del OS permanecen `NOT RUN` para dispositivo/Gate C.
- Signing, bundle/company definitivos, AAB/store, licencia/titularidad del producto y aprobación factual/comercial continúan humanos; el build Release permanece fail-closed.
- Xcode/iOS y CI Unity remota no están disponibles/verificados. No publicar, hacer push ni aceptar términos.
- Especialista factual, traducción final, protocolo de playtests con menores, territorios y modelo comercial siguen pendientes.
- Los cinco assets de contenido y el manifest Selva son Draft `PH_`: fuente/revisor, fact copy, arte/audio final y aprobación por item bloquean Release; los reports Development no los convierten en contenido aprobado.
- Save v3 no persiste mundo/checkpoint real; la reconciliación de IDs retirados deberá acompañar el primer cambio de schema que los guarde. El resultado `Missing` actual protege el progreso sin mutarlo.
- APK creció `14,386,460` bytes (`21.64%`) frente a Prompt 15 al añadir AI/Physics y el stub. Tres builds repitieron tamaño, permisos/native fueron seguros; separar costo real y fijar budget en profiling Android físico antes de Gate C.
- Tap-to-move/cámara necesitan playtest comparativo P-006; reduce-motion existe runtime pero todavía no está conectado a preferencia adulta/Save.

## Reanudación inmediata

Ejecutar preflight de Prompt 17 y leer interacción/gameplay/UI/accesibilidad más Explorer/Input reales. Añadir solo focus/acercamiento/interacción contextual con fixtures `PH_`; no introducir discovery/fotografía final, contenido masivo, segundo mundo, remote, signing, push o publicación.
