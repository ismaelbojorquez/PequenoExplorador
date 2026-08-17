# ExecPlan — Prompt 23: motor de aprendizaje no punitivo

## Propósito

Implementar un motor puro y data-driven de actividades educativas que modele inicio, intentos, pistas, éxito, salida, reanudación y reinicio; persista solo estado mínimo y estadísticas agregadas; produzca facts semánticos para Missions y reward intents para Economy; y demuestre el contrato con una fixture abstracta de selección única, sin introducir claims zoológicos nuevos ni analytics.

## Progreso

- [x] Preflight Git/documental y auditoría de contratos existentes.
- [x] Baseline completa sobre HEAD de Prompt 22: compile/Addressables, EditMode 129/129, PlayMode 22/22 y APK Development PASS.
- [x] Domain/Application: IDs, sesión, outcomes, hint policy, strategy registry, coordinator y repositorio.
- [x] Content/Presentation/Bootstrap: catálogo fixture, presenter temporal, localización/audio y composición explícita.
- [x] Save schema v9 y migración pura v8→v9.
- [x] Validadores/wrappers y pruebas EditMode/PlayMode.
- [x] Documentación canónica, revisión completa y validación integral antes del commit.

## Hallazgos

- Solo existen `ActivityId` e `IActivityDefinition`; no hay motor learning ni mezcla previa que deba refactorizarse.
- Save real es v8. Privacy todavía describe v6 y debe corregirse a la realidad al adoptar v9.
- Economy ya reserva `RewardSourceKind.Activity`; Missions usa `GameplayFact`/`IMissionFactSink`, por lo que no se creará otro bus.
- `VS-D-A01` tiene claims aprobados, pero la actividad de alimentación/hábitat pertenece al prompt siguiente. La fixture actual será observación visual abstracta, sin claim factual nuevo.
- Unity sincroniza `_Color` con `_BaseColor` en dos materiales `PH_` ajenos al alcance al importar; el churn observado se revirtió tras las ejecuciones y no se incorporó al commit.

## Decisiones

- Usar IDs textuales tipados y un registry explícito `type ID → strategy`; sin reflection ni switch central.
- Persistir una sesión por Activity ID y agregados diarios por Concept ID; no guardar cada respuesta ni taps.
- La fixture será Draft/placeholder solo Development y Release la rechazará. El motor y su schema sí serán Release-safe.
- Completion precede al grant/fact y ambos usan IDs deterministas para retry idempotente.
- Mantener nueve assemblies; no crear asmdef nuevo.
- Usar una única recompensa Draft de 1 estrella en Development para probar el contrato completo; no habilitarla en Release ni tratarla como tuning aprobado.

## Resultados esperados

- Respuesta incorrecta produce `TryAgain` o `Hint`, nunca castigo ni límite de intentos.
- Exit conserva o reinicia según definition; resume/restart son explícitos.
- Completion registra concepto agregado por día, reward una vez y un fact tipado para Missions.
- Development ejecuta una fixture táctil/localizada con replay; Release bloquea Draft/placeholder.
- Resultado implementado: 1 concepto, 1 actividad single-choice, 3 opciones, 3 niveles de pista, 15 claves nuevas ES/EN y schema v9.

## Comandos

- Baseline: `scripts/validate` — PASS, 2026-08-17.
- Acotados durante implementación: `scripts/setup-learning`, `scripts/validate-learning`, `scripts/compile`, `scripts/test-editmode`, `scripts/test-playmode`.
- Final: `scripts/validate`, `git diff --check`, `git diff --cached --check`.

Los comandos acotados pasaron: compile, validator learning, EditMode y PlayMode. `scripts/validate` final pasó con EditMode `139/139`, PlayMode `23/23`, Addressables local y APK Development; evidencia exacta en `docs/STATUS.md` y `docs/CHANGELOG.md`.

## Recovery

Si import/setup falla, conservar logs en `artifacts/`, corregir primero el comando acotado y no editar Library/artefactos. Si la migración falla, preservar v8 y no avanzar schema hasta que round-trip, v8→v9 y future schema pasen. Si el árbol deja de estar aislado, detenerse antes de stage/commit.
