# Arquitectura técnica — fronteras modulares

Estado: implementada en Fase 04, 2026-08-14. Define fronteras de compilación, no APIs de gameplay. La escena temporal `Bootstrap` conserva el comportamiento de Fase 03.

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
├─ Bootstrap
└─ Editor

PequenoExplorador.Tests.PlayMode
└─ Bootstrap
```

Son nueve assemblies de proyecto. Las suites reciben NUnit/TestRunner mediante `optionalUnityReferences: TestAssemblies`; no usan `overrideReferences`. Las dependencias implícitas de Unity solo están disponibles donde `noEngineReferences=false`.

El player Android inspeccionado contiene únicamente los seis assemblies runtime en `ManagedStripped`; `PequenoExplorador.Editor` y ambas suites no entran al APK.

## Responsabilidades y límites

| Assembly | Responsabilidad autorizada | Referencias prohibidas clave |
|---|---|---|
| Domain | Reglas y estado C# puro cuando existan casos reales. | `UnityEngine`, plataforma, filesystem, UI, SDKs. |
| Application | Casos de uso y puertos futuros sobre Domain. | Unity, concretos de Infrastructure/Presentation/Content. |
| Content | Authoring y mapeo de contenido aprobado. | Infrastructure y Presentation; estado mutable de sesión. |
| Infrastructure | Adaptadores de save/plataforma/SDK solo tras fase y ADR. | Presentation, Content y Bootstrap. |
| Presentation | Adaptadores Unity de UI/input/cámara/audio futuros. | Infrastructure, filesystem, ads, IAP y concretos de plataforma. |
| Bootstrap | Único composition root; ensambla puertos y concretos explícitamente. | Reglas de producto, service locator o singleton global. |
| Editor | Build/setup/validación que nunca entra en player. | Gameplay y estado runtime. |
| Tests | Evidencia por frontera y escena; fixtures controladas. | Dependencias innecesarias, red/reloj/azar real. |

Los markers `*AssemblyMarker` demuestran enlaces permitidos en compile sin anticipar interfaces, servicios o modelos de producto. `DiagnosticBootstrap` sigue siendo un MonoBehaviour sin reglas y no constituye todavía composición de features.

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

## Composition root y evolución

Solo Bootstrap podrá conocer simultáneamente Presentation, Content e Infrastructure. La composición futura será explícita, por constructor/factory, sin `Find*`, service locator ni `Instance` global. Los eventos serán datos inmutables de Domain y puertos de Application, no un bus estático. ScriptableObjects seguirán siendo authoring; save usará DTOs versionados en Infrastructure; servicios de plataforma y feature flags se inyectarán detrás de puertos con defaults locales seguros.

Subdividir un assembly requiere evidencia de tiempos de compilación, ownership, plataforma o aislamiento de dependencias. Añadir una feature no basta. Todo cambio de grafo actualiza la allowlist, este documento, decisión/riesgo aplicables y las suites.

## Foundation móvil conservada

- Unity `6000.3.22f1`, URP `17.3.0`, Input System `1.20.0`, Test Framework `1.6.0`, uGUI `2.0.0`.
- Bootstrap es la única escena habilitada y muestra nombre, `0.1.0-dev` y aviso temporal.
- Android sigue min API 26, target/compile 36, IL2CPP y ARM64; sin manifest/Gradle custom ni permiso sensible nuevo.
- No existen gameplay, scene flow, save, UI de producto, Addressables, ads, IAP, analytics ni servicios concretos.
