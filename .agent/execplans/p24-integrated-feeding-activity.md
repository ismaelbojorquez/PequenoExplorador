# ExecPlan — Actividad integrada de alimentación del tucán

- Fase/Gate: Prompt 24 / Gate B permanece FAIL
- Estado: Complete
- Creado/actualizado: 2026-08-17 03:31 CST (`America/Mexico_City`)
- Owner: Gameplay/Educational Activity Engineer y Content Fact Checker

## Propósito y alcance

Entregar una única actividad táctil, data-driven y no punitiva para el tucán pico canoa: elegir entre tres opciones visuales, recibir pistas graduales, observar una reacción placeholder, oír/reproducir una narración placeholder y completar aprendizaje→misión→recompensa una sola vez. Reutiliza el motor de Learning, interacción, localización, audio, economía y misiones existentes. No agrega animales, arte/audio final, analytics, adaptación algorítmica ni aprobación editorial automática.

La actividad satisface aceptación técnica si sus reglas puras, authoring, validadores, presentación, integración y persistencia pasan; como la representación pedagógica nueva no fue incluida en H-007/H-008/H-009, debe quedar `Sourced`/placeholder y fallar validación Release hasta firma humana específica.

## Contexto y orientación

HEAD inicial `259f580f8465a3bc535ce1b6eae0180d3f54ad91`, rama `main`, árbol limpio. Unity es `6000.3.22f1`. `scripts/validate` inicial pasó repository/shell, compile, Addressables, EditMode `139/139`, PlayMode `23/23` y APK Development.

Fuentes canónicas: `docs/04_EDUCATIONAL_DESIGN.md`, `docs/08_LEARNING_SYSTEM.md`, `docs/CONTENT_SOURCES.md`, `docs/VS_D_A01_TOUCAN_FACTUAL_DOSSIER.md`, `docs/09_MISSION_SYSTEM.md`, `docs/ART_ASSET_REQUIREMENTS.md`, `docs/AUDIO_REQUIREMENTS.md` y este plan. El claim Approved reutilizable es `fact.jungle.keel-billed-toucan.diet`: “Come sobre todo frutas.” No se introducirán claims de dieta exclusiva ni conservación.

Implementación relevante: `LearningActivityDefinition` y `SingleChoiceActivityStrategy` en Application; assets/catálogo en Content; `LearningActivityView` en Presentation; `InteractionSceneRoot` y `PhotographyInteractionAction` controlan la entrada actual al tucán; `DiagnosticBootstrap` compone el flujo.

## Progreso

- [x] 2026-08-17 02:55 CST — preflight, inventario, lectura canónica y baseline completa ejecutados.
- [x] 2026-08-17 03:03 CST — claim de dieta contrastado con expediente Approved y fuentes primarias/oficiales; alcance editorial nuevo identificado.
- [x] 2026-08-17 03:14 CST — authoring, reglas por tags, presenter, reacción, cues y entrada foto→actividad implementados.
- [x] 2026-08-17 03:18 CST — validadores y pruebas ES/EN, retry/replay/exit, reduce motion, safe area y regresión foto/álbum agregados.
- [x] 2026-08-17 03:31 CST — pipeline completo PASS y Android/device checks revisados; dispositivo físico no disponible.
- [x] 2026-08-17 03:31 CST — documentación canónica y plan cerrados; pendiente únicamente review Git/commit.

## Hallazgos

- La actividad abstracta actual está fijada a `activity.fixture.visual-matching`; el presenter debe dejar de depender de un único ID sin romper sus pruebas.
- El tucán tiene una sola acción explícita y actualmente abre Photography. La integración necesita una acción compuesta/selección explícita, no un segundo bus global ni un atajo UI→Save.
- El claim dietario está Approved, pero las tres opciones, pistas, reacciones y narración son una representación editorial nueva; heredar `Approved` sería una aprobación por arrastre prohibida.
- Remsen, Hyde y Chapman (1993) analizaron 326 individuos de 32 especies de tucanes y concluyeron que la fruta predomina; el expediente también registra Cornell Birds of the World. Ninguna fuente justifica “solo come fruta”.
- Unity normaliza `_Color` en dos materiales ajenos (`PH_FIXTURE_OBJECT.mat`, `PH_FIXTURE_PLANT.mat`) durante import; ese churn debe revertirse antes del commit.
- La primera corrida completa falló en `EconomyValidationService`: `SingleOrDefault` asumía una sola reward Activity. Se corrigió para validar cuatro rewards por ID; compile/pipeline posterior pasan.
- La prueba de cancelación por unload expuso que `?.` sobre un `UnityEngine.Object` destruido provoca `MissingReferenceException`; se reemplazó por el null check Unity `!= null`. PlayMode y pipeline posterior pasan.

## Decisiones

- 2026-08-17 — usar tap sobre tres tarjetas, no drag: es la interacción más simple, tolerante y compatible con targets grandes para 4–9.
- 2026-08-17 — reutilizar `SingleChoiceActivityStrategy` y enriquecer definition/options con tags y metadata semántica; no crear una estrategia específica para tucán.
- 2026-08-17 — reutilizar exactamente el claim Approved “come sobre todo frutas”; piedra y sombrero serán distractores obvios, no afirmaciones sobre alimentos animales complementarios.
- 2026-08-17 — mantener actividad/concepto/opciones/reacción/audio placeholder fuera de Release hasta revisión humana asset-specific; Development mostrará watermark.
- 2026-08-17 — preservar acceso a Photography mediante una continuación explícita tras cerrar/completar la actividad, sin cambiar el núcleo de discovery.

## Plan de implementación

1. Extender contratos de actividad/opción con tag correcto, fact/source link, cues y reacciones tipadas, conservando compatibilidad del fixture existente.
2. Crear datos mínimos del tucán, tres opciones, concepto, recompensa/copy ES-EN y cues internos placeholder; extender validación de referencias/editorial/source.
3. Generalizar `LearningActivityView`, añadir tarjetas visuales grandes, pistas, reacción cancelable/reduce-motion y flujo explícito desde interacción hacia actividad y luego fotografía.
4. Añadir pruebas puras y PlayMode del flujo correcto/incorrecto/retry/replay/exit, editorial gate, localización, safe area e idempotencia.
5. Actualizar fuentes, learning, misión, arte/audio, testing, decisiones/changelog/status; ejecutar validación completa y cerrar el plan.

## Comandos y validación

- `git status --short --branch && git branch --show-current && git log -1 --format=fuller` — preflight Git; PASS, limpio en `main`.
- `scripts/validate` — baseline previa; PASS: EditMode `139/139`, PlayMode `23/23`, APK Development generado.
- `scripts/validate` — corrida final PASS: repository/shell, compile, Addressables 45 locations/1,143,428 bytes, EditMode `141/141`, PlayMode `24/24` y APK Development.
- `adb devices -l` — ejecutado; lista vacía, prueba Android real `NOT RUN` por ausencia de hardware.
- `git diff --check && git diff --cached --check` — PASS tras staging y review final; no se detectaron errores de whitespace.

## Recovery y seguridad

Todo cambio se limita a Prompt 24 y se revisa por diff. No borrar assets válidos, no limpiar cambios ajenos, no modificar signing/publicación ni hacer push. Los generadores Editor son idempotentes y sus assets quedan versionados; si fallan, conservar logs en `artifacts/` y reanudar desde el último hito. Restaurar únicamente el churn conocido de materiales mediante patch exacto, nunca checkout/reset destructivo. Si falla estructura, compilación o el gate editorial, registrar `FAIL/BLOCKED` sin ampliar contenido.

## Resultados y retrospectiva

Actividad data-driven completa en Development: foto válida ofrece entrada explícita, tres opciones por tags, feedback amable, pistas, replay, reacción reduce-motion, fact canónico, reward/fact idempotentes y salida/unload seguro. Fact dietario sigue Approved; representación permanece `Sourced`/`PH_`/`ReleaseBlocked` con firmas vacías. APK final: 106,754,166 bytes, SHA-256 `538545dafbdaa85d494e45ae7426f166205c56907587d172c9be01b77ccfb1d5`, 64.983 s, API 26/36, IL2CPP/ARM64. Android físico y playtest: `NOT RUN`. Gate B permanece `FAIL`; siguiente fase Prompt 25.
