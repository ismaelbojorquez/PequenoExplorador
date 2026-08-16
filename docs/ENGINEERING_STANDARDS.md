# Estándares de ingeniería

Reglas verificables para la implementación incremental. La foundation de Fase 03 prueba Editor/build, pero no prueba todavía las capas de gameplay descritas aquí.

## Layout y dirección de dependencias

```text
Domain <- Application <- Presentation
                     <- Infrastructure
                     <- Content
Bootstrap/CompositionRoot -> Application + adaptadores concretos
Tests -> assembly bajo prueba
```

- **Domain:** C# puro; entidades, value objects, reglas, estados y eventos. No referencia `UnityEngine`, escenas, ScriptableObjects, filesystem, red, SDKs ni reloj global.
- **Application:** casos de uso y puertos; coordina Domain sin conocer implementaciones de plataforma.
- **Infrastructure:** save, reloj, filesystem y adaptadores de plataforma/SDK aprobados.
- **Presentation:** MonoBehaviours, UI, input, cámara, audio y conversión entre Unity y Application.
- **Content:** authoring/validación y mapeo de datos aprobados a definiciones de aplicación/dominio; no contiene estado mutable de sesión.
- **Bootstrap:** único composition root explícito. Construye y conecta dependencias; no existe service locator global ni búsqueda implícita como arquitectura.

Fase 04 fija nueve asmdefs: seis runtime, Editor y dos suites. La tabla y grafo exactos viven en [`02_TECHNICAL_ARCHITECTURE.md`](02_TECHNICAL_ARCHITECTURE.md). Las referencias siguen las flechas, sin ciclos; `Domain`/`Application` usan `noEngineReferences`, Editor no entra al player y Presentation no referencia Infrastructure. `AssemblyBoundaryRules` bloquea deriva. No subdividir por feature salvo evidencia medida de compilación, ownership, plataforma o aislamiento.

## Composition root y servicios

- Application define solo boundaries reales: lifecycle, reloj, azar, logs, mensajes y servicios de plataforma; no conoce Unity ni concretos.
- Infrastructure implementa adapters; Bootstrap es el único lugar que selecciona concretos. Presentation recibe fachadas/casos de uso, nunca Infrastructure.
- `AppContext` es inmutable y se pasa explícitamente. No hacerlo estático, publicarlo como `Instance` ni añadir `Get<T>`/registro por diccionario.
- `ServiceRegistry` es interno a Bootstrap y tipado. El orden canónico de lifecycle es MessageBus → Input → SafeArea → Haptics → Save → Localization → Audio → Analytics → Ads → Purchases, con shutdown inverso y cleanup tras fallo/cancelación.
- Development usa mocks locales solo bajo `UNITY_EDITOR` o `PE_DEVELOPMENT_SERVICES`; Release usa NullAnalytics, NoAds y UnavailablePurchase. El define Development no se persiste en PlayerSettings.
- Nuevos servicios requieren una dependencia real, ID técnico único, init/cancel/shutdown testeados, default Release seguro y actualización de la tabla canónica.

## Convenciones C#

- Namespace: `PequenoExplorador.<Layer>[.<Feature>]`; nombre de carpeta/asmdef coherente con la capa.
- Un tipo principal por archivo; nombres en inglés para APIs/código y español solo en contenido/copy aprobado.
- Habilitar nullable en asmdefs/proyectos cuando el Editor y toolchain fijados lo soporten y las pruebas pasen; no silenciarlo globalmente. Expresar ausencia deliberada con tipos/contratos claros.
- Campos serializados: `[SerializeField] private`; no exponer estado mutable público. Validar referencias en authoring y fallar con mensaje accionable en desarrollo.
- Serialización es explícita: campos/versiones conocidos, enums estables o IDs, sin depender de nombres de tipo/assembly ni serializar objetos Unity dentro de Domain/save.
- Preferir funciones pequeñas, invariantes en constructores/factories y dependencias por constructor; evitar singletons y estado estático mutable.

## Unity y ciclo de vida

- MonoBehaviours adaptan input, vista y lifecycle; reglas complejas viven en Domain/Application y tienen tests fuera de escena.
- `Awake` valida/captura referencias locales; `OnEnable` suscribe; `OnDisable` desuscribe/cancela; `Start` solo inicia orquestación que requiere objetos habilitados. No depender de orden implícito entre objetos.
- No usar `Find*`, strings de escena, tags o recursos globales como wiring permanente. El composition root entrega dependencias explícitas.
- Evitar trabajo asignante o búsqueda repetida en `Update`; toda excepción de performance requiere medición.
- Solo el adapter Input autorizado consulta Input System/EnhancedTouch. Features consumen intenciones semánticas; quedan prohibidos `UnityEngine.Input`, `Touchscreen.current` disperso y joystick permanente sin decisión P-006/playtest.
- Cada Canvas usa exactamente un root `Safe Area`; no apilar fitters/offsets. Debug input/overlays no se habilitan en Release y haptics parte no-op/off.

## Async, cancelación y errores

- Toda operación asíncrona que pueda sobrevivir a una pantalla/objeto recibe `CancellationToken` ligado a lifecycle y cancelación de aplicación.
- No `async void` salvo event handler de frontera; debe capturar/reportar fallos y delegar a una operación testeable.
- Coroutines se reservan para secuencias de frames/Unity; no sustituyen reglas de dominio ni ocultan errores.
- Cambios de API Unity ocurren en el hilo principal. Timeouts/reintentos son explícitos y nunca crean presión o recompensa infantil.
- Errores recuperables ofrecen fallback; corrupción de save conserva evidencia segura y evita sobrescritura silenciosa.

## Eventos

- Eventos de dominio son datos inmutables en C# puro y describen hechos pasados; no ejecutan UI/SDK.
- Application publica/consume mediante interfaces explícitas. No event bus estático global.
- Suscripciones de Presentation son simétricas y acotadas al lifecycle. Handlers deben ser idempotentes cuando una reanudación pueda repetir entrega.
- Eventos no transportan datos personales infantiles ni se convierten automáticamente en analytics.
- El bus local no sustituye una llamada directa. Toda suscripción conserva y dispone su token `IDisposable`; shutdown elimina listeners restantes.

## ScriptableObjects y contenido

- ScriptableObjects son authoring inmutable en runtime: IDs, referencias y parámetros; se validan y mapean a definiciones antes del uso.
- No guardar progreso/sesión en ScriptableObjects ni mutar assets para representar gameplay.
- IDs son estables y únicos; referencias rotas, duplicados, claims sin aprobación o placeholders inválidos bloquean validación.
- IDs extensibles son value objects namespaced, no enums ni GUID/path Unity. Un generador solo llena vacío; renames no cambian ID y retiros publicados exigen alias/migración.
- Catálogos se compilan una vez a modelos readonly e índices; `AssetDatabase` nunca entra al runtime ni se usa búsqueda lineal por frame.
- Hechos educativos siguen [`CONTENT_SOURCES.md`](CONTENT_SOURCES.md).

## Configuración runtime

- `AppConfigAsset` vive en Content; Bootstrap carga/mapea exactamente un perfil tipado y Application solo recibe `IAppConfig`/`IFeatureFlags` readonly.
- Build-time (target/API/signing), content-time (Addressables/claims) y preferencias adultas (Save) no se duplican dentro de AppConfig.
- IDs de perfiles/flags son enums con valores explícitos. No crear lookup por strings, diccionario global mutable ni flags sin owner/consumidor real.
- Release usa cero flags y el validador bloquea diagnóstico, mocks, simulación, cheats y bypass parental antes de build. El define Development sigue excluyendo concretos mock del player Release.
- No remote config, endpoint, segmentación infantil ni fallback silencioso. Missing/duplicate/invalid config es error accionable de import/build.
- El loader `Resources` local es exclusivo de Bootstrap y su path es constante; ninguna feature o adapter puede cargar configuración por su cuenta.

## Addressables, save y plataforma

- Addressables es local-first; catálogo remoto, actualización y descarga quedan deshabilitados en MVP. Las claves son constantes/IDs validados, no strings dispersos.
- Save usa DTOs separados del Domain, con `schemaVersion`, IDs estables, valores mínimos y migraciones encadenadas/testeadas. El contrato ejecutable está en [`10_SAVE_SYSTEM.md`](10_SAVE_SYSTEM.md): `JsonUtility` queda encapsulado en Infrastructure, checksum no se llama cifrado, future schema es read-only, escritura rota primary→backup y una recuperación preserva el backup válido.
- Localización usa `LocalizedKey` namespaced y estable; Domain no contiene copy, Presentation no lee tablas y Bootstrap inyecta `ILocalizationService`. Variables/plurales usan Smart Strings, pseudo es Development-only y Save schema v2 persiste solo ES/EN. Contrato: [`17_LOCALIZATION.md`](17_LOCALIZATION.md).
- Reloj, almacenamiento, audio focus, lifecycle y futuros servicios de plataforma entran por puertos de Application. Ads/IAP/analytics usan implementaciones null/mock hasta ADR y aprobación humana.
- Feature flags son locales, tipados, con owner/fecha de retiro y default seguro. No remote config, segmentación infantil o activación silenciosa; ads/IAP/red permanecen off por defecto.

## Logs

- Logs accionables incluyen subsistema, operación e ID técnico no personal; nunca nombre/voz/edad exacta, progreso identificable, secreto, token o ruta personal.
- No loggear por frame ni errores esperados repetidamente. Niveles y rate limiting deben evitar ruido y degradación.
- Un log no sustituye manejo de error, test ni evidencia de cumplimiento.

## Dependencias

Antes de añadir o actualizar paquete, SDK, plugin, fuente, asset o herramienta, registrar en decisión/inventario:

1. fuente oficial y método reproducible de obtención;
2. licencia/términos y aprobación del titular;
3. versión exacta/checksum o pin equivalente;
4. mantenimiento, vulnerabilidades y plan de actualización/retirada;
5. compatibilidad con Unity fijado, Android, iOS, IL2CPP y arquitectura objetivo;
6. evidencia de 16 KB para toda librería nativa Android;
7. permisos, tráfico, almacenamiento, identificadores y datos recolectados —incluidos transitivos—;
8. aptitud para público infantil/Families/Apple Kids y efecto en declaraciones de tienda;
9. impacto en tamaño, memoria, arranque y modo offline;
10. alternativa sin dependencia y rollback.

Sin evidencia/aprobación, la dependencia queda `BLOCKED`; no se instala para “probar” aceptando términos implícitos.

## Placeholders

Todo placeholder usa ID/nombre `PH_<AREA>_<DESCRIPCION>` y metadata: `placeholder=true`, owner, propósito, procedencia/licencia, fecha, fase de reemplazo y `releaseStatus=Blocked`. Debe ser visual/sonoramente identificable como temporal sin comunicar hechos no aprobados.

Un placeholder no entra en Release. Para liberarlo debe reemplazarse por material aprobado, retirar prefijo/flag, completar procedencia/revisión factual/cultural y pasar el validador de contenido; cambiar solo el nombre no lo aprueba.

## Tests

- Domain/Application: tests rápidos deterministas por regla, transición, error y cancelación aplicable.
- Infrastructure: round-trip, migraciones, corrupción, escrituras interrumpidas y adapters null/mock.
- Presentation/PlayMode: wiring, lifecycle, escenas, input, accesibilidad y estados de interrupción que requieran Unity.
- Cada bug corregido añade regresión cuando sea razonable. Tests no usan red/reloj/azar real y no dependen del orden.
- Los comandos concretos de import, EditMode y Android están en [`VALIDATION_PLAYBOOK.md`](VALIDATION_PLAYBOOK.md); ampliar al crear cada suite/target, nunca inventarlos.
