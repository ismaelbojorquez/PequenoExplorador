# ExecPlan — remediar composición UI y lifecycle Android de Gate B

- Fase/Gate: remediación posterior a Prompt 30 / Gate B
- Estado: Blocked on physical device and human playtest
- Creado/actualizado: 2026-08-17 12:45 CST
- Owner: Principal Unity UI Runtime Architect + Android Lifecycle Engineer

## Propósito y alcance

Hacer que el Vertical Slice sea visible y táctil en Android mediante ownership fail-closed de superficies UI por estado, diagnostics opt-in, safe area/orientación recuperables y regresiones de composición. Incluye suite completa, APK identificado, matriz física no destructiva y preparación de playtest. Excluye contenido, balance, SDKs, publicación, Prompt 31 y cualquier declaración de Gate B: el máximo es `READY FOR INDEPENDENT GATE B RE-AUDIT`.

## Contexto y orientación

Entrada limpia `main@8cdd0bdc51a0b5ffce9227b3324a5a07856e6bda`; auditoría vigente `docs/audits/GATE_B_2026-08-17_PHYSICAL_AND_CHILD_UX.md` = `FAIL`. Unity `6000.3.22f1`; paquetes exactos sin cambios. Bootstrap contiene 13 Canvas persistentes y un EventSystem. No existe `CanvasGroup` ni owner de composición; cada view gobierna solo paneles internos. `DiagnosticBootstrap` enlaza todas las vistas y habilita diagnostics por flag, pero no desactiva roots incompatibles al cambiar Camp/Expedition/feature. En el preflight el HONOR DNY-NX9 Android 16 estaba conectado; se desconectó antes de instalar el candidato. Su save puede contener grants debug; prohibido borrar/restaurar sin autorización.

Baseline 2026-08-17: `scripts/validate` = `PASS`, EditMode 169/169, PlayMode 29/29, APK Development; 134.38 s total. Este PASS automatizado no contradice el FAIL físico.

## Progreso

- [x] 2026-08-17 11:48 CST — preflight, Git, paquetes, docs, evidencia, dispositivo y baseline contrastados.
- [x] 2026-08-17 11:48 CST — inventario inicial: 13 roots `PH_UI_*`, EventSystem único, cero CanvasGroup/owner central.
- [x] 2026-08-17 12:08 CST — matriz tipada, coordinator de 13 superficies, sorting y diagnostics cerrados por defecto.
- [x] 2026-08-17 12:10 CST — recovery coalescido de safe area/surface/cámara en resize/orientación/focus.
- [x] 2026-08-17 12:15 CST — validator integrado; EditMode 172/172 y PlayMode 31/31 acotados.
- [x] 2026-08-17 12:34 CST — suite integral `PASS`; commit técnico `a4238c73a21eeca7d0a2572015a9f7ab93205f11`; APK exacto construido e identificado.
- [x] 2026-08-17 12:40 CST — matriz física intentada sin mutación: `adb devices -l` quedó vacío; instalación y casos físicos registrados `NOT RUN/BLOCKED`.
- [x] 2026-08-17 12:42 CST — playtest registrado `NOT RUN`: no hubo facilitador, consentimiento ni participante; no se simuló evidencia humana.

## Hallazgos

- Hecho: roots persistentes `PH_UI_DIAGNOSTIC`, `SCENE_FLOW`, `AUDIO_DIAGNOSTIC`, `TUTORIAL`, `CAMP_HUB`, `PHOTOGRAPHY`, `INPUT_FOUNDATION`, `ALBUM`, `LEARNING`, `MISSIONS`, `INTERACTION_CANVAS`, `ECONOMY` y `CUSTOMIZATION` coexisten en Bootstrap.
- Hecho: vistas modal/feature suelen ocultar un panel hijo; status, misión y economía permanecen visibles, y diagnostics se habilitan en startup sin acceso deliberado.
- Hecho: `HandleSceneFlowChanged` solo cambia input/tutorial; no aplica visibilidad ni raycasts por estado.
- Hecho: `UnitySafeAreaService.Tick` detecta tamaño/orientación, pero no existe recuperación de Canvas/cámara al recrearse la surface.
- Hallazgo durante regresión: al descargar Bootstrap, `PhotographyView.OnDestroy` podía notificar después de destruir `LearningActivityView`; cinco tests fallaron con `MissingReferenceException`. Guards de shutdown cierran la carrera y PlayMode vuelve a pasar.
- Hallazgo durante setup: Status vive bajo el `Diagnostic Canvas` ya existente; promover su content node a Canvas anidado duplicaba safe-area ownership. El setup restaura ese hierarchy y registra el Canvas owner, sin Canvas anidado.
- Hallazgo de CI: `WaitForEndOfFrame` no se invoca en batchmode. El recovery espera frames ordinarios, verificables en CI y Android, antes de reactivar cámara.
- Límite: un playtest con menores requiere facilitador, consentimiento y evidencia humana; Codex no puede producirlo.
- Bloqueo de cierre: el HONOR DNY-NX9 dejó de estar disponible antes de instalar el candidato. El save observado puede contener grants debug; un journey limpio también exige autorización expresa para resetear/restaurar progreso.

## Decisiones

- 2026-08-17 — introducir `AppUiState` BCL-only en Application y `UiCompositionCoordinator` en Presentation; Bootstrap traduce outcomes existentes. No bus global, singleton ni polling por nombre.
- 2026-08-17 — controlar roots mediante `CanvasGroup` + `GraphicRaycaster` sin destruirlos; el coordinador permanece activo fuera de los roots gestionados y aplica fail-closed.
- 2026-08-17 — diagnostics permanecen compilados solo en Development pero cerrados por defecto; un toggle deliberado puede abrirlos y nunca sustituye producto.
- 2026-08-17 — no tocar save físico hasta autorización explícita; las pruebas destructivas quedan `BLOCKED/NOT RUN` si hace falta estado limpio.
- 2026-08-17 — Application conserva estado/input/Back semánticos; la policy de roots/sorting pertenece a Presentation. Esto mantiene la dirección de dependencias y evita filtrar Canvas a Application.
- 2026-08-17 — cada superficie tiene raycaster exclusivo; primarios usan sorting 100, Interaction 200, Tutorial 300 y diagnostics 400.

## Matriz AppState/UI

| Estado | Primario | Overlays permitidos | Input | Back | Rotación |
|---|---|---|---|---|---|
| Boot | status | ninguno | UI | ignorado | reflow |
| Transition | scene-flow | ninguno | UI | ignorado | reflow |
| Camp | camp | tutorial | UI | pausa | reflow |
| Expedition | mundo 3D | interaction; tutorial | Explorer | pausa | reflow/cámara |
| Interaction | mundo 3D | interaction; tutorial | Explorer | pausa | reflow/cámara |
| LearningActivity | learning | tutorial | UI | cerrar | reflow |
| Photography | photography | tutorial | Photography | cerrar | reflow/cámara |
| DiscoveryResult | photography card | tutorial | Photography | cerrar | reflow |
| Album | album | tutorial | UI | cerrar | reflow |
| Missions | missions | ninguno | UI | cerrar | reflow |
| CampUpgrade | camp preview | tutorial | UI | cerrar | reflow |
| Customization | customization | tutorial | UI | cerrar | reflow |
| Pause | pause | ninguno interactivo debajo | UI | reanudar | reflow |
| ErrorRecovery | status | ninguno | UI | retry/stay | reflow |
| DevelopmentDiagnostics | diagnostics deliberado | nunca producto interactivo simultáneo | UI + acceso deliberado | cerrar | reflow |

Todo root no listado queda alpha 0, `interactable=false`, `blocksRaycasts=false` y sin `GraphicRaycaster` activo. La tabla coincide con la policy implementada y sus tests exhaustivos.

## Plan de implementación

1. Añadir estado/overlay semántico puro y policy exhaustiva testeable.
2. Añadir coordinador Presentation con registros explícitos serializados; nunca buscar por nombre en runtime.
3. Cablear Bootstrap y setup Editor reproducible; aplicar estado en init, SceneFlow, feature open/close, pausa y shutdown.
4. Encapsular diagnostics bajo toggle deliberado y verificar Release fail-closed.
5. Añadir adapter de surface/layout que escucha safe-area, focus y orientación, fuerza reflow y reactiva render de cámara sin reiniciar Activity.
6. Añadir EditMode/PlayMode/validator de matriz, roots, raycast, EventSystem, transición y resize.
7. Actualizar fuentes canónicas, ejecutar pipeline, APK/hash/instalación y matriz física.

## Comandos y validación

- `git status --short --branch && git branch --show-current && git log -1 --format=fuller` — `PASS`, entrada limpia esperada.
- `scripts/validate` — baseline `PASS`; 169/169 EditMode, 29/29 PlayMode, APK Development, 134.38 s.
- `adb devices -l` — `PASS`, HONOR DNY-NX9 Android 16 conectado.
- `scripts/compile` — `PASS` final acotado tras dos fallos intermedios detectados por INPUT008 durante migración de Canvas; el setup fue corregido, no se debilitó el validator.
- `scripts/test-editmode` — `PASS`, 172/172.
- `scripts/test-playmode` — `PASS`, 31/31. Fallos intermedios: teardown destroyed-reference 5 casos y `WaitForEndOfFrame` batchmode 1 caso; ambos corregidos y repetidos.
- `scripts/check-repository && git diff --check` — `PASS`, 110 Markdown, 22 JSON, 1 workflow, 0 secretos.
- `scripts/validate` posterior — `PASS` y repetido tras la revisión de races/Back: repository checks, compile/validators, Addressables local 61 locations/1,920,120 bytes, EditMode 172/172, PlayMode 31/31 y APK Development. El último APK pre-commit mide 67,454,896 bytes y tiene SHA-256 `1276ce68417c5b504da1ea9f8cd5ef43273e219855b0502ae7a699c18bc064df`; solo valida el working tree y se reconstruirá tras el commit técnico para identificar el candidato físico con commit exacto.
- Commit técnico — `PASS`: `a4238c73a21eeca7d0a2572015a9f7ab93205f11` (`fix(ui): enforce app state presentation lifecycle`).
- APK post-commit — `PASS` de build/identidad estática: 67,454,896 bytes, SHA-256 `c3492324b77d91ebc062d5ad01dd14b4296c3a685bc9382d9a80b160a8db8adf`, commit embebido exacto, API 26/36, IL2CPP ARM64. `zipalign -P 16` y LOAD ELF 16384 pasan; sin permisos sensibles.
- `adb devices -l` posterior — `BLOCKED`: lista vacía. Respaldo, instalación, hash extraído, cinco starts, journey, lifecycle, touch, rotación y profiling = `NOT RUN`.
- Journey desde save limpio — `BLOCKED`: además del dispositivo ausente, no existe autorización para borrar/restaurar el progreso posiblemente contaminado.
- Playtest — `NOT RUN`: sin facilitador, consentimiento ni participante.

## Recovery y seguridad

El cambio se limita a archivos de proyecto y artefactos ignorados. No limpiar Git/Library ni datos Android. Antes de instalar se respalda read-only el save y se identifica APK/hash. Si la composición falla, revertir por commit nuevo o corregir la policy/setup; nunca borrar progreso ni reiniciar Activity como solución de rotación. Wi-Fi/datos/orientación del dispositivo deben restaurarse a sus valores observados.

## Resultados y retrospectiva

La porción técnica terminó y quedó aislada en `a4238c73a21eeca7d0a2572015a9f7ab93205f11`: policy/ownership fail-closed, lifecycle recovery, validator y regresiones pasan la suite integral. El APK candidato exacto está retenido con hash `c349232…` y sus controles binarios estáticos pasan.

La eficacia física no se verificó porque el dispositivo se desconectó antes de instalar. Tampoco se ejecutó playtest humano. El resultado del plan es `BLOCKED`, no `READY`: Gate B conserva `FAIL`, Prompt 31 permanece bloqueado y la siguiente acción es matriz física + playtest + reauditoría independiente. El expediente técnico está en [`docs/audits/GATE_B_UI_ANDROID_REMEDIATION_2026-08-17.md`](../../docs/audits/GATE_B_UI_ANDROID_REMEDIATION_2026-08-17.md).
