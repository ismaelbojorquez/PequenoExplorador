# ExecPlan — framework data-driven de misiones

- Fase/Gate: Prompt 22 / Gate B FAIL
- Estado: Complete
- Creado/actualizado: 2026-08-17 02:17 CST
- Owner: Quest/Mission Systems Engineer

## Propósito y alcance

Implementar definitions, facts semánticos, strategies extensibles, progreso persistente, prerrequisitos, auto-completion y auto-reward para una misión fixture de fotografía. Incluye tres strategies (`discover-count`, `photograph-specific`, `interact-tag`), UI mínima, validadores, save v8 y pruebas. Excluye catálogo final, daily quests, expiración, claim manual, learning y contenido masivo.

## Contexto y orientación

HEAD inicial `30687ebbd4be22aafee88acb19c84f76e3ee4aa9`, rama `main`, árbol limpio. Save v7 persiste `completedMissionIds` vacío, economy usa transaction keys durables y ledger 32, fotografía entrega discovery/reward y `IMessageBus` ya existe como fan-out acotado. Fuentes: `AGENTS.md`, `docs/STATUS.md`, `docs/09_MISSION_SYSTEM.md`, `docs/CONTENT_MODEL.md`, `docs/ECONOMY_REWARDS.md`, `docs/10_SAVE_SYSTEM.md` y `docs/18_TESTING.md`.

## Progreso

- [x] 2026-08-17 01:33 CST — preflight Git/documental e inventario inicial completados; baseline integral iniciada.
- [x] 2026-08-17 01:45 CST — contratos puros, catálogo runtime y tres strategies sin switch central.
- [x] 2026-08-17 01:47 CST — save v8, repositorio/coordinador y auto-reward idempotente.
- [x] 2026-08-17 01:50 CST — authoring fixture, UI localizada, Bootstrap y validadores.
- [x] 2026-08-17 02:17 CST — pipeline integral final repetido tras corregir evidencia del reward catalog; APK Android Development PASS.
- [x] 2026-08-17 02:08 CST — documentación y diff revisados; commit pendiente como último paso operativo.

## Hallazgos

- `PlayerProgress.CompletedMissionIds` existe desde v1, pero no representa estado activo, objetivos ni activation sequence; no debe reinterpretarse sin migración.
- El bus existente limpia suscripciones y no debe reemplazarse. La misión necesita facts tipados con sequence/scope y un sink explícito para que los casos de uso sigan testeables sin bus global.
- La reward de discovery ya usa la economía idempotente. La misión debe reutilizar `GrantRewardUseCase` con una transaction key derivada del mission ID.
- La segunda foto usa otro fact ID: tras completion su outcome de misión es `Ignored`, no `Duplicate`; la transacción ya procesada conserva estrellas sin duplicación. El caso `Duplicate` se prueba reutilizando exactamente el mismo fact ID.
- Un fact puede satisfacer varias misiones activas. El coordinador debe completar/grant cada definition, no solo la última; se añadió fixture de multi-completion.

## Decisiones

- 2026-08-17 — mantener un registry `type ID → strategy`; agregar un tipo cambia datos+registro de composición, no una evaluación central con `switch`.
- 2026-08-17 — los hechos anteriores a activación no cuentan: cada estado guarda `activationSequence` y solo evalúa facts con sequence posterior, salvo opt-in explícito de la definition.
- 2026-08-17 — completion y reward son automáticos; progreso de misión y transaction key de Economy permanecen autoridades separadas e idempotentes.

## Plan de implementación

1. Añadir IDs, estados/facts y reglas puras de misión en Domain/Application.
2. Crear authoring Content, compiler/catálogo y validator con detección de referencias, ciclos y prerrequisitos imposibles.
3. Migrar save v7→v8 con estados vacíos y conservar `completedMissionIds` históricos.
4. Componer repositorio/coordinador en Bootstrap y enlazar fotografía/interacción mediante un sink semántico explícito.
5. Añadir panel `PH_` localizado y fixture Approved de fotografía del tucán, más fixtures Draft de las otras strategies solo para validación/tests.
6. Cubrir estrategias, pre-events, duplicados, prerequisites, completion/reward, migración, removed content y PlayMode persistence.

## Comandos y validación

- `scripts/validate` — baseline integral PASS antes de editar; validación final PASS (`PE_FULL_VALIDATION_OK`).
- `scripts/validate-missions` — `PASS`.
- `scripts/test-editmode` — `PASS`, 129/129.
- `scripts/test-playmode` — `PASS`, 22/22.
- `scripts/build-android-development` — PASS; 106,573,659 bytes, SHA-256 `4549b2b3dd3e50399b5325f1686a688032fa9f62794f56110cc35f00b576017e`, 17.782 s incremental.

## Recovery y seguridad

Todos los cambios quedan aislados en archivos versionados y una migración nueva; no se modifica una migración histórica. No borrar `Library`, artefactos ni cambios ajenos. Si Unity genera deriva no relacionada, inventariarla y revertir solo bytes atribuibles al import mediante patch. Release seguirá fail-closed por placeholders existentes y signing.

## Resultados y retrospectiva

El framework usa tres strategies registradas, un catálogo Approved de una sola misión, facts explícitos de fotografía/interacción y save v8. Compile/validator, EditMode 129/129, PlayMode 22/22, Addressables local y APK IL2CPP/ARM64 pasaron. El primer PlayMode de desarrollo detectó que una segunda captura nueva es `Ignored` y no `Duplicate`; se corrigió la expectativa sin debilitar la prueba de mismo-ID. La revisión también detectó y cubrió multi-completion y reconciliación de reward/panel al arranque. Gate B permanece `FAIL`; siguiente fase Prompt 23.
