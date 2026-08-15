# Matriz de versiones y toolchain

Fecha de corte: 2026-08-14. `Verificado local` describe este equipo; `Verificado oficial` describe documentación vigente; `Pendiente` exige evidencia posterior. No se instaló ni actualizó software.

## Baseline candidata

| Componente | Versión/objetivo | Estado | Fuente o evidencia | Conclusión |
|---|---|---|---|---|
| Unity Hub | 3.20.1 | Verificado local | `Info.plist`, 2026-08-14 | Disponible; no es parte del pin reproducible del proyecto. |
| Unity Editor | 6000.3.22f1 (`1c726e1fb402`), Apple silicon | Verificado local/oficial; pin aceptado | Editor batch + Release API oficial, 2026-08-14 | ADR-0001 cerrada tras import, tests, build y ejecución Android. |
| Rama | Unity 6.3 LTS | Verificado oficial | [anuncio Unity 6.3 LTS](https://unity.com/blog/unity-6-3-lts-is-now-available), 2026-08-14 | Dos años de soporte publicados por Unity; preferible a stream no LTS. |
| Android min API | API 26 (Android 8), provisional | Verificado en proyecto/APK | `ProjectSettings` + `aapt2`, 2026-08-14 | Decisión técnica provisional solicitada; validar mercado/dispositivos en F38. |
| Android target/compile | API 36 | Verificado oficial y en APK | `aapt2`; [Play exige API 36 desde 2026-08-31](https://support.google.com/googleplay/android-developer/answer/11926878?hl=es-419), 2026-08-14 | APK smoke reporta target/compile 36. |
| SDK Build Tools | 36.0.0 | Verificado local y oficial | Archivos del módulo + [dependencias Unity 6.3](https://docs.unity3d.com/6000.3/Documentation/Manual/android-supported-dependency-versions.html), 2026-08-14 | Usar el SDK incluido por Unity. |
| SDK Command-line Tools | 16.0 | Verificado local y oficial | `source.properties` + manual anterior, 2026-08-14 | Usar módulo de Unity. |
| SDK Platform Tools | 36.0.0 | Verificado local y oficial | `source.properties` + manual anterior, 2026-08-14 | `adb` incluido; también existe uno global, no seleccionado. |
| Plataformas SDK incluidas | 34, 35, 36, 37.0 | Verificado local | Inventario del módulo, 2026-08-14 | API 36 está disponible sin descarga. |
| NDK | r27c (`27.2.12479018`) | Verificado local y oficial | `NDK/source.properties` + manual anterior, 2026-08-14 | Versión soportada por Unity; validar cada `.so` para 16 KB. |
| OpenJDK | Temurin 17.0.18+8 | Verificado local; major oficial | Binario incluido + manual anterior, 2026-08-14 | Usar JDK de Unity, no el Homebrew global. |
| Gradle | 9.1.0 | Verificado local y oficial | JAR incluido + [compatibilidad Gradle](https://docs.unity3d.com/6000.3/Documentation/Manual/android-gradle-version-compatibility.html), 2026-08-14 | No personalizar wrapper en MVP. |
| Android Gradle Plugin | 9.0.0 | Verificado oficial | Manual anterior para `6000.3.17f1+`, 2026-08-14 | Compatible con Gradle 9.1.0; verificar Gradle exportado. |
| CMake | 3.22.1 | Verificado local y oficial | Módulo bundled + manual de dependencias Unity 6.3, 2026-08-14 | Usado por AndroidPlayer; no personalizar. |
| Formato Play | AAB + Play App Signing | Verificado oficial; pendiente cuenta | [Android App Bundle](https://developer.android.com/guide/app-bundle), 2026-08-14 | Release no será APK; claves/cuenta son pendientes humanos. |
| ABI/Scripting backend | ARM64 + IL2CPP | Verificado en APK/emulador | Build log, `aapt2`, ELF y emulador 16 KB, 2026-08-14 | Smoke Development pasó; repetir sobre AAB Release en F12/F47. |
| iOS Build Support Unity | No instalado | No disponible localmente | Inventario `PlaybackEngines`, 2026-08-14 | Instalar solo al iniciar trabajo iOS autorizado. |
| Xcode | No disponible; solo Command Line Tools activos | No disponible localmente | `xcodebuild -version`, 2026-08-14 | Bloquea build iOS, no la arquitectura iOS-ready. |
| Git | 2.50.1 (Apple Git-155) | Verificado local | `git --version`, 2026-08-14 | Suficiente para baseline. |
| Git LFS | 3.7.1 | Verificado local | `git lfs version`, 2026-08-14 | Reglas preparadas; no hay objetos LFS aún. |

## Evidencia de cierre ADR-0001 en Fase 03

1. En Fase 03, la revisión sigue siendo el último parche de 6.3 LTS, o se documenta el cambio.
2. Editor activado abre un proyecto limpio sin migración.
3. Android module usa el toolchain incluido y resuelve target API 36.
4. Smoke APK Development ARM64/IL2CPP terminó; el AAB Release corresponde a F12.
5. `zipalign -P 16`, ELF `LOAD align 0x4000` y ejecución en emulador page-size 16384 pasaron.
6. No se introdujeron rutas personales, SDKs globales ni paquetes no aprobados.

Resultado: ADR-0001 **Aceptada** el 2026-08-14. Una actualización de parche exige repetir import, tests y Android smoke; no se migra silenciosamente.

## Revalidación

- F03: Editor latest-patch, licencia, módulos, target API y smoke build.
- F12: Gradle/AGP, AAB, firma de desarrollo y pipeline Android.
- F38: matriz de dispositivos y `minSdk` comercial.
- F47: 16 KB sobre artefacto release y dispositivo/emulador compatible.
- F48: toolchain iOS/Xcode y privacy manifest.
- F52 y cada release: todos los requisitos temporales de tienda.
