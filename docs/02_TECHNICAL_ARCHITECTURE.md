# Arquitectura técnica — fronteras modulares

Estado: fronteras F04, composition root F06, scene flow local y persistencia schema v1 implementados. No define APIs de gameplay. `Bootstrap` persiste y entra a Camp placeholder.

## Grafo real de assemblies

```text
PequenoExplorador.Domain
└─ sin referencias; noEngineReferences=true

PequenoExplorador.Application
└─ Domain; noEngineReferences=true

PequenoExplorador.Content
└─ Application

PequenoExplorador.Infrastructure
├─ Application
├─ Domain
├─ Unity.Addressables
└─ Unity.ResourceManager

PequenoExplorador.Presentation
└─ Application

PequenoExplorador.Bootstrap
├─ Application
├─ Content
├─ Domain
├─ Infrastructure
└─ Presentation

PequenoExplorador.Editor [Editor only]
├─ Bootstrap
├─ Infrastructure / Presentation
├─ Unity.Addressables.Editor / Unity.InputSystem
└─ Unity.RenderPipelines.Universal.Runtime

PequenoExplorador.Tests.EditMode [Editor only]
├─ Application
├─ Bootstrap
├─ Domain
├─ Editor
├─ Infrastructure
└─ Presentation

PequenoExplorador.Tests.PlayMode
├─ Application
├─ Bootstrap
├─ Domain
└─ Infrastructure
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
| Presentation | Vistas Bootstrap/transición y futuros adaptadores Unity de UI/input/cámara/audio; consume `ISceneFlowService`. | Infrastructure, filesystem, ads, IAP y concretos de plataforma. |
| Bootstrap | Único composition root; configura perfil y ensambla puertos/concretos explícitamente. | Reglas de producto, lookup genérico, service locator o singleton global. |
| Editor | Build/setup/validación que nunca entra en player. | Gameplay y estado runtime. |
| Tests | Evidencia por frontera y escena; fixtures controladas. | Dependencias innecesarias, red/reloj/azar real. |

Los markers `*AssemblyMarker` conservan pruebas de enlace. `DiagnosticBootstrap` adapta `Awake/Start/OnDestroy` y delega el lifecycle a `ApplicationHost`; no contiene reglas de producto ni gameplay.

## Composition root y lifecycle reales

```text
DiagnosticBootstrap (Unity lifecycle)
  └─ ServiceRegistry [internal, typed, no Get<T>]
      ├─ AppContext [immutable, explicit injection]
      │   ├─ IClock / IRandomSource / IAppLogger
      │   ├─ IMessageBus
      │   ├─ ISaveService → IFileStore
      │   ├─ IAnalyticsService
      │   ├─ IAdsService
      │   ├─ IPurchaseService
      │   └─ ISceneFlowService → ISceneContentLoader
      └─ ApplicationHost
          initialize: MessageBus → Save → Analytics → Ads → Purchases
          shutdown:  Purchases → Ads → Analytics → Save → MessageBus
```

`ApplicationHost.InitializeAsync` es secuencial, comparte la misma tarea ante llamadas concurrentes, acepta `CancellationToken`, permite retry después de fallo recuperable y hace cleanup inverso. `Shutdown`/`Dispose` son idempotentes; si llegan durante inicialización solicitan cancelación del host, impiden volver a `Ready` y limpian también el servicio que estaba inicializando. `AppContext` no es estático y no ofrece lookup; Bootstrap lo retiene para inyección explícita en futuras fachadas.

## Persistencia local

```text
Domain.PlayerProgress
          ↓
Application.ISaveService / AutosaveCoordinator / IFileStore
          ↓
Infrastructure.LocalSaveService
  ├─ UnityJsonSaveSerializer → envelope/DTO v1 + SHA-256
  ├─ ISaveMigration[] → pasos n→n+1
  └─ LocalFileStore → persistentDataPath/Save
```

Application y features no conocen JSON ni paths. Infrastructure referencia Domain directamente porque implementa firmas públicas que mapean `PlayerProgress`; la dirección sigue hacia adentro y el grafo permanece acíclico. Bootstrap es el único lugar que resuelve `Application.persistentDataPath`. Presentation solo consume `SaveUserNotice` para copy recuperable. Contrato, archivos, downgrade y recovery: [`10_SAVE_SYSTEM.md`](10_SAVE_SYSTEM.md).

## Scene flow y contenido local

```text
Presentation.SceneTransitionView
             ↓ ISceneFlowService
Application.SceneFlowService ── estado / exclusión / retry / timeout
             ↓ ISceneContentLoader
Infrastructure.AddressableSceneContentLoader ── owner único de handles
             ↓
Addressables local: SharedLocal(Camp) / JungleLocal(Jungle)

Bootstrap.unity persiste; Camp/Jungle son aditivas.
```

La máquina permite `Boot→Camp`, `Camp→Expedition` y `Expedition→Camp`. Un segundo intento recibe `Busy`; error/cancel/timeout conserva el origen y un target de retry. Cancelar no abandona la operación Unity: el adapter espera un punto seguro y descarga cualquier escena resultante. Cada handle se consume una vez; volver a Camp conserva solo Camp y shutdown deja cero. Presentation nunca conoce Addressables y Bootstrap solo cablea eventos/puertos.

Addressables `4.0.1` queda local-only: perfiles `LocalDevelopment`/`LocalRelease`, grupos `SharedLocal`/`JungleLocal`, labels `scene`/`shared-local`/`world-jungle`, actualización y remote catalog deshabilitados. No hay endpoint. El validador bloquea paths no locales, labels/addresses incorrectos y dependencias Shared→Jungle. Contrato completo: [`CONTENT_PIPELINE.md`](CONTENT_PIPELINE.md).

El bus en memoria solo cubre fan-out acotado. `Subscribe<T>` devuelve `IDisposable`; dispose y shutdown eliminan listeners. No sustituye llamadas directas, no es global y no convierte mensajes en analytics.

## Perfiles de servicios

| Puerto | Development | Release |
|---|---|---|
| Analytics | `NullAnalyticsService` | `NullAnalyticsService` |
| Ads | `MockAdsService` local | `NoAdsService` |
| Purchases | `MockPurchaseService` local | `UnavailablePurchaseService` |
| Save | Local schema v1 | Local schema v1; herramientas Editor excluidas |
| Clock/random | `SystemClock` + seed inyectado | Igual, sin estado remoto |
| Logs/messages | Structured Unity + bus local | Igual, sin datos infantiles |

Editor o `PE_DEVELOPMENT_SERVICES` habilitan los mocks. El define se pasa únicamente en `BuildPlayerOptions.extraScriptingDefines` del APK Development y no se persiste en PlayerSettings. Fuera de esos símbolos, las clases mock y la rama de composición Development no compilan. El diagnóstico de producto/versión se oculta en Release; la vista mínima de estado/error recuperable permanece.

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

- Unity `6000.3.22f1`, Addressables `4.0.1`, URP `17.3.0`, Input System `1.20.0`, Test Framework `1.6.0`, uGUI `2.0.0`.
- Bootstrap es la única escena habilitada en Build Settings; Camp/Jungle son locales Addressable. Development muestra navegación/fallo simulado; Release oculta controles Development.
- Android sigue min API 26, target/compile 36, IL2CPP y ARM64; sin manifest/Gradle custom ni permiso sensible nuevo.
- No existen gameplay, UI final, contenido remoto ni SDKs comerciales. Existe save local v1 sin PII/cuentas; ads/IAP/analytics son únicamente Null/Mock/Unavailable locales sin red.
