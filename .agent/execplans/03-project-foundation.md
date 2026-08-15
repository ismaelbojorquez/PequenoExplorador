# ExecPlan — Foundation Unity 6 URP reproducible para Android

- Fase/Gate: 03 / Gate A — Foundation ready
- Estado: Complete
- Creado/actualizado: 2026-08-14 21:40 `America/Mexico_City`
- Owner: Unity Build Engineer / Mobile Game Architect

## Propósito y alcance

Crear en la raíz del repositorio un proyecto mínimo de Unity `6000.3.22f1`, URP, landscape, Android-first e iOS-ready. Debe importar y compilar sin errores, abrir una escena temporal `Bootstrap`, fijar paquetes oficiales exactos y ofrecer un método CLI que devuelva código de salida correcto para builds Android. Incluye configuración de Player/URP, estructura `_Game`, tests EditMode y documentación reproducible. Excluye gameplay, save, Addressables, Localization, IAP, ads, backend, arte final e instalación silenciosa de módulos.

La aceptación local exige evidencia de import/compile y tests. El APK/AAB y la inspección del manifest generado solo pueden ejecutarse si Android Build Support, SDK, NDK y JDK de Unity están presentes; de lo contrario se registran `NOT RUN` con el bloqueo exacto.

## Contexto y orientación

El preflight verificó `main` limpio en `c6fb92de685467b7c7954555e4bf9519ea388f0b`, sin `Assets/`, `Packages/` ni `ProjectSettings/`. Las fuentes canónicas son `AGENTS.md`, `docs/STATUS.md`, `docs/VERSION_MATRIX.md`, `docs/DECISIONS.md`, `docs/ENGINEERING_STANDARDS.md`, `docs/VALIDATION_PLAYBOOK.md` y este plan.

Unity Hub `3.20.1` y Unity Editor `6000.3.22f1 (1c726e1fb402)` están instalados en la ubicación estándar de macOS. Una prueba desechable fuera del repositorio confirmó licencia Personal activa, creación/importación headless y salida `0`. La inspección inicial miró dentro de `Unity.app/Contents/PlaybackEngines`; el build reveló la ubicación correcta de Hub, `6000.3.22f1/PlaybackEngines/AndroidPlayer`, como hermano del bundle. Android Build Support y su toolchain bundled sí están disponibles. No hay módulo iOS.

La Release API oficial de Unity devolvió `6000.3.22f1`, publicada el 2026-08-13, como la revisión más reciente de la línea 6.3 LTS observada el 2026-08-14. Google Play exige target Android 16 / API 36 para apps nuevas y actualizaciones desde 2026-08-31. La baseline usa target 36 y mínimo 26 provisional.

## Progreso

- [x] 2026-08-14 20:53 CST — preflight, inventario, Git, herramientas y fuentes oficiales contrastados; evidencia en `docs/PHASE_03_PREFLIGHT.md`.
- [x] 2026-08-14 21:02 CST — archivos de proyecto, paquetes, escena, URP y estructura mínima creados por el Editor exacto.
- [x] 2026-08-14 21:03 CST — import/compile headless código `0`; EditMode `2/2 Passed`.
- [x] 2026-08-14 21:10 CST — APK Development IL2CPP/ARM64 generado con código `0` fuera de Git.
- [x] 2026-08-14 21:28 CST — APK final instalado y ejecutado en emulador 16 KB; diagnóstico landscape visible y sin crash del package.
- [x] 2026-08-14 21:40 CST — fuentes de verdad y validaciones de documentos/JSON/Git/LFS/metas/16 KB completadas; plan cerrado para revisión y commit único.

## Hallazgos

- El Editor fijado sí está instalado y licenciado para batch; el probe temporal terminó con código `0`.
- La inspección inicial del módulo usó una subruta incompleta dentro del `.app`; el build demostró que `AndroidPlayer` sí estaba instalado como directorio hermano. El diagnóstico se corrigió antes del cierre.
- La documentación de Fase 00 reflejaba una instalación anterior distinta; la matriz debe separar pin de proyecto de disponibilidad local actual.
- Unity Hub no tiene templates locales enumerables. Se usará el proyecto vacío creado por el Editor exacto y se configurará URP oficial de forma determinista, sin assets demo.
- La primera ejecución de setup terminó con código `1`: URP `17.3.0` expone como solo lectura pública tres propiedades de luces/sombras. El log `/tmp/pequeno-phase03-setup.log` mostró `CS0200`; se corrigió mediante `SerializedObject` sobre los campos serializados del asset y debe revalidarse.
- El primer launch en el emulador mostró un ANR de System UI y el aviso de immersive mode. Tras descartarlos, el package obtuvo foco, terminó la carga y mostró el diagnóstico; logcat no registró `FATAL EXCEPTION` ni ANR del juego.

## Decisiones

- 2026-08-14 — conservar `6000.3.22f1`: es el pin ADR y la última revisión 6.3 LTS verificada; no hay justificación para cambiarlo.
- 2026-08-14 — fijar solo URP, Input System, Test Framework y uGUI oficiales; diferir Addressables y Localization hasta que exista una necesidad del slice.
- 2026-08-14 — usar `com.placeholder.pequenoexplorador` como identificador técnico estable y explícitamente no publicable; requiere decisión humana antes de Play Console/App Store Connect.
- 2026-08-14 — no instalar módulos: Android ya estaba disponible en la ubicación correcta; iOS continúa ausente y no es necesario para el smoke Android.

## Plan de implementación

1. Crear el proyecto en la raíz con el Editor exacto y reemplazar el manifest generado por dependencias mínimas fijadas.
2. Añadir scripts mínimos de runtime/editor/test, generar URP móvil y escena `Bootstrap`, y aplicar PlayerSettings Android/iOS.
3. Importar dos veces si es necesario hasta estabilizar `packages-lock.json`, compilar en batch y ejecutar EditMode.
4. Comprobar Android Build Support; ejecutar smoke fuera de Git solo si existe, registrando tamaño y SHA-256.
5. Validar Markdown, enlaces, paquetes, basura, permisos/configuración estática, ausencia de gameplay y estado Git.
6. Actualizar documentación y plan, revisar todo el diff, crear el commit solicitado y verificar árbol limpio.

## Comandos y validación

- `git status --short --branch` — estado inicial `## main`, sin cambios.
- `git branch --show-current` — `main`.
- `git log -1 --format=fuller` — commit base `c6fb92de685467b7c7954555e4bf9519ea388f0b`.
- `Unity -version` — `6000.3.22f1`.
- `find <editor>/PlaybackEngines -maxdepth 1 -mindepth 1 -type d` — `AndroidPlayer` verificado; la consulta inicial dentro de `Unity.app/Contents` quedó invalidada.
- `Unity -batchmode -nographics -quit -createProject <directorio-temporal> -logFile -` — `PASS`, código `0`, licencia e import headless verificadas fuera del repo.
- `Unity -batchmode -nographics -quit -projectPath <repo> -executeMethod PequenoExplorador.Editor.ProjectFoundationSetup.Apply` — primer intento `FAIL`, código `1`, tres `CS0200`; repetición corregida `PASS`, código `0`, marcador `PE_FOUNDATION_SETUP_OK`.
- Import/compile sin método — `PASS`, código `0`, sin errores C#.
- EditMode — `PASS`, `2/2`, XML `/tmp/pequeno-phase03-editmode.xml`.
- Smoke Android Development final — `PASS`, código `0`, `/tmp/pequeno-explorador-builds/PequenoExplorador-smoke.apk`, 57,042,975 bytes, SHA-256 `43b06b4c7adf3014b2e63fbb57e942e6d2c7f5f03659b4d09c7a2f96c46a8b8c`.
- Manifest `aapt2` — `PASS` para min/target/compile 26/36/36, ARM64 y ausencia de cámara/micrófono/ubicación/contactos/`AD_ID`; Development contiene `INTERNET` y un permiso AndroidX interno.
- 16 KB — `PASS`: zipalign, siete ELF con `LOAD 0x4000`, emulador page size `16384` y runtime visible.
- Orientación — `PASS`: `AutoRotation` solo landscape y manifest `screenOrientation=11` (`userLandscape`), diagnóstico visible tras rebuild.
- Guard de build — `PASS`: path dentro del worktree devolvió código `2`, registró error accionable y no creó artefacto/directorio.
- Markdown/enlaces/JSON/diff/LFS/attributes/ignore/metas — `PASS`; `markdownlint` dedicado `NOT RUN` porque no está instalado, sustituido por comprobación local de links y `git diff --check`.
- Los comandos del proyecto y sus resultados se añadirán al ejecutarse; nunca se convertirá `NOT RUN` en `PASS`.

## Recovery y seguridad

No se borrarán cambios ajenos ni se instalarán módulos. `Library/`, `Temp/`, `Logs/` y builds quedan ignorados y recuperables mediante reimportación. Si Unity falla, conservar log y `packages-lock.json`, detener procesos antes de editar, y reanudar desde el último hito. Si el proyecto queda abierto por otra instancia, no forzar ni eliminar locks sin comprobar el proceso. El build temporal, si procede, será una ruta explícita bajo `/tmp/pequeno-explorador-builds/`.

## Resultados y retrospectiva

Foundation completa: Unity/URP/pins/lock, Bootstrap temporal, estructura/asmdefs mínimos, setup y build CLI, configuración Android/iOS-ready, tests y documentación. Import y EditMode pasan; el APK Development API36 IL2CPP/ARM64 pasa build, manifest sensible, 16 KB y runtime en emulador. No se creó gameplay ni se instaló software durante la fase.

No ejecutado: AAB Release/firma/bundletool, build iOS y dispositivo físico. Quedan para F11/F38/F47/F48. Antes de Release deben sustituirse bundle/company placeholder, confirmar manifest Release y retirar `PH_UI_DIAGNOSTIC`.
