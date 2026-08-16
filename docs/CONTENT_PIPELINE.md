# Pipeline técnico de contenido local

Estado: baseline F07 para navegación placeholder; no autoriza producción masiva ni contenido factual sin aprobación.

## Contrato Addressables

| Elemento | Valor canónico | Uso |
|---|---|---|
| Paquete | `com.unity.addressables@4.0.1` | Pin exacto; Unity Registry, Unity `6000.0+`. |
| Perfiles | `LocalDevelopment`, `LocalRelease` | Ambos usan únicamente `Local.BuildPath`/`Local.LoadPath`. |
| Grupos | `SharedLocal`, `JungleLocal` + seis grupos Unity Localization | Camp/mundo y locales/tablas ES/EN, todos locales. |
| Addresses | `scene/camp`, `scene/jungle` | Constantes en Infrastructure; no strings dispersos. |
| Labels | `scene`, `shared-local`, `world-jungle` | Selección/validación, no reglas de gameplay. |
| Catálogo | Local, actualización al arranque deshabilitada | Incluido en el player; sin remote catalog, host, CDN o URL. |

`Bootstrap.unity` es el entry point persistente de Build Settings. `Camp.unity` y `Jungle.unity` no se añaden a esa lista: se empaquetan en bundles locales y se cargan aditivamente. `SharedLocal` no puede depender de ninguna entrada de `JungleLocal`; el validador usa dependencias reales de AssetDatabase.

Las colecciones `Shared`, `UI`, `Content`, `Voice` e `Illustrations`, locales `es`/`en` y pseudo Development pertenecen a Content. Unity Localization crea grupos `Localization-Locales`, `Localization-Assets-Shared`, string tables ES/EN y asset tables ES/EN; el validador exige `Local.BuildPath`/`Local.LoadPath` y prohíbe endpoints. Las keys y CSV se gobiernan en [`17_LOCALIZATION.md`](17_LOCALIZATION.md).

## Ownership y build

Infrastructure crea y posee cada `AsyncOperationHandle<SceneInstance>`. Application solo ve `ISceneContentHandle`; un unload idempotente consume ownership una vez. Cancelar o agotar timeout no abandona una operación Unity: el adapter espera su estado seguro, descarga el resultado si llegó a cargar y luego informa cancelación. Volver a Camp deja un handle de Camp y cero de Jungle; shutdown deja cero.

```sh
scripts/build-addressables-local
```

El comando valida configuración, cambia el target de contenido a Android, selecciona `LocalDevelopment`, construye el catálogo y escribe `artifacts/reports/addressables-local.json`. El APK Development repite ese build antes del player. Runtime output y `addressables_content_state.bin` se fuerzan bajo `Library/com.unity.addressables/`; reportes/builds viven en `artifacts/`. Todo ello es generado/ignorado; `Assets/AddressableAssetsData/`, escenas y metadata sí se versionan.

## Guardrails

- No crear `Remote*`, URL, Hosting Services, CCD, CDN o profile remoto.
- No llamar `Addressables.Release` desde Presentation, Bootstrap o features.
- No almacenar handles en ScriptableObjects, estáticos o mundos.
- Todo placeholder conserva `PH_` y metadata Release `Blocked`.
- Todo contenido factual sigue [`CONTENT_SOURCES.md`](CONTENT_SOURCES.md); ser Addressable no equivale a estar aprobado.
- Antes de añadir mundo/manifiesto: dependencia/provenance, budget, fallback offline, unload test, tres ciclos PlayMode y Android smoke.

## Evolución futura

Los mundos se describirán mediante manifiestos data-driven; no se añadirá un `switch` central por cada bioma. Descarga/remote catalogs son post-MVP y requieren caso de uso, backend, modelo de actualización/rollback, seguridad, privacidad infantil, store disclosures y ADR humana/técnica.
