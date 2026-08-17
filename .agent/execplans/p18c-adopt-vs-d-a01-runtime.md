# ExecPlan — adoptar VS-D-A01 como contenido runtime Approved

- Fase/Gate: incremento preparatorio posterior a Prompt 18; Gate B FAIL
- Estado: Complete
- Creado/actualizado: 2026-08-16 23:16 America/Mexico_City
- Owner: Content Data/Save Integration

## Propósito y alcance

Sustituir únicamente el discovery/interacción animal neutrales por `VS-D-A01` aprobado, con definitions no-`PH_`, localización ES/EN, alias desde `discovery.jungle.placeholder` y migración segura del progreso persistido. Conservar planta, objeto, mundo y audio final como placeholders explícitos. No implementar fotografía, álbum, economía ni contenido adicional. Aceptación: el runtime resuelve el tucán por IDs reales, el progreso antiguo converge sin doble grant, los validators específicos pasan y Prompt 19 queda habilitado documentalmente.

## Contexto y orientación

HEAD inicial `b468b1bb0d7adb4cf7dea4fa81c75c40902483c8`, rama `main`, árbol limpio. H-007/H-008/H-009 cubren Product/Localization, visual/rights/QA y revisión factual humana. El prefab `visual.discovery.jungle.keel-billed-toucan` es `Approved`; el catálogo aún contiene cinco definitions `PH_` y la escena usa `interaction.fixture.animal → discovery.jungle.placeholder`. Save schema v4 puede contener el ID placeholder y grant IDs que lo incluyen. Fuentes canónicas: `docs/CONTENT_MODEL.md`, `docs/CONTENT_SOURCES.md`, `docs/10_SAVE_SYSTEM.md`, `docs/07_DISCOVERY_SYSTEM.md`, `docs/INTERACTION_SYSTEM.md` y `docs/STATUS.md`.

## Progreso

- [x] 2026-08-16 22:56 — preflight Git/documental y aprobación humana contrastados; no hay cambios ajenos.
- [x] 2026-08-16 22:58 — serialización, catálogos, aliases, localización y save v4 inventariados; se eligió schema v5 explícito.
- [x] 2026-08-16 23:03 — definitions/interaction/localización `VS_` y tooling idempotente adoptados; solo PH animal/contenido retirado.
- [x] 2026-08-16 23:05 — migración v4→v5 implementada con merge, fecha temprana y normalización/deduplicación de grants.
- [x] 2026-08-16 23:16 — validators, 107 EditMode, 18 PlayMode, APK Development y gate Release ejecutados; docs/diff cerrados.

## Hallazgos

- El prefab ya es `Approved`, pero `DATA025` continúa para category/tag/source/fact/discovery neutrales y `WORLD018` para Selva; el mundo no forma parte de esta adopción.
- `discovery.jungle.placeholder` ya puede existir en saves v4, por lo que renombrar el asset sin alias/normalización perdería denominadores o permitiría grants duplicados.
- Un test heredado asumía `Sources.Single()`; la adopción de seis fuentes expuso la suposición. El test ahora escoge una fuente y controla el fallo de trazabilidad que realmente pretende verificar.

## Decisiones

- 2026-08-16 — mantener este incremento separado de Prompt 19 para que la migración de identidad sea revisable y reversible.
- 2026-08-16 — no reutilizar/renombrar destructivamente assets `PH_` de planta/objeto; crear assets reales del tucán y retirar solo el placeholder discovery de la autoridad runtime.
- 2026-08-16 — schema v5 es una copia cerrada del payload v4 con migración de identidad; el alias runtime no basta porque queries/procesamiento persistido usan IDs exactos.
- 2026-08-16 — conservación y audio final no se convierten en facts/cues aprobados; `audio.feedback.confirm` sigue siendo feedback genérico temporal.

## Plan de implementación

1. Determinar cómo el catálogo resuelve aliases y cómo Save/Discovery conservan IDs y grant keys.
2. Añadir assets aprobados mínimos para categoría animal, tag Selva, fuentes trazables, facts aprobados y discovery del tucán; añadir keys ES/EN y catálogo determinista.
3. Añadir interaction real del tucán, cambiar la escena/generator a su ID y conservar planta/objeto sin cambios.
4. Normalizar progreso/grants antiguos de placeholder hacia el ID real mediante migración versionada o un paso puro equivalente respaldado por tests, según el contrato observado.
5. Validar Development/Release específico, suites y Android; actualizar docs/plan/status y commit único.

## Comandos y validación

- `git status --short --branch`, `git branch --show-current`, `git log -1 --format=fuller` — preflight; limpio/main/HEAD esperado.
- `./scripts/validate-content` — baseline y final exit `0`.
- `./scripts/test-editmode` — primer run `FAIL` 106/107 por suposición de test; repetición `PASS` 107/107.
- `./scripts/test-playmode` — `PASS` 18/18.
- `./scripts/validate` — `PASS`, exit `0`, `4:31.06`; compile, Addressables, suites y APK.
- `./scripts/build-android-release` — exit `2` esperado: `INTERACTION005` planta/objeto, `WORLD018` mundo y signing; no `DATA025`/`TOUCAN`.

## Recovery y seguridad

Los assets PH retirados se eliminaron solo después de que catálogo/escena apuntaran a assets `VS_`; planta/objeto/mundo se preservaron. Tooling es idempotente y los GUIDs del prefab se conservan. `artifacts/`, Library y builds permanecen ignorados. No push, signing ni publicación.

## Resultados y retrospectiva

Runtime resuelve el tucán Approved por ID real y por alias retirado. Save v5 preserva/combina progreso y grants v4. El APK Development compila sin permisos sensibles; Release falla por blockers ajenos explícitos. Prompt 19 queda habilitado sin ampliar contenido ni aprobar placeholders por arrastre.
