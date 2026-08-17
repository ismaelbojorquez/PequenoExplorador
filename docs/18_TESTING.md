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
| `scripts/setup-photography` | Regenera wiring reproducible del target/UI/cámara ficticia. | `artifacts/logs/setup-photography.log`. |
| `scripts/setup-economy` | Regenera reward catalog, UI/wiring y keys de economía. | `artifacts/logs/setup-economy.log`. |
| `scripts/validate-economy` | Valida única moneda/reward, safe area, targets y límites Release. | `artifacts/logs/validate-economy.log`. |
| `scripts/setup-missions` | Regenera mission/reward catalog, UI/wiring y keys de la fixture. | `artifacts/logs/setup-missions.log`. |
| `scripts/validate-missions` | Valida definitions/strategies/referencias/grafo/editorial/UI. | `artifacts/logs/validate-missions.log`. |
| `scripts/setup-learning` | Regenera catálogo/reward/UI/keys de la fixture educativa abstracta. | `artifacts/logs/setup-learning.log`. |
| `scripts/validate-learning` | Valida definitions/conceptos/strategy/editorial/localización/UI y límites de capa. | `artifacts/logs/validate-learning.log`. |
| `scripts/setup-customization` | Regenera catálogo de 8 slots/20 opciones, rig, preview Camp, UI y localización. | `artifacts/logs/setup-customization.log`. |
| `scripts/validate-customization` | Valida defaults/costos/localización, rig/materiales, Camp, UI, capas y targets. | `artifacts/logs/validate-customization.log`. |
| `scripts/setup-design-system` | Regenera tokens, sprite redondeado, galería y tema de ocho roots críticos. | `artifacts/logs/setup-design-system.log`. |
| `scripts/validate-design-system` | Valida assembly, tokens, TMP, galería, contraste AA, superficie Paper del detalle de álbum (`UI012`), Canvas y targets 64/72. | `artifacts/logs/validate-design-system.log`. |
| `scripts/capture-ui-review before|after` | Renderiza diez superficies en 4:3, 16:9, 20:9 y 16:10. | 40 PNG por fase en `artifacts/ui-review/`. |
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

Fixture visual VS-D-A01: EditMode valida IDs, `Approved`/no-placeholder, metadata H-008/H-009, trigger, bounds, siete materiales compartidos, budgets provisionales y ledger; el gate específico ya no emite `TOUCAN019`. La prueba de idempotencia ejecuta dos generaciones, conserva GUID y compara meshes/vértices/triángulos/materiales/bounds. PlayMode exige el prefab hijo `VS_ToucanPicoCanoa`, ausencia de la cápsula visual y al menos ocho renderers antes de recorrer la interacción neutral existente. Los renders son evidencia humana, no golden tests; Android físico y peak de memoria permanecen `NOT RUN`.

Mundos EditMode cubre mapping del manifest Selva, duplicate IDs, Release Draft, locked/missing sin carga ni mutación de progreso y una fixture in-memory `world.test-ocean`. Esa fixture usa `scene/test-ocean` a través del mismo `WorldLoadUseCase`/`SceneFlowService`, sin modificar el coordinador. PlayMode enumera Selva desde `IWorldCatalog` y ejecuta tres ciclos por `WorldId`, comprobando sesión activa, unload y handles.

Save EditMode cubre default/round-trip, JSON determinista, atomicidad/checksum/backup, v0→…→v11, migraciones hasta v10→v11, photos/economy/mission/learning/Camp/customization metadata, future schema read-only, cancelación y coalescing. PlayMode recrea el servicio y valida discovery/foto/economía/misión/learning/Camp/customization. No se usa `PlayerPrefs`, red, reloj real ni rutas versionadas. Detalle: [`10_SAVE_SYSTEM.md`](10_SAVE_SYSTEM.md).

Personalización EditMode cubre catálogo/defaults, IDs duplicados, fallback por opción retirada, unlock/equip separados, saldo/prerrequisito/read-only, incompatibilidad, idempotencia, commit fallido/retry, round-trip y migración v10→v11; la fixture Release rechaza `PH_`. PlayMode ejecuta preview→unlock→equip→Selva→Camp→flush/reload, ratios `1024×768`, `1920×1080`, `2400×1080` y verifica que preview no instancia materiales. Hardware/clipping/tonos finales siguen `NOT RUN`.

Configuración EditMode cubre defaults, dos assets locales, mapping, IDs duplicados, budgets inválidos, cada flag prohibido en Release y override temporal restaurable. PlayMode comprueba que Bootstrap selecciona Development, muestra producto/versión del asset y conserva Ready/scene flow/save. `scripts/compile`/build llaman el validador de ambos perfiles; una fixture controlada Release+`MockAds` debe fallar `CONFIG008`. Contrato: [`RUNTIME_CONFIGURATION.md`](RUNTIME_CONFIGURATION.md).

Localización EditMode cubre español default, resolución, Smart variables/plurales, persistencia y restauración sin `PlayerPrefs`, fallback Development/Release y pseudo no persistible. PlayMode cambia ES/EN/pseudo sin reinicio, confirma refresh/persistencia y layouts. El validator cubre 92 `LocalizedKey` públicos, cinco colecciones, dos locales y glifos. Contrato: [`17_LOCALIZATION.md`](17_LOCALIZATION.md).

Audio EditMode cubre catálogo/addresses/import, defaults y valores inválidos, prioridad/FIFO/capacidad, cooldown con tiempo inyectado, settings persistidos y missing cue no bloqueante. PlayMode verifica siete sources únicos, ducking, subtítulo, replay, suspend/resume, cue EN y Camp↔Jungle sin duplicados. `scripts/validate-audio` registra diez placeholders pendientes; esto es PASS estructural, no aprobación Release ni prueba auditiva humana.

Input EditMode cubre tap/hold/drag, thresholds, pinch limitado, supresión multitouch, cancelación por mapa, cinco action maps, haptics no-op, cuatro presets landscape, rotación y hot path sin allocations observables tras warmup. PlayMode usa `InputTestFixture` para Back/pausa, UI↔Explorer, doble toque accidental y safe area en 4:3, 16:9, 20:9 y 16:10. El validator bloquea APIs legacy, `Touchscreen.current`, target menor a `64×64`, asset/wiring incompleto o más/menos de un fitter por Canvas. Hardware Android real sigue requerido antes de Gate C.

Locomoción EditMode cubre comando, destino inválido recuperable, pending/moving/arrived, cancelación, suspensión y 100 reemplazos de destino. PlayMode usa mouse como la misma action `<Pointer>`, valida path válido/inválido, spam, UI/Photography, reduce motion, unload y tres ciclos Selva. Registra FPS básico de Editor batch sin presentarlo como device profiling. El validator exige AI Navigation `2.0.9`, prefab/root `PH_`, un surface/data/walkable, tuning exacto, cámara cableada y cero Animator/root motion. Touch/FPS/latencia/allocations en Android físico permanecen `NOT RUN` sin hardware.

Interacción EditMode cubre prioridad→distancia→ID, rango/approach, unavailable, idempotencia/cooldown con reloj inyectado, cancelación, suspensión y target ausente. PlayMode toca un fixture mediante `InputTestFixture`, espera approach→Ready, ejecuta spam una sola vez, comprueba las tres formas sobre el mismo núcleo, fallback ES, cancel button, UI, target destruido y unload. El validator exige los tres IDs baseline/IDs únicos, permite entries adicionales data-driven y comprueba localización/audio, collider trigger grande, punto sobre NavMesh, safe area/targets `64×64`, wiring único y rechazo Release de `PH_`. Android touch físico sigue `NOT RUN` sin dispositivo.

Discovery EditMode cubre first/repeat, grant idempotente, missing, Draft, retirado, denominadores y día agregado. Fotografía añade thresholds/scoring, best-photo, filenames/budgets, storage fallback, cancelación y shutter concurrente. Economy añade wallet, grant/spend, insuficiente, overflow, source mismatch, retry/crash y ledger 32. Missions cubre strategies/pre-event/idempotencia. Learning cubre correct/incorrect, `TryAgain`→pistas, retry, exit/resume/restart, reward/fact idempotentes, Draft Release gate, agregado concepto+día y v8→v9; PlayMode usa la UI real para ES→EN, replay, completion y reload. Android físico sigue pendiente.

Camp EditMode cubre catálogo data-driven, duplicados, referencias, ciclos, prerequisitos, saldo suficiente/insuficiente, read-only, compra duplicada, commit fallido/retry, atomicidad y migración v9→v10. PlayMode cubre preview/cancel, compra por 3 Estrellas, variante visual, Selva→Camp, flush/reload y targets en `1024×768`, `1920×1080`, `2400×1080`. `scripts/validate-camp` comprueba cuatro estaciones/anchors, safe area, targets `≥64×64`, Addressables locales y bloqueo de parent area/PH_ Release. Hardware Android real sigue `NOT RUN`.

Prompt 24 amplía Learning con solución por `TagId` independiente del orden, enlace al fact Approved, source/editorial gate, cues/reacciones y actividad Sourced bloqueada en Release. El PlayMode integrado conserva regresiones de foto/álbum y recorre interacción→captura válida→botón Learning→incorrecta/pista→ES/EN→correcta/replay/reward→retorno a fotografía, incluida reacción reduce-motion. Targets/safe area son automatizados; audio/UX/touch en Android real siguen `NOT RUN`.

Álbum EditMode cubre conteos/filtro desde catálogo Approved, Draft oculto, locked sin fuga, detail/facts/photo, contenido retirado, facts/assets ausentes y lectura/cancelación de `LocalPhotoStore`. PlayMode abre desde Camp, comprueba actualización tras captura sin reinicio, foto/detail/replay deshabilitado, Back/cancel/cleanup, ES/EN/pseudo y targets/best-fit en `1024×768`, `1280×720`, `1600×720` y `1280×800`. `scripts/validate-album` bloquea metadata/wiring/pool/safe-area/targets y acceso directo de Presentation a storage/editor APIs. Android físico y fuente grande visual son `NOT RUN` sin hardware/revisión humana. Contrato: [`ALBUM_SYSTEM.md`](ALBUM_SYSTEM.md).

Contenido EditMode cubre IDs, lookup/alias, orden determinista, duplicados, referencias ausentes, localización/audio/visual, trazabilidad y gate Release. PlayMode resuelve `discovery.jungle.keel-billed-toucan` y su alias retirado sin `AssetDatabase`. El catálogo factual del tucán pasa Release; otros subsistemas Draft siguen bloqueando el proyecto Release.

Los conteos, tiempos y artefactos canónicos de la última ejecución integral se registran en [`STATUS.md`](STATUS.md); cifras de fases anteriores no se heredan como evidencia.

DesignSystem EditMode cubre asset/galería/estados, escala 1.25, reduce-motion y target mínimo. El validator está integrado en compile, Addressables y builds Android. La matriz visual es evidencia de layout, no certifica ergonomía, iluminación, VoiceOver/TalkBack ni comprensión infantil; Android físico y playtest siguen `NOT RUN`.

Los wrappers son orquestadores: configuración, validación y build viven bajo `Assets/_Game/Editor/BuildTools`. Los logs sustituyen la raíz del proyecto, home y ejecutable del Editor por marcadores antes de conservarse.

## Diagnóstico y recuperación

Para regenerar Camp: `scripts/setup-camp`. Para validar solo su authoring/wiring: `scripts/validate-camp`. Ambos escriben logs en `artifacts/logs/`; no se debe editar scene YAML o GUIDs manualmente.

1. conservar `artifacts/` y abrir el log del primer comando fallido;
2. buscar `error CS`, `Exception`, `FAILED`, `PE_` o el código indicado;
3. corregir implementación/configuración, no editar `Library` ni artefactos generados;
4. reejecutar primero el comando individual y después `scripts/validate`;
5. si falta Unity/módulo/activación, reportar `NOT RUN` o `BLOCKED` con ese motivo, no `PASS`.

El escaneo de secretos es preventivo y deliberadamente básico; no reemplaza revisión humana ni una herramienta dedicada. `shellcheck` y `actionlint` son útiles cuando estén disponibles, pero no son dependencias del repositorio.

## CI

`.github/workflows/checks.yml` fija Actions oficiales por SHA, usa `contents: read`, desactiva credenciales persistentes y no usa `pull_request_target`. El job estático puede correr en GitHub-hosted; el job Unity solo aparece en `workflow_dispatch`, con variable `UNITY_CI_ENABLED=true` y runner self-hosted etiquetado. Configuración humana: [`GITHUB_SETUP.md`](GITHUB_SETUP.md).

## Cobertura FTUE Prompt 28

- EditMode: definición/orden, evento incorrecto o duplicado, ayuda 6/12 s, gating, skip, replay, version invalidation, round-trip v12 y migración v11→v12.
- PlayMode: elección `Más guía`, visual no lector, checkpoint y recarga con instancia Bootstrap nueva, app pause/resume, ES/EN, secuencia completa y targets en 1024×768, 1280×720, 1600×720 y 1280×800.
- `scripts/validate-tutorial` ejecuta wiring/definition/cues/safe-area/targets; `scripts/validate` conserva la autoridad integral. Touch Android real y comprensión infantil son `NOT RUN` sin hardware/protocolo aprobado.

## Cobertura de integración Prompt 29

EditMode añade dos regresiones de autosave: una preferencia se aplica sobre el último checkpoint pending y un snapshot in-flight sigue siendo autoritativo hasta completar el write. PlayMode recorre únicamente controles normales: Camp→Selva, movimiento, auto-approach, error amable+pista, actividad ES/EN, foto, discovery, 4 estrellas únicas, misión autoactivada, pause/resume, Camp, álbum, cuatro ratios, mejora por 3 estrellas, segunda sesión sin FTUE obligatorio, tres recapturas, flush/reload y recuperación desde backup tras truncar el primary.

La evidencia final es EditMode `167/167`, PlayMode `29/29` y marker `PE_VERTICAL_SLICE_P29`. El journey reportó `7.947 s`, 30 frames batch a `5285.2 FPS` orientativos y delta de memoria global `-13,770,131` bytes; estas cifras solo demuestran que el harness midió y no sustituyen profiling Android. APK, touch, FPS/memoria/térmicas y cinco recorridos humanos en dispositivo continúan separados; hardware fue `NOT RUN` porque `adb devices -l` no listó dispositivo.

## Cobertura de serialización runtime Android

`RuntimeSceneSerializationValidationService` inspecciona todas las escenas habilitadas en `EditorBuildSettings` durante compile/validación/build. `SCENE002` rechaza documentos YAML `!u!115 MonoScript` incrustados y `SCENE003` rechaza referencias locales `m_Script`; ambos requieren un `.cs` homónimo con GUID externo. EditMode prueba una fixture inválida controlada y la escena Bootstrap real.

La reparación 2026-08-17 pasó `scripts/validate` con EditMode `169/169`, PlayMode `29/29` y APK Development. El APK se instaló sin limpiar datos y llegó a `ApplicationReady`/Camp en HONOR DNY-NX9 Android 16, sin `level0 is corrupted`, out-of-bounds, `SIGTRAP` ni fatal. Un build exitoso no sustituye esta prueba de Player. El segundo force-stop/rearranque quedó `NOT RUN` al desconectarse el dispositivo y se conserva pendiente en la matriz física.

## Gate B físico 2026-08-17

El APK exacto `5c382e6c3340f569350ef9ee765566fd0f0377d9403b847df5ae411c33253b80` fue extraído del HONOR DNY-NX9 y comparado byte a byte. Cinco startups entregados y cinco rechecks independientes pasan Ready→Camp sin fatal. Offline boot, Back→pausa y background/resume pasan solo a nivel parcial.

El Gate falla: capturas muestran roots/paneles incompatibles simultáneamente visibles/raycastables; SceneFlow llega a Expedition pero Selva queda oculta, un tap de locale activa Tutorial y rotación en caliente deja el framebuffer negro. Los tests actuales validan vistas aisladas y ratios sintéticos, no composición de framebuffer, exclusión de roots ni overlap de hitboxes. Añadir esas regresiones y repetir hardware antes de playtest. Evidencia y matriz: [`audits/GATE_B_2026-08-17_PHYSICAL_AND_CHILD_UX.md`](audits/GATE_B_2026-08-17_PHYSICAL_AND_CHILD_UX.md).

## Regresiones de composición UI/lifecycle Gate B

EditMode recorre todos los `AppUiState`, exige como máximo un primario, mapping de input y política Back exhaustivos, Camp fail-closed y Tutorial/diagnostics solo en coexistencias autorizadas. El validador integrado a compile/build abre Bootstrap y exige un coordinator, un EventSystem, trece bindings únicos, Canvas/CanvasGroup/GraphicRaycaster exclusivos y cero roots legacy en Camp.

PlayMode aplica la policy a trece roots controlados y verifica `alpha`, interacción, raycasts y raycaster. La escena real prueba Camp→Selva→Camp, EventSystem único, Camp oculto durante Expedition y reflow/reactivación de cámara en 4:3, 16:9, 20:9 y tablet 16:10. La suite además conserva los tres ciclos SceneFlow, pause/Back, ES/EN/pseudo, tutorial, journey y teardown; una carrera encontrada entre `PhotographyView.OnDestroy` y una `LearningActivityView` ya destruida quedó cubierta con guards de shutdown.

Estas regresiones prueban composición y resize simulado, no touch, framebuffer, cutout, térmica ni comprensión infantil reales. Esas filas solo cambian con el APK identificado y evidencia física/humana requerida por el ExecPlan activo.
