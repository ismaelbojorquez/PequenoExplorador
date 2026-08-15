# Android — evidencia histórica F03/F04

La guía operativa canónica desde F05 es [`20_ANDROID_RELEASE.md`](20_ANDROID_RELEASE.md). Este archivo conserva evidencia histórica; no define el comando actual ni habilita Release.

Fecha de evidencia: 2026-08-14. Esta baseline no firma ni publica un release.

## Toolchain fijado

| Elemento | Valor |
|---|---|
| Unity | `6000.3.22f1 (1c726e1fb402)` |
| SDK Build/Platform Tools | `36.0.0` |
| SDK Command-line Tools | `16.0` |
| NDK | `r27c (27.2.12479018)` |
| OpenJDK | Temurin `17.0.18+8` |
| CMake / Gradle | `3.22.1` / `9.1.0` |
| min / target / compile API | `26` provisional / `36` / `36` |
| Backend / ABI | IL2CPP / ARM64 (`arm64-v8a`) |

Unity usa exclusivamente sus rutas bundled. No existe custom Android manifest, Gradle template, keystore ni ruta personal versionada.

## Perfiles CLI

| Perfil | Formato | Opciones | Uso |
|---|---|---|---|
| `Debug` | APK | Development + AllowDebugging | Investigación local; no distribución. |
| `Development` | APK | Development | Smoke reproducible de Fase 03. |
| `Release` | AAB | Sin Development | Objetivo Play; firma/upload aún no configuradas. |

Los tres perfiles reafirman IL2CPP, ARM64 y API 26/36. El método falla con código `2` ante módulo, target, escena, perfil o build inválidos.

```sh
UNITY_EDITOR="/Applications/Unity/Hub/Editor/6000.3.22f1/Unity.app/Contents/MacOS/Unity"

"$UNITY_EDITOR" -batchmode -nographics \
  -projectPath "$(pwd)" \
  -executeMethod PequenoExplorador.Editor.AndroidSmokeBuild.Build \
  -peProfile Development \
  -buildPath /tmp/pequeno-explorador-builds/PequenoExplorador-smoke.apk \
  -logFile /tmp/pequeno-explorador-android.log
```

Para `Release`, usar extensión `.aab` y un path fuera del repo. Ningún comando publica, sube ni acepta términos.

## Evidencia del smoke Development

- Build: `PASS`, código `0`.
- Path temporal: `/tmp/pequeno-explorador-builds/PequenoExplorador-smoke.apk`.
- Tamaño: `57,042,975 bytes`.
- SHA-256: `43b06b4c7adf3014b2e63fbb57e942e6d2c7f5f03659b4d09c7a2f96c46a8b8c`.
- Manifest: package placeholder, versión `0.1.0`, min 26, target/compile 36, debug, ARM64 y `userLandscape` (izquierda/derecha; portrait off).
- Permisos: `INTERNET` por Development/player connection y permiso interno no exportado de AndroidX. No aparecen cámara, micrófono, ubicación, contactos ni `AD_ID`.
- 16 KB: `zipalign -P 16` pasó; todos los segmentos `LOAD` de las siete `.so` ARM64 inspeccionadas tienen alineación `0x4000`; el emulador reportó page size `16384`.
- Runtime: APK instalado en `sdk_gphone16k_arm64` (Android 17/API 37), proceso estable, sin `FATAL EXCEPTION`, ventana landscape enfocada y diagnóstico visible.

Incidencia no ocultada: la primera ejecución del emulador mostró un ANR de **System UI** y el aviso inicial de immersive mode; al descartarlos, el juego obtuvo foco y renderizó. No hubo ANR del package del juego. Debe repetirse en dispositivo físico en fases de QA.

## Manifest y release pendientes

El permiso `INTERNET` del APK Development no prueba el manifest Release. En Fase 12 se generará AAB Release, se verificará si desaparece ese permiso, se inspeccionará el manifest merged, se ejecutará bundletool y se documentará firma de desarrollo. AAB Release, Play App Signing, keystore, upload y publicación están `NOT RUN`.

## Revalidación tras fronteras de Fase 04

- Build Development: `PASS`, código `0`.
- Path temporal: `/tmp/pequeno-explorador-builds/PequenoExplorador-boundaries-smoke.apk`.
- Tamaño: `57,046,302 bytes`.
- SHA-256: `a4572df93cbcda6aaa07369f5edd0a0e77ca51e3ed1f6dc50fef463b52a4903b`.
- Configuración inspeccionada: min 26, target/compile 36, IL2CPP, `arm64-v8a`; `zipalign -P 16` PASS.
- Permisos: solo `INTERNET` de Development/player connection y el permiso interno no exportado; sin cámara, micrófono, ubicación, contactos ni `AD_ID`.
- Runtime: instalado y lanzado en `sdk_gphone16k_arm64`, page size `16384`, proceso activo, sin fatal en logcat y captura landscape con el mismo diagnóstico temporal/sin gameplay.

Este segundo APK demuestra que separar assemblies no rompió el player. No reemplaza el AAB Release ni las pruebas físicas pendientes.
