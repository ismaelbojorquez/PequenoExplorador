# Preflight — Prompt 16

Fecha: 2026-08-16 (`America/Mexico_City`). Resultado inicial: `PASS`; no había cambios staged/unstaged ni colisión ajena.

## Estado observado antes de editar

| Comprobación | Evidencia | Resultado |
|---|---|---|
| Directorio/inventario | Raíz contenía Git, proyecto Unity, `AGENTS.md`, `.agent/`, `Assets/`, `Packages/`, `ProjectSettings/`, `docs/` y scripts esperados. | `PASS`; no era carpeta vacía, coherente con Prompt 15. |
| Contrato/lectura | Se leyeron completos `AGENTS.md`, `STATUS`, planes, GDD, UI/input, mundo, arte, accesibilidad, arquitectura, decisiones, riesgos, testing y playbook. | `PASS`; P-006 seguía candidato, por lo que Prompt 16 se trata como prototipo reversible. |
| Git | `git status --short --branch`, `git branch --show-current`, `git log -1 --format=fuller` y diffs staged/unstaged. | `main`, limpio, `ahead 7`; HEAD `df7c64fcb378061f0bc0dac1770c7c8f33356cfb`. |
| Implementación | Se inspeccionaron todos los asmdefs, Input/Application/Infrastructure, Bootstrap, scene flow, World, scenes, tests y build validators relacionados. | No existían controller, locomotion, NavMesh, prefab de explorador ni gameplay. Jungle era stub Addressable con spawn. |
| Baseline real | `scripts/validate`. | Código `0` en `1:18.90`: repository/shell, compile, Addressables, EditMode `89/89`, PlayMode `11/11` y APK Development. |
| Hardware | `adb devices`. | `NOT RUN` para touch/FPS físico: lista vacía; no se conectó dispositivo Android. |

## Intake oficial AI Navigation

| Requisito | Fuente oficial | Verificada | Conclusión/impacto |
|---|---|---:|---|
| Paquete compatible | [Unity 6 Manual — AI Navigation](https://docs.unity3d.com/6000.0/Documentation/Manual/com.unity.ai.navigation.html) | 2026-08-16 | `com.unity.ai.navigation@2.0.9` figura `released` para Unity `6000.0`; se fija esa revisión exacta, sin `latest`/preview. |
| Estado released | [Unity Manual — Released packages](https://docs.unity3d.com/6000.0/Documentation/Manual/pack-safe.html) | 2026-08-16 | Unity lista AI Navigation como paquete released soportado/testeado para esa familia de Editor. |
| Funcionalidad | [AI Navigation 2.0 manual](https://docs.unity3d.com/Packages/com.unity.ai.navigation@2.0/manual/index.html) | 2026-08-16 | Provee componentes de NavMesh edit/runtime, agents, obstáculos y links; esta fase usa solo surface/agent local. |
| Licencia | [AI Navigation license](https://docs.unity3d.com/Packages/com.unity.ai.navigation@2.0/license/LICENSE.html) | 2026-08-16 | Unity Companion License para proyectos dependientes de Unity; no selecciona licencia del producto. |
| Metadata local UPM | `package.json` resuelto por el Editor fijado y `packages-lock.json`. | 2026-08-16 | `2.0.9`, requiere Unity `6000.0` y módulo AI builtin; cero `.aar`/`.so`, endpoint, permiso o SDK comercial. Import/compile posterior `PASS`. |

La rama documental `@2.0` ya muestra metadata `2.0.14`, pero la matriz explícita del manual Unity 6.0 identifica `2.0.9`; no se adoptó la revisión más nueva silenciosamente. Actualizar exige repetir intake, import, tests, Android, 16 KB y diff.

## Desviaciones y límites

- El prompt actual autoriza implementar tap-to-move, pero P-006 exige comparación infantil antes de declararlo definitivo. Se implementa como candidato `PH_`, parametrizable y reemplazable; no se cierra la decisión de producto.
- El primer compile tras cambiar asmdefs falló `ARCH004` porque la allowlist no incluía AI Navigation; se corrigió el contrato y la repetición pasó.
- El primer bake embebió `NavMeshData` y volvió binaria la escena. Se externalizó a un asset nativo con atributo Git explícito y Jungle volvió a YAML. No se ocultó el fallo.
