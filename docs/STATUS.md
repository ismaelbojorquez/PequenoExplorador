# Estado vivo del proyecto

Actualizado: 2026-08-16 12:27 (`America/Mexico_City`). Git, implementación y evidencia ejecutada prevalecen si contradicen este resumen.

## Fase y Gate

- **Fase actual:** Prompt 13 — input táctil, safe areas y adaptación de dispositivo completado.
- **Estado:** `PASS` local; Device Simulator visual y Android físico siguen `NOT RUN`, no son PASS.
- **Gate actual:** B — Vertical slice playable iniciado; Camp/Jungle todavía son placeholders sin gameplay.
- **Gate A:** [`audits/GATE_A_2026-08-15.md`](audits/GATE_A_2026-08-15.md) — `PASS`, sin Critical/Major abierto.
- **Siguiente fase:** Prompt 14 — modelo data-driven, IDs, catálogos y validación.
- **ExecPlan activo:** ninguno; [`p13-touch-input.md`](../.agent/execplans/p13-touch-input.md) está completado.

## Capacidades verificadas

| Capacidad | Estado | Evidencia/limitación |
|---|---|---|
| Git/producto/contrato | Verificado | Fases 00–06 auditadas; visión/GDD/MVP Selva y AGENTS canónicos. Cada preflight debe volver a contrastarlos. |
| Unity y assemblies | `PASS` | Unity `6000.3.22f1`, URP, nueve asmdefs, grafo acíclico; Domain/Application sin engine. |
| Pipeline local | `PASS` | Prompt 13: `scripts/validate` final código `0` en 1:18.93; checks, compile/input validator, Addressables, EditMode `77/77`, PlayMode `10/10` y APK. |
| Bootstrap/servicios | `PASS` | MessageBus→Input→SafeArea→Haptics→Save→Localization→Audio→Analytics→Ads→Purchases; shutdown inverso y perfiles fail-closed. |
| Scene flow / Addressables | `PASS` | `4.0.1`; Boot→Camp↔Jungle local, tres ciclos, handles controlados, diez clips PH_ en `SharedLocal`; sin endpoint/catálogo remoto. |
| Save local | `PASS` automatizado; dispositivo `NOT RUN` | Schema v3, JSON builtin, SHA-256, atomicidad/backup, v0→v1→v2→v3, future read-only y cinco volúmenes/subtítulos; sin PII/red. |
| Config runtime | `PASS` | Development/Release tipados, cero flags Release, sin remote config ni secretos. |
| Localización | `PASS` local | Localization `1.5.12`; 30 keys, ES/EN completos, pseudo Development, cinco slots Voice y tres subtítulos/cues localizados. |
| Audio | `PASS` estructural; final/hardware `NOT RUN` | Audio builtin `1.0.0`; 5 buses, 7 cues, 7 sources, 10 WAV `PH_` mono/48 kHz, queue/cooldown/ducking/replay/pause. `releaseFinal=0`. |
| Input/adaptación | `PASS` automatizado; hardware `NOT RUN` | Input System `1.20.0`; 5 mapas, safe area central, Back+checkpoint, no-op haptics, ratios 4:3/16:9/20:9/16:10 y supresión multitouch. Android físico requerido antes de Gate C. |
| Tests Unity | `PASS` | EditMode `77/77`, 1.735 s; PlayMode `10/10`, 4.007 s. Incluye InputTestFixture, save v3, audio, ES/EN/pseudo, scene flow y lifecycle. |
| Android | Development `PASS`; Release `BLOCKED` | APK ES `66,067,652` bytes, SHA-256 `c19c68eacf50dfb61916c1eaa09c1c787bf452fd12f7e15784206fe898600d26`, build cache 15.654 s, API 26/36, IL2CPP/ARM64. Release código esperado `3`. |
| iOS/CI remota | `NOT RUN` | Sin Xcode/módulo iOS local. `origin` existe, pero runner/licencia/checks remotos no se ejecutaron; no hubo push. |
| Paquetes | Verificado | Audio builtin `1.0.0`, Localization `1.5.12`, AndroidJNI `1.0.0`, Addressables `4.0.1`; exactos, sin preview/SDK comercial. |
| Gameplay/assets finales | No existen | Camp/Jungle/UI/audio son foundation/placeholders. Prohibido escalar contenido antes del Vertical Slice. |

## Lectura para reanudar

1. [`../AGENTS.md`](../AGENTS.md), este archivo, [`README.md`](README.md), [`ROADMAP.md`](ROADMAP.md), [`DECISIONS.md`](DECISIONS.md) y [`RISK_REGISTER.md`](RISK_REGISTER.md).
2. [`02_TECHNICAL_ARCHITECTURE.md`](02_TECHNICAL_ARCHITECTURE.md), [`ENGINEERING_STANDARDS.md`](ENGINEERING_STANDARDS.md), [`VALIDATION_PLAYBOOK.md`](VALIDATION_PLAYBOOK.md) y [`.agent/PLANS.md`](../.agent/PLANS.md).
3. [`RUNTIME_CONFIGURATION.md`](RUNTIME_CONFIGURATION.md), [`10_SAVE_SYSTEM.md`](10_SAVE_SYSTEM.md), [`17_LOCALIZATION.md`](17_LOCALIZATION.md), [`16_AUDIO.md`](16_AUDIO.md), [`AUDIO_REQUIREMENTS.md`](AUDIO_REQUIREMENTS.md), [`18_TESTING.md`](18_TESTING.md) y [`CONTENT_PIPELINE.md`](CONTENT_PIPELINE.md).
4. Prompt 14 del [catálogo](prompts/00_MASTER_CODEX_PROMPTS.md), modelo de contenido, educación, discovery/learning/missions y asset requirements.

## Bloqueos y decisiones humanas

- Los diez clips `PH_` son tonos técnicos internos, no voz/música/SFX aprobados. Actor/compositor, licencias, pronunciación, mezcla, inteligibilidad ES/EN y prueba en altavoz/audífonos bloquean audio Release.
- `adb devices` no listó hardware: touch/rotación/notch/Back/haptics, reproducción/focus, instalación, pause/force-stop, filesystem lleno y kill del OS permanecen `NOT RUN` para dispositivo/Gate C.
- Signing, bundle/company definitivos, AAB/store, licencia/titularidad del producto y aprobación factual/comercial continúan humanos; el build Release permanece fail-closed.
- Xcode/iOS y CI Unity remota no están disponibles/verificados. No publicar, hacer push ni aceptar términos.
- Especialista factual, traducción final, protocolo de playtests con menores, territorios y modelo comercial siguen pendientes.

## Reanudación inmediata

Ejecutar el preflight de Prompt 14, contrastar Git/tests y leer content model, educación, discovery/learning/missions, localización, asset requirements y arquitectura. No escalar contenido, fijar tap-to-move, añadir haptics físicos, signing, push o publicación.
