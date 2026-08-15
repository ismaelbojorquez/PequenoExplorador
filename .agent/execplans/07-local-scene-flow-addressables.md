# ExecPlan — flujo aditivo y Addressables locales

- Fase/Gate: Fase 07, Gate B
- Estado: Complete
- Creado/actualizado: 2026-08-15 02:14 CST (`America/Mexico_City`)
- Owner: Unity Runtime Architect

## Propósito y alcance

Entregar navegación placeholder offline `Boot → Camp ↔ Expedition/Jungle`, con una sola transición a la vez, progreso/error recuperable, cancelación/timeout y ownership demostrable de cada escena Addressable. Incluye paquete oficial fijado, perfiles/grupos/labels locales, setup y validadores Editor, tests EditMode/PlayMode, build de contenido local y smoke Android. Excluye gameplay, contenido final, safe area completa, remote catalogs, CDN y descargas.

La aceptación requiere escenas correctas durante tres ciclos, cero handle vivo tras volver a Camp/shutdown, catálogo local incluido, Release sin controles Development, suites y smoke Android ejecutados. Un comando no ejecutado se registra `NOT RUN`, nunca `PASS`.

## Contexto y orientación

Gate A consta `PASS` en `docs/STATUS.md` y `docs/audits/GATE_A_2026-08-15.md`. HEAD de entrada es `31a6835ad2558e2cf7afc72a90f6ca21e8fba4c4`, rama `main`, árbol limpio. Unity está fijado en `6000.3.22f1`. La baseline previa pasó compile, EditMode `21/21` y PlayMode `2/2`.

Application permanece BCL-only y define navegación/puertos. Infrastructure adapta Addressables. Presentation renderiza transición. Bootstrap compone y conserva los servicios. No se crean asmdefs adicionales: se mantienen los nueve existentes y se actualiza su allowlist solo con referencias oficiales necesarias.

Fuentes oficiales consultadas el 2026-08-15: Unity Registry `https://packages.unity.com/com.unity.addressables`, tarball oficial `com.unity.addressables-4.0.1.tgz` SHA-1 `37a0b4bd16b0a191e1e08e9b62908ca4284b0f76`, documentación `https://docs.unity3d.com/Packages/com.unity.addressables@4.0/manual/index.html` y manual del Editor. El registro fija `4.0.1`, Unity `6000.0+`, Unity Companion License y Scriptable Build Pipeline `4.0.0`; no se observaron binarios nativos.

## Progreso

- [x] 2026-08-15 01:07 CST — preflight, Gate A, Git, documentos, implementación, paquetes y baseline contrastados.
- [x] 2026-08-15 01:07 CST — intake oficial de Addressables `4.0.1` cerrado para prueba local.
- [x] 2026-08-15 01:11 CST — pin/lock importado; compile confirmó assemblies/APIs `4.0.1`.
- [x] 2026-08-15 01:24 CST — contratos, máquina, adapter, composición, escenas y settings locales implementados.
- [x] 2026-08-15 01:37 CST — validadores/build local y suites de lifecycle/repetición implementados.
- [x] 2026-08-15 01:45 CST — APK actual inspeccionado y arrancado offline en emulador 16 KB hasta Camp.
- [x] 2026-08-15 02:00 CST — `scripts/validate` final pasó; APK/catálogo/manifiesto y arranque offline inspeccionados.
- [x] 2026-08-15 02:10 CST — documentación, ExecPlan y evidencia cerrados; revisión Git final pendiente solo del commit autorizado.

## Hallazgos

- El manual general de Unity 6.0 indexa `2.7.6`, mientras el Unity Registry oficial vigente publica `4.0.1` estable para Unity `6000.0+`. La selección se basa en registry/tarball exactos y queda condicionada a evidencia local completa.
- El roadmap canónico asigna F07 también al shell/safe area/input; este ExecPlan entrega solo el incremento de scene flow solicitado. `STATUS` no declarará toda F07 completa mientras esos entregables sigan pendientes.
- PlayMode falló inicialmente por tres defectos reales/fixture: status `Ready` observado antes de Camp, consulta de un unload handle auto-liberado y teardown pintando una vista destruida. Tras corregirlos, el runner reveló además coexistencia de roots durante un frame entre tests; el helper exige un root único. Los fallos permanecen registrados y no se contaron como PASS.
- Un APK intermedio se redujo a `41,653,368` bytes tras recompilación IL2CPP completa; el artefacto final figura en comandos/resultados. Ambos contienen settings/catálogo y bundles locales; el arranque sin wifi/datos llegó a `ApplicationReady` y `TransitionCompleted Camp` en page size 16384.
- El primer `scripts/validate` de cierre devolvió código `2`: el tercer ciclo PlayMode superó 600 frames aunque no 20 segundos. Sustituir límites por deadline monotónico eliminó la dependencia de framerate del batch runner; PlayMode aislado y pipeline completo posteriores pasaron.

## Decisiones

- 2026-08-15 — fijar `com.unity.addressables@4.0.1`, no `latest`, preview ni rango; si import/compile/build muestra incompatibilidad, revertir el pin/settings y registrar `FAIL` antes de evaluar una versión anterior.
- 2026-08-15 — Bootstrap permanece como escena persistente en Build Settings; Camp/Jungle se cargan aditivamente por claves constantes y se descargan mediante un único owner de handles.
- 2026-08-15 — perfiles `LocalDevelopment`/`LocalRelease`, grupos `SharedLocal`/`JungleLocal`; remote catalog y endpoints quedan ausentes.

## Plan de implementación

1. Añadir el paquete exacto y dejar que Unity resuelva lock/transitivos oficiales; inspeccionar licencia, asmdefs y APIs importadas.
2. Crear en Application la máquina de estados/puertos/resultados; probar exclusión mutua, fallo, cancelación, timeout y recovery sin Unity.
3. Implementar adapter Addressables con ownership idempotente y cleanup tras cancelación/error; Presentation solo consume `ISceneFlowService`.
4. Extender Bootstrap para componer, iniciar `Boot → Camp`, apagar flujo antes de servicios y exponer controles Development sin incluirlos en Release.
5. Crear mediante setup Editor determinista las escenas placeholder y los settings/grupos/profiles/labels locales; añadir validador de ausencia remota y dependencia `Shared → Jungle`.
6. Integrar build Addressables local al pipeline, PlayMode de tres ciclos y smoke Android; revisar catálogo, handles, escenas, permisos y Git.
7. Actualizar arquitectura, mundo, pipeline de contenido, testing, decisiones, dependencia, riesgos, changelog y status; revisar diff y commit autorizado.

## Comandos y validación

- `scripts/compile` — baseline `PASS`, 13.881 s.
- `scripts/test-editmode` — baseline `PASS`, 21/21, 15.031 s.
- `scripts/test-playmode` — baseline `PASS`, 2/2, 15.274 s.
- `scripts/build-addressables-local` — `PASS`, 24.952 s; 6 locations, 8 archivos/181,040 bytes, remote=false.
- `scripts/test-editmode` — `PASS` de corte, 27/27 antes del último validador test; posteriormente repetido por el pipeline final.
- `scripts/test-playmode` — `PASS`, 4/4 en 15.561 s tras conservar intentos fallidos previos.
- `scripts/build-android-development` — primer build integrado `PASS`, 190.273 s; artefacto intermedio `41,653,368` bytes, SHA-256 `d2667ee64529233c8a933845b75638cc007bbcf3bf1961107bc386d6366c5dfd`.
- `adb install/start` con wifi/datos deshabilitados — `PASS`; foco en juego, Boot→Camp, `PAGE_SIZE=16384`; red restaurada por trap.
- `scripts/check-repository` — `PASS`, Markdown 54, JSON 16, workflow 1, secrets 0, shell syntax OK.
- `scripts/validate` — primer intento `FAIL`, código `2`: EditMode 29/29 y PlayMode 3/4 por timeout de fixture basado en frames.
- `scripts/test-playmode` — repetición tras deadline monotónico `PASS`, 4/4.
- `scripts/validate` — repetición completa `PASS`, código `0`, aproximadamente 3:10; checks, compile, local Addressables, EditMode 29/29, PlayMode 4/4 y Android Development.
- `scripts/validate` — última repetición tras mover el estado generado a `Library`, `PASS`, código `0`, aproximadamente 2:05 con caché; mismos conteos y APK Development.
- APK final — `41,722,038` bytes, SHA-256 `789d8342fd9af78151e10b55c566c7778e752d6bbadc07badbe9f473a6d3c29c`; IL2CPP/ARM64, min/target 26/36.
- APK/arranque offline final — catálogo y bundles `SharedLocal`/`JungleLocal` incluidos; sin permiso sensible/`AD_ID`; emulador `PAGE_SIZE=16384` enfocado en la app y `Boot→Camp` completado con wifi/datos deshabilitados.

## Recovery y seguridad

No editar `Library`, caches ni artefactos. Los settings/escenas generados se versionan y pueden retirarse junto con el pin si el import falla. No configurar URL, remoto, signing, permisos, SDK o publicación. Ante cancelación, esperar/limpiar la operación Unity de forma segura y liberar cada handle una sola vez. Conservar cambios ajenos; si aparece una colisión no aislable, detenerse.

## Resultados y retrospectiva

El incremento cumple el alcance: flujo placeholder offline resiliente, ownership de handles acotado, perfiles/grupos locales, validación estructural, tres ciclos PlayMode y smoke Android. No se observó crecimiento de handles: uno para el mundo activo y cero tras shutdown; Unity informó memoria estable alrededor de 196 MB durante teardown, dato orientativo y no presupuesto formal. Se evitó fragmentar assemblies y no se añadió remoto, gameplay ni SDK comercial. F07 permanece abierta únicamente por sus entregables canónicos de shell/safe area/input.
