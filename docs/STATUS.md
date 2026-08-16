# Estado vivo del proyecto

Actualizado: 2026-08-16 13:31 (`America/Mexico_City`). Git, implementación y evidencia ejecutada prevalecen si contradicen este resumen.

## Fase y Gate

- **Fase actual:** Prompt 14 — modelo data-driven, IDs, catálogo y validación completados.
- **Estado:** `PASS` Development local; Release permanece `BLOCKED` por cinco definitions Draft y signing externo.
- **Gate actual:** B — Vertical slice playable iniciado; Camp/Jungle todavía son placeholders sin gameplay.
- **Gate A:** [`audits/GATE_A_2026-08-15.md`](audits/GATE_A_2026-08-15.md) — `PASS`, sin Critical/Major abierto.
- **Siguiente fase:** Prompt 15 — framework extensible de mundos.
- **ExecPlan activo:** ninguno; [`p14-data-driven-content.md`](../.agent/execplans/p14-data-driven-content.md) está completado.

## Capacidades verificadas

| Capacidad | Estado | Evidencia/limitación |
|---|---|---|
| Git/producto/contrato | Verificado | Fases 00–06 auditadas; visión/GDD/MVP Selva y AGENTS canónicos. Cada preflight debe volver a contrastarlos. |
| Unity y assemblies | `PASS` | Unity `6000.3.22f1`, URP, nueve asmdefs, grafo acíclico; Domain/Application sin engine. |
| Pipeline local | `PASS` | Prompt 14: `scripts/validate` final código `0` en 3:51.84; checks, compile/catalog validator, Addressables, EditMode `85/85`, PlayMode `11/11` y APK. |
| Bootstrap/servicios | `PASS` | MessageBus→Input→SafeArea→Haptics→Save→Localization→Audio→Analytics→Ads→Purchases; shutdown inverso y perfiles fail-closed. |
| Scene flow / Addressables | `PASS` | `4.0.1`; Boot→Camp↔Jungle local, tres ciclos, handles controlados, diez clips PH_ en `SharedLocal`; sin endpoint/catálogo remoto. |
| Save local | `PASS` automatizado; dispositivo `NOT RUN` | Schema v3, JSON builtin, SHA-256, atomicidad/backup, v0→v1→v2→v3, future read-only y cinco volúmenes/subtítulos; sin PII/red. |
| Config runtime | `PASS` | Development/Release tipados, cero flags Release, sin remote config ni secretos. |
| Localización | `PASS` local | Localization `1.5.12`; 31 keys, ES/EN completos, pseudo Development, cinco slots Voice y tres subtítulos/cues localizados. |
| Audio | `PASS` estructural; final/hardware `NOT RUN` | Audio builtin `1.0.0`; 5 buses, 7 cues, 7 sources, 10 WAV `PH_` mono/48 kHz, queue/cooldown/ducking/replay/pause. `releaseFinal=0`. |
| Input/adaptación | `PASS` automatizado; hardware `NOT RUN` | Input System `1.20.0`; 5 mapas, safe area central, Back+checkpoint, no-op haptics, ratios 4:3/16:9/20:9/16:10 y supresión multitouch. Android físico requerido antes de Gate C. |
| Contenido data-driven | Development `PASS`; Release `BLOCKED` | Un discovery neutral Draft resuelve por `DiscoveryId` en catálogo O(1); IDs/referencias/aliases/editorial generan reports. Release devuelve `DATA025` por cinco assets Draft. |
| Tests Unity | `PASS` | EditMode `85/85`, 1.596 s; PlayMode `11/11`, 4.278 s. Incluye catálogo/traceability/Release Draft, InputTestFixture, save v3, audio, ES/EN/pseudo, scene flow y lifecycle. |
| Android | Development `PASS`; Release `BLOCKED` | APK ES `66,353,374` bytes, SHA-256 `d244ba03df0ab7c8699b012a9f6a484c63db4496ce7437f6f59c620f49298cea`, build IL2CPP 169.670 s, API 26/36, IL2CPP/ARM64. Release bloqueado por Draft antes de signing. |
| iOS/CI remota | `NOT RUN` | Sin Xcode/módulo iOS local. `origin` existe, pero runner/licencia/checks remotos no se ejecutaron; no hubo push. |
| Paquetes | Verificado | Audio builtin `1.0.0`, Localization `1.5.12`, AndroidJNI `1.0.0`, Addressables `4.0.1`; exactos, sin preview/SDK comercial. |
| Gameplay/assets finales | No existen | Camp/Jungle/UI/audio son foundation/placeholders. Prohibido escalar contenido antes del Vertical Slice. |

## Lectura para reanudar

1. [`../AGENTS.md`](../AGENTS.md), este archivo, [`README.md`](README.md), [`ROADMAP.md`](ROADMAP.md), [`DECISIONS.md`](DECISIONS.md) y [`RISK_REGISTER.md`](RISK_REGISTER.md).
2. [`02_TECHNICAL_ARCHITECTURE.md`](02_TECHNICAL_ARCHITECTURE.md), [`ENGINEERING_STANDARDS.md`](ENGINEERING_STANDARDS.md), [`VALIDATION_PLAYBOOK.md`](VALIDATION_PLAYBOOK.md) y [`.agent/PLANS.md`](../.agent/PLANS.md).
3. [`CONTENT_MODEL.md`](CONTENT_MODEL.md), [`CONTENT_PIPELINE.md`](CONTENT_PIPELINE.md), [`RUNTIME_CONFIGURATION.md`](RUNTIME_CONFIGURATION.md), [`10_SAVE_SYSTEM.md`](10_SAVE_SYSTEM.md), [`17_LOCALIZATION.md`](17_LOCALIZATION.md), [`16_AUDIO.md`](16_AUDIO.md) y [`18_TESTING.md`](18_TESTING.md).
4. Prompt 15 del [catálogo](prompts/00_MASTER_CODEX_PROMPTS.md), world design, scene flow, Addressables, config y arquitectura.

## Bloqueos y decisiones humanas

- Los diez clips `PH_` son tonos técnicos internos, no voz/música/SFX aprobados. Actor/compositor, licencias, pronunciación, mezcla, inteligibilidad ES/EN y prueba en altavoz/audífonos bloquean audio Release.
- `adb devices` no listó hardware: touch/rotación/notch/Back/haptics, reproducción/focus, instalación, pause/force-stop, filesystem lleno y kill del OS permanecen `NOT RUN` para dispositivo/Gate C.
- Signing, bundle/company definitivos, AAB/store, licencia/titularidad del producto y aprobación factual/comercial continúan humanos; el build Release permanece fail-closed.
- Xcode/iOS y CI Unity remota no están disponibles/verificados. No publicar, hacer push ni aceptar términos.
- Especialista factual, traducción final, protocolo de playtests con menores, territorios y modelo comercial siguen pendientes.
- Los cinco assets de catálogo son Draft `PH_`: fuente/revisor, fact copy, arte/audio final y aprobación por item bloquean Release; el report Development no los convierte en contenido aprobado.

## Reanudación inmediata

Ejecutar el preflight de Prompt 15, contrastar Git/tests y leer world design, content model/pipeline, scene flow, Addressables, config y arquitectura. Mantener Jungle como stub, sin segundo mundo real, contenido masivo, descarga remota, signing, push o publicación.
