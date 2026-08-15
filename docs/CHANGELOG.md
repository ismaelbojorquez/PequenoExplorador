# Changelog

Todos los cambios notables de ingeniería se registran aquí. La versión técnica de desarrollo es `0.1.0-dev`; no representa un release comercial.

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
