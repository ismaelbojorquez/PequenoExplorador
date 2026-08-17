# Estado vivo del proyecto

Actualizado: 2026-08-17 (`America/Mexico_City`). Git, implementación y evidencia ejecutada prevalecen si contradicen este resumen.

## Fase y Gate

- **Fase implementada más reciente:** Prompt 25 — hub Camp progresivo y primera mejora atómica/persistente.
- **Preparación editorial H-007/H-008/H-009:** [`VS-D-A01`](VS_D_A01_TOUCAN_FACTUAL_DOSSIER.md) con Product/Localization, visual/rights/QA y revisión factual humana aprobados. La competencia factual declarada es investigación/búsqueda ampliada, no credencial ornitológica externa. Conservación y audio final permanecen fuera del contenido adoptado.
- **Auditoría actual:** Prompt 30 — [`Gate B`](audits/GATE_B_2026-08-16.md) ejecutado sobre HEAD real; `FAIL` por ausencia estructural de Fases 19–29 y del journey end-to-end.
- **Gate actual:** B — `FAIL`; no ejecutar Prompt 31 hasta completar y volver a auditar el loop mínimo.
- **Gate A:** [`audits/GATE_A_2026-08-15.md`](audits/GATE_A_2026-08-15.md) — `PASS`, sin Critical/Major abierto.
- **Siguiente fase:** Prompt 26 — personalización inclusiva por slots; reutilizar Camp/Economy/Save sin monetización real.
- **ExecPlan activo:** ninguno. El plan completado de Prompt 25 queda en [`.agent/execplans/p25-progressive-camp-hub.md`](../.agent/execplans/p25-progressive-camp-hub.md).

## Capacidades verificadas

| Capacidad | Estado | Evidencia/limitación |
|---|---|---|
| Git/producto/contrato | Verificado | Fases 00–06 auditadas; visión/GDD/MVP Selva y AGENTS canónicos. Cada preflight debe volver a contrastarlos. |
| Unity y assemblies | `PASS` | Unity `6000.3.22f1`, URP, nueve asmdefs, grafo acíclico; Domain/Application sin engine. |
| Pipeline local | `PASS` | `scripts/validate` final código `0`: repository/shell, compile/validadores, Addressables, EditMode `147/147`, PlayMode `25/25` y APK Development. |
| Bootstrap/servicios | `PASS` | MessageBus→Input→SafeArea→Haptics→Save→Photos→Localization→Audio→Analytics→Ads→Purchases; shutdown inverso y perfiles fail-closed. |
| Scene flow / Addressables | `PASS` | `4.0.1`; Boot→Camp↔Selva por `WorldManifest`, tres ciclos, sesión/handles controlados, 47 locations/1,282,243 bytes; sin endpoint/catálogo remoto. |
| Save local | `PASS` automatizado; dispositivo `NOT RUN` | Schema v10, JSON builtin, SHA-256, atomicidad/backup, v0→…→v10 y future read-only; v9→v10 conserva progreso y añade unlocks Camp vacíos. PNG/manifest viven separados; sin PII/red/pixels. |
| Config runtime | `PASS` | Development/Release tipados, cero flags Release, sin remote config ni secretos. |
| Localización | `PASS` local | Localization `1.5.12`; 140 entries runtime ES/EN, pseudo Development; cámara, álbum, economía, misión, learning/actividad y Camp actualizan copy sin reinicio. Audio final sigue pendiente. |
| Audio | `PASS` estructural; final/hardware `NOT RUN` | Audio builtin `1.0.0`; 5 buses, 9 cues, 7 sources, 14 WAV `PH_` mono/48 kHz, incluidos instrucción/fact de actividad; queue/cooldown/ducking/replay/pause. `releaseFinal=0`. |
| Input/adaptación | `PASS` automatizado; hardware `NOT RUN` | Input System `1.20.0`; 5 mapas, safe area central, Back+checkpoint, no-op haptics, ratios 4:3/16:9/20:9/16:10 y supresión multitouch. Android físico requerido antes de Gate C. |
| Locomoción candidata | `PASS` automatizado; UX/hardware pendiente | AI Navigation `2.0.9`; tap válido/inválido, spam, UI/Photography, cancelación, reduce-motion, camera bounds, unload y Selva x3. Sin joystick/root motion; P-006 sigue abierto. |
| Interacción contextual | `PASS` automatizado; hardware pendiente | Foco único, auto-approach y prompt ES/EN; `interaction.jungle.keel-billed-toucan` es Approved y planta/objeto continúan `PH_`. Touch Android `NOT RUN`. |
| Discovery/fotografía | `PASS` automatizado; hardware pendiente | Interacción→Photography; evaluator `0.08/10/0.36/0.35` + LOS; inválido amable, válido first/repeat, best-photo, failure fallback y grants idempotentes. Thumbnail `384×216`; recursos temporales vuelven a 0. |
| Álbum | `PASS` automatizado; visual/hardware pendiente | Query Approved + discovery/photo, locked sin fuga, contadores/filtro, detalle/fallback, pool/caché 8, cancellation, ES/EN/pseudo y 4:3/16:9/20:9/16:10. Tamaño y audio usan fallback; UI `PH_`. |
| Economía | `PASS` automatizado; tuning/playtest pendiente | Una moneda `ExplorerStars`, 4 definitions (3 no-debug), grant/spend idempotente, transaction keys durables, ledger 32 y mejora Camp por 3 provisional. Debug solo Development; sin IAP/premium/azar. |
| Camp progresivo | `PASS` automatizado; arte/hardware pendiente | Cuatro estaciones data-driven; Expedición/Álbum activas, Personalización/Parents futuras. Preview + compra atómica de rincón por 3 estrellas, save v10, Selva↔Camp/reload y ratios pasan. Todo visual sigue `PH_`. |
| Misiones | `PASS` automatizado; UX/hardware pendiente | 1 misión Approved, 3 objective strategies sin switch, pre-event/duplicate/prerequisite/cycle/multi-completion, auto-reward y save v10 (misiones se introdujeron en v8). Foto→misión→2 estrellas→reload pasa; sin expiry/daily/manual claim. |
| Learning engine | `PASS` automatizado; UX/hardware pendiente | Motor + actividad del tucán: single-choice por tags, fact/cues/reactions enlazados, foto→actividad, retry/pistas/replay/exit, reduce motion y reward/fact idempotentes. Fact Approved; representación Sourced/PH_ se bloquea en Release. |
| Contenido data-driven | Tucán Release `PASS`; proyecto Release `BLOCKED` | Catálogo O(1) contiene 1 discovery, 7 facts y 6 fuentes Approved; alias `discovery.jungle.placeholder → discovery.jungle.keel-billed-toucan`. Planta/objeto, mundo y audio final siguen bloqueando Release. |
| Expediente factual VS-D-A01 | Runtime `APPROVED` | H-007/H-008/H-009 cubren claims/copy, nombres, visual/rights/QA y revisión factual humana. Conservación excluida; no se atribuye credencial ornitológica externa. |
| Visual tucán VS-D-A01 | Runtime `APPROVED` | `visual.discovery.jungle.keel-billed-toucan`, prefab/materiales propios y ledger; discovery/interacción reales lo referencian sin `PH_`. |
| Mundos data-driven | Development `PASS`; Release `BLOCKED` | `world.jungle` compila desde manifest con escena/labels/spawn/checkpoint/catálogos/cues/version/tamaño. Fixture `world.test-ocean` prueba expansión sin switch; Release devuelve `WORLD018` por Draft/PH_. |
| Tests Unity | `PASS` | EditMode `147/147`; PlayMode `25/25`. Prompt 25 añade catálogo/ciclos, Release gate, atomicidad/retry/migración y preview→compra→Selva→Camp→reload/ratios. |
| Android | Development `PASS`; Release `BLOCKED` | APK ES final `106,873,974` bytes, SHA-256 `bc0117ceec68cc73821a8ae337286bc5625b69daf38d46ded76a5fac0fb8f925`, `15.980 s` incremental (`200.231 s` cold previo), API 26/36, IL2CPP/ARM64. El comando es reproducible; el APK Development no fue byte-reproducible entre runs por metadatos internos. Solo `INTERNET` heredado + receiver interno; sin CAMERA/micrófono/ubicación/contactos/AD_ID/BILLING. |
| iOS/CI remota | `NOT RUN` | Sin Xcode/módulo iOS local. `origin` existe, pero runner/licencia/checks remotos no se ejecutaron; no hubo push. |
| Paquetes | Verificado | AI Navigation `2.0.9`, Audio builtin `1.0.0`, Localization `1.5.12`, AndroidJNI `1.0.0`, Addressables `4.0.1`; exactos, sin preview/SDK comercial. |
| Fotografía memory/storage | Editor `PASS`; dispositivo `NOT RUN` | Peak estimado `582,182` bytes, delta global orientativo `289,481`, cero temporales; store 512 KiB/archivo, 64/32 MiB. Falta presión real de memoria/disco Android. |
| Gameplay/assets finales | Foto/álbum/economía/misión/learning/actividad/Camp funcionales con UI/cues `PH_` | No existen assets finales. Actividad VS-A01 requiere firma asset-specific; Camp/Jungle/UI/audio/personaje/fixtures restantes siguen placeholders. |
| Gate B journey | `FAIL` | Prompts 19–25 existen; faltan Prompts 26–29, journey integrado y FTUE. Cinco runs, primera/segunda sesión y UX no lectora: `NOT RUN`. |

## Lectura para reanudar

1. [`../AGENTS.md`](../AGENTS.md), este archivo, [`README.md`](README.md), [`ROADMAP.md`](ROADMAP.md), [`DECISIONS.md`](DECISIONS.md) y [`RISK_REGISTER.md`](RISK_REGISTER.md).
2. [`02_TECHNICAL_ARCHITECTURE.md`](02_TECHNICAL_ARCHITECTURE.md), [`ENGINEERING_STANDARDS.md`](ENGINEERING_STANDARDS.md), [`VALIDATION_PLAYBOOK.md`](VALIDATION_PLAYBOOK.md) y [`.agent/PLANS.md`](../.agent/PLANS.md).
3. [`CONTENT_MODEL.md`](CONTENT_MODEL.md), [`CONTENT_PIPELINE.md`](CONTENT_PIPELINE.md), [`RUNTIME_CONFIGURATION.md`](RUNTIME_CONFIGURATION.md), [`10_SAVE_SYSTEM.md`](10_SAVE_SYSTEM.md), [`17_LOCALIZATION.md`](17_LOCALIZATION.md), [`16_AUDIO.md`](16_AUDIO.md) y [`18_TESTING.md`](18_TESTING.md).
4. Prompt 26 del [catálogo](prompts/00_MASTER_CODEX_PROMPTS.md), [`CAMP_SYSTEM.md`](CAMP_SYSTEM.md), Economy, Save, accesibilidad y requisitos de arte.

## Bloqueos y decisiones humanas

- Los diez clips `PH_` son tonos técnicos internos, no voz/música/SFX aprobados. Actor/compositor, licencias, pronunciación, mezcla, inteligibilidad ES/EN y prueba en altavoz/audífonos bloquean audio Release.
- `adb devices` no listó hardware: touch/rotación/notch/Back/haptics, reproducción/focus, instalación, pause/force-stop, filesystem lleno y kill del OS permanecen `NOT RUN` para dispositivo/Gate C.
- Signing, bundle/company definitivos, AAB/store, licencia/titularidad del producto y aprobación factual/comercial continúan humanos; el build Release permanece fail-closed.
- Xcode/iOS y CI Unity remota no están disponibles/verificados. No publicar, hacer push ni aceptar términos.
- Validación ornitológica externa independiente, traducción final, protocolo de playtests con menores, territorios y modelo comercial siguen pendientes; la primera no bloquea el Vertical Slice bajo la aprobación humana H-009, pero sí es mitigación recomendada antes de Release.
- El tucán runtime ya no es placeholder. Manifest Selva, planta/objeto y audio final continúan Draft/`PH_`; no aprobarlos por arrastre.
- Save v10 persiste grants/transaction/fact keys, sesiones learning mínimas, unlocks Camp y referencias de thumbnail; medir crecimiento/reconciliación. Todo retiro futuro sigue exigiendo alias + migración.
- La actividad `activity.fixture.visual-matching` sigue Draft. `activity.jungle.keel-billed-toucan.choose-food` usa fact Approved pero su representación/opciones/reacción/cues son `Sourced`/`PH_`; [`VS-A01`](VS_A01_TOUCAN_FEEDING_ACTIVITY.md) conserva seis firmas humanas pendientes y Release fail-closed.
- Tap-to-move/cámara necesitan playtest comparativo P-006; reduce-motion existe runtime pero todavía no está conectado a preferencia adulta/Save.
- Planta/objeto y UI/cues de interacción/fotografía son `PH_` Draft; no son contenido final. Target, framing, touch, audio, pause y storage necesitan Android físico/playtest infantil antes de Gate C.
- La cámara es solo virtual y no solicita CAMERA; cualquier cámara física/galería/compartir/cloud requiere ADR y revisión de privacidad, no una extensión silenciosa.
- Prompt 19 usa el animal/discovery/interacción/facts Approved. Conservación, audio final y publicación no quedan autorizados por ello.
- Prompt 20 usa solo ese discovery Approved. El tamaño no se inventa, replay permanece deshabilitado y el chrome/fallback `PH_` bloquea presentación Release.

## Reanudación inmediata

Ejecutar Prompt 26 para personalización inclusiva por slots usando la estación futura ya definida. Reutilizar Economy/Save/Content, conservar preview antes de unlock/equip y no introducir género forzado, IAP, compra real, remote, signing, push o publicación.
