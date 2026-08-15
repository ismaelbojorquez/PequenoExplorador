# Fase 00 — Evidencia de preflight

Fecha de ejecución: 2026-08-14 (`America/Mexico_City`). Alcance: inspección y baseline documental; no se creó proyecto Unity.

## Estado inicial verificado

- Directorio actual: raíz del workspace esperada; `pwd` se verificó y la ruta personal se omite deliberadamente del repositorio.
- `find . -maxdepth 2 -mindepth 1 -print`: sin salida; la carpeta era literalmente vacía.
- `AGENTS.md`: ausente, permitido únicamente durante Fase 00; no había documentos previos que leer.
- `git status --short --branch`: `fatal: not a git repository`.
- `git branch --show-current`: `fatal: not a git repository`.
- `git log -1 --format=fuller`: `fatal: not a git repository`.
- Implementación, configuración, pruebas y diff: inexistentes; por tanto no había cambios ajenos ni colisiones.

## Herramientas locales verificadas

| Herramienta | Estado | Evidencia resumida |
|---|---|---|
| Git | Verificado | `/usr/bin/git`, 2.50.1 (Apple Git-155). |
| Git LFS | Verificado | `git lfs version`, 3.7.1. |
| Unity Hub | Verificado | Aplicación presente; CLI enumeró Editor instalado. |
| Unity Editor | Verificado | `6000.3.22f1`, Apple silicon. No se abrió ni creó proyecto. |
| Android Build Support | Verificado | Módulo instalado con SDK, NDK y OpenJDK incluidos por Unity. |
| OpenJDK de Unity | Verificado | Temurin 17.0.18+8. |
| SDK de Unity | Verificado | Build Tools 36.0.0, Platform Tools 36.0.0, Command-line Tools 16.0. |
| NDK de Unity | Verificado | r27c, `27.2.12479018`. |
| Gradle de Unity | Verificado | 9.1.0; AGP 9.0.0 confirmado por manual Unity 6.3. |
| SDK/NDK global adicional | Disponible, no seleccionado | SDK Android de usuario tiene plataformas 37.x y NDK 27.1/28.2; no se modificó ni se adoptó. |
| Xcode completo | No disponible localmente | `xcodebuild` apunta a Command Line Tools y rechaza builds Xcode. iOS queda pendiente. |
| Licencia/activación Unity | Pendiente humano/Fase 01 | La enumeración del Editor no prueba activación para batch build. |

## Investigación oficial

La evidencia, URL, fecha, conclusión e impacto se normalizan en [`VERSION_MATRIX.md`](VERSION_MATRIX.md) y [`POLICY_SOURCE_REGISTER.md`](POLICY_SOURCE_REGISTER.md). La API oficial de releases de Unity y la página de release confirmaron que `6000.3.22f1` fue publicada el 2026-08-13 y era la revisión 6.3 LTS más reciente devuelta el 2026-08-14.

## Desviaciones

Ninguna respecto del estado inicial esperado. Tras esta inspección se inicializó Git en `main`; no se instalaron herramientas, paquetes ni dependencias.
