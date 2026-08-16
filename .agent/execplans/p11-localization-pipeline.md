# ExecPlan — baseline de localización ES/EN y pseudo-locale

- Fase/Gate: Prompt 11 / Gate B
- Estado: Complete
- Creado/actualizado: 2026-08-16 10:58 America/Mexico_City
- Owner: Unity Localization Engineer / Child UX Writer

## Propósito y alcance

Crear un pipeline local y offline de localización para todos los textos runtime actuales, con español predeterminado, inglés completo, pseudo-localización, cambio en vivo, persistencia versionada y validación bloqueante. Incluye el paquete oficial fijado, contratos Application, adapter Infrastructure, tablas Content, wiring Bootstrap, Presentation, tests, build y documentación. Excluye traducción masiva del MVP, voz humana, contenido factual nuevo, red y remote config.

La aceptación exige: cero texto visible de producción fuera de tablas; fallback Development/Release diferenciado; locale persistido mediante Save con migración; variables/plurales por Smart Strings; ES/EN/pseudo verificados; perfiles Development/Release y Android smoke; Git limpio tras un commit único.

## Contexto y orientación

HEAD inicial `d201d2d349ad65f63a039d55615b27b84fa6fe05`, rama `main`, árbol limpio y `origin/main` dos commits atrás. Unity `6000.3.22f1`; baseline previa: Addressables `4.0.1`, save schema v1, config Development/Release, Bootstrap/Camp/Jungle y nueve asmdefs. Fuentes canónicas: `AGENTS.md`, `docs/STATUS.md`, `docs/02_TECHNICAL_ARCHITECTURE.md`, `docs/10_SAVE_SYSTEM.md`, `docs/RUNTIME_CONFIGURATION.md`, `docs/14_UI_UX.md`, `docs/16_AUDIO.md`, `docs/CONTENT_PIPELINE.md`, `docs/DECISIONS.md` y este plan.

El inventario inicial encontró texto visible en `BootstrapStatusView`, `SceneTransitionView` y tres escenas. Domain no debe conocer texto ni APIs Unity. Presentation consumirá `LocalizedKey`; Infrastructure será el único adapter del paquete; Bootstrap compondrá e inyectará el servicio. La preferencia de locale pertenece a Save, por lo que cambiar el schema exige DTO v2 y migración v1→v2.

## Progreso

- [x] 2026-08-16 10:55 — preflight Git/documental/implementación completo; no había cambios ajenos ni baseline de localización.
- [x] 2026-08-16 11:00 — `scripts/validate` baseline PASS en 82.01 s; EditMode 57/57, PlayMode 5/5 y APK Development.
- [x] 2026-08-16 11:07 — Registry/manual/licencia oficiales verifican Localization `1.5.12`, estable y Unity Companion License.
- [x] 2026-08-16 — pin/import auditados: Localization `1.5.12`, AndroidJNI builtin `1.0.0`, lock exacto y licencia oficial.
- [x] 2026-08-16 — contrato, cinco colecciones, servicio, save v2, UI/escenas, validator y CSV implementados.
- [x] 2026-08-16 — ES/EN/pseudo, 62 EditMode, 6 PlayMode, catálogo y APK ES/EN ejecutados; Release quedó bloqueado por signing como corresponde.
- [x] 2026-08-16 — documentación canónica, diff y controles Git completados; commit se registra al cierre.

## Hallazgos

- No existía documento de localización, paquete, locale, tabla ni servicio; el estado esperado de Prompt 11 sí estaba satisfecho por config/save/scene flow/bootstrap.
- Hay 13 valores visibles hardcodeados en escenas y Presentation; `BootstrapStatusView` además usa copy técnico en inglés.
- Save v1 contiene preferencias adultas pero no locale. Usar `PlayerPrefs` duplicaría la autoridad y contradice el contrato de persistencia.
- El Registry oficial consultado el 2026-08-16 marca `1.5.12` como `latest`; declara Addressables `1.25.0` y Newtonsoft JSON `3.0.2` como dependencias. El pin directo Addressables `4.0.1` debe permanecer y el lock real decidirá la resolución compatible.
- El primer build Addressables tras importar Localization falló porque `SystemLocaleSelector` referencia `AndroidJavaClass` sin declarar el módulo builtin. Fijar `com.unity.modules.androidjni@1.0.0` corrigió compilación Android; no se ocultó el fallo.
- La primera invocación de setup usó zsh sobre un wrapper Bash y falló antes de Unity. La invocación Bash creó assets, pero el método de authoring no cerraba el Editor y fue interrumpido después del marker de éxito; compile/validator posteriores verificaron los assets. El setup queda para menú/authoring, no como comando canónico.

## Decisiones

- 2026-08-16 — fijar `com.unity.localization@1.5.12`; es la versión estable oficial vigente, sin rango ni preview. Rollback: retirar pin/asset/adapter mediante este mismo plan y restaurar tablas/copy del commit anterior.
- 2026-08-16 — persistir `LocaleId` como preferencia adulta dentro de schema v2; migrar v1→v2 con `es` por defecto y conservar v0→v1→v2. No usar PlayerPrefs ni copiar AppConfig al save.
- 2026-08-16 — español e inglés son locales de usuario; pseudo es herramienta Development y no se persiste como preferencia de Release.
- 2026-08-16 — conservar nueve assemblies: añadir referencias de paquete a Infrastructure/Editor/tests sin crear un assembly nuevo.

## Plan de implementación

1. Añadir el pin exacto, importar con el Editor fijado y registrar licencia, checksum, transitivos, compatibilidad móvil/offline y rollback.
2. Definir IDs/keys y `ILocalizationService` BCL-only; implementar adapter Unity Localization con init, locale change, Smart Strings, fallback y lifecycle.
3. Crear settings/locales/tablas `Shared`, `UI`, `Content` y asset tables `Voice`/`Illustrations`; español completo, inglés no vacío y pseudo desde español.
4. Migrar Save a schema v2 con locale estable y v1→v2; conectar Bootstrap y autosave al cambio de locale.
5. Reemplazar strings de Presentation/escenas, añadir selector Development y refresh por evento sin reinicio.
6. Extender validator Editor/CLI, export/import CSV y checks de keys, locales, tablas, fonts y seguridad Release.
7. Añadir EditMode/PlayMode, ejecutar ambos locales/pseudo en resoluciones objetivo, Android ES/EN, diff/permisos/tamaño y documentación.

## Comandos y validación

- `git status --short --branch`, `git branch --show-current`, `git log -1 --format=fuller` — preflight PASS; `main`, limpio, HEAD inicial registrado.
- `scripts/validate` — baseline PASS, 82.01 s; compile, catálogo local, EditMode 57/57, PlayMode 5/5 y APK Development.
- `curl -fsSL https://packages.unity.com/com.unity.localization` — Registry oficial: `latest=1.5.12`, SHA-1 tarball `b0a588a05f2a20af8e4afc33cf1c4591b7df5a28`.
- `scripts/validate-localization` — PASS en 9.70 s; ES/EN/pseudo, 25 keys, 3 string tables y 2 asset tables.
- `scripts/test-editmode` — PASS `62/62`; `scripts/test-playmode` — PASS `6/6`, pseudo/layout en 1280×720 y 1920×1080.
- `scripts/build-android-locales` — PASS total en 368.50 s; ES/EN API 26/36, IL2CPP/ARM64. Ambos APK pasaron `zipalign -P 16`; sin dispositivo adb.
- `scripts/validate` final pre-commit — PASS en 123.96 s, `PE_FULL_VALIDATION_OK`.
- `scripts/build-android-release` — `BLOCKED` esperado, código `3`, signing externo ausente; no se construyó ni firmó Release.

## Recovery y seguridad

No modificar paquetes desde otro Editor ni editar `Library`. Antes de una migración destructiva, las fixtures usan stores en memoria/temporales; no tocar save personal. Si import/package falla, conservar logs y revertir solo archivos propios mediante patch explícito, nunca limpiar cambios ajenos. El paquete no autoriza red, telemetría, SDK comercial, permisos, contenido remoto ni aceptación de términos. Release debe excluir selector pseudo/diagnóstico y fallar si falta tabla/locale.

## Resultados y retrospectiva

La fase entrega ES/EN completos para la UI existente, pseudo Development, cambio live/persistido schema v2, fallback fail-safe, cinco colecciones y catálogo local sin endpoint. EditMode `62/62`, PlayMode `6/6`, CSV export y APK ES/EN pasaron. El manifest Development conserva `INTERNET`/receiver interno ya documentados por Gate A, sin cámara, micrófono, ubicación, contactos ni AD_ID; Release AAB sigue bloqueado para Prompt 12/release pipeline. Restan revisión lingüística humana, voces/assets finales y dispositivo Android físico.
