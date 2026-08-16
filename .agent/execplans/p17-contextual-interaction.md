# ExecPlan — interacción contextual accesible

- Fase/Gate: Prompt 17 / Gate B
- Estado: Complete
- Creado/actualizado: 2026-08-16 15:23 America/Mexico_City
- Owner: Gameplay Interaction Engineer / Child UX

## Propósito y alcance

Entregar un núcleo genérico que priorice un único target, acerque al explorador, muestre una acción localizada grande y ejecute una respuesta idempotente para tres fixtures `PH_` de Selva. Incluye contratos puros, authoring tipado, detector indexado, coordinator, prompt safe-area, audio existente, validadores, tests y documentación. Excluye discovery, fotografía, actividades, contenido factual/final y nuevos paquetes.

## Contexto y orientación

HEAD inicial `d17935f77d5d65a27750aa82d557642177344c2c`, `main`, limpio y `ahead 8`. Explorer ya posee `ExplorerLocomotionController`, `NavMeshAgent`, cámara y entrada semántica; hoy cada tap se interpreta como suelo. Bootstrap es el único composition root. Application/Domain no referencian Unity; Presentation no referencia Content/Infrastructure. La baseline completa pasa `94/94` EditMode, `14/14` PlayMode y APK.

## Progreso

- [x] 2026-08-16 14:56 — preflight, inventario, perfil del loop y baseline integral completados.
- [x] Implementar contratos/coordinator/selector puros y tests EditMode.
- [x] Implementar authoring/fixtures/detector/prompt/composición y validator.
- [x] Ejecutar pipeline/Android integral y revisar rendimiento/artefacto; hardware real queda `NOT RUN`.
- [x] Actualizar fuentes canónicas y preparar revisión/commit único.

## Hallazgos

- No existe interacción previa que preservar; el punto de integración mínimo es dar prioridad a un handler contextual antes del raycast de suelo.
- No hay dispositivo ADB conectado. El Editor puede probar la misma action `<Pointer>`, pero touch físico seguirá `NOT RUN`.
- El grafo documentado conserva referencias AI obsoletas que los asmdefs reales ya no tienen; corregirlo forma parte de mantener la fuente canónica veraz.
- El primer compile dirigido falló porque Presentation consume el `InteractionId` Domain expuesto por `InteractionDefinition` sin referencia asmdef directa. La dirección sigue hacia adentro; se añadió `Presentation → Domain` a asmdef/allowlist en vez de degradar IDs a strings.
- Iteraciones de compile encontraron un `using` Bootstrap y keys ES/EN todavía no materializadas; ambos wiring gaps se cerraron antes de aceptar compile.
- EditMode encontró que `Missing` actualizaba el snapshot pero devolvía `None`; outcomes de sistema ahora mantienen state/result coherentes.
- PlayMode encontró un `GameObject` UI ya destruido durante `OnDestroy` y una cadena YAML sin comillas que exponía `\\xED`; se añadieron guards Unity-null y quoting correcto.

## Decisiones

- 2026-08-16 — selección determinista por `priority desc → ray distance asc → InteractionId ordinal`; evita categoría hardcodeada y hace predecibles solapes.
- 2026-08-16 — definitions/IDs viven en Domain/Application y authoring en Content; Presentation indexa colliders una vez y Bootstrap compone. No se usa bus global.
- 2026-08-16 — los fixtures reutilizan audio confirm/retry/explore ya permitido; no se crea voz o SDK nuevo.

## Plan de implementación

1. Añadir `InteractionId`, definitions/context/result/snapshot, `IInteractable`, approach port, selector y coordinator puro.
2. Añadir assets Content de catálogo/definitions `PH_`, compilador Development/Release y tres IDs neutrales.
3. Añadir view/detector/scene root y prompt localizado; integrar un handler prioritario con Explorer y Bootstrap.
4. Crear setup Editor idempotente, colliders/puntos/indicators, UI safe-area y validador CLI/build.
5. Probar prioridad/rango/cooldown/cancel/idempotencia y PlayMode de acercamiento/spam/destrucción/unload/UI/tres fixtures.
6. Ejecutar pipeline completo, actualizar docs/plan/status, revisar staging y commit.

## Comandos y validación

- `scripts/validate` — baseline `PASS`, código `0`, `80.40 s`, EditMode `94/94`, PlayMode `14/14`, Addressables y APK.
- `adb devices` — ejecutado; cero dispositivos, touch Android físico `NOT RUN`.
- `scripts/compile` — `PASS` dirigido tras las correcciones y setup.
- `scripts/test-editmode` — primera ejecución `FAIL` 98/99; corrección de outcome y repetición `PASS` 99/99.
- `scripts/test-playmode` — primera ejecución `FAIL` 12/17; guards lifecycle + YAML corregidos y repetición `PASS` 17/17.
- `scripts/build-android-development` — `PASS` dentro del pipeline final.
- `scripts/validate` — `PASS` repetido sobre el ajuste final, código `0`, `2:08.77`; Addressables 41/896,715 bytes, EditMode 99/99, PlayMode 17/17 y APK.
- APK — 80,931,145 bytes, SHA-256 `752b0fd41eb0558d6fa162d8fa8137cde46c08350645842d56f3ff508fe8a4f4`, API 26/36, ARM64/IL2CPP, zipalign/7 ELF 16 KB.
- `scripts/build-android-release` — `FAIL` esperado, código `2`: `INTERACTION005` rechaza los tres `PH_` antes de signing; no se produjo Release.
- `git diff --check`, `git diff --cached --check`, `scripts/check-repository` — pendientes antes del commit.

## Recovery y seguridad

Todos los cambios serán locales bajo las capas existentes; no hay paquete, red, permiso, signing ni push. Los fixtures/artefactos se marcan `PH_` y Release permanece fail-closed. Si setup/import falla, conservar logs ignorados, corregir fuentes/setup y regenerar solo roots/assets conocidos; no editar `Library`, borrar escenas ajenas ni limpiar cambios no propios.

## Resultados y retrospectiva

Tres fixtures comparten núcleo y catálogo sin hardcode de categoría. Compile/validators, 99 EditMode, 17 PlayMode, Addressables y APK Development pasan. Release permanece fail-closed y no se añadió discovery. Hardware/playtest continúan `NOT RUN`; la revisión Git/commit se completa fuera del plan sin cambiar alcance.
