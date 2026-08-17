# Contrato operativo para agentes

Este archivo es la entrada obligatoria para toda sesión. **Pequeño Explorador: Aprende Jugando** es un juego infantil 4–9, landscape, Android-first, iOS-ready, offline-first y sin backend en el MVP. El único mundo MVP es Selva y su pilar es `acción → descubrimiento → aprendizaje → recompensa`.

## Empezar o reanudar

Antes de editar:

1. leer este archivo completo y [`docs/STATUS.md`](docs/STATUS.md);
2. seguir el orden de lectura que `STATUS` indique y consultar [`docs/README.md`](docs/README.md) para las fuentes canónicas;
3. ejecutar `git status --short --branch`, `git branch --show-current` y `git log -1 --format=fuller`;
4. inventariar e inspeccionar implementación, configuración, pruebas, diff staged/unstaged y artefactos relacionados;
5. contrastar fases previas con archivos y comandos: sus reportes no son evidencia heredable;
6. preservar cambios ajenos y detenerse si una colisión no puede aislarse.

La ausencia de este archivo o de `docs/STATUS.md` bloquea el trabajo. Un estado sucio no autoriza limpiar, sobrescribir ni incorporar cambios ajenos.

## Jerarquía de fuentes de verdad

1. solicitud humana actual y políticas aplicables;
2. este contrato y [`docs/STATUS.md`](docs/STATUS.md);
3. documentos canónicos listados en [`docs/README.md`](docs/README.md);
4. ExecPlan activo, si existe;
5. implementación, tests, configuración y evidencia Git observada.

Una contradicción se resuelve en la fuente canónica y sus referencias, no duplicando reglas. Decisiones humanas/comerciales no pueden cerrarse por inferencia técnica.
Esta jerarquía gobierna intención y alcance; la realidad observable gobierna afirmaciones de estado. Si implementación, tests o Git contradicen un reporte, registrar la desviación y corregir el documento antes de continuar.

## Trabajo incremental y planes

- Limitar cada fase a un resultado revisable, conservar el alcance y revisar el diff completo.
- Crear y mantener un ExecPlan según [`.agent/PLANS.md`](.agent/PLANS.md) para features/refactors complejos, riesgos altos o trabajo de varias sesiones. No usarlo para un cambio trivial y autocontenido.
- Actualizar `docs/STATUS.md` al cambiar fase, siguiente acción, capacidad verificada o bloqueo.
- No depender de memoria del chat: decisiones, comandos, hallazgos y recovery deben quedar en archivos.
- Commits cubren una sola fase/propósito y se hacen tras validar; no push, amend o reescritura de historia sin autorización explícita.

## Arquitectura y convenciones

La dirección es `Domain` C# puro → `Application` → adaptadores `Infrastructure`/`Presentation`/`Content`, unidos por un composition root explícito. `Domain` no referencia `UnityEngine`; `MonoBehaviour` adapta ciclo de vida/entrada/vista y no contiene reglas complejas. Las reglas completas están en [`docs/ENGINEERING_STANDARDS.md`](docs/ENGINEERING_STANDARDS.md).

Layout actual: raíz para controles y proyecto Unity; `Assets/_Game/` contiene Domain/Application/Content/Infrastructure/Presentation/Bootstrap/DesignSystem, tooling Editor y tests; `docs/` contiene fuentes canónicas y `.agent/` planes. Diez asmdefs fijan el grafo de [`docs/02_TECHNICAL_ARCHITECTURE.md`](docs/02_TECHNICAL_ARCHITECTURE.md); no subdividir sin evidencia. Código usa namespaces `PequenoExplorador.<Layer>`, serialización privada explícita, suscripciones/cancelación ligadas al lifecycle y tests deterministas según frontera.

Toda escena runtime debe referenciar cada `MonoBehaviour` mediante un asset `.cs` homónimo/GUID externo. Se prohíben documentos YAML `!u!115 MonoScript` incrustados y referencias `m_Script` locales; los gates `SCENE002/SCENE003` deben permanecer fail-closed.

El pin es Unity `6000.3.22f1`; AI Navigation está fijado en `2.0.9`. Desde la raíz, `scripts/validate` ejecuta checks, compile/validadores, build Addressables local, EditMode, PlayMode y APK Development; `scripts/build-addressables-local`, `scripts/validate-localization`, `scripts/validate-album`, `scripts/validate-economy`, `scripts/validate-missions`, `scripts/validate-learning`, `scripts/validate-camp`, `scripts/validate-tutorial` y `scripts/build-android-locales` permiten pasos aislados. Los comandos individuales están en [`docs/VALIDATION_PLAYBOOK.md`](docs/VALIDATION_PLAYBOOK.md). Los fallos devuelven código no cero y dejan evidencia ignorada en `artifacts/`. No usar otro Editor ni inventar targets/perfiles.

UI runtime usa tokens/componentes locales de `PequenoExplorador.DesignSystem`; `scripts/setup-design-system`, `scripts/validate-design-system` y `scripts/capture-ui-review <fase>` son reproducibles. TMP es canónico para componentes nuevos; el bridge a `UnityEngine.UI.Text` solo preserva vistas serializadas hasta una migración segura. Targets táctiles: mínimo 64 y recomendado 72 unidades lógicas; reduce-motion cancela animaciones y restaura estado neutral.

Camp se define por catálogo/IDs: Bootstrap inyecta acciones existentes y Presentation solo muestra estaciones/variantes. Una mejora debe construir gasto+unlock en un único `PlayerProgress`; nunca introducir IAP, paywall educativo, switch por edificio ni habilitar el área adulta sin parental gate real.

Servicios transversales se declaran como puertos en Application, se implementan en Infrastructure y solo se ensamblan en Bootstrap. `AppContext` se entrega explícitamente y nunca es global; `ServiceRegistry` permanece interno y tipado, sin lookup genérico. El orden canónico es MessageBus → Input → SafeArea → Haptics → Save → Photos → Localization → Audio → Analytics → Ads → Purchases, con shutdown inverso. Save v12 sigue [`docs/10_SAVE_SYSTEM.md`](docs/10_SAVE_SYSTEM.md): features usan repositorios/checkpoints, nunca JSON/filesystem; learning guarda sesión mínima/agregados concepto+día, Camp/personalización guardan IDs y tutorial solo versión/paso/estado, nunca respuestas/taps/tiempos ni definitions. Development puede compilar mocks locales mediante el define exclusivo de build; Release usa NullAnalytics, NoAds y UnavailablePurchase y no contiene selección de simuladores.

Todo texto visible de producción usa `LocalizedKey` y [`docs/17_LOCALIZATION.md`](docs/17_LOCALIZATION.md). Español e inglés son locales de usuario; pseudo, selector diagnóstico y markers de key faltante son solo Development. Domain no contiene copy localizada, Presentation no lee tablas y Save persiste solo la preferencia adulta. No concatenar frases localizables ni activar locale de sistema, red o catálogo remoto.

Configuración runtime sigue [`docs/RUNTIME_CONFIGURATION.md`](docs/RUNTIME_CONFIGURATION.md): Content posee exactamente un `AppConfigAsset` Development y uno Release, Bootstrap es el único loader y Application consume `IAppConfig` readonly. Todo flag es local/tipado; Release lleva cero flags y prohíbe diagnóstico, mocks, fallo simulado, cheats y bypass parental. Build-time, Addressables/content-time y preferencias de Save son autoridades separadas. No usar remote config, `Resources.Load` fuera del loader ni overrides fuera de tests Editor.

`ISceneFlowService` serializa `Boot → Camp ↔ Expedition`; Infrastructure es el único owner de handles Addressables y Bootstrap conserva la escena persistente. No cargar escenas por strings dispersos, liberar handles fuera del adapter, referenciar Jungle desde Shared ni activar remote catalogs/endpoints. Todo cambio de grupo/perfil pasa el validador local y tres ciclos PlayMode.

La interacción contextual mantiene un solo foco y la selección determinista `prioridad → distancia → ID`; Application coordina approach/cancel/cooldown sobre contratos genéricos y Presentation adapta raycast/prompt. No introducir hardcode animal, `GetComponent` por frame ni conceder discovery mediante evento global. Seguir [`docs/INTERACTION_SYSTEM.md`](docs/INTERACTION_SYSTEM.md).

La locomoción candidata usa `Application.ExplorerLocomotionController → IPathNavigator`; Presentation adapta raycast/NavMeshAgent/cámara y Bootstrap enlaza una sola raíz `PH_` al cargar Selva. No consultar input/NavMesh por APIs dispersas, mover durante UI/fotografía/pausa, añadir joystick/root motion ni declarar tap-to-move definitivo antes del playtest P-006.

Fotografía sigue [`docs/PHOTOGRAPHY_SYSTEM.md`](docs/PHOTOGRAPHY_SYSTEM.md): es render virtual local, nunca cámara física. `CapturePhotoUseCase` confirma discovery antes de thumbnail, conserva solo la mejor foto acotada y degrada a imagen canónica si falla storage. No usar `ScreenCapture`, solicitar CAMERA, mezclar PNG con save JSON ni conservar RenderTextures/handles tras capture/unload.

El FTUE sigue [`docs/TUTORIAL_SYSTEM.md`](docs/TUTORIAL_SYSTEM.md): Content define siete pasos versionados, Application avanza solo por outcomes semánticos y Presentation muestra una instrucción localizada por vez. Back/pause nunca se bloquean; skip y replay son visibles; no añadir polling por nombre, ramas de tutorial dentro de features, analytics remoto ni autoavance por tiempo. Cambiar pasos o triggers incrementa la versión y exige migración/reanudación probada.

`IAudioService` recibe IDs semánticos; Content posee cues/mixer y Infrastructure sources/cola/ducking. Domain no conoce clips. No usar `Resources.Load`, crear `AudioSource` fuera de Bootstrap, solapar voz instructiva ni aceptar audio sin ledger/licencia. Ejecutar `scripts/validate-audio`; cualquier `PH_` permanece bloqueado para Release.

El álbum es read-only: `AlbumQueryService` combina catálogo Approved, discovery y metadata de foto; Presentation no lee Save/filesystem/`AssetDatabase`, no concede progreso y no revela facts/nombre/visual de locked. `IPhotoStore` valida referencias del manifest. Toda carga se cancela por lifecycle y caché/pool permanecen acotados según [`docs/ALBUM_SYSTEM.md`](docs/ALBUM_SYSTEM.md).

Economía sigue [`docs/ECONOMY_REWARDS.md`](docs/ECONOMY_REWARDS.md): una sola moneda ganable, `RewardDefinition` data-driven, transaction keys durables y ledger reciente acotado. Features producen intents; solo Economy muta saldo. Prohibido introducir IAP, premium, azar, rachas, expiry o usar el ledger como autoridad de idempotencia. Debug grant es exclusivamente Development.

Misiones siguen [`docs/09_MISSION_SYSTEM.md`](docs/09_MISSION_SYSTEM.md): facts tipados entran por `IMissionFactSink`, objectives se evalúan mediante registry de strategies y Economy auto-concede la reward idempotente. No crear un bus paralelo, switch central por tipo, timers/dailies/expiry, claim manual o facts anteriores a activación sin opt-in explícito y test.

Input sigue [`docs/INPUT_ACCESSIBILITY.md`](docs/INPUT_ACCESSIBILITY.md): features consumen intenciones `IInputService`; solo Infrastructure consulta Input System. Mapas son UI/Explorer/Photography/Parents y Debug aditivo Development. No usar API legacy, `Touchscreen.current` disperso, joystick permanente ni offsets safe-area duplicados. Back solicita checkpoint y pausa reversible; haptics permanece no-op/off hasta aprobación y prueba física.

Contenido data-driven sigue [`docs/CONTENT_MODEL.md`](docs/CONTENT_MODEL.md): Domain posee IDs tipados, Content mapea ScriptableObjects a definitions readonly y `IContentCatalog` resuelve por índice. No usar GUID/path como negocio, `AssetDatabase` runtime, enums cerrados para tags/categorías/mundos ni mutar assets. Development exige watermark para Draft; Release acepta solo Approved no-placeholder. Todo ID retirado requiere alias y, si fue persistido, migración.

## Dependencias y no-go

- No instalar paquete, SDK o herramienta ni aceptar términos sin autorización y revisión de fuente oficial, licencia, versión exacta, mantenimiento, soporte Android/iOS, 16 KB, datos recolectados y aptitud infantil.
- La allowlist actual está en [`docs/DEPENDENCY_REGISTER.md`](docs/DEPENDENCY_REGISTER.md); cualquier cambio requiere intake. Terceros parten bloqueados.
- No añadir ads, IAP, analytics remota, login, red, contenido remoto, permisos sensibles, secretos o rutas personales sin ADR y aprobaciones requeridas.
- No publicar, hacer push, crear cuentas, firmar artefactos, subir a stores ni aceptar contratos salvo petición explícita.
- `scripts/build-android-release` debe permanecer bloqueado hasta signing externo autorizado; no convertir el placeholder en un bypass ni registrar credenciales en archivos/logs.
- No destruir cambios ajenos, usar comandos Git destructivos ni ampliar más allá de Selva/Vertical Slice.
- Placeholders usan prefijo `PH_` y metadata obligatoria; permanecen bloqueados para Release según los estándares.

## Seguridad infantil

Aplicar privacidad/minimización por defecto, experiencia inicial ad-free, dos guías sin pedir edad, feedback no punitivo, descansos neutrales y ausencia de rachas, energía, gacha, loot boxes o FOMO. Hechos educativos requieren el proceso de [`docs/CONTENT_SOURCES.md`](docs/CONTENT_SOURCES.md). Las políticas son controles de ingeniería, no asesoría legal.

## Validación y definición de terminado

Seguir [`docs/VALIDATION_PLAYBOOK.md`](docs/VALIDATION_PLAYBOOK.md) y revisar con [`docs/CODE_REVIEW_RULES.md`](docs/CODE_REVIEW_RULES.md). Solo afirmar “compila”, “pasa”, “cumple” o “listo” con evidencia ejecutada sobre el estado reportado. Si un comando no se ejecutó, registrar `NOT RUN` y motivo; nunca convertirlo en `PASS`.

Una tarea termina únicamente cuando: alcance implementado o documentado; tests pertinentes ejecutados; build requerido ejecutado; documentación canónica aplicable (`STATUS`, decisiones, riesgos, roadmap, changelog e índice) actualizada; diff revisado; Git contiene solo cambios intencionales; y reporte final distingue `PASS`, `FAIL`, `BLOCKED` y `NOT RUN`. Un bloqueo externo permanece bloqueo.
