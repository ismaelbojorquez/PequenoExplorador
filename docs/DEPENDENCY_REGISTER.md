# Registro de dependencias Unity

Baseline observada: 2026-08-15. `Packages/manifest.json` y `packages-lock.json` son la fuente exacta ejecutable; esta tabla explica el intake humano/técnico.

| Dependencia directa | Versión | Fuente/licencia | Necesidad actual | Datos/permisos/native | Compatibilidad y rollback |
|---|---:|---|---|---|---|
| Input System | `1.20.0` | Unity Registry oficial; Unity Companion License | Baseline de input móvil; backend nuevo habilitado, sin acciones de gameplay | Sin SDK comercial ni recolección declarada en el proyecto; no añade permiso sensible observado | Compatible con `6000.3.22f1`; retirar manifest entry y volver a old input requiere reimport y decisión |
| Universal Render Pipeline | `17.3.0` | Built-in oficial; Unity Companion License + Third Party Notices | Pipeline móvil conservador y reproducible | No tráfico/identificador; transitivos generan librerías de player revisadas en APK | Coincide con catálogo del Editor; rollback exige recrear/reasignar pipeline |
| Unity Test Framework | `1.6.0` | Built-in oficial; Unity Companion License | EditMode/PlayMode y fronteras | Solo Editor/tests; no API runtime propia ni permiso observado | Retirable al costo de perder evidencia automatizada |
| uGUI | `2.0.0` | Built-in oficial; Unity Companion License | Diagnóstico temporal y futura UI runtime/TMP | Sin tráfico/identificador ni permiso sensible observado | Coincide con catálogo del Editor; necesario para escena actual |
| Addressables | `4.0.1` | [Unity Registry](https://packages.unity.com/com.unity.addressables) y [manual 4.0](https://docs.unity3d.com/Packages/com.unity.addressables@4.0/manual/index.html); Unity Companion License; SHA-1 `37a0b4bd16b0a191e1e08e9b62908ca4284b0f76` | Carga/descarga asíncrona local de Camp/Jungle y ownership medible | Sin `.so/.aar`; módulos UnityWebRequest transitivos existen pero catálogo/paths son locales, sin endpoint ni datos; APK conserva solo `INTERNET` de Development | Registry declara Unity `6000.0+`; import, EditMode/PlayMode, catálogo Android y APK offline pasaron. Rollback: retirar pin/settings/adapter y volver a escenas directas mediante plan. |

No se añadieron paquetes preview, SDKs comerciales ni Localization. Addressables solicita transitivamente Scriptable Build Pipeline `4.0.0` y Profiling Core `1.0.2`; el lock resuelve exactamente `4.0.0` y `1.0.3`, respectivamente. El resto del lock conserva transitivos oficiales. Ninguna dependencia directa nueva contiene `.so`/`.aar`; por ello no cambia la readiness nativa 16 KB, que se revalida sobre cada artefacto.

F09 no añadió dependencia: encapsula `UnityEngine.JsonUtility` del módulo builtin ya resuelto `com.unity.modules.jsonserialize` `1.0.0`. Su uso queda limitado a DTOs Infrastructure según `10_SAVE_SYSTEM.md`; cambiarlo exige intake/ADR y migraciones.

La licencia de estos paquetes no selecciona la licencia del producto; `LICENSE_NOT_SELECTED.md` sigue gobernando. Toda actualización repite fuente, licencia, pin, mantenimiento, mobile/IL2CPP, 16 KB, permisos/datos, aptitud infantil, tamaño y rollback conforme a `ENGINEERING_STANDARDS.md`.
