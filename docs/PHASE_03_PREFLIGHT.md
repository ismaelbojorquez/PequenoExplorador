# Evidencia de preflight — Fase 03

Fecha de observación: 2026-08-14 (`America/Mexico_City`). Este documento registra hechos locales y fuentes temporales; no sustituye asesoría legal ni garantiza aceptación futura de tiendas.

## Repositorio antes de editar

| Comprobación | Resultado observado |
|---|---|
| Directorio | Raíz del repositorio `PequenoExplorador`; no se conserva la ruta personal |
| Inventario | Baseline documental de Fases 00–02; no existían `Assets/`, `Packages/` ni `ProjectSettings/` |
| `AGENTS.md` | Presente y leído completo; no hubo bloqueo por ausencia |
| Documentos leídos | `.agent/PLANS.md`, `README.md`, `docs/STATUS.md`, `ROADMAP.md`, `VERSION_MATRIX.md`, `DECISIONS.md`, `RISK_REGISTER.md`, `POLICY_SOURCE_REGISTER.md`, `ENGINEERING_STANDARDS.md`, `CODE_REVIEW_RULES.md`, `VALIDATION_PLAYBOOK.md`, `MVP_SCOPE.md` e índice |
| `git status --short --branch` | `## main`, limpio |
| `git branch --show-current` | `main` |
| `git log -1 --format=fuller` | `c6fb92de685467b7c7954555e4bf9519ea388f0b`, `docs(agents): establish Codex execution contract` |
| Diff staged/unstaged | Vacío |
| Cambios ajenos/colisiones | No observados |

## Herramientas y desviaciones

| Capacidad | Clasificación | Evidencia e impacto |
|---|---|---|
| Git | Verificado | `2.50.1` |
| Git LFS | Verificado | `3.7.1` |
| Unity Hub | Verificado | `3.20.1` en `/Applications` |
| Unity Editor | Verificado | `6000.3.22f1 (1c726e1fb402)`, Apple silicon |
| Licencia batch | Verificado | Proyecto desechable creado/importado con código de salida `0` |
| Android Build Support | Verificado tras corregir la ruta | Existe en `6000.3.22f1/PlaybackEngines/AndroidPlayer`, como hermano de `Unity.app`; produjo un APK real |
| SDK/NDK/JDK de Unity | Verificados | Build/Platform Tools 36.0.0, cmdline-tools 16.0, NDK r27c, CMake 3.22.1, Gradle 9.1.0 y OpenJDK 17.0.18 bundled |
| iOS Build Support | No disponible localmente | No existe módulo iOS en la ubicación correcta de `PlaybackEngines`; iOS no se construye en esta fase |
| Xcode completo | No disponible localmente | `xcodebuild` apunta a Command Line Tools; no bloquea import Android-first |
| JDK global | Disponible, no seleccionado | Homebrew OpenJDK 17.0.18; no se acopla el proyecto a una ruta local |

Corrección de evidencia: la primera inspección consultó `Unity.app/Contents/PlaybackEngines`, que solo contiene soporte standalone. Unity Hub instaló Android como hermano del bundle, en `6000.3.22f1/PlaybackEngines/AndroidPlayer`. El build real corrigió esa conclusión antes del cierre. No se instaló ni modificó ningún módulo.

## Fuentes oficiales revalidadas

| Requisito | Fuente oficial | Fecha de consulta | Conclusión/impacto | Volver a verificar |
|---|---|---|---|---|
| Última revisión Unity 6.3 LTS | [Unity Release API](https://services.docs.unity.com/release/v1/) y endpoint `services.api.unity.com/unity/editor/release/v1/releases` | 2026-08-14 | La API devolvió `6000.3.22f1`, publicada 2026-08-13, como la revisión más reciente observada de 6.3 LTS; se conserva el pin | Antes de cambiar Editor y en cada release candidate |
| Dependencias Android de Unity | [Unity Manual — Android environment setup](https://docs.unity3d.com/6000.3/Documentation/Manual/android-sdksetup.html) | 2026-08-14 | Android requiere módulo Build Support y SDK/NDK/OpenJDK; Unity recomienda instalarlos con Hub | Al instalar/reparar módulo Android |
| Versiones soportadas | [Unity Manual — Supported dependency versions](https://docs.unity3d.com/6000.3/Documentation/Manual/android-supported-dependency-versions.html) | 2026-08-14 | Para revisiones actuales de Unity 6: JDK 17, NDK r27c y herramientas Android 36 aplican según la tabla oficial; deben validarse contra el módulo realmente instalado | Fase 03 cierre y cada actualización de Editor |
| Target Google Play | [Google Play — Target API requirements](https://support.google.com/googleplay/android-developer/answer/11926878?hl=es-419) | 2026-08-14 | Apps nuevas/updates deben apuntar a Android 16/API 36 desde 2026-08-31; baseline `target 36` | Antes del primer build de release y publicación |

## Decisión de continuación

La creación/importación y el smoke pueden continuar con `6000.3.22f1` y su toolchain bundled. El APK Development se construyó fuera de Git; el módulo iOS sigue ausente y no se instalará sin autorización.
