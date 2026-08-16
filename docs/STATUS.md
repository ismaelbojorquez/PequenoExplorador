# Estado vivo del proyecto

Actualizado: 2026-08-16 11:47 (`America/Mexico_City`). Git, implementación y evidencia ejecutada prevalecen si contradicen este resumen.

## Fase y Gate

- **Fase actual:** Prompt 12 — framework de audio infantil completado; no equivale a Fase 12 del roadmap histórico.
- **Estado:** `PASS` local. Audio final/licencias y prueba auditiva física siguen pendientes humanos; no están aprobados.
- **Gate actual:** B — Vertical slice playable iniciado; Camp/Jungle todavía son placeholders sin gameplay.
- **Gate A:** [`audits/GATE_A_2026-08-15.md`](audits/GATE_A_2026-08-15.md) — `PASS`, sin Critical/Major abierto.
- **Siguiente fase:** Prompt 13 — input táctil, safe areas y adaptación de dispositivo.
- **ExecPlan activo:** ninguno; [Prompt 12](../.agent/execplans/p12-audio-framework.md) está completado.

## Capacidades verificadas

| Capacidad | Estado | Evidencia/limitación |
|---|---|---|
| Git/producto/contrato | Verificado | Fases 00–06 auditadas; visión/GDD/MVP Selva y AGENTS canónicos. Cada preflight debe volver a contrastarlos. |
| Unity y assemblies | `PASS` | Unity `6000.3.22f1`, URP, nueve asmdefs, grafo acíclico; Domain/Application sin engine. |
| Pipeline local | `PASS` | Prompt 12: `scripts/validate` código `0`; checks, compile/validators, Addressables, EditMode `70/70`, PlayMode `7/7` y APK. Logs 11:44–11:45 en `artifacts/`. |
| Bootstrap/servicios | `PASS` | MessageBus→Save→Localization→Audio→Analytics→Ads→Purchases; shutdown inverso, retry/cancel/cleanup y perfiles fail-closed. |
| Scene flow / Addressables | `PASS` | `4.0.1`; Boot→Camp↔Jungle local, tres ciclos, handles controlados, diez clips PH_ en `SharedLocal`; sin endpoint/catálogo remoto. |
| Save local | `PASS` automatizado; dispositivo `NOT RUN` | Schema v3, JSON builtin, SHA-256, atomicidad/backup, v0→v1→v2→v3, future read-only y cinco volúmenes/subtítulos; sin PII/red. |
| Config runtime | `PASS` | Development/Release tipados, cero flags Release, sin remote config ni secretos. |
| Localización | `PASS` local | Localization `1.5.12`; 28 keys, ES/EN completos, pseudo Development, cinco slots Voice y tres subtítulos/cues localizados. |
| Audio | `PASS` estructural; final/hardware `NOT RUN` | Audio builtin `1.0.0`; 5 buses, 7 cues, 7 sources, 10 WAV `PH_` mono/48 kHz, queue/cooldown/ducking/replay/pause. `releaseFinal=0`. |
| Tests Unity | `PASS` | EditMode `70/70`, 1.60 s; PlayMode `7/7`, 3.39 s. Incluye save v3, audio, ES/EN/pseudo, scene flow y lifecycle. |
| Android | Development `PASS`; Release `BLOCKED` | APK ES `66,037,223` bytes, SHA-256 `9564026c1dae24c69d3f96ff4ac46650267a2fad9f2677c63a9ddacc614ec046`, build 16.472 s cache, API 26/36, IL2CPP/ARM64, zipalign 16 KB. Release código esperado `3`. |
| iOS/CI remota | `NOT RUN` | Sin Xcode/módulo iOS local. `origin` existe, pero runner/licencia/checks remotos no se ejecutaron; no hubo push. |
| Paquetes | Verificado | Audio builtin `1.0.0`, Localization `1.5.12`, AndroidJNI `1.0.0`, Addressables `4.0.1`; exactos, sin preview/SDK comercial. |
| Gameplay/assets finales | No existen | Camp/Jungle/UI/audio son foundation/placeholders. Prohibido escalar contenido antes del Vertical Slice. |

## Lectura para reanudar

1. [`../AGENTS.md`](../AGENTS.md), este archivo, [`README.md`](README.md), [`ROADMAP.md`](ROADMAP.md), [`DECISIONS.md`](DECISIONS.md) y [`RISK_REGISTER.md`](RISK_REGISTER.md).
2. [`02_TECHNICAL_ARCHITECTURE.md`](02_TECHNICAL_ARCHITECTURE.md), [`ENGINEERING_STANDARDS.md`](ENGINEERING_STANDARDS.md), [`VALIDATION_PLAYBOOK.md`](VALIDATION_PLAYBOOK.md) y [`.agent/PLANS.md`](../.agent/PLANS.md).
3. [`RUNTIME_CONFIGURATION.md`](RUNTIME_CONFIGURATION.md), [`10_SAVE_SYSTEM.md`](10_SAVE_SYSTEM.md), [`17_LOCALIZATION.md`](17_LOCALIZATION.md), [`16_AUDIO.md`](16_AUDIO.md), [`AUDIO_REQUIREMENTS.md`](AUDIO_REQUIREMENTS.md), [`18_TESTING.md`](18_TESTING.md) y [`CONTENT_PIPELINE.md`](CONTENT_PIPELINE.md).
4. Prompt 13 del [catálogo](prompts/00_MASTER_CODEX_PROMPTS.md), UI/UX y archivos Input existentes antes de editar.

## Bloqueos y decisiones humanas

- Los diez clips `PH_` son tonos técnicos internos, no voz/música/SFX aprobados. Actor/compositor, licencias, pronunciación, mezcla, inteligibilidad ES/EN y prueba en altavoz/audífonos bloquean audio Release.
- `adb devices` no listó hardware: reproducción/focus real, instalación, pause/force-stop, filesystem lleno y kill del OS permanecen `NOT RUN` para dispositivo/Gate C.
- Signing, bundle/company definitivos, AAB/store, licencia/titularidad del producto y aprobación factual/comercial continúan humanos; el build Release permanece fail-closed.
- Xcode/iOS y CI Unity remota no están disponibles/verificados. No publicar, hacer push ni aceptar términos.
- Especialista factual, traducción final, protocolo de playtests con menores, territorios y modelo comercial siguen pendientes.

## Reanudación inmediata

Ejecutar el preflight de Prompt 13, contrastar Git/tests y leer UI/UX, Input System, arquitectura, audio y status. No ampliar a gameplay final, joystick permanente, haptics invasivos, remote config, contenido masivo, signing, push o publicación.
