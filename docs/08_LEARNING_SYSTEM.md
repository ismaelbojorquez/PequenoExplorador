# 08 — Sistema de aprendizaje y actividades

Estado: Prompt 23 implementado. Taxonomía/métricas: [`04_EDUCATIONAL_DESIGN.md`](04_EDUCATIONAL_DESIGN.md). Cantidades: [`MVP_SCOPE.md`](MVP_SCOPE.md). La fixture actual demuestra el motor; no sustituye la actividad integrada del tucán del Prompt 24.

## Contrato y máquina de estados

```text
Content LearningActivityDefinitionAsset + LearningConceptDefinitionAsset
  → validator/compiler → ILearningCatalog readonly O(1)
  → LearningCoordinator → ILearningActivityStrategy registry
       ├─ ILearningRepository → PlayerProgress v9
       ├─ GrantRewardUseCase → Economy
       └─ GameplayFact learning-completed → IMissionFactSink
  → LearningActivityView temporal localizado
```

```text
sin sesión ─Start→ Active
Active ─respuesta no resuelta→ TryAgain ─siguiente intento→ Hint(level 1…3) ─→ Active
Active ─pista solicitada→ Hint(level 1…3) ─→ Active
Active ─Exit→ Exited ─Start→ Active (Resume)
Active/Exited ─Restart→ Active(attempts=0, hint=0)
Active ─respuesta correcta→ Completed
Completed ─Start/retry→ AlreadyCompleted + reconciliación idempotente
```

No hay estado Failed, vidas, score negativo, timer, límite de intentos ni pérdida de recompensa. `attempts` cuenta respuestas no resueltas de la sesión para escalar pistas; no es una nota ni un raw event log.

## Contratos runtime

- `LearningActivityDefinition`: ID/type ID, keys de título/instrucción/éxito/retry, conceptos, opciones, solución, `HintPolicy`, resumibilidad, reward y metadata editorial.
- `LearningSession`: estado puro, intentos no resueltos y nivel de pista; no contiene UI/audio/GameObjects.
- `ActivityOutcome`: `Started`, `Resumed`, `TryAgain`, `Hint`, `Completed`, `Exited`, `Restarted`, fallbacks explícitos y estados idempotentes.
- `ILearningActivityStrategy`: evalúa una submission tipada. `SingleChoiceActivityStrategy` es la única strategy actual; el registry explícito evita reflection y switch central.
- `LearningConceptDailyProgress`: solo `{conceptId, yyyy-MM-dd, seenCount, completedCount}`. No persiste opciones elegidas, taps, tiempos, texto libre ni sesión analítica.

## Fixture Development

`activity.fixture.visual-matching` practica `concept.observation.visual-matching`: elegir círculo entre círculo/triángulo/cuadrado. Es deliberadamente abstracta, no añade un claim factual ni reutiliza sin permiso el expediente zoológico. Tiene tres pistas graduadas, replay mediante el servicio de audio actual, ES/EN y UI `PH_UI_LEARNING` dentro de safe area con targets ≥64.

La activity y el concepto son `Draft`, `placeholder=true`, owner `Learning Design` y watermark `BORRADOR · PH_`. Development puede ejecutarlos; Release los rechaza. La reward provisional `reward.activity.visual-matching.complete` concede una estrella una vez, sin reducirse por intentos/pistas. Prompt 24 deberá crear contenido animal data-driven y someter cualquier claim/representación a [`CONTENT_SOURCES.md`](CONTENT_SOURCES.md).

## Persistencia, idempotencia y privacidad

Save v9 añade `learningSessions[]` y `learningConcepts[]` mediante migración pura v8→v9 con arrays vacíos. Completion se confirma antes de emitir reward/fact. Economy usa `economy-tx.activity.<activity-id>` y Missions recibe `gameplay-fact.learning.<activity-id>`; un retry/reload reconcilia las mismas keys sin doble grant. Un save futuro permanece read-only.

No existe analytics remoto. Los agregados son locales, por concepto/día, y no perfilan edad/capacidad. La UI no lee DTO/JSON; Content no guarda sesión; Domain/Application no referencia UnityEngine.

## Validación

- `scripts/setup-learning`: authoring/localización/UI/wiring reproducible.
- `scripts/validate-learning`: IDs, catálogo/reward/keys, strategy, Draft Release gate, safe area/targets y ausencia de Unity/UI/analytics/reflection en Application Learning.
- EditMode: correct/incorrect, pistas/cap, retry, exit/resume/restart, missing/invalid, idempotencia reward/fact, agregado diario y migración v8→v9.
- PlayMode: fixture real ES→EN, retry amable, hint, exit/resume, replay, completion, flush/reload y `AlreadyProcessed`.

Android físico, comprensión infantil, audio final, pseudo visual humano y la actividad factual integrada siguen `NOT RUN`/pendientes; compile/tests/APK no sustituyen esas revisiones.
