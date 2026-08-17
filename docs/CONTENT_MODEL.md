# Modelo data-driven de contenido

Contrato canónico de Prompt 14, ampliado por Prompt 15–18 y la adopción técnica de `VS-D-A01`. El único discovery animal runtime, sus siete facts, seis fuentes, categoría/tag e interacción son `Approved`; mundo, planta, objeto y audio final permanecen Draft/placeholder.

## Flujo y autoridades

```text
Content ScriptableObjects
  → ContentCatalogCompiler + validator Editor
  → definitions readonly + ContentCatalog indexado
  → IContentCatalog en AppContext/Application
```

Domain posee value IDs y no conoce `UnityEngine`, GUID, paths o assets. Content posee ScriptableObjects y mapping; Bootstrap compila una vez el asset serializado y entrega `IContentCatalog`. Runtime nunca usa `AssetDatabase`, no depende del orden de arrays y no muta authoring.

## Esquema de IDs

IDs usan minúsculas ASCII, dígitos, puntos y guiones; el primer segmento fija el tipo. Renombrar archivo/asset no cambia el ID.

| Tipo | Forma | Ejemplo baseline |
|---|---|---|
| Discovery | `discovery.<mundo>.<slug>` | `discovery.jungle.keel-billed-toucan` |
| Category | `category.<dominio>.<slug>` | `category.discovery.animals` |
| Tag | `tag.<dominio>.<slug>` | `tag.world.jungle` |
| Fact | `fact.<dominio>.<slug>` | `fact.jungle.keel-billed-toucan.diet` |
| Source | `source.<owner>.<slug>` | `source.conabio.ramphastos-sulfuratus-2025` |
| World | `world.<slug>` | `world.jungle` |
| Scene/catalog/spawn/checkpoint/requirement | `scene/*`, `catalog.*`, `spawn.*`, `checkpoint.*`, `requirement.*` | Contrato del manifest Selva |
| Mission/activity/reward | `mission.*` / `activity.*` / `reward.*` | Solo contratos; sin instancias runtime |
| Visual | `visual.<owner>.<slug>` | `visual.discovery.jungle.keel-billed-toucan` |
| Interaction | `interaction.<owner>.<slug>` | `interaction.jungle.keel-billed-toucan` |
| Discovery grant | `grant.<origen>.<slug>` | `grant.interaction.<ticks>.discovery.jungle.keel-billed-toucan` |

El mapping vigente es `interaction.jungle.keel-billed-toucan → discovery.jungle.keel-billed-toucan`. El alias `discovery.jungle.placeholder → discovery.jungle.keel-billed-toucan` resuelve referencias retiradas y save v5 normaliza progreso/grants v4.

## Candidato visual reproducible

`ToucanReviewFixtureMetadata` declara identidad, autoría, estado `Approved`, aprobaciones visual/factual, bounds y referencias readonly. `ToucanFixtureSetup` conecta el prefab propio al interactable real; Application no conoce especie, path, prefab ni GameObject. `H-008`/`H-009` son exigidas por validator. Las definitions neutrales retiradas fueron eliminadas del catálogo y reemplazadas por assets `VS_`.

Category/world/tag no son enums cerrados. El botón explícito `Generate stable ID if empty` crea un ID solo cuando el campo está vacío; nunca sobrescribe. Retirar un Discovery ID exige alias `previous → current` en el catálogo y migración de save cuando el ID ya se haya publicado. Un alias no puede colisionar con un ID vigente ni apuntar fuera del catálogo.

## Definitions

| Runtime readonly | Authoring | Referencias mínimas |
|---|---|---|
| `DiscoveryDefinition` | `DiscoveryDefinitionAsset` | world/category/tags/facts, `LocalizedKey`, `AudioCueId`, `VisualAssetId`, metadata editorial |
| `CategoryDefinition` | `CategoryDefinitionAsset` | ID y metadata editorial |
| `TagDefinition` | `TagDefinitionAsset` | ID y metadata editorial |
| `EducationalFactDefinition` | `EducationalFactDefinitionAsset` | copy localizada, claim de revisión y source records |
| `ContentSourceRecord` | `ContentSourceRecordAsset` | institución/autor/título/referencia/consulta/revisor según avance |
| `WorldManifest` / `IWorldDefinition` | `WorldManifestAsset` | escena/labels/spawn/checkpoints/catálogos/cues/requirements/version/tamaño/editorial |
| `IMissionDefinition`, `IActivityDefinition`, `IRewardDefinition` | Diferido | Solo ID tipado; no reglas ficticias |
| `InteractionDefinition` | `InteractionDefinitionAsset` | ID, copy/cues, rango, cooldown, prioridad, discovery directo opcional y metadata editorial |

El catálogo de contenido mantiene diccionarios privados O(1) para category, tag, source, fact y discovery, más una colección de discoveries ordenada por ID. `TryGetDiscovery` resuelve el ID vigente; `TryResolveDiscovery` consulta aliases. `WorldCatalog` mantiene un índice O(1) separado y orden estable; su disponibilidad técnica no decide acceso comercial.

## Estados editoriales y Release

`Draft → Sourced → Reviewed → Approved`; `Rejected` sale del catálogo. Development acepta Draft/placeholder solo con owner y watermark `BORRADOR · PH_`. Release acepta exclusivamente `Approved` y `isPlaceholder=false`; cada asset se valida por separado. Approved no concede licencia ni sustituye `ReleaseLocked` humano de [`CONTENT_SOURCES.md`](CONTENT_SOURCES.md).

`VS-D-A01` usa keys namespaced ES/EN y el cue no factual `audio.feedback.confirm` como confirmación temporal, no como voz de nombre. Conservación no se materializa como fact runtime. Audio específico continúa pendiente y no invalida el fallback silencioso.

## Validación y reportes

`scripts/validate-content` ejecuta modo Development. Compile, Addressables y APK Development repiten el mismo gate. El path Release ejecuta modo Release antes del bloqueo de signing.

Se detectan: ID inválido/duplicado, alias inválido, referencia de catálogo ausente, key ES/EN inexistente, cue ausente, visual local ausente, slots duplicados, source/claim/revisor incompleto y estado no Approved. Cada error incluye código `DATA###`, asset path y corrección. Reportes ignorados:

- `artifacts/reports/content-catalog-development.{json,md}`;
- `artifacts/reports/content-catalog-release.{json,md}`.

Para agregar un placeholder: crear definitions/metadata, asignar referencias, añadirlo al único `ContentCatalogAsset`, ejecutar validator y tests. No se modifica `ContentCatalog`, Bootstrap, un switch central ni sistemas de gameplay. Producir catálogo masivo continúa bloqueado por Gate B y P-008.

Las interacciones usan un catálogo separado compilado una vez y el mismo `WorldInteractableView`, sin enums animal/planta/objeto. El enlace opcional a `DiscoveryId` se resuelve por datos; el adapter llama `DiscoverUseCase` y no cambia selección, detector o locomoción. Progress/count/grants viven en Save v5, no en ScriptableObjects ni catálogo.
