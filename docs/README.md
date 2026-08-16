# Índice documental

Estado: foundation, scene flow local, persistencia schema v3, localización ES/EN y framework de audio placeholder disponibles; todavía no existe gameplay ni contenido final. Empezar siempre por [`../AGENTS.md`](../AGENTS.md) y [`STATUS.md`](STATUS.md).

## Fuentes de verdad

| Tema | Documento canónico | Regla |
|---|---|---|
| Visión y promesa | [`00_PRODUCT_VISION.md`](00_PRODUCT_VISION.md) | Define por qué existe el juego y qué significa divertido/educativo. |
| Experiencia | [`01_GDD.md`](01_GDD.md) | Define fantasía, flujo, tono, economía y contratos de producto. |
| Cantidades/prioridad | [`MVP_SCOPE.md`](MVP_SCOPE.md) | Única fuente para Vertical Slice, MVP, post-MVP y MoSCoW. |
| Secuencia de trabajo | [`ROADMAP.md`](ROADMAP.md) | Único orden de Gates A–F y Fases 00–57. |
| Hechos educativos | [`CONTENT_SOURCES.md`](CONTENT_SOURCES.md) | Ningún claim llega a Release sin trazabilidad y aprobación humana. |
| Contenido runtime | [`CONTENT_PIPELINE.md`](CONTENT_PIPELINE.md) | Define grupos/perfiles locales, ownership y prohibición de remoto. |
| Persistencia | [`10_SAVE_SYSTEM.md`](10_SAVE_SYSTEM.md) | Schema, archivos, migración, atomicidad, privacidad y recovery. |
| Configuración runtime | [`RUNTIME_CONFIGURATION.md`](RUNTIME_CONFIGURATION.md) | Perfiles/flags locales, autoridades, mapping y guardrails Release. |
| Localización | [`17_LOCALIZATION.md`](17_LOCALIZATION.md) | Locales ES/EN, pseudo Development, keys/tablas, fallback, CSV y validación. |
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
7. [`08_LEARNING_SYSTEM.md`](08_LEARNING_SYSTEM.md)
8. [`09_MISSION_SYSTEM.md`](09_MISSION_SYSTEM.md)
9. [`14_UI_UX.md`](14_UI_UX.md)
10. [`15_ART_DIRECTION.md`](15_ART_DIRECTION.md)
11. [`16_AUDIO.md`](16_AUDIO.md)
12. [`MVP_SCOPE.md`](MVP_SCOPE.md)
13. [`PLAYTEST_PLAN.md`](PLAYTEST_PLAN.md)
14. [`CONTENT_SOURCES.md`](CONTENT_SOURCES.md)

Los saltos numéricos reservan espacio para documentos técnicos futuros; no implican archivos ausentes requeridos en esta fase.

## Ingeniería, políticas y producción

- [`ENGINEERING_STANDARDS.md`](ENGINEERING_STANDARDS.md): capas, C#/Unity, eventos, save, plataforma, dependencias y placeholders.
- [`CODE_REVIEW_RULES.md`](CODE_REVIEW_RULES.md): bloqueantes y checklists técnicos/infantiles.
- [`VALIDATION_PLAYBOOK.md`](VALIDATION_PLAYBOOK.md): comandos conocidos, evidencia y prueba de reanudación.
- [`18_TESTING.md`](18_TESTING.md): wrappers, outputs JUnit, diagnóstico y CI.
- [`VERSION_MATRIX.md`](VERSION_MATRIX.md): Editor y toolchain verificados.
- [`02_TECHNICAL_ARCHITECTURE.md`](02_TECHNICAL_ARCHITECTURE.md): grafo real de nueve assemblies, límites y enforcement.
- [`TECHNICAL_ARCHITECTURE.md`](TECHNICAL_ARCHITECTURE.md): alias histórico hacia la arquitectura canónica.
- [`DEPENDENCY_REGISTER.md`](DEPENDENCY_REGISTER.md): intake y pins de paquetes directos/transitivos.
- [`CONTENT_PIPELINE.md`](CONTENT_PIPELINE.md): Addressables local, addresses/labels, ownership, build y evolución.
- [`10_SAVE_SYSTEM.md`](10_SAVE_SYSTEM.md): schema v3, DTOs, checksum, backup, migraciones, autosave y recuperación.
- [`17_LOCALIZATION.md`](17_LOCALIZATION.md): paquete exacto, ES/EN, pseudo, tablas/keys y pipeline CSV local.
- [`RUNTIME_CONFIGURATION.md`](RUNTIME_CONFIGURATION.md): AppConfig Development/Release, flags tipados y validación build.
- [`20_ANDROID_RELEASE.md`](20_ANDROID_RELEASE.md): build Android actual y bloqueo de Release.
- [`ANDROID_RELEASE.md`](ANDROID_RELEASE.md): evidencia histórica de smoke F03/F04.
- [`GITHUB_SETUP.md`](GITHUB_SETUP.md): remoto, protección, runner/licencia y secrets como pasos humanos.
- [`POLICY_SOURCE_REGISTER.md`](POLICY_SOURCE_REGISTER.md): políticas oficiales y revalidación.
- [`RISK_REGISTER.md`](RISK_REGISTER.md): riesgos, owners y mitigaciones.
- [`ART_ASSET_REQUIREMENTS.md`](ART_ASSET_REQUIREMENTS.md): contrato técnico de assets.
- [`16_AUDIO.md`](16_AUDIO.md) y [`AUDIO_REQUIREMENTS.md`](AUDIO_REQUIREMENTS.md): dirección, framework, buses, cues, ledger `PH_` y blockers humanos.
- [`CHANGELOG.md`](CHANGELOG.md): cambios por fase.
- [`audits/GATE_A_2026-08-15.md`](audits/GATE_A_2026-08-15.md): auditoría independiente de foundation, hallazgos y evidencia ejecutada.

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

## Lectura obligatoria para el siguiente incremento

Seguir el orden exacto de [`STATUS.md`](STATUS.md), abrir allí el prompt siguiente del [catálogo 00–57](prompts/00_MASTER_CODEX_PROMPTS.md) y reejecutar `scripts/validate`. El catálogo describe la cadena completa, pero no sustituye el estado vivo ni autoriza repetir fases, saltar Gates o asumir evidencia. Para cualquier cambio de progreso/save, leer `10_SAVE_SYSTEM` y añadir migración/test antes de cambiar schema. La persistencia no autoriza producir catálogo MVP ni ampliar más allá de Selva.

Las notas regulatorias son baseline de ingeniería, no asesoría legal definitiva. Las políticas deben revalidarse en la fase indicada y antes de cada envío.
