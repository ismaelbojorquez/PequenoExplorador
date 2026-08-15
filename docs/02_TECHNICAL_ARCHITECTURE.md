# Arquitectura técnica — fronteras modulares

Estado: fronteras implementadas en F04 y composition root/lifecycle completado en F06 el 2026-08-15. No define APIs de gameplay. La escena `Bootstrap` muestra estado técnico `Ready` y conserva el placeholder temporal.

## Grafo real de assemblies

```text
PequenoExplorador.Domain
└─ sin referencias; noEngineReferences=true

PequenoExplorador.Application
└─ Domain; noEngineReferences=true

PequenoExplorador.Content
└─ Application

PequenoExplorador.Infrastructure
└─ Application

PequenoExplorador.Presentation
└─ Application

PequenoExplorador.Bootstrap
├─ Application
├─ Content
├─ Infrastructure
└─ Presentation

PequenoExplorador.Editor [Editor only]
├─ Bootstrap
└─ Unity.RenderPipelines.Universal.Runtime

PequenoExplorador.Tests.EditMode [Editor only]
├─ Application
├─ Bootstrap
├─ Editor
├─ Infrastructure
└─ Presentation

PequenoExplorador.Tests.PlayMode
├─ Application
└─ Bootstrap
```

Son nueve assemblies de proyecto. Las suites reciben NUnit/TestRunner mediante `optionalUnityReferences: TestAssemblies`; no usan `overrideReferences`. Las dependencias implícitas de Unity solo están disponibles donde `noEngineReferences=false`.

El player Android inspeccionado contiene únicamente los seis assemblies runtime en `ManagedStripped`; `PequenoExplorador.Editor` y ambas suites no entran al APK.

## Responsabilidades y límites

| Assembly | Responsabilidad autorizada | Referencias prohibidas clave |
|---|---|---|
| Domain | Reglas y estado C# puro cuando existan casos reales. | `UnityEngine`, plataforma, filesystem, UI, SDKs. |
| Application | Lifecycle, contexto inmutable y puertos sobre Domain/BCL. | Unity, concretos de Infrastructure/Presentation/Content. |
| Content | Authoring y mapeo de contenido aprobado. | Infrastructure y Presentation; estado mutable de sesión. |
| Infrastructure | Reloj/random/logger/bus y adapters Null/Mock/seguros; SDK/save solo tras fase y ADR. | Presentation, Content y Bootstrap. |
| Presentation | Vista de estado Bootstrap y futuros adaptadores Unity de UI/input/cámara/audio. | Infrastructure, filesystem, ads, IAP y concretos de plataforma. |
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
      │   ├─ IAnalyticsService
      │   ├─ IAdsService
      │   └─ IPurchaseService
      └─ ApplicationHost
          initialize: MessageBus → Analytics → Ads → Purchases
          shutdown:  Purchases → Ads → Analytics → MessageBus
```

`ApplicationHost.InitializeAsync` es secuencial, comparte la misma tarea ante llamadas concurrentes, acepta `CancellationToken`, permite retry después de fallo recuperable y hace cleanup inverso. `Shutdown`/`Dispose` son idempotentes. `AppContext` no es estático y no ofrece lookup; Bootstrap lo retiene para inyección explícita en futuras fachadas.

El bus en memoria solo cubre fan-out acotado. `Subscribe<T>` devuelve `IDisposable`; dispose y shutdown eliminan listeners. No sustituye llamadas directas, no es global y no convierte mensajes en analytics.

## Perfiles de servicios

| Puerto | Development | Release |
|---|---|---|
| Analytics | `NullAnalyticsService` | `NullAnalyticsService` |
| Ads | `MockAdsService` local | `NoAdsService` |
| Purchases | `MockPurchaseService` local | `UnavailablePurchaseService` |
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

Solo Bootstrap conoce simultáneamente Presentation, Content e Infrastructure. La composición futura seguirá explícita, por constructor/factory, sin `Find*`, service locator ni `Instance` global. ScriptableObjects seguirán siendo authoring; save usará DTOs versionados en Infrastructure; todo SDK real requiere reemplazar un adapter mediante ADR, no filtrar su tipo hacia gameplay.

Subdividir un assembly requiere evidencia de tiempos de compilación, ownership, plataforma o aislamiento de dependencias. Añadir una feature no basta. Todo cambio de grafo actualiza la allowlist, este documento, decisión/riesgo aplicables y las suites.

## Foundation móvil conservada

- Unity `6000.3.22f1`, URP `17.3.0`, Input System `1.20.0`, Test Framework `1.6.0`, uGUI `2.0.0`.
- Bootstrap es la única escena habilitada; Development muestra nombre/versión y estado `Ready`, Release solo estado seguro.
- Android sigue min API 26, target/compile 36, IL2CPP y ARM64; sin manifest/Gradle custom ni permiso sensible nuevo.
- No existen gameplay, scene flow, save, UI de producto, Addressables ni SDKs. Ads/IAP/analytics son únicamente Null/Mock/Unavailable locales sin red/cuentas.
