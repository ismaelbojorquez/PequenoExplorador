# Matriz de versiones y toolchain

Fecha de corte: 2026-08-14. `Verificado local` describe este equipo; `Verificado oficial` describe documentación vigente; `Pendiente` exige evidencia posterior. No se instaló ni actualizó software.

## Baseline candidata

| Componente | Versión/objetivo | Estado | Fuente o evidencia | Conclusión |
|---|---|---|---|---|
| Unity Hub | 3.20.1 | Verificado local | `Info.plist`, 2026-08-14 | Disponible; no es parte del pin reproducible del proyecto. |
| Unity Editor | 6000.3.22f1, Apple silicon | Verificado local y oficial | Hub CLI + [release oficial](https://unity.com/releases/editor/whats-new/6000.3.22f1), 2026-08-14 | Candidato ADR-0001; release 2026-08-13 y último resultado 6.3 LTS de API oficial al corte. |
| Rama | Unity 6.3 LTS | Verificado oficial | [anuncio Unity 6.3 LTS](https://unity.com/blog/unity-6-3-lts-is-now-available), 2026-08-14 | Dos años de soporte publicados por Unity; preferible a stream no LTS. |
| Android min API | API 25 (Android 7.1), por validar con mercado | Verificado oficial | [requisitos Unity 6.3 Android](https://docs.unity3d.com/6000.3/Documentation/Manual/android-requirements-and-compatibility.html), 2026-08-14 | Unity soporta API 25+; el `minSdk` comercial se decidirá con matriz de dispositivos. |
| Android target/compile | API 36 | Verificado oficial; pendiente build | Unity soporta API 35/36; [Play exige API 36 desde 2026-08-31](https://support.google.com/googleplay/android-developer/answer/11926878?hl=es-419), 2026-08-14 | Configurar target 36 en Fase 03 para evitar deuda inmediata. |
| SDK Build Tools | 36.0.0 | Verificado local y oficial | Archivos del módulo + [dependencias Unity 6.3](https://docs.unity3d.com/6000.3/Documentation/Manual/android-supported-dependency-versions.html), 2026-08-14 | Usar el SDK incluido por Unity. |
| SDK Command-line Tools | 16.0 | Verificado local y oficial | `source.properties` + manual anterior, 2026-08-14 | Usar módulo de Unity. |
| SDK Platform Tools | 36.0.0 | Verificado local y oficial | `source.properties` + manual anterior, 2026-08-14 | `adb` incluido; también existe uno global, no seleccionado. |
| Plataformas SDK incluidas | 34, 35, 36, 37.0 | Verificado local | Inventario del módulo, 2026-08-14 | API 36 está disponible sin descarga. |
| NDK | r27c (`27.2.12479018`) | Verificado local y oficial | `NDK/source.properties` + manual anterior, 2026-08-14 | Versión soportada por Unity; validar cada `.so` para 16 KB. |
| OpenJDK | Temurin 17.0.18+8 | Verificado local; major oficial | Binario incluido + manual anterior, 2026-08-14 | Usar JDK de Unity, no el Homebrew global. |
| Gradle | 9.1.0 | Verificado local y oficial | JAR incluido + [compatibilidad Gradle](https://docs.unity3d.com/6000.3/Documentation/Manual/android-gradle-version-compatibility.html), 2026-08-14 | No personalizar wrapper en MVP. |
| Android Gradle Plugin | 9.0.0 | Verificado oficial | Manual anterior para `6000.3.17f1+`, 2026-08-14 | Compatible con Gradle 9.1.0; verificar Gradle exportado. |
| CMake | 3.22.1 soportado | Verificado oficial; presencia no auditada | Manual de dependencias Unity 6.3, 2026-08-14 | No es dependencia directa en Fase 00. |
| Formato Play | AAB + Play App Signing | Verificado oficial; pendiente cuenta | [Android App Bundle](https://developer.android.com/guide/app-bundle), 2026-08-14 | Release no será APK; claves/cuenta son pendientes humanos. |
| ABI/Scripting backend | ARM64 + IL2CPP | Decisión técnica; pendiente build | ADR T-009, 2026-08-14 | Probar 16 KB y dispositivos reales antes de Gate E. |
| iOS Build Support Unity | No instalado | No disponible localmente | Inventario `PlaybackEngines`, 2026-08-14 | Instalar solo al iniciar trabajo iOS autorizado. |
| Xcode | No disponible; solo Command Line Tools activos | No disponible localmente | `xcodebuild -version`, 2026-08-14 | Bloquea build iOS, no la arquitectura iOS-ready. |
| Git | 2.50.1 (Apple Git-155) | Verificado local | `git --version`, 2026-08-14 | Suficiente para baseline. |
| Git LFS | 3.7.1 | Verificado local | `git lfs version`, 2026-08-14 | Reglas preparadas; no hay objetos LFS aún. |

## Criterios para cerrar ADR-0001 en Fase 03

1. En Fase 03, la revisión sigue siendo el último parche de 6.3 LTS, o se documenta el cambio.
2. Editor activado abre un proyecto limpio sin migración.
3. Android module usa el toolchain incluido y resuelve target API 36.
4. Smoke build AAB ARM64/IL2CPP termina y se inspecciona; si todavía no hay gameplay, basta player vacío.
5. Dependencias nativas del artefacto soportan 16 KB o el fallo queda registrado como bloqueo real.
6. No se introducen rutas absolutas, SDKs globales ni paquetes no aprobados.

## Revalidación

- F03: Editor latest-patch, licencia, módulos, target API y smoke build.
- F11: Gradle/AGP, AAB, firma de desarrollo y pipeline Android.
- F38: matriz de dispositivos y `minSdk` comercial.
- F47: 16 KB sobre artefacto release y dispositivo/emulador compatible.
- F48: toolchain iOS/Xcode y privacy manifest.
- F52 y cada release: todos los requisitos temporales de tienda.
