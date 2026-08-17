# Estado vivo del proyecto

Actualizado: 2026-08-17 (`America/Mexico_City`). Git, implementación y evidencia ejecutada prevalecen si contradicen este resumen.

## Fase y Gate

- **Fase implementada más reciente:** Prompt 27 — DesignSystem infantil, galería TMP y migración visual de ocho superficies críticas.
- **Preparación editorial H-007/H-008/H-009:** [`VS-D-A01`](VS_D_A01_TOUCAN_FACTUAL_DOSSIER.md) con Product/Localization, visual/rights/QA y revisión factual humana aprobados. La competencia factual declarada es investigación/búsqueda ampliada, no credencial ornitológica externa. Conservación y audio final permanecen fuera del contenido adoptado.
- **Auditoría actual:** Prompt 30 — [`Gate B`](audits/GATE_B_2026-08-16.md) ejecutado sobre HEAD real; `FAIL` por ausencia estructural de Fases 19–29 y del journey end-to-end.
- **Gate actual:** B — `FAIL`; no ejecutar Prompt 31 hasta completar y volver a auditar el loop mínimo.
- **Gate A:** [`audits/GATE_A_2026-08-15.md`](audits/GATE_A_2026-08-15.md) — `PASS`, sin Critical/Major abierto.
- **Fase en curso:** ninguna; Prompt 27 quedó validado y commiteado en esta fase.
- **Siguiente fase:** Prompt 28 — FTUE audiovisual.
- **ExecPlan cerrado:** [`.agent/execplans/p27-premium-child-ui-design-system.md`](../.agent/execplans/p27-premium-child-ui-design-system.md).

## Capacidades verificadas

| Capacidad | Estado | Evidencia/limitación |
|---|---|---|
| Git/producto/contrato | Verificado | Fases 00–06 auditadas; visión/GDD/MVP Selva y AGENTS canónicos. Cada preflight debe volver a contrastarlos. |
| Unity y assemblies | `PASS` | Unity `6000.3.22f1`, URP, diez asmdefs incluido DesignSystem, grafo acíclico; Domain/Application sin engine. |
| Pipeline local | `PASS` | `scripts/validate`: repository/shell, compile/validadores, Addressables, EditMode `158/158`, PlayMode `26/26` y APK Development. |
| Bootstrap/servicios | `PASS` | MessageBus→Input→SafeArea→Haptics→Save→Photos→Localization→Audio→Analytics→Ads→Purchases; shutdown inverso y perfiles fail-closed. |
| Scene flow / Addressables | `PASS` | `4.0.1`; Boot→Camp↔Selva por `WorldManifest`, tres ciclos, sesión/handles controlados, 47 locations/1,289,237 bytes; sin endpoint/catálogo remoto. |
| Save local | `PASS` automatizado; dispositivo `NOT RUN` | Schema v11, JSON builtin, SHA-256, atomicidad/backup, v0→…→v11 y future read-only; v10→v11 conserva progreso e inicia ownership/equipped vacíos. PNG/manifest viven separados; sin PII/género/red/pixels. |
| Config runtime | `PASS` | Development/Release tipados, cero flags Release, sin remote config ni secretos. |
| Localización | `PASS` local | Localization `1.5.12`; 213 entries ES/EN, pseudo Development; TMP Essential Resources oficiales/versionados y bridge temporal para vistas serializadas. Audio/fuente final siguen pendientes. |
| Audio | `PASS` estructural; final/hardware `NOT RUN` | Audio builtin `1.0.0`; 5 buses, 9 cues, 7 sources, 14 WAV `PH_` mono/48 kHz, incluidos instrucción/fact de actividad; queue/cooldown/ducking/replay/pause. `releaseFinal=0`. |
| Input/adaptación | `PASS` automatizado; hardware `NOT RUN` | Input System `1.20.0`; 5 mapas, safe area central, Back+checkpoint, no-op haptics, ratios 4:3/16:9/20:9/16:10 y supresión multitouch. Android físico requerido antes de Gate C. |
| Locomoción candidata | `PASS` automatizado; UX/hardware pendiente | AI Navigation `2.0.9`; tap válido/inválido, spam, UI/Photography, cancelación, reduce-motion, camera bounds, unload y Selva x3. Sin joystick/root motion; P-006 sigue abierto. |
| Interacción contextual | `PASS` automatizado; hardware pendiente | Foco único, auto-approach y prompt ES/EN; `interaction.jungle.keel-billed-toucan` es Approved y planta/objeto continúan `PH_`. Touch Android `NOT RUN`. |
| Discovery/fotografía | `PASS` automatizado; hardware pendiente | Interacción→Photography; evaluator `0.08/10/0.36/0.35` + LOS; inválido amable, válido first/repeat, best-photo, failure fallback y grants idempotentes. Thumbnail `384×216`; recursos temporales vuelven a 0. |
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
| DesignSystem UI | `PASS` automatizado/visual; hardware pendiente | 8 roots, galería TMP, tokens, 64/72, contraste AA, motion cancelable y 40 capturas after para 10 superficies × 4 ratios. Bridge legacy, escala 1.25 humana, lector de pantalla y Android físico pendientes. |
| Tests Unity | `PASS` | EditMode `158/158`; PlayMode `26/26`. Prompt 27 añade tokens/galería/estados, escala 1.25, reduce-motion, targets y validator integrado. |
| Android | Development `PASS`; Release `BLOCKED` | APK ES `67,424,654` bytes, SHA-256 `e771d32998e31630937c6df6a56e1487e480819f7a6b9d72de24b523b359a090`, `69.515 s`, API 26/36, IL2CPP/ARM64. Sin CAMERA/micrófono/ubicación/contactos/AD_ID/BILLING. Release sigue bloqueado por signing/contenido `PH_`. |
| iOS/CI remota | `NOT RUN` | Sin Xcode/módulo iOS local. `origin` existe, pero runner/licencia/checks remotos no se ejecutaron; no hubo push. |
| Paquetes | Verificado | AI Navigation `2.0.9`, Audio builtin `1.0.0`, Localization `1.5.12`, AndroidJNI `1.0.0`, Addressables `4.0.1`; exactos, sin preview/SDK comercial. |
| Fotografía memory/storage | Editor `PASS`; dispositivo `NOT RUN` | Peak estimado `582,182` bytes, delta global orientativo `289,481`, cero temporales; store 512 KiB/archivo, 64/32 MiB. Falta presión real de memoria/disco Android. |
| Gameplay/assets finales | Foto/álbum/economía/misión/learning/actividad/Camp/customization funcionales con UI/cues `PH_` | No existen assets finales. Actividad VS-A01 requiere firma asset-specific; Camp/Jungle/UI/audio/personaje/cosméticos/fixtures restantes siguen placeholders. |
| Gate B journey | `FAIL` | Prompts 19–27 existen; faltan Prompts 28–29, journey integrado y FTUE. Cinco runs, primera/segunda sesión y UX no lectora: `NOT RUN`. |

## Lectura para reanudar

1. [`../AGENTS.md`](../AGENTS.md), este archivo, [`README.md`](README.md), [`ROADMAP.md`](ROADMAP.md), [`DECISIONS.md`](DECISIONS.md) y [`RISK_REGISTER.md`](RISK_REGISTER.md).
2. [`02_TECHNICAL_ARCHITECTURE.md`](02_TECHNICAL_ARCHITECTURE.md), [`ENGINEERING_STANDARDS.md`](ENGINEERING_STANDARDS.md), [`VALIDATION_PLAYBOOK.md`](VALIDATION_PLAYBOOK.md) y [`.agent/PLANS.md`](../.agent/PLANS.md).
3. [`CONTENT_MODEL.md`](CONTENT_MODEL.md), [`CONTENT_PIPELINE.md`](CONTENT_PIPELINE.md), [`RUNTIME_CONFIGURATION.md`](RUNTIME_CONFIGURATION.md), [`10_SAVE_SYSTEM.md`](10_SAVE_SYSTEM.md), [`17_LOCALIZATION.md`](17_LOCALIZATION.md), [`16_AUDIO.md`](16_AUDIO.md) y [`18_TESTING.md`](18_TESTING.md).
4. Prompt 28 del [catálogo](prompts/00_MASTER_CODEX_PROMPTS.md), GDD/FTUE, gameplay loop, audio/localización, accesibilidad, Save y UI.

## Bloqueos y decisiones humanas

- Los diez clips `PH_` son tonos técnicos internos, no voz/música/SFX aprobados. Actor/compositor, licencias, pronunciación, mezcla, inteligibilidad ES/EN y prueba en altavoz/audífonos bloquean audio Release.
- `adb devices` no listó hardware: touch/rotación/notch/Back/haptics, reproducción/focus, instalación, pause/force-stop, filesystem lleno y kill del OS permanecen `NOT RUN` para dispositivo/Gate C.
- Signing, bundle/company definitivos, AAB/store, licencia/titularidad del producto y aprobación factual/comercial continúan humanos; el build Release permanece fail-closed.
- Xcode/iOS y CI Unity remota no están disponibles/verificados. No publicar, hacer push ni aceptar términos.
- Validación ornitológica externa independiente, traducción final, protocolo de playtests con menores, territorios y modelo comercial siguen pendientes; la primera no bloquea el Vertical Slice bajo la aprobación humana H-009, pero sí es mitigación recomendada antes de Release.
- El tucán runtime ya no es placeholder. Manifest Selva, planta/objeto y audio final continúan Draft/`PH_`; no aprobarlos por arrastre.
- Save v11 persiste grants/transaction/fact keys, sesiones learning mínimas, unlocks Camp/cosméticos, equipped y referencias de thumbnail; medir crecimiento/reconciliación. Todo retiro futuro sigue exigiendo alias + migración/fallback.
- La actividad `activity.fixture.visual-matching` sigue Draft. `activity.jungle.keel-billed-toucan.choose-food` usa fact Approved pero su representación/opciones/reacción/cues son `Sourced`/`PH_`; [`VS-A01`](VS_A01_TOUCAN_FEEDING_ACTIVITY.md) conserva seis firmas humanas pendientes y Release fail-closed.
- Tap-to-move/cámara necesitan playtest comparativo P-006; reduce-motion existe runtime pero todavía no está conectado a preferencia adulta/Save.
- Planta/objeto y UI/cues de interacción/fotografía son `PH_` Draft; no son contenido final. Target, framing, touch, audio, pause y storage necesitan Android físico/playtest infantil antes de Gate C.
- La cámara es solo virtual y no solicita CAMERA; cualquier cámara física/galería/compartir/cloud requiere ADR y revisión de privacidad, no una extensión silenciosa.
- Prompt 19 usa el animal/discovery/interacción/facts Approved. Conservación, audio final y publicación no quedan autorizados por ello.
- Prompt 20 usa solo ese discovery Approved. El tamaño no se inventa, replay permanece deshabilitado y el chrome/fallback `PH_` bloquea presentación Release.

## Reanudación inmediata

Ejecutar Prompt 28 para el FTUE audiovisual contextual. Conservar el DesignSystem, targets/safe area/reduce-motion, límites `PH_`, Gate B `FAIL` y la prohibición de IAP/remote/signing/push/publicación.
