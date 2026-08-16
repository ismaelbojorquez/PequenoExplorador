# 17 — Localización ES/EN

Estado: baseline técnica de Prompt 11. Español e inglés están completos para la UI runtime actual; pseudo-localización es una herramienta Development. Esto no declara final la traducción del MVP ni incluye voces humanas.

## Dependencia y autoridad

El proyecto fija `com.unity.localization@1.5.12`, estable en Unity Registry el 2026-08-16, bajo Unity Companion License. La versión y licencia se contrastaron en el [manual oficial](https://docs.unity3d.com/Packages/com.unity.localization@1.5/manual/index.html) y la [licencia oficial](https://docs.unity3d.com/Packages/com.unity.localization@1.5/license/LICENSE.html). El tarball del Registry declara SHA-1 `b0a588a05f2a20af8e4afc33cf1c4591b7df5a28`; el lock mantiene Addressables `4.0.1` y Newtonsoft JSON `3.2.2` ya resueltos.

Localization usa `AndroidJavaClass` en su selector de locale del sistema. Por ello el manifest fija también el módulo builtin oficial `com.unity.modules.androidjni@1.0.0`; no es un SDK comercial, no contiene endpoint y no cambia la selección de locale, que sigue siendo explícita. Ninguno de estos paquetes introduce `.so` o `.aar` propio en PackageCache. Toda actualización repite intake, import, IL2CPP, permisos, 16 KB y rollback.

## Contrato y flujo

```text
Content: tablas + assets de authoring
       ↓ LocalizedKey estable
Application: ILocalizationService
       ↓
Infrastructure: UnityLocalizationService → Unity Localization
       ↑
Bootstrap: Save → Localization → Presentation
       ↓
Presentation: resuelve keys y refresca al evento LocaleChanged
```

Domain no contiene copy localizada ni referencia Unity. `LocalizedKey` identifica `Table + Entry`; no usa el español como key. Bootstrap inicializa Save antes de Localization y Audio después de Localization. Cambiar `es`/`en` guarda mediante `ISaveService` schema v3, refresca vistas/subtítulos y selecciona el clip de voz correspondiente sin reinicio. Si el guardado falla, el servicio revierte el locale visible.

Español (`es`) es locale de proyecto, startup y fallback humano. Inglés (`en`) es una baseline no vacía. `qps-ploc` expande español solo en Development, nunca se persiste y Release lo rechaza. No se consulta idioma del sistema, red, catálogo remoto ni backend.

## Tablas y claves

| Tabla | Claves | Responsabilidad |
|---|---:|---|
| `Shared` | 4 | Nombre, versión Smart, fallback seguro y plural Smart de estrellas. |
| `UI` | 16 | Estados, errores, transición, acciones y selector Development. |
| `Content` | 8 | Nombres/placeholder de mundos y tres subtítulos de audio. |
| `Voice` | 5 slots conceptuales | Camp/Jungle más tres cues de voz ES/EN; clips finales pendientes. |
| `Illustrations` | 2 slots conceptuales | Futuros fondos Camp/Jungle; sin assets finales. |

Las 30 keys de texto son namespaced (`shared.*`, `ui.*`, `content.*`) y su catálogo canónico está en `LocalizationKeys.cs`. Voz y subtítulo comparten concepto, por ejemplo `content.audio.name.jungle`; las tablas no conocen archivos. Las tablas y clips baseline se mantienen en Addressables locales; no existen `Remote*`, URL ni update-on-start.

Variables, números y plurales usan Smart Strings de Unity Localization. Están cubiertos `shared.build.version`, `shared.progress.stars` y `ui.transition.preparing`; no se concatenan frases traducibles en Presentation.

## Fallback y copy infantil

- Development: una key ausente muestra `[missing Tabla:key]` y deja log estructurado sin datos infantiles.
- Release: intenta `shared.fallback.safe` en el locale seleccionado y, si también falta, muestra `…`; nunca enseña IDs técnicos o una excepción.
- Las instrucciones actuales son breves, invitacionales y no punitivas. El copy factual futuro sigue `CONTENT_SOURCES.md` antes de Release.
- `LegacyRuntime.ttf` cubre `áéíóúüñÁÉÍÓÚÜÑ¿¡…·`; reemplazar la fuente exige repetir glifos, ES/EN y pseudo en resoluciones objetivo.

## Authoring, validación y CSV

Los assets viven en `Assets/_Game/Content/Localization/`. `LocalizationFoundationSetup` crea/reconcilia settings, dos locales, pseudo y cinco colecciones; no debe usarse para regenerar a ciegas un proyecto con contenido humano sin revisar diff.

```sh
scripts/validate-localization
scripts/build-android-locales
```

El validador bloquea settings/locales incorrectos, keys duplicadas/no namespaced, traducciones ES/EN vacías, slots conceptuales ausentes, texto serializado en escenas y glifos mínimos. `scripts/validate` lo ejecuta dentro de compile/content validation y después corre suites y APK español. El comando dual construye APK Development español e inglés bajo `artifacts/builds/`.

Export: menú `Pequeño Explorador/Development/Localization/Export CSV to artifacts`; genera `Shared.csv`, `UI.csv` y `Content.csv` ignorados. Import: `Import CSV (merge)…`; el archivo debe llamarse igual que la tabla, conserva entradas no incluidas y requiere revisar diff, validator, pseudo y tests antes de commit. CSV es intercambio, no una segunda fuente de verdad.

## Aceptación y límites

EditMode prueba resolución, Smart Strings/plurales, persistencia schema v3, fallback Development/Release, keys de subtítulo y pseudo no persistible. PlayMode cambia ES→EN→pseudo→ES sin reinicio, verifica expansión/layout y selecciona cue EN. APK ES/EN demuestra compilación IL2CPP/ARM64 y contenido local; reproducción en dispositivo físico permanece `NOT RUN` cuando `adb` no lista hardware.

El selector runtime actual es diagnóstico Development. El futuro selector parental deberá vivir tras el flujo adulto, conservar estas mismas preferencias y pasar Child UX/política; no se implementó ese flujo aquí. Traducción masiva, revisión lingüística humana, narraciones y assets localizados finales siguen pendientes.
