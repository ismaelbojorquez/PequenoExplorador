# Preflight — Prompt 15 framework de mundos

Fecha: 2026-08-16 13:38 (`America/Mexico_City`). Alcance observado antes de editar.

## Estado inicial verificado

- Git: rama `main`, árbol limpio, `origin/main [ahead 6]`.
- HEAD: `53e3fd4d3fc104182984dfcac33677b456923373`, `feat(content): add validated data driven content model`.
- Gate A: [`audits/GATE_A_2026-08-15.md`](audits/GATE_A_2026-08-15.md) declara `PASS`; se contrastó con implementación y baseline actual.
- Unity `6000.3.22f1`; Addressables `4.0.1`, sin instalación ni cambio de dependencias.
- Diff staged/unstaged: vacío; no hubo cambios ajenos ni colisión.

## Lectura e inventario

Se leyeron completos `AGENTS`, `STATUS`, índice, ExecPlan contract, world design, content model/pipeline, arquitectura, runtime config, testing/playbook, decisiones, riesgos y auditoría Gate A. Se inspeccionaron escenas Camp/Jungle, Bootstrap, Presentation, todo SceneFlow Application/Infrastructure, fixtures/tests, setup/validator/build Addressables, manifest/lock y los assets reales de grupos/perfiles.

Los grupos reales son `SharedLocal` y `JungleLocal`; los perfiles son `LocalDevelopment`/`LocalRelease`; `scene/camp` y `scene/jungle` usan labels `scene`, `shared-local` y `world-jungle`. Catálogo remoto/update permanecen deshabilitados. La desviación que Prompt 15 debe resolver es real: `SceneContentId` es un enum Camp/Jungle y `LocalSceneAddresses.For` contiene un switch central; no existen `WorldManifest`, `IWorldCatalog`, `IWorldSession` ni disponibilidad independiente.

## Baseline ejecutada

`scripts/validate` terminó código `0` en `1:20.53`: repository checks, compile, Addressables local, EditMode `85/85`, PlayMode `11/11` y APK Development. Addressables: 41 locations, 820,898 bytes, `remoteCatalog=false`. APK: 66,353,374 bytes, SHA-256 `7ce90b3c18aeeea675ce4e0d8d2392e309be6c7947ea5e36b82edd8881c89733`, API 26/36, IL2CPP/ARM64. Esta evidencia describe solo el estado anterior.

## Límites

- Jungle seguirá siendo un stub local; Dinosaurios/Océano/Espacio/Polar/Desierto son únicamente ejemplos documentales, no assets ni contenido.
- No se cambia save schema, paquetes, remote catalogs, permisos, signing o publicación.
- El segundo mundo existe solo como fixture en memoria dentro de tests.
- El trabajo transversal activa [`p15-extensible-worlds.md`](../.agent/execplans/p15-extensible-worlds.md).
