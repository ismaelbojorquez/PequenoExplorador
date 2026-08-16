# ExecPlan — explorador placeholder con tap-to-move y cámara asistida

- Fase/Gate: Prompt 16 / Gate B
- Estado: Complete
- Creado/actualizado: 2026-08-16 14:43 America/Mexico_City
- Owner: Character Controller Engineer / Child Interaction Designer

## Propósito y alcance

Entregar en Selva stub un explorador `PH_` que reciba taps semánticos del Input System, navegue solo por suelo válido, muestre feedback no punitivo, se detenga al cambiar de contexto y sea seguido por una cámara limitada. Incluye paquete oficial AI Navigation, contratos puros de locomoción, adaptadores Unity, authoring de escena, validadores, tests y documentación. Excluye personalización, interacción, combate, joystick, arte/animación finales y decisión definitiva de UX antes de playtest.

## Contexto y orientación

HEAD inicial `df7c64fcb378061f0bc0dac1770c7c8f33356cfb` en `main`, árbol limpio y siete commits ahead de `origin/main`. Unity está fijado en `6000.3.22f1`; Input System `1.20.0`; Jungle es Addressable aditiva y contiene `PH_SPAWN_JUNGLE_ENTRY`. `P-006` conserva tap-to-move como candidato: esta fase implementa un prototipo reversible autorizado, no cierra su validación infantil. Bootstrap es el composition root y Presentation no referencia Infrastructure.

## Progreso

- [x] 2026-08-16 14:17 — preflight Git/documental/implementación y baseline completados sin cambios ajenos; `scripts/validate` PASS 89/89 + 11/11 + APK.
- [x] 2026-08-16 14:17 — AI Navigation oficial verificado: `2.0.9` released para Unity 6000.0; Unity Companion License; sin SDK comercial/endpoints/datos.
- [x] 2026-08-16 14:27 — pin, contratos/adaptadores, prefab/materiales/NavMesh/escena `PH_` y validator implementados.
- [x] 2026-08-16 14:43 — validadores, 94/94 EditMode, 14/14 PlayMode, tres ciclos, Addressables y dos APK Development pasaron.
- [x] 2026-08-16 14:43 — documentos canónicos, evidencia y revisión final Git cerrados; commit listo.

## Hallazgos

- La documentación GDD/DECISIONS aún exige comparar tap-to-move con control directo. La implementación se mantiene parametrizable y placeholder para playtest; no convierte P-006 en decisión final.
- No había paquete `com.unity.ai.navigation`, NavMesh, controlador, prefab ni hardware Android conectado. `adb devices` devolvió lista vacía; prueba táctil/FPS en dispositivo físico será `NOT RUN` salvo cambio de entorno.
- El pipeline previo tarda cerca de 79 s; las iteraciones deben usar compile/tests dirigidos y cerrar con `scripts/validate` completo.
- El primer bake dejó Jungle binaria al embeber NavMeshData. Se externalizó al asset técnico `PH_Jungle_NavMesh.asset`, Jungle volvió a YAML y `.gitattributes` trata solo ese asset como binario.
- El primer PlayMode de la feature encontró que el tap inválido mostraba marker pero conservaba `PathPending`; `RejectDestination` hizo explícito el estado y la repetición pasó 14/14.
- APK creció de `66,473,622` a `80,860,082` bytes (`+21.64%`). Tres builds dieron tamaño idéntico; no hay `.aar/.so` del paquete y los siete ELF existentes mantienen LOAD 16 KB, pero el presupuesto requiere profiling en Prompt 12/Gate C.

## Decisiones

- 2026-08-16 — fijar `com.unity.ai.navigation` `2.0.9`: es la revisión que el manual oficial de Unity 6.0 declara released/compatible. La rama documental 2.0 muestra 2.0.14, pero no se adopta silenciosamente sin una matriz explícita para el Editor fijado.
- 2026-08-16 — usar `NavMeshAgent` sin root motion y una animación procedural inequívocamente temporal: reduce acoplamiento a un rig inexistente y mantiene parámetros/pathfinding testeables.
- 2026-08-16 — el composition root enlaza una única raíz de explorador al cargar la escena; no se introduce singleton/service locator ni búsquedas por frame.

## Plan de implementación

1. Añadir el pin UPM, intake y referencias asmdef mínimas; resolver lock mediante el Editor fijado.
2. Añadir modelo/controlador puro de locomoción y evento de cambio de mapa para suspensión determinista.
3. Añadir root Presentation con raycast/NavMeshAgent, feedback, cámara y bob placeholder; ensamblarlo desde Bootstrap al cargar Selva.
4. Añadir setup Editor idempotente para prefab/materiales/suelo/NavMesh y validador de build accionable.
5. Probar estados/comandos en EditMode; paths, taps, pausa/mapas, unload, reduce motion, repetición y FPS básico en PlayMode.
6. Actualizar documentos canónicos, cerrar plan, revisar diff y commit.

## Comandos y validación

- `scripts/validate` — baseline previa PASS en 1:18.90; EditMode 89/89, PlayMode 11/11, APK Development PASS.
- `adb devices` — ejecutado; cero dispositivos, prueba Android física `NOT RUN` por hardware ausente.
- `scripts/compile` — PASS con `PE_EXPLORER_FOUNDATION_OK`.
- `scripts/test-editmode` — PASS `94/94`, `1.719 s`.
- `scripts/test-playmode` — PASS `14/14`, `4.876 s`; FPS Editor batch diagnóstico `11660.6`, no hardware.
- `scripts/validate` — PASS código `0` en `4:23.73`, incluido Addressables y APK.
- `scripts/build-android-development` — repetición final sobre el estado staged PASS en `68.07 s` wrapper/`49.551 s` Unity; tamaño estable `80,860,082` bytes, hash final `1d04f40d81a34794d18949ccc562cb768ffcd6afdd69a5df2bfb898e909b7e79`.
- `aapt2 dump permissions` / `zipalign -P 16` / `llvm-readobj` — PASS: solo `INTERNET` + permiso interno receiver, APK alineado y siete ELF LOAD `16384`.

## Recovery y seguridad

Todos los cambios son locales y versionados; no hay push, signing ni remote content. Si UPM o import falla, retirar únicamente el pin AI Navigation y cambios de esta fase con parches dirigidos, preservando cambios ajenos. Los assets generados permanecen bajo `Assets/_Game` y las salidas en `artifacts/` ignorado. No modificar `Library` manualmente ni aceptar términos.

## Resultados y retrospectiva

Locomoción candidata `PH_` quedó implementada y validada localmente con pin exacto, fronteras, estados recuperables, cámara, assets, tres ciclos, suites y APK. No se ejecutó touch/FPS en hardware ni playtest infantil; P-006 continúa abierto. El incremento siguiente es Prompt 17, interacción contextual accesible, después del commit limpio de esta fase.
