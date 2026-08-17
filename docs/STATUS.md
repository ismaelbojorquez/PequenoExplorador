# Estado vivo del proyecto

Actualizado: 2026-08-16 21:25 (`America/Mexico_City`). Git, implementación y evidencia ejecutada prevalecen si contradicen este resumen.

## Fase y Gate

- **Fase implementada más reciente:** Prompt 18 — discovery persistente e idempotente completado y validado.
- **Preparación editorial H-007:** [`VS-D-A01`](VS_D_A01_TOUCAN_FACTUAL_DOSSIER.md) en `Sourced`; no es fase gameplay ni aprobación. Runtime permanece Draft/PH_.
- **Auditoría actual:** Prompt 30 — [`Gate B`](audits/GATE_B_2026-08-16.md) ejecutado sobre HEAD real; `FAIL` por ausencia estructural de Fases 19–29 y del journey end-to-end.
- **Gate actual:** B — `FAIL`; no ejecutar Prompt 31 hasta completar y volver a auditar el loop mínimo.
- **Gate A:** [`audits/GATE_A_2026-08-15.md`](audits/GATE_A_2026-08-15.md) — `PASS`, sin Critical/Major abierto.
- **Siguiente fase:** Prompt 19 — cámara fotográfica in-game, **todavía bloqueado** hasta firmas factual/Product/Localization/Art-Rights y assets Approved conforme al dossier; después continuar secuencialmente hasta Prompt 29 y repetir Gate B.
- **ExecPlan:** [`p18-persistent-discovery.md`](../.agent/execplans/p18-persistent-discovery.md), completo.

## Capacidades verificadas

| Capacidad | Estado | Evidencia/limitación |
|---|---|---|
| Git/producto/contrato | Verificado | Fases 00–06 auditadas; visión/GDD/MVP Selva y AGENTS canónicos. Cada preflight debe volver a contrastarlos. |
| Unity y assemblies | `PASS` | Unity `6000.3.22f1`, URP, nueve asmdefs, grafo acíclico; Domain/Application sin engine. |
| Pipeline local | `PASS` | Baseline previo al dossier: `scripts/validate` código `0` en `1:29.98`; checks, compile/validadores, Addressables, EditMode `103/103`, PlayMode `18/18` y APK. El cambio posterior es solo Markdown y requiere checks documentales. |
| Bootstrap/servicios | `PASS` | MessageBus→Input→SafeArea→Haptics→Save→Localization→Audio→Analytics→Ads→Purchases; shutdown inverso y perfiles fail-closed. |
| Scene flow / Addressables | `PASS` | `4.0.1`; Boot→Camp↔Selva por `WorldManifest`, tres ciclos, sesión/handles controlados, 41 locations/896,909 bytes; sin endpoint/catálogo remoto. |
| Save local | `PASS` automatizado; dispositivo `NOT RUN` | Schema v4, JSON builtin, SHA-256, atomicidad/backup, v0→v1→v2→v3→v4, future read-only, discovery records/grants y preferencias; sin PII/red. |
| Config runtime | `PASS` | Development/Release tipados, cero flags Release, sin remote config ni secretos. |
| Localización | `PASS` local | Localization `1.5.12`; 43 keys, ES/EN completos, pseudo Development, feedback discovery, cinco slots Voice y tres subtítulos/cues localizados. |
| Audio | `PASS` estructural; final/hardware `NOT RUN` | Audio builtin `1.0.0`; 5 buses, 7 cues, 7 sources, 10 WAV `PH_` mono/48 kHz, queue/cooldown/ducking/replay/pause. `releaseFinal=0`. |
| Input/adaptación | `PASS` automatizado; hardware `NOT RUN` | Input System `1.20.0`; 5 mapas, safe area central, Back+checkpoint, no-op haptics, ratios 4:3/16:9/20:9/16:10 y supresión multitouch. Android físico requerido antes de Gate C. |
| Locomoción candidata | `PASS` automatizado; UX/hardware pendiente | AI Navigation `2.0.9`; tap válido/inválido, spam, UI/Photography, cancelación, reduce-motion, camera bounds, unload y Selva x3. Sin joystick/root motion; P-006 sigue abierto. |
| Interacción contextual | `PASS` automatizado; hardware pendiente | Foco único, selección prioridad/distancia/ID, auto-approach, prompt safe-area ES/EN, cooldown y tres fixtures genéricos `PH_`; animal aporta discovery directo por datos. Touch Android `NOT RUN`. |
| Discovery progression | `PASS` automatizado; contenido final bloqueado | First/repeat/already/missing/unapproved, count/día local agregado, grants idempotentes, denominadores Approved y persistencia reload. Fixture neutral sigue Draft Development; no hay reward/economía/foto/álbum. |
| Contenido data-driven | Development `PASS`; Release `BLOCKED` | Un discovery neutral Draft resuelve por `DiscoveryId` en catálogo O(1); IDs/referencias/aliases/editorial generan reports. Release devuelve `DATA025` por cinco assets Draft. |
| Expediente factual VS-D-A01 | `Sourced`; aprobación `BLOCKED` | Seis fuentes institucionales/académicas, ocho claims y conflictos de nombre/conservación registrados. Reviewer/ApprovedBy/Rights/QA vacíos; ningún asset runtime cambió. |
| Mundos data-driven | Development `PASS`; Release `BLOCKED` | `world.jungle` compila desde manifest con escena/labels/spawn/checkpoint/catálogos/cues/version/tamaño. Fixture `world.test-ocean` prueba expansión sin switch; Release devuelve `WORLD018` por Draft/PH_. |
| Tests Unity | `PASS` | EditMode `103/103`, `2.141 s`; PlayMode `18/18`, `11.757 s`. Añade discovery first/repeat/idempotencia/queries/migración y persistencia por reload Selva. |
| Android | Development `PASS`; Release `BLOCKED` | Baseline H-007 produjo APK ES `105,942,573` bytes, SHA-256 `dabcca5f17f14c8e3d67d2f59fb482405113ca88387242e7245459189e0979c8`, `15.737 s` Unity, API 26/36, IL2CPP/ARM64 y zipalign 16 KB. Es foundation, no Vertical Slice; sin permiso CAMERA/sensible nuevo. |
| iOS/CI remota | `NOT RUN` | Sin Xcode/módulo iOS local. `origin` existe, pero runner/licencia/checks remotos no se ejecutaron; no hubo push. |
| Paquetes | Verificado | AI Navigation `2.0.9`, Audio builtin `1.0.0`, Localization `1.5.12`, AndroidJNI `1.0.0`, Addressables `4.0.1`; exactos, sin preview/SDK comercial. |
| Gameplay/assets finales | Locomoción + interacción + discovery directo `PH_` | No existen fotografía/álbum/economía ni assets finales. Camp/Jungle/UI/audio/personaje/fixtures son placeholders; prohibido escalar contenido antes del Vertical Slice. |
| Gate B journey | `FAIL` | No existen Fases 19–29, journey automatizado, actividad, fotografía, estrellas, misión, álbum, mejora ni FTUE. Cinco runs, primera/segunda sesión y UX no lectora: `NOT RUN`. |

## Lectura para reanudar

1. [`../AGENTS.md`](../AGENTS.md), este archivo, [`README.md`](README.md), [`ROADMAP.md`](ROADMAP.md), [`DECISIONS.md`](DECISIONS.md) y [`RISK_REGISTER.md`](RISK_REGISTER.md).
2. [`02_TECHNICAL_ARCHITECTURE.md`](02_TECHNICAL_ARCHITECTURE.md), [`ENGINEERING_STANDARDS.md`](ENGINEERING_STANDARDS.md), [`VALIDATION_PLAYBOOK.md`](VALIDATION_PLAYBOOK.md) y [`.agent/PLANS.md`](../.agent/PLANS.md).
3. [`CONTENT_MODEL.md`](CONTENT_MODEL.md), [`CONTENT_PIPELINE.md`](CONTENT_PIPELINE.md), [`RUNTIME_CONFIGURATION.md`](RUNTIME_CONFIGURATION.md), [`10_SAVE_SYSTEM.md`](10_SAVE_SYSTEM.md), [`17_LOCALIZATION.md`](17_LOCALIZATION.md), [`16_AUDIO.md`](16_AUDIO.md) y [`18_TESTING.md`](18_TESTING.md).
4. Prompt 19 del [catálogo](prompts/00_MASTER_CODEX_PROMPTS.md), [`INTERACTION_SYSTEM.md`](INTERACTION_SYSTEM.md), discovery, save, UI/accesibilidad/performance y arquitectura; verificar primero el blocker Approved.

## Bloqueos y decisiones humanas

- Los diez clips `PH_` son tonos técnicos internos, no voz/música/SFX aprobados. Actor/compositor, licencias, pronunciación, mezcla, inteligibilidad ES/EN y prueba en altavoz/audífonos bloquean audio Release.
- `adb devices` no listó hardware: touch/rotación/notch/Back/haptics, reproducción/focus, instalación, pause/force-stop, filesystem lleno y kill del OS permanecen `NOT RUN` para dispositivo/Gate C.
- Signing, bundle/company definitivos, AAB/store, licencia/titularidad del producto y aprobación factual/comercial continúan humanos; el build Release permanece fail-closed.
- Xcode/iOS y CI Unity remota no están disponibles/verificados. No publicar, hacer push ni aceptar términos.
- Especialista factual, traducción final, protocolo de playtests con menores, territorios y modelo comercial siguen pendientes.
- Los cinco assets de contenido y el manifest Selva son Draft `PH_`: fuente/revisor, fact copy, arte/audio final y aprobación por item bloquean Release; los reports Development no los convierten en contenido aprobado.
- Save v4 persiste grants sin compactación; medir crecimiento al añadir orígenes/catálogo real. IDs retirados se preservan y no cuentan en denominadores; alias/migración siguen obligatorios al renombrar contenido publicado.
- Tap-to-move/cámara necesitan playtest comparativo P-006; reduce-motion existe runtime pero todavía no está conectado a preferencia adulta/Save.
- Las tres interacciones y su UI son `PH_` Draft: no son contenido final; el animal concede solo discovery neutral Development. Target size, overlap, approach, audio y cancelación necesitan Android físico/playtest infantil antes de Gate C.
- Prompt 19 exige un animal fixture Approved, pero el único discovery/animal runtime sigue neutral Draft. VS-D-A01 ya está Sourced; H-007 conserva pendientes especialista factual, Product/Education, nombre regional, Art/Rights y QA.
- El APK final mide `25,011,428` bytes más que Prompt 17; la mayor parte visible está en binarios nativos comprimidos. No se atribuye causalidad a Discovery sin profiling; investigar reproducibilidad/strip/budget antes de Gate C.

## Reanudación inmediata

Completar las firmas humanas de [`VS-D-A01`](VS_D_A01_TOUCAN_FACTUAL_DOSSIER.md), crear después assets no-placeholder con validator/aliases aplicables y solo entonces ejecutar el preflight de Prompt 19. No marcar Draft como Approved, introducir contenido masivo, cámara física/permisos, segundo mundo, remote, signing, push o publicación.
