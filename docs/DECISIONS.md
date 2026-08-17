# Registro de decisiones

Estados: **Provisional** requiere validación de fase; **Aceptada** gobierna el alcance; **Humana pendiente** no puede resolverse solo con ingeniería.

## ADR-0001 — Selección de Unity 6.3 LTS

- Estado: **Aceptada**, 2026-08-14.
- Decisión: fijar `Unity 6000.3.22f1 (1c726e1fb402)` para el proyecto.
- Contexto: es Unity 6.3 LTS, la última revisión que devolvió la API oficial de releases el 2026-08-14; se publicó el 2026-08-13 y está instalada localmente con módulos Android. Unity anuncia dos años de soporte para 6.3 LTS.
- Evidencia: Release API revalidada; licencia batch; creación/import sin errores; paquetes exactos; EditMode `2/2`; APK Development API 36, IL2CPP/ARM64; inspección de manifest/ELF/zipalign; ejecución visual en emulador 16 KB. El AAB Release sigue diferido a F12.
- Consecuencias: `ProjectVersion.txt` gobierna; no se usan otras revisiones ni se migra silenciosamente. Cada cambio de Editor requiere ADR/plan, backup y repetición de import, tests y smoke.
- Alternativa: si hay bloqueo reproducible, evaluar explícitamente otra revisión parcheada de 6.3 LTS; solo después, una LTS soportada alternativa. No instalar ni migrar silenciosamente.
- Rollback: antes de contenido, recrear el proyecto vacío con la revisión aprobada; después de contenido, exigir plan de migración y backup/branch.
- Fuentes: [release 6000.3.22f1](https://unity.com/releases/editor/whats-new/6000.3.22f1), [anuncio 6.3 LTS](https://unity.com/blog/unity-6-3-lts-is-now-available), verificadas 2026-08-14.

## Decisiones técnicas aceptadas

| ID | Decisión | Razón | Reabrir |
|---|---|---|---|
| T-001 | Android-first, landscape; iOS-ready. | Prioriza el MVP sin acoplar dominio a plataforma. | Si cambia mercado objetivo. |
| T-002 | Offline-first, sin backend en MVP. | Reduce datos infantiles, operación y riesgo. | Solo con caso de uso, DPIA/privacidad y presupuesto aprobados. |
| T-003 | Capas `Domain` C# puro → `Application` → `Infrastructure`/`Presentation`/`Content`. | Testabilidad y fronteras claras con Unity/SDKs. | En revisión de arquitectura Fase 03. |
| T-004 | Composition root explícito; sin service locator global. | Dependencias visibles y reemplazables. | Solo si una feature demuestra que la inyección explícita no escala. |
| T-005 | ScriptableObjects para authoring, no como estado mutable de sesión. | Flujo de contenido amigable sin contaminar dominio. | Tras prueba del pipeline de contenido. |
| T-006 | uGUI + TextMeshPro para runtime del MVP. | Madurez en UI móvil Unity y texto localizado. | Prototipo de UI Fase 07. |
| T-007 | Addressables local-first; catálogo/contenido remoto deshabilitado en MVP. | Organización y memoria sin dependencia de red. | Si tamaño o live content lo exige. |
| T-008 | Puertos con implementaciones mock/null para analytics, ads e IAP. | No integrar SDKs ni transmitir datos por accidente. | Tras aprobación comercial/legal. |
| T-009 | AAB, ARM64 e IL2CPP como objetivo de release Android. | Alineación con Play y native libraries; debe validarse 16 KB. | Build pipeline Android. |
| T-010 | ExecPlan vivo solo para trabajo complejo, transversal, riesgoso o multisesión. | Conserva contexto/recovery donde aporta; evitarlo en cambios triviales reduce duplicación y planes obsoletos. | Si la reanudación o trazabilidad resultan insuficientes. |
| T-011 | Evidencia con estados `PASS`, `FAIL`, `BLOCKED` y `NOT RUN`. | Impide convertir intención, documentación o bloqueo externo en resultado ejecutado. | No se relaja; puede ampliarse el playbook. |
| T-012 | Placeholders `PH_` con metadata y bloqueo de Release. | Permite prototipar sin confundir material temporal con contenido aprobado. | Tras implementar validador de contenido. |
| T-013 | Paquetes directos F03: Input System `1.20.0`, URP `17.3.0`, Test Framework `1.6.0`, uGUI `2.0.0`. | Es la baseline oficial mínima que compila y construye; Addressables/Localization no tienen necesidad todavía. | Intake y evidencia completa antes de cualquier cambio. |
| T-014 | Bundle ID técnico `com.placeholder.pequenoexplorador` y company `Placeholder Studio`. | Permiten builds reproducibles sin asumir titularidad comercial. | Obligatorio reemplazar mediante decisión humana antes de crear registros en stores. |
| T-015 | Nueve assemblies con allowlist ejecutable: Domain, Application, Content, Infrastructure, Presentation, Bootstrap, Editor y dos Tests. | Hace físicas las fronteras sin fragmentar por features inexistentes; Bootstrap es el único composition root. | Solo con evidencia medida y actualización de validador, tests, arquitectura y riesgo. |
| T-016 | Pipeline local canónico en `scripts/validate`; lógica de Unity en Editor, CI manual/self-hosted y Release fail-closed. | Un comando conserva paridad local/CI sin entregar licencia/signing a una Action de terceros ni fingir disponibilidad remota. | Cuando exista remoto/runner/licencia aprobados o se diseñe signing externo. |
| T-017 | `ApplicationHost` inicializa MessageBus → Save → Localization → Audio → Analytics → Ads → Purchases y apaga en orden inverso; `AppContext` es inmutable y el registro tipado es interno a Bootstrap. | Lifecycle determinista sin singleton/service locator y defaults offline seguros. Save/Localization cargan antes de Audio; Development usa mocks compilados solo con define de build; Release usa NullAnalytics/NoAds/UnavailablePurchase. | Si un servicio real recibe ADR/aprobación o el orden tiene una dependencia medida. |
| T-018 | Addressables `4.0.1` local-only: perfiles `LocalDevelopment`/`LocalRelease`, grupos `SharedLocal`/`JungleLocal`, Bootstrap persistente y escenas aditivas por IDs. | El Registry oficial vigente lo declara estable para Unity `6000.0+`; permite ownership/descarga medible sin backend. Remote catalog/update/URL están deshabilitados y cada handle pertenece al adapter. | Si compatibilidad, memoria o tamaño medidos justifican downgrade/cambio; contenido remoto exige ADR, privacidad y backend aprobados. |
| T-019 | Save nació en schema v1 con `JsonUtility` builtin `com.unity.modules.jsonserialize` `1.0.0`, envelope con SHA-256, DTOs Infrastructure, replace atómico/backup y future schema read-only; T-021 lo migró a v2, T-022 a v3 y T-028 a v4. | No añade dependencia; es compatible con IL2CPP y DTOs cerrados. Distingue integridad de cifrado, conserva archivos futuros/corruptos y no guarda PII. | Todo schema/serializer nuevo exige migración; cloud/cifrado/entitlements requieren ADR y necesidad aprobada. |
| T-020 | Dos `AppConfigAsset` locales (Development/Release) se mapean a `IAppConfig` readonly; Bootstrap es el único loader y Release permite cero flags. | Hace explícita la selección, migra hardcodes reales, bloquea mocks/diagnóstico/cheats/bypass y separa build/content/Save sin remote config. | Añadir un perfil/flag/knob exige consumidor real, owner/retiro, tests y revisión de seguridad; remoto requiere ADR/privacidad. |
| T-021 | Localization `1.5.12` + AndroidJNI builtin `1.0.0`; ES startup, EN completo y pseudo Development. `LocalizedKey` gobierna copy y Save persiste solo ES/EN (actualmente v4 por T-028). | Versiones oficiales estables verificadas 2026-08-16; Smart Strings/fallback/asset tables evitan copy disperso y conservan offline/IL2CPP. Pseudo/markers/selector diagnóstico no llegan a Release. | Actualizar paquete/locale/serializer exige intake, migración, validator, ES/EN/pseudo, permisos, IL2CPP y APK; contenido remoto requiere ADR/privacidad. |
| T-022 | Audio usa módulo builtin `com.unity.modules.audio@1.0.0`, mixer Master/Music/Ambience/Effects/Voice, siete sources acotados, cues Content y `IAudioService`; Save schema v3 persiste cinco volúmenes/subtítulos mediante migración v2→v3. | Sin SDK, red ni `Resources.Load`; voz se serializa por prioridad, ducking preserva inteligibilidad y placeholders internos `PH_` permiten validar offline sin fingir contenido final. | Todo placeholder bloquea Release; voces/música/licencias/mezcla y prueba auditiva móvil requieren aprobación humana. Cambiar mixer/pool/formato/schema exige medición, migración y tests. |
| T-023 | Input System `1.20.0` se encapsula en `IInputService`; cinco mapas semánticos, clasificador puro, safe area central y haptics no-op/off. | Evita APIs legacy/lecturas dispersas, doble toque accidental y offsets duplicados. `Explorer` conserva tap-to-move como intención candidata, no movimiento fijado; Debug es aditivo Development-only. | Hardware Android y playtest P-006 antes de Gate C. Implementar haptics, movimiento o nuevos gestos exige necesidad, preferencia adulta y evidencia física. |
| T-024 | IDs de contenido son value objects namespaced en Domain; ScriptableObjects Content compilan a definitions readonly y `IContentCatalog` O(1). Release acepta solo Approved no-placeholder; retiros usan aliases/migración. | Hace extensible el authoring sin GUID/enums/switches de negocio, evita `AssetDatabase` runtime y ejecuta el gate factual/editorial. El ejemplo neutral permanece Draft con watermark. | Cambiar formato/ID publicado exige compatibilidad y migración; producir contenido final exige H-007, fuentes, derechos y Gate B. |
| T-025 | Mundos usan `WorldManifestAsset` → `WorldManifest` readonly, `IWorldCatalog` O(1) y `IWorldSession`; scene flow recibe `SceneContentId` semántico y no enum/switch por bioma. Disponibilidad local está separada de entitlements. | Selva y mundos futuros aportan datos/escena/contenido sin cambiar coordinador, mantienen Addressables/handles en Infrastructure y evitan filtrar GUID/SKU al núcleo. `world.jungle` sigue Draft local; el segundo mundo solo existe como fixture. | Mundo real nuevo exige alcance, manifest, budget/provenance, localización/audio, unload/performance y Gate aplicable. Descarga/remote/entitlement exige ADR separada. |
| T-026 | AI Navigation `2.0.9` fija el prototipo tap-to-move: Application controla estados sobre `IPathNavigator`; Presentation adapta NavMesh/raycast/cámara y Bootstrap enlaza una raíz por escena. Sin root motion/joystick. | Es el pin released explícito para Unity 6000.0, sin native SDK/red/datos. Mantiene reglas testeables, cancelación por mapa/lifecycle y authoring reemplazable; el marker/arte/bob son `PH_`. | P-006 sigue abierto hasta playtest físico. Cambiar paquete/control/root motion/tuning exige intake, device profiling, accesibilidad y suites/build. |
| T-027 | Interacción contextual usa `InteractionId` + definition readonly, selector `prioridad→distancia→ID`, un `InteractionCoordinator` Application y adapters Presentation indexados; Bootstrap enlaza una raíz por mundo. | Las categorías comparten foco/approach/prompt sin hardcode animal, lookup por frame, evento global o dependencia de discovery. Cancelación, lifecycle, cooldown y reloj son deterministas; Content conserva authoring `PH_`. | Discovery/learning deben conectarse por casos de uso desde Prompt 18. Cambiar prioridad/rango/copy/cues requiere datos, tests de overlap/touch y revisión infantil; fixtures Draft bloquean Release. |
| T-028 | Discovery usa `DiscoverUseCase`, outcomes explícitos y `grant.*` persistida; Save schema v4 guarda record `{id,count,firstObservedLocalDate}` + grants procesadas y migra v3→v4 sin inventar historia. | Garantiza first/repeat y reward-once independiente de UI/economía, deriva denominadores del catálogo Approved y minimiza fecha a día local. `AutosaveCoordinator.Latest` evita partir de snapshots obsoletos durante debounce. | Estados detectado/foto/explorado, compactación de grants o cambio de fecha requieren evidencia, migración y ADR; Economy/Album consumen resultado sin alterar Discovery. |

## Decisiones de producto aceptadas

| ID | Decisión | Límite |
|---|---|---|
| P-001 | Público de 4–9 años. | UX y contenido deben funcionar para prelectores y lectores tempranos. |
| P-002 | MVP limitado al mundo Selva. | Otros biomas, multiplayer, cuentas y contenido remoto quedan fuera. |
| P-003 | Sesiones breves y feedback positivo, sin castigo ni dark patterns. | No rachas coercitivas, loot boxes, chat ni presión de compra. |
| P-004 | Experiencia inicial sin publicidad. | Ads es una decisión posterior condicionada, no un backlog implícito. |
| P-005 | Dos modos manuales: `Más guía` y `Guía estándar`. | No se pide edad/fecha; mismo contenido, progreso y recompensa. |
| P-006 | Tap-to-move es candidato con prototipo `PH_`, no control final. | Comparar con alternativa simplificada mediante playtest 4–9; hardware, ergonomía, comprensión y reducción de movimiento antes de aprobar producción. |
| P-007 | Economía blanda determinista para campamento. | Sin pérdida, compra, azar, caducidad, rachas o ventaja. |
| P-008 | Vertical Slice bloquea escalado de contenido. | No producir catálogo MVP hasta pasar Gate B y aprobación factual. |

## Decisiones humanas/comerciales pendientes

| ID | Decisión pendiente | Quién decide | Fecha límite sugerida |
|---|---|---|---|
| H-001 | Titular legal, licencia del producto y cadena de derechos. | Propietario + asesoría legal | Antes de incorporar terceros. |
| H-002 | Modelo de negocio: pago único, gratis, IAP o sin monetización. | Producto/negocio + legal | Antes del Gate D. |
| H-003 | Participación en Apple Kids Category y bandas 5-under/6–8/9–11. | Producto + legal | Antes de metadata iOS. |
| H-004 | Países de lanzamiento y revisión COPPA/GDPR/leyes locales. | Negocio + privacidad/legal | Antes de pruebas externas con menores. |
| H-005 | Política de privacidad pública y datos de contacto. | Titular + legal | Antes de fichas de tienda. |
| H-006 | Si alguna vez habrá ads/analytics/IAP y proveedores aprobados. | Producto + legal + ingeniería | ADR separada; no antes de Gate D. |
| H-007 | Especialista factual y responsables de aprobación de contenido. El [dossier VS-D-A01](VS_D_A01_TOUCAN_FACTUAL_DOSSIER.md) está Sourced; Reviewer/ApprovedBy/Rights/QA siguen vacíos. | Producto + educación | Antes de producir contenido del slice o iniciar Prompt 19. |
| H-008 | Protocolo, consentimiento y reclutamiento de playtests con menores. | Research + legal/privacidad | Antes de playtest con participantes. |
