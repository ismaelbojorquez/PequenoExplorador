# Arquitectura técnica — fronteras modulares

Estado: foundation, catálogo data-driven Draft, manifiesto local de Selva, scene flow, save v3, localización, audio, input, locomoción e interacción contextual `PH_` implementados. No existen discovery ni contenido final. `Bootstrap` persiste y entra a Camp placeholder.

## Grafo real de assemblies

```text
PequenoExplorador.Domain
└─ sin referencias; noEngineReferences=true

PequenoExplorador.Application
└─ Domain; noEngineReferences=true

PequenoExplorador.Content
├─ Application
├─ Domain
├─ Unity.Addressables
└─ UnityEngine.AudioModule

PequenoExplorador.Infrastructure
├─ Application
├─ Domain
├─ Unity.Addressables
├─ Unity.ResourceManager
├─ Unity.Localization
├─ Unity.InputSystem
└─ UnityEngine.AudioModule

PequenoExplorador.Presentation
├─ Application
├─ Domain
└─ UnityEngine.AIModule

PequenoExplorador.Bootstrap
├─ Application
├─ Content
├─ Domain
├─ Infrastructure
├─ Presentation
├─ Unity.InputSystem
└─ UnityEngine.AudioModule

PequenoExplorador.Editor [Editor only]
├─ Application / Bootstrap / Content / Domain
├─ Infrastructure / Presentation
├─ Unity.Addressables / Unity.Addressables.Editor / Unity.AI.Navigation / Unity.InputSystem
├─ Unity.Localization / Unity.Localization.Editor / Unity.RenderPipelines.Universal.Runtime
└─ UnityEngine.AudioModule

PequenoExplorador.Tests.EditMode [Editor only]
├─ Application
├─ Bootstrap
├─ Content
├─ Domain
├─ Editor
├─ Infrastructure
├─ Presentation
├─ Unity.Localization / Unity.Localization.Editor / Unity.InputSystem
└─ UnityEngine.AudioModule

PequenoExplorador.Tests.PlayMode
├─ Application
├─ Bootstrap
├─ Domain
├─ Infrastructure / Presentation
└─ Unity.InputSystem / Unity.InputSystem.TestFramework / UnityEngine.AIModule / UnityEngine.AudioModule
```

Son nueve assemblies de proyecto. Las suites reciben NUnit/TestRunner mediante `optionalUnityReferences: TestAssemblies`; no usan `overrideReferences`. Las dependencias implícitas de Unity solo están disponibles donde `noEngineReferences=false`.

El player Android inspeccionado contiene únicamente los seis assemblies runtime en `ManagedStripped`; `PequenoExplorador.Editor` y ambas suites no entran al APK.

## Responsabilidades y límites

| Assembly | Responsabilidad autorizada | Referencias prohibidas clave |
|---|---|---|
| Domain | Reglas y estado C# puro cuando existan casos reales. | `UnityEngine`, plataforma, filesystem, UI, SDKs. |
| Application | Lifecycle, contexto inmutable y puertos sobre Domain/BCL. | Unity, concretos de Infrastructure/Presentation/Content. |
| Content | Authoring y mapeo de contenido aprobado. | Infrastructure y Presentation; estado mutable de sesión. |
| Infrastructure | Reloj/random/logger/bus, adapters Null/Mock/seguros, ownership Addressables y save DTO/filesystem. | Presentation, Content y Bootstrap. |
| Presentation | Vistas y adapters Unity de UI/input/cámara/locomoción; NavMesh implementa un puerto Application sin filtrar al núcleo. | Infrastructure, filesystem, ads, IAP y concretos de plataforma. |
| Bootstrap | Único composition root; configura perfil y ensambla puertos/concretos explícitamente. | Reglas de producto, lookup genérico, service locator o singleton global. |
| Editor | Build/setup/validación que nunca entra en player. | Gameplay y estado runtime. |
| Tests | Evidencia por frontera y escena; fixtures controladas. | Dependencias innecesarias, red/reloj/azar real. |

Los markers `*AssemblyMarker` conservan pruebas de enlace. `DiagnosticBootstrap` adapta `Awake/Start/OnDestroy` y delega el lifecycle a `ApplicationHost`; no contiene reglas de producto ni gameplay.

## Composition root y lifecycle reales

```text
DiagnosticBootstrap (Unity lifecycle)
  ├─ BuildProfileConfiguration → Content AppConfig assets → IAppConfig readonly
  └─ ServiceRegistry [internal, typed, no Get<T>]
      ├─ AppContext [immutable, explicit injection]
      │   ├─ IAppConfig / IFeatureFlags
      │   ├─ IClock / IRandomSource / IAppLogger
      │   ├─ IMessageBus
      │   ├─ IInputService / ISafeAreaService / IHapticsService
      │   ├─ IContentCatalog [readonly, O(1), no lifecycle]
      │   ├─ IWorldCatalog / IWorldSession [readonly catalog + sesión explícita]
      │   ├─ ISaveService → IFileStore
      │   ├─ ILocalizationService → Unity Localization
      │   ├─ IAnalyticsService
      │   ├─ IAdsService
      │   ├─ IPurchaseService
      │   └─ ISceneFlowService → ISceneContentLoader
      └─ ApplicationHost
          initialize: MessageBus → Input → SafeArea → Haptics → Save → Localization → Audio → Analytics → Ads → Purchases
          shutdown:  Purchases → Ads → Analytics → Audio → Localization → Save → Haptics → SafeArea → Input → MessageBus
```

`ApplicationHost.InitializeAsync` es secuencial, comparte la misma tarea ante llamadas concurrentes, acepta `CancellationToken`, permite retry después de fallo recuperable y hace cleanup inverso. `Shutdown`/`Dispose` son idempotentes; si llegan durante inicialización solicitan cancelación del host, impiden volver a `Ready` y limpian también el servicio que estaba inicializando. `AppContext` no es estático y no ofrece lookup; Bootstrap lo retiene para inyección explícita en futuras fachadas.

## Configuración runtime

Content posee dos ScriptableObjects locales: Development y Release. `AppConfigMapper` convierte authoring a `AppConfig`/`FeatureFlags` inmutables de Application; `AppConfigCatalog` exige exactamente un asset por ID. Bootstrap es el único consumidor de `Resources/Configuration`, selecciona por define compilado y entrega la interfaz al contexto/registry. No hay fallback inventado: config ausente/duplicada/insegura bloquea import/build.

Seed, timeout scene flow, debounce save, nombre/versión técnica y selección de mocks/diagnóstico dejaron de ser hardcodes de Bootstrap. Android/API/signing, Addressables y preferencias parentales siguen en sus autoridades respectivas; detalles y tabla de flags: [`RUNTIME_CONFIGURATION.md`](RUNTIME_CONFIGURATION.md).

## Persistencia local

```text
Domain.PlayerProgress
          ↓
Application.ISaveService / AutosaveCoordinator / IFileStore
          ↓
Infrastructure.LocalSaveService
  ├─ UnityJsonSaveSerializer → envelope/DTO v1/v2/v3 + SHA-256
  ├─ ISaveMigration[] → pasos n→n+1
  └─ LocalFileStore → persistentDataPath/Save
```

Application y features no conocen JSON ni paths. Infrastructure referencia Domain directamente porque implementa firmas públicas que mapean `PlayerProgress`; la dirección sigue hacia adentro y el grafo permanece acíclico. Bootstrap es el único lugar que resuelve `Application.persistentDataPath`. Presentation solo consume `SaveUserNotice` para copy recuperable. Contrato, archivos, downgrade y recovery: [`10_SAVE_SYSTEM.md`](10_SAVE_SYSTEM.md).

### Localización

`Application.Localization` define `LocalizedKey`/`ILocalizationService`; Infrastructure es el único adapter de Unity Localization; Content posee tablas/locales; Bootstrap lo inicializa después de Save e inyecta la fachada en Presentation. Domain solo guarda el enum de preferencia ES/EN, no texto. `LocaleChanged` refresca vistas y se limpia en shutdown; pseudo no llega a Release ni al save. Contrato y fallback: [`17_LOCALIZATION.md`](17_LOCALIZATION.md).

### Audio localizado

```text
Application: IAudioService + AudioCueId/settings/subtitle
            ↓
Infrastructure: UnityAudioService → AudioSource/AudioMixer/voice queue
            ↑
Content: AudioCueDefinition + AudioCueCatalog + WAV locales PH_
            ↑
Bootstrap: mapping/composition/lifecycle
            ↓
Presentation: play/replay/settings/subtitle
```

Domain no conoce audio ni archivos. El root Bootstrap posee exactamente siete sources y un driver; Music/Ambience son exclusivos, Effects tiene pool fijo de cuatro y Voice serializa una cola de cuatro con prioridad. `LocaleChanged` afecta clip/subtítulo; pause/focus suspende, shutdown detiene loops, vacía cola/cooldown y limpia listeners. Faltantes devuelven `Missing` sin bloquear. Contrato/ledger: [`16_AUDIO.md`](16_AUDIO.md) y [`AUDIO_REQUIREMENTS.md`](AUDIO_REQUIREMENTS.md).

### Input y adaptación móvil

Application define `IInputService`, `ISafeAreaService` e `IHapticsService`; el clasificador de gestos es C# puro. Infrastructure concentra Input System/EnhancedTouch y `Screen.safeArea`; Presentation recibe intenciones y snapshots. Bootstrap selecciona `UI` para Camp/transición/pausa y `Explorer` para Expedition. `Photography`/`Parents` quedan preparados sin feature, mientras `Debug` es aditivo y Development-only. Haptics es no-op/desactivado. Contrato, thresholds, ratios y hardware pendiente: [`INPUT_ACCESSIBILITY.md`](INPUT_ACCESSIBILITY.md).

### Locomoción candidata

```text
Infrastructure.UnityInputService
          ↓ InputIntent(Tap) / MapChanged
Application.ExplorerLocomotionController → IPathNavigator
          ↑                                  ↑
Presentation.ExplorerLocomotionRoot ─ UnityNavMeshPathNavigator
          ├─ raycast + marker PH_
          ├─ NavMeshAgent / NavMeshSurface 2.0.9
          └─ camera follow + bounds + reduce motion

Bootstrap enlaza exactamente una raíz tras carga Addressable y la suelta antes del unload.
```

Application conserva posiciones/estado/settings BCL-only. Presentation es owner de referencias Unity, raycast, path, visual y cámara; Bootstrap puede inspeccionar roots una sola vez como composition root, nunca por frame. Un cambio de mapa fuera de `Explorer`, pause/focus o unload cancela el destino. No hay singleton, joystick, root motion ni `FindObjectOfType` de gameplay. Parámetros, riesgos y límites de hardware: [`INPUT_ACCESSIBILITY.md`](INPUT_ACCESSIBILITY.md).

### Catálogos data-driven

Domain define value IDs textuales por tipo. Content posee definitions ScriptableObject y compiladores; Bootstrap compila una vez a modelos Application readonly y entrega `IContentCatalog`/`IWorldCatalog` en `AppContext`. El catálogo de contenido indexa category, tag, source, fact y discovery; el catálogo de mundos indexa manifests por `WorldId`. Editor es el único consumidor de `AssetDatabase` para comprobar paths, localización, audio, visuales, escenas, labels, spawn y estado editorial. Contratos: [`CONTENT_MODEL.md`](CONTENT_MODEL.md) y [`CONTENT_PIPELINE.md`](CONTENT_PIPELINE.md).

### Interacción contextual

```text
Presentation detector/prompt ──→ Application coordinator/contracts ──→ Domain InteractionId
           │                              │
           └─ IInteractionApproach ───────┘
Content InteractionDefinitionAsset ──compile──→ readonly catalog
Bootstrap enlaza catálogo + raíz de Selva + servicios localización/audio
```

Presentation referencia Domain solo porque la definición pública expone el value ID tipado; la regla queda en la allowlist y no invierte dependencias. `InteractionCoordinator` conserva un foco, serializa approach/acción y limpia por UI, pause, target destruido o unload. El adapter de detección indexa colliders al bind y usa raycast non-alloc; no hay `GetComponent` por frame, categoría animal, service locator ni bus global. `DiscoveryInteractionAction` implementa la primera acción concreta mediante `DiscoverUseCase`; learning seguirá una conexión explícita posterior. Contrato: [`INTERACTION_SYSTEM.md`](INTERACTION_SYSTEM.md).

### Discovery persistente

```text
Content DiscoveryDefinition ──→ DiscoverUseCase ←── IClock + grant.*
                                      ↓
                         IDiscoveryProgressRepository
                                      ↓
               PlayerProgress v4 → AutosaveCoordinator.Latest
                                      ↓
                    Infrastructure Save DTO/migration/file
```

Domain posee `DiscoveryProgress` y value IDs; Application decide first/repeat/idempotencia/aprobación y calcula queries contra el catálogo; Infrastructure solo serializa DTO v4; Bootstrap compone. `DiscoverResult` no contiene estrellas, UI ni Audio. El día local se reduce a `yyyy-MM-dd`; no se guarda hora/zona/identidad. Los denominadores se derivan de definitions Approved vigentes y records retirados se preservan sin contarlos.

## Scene flow y contenido local

```text
Presentation.SceneTransitionView
             ↓ WorldId
Application.WorldLoadUseCase → IWorldCatalog / IWorldSession
             ↓ SceneContentId del manifest
Application.SceneFlowService ── estado / exclusión / retry / timeout
             ↓ ISceneContentLoader
Infrastructure.AddressableSceneContentLoader ── owner único de handles
             ↓
Addressables local: SharedLocal(Camp) / JungleLocal(Jungle)

Bootstrap.unity persiste; Camp/Jungle son aditivas.
```

La máquina permite `Boot→Camp`, `Camp→Expedition` y `Expedition→Camp`. `WorldLoadUseCase` resuelve `world.jungle` en O(1), distingue missing/locked de entitlement comercial y entrega al scene flow el address semántico del manifest. Un segundo intento recibe `Busy`; error/cancel/timeout conserva mundo/escena para retry. Cancelar no abandona la operación Unity: el adapter espera un punto seguro y descarga cualquier escena resultante. Cada handle se consume una vez; volver a Camp borra la sesión activa, conserva solo Camp y shutdown deja cero. Presentation no conoce Addressables y Bootstrap solo cablea eventos/puertos.

`WorldManifestAsset` pertenece a Content y contiene `AssetReference` de escena, labels, spawn, checkpoints, catálogos, cues, requisitos, versiones y tamaño instalado estimado. El compilador elimina Unity/GUIDs del modelo runtime `WorldManifest`; Application solo ve IDs/keys/cues readonly. Selva es el único asset real (`world.jungle`); el segundo mundo es una fixture in-memory que demuestra que el coordinador no cambia.

Addressables `4.0.1` queda local-only: perfiles `LocalDevelopment`/`LocalRelease`, grupos `SharedLocal`/`JungleLocal`, labels `scene`/`shared-local`/`world-jungle`, actualización y remote catalog deshabilitados. No hay endpoint. El validador bloquea paths no locales, labels/addresses incorrectos y dependencias Shared→Jungle. Contrato completo: [`CONTENT_PIPELINE.md`](CONTENT_PIPELINE.md).

El bus en memoria solo cubre fan-out acotado. `Subscribe<T>` devuelve `IDisposable`; dispose y shutdown eliminan listeners. No sustituye llamadas directas, no es global y no convierte mensajes en analytics.

## Perfiles de servicios

| Puerto | Development | Release |
|---|---|---|
| Analytics | `NullAnalyticsService` | `NullAnalyticsService` |
| Ads | `MockAdsService` si flag local ON; default ON | `NoAdsService`; `MockAds` prohibido |
| Purchases | `MockPurchaseService` si flag local ON; default ON | `UnavailablePurchaseService`; `MockPurchases` prohibido |
| Save | Local schema v4 | Local schema v4; herramientas Editor excluidas |
| Localization | ES/EN + pseudo y selector diagnóstico | ES/EN; pseudo/selector diagnóstico excluidos |
| Audio | Mixer/cues PH_, panel y replay diagnóstico | Servicio local; panel oculto y placeholders bloquean Release de contenido |
| Input/safe area | 5 mapas; Debug overlay local; presets de ratio | Mapas de producto, Debug deshabilitado; safe area local |
| Haptics | No-op, off por defecto | No-op, off por defecto |
| Interaction | Fixtures `PH_`, diagnóstico localizado | Catálogo final Approved; fixtures `PH_` bloqueados |
| Clock/random | `SystemClock` + seed inyectado | Igual, sin estado remoto |
| Logs/messages | Structured Unity + bus local | Igual, sin datos infantiles |

Editor o `PE_DEVELOPMENT_SERVICES` compilan la posibilidad de mocks; `IAppConfig` decide explícitamente si se seleccionan. El define se pasa únicamente en `BuildPlayerOptions.extraScriptingDefines` del APK Development y no se persiste en PlayerSettings. Fuera de esos símbolos, las clases mock y la rama Development no compilan. Release carga un perfil con cero flags y el validador rechaza cualquier flag inseguro. El diagnóstico de producto/versión se oculta en Release; la vista mínima de estado/error recuperable permanece.

## Enforcement

`AssemblyBoundaryRules` carga los nueve `.asmdef` reales y exige:

- allowlist exacta de referencias y namespaces `PequenoExplorador.*`;
- Domain/Application sin engine;
- Editor y EditMode restringidos a Editor;
- ningún `autoReferenced` u `overrideReferences`;
- grafo sin ciclos y sin assembly adicional no revisado.

El comando CLI es:

```sh
"$UNITY_EDITOR" -batchmode -nographics -quit \
  -projectPath "$(pwd)" \
  -executeMethod PequenoExplorador.Editor.AssemblyBoundaryValidationCli.Validate \
  -logFile /tmp/pequeno-explorador-boundaries.log
```

Una fixture EditMode añade en memoria `Presentation → Infrastructure` y verifica su rechazo; otra crea un ciclo Domain/Application. Ningún `.asmdef` real se rompe para probar el guardrail.

## Evolución

Solo Bootstrap conoce simultáneamente Presentation, Content e Infrastructure. La composición futura seguirá explícita, por constructor/factory, sin `Find*`, service locator ni `Instance` global. ScriptableObjects siguen siendo authoring; save usa DTOs versionados exclusivamente en Infrastructure; todo SDK real requiere reemplazar un adapter mediante ADR, no filtrar su tipo hacia gameplay.

Subdividir un assembly requiere evidencia de tiempos de compilación, ownership, plataforma o aislamiento de dependencias. Añadir una feature no basta. Todo cambio de grafo actualiza la allowlist, este documento, decisión/riesgo aplicables y las suites.

## Foundation móvil conservada

- Unity `6000.3.22f1`, Addressables `4.0.1`, AI Navigation `2.0.9`, URP `17.3.0`, Input System `1.20.0`, Test Framework `1.6.0`, uGUI `2.0.0`.
- Bootstrap es la única escena habilitada en Build Settings; Camp/Jungle son locales Addressable. Development muestra navegación/fallo simulado; Release oculta controles Development.
- Android sigue min API 26, target/compile 36, IL2CPP y ARM64; sin manifest/Gradle custom ni permiso sensible nuevo.
- Existen locomoción, interacción y discovery directo `PH_`; no existen fotografía/álbum/economía/UI final, contenido remoto ni SDKs comerciales. Save schema v4 no guarda PII/cuentas; ads/IAP/analytics son únicamente Null/Mock/Unavailable locales sin red.
