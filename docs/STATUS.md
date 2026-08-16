# Estado vivo del proyecto

Actualizado: 2026-08-16 15:23 (`America/Mexico_City`). Git, implementación y evidencia ejecutada prevalecen si contradicen este resumen.

## Fase y Gate

- **Fase actual:** Prompt 17 — interacción contextual accesible completada.
- **Estado:** `PASS` local automatizado; Android físico/playtest permanecen `NOT RUN`.
- **Gate actual:** B — Vertical slice playable iniciado; Camp/Jungle todavía son placeholders sin gameplay.
- **Gate A:** [`audits/GATE_A_2026-08-15.md`](audits/GATE_A_2026-08-15.md) — `PASS`, sin Critical/Major abierto.
- **Siguiente fase:** Prompt 18 — discovery y progreso, solo después de cerrar Prompt 17.
- **ExecPlan activo:** ninguno. El plan cerrado es [`p17-contextual-interaction.md`](../.agent/execplans/p17-contextual-interaction.md).

## Capacidades verificadas

| Capacidad | Estado | Evidencia/limitación |
|---|---|---|
| Git/producto/contrato | Verificado | Fases 00–06 auditadas; visión/GDD/MVP Selva y AGENTS canónicos. Cada preflight debe volver a contrastarlos. |
| Unity y assemblies | `PASS` | Unity `6000.3.22f1`, URP, nueve asmdefs, grafo acíclico; Domain/Application sin engine. |
| Pipeline local | `PASS` | Prompt 17 final: `scripts/validate` código `0` en `2:08.77`; checks, compile/validadores, Addressables, EditMode `99/99`, PlayMode `17/17` y APK. |
| Bootstrap/servicios | `PASS` | MessageBus→Input→SafeArea→Haptics→Save→Localization→Audio→Analytics→Ads→Purchases; shutdown inverso y perfiles fail-closed. |
| Scene flow / Addressables | `PASS` | `4.0.1`; Boot→Camp↔Selva por `WorldManifest`, tres ciclos, sesión/handles controlados, 41 locations/896,715 bytes; sin endpoint/catálogo remoto. |
| Save local | `PASS` automatizado; dispositivo `NOT RUN` | Schema v3, JSON builtin, SHA-256, atomicidad/backup, v0→v1→v2→v3, future read-only y cinco volúmenes/subtítulos; sin PII/red. |
| Config runtime | `PASS` | Development/Release tipados, cero flags Release, sin remote config ni secretos. |
| Localización | `PASS` local | Localization `1.5.12`; 40 keys, ES/EN completos, pseudo Development, cinco slots Voice y tres subtítulos/cues localizados. |
| Audio | `PASS` estructural; final/hardware `NOT RUN` | Audio builtin `1.0.0`; 5 buses, 7 cues, 7 sources, 10 WAV `PH_` mono/48 kHz, queue/cooldown/ducking/replay/pause. `releaseFinal=0`. |
| Input/adaptación | `PASS` automatizado; hardware `NOT RUN` | Input System `1.20.0`; 5 mapas, safe area central, Back+checkpoint, no-op haptics, ratios 4:3/16:9/20:9/16:10 y supresión multitouch. Android físico requerido antes de Gate C. |
| Locomoción candidata | `PASS` automatizado; UX/hardware pendiente | AI Navigation `2.0.9`; tap válido/inválido, spam, UI/Photography, cancelación, reduce-motion, camera bounds, unload y Selva x3. Sin joystick/root motion; P-006 sigue abierto. |
| Interacción contextual | `PASS` dirigido; hardware pendiente | Foco único, selección prioridad/distancia/ID, auto-approach, prompt safe-area ES/EN, cooldown/idempotencia y tres fixtures genéricos `PH_`. EditMode `99/99`; PlayMode `17/17`; touch Android `NOT RUN`. |
| Contenido data-driven | Development `PASS`; Release `BLOCKED` | Un discovery neutral Draft resuelve por `DiscoveryId` en catálogo O(1); IDs/referencias/aliases/editorial generan reports. Release devuelve `DATA025` por cinco assets Draft. |
| Mundos data-driven | Development `PASS`; Release `BLOCKED` | `world.jungle` compila desde manifest con escena/labels/spawn/checkpoint/catálogos/cues/version/tamaño. Fixture `world.test-ocean` prueba expansión sin switch; Release devuelve `WORLD018` por Draft/PH_. |
| Tests Unity | `PASS` | EditMode `99/99`, `1.904 s`; PlayMode `17/17`, `7.417 s`. Añade selección/rango/cooldown/cancel y tap/approach/spam/unavailable/destroy/unload. |
| Android | Development `PASS`; Release `BLOCKED` | APK ES `80,931,145` bytes, SHA-256 `752b0fd41eb0558d6fa162d8fa8137cde46c08350645842d56f3ff508fe8a4f4`, `59.545 s` Unity, API 26/36, IL2CPP/ARM64, zipalign/7 ELF `0x4000`. Release rechaza 3 interacciones `PH_` (`INTERACTION005`) antes de signing. |
| iOS/CI remota | `NOT RUN` | Sin Xcode/módulo iOS local. `origin` existe, pero runner/licencia/checks remotos no se ejecutaron; no hubo push. |
| Paquetes | Verificado | AI Navigation `2.0.9`, Audio builtin `1.0.0`, Localization `1.5.12`, AndroidJNI `1.0.0`, Addressables `4.0.1`; exactos, sin preview/SDK comercial. |
| Gameplay/assets finales | Locomoción + interacción `PH_` | No existen discovery/foto ni assets finales. Camp/Jungle/UI/audio/personaje/fixtures son placeholders; prohibido escalar contenido antes del Vertical Slice. |

## Lectura para reanudar

1. [`../AGENTS.md`](../AGENTS.md), este archivo, [`README.md`](README.md), [`ROADMAP.md`](ROADMAP.md), [`DECISIONS.md`](DECISIONS.md) y [`RISK_REGISTER.md`](RISK_REGISTER.md).
2. [`02_TECHNICAL_ARCHITECTURE.md`](02_TECHNICAL_ARCHITECTURE.md), [`ENGINEERING_STANDARDS.md`](ENGINEERING_STANDARDS.md), [`VALIDATION_PLAYBOOK.md`](VALIDATION_PLAYBOOK.md) y [`.agent/PLANS.md`](../.agent/PLANS.md).
3. [`CONTENT_MODEL.md`](CONTENT_MODEL.md), [`CONTENT_PIPELINE.md`](CONTENT_PIPELINE.md), [`RUNTIME_CONFIGURATION.md`](RUNTIME_CONFIGURATION.md), [`10_SAVE_SYSTEM.md`](10_SAVE_SYSTEM.md), [`17_LOCALIZATION.md`](17_LOCALIZATION.md), [`16_AUDIO.md`](16_AUDIO.md) y [`18_TESTING.md`](18_TESTING.md).
4. Prompt 18 del [catálogo](prompts/00_MASTER_CODEX_PROMPTS.md), [`INTERACTION_SYSTEM.md`](INTERACTION_SYSTEM.md), discovery, save, GDD, input/UI y arquitectura.

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
- Las tres interacciones y su UI son `PH_` Draft: no son contenido final ni conceden discovery. Target size, overlap, approach, audio y cancelación necesitan Android físico/playtest infantil antes de Gate C.

## Reanudación inmediata

Ejecutar el preflight de Prompt 18 para conectar discovery/progreso mediante caso de uso sobre `IInteractable`; no introducir fotografía, contenido masivo, segundo mundo, remote, signing, push o publicación.
