# Estado vivo del proyecto

Actualizado: 2026-08-16 23:06 (`America/Mexico_City`). Git, implementación y evidencia ejecutada prevalecen si contradicen este resumen.

## Fase y Gate

- **Fase implementada más reciente:** adopción técnica posterior a Prompt 18 — `VS-D-A01` es el único discovery/interacción animal runtime `Approved`; Prompt 19 aún no está implementado.
- **Preparación editorial H-007/H-008/H-009:** [`VS-D-A01`](VS_D_A01_TOUCAN_FACTUAL_DOSSIER.md) con Product/Localization, visual/rights/QA y revisión factual humana aprobados. La competencia factual declarada es investigación/búsqueda ampliada, no credencial ornitológica externa. Conservación y audio final permanecen fuera del contenido adoptado.
- **Auditoría actual:** Prompt 30 — [`Gate B`](audits/GATE_B_2026-08-16.md) ejecutado sobre HEAD real; `FAIL` por ausencia estructural de Fases 19–29 y del journey end-to-end.
- **Gate actual:** B — `FAIL`; no ejecutar Prompt 31 hasta completar y volver a auditar el loop mínimo.
- **Gate A:** [`audits/GATE_A_2026-08-15.md`](audits/GATE_A_2026-08-15.md) — `PASS`, sin Critical/Major abierto.
- **Siguiente fase:** Prompt 19 — cámara ficticia asistida; no cámara física, compartir ni permisos sensibles.
- **Último ExecPlan:** [`p18c-adopt-vs-d-a01-runtime.md`](../.agent/execplans/p18c-adopt-vs-d-a01-runtime.md), adopción de IDs/definitions/save completada antes de Prompt 19.

## Capacidades verificadas

| Capacidad | Estado | Evidencia/limitación |
|---|---|---|
| Git/producto/contrato | Verificado | Fases 00–06 auditadas; visión/GDD/MVP Selva y AGENTS canónicos. Cada preflight debe volver a contrastarlos. |
| Unity y assemblies | `PASS` | Unity `6000.3.22f1`, URP, nueve asmdefs, grafo acíclico; Domain/Application sin engine. |
| Pipeline local | `PASS` | `scripts/validate` código `0` en `4:31.06`: checks, compile/validadores, Addressables, EditMode `107/107`, PlayMode `18/18` y APK Development. |
| Bootstrap/servicios | `PASS` | MessageBus→Input→SafeArea→Haptics→Save→Localization→Audio→Analytics→Ads→Purchases; shutdown inverso y perfiles fail-closed. |
| Scene flow / Addressables | `PASS` | `4.0.1`; Boot→Camp↔Selva por `WorldManifest`, tres ciclos, sesión/handles controlados, 41 locations/896,909 bytes; sin endpoint/catálogo remoto. |
| Save local | `PASS` automatizado; dispositivo `NOT RUN` | Schema v5, JSON builtin, SHA-256, atomicidad/backup, v0→…→v5, future read-only; v4→v5 migra/mezcla el ID retirado del tucán y normaliza grants sin duplicarlos; sin PII/red. |
| Config runtime | `PASS` | Development/Release tipados, cero flags Release, sin remote config ni secretos. |
| Localización | `PASS` local | Localization `1.5.12`; ES/EN completos, pseudo Development y ocho keys nuevas aprobadas para nombre/claims del tucán; audio final sigue pendiente. |
| Audio | `PASS` estructural; final/hardware `NOT RUN` | Audio builtin `1.0.0`; 5 buses, 7 cues, 7 sources, 10 WAV `PH_` mono/48 kHz, queue/cooldown/ducking/replay/pause. `releaseFinal=0`. |
| Input/adaptación | `PASS` automatizado; hardware `NOT RUN` | Input System `1.20.0`; 5 mapas, safe area central, Back+checkpoint, no-op haptics, ratios 4:3/16:9/20:9/16:10 y supresión multitouch. Android físico requerido antes de Gate C. |
| Locomoción candidata | `PASS` automatizado; UX/hardware pendiente | AI Navigation `2.0.9`; tap válido/inválido, spam, UI/Photography, cancelación, reduce-motion, camera bounds, unload y Selva x3. Sin joystick/root motion; P-006 sigue abierto. |
| Interacción contextual | `PASS` automatizado; hardware pendiente | Foco único, auto-approach y prompt ES/EN; `interaction.jungle.keel-billed-toucan` es Approved y planta/objeto continúan `PH_`. Touch Android `NOT RUN`. |
| Discovery progression | `PASS` automatizado | First/repeat/already, count/día agregado y grants idempotentes operan sobre `discovery.jungle.keel-billed-toucan`; alias y save v5 preservan el placeholder retirado. No hay reward/economía/foto/álbum. |
| Contenido data-driven | Tucán Release `PASS`; proyecto Release `BLOCKED` | Catálogo O(1) contiene 1 discovery, 7 facts y 6 fuentes Approved; alias `discovery.jungle.placeholder → discovery.jungle.keel-billed-toucan`. Planta/objeto, mundo y audio final siguen bloqueando Release. |
| Expediente factual VS-D-A01 | Runtime `APPROVED` | H-007/H-008/H-009 cubren claims/copy, nombres, visual/rights/QA y revisión factual humana. Conservación excluida; no se atribuye credencial ornitológica externa. |
| Visual tucán VS-D-A01 | Runtime `APPROVED` | `visual.discovery.jungle.keel-billed-toucan`, prefab/materiales propios y ledger; discovery/interacción reales lo referencian sin `PH_`. |
| Mundos data-driven | Development `PASS`; Release `BLOCKED` | `world.jungle` compila desde manifest con escena/labels/spawn/checkpoint/catálogos/cues/version/tamaño. Fixture `world.test-ocean` prueba expansión sin switch; Release devuelve `WORLD018` por Draft/PH_. |
| Tests Unity | `PASS` | EditMode `107/107`; PlayMode `18/18`. Incluye catálogo Approved/alias y migración v4→v5 con merge/grants. |
| Android | Development `PASS`; Release `BLOCKED` | APK ES `105,983,639` bytes, SHA-256 `54876d08ce3de1b15b12f628471a23b17139f064dbdfdd92d278ca0bb333fdf9`, `190.013 s` Unity, API 26/36, IL2CPP/ARM64. Manifest: solo `INTERNET` heredado + permiso interno receiver; sin `CAMERA`, micrófono, ubicación, contactos ni `AD_ID`. Release ejecutado: exit `2` esperado por planta/objeto `INTERACTION005`, mundo `WORLD018` y signing; cero `DATA025`/`TOUCAN` para VS-D-A01. |
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
- Validación ornitológica externa independiente, traducción final, protocolo de playtests con menores, territorios y modelo comercial siguen pendientes; la primera no bloquea el Vertical Slice bajo la aprobación humana H-009, pero sí es mitigación recomendada antes de Release.
- El tucán runtime ya no es placeholder. Manifest Selva, planta/objeto y audio final continúan Draft/`PH_`; no aprobarlos por arrastre.
- Save v5 persiste grants sin compactación; medir crecimiento. La migración v4→v5 es específica y todo retiro futuro sigue exigiendo alias + migración.
- Tap-to-move/cámara necesitan playtest comparativo P-006; reduce-motion existe runtime pero todavía no está conectado a preferencia adulta/Save.
- Las tres interacciones y su UI son `PH_` Draft: no son contenido final; el animal concede solo discovery neutral Development. Target size, overlap, approach, audio y cancelación necesitan Android físico/playtest infantil antes de Gate C.
- Prompt 19 ya tiene animal/discovery/interacción/facts Approved y migrados. Conservación, audio final y publicación no quedan autorizados por ello.
- El APK final mide `25,011,428` bytes más que Prompt 17; la mayor parte visible está en binarios nativos comprimidos. No se atribuye causalidad a Discovery sin profiling; investigar reproducibilidad/strip/budget antes de Gate C.

## Reanudación inmediata

Ejecutar Prompt 19 sobre `interaction.jungle.keel-billed-toucan → discovery.jungle.keel-billed-toucan`, usando el alias/save v5 ya validados. No aprobar otros Draft por arrastre, introducir contenido masivo, cámara física/permisos, segundo mundo, remote, signing, push o publicación.
