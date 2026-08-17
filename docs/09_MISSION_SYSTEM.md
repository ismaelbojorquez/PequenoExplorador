# 09 — Sistema de misiones

Estado: Prompt 22 implementado. Las misiones dan intención sin convertir el juego en lista de tareas, examen o mecanismo de retorno. Cantidades finales: [`MVP_SCOPE.md`](MVP_SCOPE.md).

## Contrato runtime

```text
Content MissionDefinitionAsset
  → MissionCatalogAsset/validator
  → MissionDefinition readonly + IMissionCatalog O(1)
  → MissionCoordinator
       ├─ IMissionObjectiveStrategy registry
       ├─ IMissionRepository → PlayerProgress v8
       └─ GrantRewardUseCase → Economy transaction idempotente
```

Cada definition declara ID, keys localizadas de título/resumen/cierre, uno a cuatro objetivos, prerrequisitos, reward opcional y metadata editorial. Siempre usa auto-completion/auto-claim, nunca expira y no contiene timers. `MissionProgress` conserva estado, `activationSequence` y contadores por `MissionObjectiveId`; `completedMissionIds` sigue siendo la lista compatible para queries/prerrequisitos.

Los hechos de gameplay son contratos semánticos tipados (`GameplayFact`) con ID idempotente, `GameplayFactTypeId`, sujeto, tags, scope y secuencia asignada al registrar. Fotografía produce hechos `photograph` y, solo en el primer discovery, `discovery`; la interacción produce `interaction` con los tags aprobados del contenido. No se creó un segundo bus global: los casos de uso reciben `IMissionFactSink` explícito.

## Strategies implementadas

| Type ID | Regla | Fixture/evidencia |
|---|---|---|
| `mission-objective-type.discover-count` | Cuenta discoveries que coinciden con el tag opcional. | Fixture EditMode por `tag.world.jungle`. |
| `mission-objective-type.photograph-specific` | Cuenta fotos del `DiscoveryId` requerido. | Misión runtime del tucán. |
| `mission-objective-type.interact-tag` | Cuenta interacciones cuyo set contiene el tag requerido. | Fixture EditMode por `tag.world.jungle`. |

Agregar un tipo registra una strategy nueva en Bootstrap y authoring; no modifica un switch central. El validator rechaza type IDs sin strategy, IDs/referencias/keys/rewards ausentes, ciclos y prerrequisitos imposibles. Release acepta únicamente definitions Approved no-placeholder.

## Vertical Slice actual

`mission.vertical-slice.photograph-toucan` — “Fotografía al tucán pico canoa”:

- un objetivo `photograph-specific` para `discovery.jungle.keel-billed-toucan`;
- reward `reward.mission.photograph-toucan.complete` de 2 Estrellas de Explorador, provisional y transparente;
- activación explícita desde el panel baseline `PH_UI_MISSIONS`;
- progreso visible con poco texto, cierre automático positivo y checkpoint mediante el repositorio;
- ningún hecho anterior a activación cuenta; una captura posterior completa misión y reward una vez.

El catálogo MVP de 10+ misiones no se produce antes de Gate B. Exploración, observación, educación, ayuda y puzzles están modelados por contratos extensibles, no por contenido inventado en esta fase.

## Idempotencia, save y lifecycle

La secuencia de activación separa pre-eventos de hechos posteriores. `processedMissionFactIds` evita reprocesar el mismo hecho; Economy usa `economy-tx.mission.<mission-id>` como segunda barrera durable. Si el proceso cae después de completar la misión y antes de conceder la reward, el arranque y cualquier hecho posterior reconcilian la misma transacción. Un mismo hecho puede completar varias misiones activas; cada una usa su propia transacción. Tras cargar Save, Bootstrap reconcilia rewards y refresca el panel antes de mostrar Ready.

Contenido retirado conserva su estado en save y se ignora de forma segura. Schema futuro permanece read-only. Los IDs de hechos técnicos pueden crecer con el juego; medir/compactar de forma migrada antes de escalar contenido, nunca borrar claves a ciegas.

## Reglas infantiles

- Sin misiones diarias/semanales, rachas, caducidad, FOMO, pérdida de progreso o castigo.
- Sin claim manual obligatorio, velocidad premiada, scoring negativo ni reducción por pistas/intentos.
- Una misión incompleta comunica próximo paso sin culpa y permite salir/pausar.
- Rewards son Estrellas ganables, nunca dinero ni compra.
- Facts educativos siguen [`CONTENT_SOURCES.md`](CONTENT_SOURCES.md); la misión actual no añade claims nuevos al expediente Approved del tucán.

## Validación y aceptación

`scripts/validate-missions` verifica catálogo, referencias, localization, reward, editorial, UI/safe area y ausencia de un switch central. EditMode cubre tres strategies, pre-eventos, duplicados, multi-completion, reconciliación de arranque, prerequisites/ciclos, contenido retirado y migración v7→v8. PlayMode cubre foto→misión→reward→flush→reload y segunda captura sin duplicación. La comprensión infantil y Android táctil físico siguen pendientes; no son sustituidos por suites automatizadas.
