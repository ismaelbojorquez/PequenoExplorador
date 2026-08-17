# Changelog

## Toucan visual review fixture — 2026-08-16

### Added

- Prefab propio reproducible `VS_ToucanPicoCanoa` con 16 primitives, siete materiales URP compartidos, collider amplio, interaction point, photo anchor/bounds candidatos y metadata `Sourced`.
- Generator Editor idempotente, ledger de provenance con hashes, validator Development/Release, wrapper local y tres tests EditMode adicionales.
- Seis renders ignorados para revisión humana en 16:9/20:9 y métricas de geometría/memoria Editor.

### Changed

- Jungle sustituye solo la cápsula visual del interactable `interaction.fixture.animal`; conserva discovery neutral, planta, objeto, comportamiento y save sin migración.
- Pipeline/documentación distinguen el asset visual propio de las definitions Draft y mantienen Prompt 19 bloqueado.

### Verified

- Baseline previa completa: repository/compile/Addressables, EditMode `103/103`, PlayMode `18/18` y APK Development.
- `scripts/validate` final pasó en `1:27.92`: compile/validators, Addressables, EditMode `106/106`, PlayMode `18/18` y APK Development.
- APK `105,939,602` bytes, SHA-256 `7ee333517ecbff71bf3a47a74eab189460c87bacb7f0f709171326b6b06bbdd3`, API 26/36, IL2CPP/ARM64 y zipalign 16 KB; manifest sin `CAMERA`/micrófono/ubicación/contactos/`AD_ID`.
- `scripts/build-android-release` terminó con código esperado `2`; el log contiene `TOUCAN019` y no produjo Release.

### Still blocked

- Especialista factual y firma visual asset-specific de Ismael Bojórquez. No se añadió fotografía, audio/animación final, fact/discovery aprobado, permiso sensible, paquete, red, signing, push ni publicación.

## Human review record H-007 — 2026-08-16

### Recorded

- Ismael Bojórquez, competencia/autoridad declarada `Creador`, aprobó claims, Product/Education y Localization de `VS-D-A01`; eligió `Tucán pico canoa` / `Keel-billed Toucan` y excluyó conservación del Vertical Slice.
- Declaró Art, Audio, Rights y QA aprobados con referencias propias. El expediente conserva el alcance humano sin atribuir competencia de especialista factual ni aprobación a assets inexistentes.

### Still blocked

- Especialista factual y aprobación asset-specific siguen pendientes. El visual no-`PH_`/ledger se añadió después; discovery e interacción permanecen Draft neutrales y Prompt 19 no se desbloquea todavía.

## Content sourcing H-007 — 2026-08-16

### Added

- Expediente `VS-D-A01` para `Ramphastos sulfuratus`: seis fuentes institucionales/académicas, ocho claims atómicos, copy ES/EN propuesto, límites, conflictos y checklist de firmas humanas.
- IDs documentales reservados para discovery/facts futuros y matriz de qué arte, audio, actividad, foto, álbum y localización vuelve a revisión si cambia especie/claim.

### Changed

- Fuentes, MVP, discovery, modelo/pipeline, arte, audio, decisiones, riesgos, índice y status distinguen `Sourced` de `Reviewed/Approved/ReleaseLocked`.
- Se preservan dos incertidumbres: `pico canoa` para México vs. `pico iris` para Costa Rica/Panamá, y ADW histórico `LC` vs. CONABIO/IUCN 2025-2 `NT`.

### Verified

- Baseline antes de editar: `scripts/validate` código `0` en `1:29.98`; EditMode `103/103`, PlayMode `18/18` y APK Development.
- Assets Unity, catálogo, localización y placeholders permanecen sin cambios; el expediente no autoaprueba contenido ni concede derechos de media.

## Gate B — 2026-08-16

### Audited

- Se contrastó la expectativa de Fase 29 con Git, `STATUS`, historial, último ExecPlan, runtime y tests: HEAD continúa en Prompt 18 y no existe el loop end-to-end.
- `scripts/validate` pasó en `1:25.81` con EditMode `103/103`, PlayMode `18/18` y APK Development; se registra como salud de foundation, no como evidencia Gate B.
- Gate B queda `FAIL`: fotografía, álbum, economía, misiones runtime, learning, mejora de Camp, tutorial y journey automatizado están ausentes; el animal continúa Draft y sin aprobación factual.

### Not run

- Journey completo, cinco runs sin debug, primera/segunda sesión, no lector, corrupt-save integrado, ES/EN/ratios del slice y Android físico. No se convirtieron en `PASS` ni se crearon sistemas dentro de la auditoría.

## Prompt 18 — 2026-08-16

### Added

- `DiscoveryGrantId`, `DiscoveryProgress`, outcomes explícitos, `DiscoverUseCase`, repository sobre `PlayerProgress` y queries por world/category derivadas del catálogo Approved.
- Save DTO v4 y migración pura v3→v4: IDs históricos pasan a count 1 sin fecha/grant inventadas; checksum, backup, future read-only y serializer builtin se conservan.
- Acción data-driven `interaction.fixture.animal → discovery.jungle.placeholder`, feedback ES/EN nuevo/repetido y contador técnico Development.
- Tests first/repeat/idempotency/missing/unapproved/removed/denominadores/migración y PlayMode interact→flush→reload Selva→repeat.

### Changed

- `AutosaveCoordinator.Latest` mantiene el snapshot pendiente como autoridad durante debounce y evita que features partan de `ISaveService.Current` obsoleto.
- `InteractionCoordinator` conserva feedback/cue semántico de la acción concreta; planta/objeto permanecen neutrales y el núcleo no hardcodea categoría animal.
- Arquitectura, discovery, save, content model, localización, testing, decisiones, riesgos, índice y estado reflejan schema v4 y la frontera previa a fotografía/álbum/economía.

### Verified

- Baseline Prompt 17: `scripts/validate` código `0` en `1:17.19`, EditMode `99/99`, PlayMode `17/17` y APK Development.
- Durante implementación, compile detectó una comparación `WorldId` inválida; EditMode detectó tres asserts incompatibles con `IReadOnlyList`; PlayMode detectó regresión de copy al regenerar localización. Se corrigieron y repitieron los comandos dirigidos.
- `scripts/validate` final repetido tras revisión de inmutabilidad/wiring y limpieza de YAML: código `0` en `2:12.27`; repository/compile/validadores, Addressables local 41 locations/896,909 bytes, EditMode `103/103` (2.141 s), PlayMode `18/18` (11.757 s) y APK Development.
- APK `105,942,573` bytes, SHA-256 `abaa634d2e6ea10bcd84d8badd7480610ed4666c42a74a3fc4dad469f8eb2274`, `59.322 s` Unity, API 26/36, IL2CPP/ARM64 y zipalign 16 KB. Permisos: `INTERNET` heredado + permiso interno receiver; sin cámara/micrófono/ubicación/contactos/`AD_ID`.

### Not added

- Cámara física o in-game, miniaturas, álbum, economía/reward concreto, contenido factual/final, SDK/red/permiso, signing, push o publicación. Android físico sigue `NOT RUN`; `adb devices` no listó hardware.

## Prompt 17 — 2026-08-16

### Added

- `InteractionId`, definitions/context/result y catálogo readonly; selector determinista prioridad/distancia/ID y coordinador de foco, approach, cooldown, idempotencia, cancelación y lifecycle.
- Authoring/validator y tres fixtures neutrales `PH_` animal/planta/objeto sobre el mismo `WorldInteractableView`, con colliders, puntos NavMesh, indicador y metadata ReleaseBlocked.
- Detector `RaycastNonAlloc` indexado, prompt safe-area localizado ES/EN, feedback audio existente y tests EditMode/PlayMode de overlap, rango, spam, unavailable, UI, destroy y unload.

### Changed

- El tap Explorer intenta primero interacción contextual y, si no hay target, conserva tap-to-move; Bootstrap compila el catálogo y enlaza una raíz por escena sin lookup por frame.
- Presentation añade la referencia Domain explícita requerida por el `InteractionId` público; allowlist mantiene nueve assemblies y cero ciclos.
- Gameplay, contenido, UX, input, arquitectura, testing, assets, decisiones, riesgos, contrato e índice describen la frontera anterior a discovery.

### Verified

- Baseline Prompt 16 `scripts/validate` código `0` en `80.40 s`: compile, Addressables, EditMode `94/94`, PlayMode `14/14` y APK `80,860,082` bytes.
- Iteración dirigida: dos fallos de compilación por referencias/using y un validator de keys detectaron wiring incompleto; EditMode detectó resultado `Missing` incoherente y PlayMode detectó referencia UI destruida/cadena YAML sin comillas. Todos se corrigieron y repitieron.
- `scripts/validate` final repetido tras revisión de extensibilidad, código `0` en `2:08.77`: repository/compile/validators, Addressables 41 locations/896,715 bytes, EditMode `99/99` (1.904 s), PlayMode `17/17` (7.417 s) y APK Development.
- APK `80,931,145` bytes, SHA-256 `752b0fd41eb0558d6fa162d8fa8137cde46c08350645842d56f3ff508fe8a4f4`, `59.545 s` Unity, API 26/36, IL2CPP/ARM64, zipalign 16 KB y siete ELF LOAD `0x4000`. Solo `INTERNET` heredado + permiso interno; sin cámara/mic/ubicación/contactos/`AD_ID`.
- Release guard falló de forma esperada antes de signing: `INTERACTION005` bloqueó los tres fixtures Draft/placeholder junto con los bloqueos de contenido/mundo existentes.

### Not added

- Discovery, fotografía, aprendizaje, recompensa, progreso nuevo, contenido factual/final, SDK/red/permiso, signing, push o publicación. Touch/ergonomía/FPS en Android físico siguen `NOT RUN` porque `adb devices` no listó dispositivo.

## Prompt 16 — 2026-08-16

### Added

- AI Navigation `2.0.9` oficial fijado; `ExplorerLocomotionController`/`IPathNavigator` puros y adapter NavMesh/raycast en Presentation.
- Prefab, materiales, geometría, NavMeshData, markers, bob y luz `PH_`; cámara asistida con bounds/damping/reduce-motion y cancelación por mapa/lifecycle/unload.
- Setup idempotente, validator de package/prefab/scene/camera/tuning y pruebas de comandos, inválidos, spam, pausa/mapas, tres ciclos, unload y FPS Editor.

### Changed

- `IInputService` publica cambio de mapa; Bootstrap enlaza/desenlaza una única raíz de explorador al cargar/soltar Selva sin lookup por frame.
- Presentation/Editor/tests añaden solo referencias AI Navigation/AIModule necesarias; allowlist conserva nueve assemblies y cero ciclos.
- Jungle sigue stub, pero ahora es un claro navegable técnico; la escena permanece YAML y su NavMeshData nativo usa atributo binario explícito.

### Verified

- Baseline previa `scripts/validate` PASS en `1:18.90`: EditMode `89/89`, PlayMode `11/11` y APK Development.
- Iteración dirigida: compile/validator PASS, EditMode `93/93`, PlayMode `14/14`; un fallo inicial de estado de tap inválido fue corregido y repetido.
- `scripts/validate` final código `0` en `4:23.73`: repository/compile/validator, Addressables 41 locations/893,594 bytes, EditMode `94/94` (1.719 s), PlayMode `14/14` (4.876 s) y APK.
- APK repetido tres veces con tamaño estable `80,860,082` bytes; último hash staged `1d04f40d81a34794d18949ccc562cb768ffcd6afdd69a5df2bfb898e909b7e79` (`49.551 s` Unity/`68.07 s` wrapper), API 26/36, IL2CPP/ARM64, zipalign 16 KB y siete ELF LOAD 16384. Solo `INTERNET` heredado + permiso interno receiver; sin cámara/mic/ubicación/contactos/`AD_ID`. El hash Development varía por metadatos/firma temporal, por lo que no se afirma reproducibilidad bit a bit.
- El tamaño aumentó `14,386,460` bytes (`21.64%`) frente a Prompt 15; se registra como deuda de profiling, no se oculta como PASS de budget.

### Not added

- Interacción contextual, discovery, fotografía, joystick, root motion, controller tercero, arte/animación final, preferencia reduce-motion persistida, SDK/red/permiso, signing, push o publicación.

## Prompt 15 — 2026-08-16

### Added

- `WorldManifest` readonly y authoring `WorldManifestAsset` con `world.jungle`, versiones, escena, labels, spawn/checkpoint, catálogos, cues, requirements, tamaño estimado y metadata editorial.
- `IWorldCatalog`, índice O(1), disponibilidad local, `IWorldSession` y `WorldLoadUseCase`; selector Camp toma Selva del catálogo y muestra respuestas ES/EN para locked/missing.
- Validator Editor/CLI con reportes JSON/Markdown, marker `spawn.jungle.entry` y fixture in-memory `world.test-ocean` que usa el coordinador sin modificarlo.

### Changed

- `SceneContentId` dejó de ser enum Camp/Jungle: ahora es address semántico `scene/*`; Addressables conserva ownership único y retry por contenido.
- Bootstrap compila/injecta catálogos Content/World, conserva la sesión de mundo y reproduce cues aportados por manifest; Content añade solo la referencia oficial Addressables ya fijada.
- Arquitectura, world design, content model/pipeline, testing, decisiones y riesgos reflejan mundos local-only sin entitlement, download ni switch central.

### Verified

- `scripts/validate` final código `0` en 120.04 s: repository/compile/validators, Addressables local, EditMode `89/89` (1.670 s), PlayMode `11/11` (4.321 s) y APK Development.
- Addressables `4.0.1`: 41 locations, 15 archivos, 821,576 bytes, `LocalDevelopment`, `remoteCatalog=false`; tres ciclos Selva y cero handles tras shutdown.
- APK `66,473,622` bytes, SHA-256 `07cd4ad69994f79790c0f8ea14c985c63d05a357e333cce466d29b3c1ec75c9c`, API 26/36, IL2CPP/ARM64. Manifest sin cámara, micrófono, ubicación, contactos ni `AD_ID`; conserva `INTERNET` heredado.
- World report Development `PASS` con `world.jungle`; Release `FAIL` esperado `WORLD018` por Draft/PH_, no se convirtió en PASS.

### Not added

- Gameplay, personaje, otro mundo real, download/remote catalog, entitlement/SKU, cambio de save, assets finales, SDK, permiso sensible, signing, push o publicación.

## Prompt 14 — 2026-08-16

### Added

- Value IDs namespaced para discovery/category/tag/fact/source/world/mission/activity/reward/visual; sin GUID como lógica.
- Definitions readonly, contratos mínimos y `IContentCatalog` indexado O(1) con aliases.
- Authoring ScriptableObject, metadata editorial, generador que no sobrescribe, help boxes y catálogo neutral Draft `PH_`.
- Validator Development/Release con paths/soluciones y reportes JSON/Markdown; casos EditMode/PlayMode de mapping y resolución.

### Changed

- Bootstrap compila e inyecta el catálogo; build Development valida Draft con watermark y Release lo rechaza antes de signing.
- Content/Editor referencian Domain explícitamente; el grafo sigue en nueve assemblies y sin ciclos.
- Localización suma `content.discovery.placeholder.name`; requisitos/pipeline/educación documentan trazabilidad por ID.

### Verified

- Pipeline final `scripts/validate` código `0` en `3:51.84`: compile/catalog, Addressables, EditMode `85/85` (1.596 s), PlayMode `11/11` (4.278 s) y APK Development.
- APK `66,353,374` bytes, SHA-256 `d244ba03df0ab7c8699b012a9f6a484c63db4496ce7437f6f59c620f49298cea`, API 26/36, IL2CPP/ARM64; sin permiso sensible nuevo.
- Report Development `PASS` con 1 category/tag/source/fact/discovery; report Release `FAIL` esperado con cinco `DATA025` y paths accionables.

### Not added

- Discovery final, hecho de tucán, gameplay, reglas mission/activity/reward, contenido masivo, remote catalog, dependencia, save schema, signing o publicación.

## Prompt 13 — 2026-08-16

### Added

- `IInputService`, `ISafeAreaService` e `IHapticsService`; clasificador C# puro de tap/hold/drag/pinch con cinco slots y cancelación determinista.
- Action maps `UI`, `Explorer`, `Photography`, `Parents` y `Debug`; thresholds Content, adapter Input System único y driver de lifecycle.
- Safe area central por Canvas, pausa Back con checkpoint/copy ES-EN, overlay touch/viewport Development y haptics no-op/off.
- Validator build de mapas, APIs, wiring, targets 64×64 y fitters; tests EditMode y PlayMode `InputTestFixture` para 4:3, 16:9, 20:9 y 16:10.

### Changed

- Bootstrap selecciona UI en Camp/transición/pausa y Explorer en Expedition; AppContext/lifecycle exponen input, safe area y haptics sin locator.
- Dos nuevas keys localizadas; los locale buttons diagnóstico existentes se ampliaron para cumplir target táctil.
- Arquitectura, UX, testing, decisiones, riesgo, índice, contrato y estado reflejan adaptación móvil sin gameplay.

### Verified

- Baseline previa `scripts/validate` PASS: EditMode `70/70`, PlayMode `7/7` y APK Development.
- Pipeline final repetido tras documentación/cleanup: `scripts/validate` código `0` en `1:18.93`; compile/input validator, Addressables, EditMode `77/77` (1.735 s), PlayMode `10/10` (4.007 s) y APK Development.
- APK `66,067,652` bytes, SHA-256 `c19c68eacf50dfb61916c1eaa09c1c787bf452fd12f7e15784206fe898600d26`, build cache `15.654 s`, API 26/36, IL2CPP/ARM64; manifest sin cámara, micrófono, ubicación, contactos ni `AD_ID`.
- Release guard devolvió el código esperado `3`; hardware Android `NOT RUN` porque `adb devices` no listó dispositivo.

### Not added

- Movimiento, joystick, cámara/fotografía final, UI final, vibración física, Device Simulator como dependencia, SDK, permiso, red, signing, push o publicación.

## Prompt 12 — 2026-08-16

### Added

- `IAudioService`, IDs/categorías/buses/prioridades, settings, resultados y subtítulos BCL-only; mixer Master/Music/Ambience/Effects/Voice y servicio Unity con siete sources acotados.
- Cola Voice priorizada/FIFO, cooldown, ducking 0.35×, replay, pause/focus/shutdown y panel exclusivamente Development.
- Siete cues Content, diez WAV mono/48 kHz `PH_` generados internamente, addresses/labels locales y validator `scripts/validate-audio` con bloqueo Release.
- Save schema v3 y migración pura v2→v3 para cinco volúmenes/subtítulos; tres keys de subtítulo ES/EN y slots Voice conceptuales.

### Changed

- Composition root inicializa Save→Localization→Audio antes de servicios comerciales; AppContext expone el puerto, no un locator.
- Addressables `SharedLocal`, arquitectura, localización, save, UX, testing, dependencias, decisiones y riesgos reflejan audio offline y sin SDK.

### Verified

- Pipeline `scripts/validate` PASS: repository/compile/content/audio/Addressables, EditMode `70/70` y PlayMode `7/7`.
- APK Development `66,037,223` bytes, SHA-256 `9564026c1dae24c69d3f96ff4ac46650267a2fad9f2677c63a9ddacc614ec046`, API 26/36, IL2CPP/ARM64 y zipalign 16 KB; Release guard devolvió el código esperado `3`.
- Los clips no exceden el umbral de clipping del validator; un cue ausente retorna `Missing` y los placeholders se registran como pendientes, nunca aprobados.

### Not added

- Voz humana, música/ambiente/SFX final, claims animales, middleware, red, micrófono, remote content, gameplay, signing, push o publicación.

## Prompt 11 — 2026-08-16

### Added

- Unity Localization `1.5.12`, locales ES/EN, pseudo Development y tablas `Shared`/`UI`/`Content` más slots `Voice`/`Illustrations`.
- `ILocalizationService`, keys namespaced, cambio live/persistido, fallback seguro por perfil, Smart Strings y selector diagnóstico.
- Save schema v2 con migración v1→v2, validator CLI, CSV export/import, tests ES/EN/pseudo y builds Android duales.

### Changed

- Todo texto visible actual migra desde escenas/Presentation a tablas; Bootstrap inicializa Localization después de Save.
- Addressables incluye solo grupos Localization locales; arquitectura, UX, audio, contenido, dependencias, testing, riesgos y estado reflejan la implementación.

### Verified

- Compile/import, validator, Addressables local, EditMode `62/62`, PlayMode `6/6`, pseudo en dos resoluciones y APK Development ES/EN IL2CPP/ARM64 API 26/36.
- CSV exportó tres tablas; APK no mostró permisos sensibles/`AD_ID` y pasó `zipalign -P 16`. Dispositivo físico quedó `NOT RUN`.

### Not added

- Gameplay, traducción masiva/final, narración humana, assets finales, selector parental de producto, red, remote catalog, SDK comercial, signing o publicación.

Todos los cambios notables de ingeniería se registran aquí. La versión técnica de desarrollo es `0.1.0-dev`; no representa un release comercial.

## Prompt 10 — configuración runtime local — 2026-08-16

### Added

- `IAppConfig`/`IFeatureFlags`, IDs estables y defaults tipados en Application; dos ScriptableObjects locales Development/Release y mapping/catálogo en Content.
- Loader único Bootstrap, override disposable solo Editor, validador Editor/CLI y guardrails que rechazan todo flag Release.
- Tests de defaults, mapping, budgets, duplicados, seis flags inseguros, perfil PlayMode y fallo real `CONFIG008`.

### Changed

- Seed, producto/versión, timeout scene flow, debounce autosave y selección de diagnóstico/mocks/fallo simulado dejaron de estar hardcodeados en Bootstrap.
- Build Release valida configuración antes de conservar su bloqueo de signing; build-time, Addressables y preferencias Save permanecen separados.
- La referencia Editor→Application/Content se hizo explícita; usos de `UnityEngine.Application` quedaron calificados para evitar colisión de namespace.

### Verified

- Primer compile incremental `FAIL` por colisión del namespace `Application`; corrección explícita y repetición `PASS`. No se ocultó el intento.
- Fixture controlada Release+`MockAds`: `scripts/compile` devolvió `2` con `CONFIG008`; asset restaurado y pipeline final `PASS`.
- `scripts/validate` código `0` en 3:27.34: repository/config checks, compile, Addressables local, EditMode `57/57`, PlayMode `5/5` y APK Development.
- APK post-commit `60,310,101` bytes, SHA-256 `94d56d67b7f24055630f81603beea46cb0bf4ff934cd3da143d3177502075802`, manifest ligado al commit de feature, API 26/36, IL2CPP/ARM64; sin permisos sensibles/`AD_ID`.
- Release unsigned validó perfiles/local-only y devolvió el código esperado `3` por signing. Dispositivo Android `NOT RUN` por `adb devices` vacío.
- La comprobación post-commit detectó `origin` ya configurado; se corrigió la afirmación documental obsoleta. No hubo push ni inspección/ejecución remota.

### Not added

- No remote config, red, secretos, gameplay, tuning ficticio, paquete, SDK, permiso, signing, push o publicación.

## Catálogo maestro de prompts — 2026-08-16

### Added

- Cadena documental completa de 58 prompts ejecutables, numerados 00–57, con Gates A–F, preflights, criterios de aceptación y reportes requeridos.
- Enlace canónico desde el índice documental y regla de reanudación: `STATUS.md` selecciona el prompt siguiente y la cadena no sustituye evidencia Git/tests.

### Verified

- Secuencia continua 00–57, 58 marcadores de inicio y 58 de fin; Markdown, enlaces, secretos y whitespace se validan antes del commit.

### Not added

- No se modificó código, configuración Unity, paquetes, gameplay, assets, binarios, secretos, signing, push o publicación.

## Prompt/Fase 09 — persistencia local — 2026-08-16

### Added

- `PlayerProgress`/preferencias C# puro y puertos `ISaveService`, `IFileStore`, migración y autosave sin exponer formato a features.
- Schema v1 `JsonUtility` builtin: envelope, SHA-256, DTOs, secuencia técnica, v0→v1, future read-only y archivos primary/backup/temp bajo `persistentDataPath`.
- Escritura temp/flush/replace, recuperación que preserva backup, copy no alarmante y menú Editor Development de inspect/reset con confirmación.
- Failpoints in-memory, replace físico y PlayMode de recreación/recarga; documentación de privacidad y recuperación manual.

### Verified

- Baseline previo: compile, Addressables, EditMode `29/29`, PlayMode `4/4` y APK Development.
- Suite incremental: compile `PASS`, EditMode `46/46` y PlayMode `5/5`. El primer compile detectó referencias asmdef directas faltantes y la primera ejecución EditMode falló `1/43` por expectativa demasiado estricta `OperationCanceledException` vs subtipo `TaskCanceledException`; ambos intentos permanecen registrados y las repeticiones pasaron.
- `scripts/validate` final código `0`; APK Development `60,278,339` bytes, SHA-256 `523ff0d5debf5974643e4106eb8d0743ee03ffdd82e2f9ef4ef6adaf9728e011`, API 26/36, IL2CPP/ARM64. Release guard devolvió el código esperado `3`; dispositivo Android físico `NOT RUN` por `adb devices` vacío.

### Not added

- Cloud, cuenta, sincronización, cifrado, PII, entitlements, gameplay, contenido real, SDK, red, permiso, signing, push o publicación.

## Gate A — revalidación posterior a F07 — 2026-08-15

### Changed

- Retirado del control de versiones `Assets/AddressableAssetsData/link.xml` y su `.meta`: Addressables los elimina/regenera durante builds y cambia el GUID; ahora se ignoran como salida transitoria para que una validación no altere fuentes.
- Status, riesgo y auditoría distinguen el Gate A original de esta revalidación sobre el HEAD F07 real.

### Verified

- Baseline aislado: compile, EditMode `29/29` y PlayMode `4/4`; el test PlayMode repite Camp→Jungle→Camp tres veces, conserva un único bootstrap/handle y libera al cerrar.
- `scripts/validate` código `0` en 109.43 s y segundo `scripts/build-android-development` código `0` en 62.22 s.
- APK run 1: `41,722,038 bytes`, SHA-256 `03df45c6f5bfaaa9e54a56027d04bd85b88a6a9d9d03214da66d05efb1fe61ae`; run 2: `41,722,037 bytes`, SHA-256 `0a9ae311635ec636e4d70057d6c4a6a8c861d60727ed4d0571fbdc20930bb1e1`.
- Segundo APK: API 26/36, IL2CPP/ARM64, siete ELF con `LOAD 0x4000`, `zipalign -P 16`, Addressables local incluido y sin cámara, micrófono, ubicación, contactos, storage o `AD_ID`.
- Build Addressables posterior a GAR-001: código `0` en 44.11 s; regeneró ambos archivos como ignorados y no añadió diff unstaged.
- La comprobación adicional en emulador API 37/16 KB fue `INCONCLUSIVE`: el sistema invitado produjo ANR de System UI/teléfono/servicios Google antes del primer frame estable. Unity documenta que los emuladores Android no están soportados; no se atribuye ese intento al APK ni se cuenta como PASS.

### Not run

- AAB Release/firma, dispositivo Android físico, CI Unity remota e iOS; permanecen bloqueados o sin toolchain/infraestructura humana. No se añadieron gameplay, SDKs, permisos sensibles, secretos, signing, push ni publicación.

## Fase 07 — incremento scene flow local — 2026-08-15

### Added

- Máquina Application explícita `Boot → Camp ↔ Expedition`, exclusión mutua, progreso, retry, cancelación, timeout y resultados técnicos sin gameplay.
- Adapter Addressables con owner único/idempotente de handles, escenas placeholder `Camp`/`Jungle`, transición uGUI Development y fallo simulado recuperable.
- Addressables `4.0.1` fijado con perfiles `LocalDevelopment`/`LocalRelease`, grupos `SharedLocal`/`JungleLocal`, labels y catálogo exclusivamente local.
- Validador de perfiles/grupos/paths/labels/no endpoint/no `Shared → Jungle`, build local reproducible y tests EditMode/PlayMode de lifecycle.

### Verified

- Import/compile y Addressables Android local: 6 locations, 8 archivos, 181,040 bytes en runtime output, sin remote catalog.
- `scripts/validate` final, código `0`: checks, compile, Addressables, EditMode `29/29` (28 del proyecto + stub documental del paquete) y PlayMode `4/4`; incluidos tres ciclos y cero handles tras shutdown.
- APK Development offline en emulador 16 KB: 41,722,038 bytes, SHA-256 `789d8342fd9af78151e10b55c566c7778e752d6bbadc07badbe9f473a6d3c29c`; catálogo/bundles locales incluidos, API 26/36, ARM64/IL2CPP, `Boot→Camp` sin red.
- Un primer pipeline final devolvió código `2`: el tercer ciclo PlayMode agotó un límite de frames no representativo en batchmode. El test se corrigió a deadline monotónico de 20 s; ejecución aislada y pipeline completo posteriores pasaron. No se ocultó ni contabilizó aquel intento como `PASS`.

### Not added

- Gameplay, save, remote catalog, CDN, descarga, backend, SDK comercial, permiso sensible nuevo, signing, push o publicación.

## Gate A — 2026-08-15

### Added

- Auditoría independiente de foundation con matriz de severidad, comandos, artefactos, permisos, licencias y riesgo infantil.
- Dos tests EditMode para shutdown durante inicialización y cleanup del servicio que falla antes de un retry recuperable.

### Changed

- `ApplicationHost` posee la cancelación de inicialización: shutdown ya no puede volver a `Ready` y el servicio en curso recibe cleanup aun si falla o ignora temporalmente el token.
- Registro de política 16 KB alineado con la fuente oficial actualizada 2026-08-05 y deadline de updates 2027-02-01.
- Arquitectura, testing, riesgos, matriz de versiones, índice y status reflejan evidencia Gate A; no cambió ninguna decisión ADR.

### Verified

- `scripts/validate`: código `0` en 9:11.06 con recompilación IL2CPP; EditMode `21/21`, PlayMode `2/2` y APK Development.
- APK final: `57,079,091 bytes`, SHA-256 `8710f8ccf27489fa72ec9b9130014e0ec0f79fdadea621edf670189c582fb22f`, min/target/compile 26/36/36 y ARM64/IL2CPP.
- Manifest sin permiso sensible/`AD_ID`; `zipalign -P 16` y siete ELF `0x4000`; instalación y `Ready` en emulador `PAGE_SIZE=16384` con backcompat `fatal`.
- Guard Release: código esperado `3` sin build/firma. CI remota, AAB firmado e iOS continúan `NOT RUN`.

### Not added

- Gameplay, features, save, SDKs, paquetes, permisos, signing, AAB, secretos, remoto, push o publicación.

## Fase 06 — 2026-08-15

### Added

- `ApplicationHost` BCL-only con inicio secuencial, cierre inverso, cancelación, retry e idempotencia; `AppContext` inmutable y puertos mínimos de reloj, azar, logs, mensajes, analytics, ads y compras.
- Implementaciones locales `Null`, `Mock`, `NoAds` y `Unavailable`, bus con suscripciones desechables y fixtures deterministas de clock/random.
- Composition root único en `DiagnosticBootstrap`, registro tipado privado, selección Development/Release y vista recuperable de estado `Ready`/error.
- Cobertura EditMode de lifecycle, perfiles, servicios y listeners, más PlayMode de arranque/reload sin duplicados.

### Changed

- El APK Development recibe `PE_DEVELOPMENT_SERVICES` solo mediante `BuildPlayerOptions`; PlayerSettings y Release permanecen sin el símbolo y los mocks quedan fuera de compilación Release.
- La escena Bootstrap conserva el diagnóstico temporal, ahora cableado explícitamente a Presentation y sin búsqueda global ni persistencia dispersa.
- Arquitectura, estándares, testing, decisiones, riesgos, roadmap, AGENTS, README e índice documentan orden, perfiles y siguiente F07.

### Verified

- Validación final `scripts/validate`: código `0` en 1:14.27 con caché; checks, compile, EditMode `19/19`, PlayMode `2/2` y APK Development.
- APK: `57,069,510 bytes`, API 26/36, IL2CPP/ARM64 y `zipalign -P 16` correcto; BuildTools registró SHA-256 y commit de entrada en el manifest ignorado de cada ejecución.
- Manifest: solo `INTERNET` y permiso interno de receiver; no cámara, micrófono, ubicación, contactos ni `AD_ID`. `scripts/build-android-release` confirmó el guard esperado con código `3`, antes de BuildPipeline y sin signing autorizado.

### Not added

- Gameplay, save, scene flow, SDKs, red/telemetría real, ads reales, IAP real, dependencias, permisos sensibles, signing, AAB, push o publicación.

## Fase 05 — 2026-08-14

### Added

- BuildTools Editor para compile/fronteras/contenido, APK Development y Release fail-closed, con reportes de entorno/build y SHA-256.
- Wrappers Bash macOS/Linux, XML NUnit/JUnit, logs sanitizados y un comando completo `scripts/validate`.
- Checks sin dependencias para Markdown/enlaces, JSON/asmdefs, package pins, YAML/Actions, secretos básicos y shell.
- Workflow GitHub con Actions oficiales fijadas por SHA, permisos read-only y job Unity manual/self-hosted; guía humana de GitHub.

### Changed

- Outputs convergen en `artifacts/` ignorado; el smoke legado delega al BuildTools actual.
- Roadmap asigna pipeline a F05, mueve shell/input/contenido y combina AAB con budgets en F12, sin alterar 00–57 ni el alcance Selva.
- README, playbook, Android, decisiones, riesgos, índice y status enlazan comandos reales y distinguen CI `NOT RUN`.

### Verified

- `scripts/validate`: código `0`, checks, compile, EditMode `5/5`, PlayMode `1/1` y APK Development en 2:05.16.
- APK: `57,046,302 bytes`, SHA-256 `3d0a7385023e3c7d4f9772303027de2e448935bacfea73966ef71824f014b479`, min/target/compile 26/36/36, ARM64 y zipalign 16 KB.
- `scripts/build-android-release`: fallo controlado esperado, código `3`, sin signing; CI remota `NOT RUN` por ausencia de remoto/runner.

### Not added

- Gameplay, signing, AAB Release, secretos, remote/push, publicación, SDKs/Actions de terceros o dependencias nuevas.

## Fase 04 — 2026-08-14

### Added

- Nueve assemblies físicos para Domain, Application, Content, Infrastructure, Presentation, Bootstrap, Editor y tests EditMode/PlayMode.
- Markers mínimos de prueba, validador Editor/CLI con allowlist y detección de ciclos, y fixtures inválidas sin romper asmdefs reales.
- Test PlayMode del diagnóstico temporal y arquitectura canónica `02_TECHNICAL_ARCHITECTURE.md`.

### Changed

- Retirado el runtime asmdef monolítico; Editor y tests ahora referencian solo fronteras necesarias, sin `overrideReferences`.
- Roadmap concentra prototipos/playtests de interacción en F07 junto con input; F04 queda limitada a modularidad sin gameplay.
- Estándares, playbook, decisiones, riesgos, README, índice y estado reflejan el grafo ejecutable.

### Verified

- Compile batch código `0`; validador `assemblies=9 cycles=0`; EditMode `5/5`; PlayMode `1/1`.
- APK Development API 26/36 IL2CPP/ARM64: `57,046,302 bytes`, SHA-256 `a4572df93cbcda6aaa07369f5edd0a0e77ca51e3ed1f6dc50fef463b52a4903b`.
- `zipalign -P 16`, instalación/launch en emulador page-size 16384, diagnóstico landscape visible y ausencia de fatal en logcat.

### Not added

- Gameplay, scene flow, save, UI de producto, servicios concretos, SDKs, paquetes, permisos sensibles, assets finales o publicación.

## Fase 03 — 2026-08-14

### Added

- Proyecto Unity `6000.3.22f1` URP mínimo en la raíz, escena temporal `Bootstrap` y estructura `_Game` sin gameplay.
- Paquetes oficiales exactos, lock reproducible, URP móvil, landscape, Input System only y PlayerSettings Android/iOS-ready.
- Build CLI Android con perfiles Debug/Development/Release y código de salida explícito.
- Dos tests EditMode, documentación técnica/release/dependencias y metadata `PH_UI_DIAGNOSTIC`.

### Verified

- Import/compile headless y EditMode `2/2`.
- APK Development API 36, min 26, IL2CPP/ARM64: 57,042,975 bytes y SHA-256 documentado.
- Manifest sin cámara, micrófono, ubicación, contactos o `AD_ID`; 16 KB por zipalign/ELF/emulador y diagnóstico visible en landscape.

### Not added

- Gameplay, save, Addressables, Localization, IAP, ads, analytics, backend, arte/audio final, custom manifest/Gradle, firma, AAB Release o build iOS.

## Fase 02 — 2026-08-14

### Added

- Contrato operativo de agentes con jerarquía de verdad, preflight, límites, evidencia y Definition of Done.
- Plantilla/mantenimiento de ExecPlans vivos y directorio sin planes ficticios.
- Estándares de arquitectura/C#, dependencias, placeholders, review infantil y playbook de validación.
- Estado vivo y prueba de reanudación para sesiones sin memoria de chat.

### Changed

- Roadmap: contrato de agentes ocupa F02; foundation Unity y scaffolding se consolidan en F03.
- Decisiones y riesgos incorporan planes selectivos, evidencia, intake de dependencias, deriva de contexto y placeholders.
- README, índice, matrices y fuentes de política apuntan a la nueva siguiente fase.

### Not added

- Proyecto Unity, C#, asmdefs, paquetes, dependencias, gameplay, assets, tests Unity o builds.

## Fase 01 — 2026-08-14

### Added

- Visión, GDD, loops y contratos de producto data-driven.
- Alcance canónico del Vertical Slice y MVP con cantidades, MoSCoW, dependencias, estados y aceptación.
- Sistemas de mundo, descubrimiento, aprendizaje, misiones, UI/UX, arte y audio.
- Taxonomía educativa, dos modos de guía sin edad, proceso factual bloqueante y plan de playtests.
- Simulaciones de papel para prelector, lector y persona adulta.

### Changed

- Roadmap: Fase 01 pasa a especificación de producto y creación Unity se mueve a Fase 02.
- Decisiones y riesgos incorporan tap-to-move candidato, ad-free, no compulsión y gate de contenido.

### Not added

- Proyecto Unity, código, gameplay, contenido masivo, assets finales, precios, SKUs o textos legales finales.

## Fase 00 — 2026-08-14

### Added

- Repositorio Git en rama `main` y archivos raíz de higiene para Unity.
- Baseline de producto, arquitectura, versiones, políticas, riesgos y roadmap 00–57.
- ADR provisional para Unity `6000.3.22f1`.
- Requisitos iniciales de arte y audio.
- Evidencia de preflight y registro de fuentes oficiales.

### Not added

- Proyecto Unity, `Assets/`, `ProjectSettings/`, C#, escenas, assets, paquetes, SDKs, builds o publicación.
