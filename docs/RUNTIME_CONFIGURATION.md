# Configuración runtime local

Estado: Prompt 10 implementado. Configuración tipada y local para foundation; no contiene tuning de gameplay, remote config, secretos ni segmentación infantil.

## Autoridades separadas

| Autoridad | Ejemplos | Fuente | Persistencia/runtime |
|---|---|---|---|
| Build-time | API 26/36, IL2CPP, ARM64, define Development, signing | BuildTools/ProjectSettings | No forma parte de `AppConfig`; Release sigue bloqueado sin signing. |
| Content-time | perfiles/grupos/labels Addressables, assets aprobados | Content pipeline/Addressables | Validado antes de build; no es preferencia de usuario. |
| Runtime inmutable | perfil, producto/versión técnica, seed, timeout, debounce y flags | Dos `AppConfigAsset` bajo Content | Mapea una vez al arrancar; no se muta ni se guarda. |
| Preferencia adulta | modo de guía, música, SFX, narración, locale ES/EN | Save schema v2 | Mutable y versionada; no copia `AppConfig`. |

Ningún valor se consulta por red. No existe endpoint, remote catalog, remote config, identidad, variante por niño ni activación silenciosa.

## Contrato y flujo

```text
Content
  AppConfigDevelopment.asset + AppConfigRelease.asset
       ↓ AppConfigMapper / AppConfigCatalog (exactamente uno de cada)
Application
  IAppConfig + IFeatureFlags (readonly) / IDs enum explícitos
       ↓ BuildProfileConfiguration (Bootstrap, único loader)
Bootstrap
  AppContext.Configuration → ServiceRegistry / Presentation
```

Los IDs `BuildProfile` y `FeatureFlag` tienen valores enteros explícitos y no dependen de nombres de asset o strings dispersos. `Resources/Configuration` es una excepción local, sincrónica y acotada al composition root para datos de arranque pequeños: ninguna feature, Presentation o Infrastructure llama `Resources.Load`. Ausencia, duplicado, path incorrecto o asset inválido detiene inicialización/build con código `CONFIG*` accionable.

## Perfiles

| Campo | Development | Release | Origen previo migrado |
|---|---|---|---|
| `BuildProfile` | `Development` (`1`) | `Release` (`2`) | Rama por define en Bootstrap. |
| Producto | `Pequeño Explorador: Aprende Jugando` | Igual | Constante del diagnóstico. |
| Versión técnica | `0.1.0-dev` | `0.1.0` | Constante/PlayerSettings; PlayerSettings sigue build-time. |
| Random seed | `20260814` | `20260814` | `BuildProfileConfiguration`. |
| Timeout scene flow | `20 s` | `20 s` | `ServiceRegistry`. |
| Debounce autosave | `500 ms` | `500 ms` | `ServiceRegistry`. |
| Flags habilitados | Diagnóstico, fallo simulado, Mock Ads, Mock Purchases | Ninguno | Condicionales por entorno. |

Son defaults de foundation, no presupuestos de gameplay. Cambiarlos requiere editar el asset correspondiente, ejecutar tests/validator y revisar impacto; no se añade un knob sin consumidor real.

## Flags y política Release

| ID estable | Dev default | Release | Owner / retiro o revisión | Uso actual |
|---|:---:|:---:|---|---|
| `DevelopmentDiagnostics` (`1`) | ON | Prohibido | Bootstrap/UI; retirar overlay antes de Gate F | Overlay/controles temporales. |
| `SimulatedSceneFailure` (`2`) | ON | Prohibido | Runtime/QA; conservar solo fixture Editor/Development | Fixture recuperable de scene flow. |
| `MockAds` (`3`) | ON | Prohibido | Platform/Privacy; revisar en Gate D, nunca habilitar implícitamente | Adapter local sin red. |
| `MockPurchases` (`4`) | ON | Prohibido | Platform/Commerce; revisar en Gate D, nunca habilitar implícitamente | Adapter local sin tienda/red. |
| `Cheats` (`5`) | OFF | Prohibido | Security/QA; retirar ID si Gate D confirma que no existe consumidor | Control negativo; no implementa cheats. |
| `ParentalGateBypass` (`6`) | OFF | Prohibido | Child Safety; retirar ID si Gate D confirma que no existe consumidor | Control negativo; no implementa bypass. |

En compilación Release las clases Mock/fallo simulado siguen excluidas por define. Además, `AppConfigValidator` rechaza cualquier flag habilitado en el perfil Release; la defensa no depende de una sola capa. Release selecciona exclusivamente `AppConfigRelease` y servicios `NullAnalytics`, `NoAds`, `UnavailablePurchase`.

## Validación y build

```sh
scripts/validate-content
scripts/compile
scripts/validate
scripts/build-android-release  # valida perfiles y luego devuelve 3 por signing
```

`RuntimeConfigurationValidationService` exige dos assets, exactamente un Development/Release, ubicación local correcta, campos/rangos válidos, IDs sin duplicados y cero flags Release. `BuildToolsCli.ValidateRuntimeConfiguration` permite invocación CLI aislada. Compile, Addressables y ambos paths Android llaman validación de contenido antes de construir.

El override temporal `BuildProfileConfiguration.PushOverrideForTests` existe solo bajo `UNITY_EDITOR`, requiere config válida, impide anidamiento y restaura al hacer `Dispose`. No es un switch runtime ni entra al player Release.

## Modificación segura

1. Confirmar que el valor tiene consumidor real y pertenece a runtime inmutable.
2. Editar solo uno o ambos assets según intención; no duplicarlos ni moverlos fuera de la ruta validada.
3. Añadir/actualizar test de mapping, rango y perfil seguro.
4. Ejecutar `scripts/validate`; para cambios Release ejecutar también el guard y esperar código `3`, nunca `0`.
5. No guardar el valor en Save, no añadir endpoint y no usar el flag para lógica comercial oculta.
