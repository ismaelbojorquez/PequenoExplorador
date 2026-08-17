# Índice documental

Estado: foundation, scene flow local, persistencia schema v12, localización/audio/input, `VS-D-A01` runtime Approved, locomoción, interacción, discovery, fotografía, álbum, economía, misión, learning, actividad integrada, Camp, personalización inclusiva y FTUE contextual disponibles; todavía no existe contenido masivo ni arte final. Empezar siempre por [`../AGENTS.md`](../AGENTS.md) y [`STATUS.md`](STATUS.md).

## Fuentes de verdad

| Tema | Documento canónico | Regla |
|---|---|---|
| Visión y promesa | [`00_PRODUCT_VISION.md`](00_PRODUCT_VISION.md) | Define por qué existe el juego y qué significa divertido/educativo. |
| Experiencia | [`01_GDD.md`](01_GDD.md) | Define fantasía, flujo, tono, economía y contratos de producto. |
| Cantidades/prioridad | [`MVP_SCOPE.md`](MVP_SCOPE.md) | Única fuente para Vertical Slice, MVP, post-MVP y MoSCoW. |
| Secuencia de trabajo | [`ROADMAP.md`](ROADMAP.md) | Único orden de Gates A–F y Fases 00–57. |
| Hechos educativos | [`CONTENT_SOURCES.md`](CONTENT_SOURCES.md) | Ningún claim llega a Release sin trazabilidad y aprobación humana. |
| Expediente factual del slice | [`VS_D_A01_TOUCAN_FACTUAL_DOSSIER.md`](VS_D_A01_TOUCAN_FACTUAL_DOSSIER.md) | Claims/firmas del único animal runtime Approved; conservación/audio final/publicación siguen excluidos. |
| Expediente de actividad del slice | [`VS_A01_TOUCAN_FEEDING_ACTIVITY.md`](VS_A01_TOUCAN_FEEDING_ACTIVITY.md) | Mapea fact Approved a opciones/pistas/reacción/cues; representación Sourced/PH_ pendiente de firma humana. |
| Contenido runtime | [`CONTENT_PIPELINE.md`](CONTENT_PIPELINE.md) | Define grupos/perfiles locales, ownership y prohibición de remoto. |
| Modelo de datos | [`CONTENT_MODEL.md`](CONTENT_MODEL.md) | IDs tipados, definitions, authoring, catálogo, aliases y gate editorial. |
| Persistencia | [`10_SAVE_SYSTEM.md`](10_SAVE_SYSTEM.md) | Schema, archivos, migración, atomicidad, privacidad y recovery. |
| Tutorial/FTUE | [`TUTORIAL_SYSTEM.md`](TUTORIAL_SYSTEM.md) | Siete pasos semánticos, guía, gating, persistencia, recuperación y límites. |
| Fotografía ficticia | [`PHOTOGRAPHY_SYSTEM.md`](PHOTOGRAPHY_SYSTEM.md) | Evaluación asistida, captura virtual, thumbnail/store, fallos, privacidad y budgets. |
| Economía | [`ECONOMY_REWARDS.md`](ECONOMY_REWARDS.md) | Única moneda, rewards/spend idempotentes y prohibiciones no manipulativas. |
| Camp | [`CAMP_SYSTEM.md`](CAMP_SYSTEM.md) | Estaciones, primera mejora, transacción atómica, persistencia y límites infantiles. |
| Personalización | [`CUSTOMIZATION_SYSTEM.md`](CUSTOMIZATION_SYSTEM.md) | Slots inclusivos, catálogo, unlock/equip, compatibilidad, save v11 y placeholders. |
| Misiones | [`09_MISSION_SYSTEM.md`](09_MISSION_SYSTEM.md) | Facts, strategies, progreso/prerrequisitos, auto-completion y reward. |
| Configuración runtime | [`RUNTIME_CONFIGURATION.md`](RUNTIME_CONFIGURATION.md) | Perfiles/flags locales, autoridades, mapping y guardrails Release. |
| Localización | [`17_LOCALIZATION.md`](17_LOCALIZATION.md) | Locales ES/EN, pseudo Development, keys/tablas, fallback, CSV y validación. |
| Input y locomoción | [`INPUT_ACCESSIBILITY.md`](INPUT_ACCESSIBILITY.md) | Mapas semánticos, gestos, tap-to-move candidato, cámara asistida, safe area, Back, haptics y límites de hardware. |
| Cadena de prompts | [`prompts/00_MASTER_CODEX_PROMPTS.md`](prompts/00_MASTER_CODEX_PROMPTS.md) | Catálogo ejecutable 00–57; `STATUS.md` decide el siguiente prompt y los Gates impiden saltos. |
| Decisiones | [`DECISIONS.md`](DECISIONS.md) | Separa decisiones técnicas/producto de pendientes humanos. |
| Estado operativo | [`STATUS.md`](STATUS.md) | Fase/Gate, capacidades, plan activo, bloqueos y siguiente acción. |
| Ejecución | [`../.agent/PLANS.md`](../.agent/PLANS.md) | Decide cuándo y cómo mantener un ExecPlan vivo. |

## Especificación de producto

1. [`00_PRODUCT_VISION.md`](00_PRODUCT_VISION.md)
2. [`01_GDD.md`](01_GDD.md)
3. [`03_GAMEPLAY_LOOP.md`](03_GAMEPLAY_LOOP.md)
4. [`04_EDUCATIONAL_DESIGN.md`](04_EDUCATIONAL_DESIGN.md)
5. [`06_WORLD_DESIGN.md`](06_WORLD_DESIGN.md)
6. [`07_DISCOVERY_SYSTEM.md`](07_DISCOVERY_SYSTEM.md)
7. [`ALBUM_SYSTEM.md`](ALBUM_SYSTEM.md)
8. [`ECONOMY_REWARDS.md`](ECONOMY_REWARDS.md)
9. [`08_LEARNING_SYSTEM.md`](08_LEARNING_SYSTEM.md)
10. [`09_MISSION_SYSTEM.md`](09_MISSION_SYSTEM.md)
11. [`14_UI_UX.md`](14_UI_UX.md)
12. [`15_ART_DIRECTION.md`](15_ART_DIRECTION.md)
13. [`16_AUDIO.md`](16_AUDIO.md)
14. [`MVP_SCOPE.md`](MVP_SCOPE.md)
15. [`PLAYTEST_PLAN.md`](PLAYTEST_PLAN.md)
16. [`CONTENT_SOURCES.md`](CONTENT_SOURCES.md)
17. [`VS_D_A01_TOUCAN_FACTUAL_DOSSIER.md`](VS_D_A01_TOUCAN_FACTUAL_DOSSIER.md)

Los saltos numéricos reservan espacio para documentos técnicos futuros; no implican archivos ausentes requeridos en esta fase.

## Ingeniería, políticas y producción

- [`ENGINEERING_STANDARDS.md`](ENGINEERING_STANDARDS.md): capas, C#/Unity, eventos, save, plataforma, dependencias y placeholders.
- [`CODE_REVIEW_RULES.md`](CODE_REVIEW_RULES.md): bloqueantes y checklists técnicos/infantiles.
- [`VALIDATION_PLAYBOOK.md`](VALIDATION_PLAYBOOK.md): comandos conocidos, evidencia y prueba de reanudación.
- [`18_TESTING.md`](18_TESTING.md): wrappers, outputs JUnit, diagnóstico y CI.
- [`VERSION_MATRIX.md`](VERSION_MATRIX.md): Editor y toolchain verificados.
- [`02_TECHNICAL_ARCHITECTURE.md`](02_TECHNICAL_ARCHITECTURE.md): grafo real de diez assemblies, límites y enforcement.
- [`TECHNICAL_ARCHITECTURE.md`](TECHNICAL_ARCHITECTURE.md): alias histórico hacia la arquitectura canónica.
- [`DEPENDENCY_REGISTER.md`](DEPENDENCY_REGISTER.md): intake y pins de paquetes directos/transitivos.
- [`CONTENT_PIPELINE.md`](CONTENT_PIPELINE.md): Addressables local, addresses/labels, ownership, build y evolución.
- [`CONTENT_MODEL.md`](CONTENT_MODEL.md): catálogo O(1), ScriptableObjects, IDs, reports y bloqueo Release de Draft.
- [`10_SAVE_SYSTEM.md`](10_SAVE_SYSTEM.md): schema v12, DTOs, checksum, backup, migraciones, autosave y recuperación.
- [`CUSTOMIZATION_SYSTEM.md`](CUSTOMIZATION_SYSTEM.md): ocho slots, veinte opciones `PH_`, transacciones, rig y validación.
- [`ECONOMY_REWARDS.md`](ECONOMY_REWARDS.md): Estrellas de Explorador, fuentes/usos, idempotencia, ledger y límites infantiles.
- [`PHOTOGRAPHY_SYSTEM.md`](PHOTOGRAPHY_SYSTEM.md): cámara del explorador sin cámara física, evaluator, thumbnail acotada y store local.
- [`PRIVACY_ENGINEERING.md`](PRIVACY_ENGINEERING.md): inventario técnico de datos/permisos y controles offline; no es asesoría legal.
- [`17_LOCALIZATION.md`](17_LOCALIZATION.md): paquete exacto, ES/EN, pseudo, tablas/keys y pipeline CSV local.
- [`RUNTIME_CONFIGURATION.md`](RUNTIME_CONFIGURATION.md): AppConfig Development/Release, flags tipados y validación build.
- [`INPUT_ACCESSIBILITY.md`](INPUT_ACCESSIBILITY.md): Input System `1.20.0`, action maps, gestures, safe area, haptics y device harness.
- [`INTERACTION_SYSTEM.md`](INTERACTION_SYSTEM.md): foco, auto-acercamiento, selección determinista, prompt, fixtures y límites antes de discovery.
- [`20_ANDROID_RELEASE.md`](20_ANDROID_RELEASE.md): build Android actual y bloqueo de Release.
- [`ANDROID_RELEASE.md`](ANDROID_RELEASE.md): evidencia histórica de smoke F03/F04.
- [`GITHUB_SETUP.md`](GITHUB_SETUP.md): remoto, protección, runner/licencia y secrets como pasos humanos.
- [`POLICY_SOURCE_REGISTER.md`](POLICY_SOURCE_REGISTER.md): políticas oficiales y revalidación.
- [`RISK_REGISTER.md`](RISK_REGISTER.md): riesgos, owners y mitigaciones.
- [`ART_ASSET_REQUIREMENTS.md`](ART_ASSET_REQUIREMENTS.md): contrato técnico de assets.
- [`16_AUDIO.md`](16_AUDIO.md) y [`AUDIO_REQUIREMENTS.md`](AUDIO_REQUIREMENTS.md): dirección, framework, buses, cues, ledger `PH_` y blockers humanos.
- [`CHANGELOG.md`](CHANGELOG.md): cambios por fase.
- [`audits/GATE_A_2026-08-15.md`](audits/GATE_A_2026-08-15.md): auditoría independiente de foundation, hallazgos y evidencia ejecutada.
- [`audits/GATE_B_2026-08-17.md`](audits/GATE_B_2026-08-17.md): auditoría condicional histórica previa a la matriz física; integración técnica automatizada 5/5, Android físico y playtest infantil/no lector entonces `NOT RUN`.
- [`audits/GATE_B_2026-08-17_ANDROID_BOOTSTRAP_FIX.md`](audits/GATE_B_2026-08-17_ANDROID_BOOTSTRAP_FIX.md): adenda histórica del crash tras splash; causa `MonoScript` incrustado reparada, validator fail-closed y arranque físico hasta Camp verificado. Gate B permanecía `CONDITIONAL` en ese punto.
- [`audits/GATE_B_2026-08-17_PHYSICAL_AND_CHILD_UX.md`](audits/GATE_B_2026-08-17_PHYSICAL_AND_CHILD_UX.md): auditoría física vigente; `FAIL` por composición/hitboxes UI, Selva oculta, rotación negra y playtest no ejecutado. Sustituye el veredicto condicional para decidir la siguiente acción.
- [`audits/GATE_B_2026-08-16.md`](audits/GATE_B_2026-08-16.md): auditoría histórica sobre Prompt 18; su `FAIL` documentó correctamente que el journey todavía no existía.

## Evidencia de preflight

- [`PHASE_00_PREFLIGHT.md`](PHASE_00_PREFLIGHT.md)
- [`PHASE_01_PREFLIGHT.md`](PHASE_01_PREFLIGHT.md)
- [`PHASE_02_PREFLIGHT.md`](PHASE_02_PREFLIGHT.md)
- [`PHASE_03_PREFLIGHT.md`](PHASE_03_PREFLIGHT.md)
- [`PHASE_04_PREFLIGHT.md`](PHASE_04_PREFLIGHT.md)
- [`PHASE_05_PREFLIGHT.md`](PHASE_05_PREFLIGHT.md)
- [`PHASE_06_PREFLIGHT.md`](PHASE_06_PREFLIGHT.md)
- [`PHASE_07_PREFLIGHT.md`](PHASE_07_PREFLIGHT.md)
- [`PHASE_09_PREFLIGHT.md`](PHASE_09_PREFLIGHT.md)
- [`PHASE_10_PREFLIGHT.md`](PHASE_10_PREFLIGHT.md)
- [`PHASE_11_PREFLIGHT.md`](PHASE_11_PREFLIGHT.md)
- [`PHASE_13_PREFLIGHT.md`](PHASE_13_PREFLIGHT.md)
- [`PHASE_14_PREFLIGHT.md`](PHASE_14_PREFLIGHT.md)
- [`PHASE_15_PREFLIGHT.md`](PHASE_15_PREFLIGHT.md)
- [`PHASE_16_PREFLIGHT.md`](PHASE_16_PREFLIGHT.md)
- [`PHASE_17_PREFLIGHT.md`](PHASE_17_PREFLIGHT.md)
- [`PHASE_18_PREFLIGHT.md`](PHASE_18_PREFLIGHT.md)

## Lectura obligatoria para el siguiente incremento

Seguir el orden exacto de [`STATUS.md`](STATUS.md), abrir allí el prompt siguiente del [catálogo 00–57](prompts/00_MASTER_CODEX_PROMPTS.md) y reejecutar `scripts/validate`. El catálogo describe la cadena completa, pero no sustituye el estado vivo ni autoriza repetir fases, saltar Gates o asumir evidencia. Para cualquier cambio de progreso/save, leer `10_SAVE_SYSTEM` y añadir migración/test antes de cambiar schema. La persistencia no autoriza producir catálogo MVP ni ampliar más allá de Selva.

Las notas regulatorias son baseline de ingeniería, no asesoría legal definitiva. Las políticas deben revalidarse en la fase indicada y antes de cada envío.
