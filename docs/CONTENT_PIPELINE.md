# Pipeline técnico de contenido local

Estado: navegación local F07, catálogo data-driven Prompt 14 y manifiesto extensible de mundos Prompt 15. El único animal candidato tiene [dossier Sourced](VS_D_A01_TOUCAN_FACTUAL_DOSSIER.md), que no autoriza producción masiva, assets runtime ni contenido factual sin aprobación humana.

## Contrato Addressables

| Elemento | Valor canónico | Uso |
|---|---|---|
| Paquete | `com.unity.addressables@4.0.1` | Pin exacto; Unity Registry, Unity `6000.0+`. |
| Perfiles | `LocalDevelopment`, `LocalRelease` | Ambos usan únicamente `Local.BuildPath`/`Local.LoadPath`. |
| Grupos | `SharedLocal`, `JungleLocal` + seis grupos Unity Localization | Camp/mundo, diez clips audio PH_ y locales/tablas ES/EN, todos locales. |
| Addresses | `scene/camp`, `scene/jungle`, `audio/<categoría>/<cue>[/locale]` | IDs estables; no paths/strings dispersos en Domain. |
| Labels | `scene`, `shared-local`, `world-jungle`, `audio-local`, `audio-placeholder` | Selección/validación, no reglas de gameplay. |
| Catálogo | Local, actualización al arranque deshabilitada | Incluido en el player; sin remote catalog, host, CDN o URL. |

## Manifiestos de mundo

`WorldCatalogAsset` referencia `WorldManifestAsset` locales. Selva aporta `world.jungle` con `AssetReference` a `scene/jungle`, labels `scene`/`world-jungle`, spawn/checkpoint tipados, catálogo de contenido, cues, requirements, `manifestVersion=1`, `contentVersion=0.1.0-placeholder` y tamaño estimado. El asset/GUID queda en Content; `WorldCatalogCompiler` produce un catálogo readonly indexado O(1) sin `AssetDatabase` runtime.

El validator comprueba IDs/duplicados, ES/EN, cues, escena/address/grupo/labels locales, spawn, catálogos, versiones, tamaño y estado editorial. Outputs: `artifacts/reports/world-catalog-{development|release}.{json,md}`. Development acepta el stub Draft con watermark; Release produce `WORLD018` hasta reemplazo/aprobación. Disponibilidad `Locked` no equivale a entitlement, SKU ni compra.

Flujo: Camp enumera `IWorldCatalog` → selección entrega `WorldId` → `WorldLoadUseCase` resuelve manifest → scene flow carga su `SceneContentId` → la sesión retiene el manifest activo → volver a Camp descarga el handle y limpia la sesión. Añadir una fixture de mundo cambia solo datos del catálogo; no el loader/coordinador. No existe descarga, tamaño remoto ni endpoint.

El `ContentCatalogAsset` local se serializa desde Bootstrap y se compila una vez a `IContentCatalog`; no es un remote catalog ni usa `AssetDatabase` runtime. `MissionCatalogAsset`, `LearningCatalogAsset` y `RewardCatalogAsset` hacen lo mismo para sus definitions/referencias. Definitions referencian IDs semánticos, nunca GUID como lógica. Esquema y gate editorial: [`CONTENT_MODEL.md`](CONTENT_MODEL.md).

`Bootstrap.unity` es el entry point persistente de Build Settings. `Camp.unity` y `Jungle.unity` no se añaden a esa lista: se empaquetan en bundles locales y se cargan aditivamente. `SharedLocal` no puede depender de ninguna entrada de `JungleLocal`; el validador usa dependencias reales de AssetDatabase.

Las colecciones `Shared`, `UI`, `Content`, `Voice` e `Illustrations`, locales `es`/`en` y pseudo Development pertenecen a Content. Unity Localization crea grupos `Localization-Locales`, `Localization-Assets-Shared`, string tables ES/EN y asset tables ES/EN; el validador exige `Local.BuildPath`/`Local.LoadPath` y prohíbe endpoints. Las keys y CSV se gobiernan en [`17_LOCALIZATION.md`](17_LOCALIZATION.md).

Los diez WAV baseline también viven en `SharedLocal`, con addresses/labels validados y cero endpoint. `AudioCueDefinition` conserva referencias y direcciones preparadas para carga addressable futura; Prompt 12 no introduce descarga ni ownership de handles de audio. Reemplazar clips exige ledger/licencia, diff de grupo, build local, prueba de duplicación/tamaño y aprobación Release según [`AUDIO_REQUIREMENTS.md`](AUDIO_REQUIREMENTS.md).

## Ownership y build

Infrastructure crea y posee cada `AsyncOperationHandle<SceneInstance>`. Application solo ve `ISceneContentHandle`; un unload idempotente consume ownership una vez. Cancelar o agotar timeout no abandona una operación Unity: el adapter espera su estado seguro, descarga el resultado si llegó a cargar y luego informa cancelación. Volver a Camp deja un handle de Camp y cero de Jungle; shutdown deja cero.

```sh
scripts/build-addressables-local
```

El comando valida configuración, cambia el target de contenido a Android, selecciona `LocalDevelopment`, construye el catálogo y escribe `artifacts/reports/addressables-local.json`. El APK Development repite ese build antes del player. Runtime output y `addressables_content_state.bin` se fuerzan bajo `Library/com.unity.addressables/`; reportes/builds viven en `artifacts/`. Todo ello es generado/ignorado; `Assets/AddressableAssetsData/`, escenas y metadata sí se versionan.

### Fixture visual del tucán

`scripts/apply-toucan-fixture` invoca tooling Editor gráfico, genera materiales/prefab deterministas, integra solo el hijo visual del interactable animal neutral y escribe evidencia ignorada. No descarga, no usa `Resources`, no crea paquetes ni activa endpoints. Sus outputs versionados están bajo `Assets/_Game/Content/Discoveries/Jungle/KeelBilledToucan`; métricas y renders quedan bajo `artifacts/`.

El generator es idempotente: conserva el GUID del prefab y, si la instancia correcta ya existe, no reconstruye Jungle. Release permanece fail-closed porque `Sourced` no equivale a `Approved`. Adoptar los IDs futuros exige una fase separada con aliases/save/localización y firmas; no se cambia el catálogo factual desde este tooling.

## Guardrails

- No crear `Remote*`, URL, Hosting Services, CCD, CDN o profile remoto.
- No llamar `Addressables.Release` desde Presentation, Bootstrap o features.
- No almacenar handles en ScriptableObjects, estáticos o mundos.
- Todo placeholder conserva `PH_` y metadata Release `Blocked`.
- Todo contenido factual sigue [`CONTENT_SOURCES.md`](CONTENT_SOURCES.md); ser Addressable no equivale a estar aprobado.
- Todo Draft lleva watermark Development; Release rechaza cada definition que no sea Approved o conserve placeholder.
- Antes de añadir mundo/manifiesto: dependencia/provenance, tamaño estimado/budget aprobado, fallback offline, unload test, tres ciclos PlayMode y Android smoke.
- Antes de añadir misión: objectives con strategy registrada, referencias/keys/reward existentes, grafo de prerequisites acíclico, editorial Approved para Release y prueba de pre-event/idempotencia.

## Evolución futura

Los mundos ya se describen mediante manifiestos data-driven; no se añadirá un `switch` central por cada bioma. Descarga/remote catalogs son post-MVP y requieren caso de uso, backend, modelo de actualización/rollback, seguridad, privacidad infantil, store disclosures y ADR humana/técnica.
