# Testing y comandos reproducibles

Este documento es la guía operativa del pipeline local. La política de evidencia y la matriz por tipo de cambio permanecen en [`VALIDATION_PLAYBOOK.md`](VALIDATION_PLAYBOOK.md).

## Requisitos

- macOS o Linux con Bash, Ruby estándar, Git y Unity `6000.3.22f1` activado;
- Android Build Support bundled para el paso APK;
- raíz del repositorio como directorio actual.

El localizador usa la instalación de Unity Hub para la revisión fijada. En otra disposición, definir `UNITY_EDITOR` únicamente en el entorno local; ningún path de máquina se guarda en el repo.

## Comando completo

```sh
scripts/validate
```

Ejecuta en orden y se detiene al primer fallo: `check-repository`, `compile`, build Addressables local, EditMode, PlayMode y APK Android Development. Un éxito total imprime `PE_FULL_VALIDATION_OK`. Los outputs quedan en `artifacts/logs`, `artifacts/reports`, `artifacts/test-results` y `artifacts/builds`.

## Comandos individuales

| Comando | Responsabilidad | Salida principal |
|---|---|---|
| `scripts/check-repository` | Markdown, enlaces relativos, JSON/asmdefs, package pins, YAML/Actions, secretos básicos y Bash. | stdout y código de salida. |
| `scripts/compile` | Import/compile Unity, fronteras, placeholders y reporte de entorno. | `artifacts/logs/compile.log`, `artifacts/reports/environment.json`. |
| `scripts/validate-content` | Catálogo Development, IDs/referencias/editorial y metadata de placeholders. | Log + `artifacts/reports/content-catalog-development.{json,md}`. |
| `scripts/validate-localization` | Locales/tablas/keys/ES-EN/assets/glifos y escenas sin texto serializado. | `artifacts/logs/validate-localization.log`. |
| `scripts/validate-audio` | Mixer/buses, cues, mono/48 kHz, clipping, addresses y bloqueo de placeholders. | `artifacts/logs/validate-audio.log`. |
| `scripts/build-addressables-local` | Valida perfiles/grupos/labels/dependencias y construye catálogo Android local. | Log + `artifacts/reports/addressables-local.json`; runtime data ignorada bajo `Library`. |
| `scripts/test-editmode` | Suite EditMode. | XML NUnit y JUnit en `artifacts/test-results/`. |
| `scripts/test-playmode` | Suite PlayMode/escena. | XML NUnit y JUnit en `artifacts/test-results/`. |
| `scripts/build-android-development` | APK Development IL2CPP/ARM64 API 26/36. | APK, log y manifest JSON con tamaño/hash. |
| `scripts/build-android-locales` | APK Development español e inglés con el mismo contenido local. | Dos APK/logs/manifests diferenciados. |
| `scripts/build-android-release` | Guard rail de Release. | Código no cero y `android-release-blocked.json`; nunca construye/firma. |

## Suites de lifecycle y servicios

EditMode cubre orden y shutdown inverso, idempotencia secuencial/concurrente, fallo recuperable, cleanup del servicio que falla, cancelación externa, shutdown durante inicialización sin retorno a `Ready`, dispose, IDs duplicados, clock manual, random seeded, perfiles Development/Release, resultados Mock/NoAds/Unavailable, define no persistido y cleanup de listeners. PlayMode carga/reload de `Bootstrap`, exige un solo root y espera `Ready` visible.

La tabla de perfiles y orden de servicios es canónica en [`02_TECHNICAL_ARCHITECTURE.md`](02_TECHNICAL_ARCHITECTURE.md). Un adapter nuevo no está validado solo porque compile: debe ampliar estas suites sin reloj/azar/red reales.

Scene flow EditMode cubre estados, exclusión mutua, error/retry, cancelación, timeout, unload y shutdown. PlayMode exige un único Bootstrap, `Boot→Camp`, tres ciclos `Camp→Jungle→Camp`, un solo handle para la escena actual, cero tras shutdown y recuperación del fallo Development. El stub documental incluido por Addressables se distingue del conteo de tests del proyecto.

Mundos EditMode cubre mapping del manifest Selva, duplicate IDs, Release Draft, locked/missing sin carga ni mutación de progreso y una fixture in-memory `world.test-ocean`. Esa fixture usa `scene/test-ocean` a través del mismo `WorldLoadUseCase`/`SceneFlowService`, sin modificar el coordinador. PlayMode enumera Selva desde `IWorldCatalog` y ejecuta tres ciclos por `WorldId`, comprobando sesión activa, unload y handles.

Save EditMode cubre default/round-trip, JSON determinista, write/flush/commit inyectados, truncado, checksum, backup preservado, v0→v1→v2→v3, migración v2→v3, migración ausente, future schema read-only, cancelación, reset, coalescing y replace físico repetido. PlayMode recrea el servicio tras recarga de escena y verifica el checkpoint. Los directorios físicos de tests son temporales con prefijo controlado; no se usa `PlayerPrefs`, red, reloj real ni rutas versionadas. Detalle: [`10_SAVE_SYSTEM.md`](10_SAVE_SYSTEM.md).

Configuración EditMode cubre defaults, dos assets locales, mapping, IDs duplicados, budgets inválidos, cada flag prohibido en Release y override temporal restaurable. PlayMode comprueba que Bootstrap selecciona Development, muestra producto/versión del asset y conserva Ready/scene flow/save. `scripts/compile`/build llaman el validador de ambos perfiles; una fixture controlada Release+`MockAds` debe fallar `CONFIG008`. Contrato: [`RUNTIME_CONFIGURATION.md`](RUNTIME_CONFIGURATION.md).

Localización EditMode cubre español default, resolución, Smart variables/plurales, persistencia y restauración v3 sin `PlayerPrefs`, fallback Development/Release y pseudo no persistible. PlayMode cambia ES/EN/pseudo sin reinicio, confirma refresh/persistencia y layouts a `1280×720`/`1920×1080`. El validator cubre 31 keys, cinco colecciones, dos locales y glifos. Contrato: [`17_LOCALIZATION.md`](17_LOCALIZATION.md).

Audio EditMode cubre catálogo/addresses/import, defaults y valores inválidos, prioridad/FIFO/capacidad, cooldown con tiempo inyectado, settings persistidos y missing cue no bloqueante. PlayMode verifica siete sources únicos, ducking, subtítulo, replay, suspend/resume, cue EN y Camp↔Jungle sin duplicados. `scripts/validate-audio` registra diez placeholders pendientes; esto es PASS estructural, no aprobación Release ni prueba auditiva humana.

Input EditMode cubre tap/hold/drag, thresholds, pinch limitado, supresión multitouch, cancelación por mapa, cinco action maps, haptics no-op, cuatro presets landscape, rotación y hot path sin allocations observables tras warmup. PlayMode usa `InputTestFixture` para Back/pausa, UI↔Explorer, doble toque accidental y safe area en 4:3, 16:9, 20:9 y 16:10. El validator bloquea APIs legacy, `Touchscreen.current`, target menor a `64×64`, asset/wiring incompleto o más/menos de un fitter por Canvas. Hardware Android real sigue requerido antes de Gate C.

Contenido EditMode cubre parseo/igualdad de IDs, lookup/alias, orden determinista, duplicados, referencias ausentes, localización/audio/visual inexistentes, watermark, generador no destructivo y rechazo Release de Draft. PlayMode resuelve `discovery.jungle.placeholder` desde Bootstrap sin `AssetDatabase`. El reporte Release controlado es `FAIL` esperado mientras el placeholder no esté Approved.

Los conteos, tiempos y artefactos canónicos de la última ejecución integral se registran en [`STATUS.md`](STATUS.md); cifras de fases anteriores no se heredan como evidencia.

Los wrappers son orquestadores: configuración, validación y build viven bajo `Assets/_Game/Editor/BuildTools`. Los logs sustituyen la raíz del proyecto, home y ejecutable del Editor por marcadores antes de conservarse.

## Diagnóstico y recuperación

1. conservar `artifacts/` y abrir el log del primer comando fallido;
2. buscar `error CS`, `Exception`, `FAILED`, `PE_` o el código indicado;
3. corregir implementación/configuración, no editar `Library` ni artefactos generados;
4. reejecutar primero el comando individual y después `scripts/validate`;
5. si falta Unity/módulo/activación, reportar `NOT RUN` o `BLOCKED` con ese motivo, no `PASS`.

El escaneo de secretos es preventivo y deliberadamente básico; no reemplaza revisión humana ni una herramienta dedicada. `shellcheck` y `actionlint` son útiles cuando estén disponibles, pero no son dependencias del repositorio.

## CI

`.github/workflows/checks.yml` fija Actions oficiales por SHA, usa `contents: read`, desactiva credenciales persistentes y no usa `pull_request_target`. El job estático puede correr en GitHub-hosted; el job Unity solo aparece en `workflow_dispatch`, con variable `UNITY_CI_ENABLED=true` y runner self-hosted etiquetado. Configuración humana: [`GITHUB_SETUP.md`](GITHUB_SETUP.md).
