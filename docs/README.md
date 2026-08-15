# Índice documental

Estado: Fases 00–02 documentadas el 2026-08-14. Todavía no existe proyecto Unity, gameplay, assets ni build. Empezar siempre por [`../AGENTS.md`](../AGENTS.md) y [`STATUS.md`](STATUS.md).

## Fuentes de verdad

| Tema | Documento canónico | Regla |
|---|---|---|
| Visión y promesa | [`00_PRODUCT_VISION.md`](00_PRODUCT_VISION.md) | Define por qué existe el juego y qué significa divertido/educativo. |
| Experiencia | [`01_GDD.md`](01_GDD.md) | Define fantasía, flujo, tono, economía y contratos de producto. |
| Cantidades/prioridad | [`MVP_SCOPE.md`](MVP_SCOPE.md) | Única fuente para Vertical Slice, MVP, post-MVP y MoSCoW. |
| Secuencia de trabajo | [`ROADMAP.md`](ROADMAP.md) | Único orden de Gates A–F y Fases 00–57. |
| Hechos educativos | [`CONTENT_SOURCES.md`](CONTENT_SOURCES.md) | Ningún claim llega a Release sin trazabilidad y aprobación humana. |
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
- [`VERSION_MATRIX.md`](VERSION_MATRIX.md): Editor y toolchain verificados.
- [`POLICY_SOURCE_REGISTER.md`](POLICY_SOURCE_REGISTER.md): políticas oficiales y revalidación.
- [`RISK_REGISTER.md`](RISK_REGISTER.md): riesgos, owners y mitigaciones.
- [`ART_ASSET_REQUIREMENTS.md`](ART_ASSET_REQUIREMENTS.md): contrato técnico de assets.
- [`AUDIO_REQUIREMENTS.md`](AUDIO_REQUIREMENTS.md): contrato técnico de audio.
- [`CHANGELOG.md`](CHANGELOG.md): cambios por fase.

## Evidencia de preflight

- [`PHASE_00_PREFLIGHT.md`](PHASE_00_PREFLIGHT.md)
- [`PHASE_01_PREFLIGHT.md`](PHASE_01_PREFLIGHT.md)
- [`PHASE_02_PREFLIGHT.md`](PHASE_02_PREFLIGHT.md)

## Lectura obligatoria para Fase 03

Seguir el orden exacto de [`STATUS.md`](STATUS.md). Revalidar ADR-0001 y R-001/R-002/R-003/R-006/R-009 antes de crear el proyecto; evaluar un ExecPlan real. La foundation no autoriza producir el catálogo MVP: después se implementa solo el Vertical Slice.

Las notas regulatorias son baseline de ingeniería, no asesoría legal definitiva. Las políticas deben revalidarse en la fase indicada y antes de cada envío.
