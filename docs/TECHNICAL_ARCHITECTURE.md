# Arquitectura técnica — foundation

Estado: baseline de Fase 03, 2026-08-14. El proyecto contiene infraestructura mínima de arranque; no contiene gameplay ni implementa todavía las capas de producto.

## Foundation implementada

```text
Assets/_Game/
├── Bootstrap/       escena de entrada y diagnóstico temporal
├── Shared/          vacío; utilidades aprobadas futuras
├── Features/        vacío; slices futuros
├── Worlds/          vacío; Selva futura
├── Content/         URP y metadata de placeholder
├── UI/              vacío; shell futuro
├── Audio/           vacío
├── VFX/             vacío
├── Editor/          setup reproducible y Android build CLI
└── Tests/EditMode/  contrato mínimo de Bootstrap
```

Solo existen tres asmdefs: runtime foundation, Editor y EditMode. Es una frontera de compilación mínima, no la arquitectura de gameplay. `Bootstrap.unity` es el único entry point habilitado. `DiagnosticBootstrap` es un marcador temporal sin reglas.

## Dirección candidata, todavía no implementada

```text
Domain C# puro <- Application <- Presentation
                           \- Infrastructure
                           \- Content
Bootstrap/CompositionRoot -> casos de uso + adaptadores concretos
```

- `Domain` no referencia `UnityEngine`.
- `Application` define casos de uso y puertos.
- `Infrastructure`, `Presentation` y `Content` dependen hacia Application/Domain, nunca al revés.
- El composition root futuro vive en Bootstrap y conecta implementaciones explícitas; no habrá service locator global.
- ScriptableObjects serán authoring, no estado mutable de sesión.
- uGUI/TMP será UI runtime; Addressables será local-first cuando una fase justifique añadirlo.
- Analytics, ads e IAP permanecerán como puertos null/mock hasta ADR y aprobación humana.

Fase 03 no crea clases, servicios ni asmdefs vacíos para aparentar esta arquitectura. Las capas nacen incrementalmente cuando el Vertical Slice tenga un caso de uso real.

## Configuración móvil

- URP `17.3.0`, render scale `1.0`, HDR off, MSAA `2x`, luz adicional per-vertex, sombras adicionales off y distancia de sombras `20`.
- Color space Linear; calidad móvil por defecto `Medium`; landscape left/right.
- Input System only; safe area se difiere a Fase 05.
- Android: min API 26 provisional, target/compile 36, IL2CPP y ARM64; sin manifest ni Gradle personalizados.
- iOS-ready significa orientación e identificador configurables y fronteras de plataforma, no build iOS demostrado.

## Identidad y placeholders

`Placeholder Studio` y `com.placeholder.pequenoexplorador` son valores técnicos no publicables. `PH_UI_DIAGNOSTIC` incluye metadata y `releaseApproved=false`. Titular, company name e identificadores finales son decisiones humanas previas a cualquier consola de tienda.
