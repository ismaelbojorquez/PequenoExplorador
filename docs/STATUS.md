# Estado vivo del proyecto

Actualizado: 2026-08-17 (`America/Mexico_City`). Git, implementación y evidencia ejecutada prevalecen si contradicen este resumen.

## Fase y Gate

- **Fase implementada más reciente:** Prompt 29 — loop mínimo Vertical Slice integrado de extremo a extremo.
- **Preparación editorial H-007/H-008/H-009:** [`VS-D-A01`](VS_D_A01_TOUCAN_FACTUAL_DOSSIER.md) con Product/Localization, visual/rights/QA y revisión factual humana aprobados. La competencia factual declarada es investigación/búsqueda ampliada, no credencial ornitológica externa. Conservación y audio final permanecen fuera del contenido adoptado.
- **Auditoría actual:** [Gate B física/Child UX 2026-08-17](audits/GATE_B_2026-08-17_PHYSICAL_AND_CHILD_UX.md) — `FAIL`. Diez arranques físicos pasan y el crash `level0` sigue resuelto, pero UI/diagnósticos se superponen, Selva queda oculta, rotación en caliente produce pantalla negra y no existe playtest infantil/no lector. Las auditorías [condicional](audits/GATE_B_2026-08-17.md) y [reparación de arranque](audits/GATE_B_2026-08-17_ANDROID_BOOTSTRAP_FIX.md) quedan como evidencia histórica.
- **Gate actual:** B — `FAIL`. No escalar contenido ni ejecutar Prompt 31 hasta remediar composición/lifecycle UI Android y repetir hardware + playtest.
- **Gate A:** [`audits/GATE_A_2026-08-15.md`](audits/GATE_A_2026-08-15.md) — `PASS`, sin Critical/Major abierto.
- **Fase en curso:** remediación requerida posterior a la auditoría física de Gate B.
- **Siguiente acción:** crear ExecPlan para ownership de roots UI, exclusión visual/raycast, diagnostics opt-in y recuperación de surface al rotar; después repetir suite, APK exacto, matriz física y solo entonces playtest consentido/no lector. Prompt 31 permanece bloqueado.
- **ExecPlan activo:** ninguno. Último plan cerrado: [reparación de serialización Android](../.agent/execplans/gate-b-android-bootstrap-serialization-repair.md).

## Capacidades verificadas

| Capacidad | Estado | Evidencia/limitación |
|---|---|---|
| Git/producto/contrato | Verificado | Fases 00–06 auditadas; visión/GDD/MVP Selva y AGENTS canónicos. Cada preflight debe volver a contrastarlos. |
| Unity y assemblies | `PASS` | Unity `6000.3.22f1`, URP, diez asmdefs incluido DesignSystem, grafo acíclico; Domain/Application sin engine. |
| Pipeline local | `PASS` | Auditoría física: `scripts/validate` pasó repository/shell, compile/validadores, Addressables, EditMode `169/169`, PlayMode `29/29` y APK Development; el rebuild no sustituye el APK físico exacto auditado. |
| Bootstrap/servicios | `PASS` | MessageBus→Input→SafeArea→Haptics→Save→Photos→Localization→Audio→Analytics→Ads→Purchases; shutdown inverso y perfiles fail-closed. |
| Scene flow / Addressables | `PASS` | `4.0.1`; Boot→Camp↔Selva por `WorldManifest`, tres ciclos, sesión/handles controlados, 61 locations/1,920,120 bytes; sin endpoint/catálogo remoto. |
| Save local | `PASS` automatizado; dispositivo `PARTIAL/CONTAMINATED` | Schema v12 y migraciones pasan. En HONOR, cinco+cinco arranques cargan save; no hay discovery/foto completados. Hitboxes superpuestos añadieron grants `economy-tx.debug.8/.9` y reiniciaron tutorial durante auditoría; no restaurar/borrar sin autorización. |
| Config runtime | `PASS` | Development/Release tipados, cero flags Release, sin remote config ni secretos. |
| Localización | `PASS` automatizado; físico `FAIL/NOT RUN` | ES se renderiza, pero el selector solapado no permite cambio confiable: un tap destinado a locale activó “Repetir guía”. EN/pseudo físicos no demostrados. |
| Audio | `PASS` estructural; final/hardware `NOT RUN` | Audio builtin `1.0.0`; 5 buses, 16 cues, 7 sources, 28 WAV `PH_` mono/48 kHz, incluidos 14 tonos tutorial ES/EN; queue/cooldown/ducking/replay/pause. `releaseFinal=0`. |
| Input/adaptación | `PASS` automatizado; hardware `FAIL` | Back abre pausa y background/resume conserva proceso; pero hitboxes/Canvas superpuestos interceptan acciones y la rotación landscape opuesta deja framebuffer negro hasta reiniciar Activity. Safe area/touch no cumplen Gate B. |
| Locomoción candidata | `PASS` automatizado; hardware `NOT RUN/BLOCKED` | AI Navigation `2.0.9`; Selva queda oculta por roots persistentes tras transición física, por lo que tap-to-move no pudo evaluarse. P-006 sigue abierto. |
| Interacción contextual | `PASS` automatizado; hardware `NOT RUN/BLOCKED` | Core/fixture pasan suites, pero la UI física impide alcanzar tucán/approach. |
| Discovery/fotografía | `PASS` automatizado; hardware `NOT RUN/BLOCKED` | Core pasa; save físico conserva discoveries/photos vacíos porque el journey no es alcanzable con la composición actual. |
| Álbum | `PASS` automatizado; visual/hardware pendiente | Query Approved + discovery/photo, locked sin fuga, contadores/filtro, detalle/fallback, pool/caché 8, cancellation, ES/EN/pseudo y 4:3/16:9/20:9/16:10. Tamaño y audio usan fallback; UI `PH_`. |
| Economía | `PASS` automatizado; tuning/playtest pendiente | Una moneda `ExplorerStars`, 4 definitions (3 no-debug), grant/spend idempotente, transaction keys durables, ledger 32 y mejora Camp por 3 provisional. Debug solo Development; sin IAP/premium/azar. |
| Camp progresivo | `PASS` automatizado; arte/hardware pendiente | Cuatro estaciones data-driven; Expedición/Álbum/Personalización activas, Parents futura. Preview + compra atómica del rincón, Selva↔Camp/reload y ratios pasan. Todo visual Camp sigue `PH_`. |
| Personalización | `PASS` automatizado; arte/hardware pendiente | 8 slots/20 opciones `PH_`, defaults diversos/gratuitos, unlock atómico+equip separado, compatibilidad/fallback, save v11, Camp↔Selva↔reload, ratios y shared materials. Sin género/IAP; Release falla `CUSTOM005`. |
| Misiones | `PASS` automatizado; UX/hardware pendiente | 1 misión Approved, 3 objective strategies sin switch, pre-event/duplicate/prerequisite/cycle/multi-completion, auto-reward y save v10 (misiones se introdujeron en v8). Foto→misión→2 estrellas→reload pasa; sin expiry/daily/manual claim. |
| Learning engine | `PASS` automatizado; UX/hardware pendiente | Motor + actividad del tucán: single-choice por tags, fact/cues/reactions enlazados, foto→actividad, retry/pistas/replay/exit, reduce motion y reward/fact idempotentes. Fact Approved; representación Sourced/PH_ se bloquea en Release. |
| Contenido data-driven | Tucán Release `PASS`; proyecto Release `BLOCKED` | Catálogo O(1) contiene 1 discovery, 7 facts y 6 fuentes Approved; alias `discovery.jungle.placeholder → discovery.jungle.keel-billed-toucan`. Planta/objeto, mundo y audio final siguen bloqueando Release. |
| Expediente factual VS-D-A01 | Runtime `APPROVED` | H-007/H-008/H-009 cubren claims/copy, nombres, visual/rights/QA y revisión factual humana. Conservación excluida; no se atribuye credencial ornitológica externa. |
| Visual tucán VS-D-A01 | Runtime `APPROVED` | `visual.discovery.jungle.keel-billed-toucan`, prefab/materiales propios y ledger; discovery/interacción reales lo referencian sin `PH_`. |
| Mundos data-driven | Development `PASS`; Release `BLOCKED` | `world.jungle` compila desde manifest con escena/labels/spawn/checkpoint/catálogos/cues/version/tamaño. Fixture `world.test-ocean` prueba expansión sin switch; Release devuelve `WORLD018` por Draft/PH_. |
| DesignSystem UI | Automatizado `PASS`; hardware `FAIL` | Roots/paneles incompatibles quedan simultáneamente visibles/raycastables en Camp/Expedition; texto cortado, controles solapados y Selva oculta. Rotación en caliente queda negra. Requiere ownership/layer state estructural y regresión de framebuffer/hitboxes. |
| Tests Unity | `PASS`, insuficientes para Gate B | EditMode `169/169` (5.479 s), PlayMode `29/29` (28.055 s). No detectan composición simultánea de roots ni framebuffer negro físico. |
| Android | Startup/binary `PASS`; UX/journey `FAIL`; Release `BLOCKED` | APK evaluado exacto `5c382e…`, HONOR DNY-NX9 Android 16: diez arranques Ready→Camp sin fatal, offline boot y Back/background parciales. Camp/Expedition no son jugables por Canvas solapados; rotación deja negro. API 26/36, ARM64, 16 KB; sin permisos sensibles. PSS Camp 362,196 KiB/RSS 513,416 KiB, sin perfil journey. |
| iOS/CI remota | `NOT RUN` | Sin Xcode/módulo iOS local. `origin` existe, pero runner/licencia/checks remotos no se ejecutaron; no hubo push. |
| Paquetes | Verificado | AI Navigation `2.0.9`, Audio builtin `1.0.0`, Localization `1.5.12`, AndroidJNI `1.0.0`, Addressables `4.0.1`; exactos, sin preview/SDK comercial. |
| Fotografía memory/storage | Editor `PASS`; dispositivo `NOT RUN` | Peak estimado `582,182` bytes, delta global orientativo `313,787`, cero temporales; store 512 KiB/archivo, 64/32 MiB. Falta presión real de memoria/disco Android. |
| Gameplay/assets finales | Foto/álbum/economía/misión/learning/actividad/Camp/customization funcionales con UI/cues `PH_` | No existen assets finales. Actividad VS-A01 requiere firma asset-specific; Camp/Jungle/UI/audio/personaje/cosméticos/fixtures restantes siguen placeholders. |
| FTUE | `PASS` automatizado; voz/hardware/playtest pendientes | Dos modos sin edad, siete pasos semánticos, ayuda a 6/12 s, skip/replay/recovery, input gating sin atrapar Back/pause y save v12. Catorce voces ES/EN son tonos `PH_`; Android físico y comprensión infantil siguen `NOT RUN`. |
| Gate B journey | **`FAIL`** | El journey automatizado sigue verde, pero en Android físico Camp/Expedition quedan cubiertos por roots y diagnósticos simultáneos; move→actividad→foto y el resto del loop no son alcanzables. Rotación deja framebuffer negro y el playtest infantil/no lector es `NOT RUN`; ver auditoría física 2026-08-17. |

## Lectura para reanudar

1. [`../AGENTS.md`](../AGENTS.md), este archivo, [`README.md`](README.md), [`ROADMAP.md`](ROADMAP.md), [`DECISIONS.md`](DECISIONS.md) y [`RISK_REGISTER.md`](RISK_REGISTER.md).
2. [`02_TECHNICAL_ARCHITECTURE.md`](02_TECHNICAL_ARCHITECTURE.md), [`ENGINEERING_STANDARDS.md`](ENGINEERING_STANDARDS.md), [`VALIDATION_PLAYBOOK.md`](VALIDATION_PLAYBOOK.md) y [`.agent/PLANS.md`](../.agent/PLANS.md).
3. [`CONTENT_MODEL.md`](CONTENT_MODEL.md), [`CONTENT_PIPELINE.md`](CONTENT_PIPELINE.md), [`RUNTIME_CONFIGURATION.md`](RUNTIME_CONFIGURATION.md), [`10_SAVE_SYSTEM.md`](10_SAVE_SYSTEM.md), [`17_LOCALIZATION.md`](17_LOCALIZATION.md), [`16_AUDIO.md`](16_AUDIO.md) y [`18_TESTING.md`](18_TESTING.md).
4. Prompt 30 del [catálogo](prompts/00_MASTER_CODEX_PROMPTS.md), audit histórico Gate B, ExecPlan P29, Vertical Slice, gameplay loop, arquitectura, testing y riesgos.

## Bloqueos y decisiones humanas

- Los 28 clips `PH_` son tonos técnicos internos, no voz/música/SFX aprobados. Actor/compositor, licencias, pronunciación, mezcla, inteligibilidad ES/EN y prueba en altavoz/audífonos bloquean audio Release.
- HONOR DNY-NX9 sí fue auditado: arranque/instalación pasan, pero touch/safe area/rotación fallan por composición y surface lifecycle. Audio/focus, haptics, filesystem lleno, OS kill y journey de performance siguen `NOT RUN`.
- Signing, bundle/company definitivos, AAB/store, licencia/titularidad del producto y aprobación factual/comercial continúan humanos; el build Release permanece fail-closed.
- Xcode/iOS y CI Unity remota no están disponibles/verificados. No publicar, hacer push ni aceptar términos.
- Validación ornitológica externa independiente, traducción final, protocolo de playtests con menores, territorios y modelo comercial siguen pendientes; la primera no bloquea el Vertical Slice bajo la aprobación humana H-009, pero sí es mitigación recomendada antes de Release.
- El tucán runtime ya no es placeholder. Manifest Selva, planta/objeto y audio final continúan Draft/`PH_`; no aprobarlos por arrastre.
- Save v12 persiste grants/transaction/fact keys, sesiones learning mínimas, unlocks Camp/cosméticos, equipped, referencias de thumbnail y estado tutorial mínimo; medir crecimiento/reconciliación. Todo retiro futuro sigue exigiendo alias + migración/fallback.
- La actividad `activity.fixture.visual-matching` sigue Draft. `activity.jungle.keel-billed-toucan.choose-food` usa fact Approved pero su representación/opciones/reacción/cues son `Sourced`/`PH_`; [`VS-A01`](VS_A01_TOUCAN_FEEDING_ACTIVITY.md) conserva seis firmas humanas pendientes y Release fail-closed.
- Tap-to-move/cámara necesitan playtest comparativo P-006; reduce-motion existe runtime pero todavía no está conectado a preferencia adulta/Save.
- Planta/objeto y UI/cues de interacción/fotografía son `PH_` Draft; no son contenido final. Target, framing, touch, audio, pause y storage necesitan Android físico/playtest infantil antes de Gate C.
- La cámara es solo virtual y no solicita CAMERA; cualquier cámara física/galería/compartir/cloud requiere ADR y revisión de privacidad, no una extensión silenciosa.
- Prompt 19 usa el animal/discovery/interacción/facts Approved. Conservación, audio final y publicación no quedan autorizados por ello.
- Prompt 20 usa solo ese discovery Approved. El tamaño no se inventa, replay permanece deshabilitado y el chrome/fallback `PH_` bloquea presentación Release.

## Reanudación inmediata

Remediar primero UI/lifecycle físico con ExecPlan: un owner de roots/layers por AppState, diagnostics opt-in no raycastables por defecto, Selva visible y recuperación de surface entre LandscapeLeft/Right. Añadir regresiones de composición/hitboxes/framebuffer. Tras suite+APK exacto, repetir matriz desde save limpio autorizado y solo entonces playtest consentido 4–5/6–9/no lector según [`PLAYTEST_PLAN.md`](PLAYTEST_PLAN.md). Reemitir Gate B; no ejecutar Prompt 31 mientras siga `FAIL`.
