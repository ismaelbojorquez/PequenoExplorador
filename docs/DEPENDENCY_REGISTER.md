# Registro de dependencias Unity

Baseline observada: 2026-08-14. `Packages/manifest.json` y `packages-lock.json` son la fuente exacta ejecutable; esta tabla explica el intake humano/técnico.

| Dependencia directa | Versión | Fuente/licencia | Necesidad actual | Datos/permisos/native | Compatibilidad y rollback |
|---|---:|---|---|---|---|
| Input System | `1.20.0` | Unity Registry oficial; Unity Companion License | Baseline de input móvil; backend nuevo habilitado, sin acciones de gameplay | Sin SDK comercial ni recolección declarada en el proyecto; no añade permiso sensible observado | Compatible con `6000.3.22f1`; retirar manifest entry y volver a old input requiere reimport y decisión |
| Universal Render Pipeline | `17.3.0` | Built-in oficial; Unity Companion License + Third Party Notices | Pipeline móvil conservador y reproducible | No tráfico/identificador; transitivos generan librerías de player revisadas en APK | Coincide con catálogo del Editor; rollback exige recrear/reasignar pipeline |
| Unity Test Framework | `1.6.0` | Built-in oficial; Unity Companion License | EditMode/PlayMode y fronteras | Solo Editor/tests; no API runtime propia ni permiso observado | Retirable al costo de perder evidencia automatizada |
| uGUI | `2.0.0` | Built-in oficial; Unity Companion License | Diagnóstico temporal y futura UI runtime/TMP | Sin tráfico/identificador ni permiso sensible observado | Coincide con catálogo del Editor; necesario para escena actual |

No se añadieron paquetes preview, SDKs comerciales, Addressables ni Localization. El lock contiene transitivos oficiales, incluidos Core RP, Shader Graph, Burst, Collections, Mathematics y módulos built-in. Ninguna dependencia directa contiene `.so`/`.aar` propia en sus carpetas inspeccionadas; el APK final sí contiene siete `.so` generadas/bundled, todas ARM64 y con `LOAD align 0x4000`.

La licencia de estos paquetes no selecciona la licencia del producto; `LICENSE_NOT_SELECTED.md` sigue gobernando. Toda actualización repite fuente, licencia, pin, mantenimiento, mobile/IL2CPP, 16 KB, permisos/datos, aptitud infantil, tamaño y rollback conforme a `ENGINEERING_STANDARDS.md`.
