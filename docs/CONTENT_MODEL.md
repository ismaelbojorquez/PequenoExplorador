# Modelo data-driven de contenido

Contrato canónico de Prompt 14. Permite authoring y resolución local de contenido sin implementar reglas de discovery, world, mission, activity o reward. El único discovery versionado es neutral, `Draft` y placeholder; no representa contenido factual final.

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
| Discovery | `discovery.<mundo>.<slug>` | `discovery.jungle.placeholder` |
| Category | `category.<dominio>.<slug>` | `category.nature.placeholder` |
| Tag | `tag.<dominio>.<slug>` | `tag.jungle.placeholder` |
| Fact | `fact.<dominio>.<slug>` | `fact.jungle.placeholder.pending` |
| Source | `source.<owner>.<slug>` | `source.pending.human-review` |
| World | `world.<slug>` | `world.jungle` |
| Mission/activity/reward | `mission.*` / `activity.*` / `reward.*` | Solo contratos; sin instancias runtime |
| Visual | `visual.<owner>.<slug>` | `visual.discovery.jungle.placeholder` |

Category/world/tag no son enums cerrados. El botón explícito `Generate stable ID if empty` crea un ID solo cuando el campo está vacío; nunca sobrescribe. Retirar un Discovery ID exige alias `previous → current` en el catálogo y migración de save cuando el ID ya se haya publicado. Un alias no puede colisionar con un ID vigente ni apuntar fuera del catálogo.

## Definitions

| Runtime readonly | Authoring | Referencias mínimas |
|---|---|---|
| `DiscoveryDefinition` | `DiscoveryDefinitionAsset` | world/category/tags/facts, `LocalizedKey`, `AudioCueId`, `VisualAssetId`, metadata editorial |
| `CategoryDefinition` | `CategoryDefinitionAsset` | ID y metadata editorial |
| `TagDefinition` | `TagDefinitionAsset` | ID y metadata editorial |
| `EducationalFactDefinition` | `EducationalFactDefinitionAsset` | copy localizada, claim de revisión y source records |
| `ContentSourceRecord` | `ContentSourceRecordAsset` | institución/autor/título/referencia/consulta/revisor según avance |
| `IWorldDefinition`, `IMissionDefinition`, `IActivityDefinition`, `IRewardDefinition` | Diferido | Solo ID tipado; no reglas ficticias |

El catálogo mantiene diccionarios privados O(1) para category, tag, source, fact y discovery, más una colección de discoveries ordenada por ID para enumeración determinista. `TryGetDiscovery` resuelve el ID vigente; `TryResolveDiscovery` también consulta aliases.

## Estados editoriales y Release

`Draft → Sourced → Reviewed → Approved`; `Rejected` sale del catálogo. Development acepta Draft/placeholder solo con owner y watermark `BORRADOR · PH_`. Release acepta exclusivamente `Approved` y `isPlaceholder=false`; cada asset se valida por separado. Approved no concede licencia ni sustituye `ReleaseLocked` humano de [`CONTENT_SOURCES.md`](CONTENT_SOURCES.md).

El ejemplo neutral usa la key `content.discovery.placeholder.name`, el cue no factual `audio.feedback.confirm` y metadata JSON propia como visual técnico. No afirma especie, conducta o rasgo. El candidato tucán sigue fuera de runtime hasta fuente y revisión humana.

## Validación y reportes

`scripts/validate-content` ejecuta modo Development. Compile, Addressables y APK Development repiten el mismo gate. El path Release ejecuta modo Release antes del bloqueo de signing.

Se detectan: ID inválido/duplicado, alias inválido, referencia de catálogo ausente, key ES/EN inexistente, cue ausente, visual local ausente, slots duplicados, source/claim/revisor incompleto y estado no Approved. Cada error incluye código `DATA###`, asset path y corrección. Reportes ignorados:

- `artifacts/reports/content-catalog-development.{json,md}`;
- `artifacts/reports/content-catalog-release.{json,md}`.

Para agregar un placeholder: crear definitions/metadata, asignar referencias, añadirlo al único `ContentCatalogAsset`, ejecutar validator y tests. No se modifica `ContentCatalog`, Bootstrap, un switch central ni sistemas de gameplay. Producir catálogo masivo continúa bloqueado por Gate B y P-008.
