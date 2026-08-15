# Changelog

Todos los cambios notables de ingeniería se registran aquí. La versión técnica de desarrollo es `0.1.0-dev`; no representa un release comercial.

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
