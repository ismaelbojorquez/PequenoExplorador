# Estado vivo del proyecto

Actualizado: 2026-08-16 10:14 (`America/Mexico_City`). Git y archivos observados prevalecen si contradicen este resumen y obligan a actualizarlo.
Las evidencias `PHASE_00/01_PREFLIGHT.md` conservan referencias históricas al roadmap vigente cuando se escribieron; este archivo y `ROADMAP.md` contienen la asignación actual.

## Fase y Gate

- **Fase actual:** Prompt 10 — configuración runtime y feature flags locales completada; no equivale a Fase 10 (audio) del roadmap histórico.
- **Estado de fase:** `PASS` local; dispositivo físico `NOT RUN`. Save v1 y scene flow/Addressables siguen validados; sin gameplay ni progreso real.
- **Auditoría Gate A:** [`audits/GATE_A_2026-08-15.md`](audits/GATE_A_2026-08-15.md) — `PASS` original y revalidación independiente sobre HEAD F07 `PASS`, sin Critical/Major abierto.
- **Gate actual:** B — Vertical slice playable, iniciado; el flujo actual es placeholder y no gameplay.
- **Siguiente fase:** Prompt 11 — localización español/inglés sin textos hardcodeados, tras preflight y contraste con el roadmap canónico.
- **ExecPlan cerrado:** [`.agent/execplans/p10-validated-runtime-config.md`](../.agent/execplans/p10-validated-runtime-config.md). Los planes F09/Gate A/F07 permanecen cerrados como evidencia histórica.

## Capacidades verificadas

| Capacidad | Estado | Evidencia/limitación |
|---|---|---|
| Git baseline | Verificado | Fases 00–06 forman la base auditada; debe contrastarse en cada preflight. |
| Producto/Vertical Slice/MVP | Especificado | Fuentes canónicas en visión, GDD y `MVP_SCOPE.md`; no implementado. |
| Contrato de agentes | Especificado | `AGENTS`, planes, estándares, review y playbook de F02. |
| Proyecto Unity | Verificado | `6000.3.22f1`, URP, Bootstrap y estructura mínima en raíz. |
| Pipeline local | `PASS` revalidado | Prompt 10: `scripts/validate` código `0` en 3:27.34 tras recompilación; checks/config, compile, catálogo local, EditMode, PlayMode y APK. JUnit/logs/manifests en `artifacts/`. |
| Fronteras/compilación | `PASS` | Nueve asmdefs, allowlist/cycles y placeholder validados; Domain/Application sin engine. |
| Lifecycle y servicios | `PASS` | Inicio secuencial, cierre inverso, idempotencia, retry, cancelación propia del host, cleanup del servicio en curso, perfiles fail-closed y limpieza de listeners cubiertos por tests. |
| Scene flow / Addressables | `PASS` | `4.0.1`; Boot→Camp↔Jungle local, exclusión/error/cancel/timeout, tres ciclos, un handle actual y cero tras shutdown; perfiles/grupos/labels validados, sin remoto. |
| Save local | `PASS` automatizado; dispositivo `NOT RUN` | Schema v1, `JsonUtility` builtin `1.0.0`, SHA-256, temp/flush/replace, backup, v0→v1, future read-only, autosave y reset Editor; sin PII/red. Contrato en `10_SAVE_SYSTEM.md`. |
| Config runtime | `PASS` | Dos assets tipados Development/Release, mapping readonly, cero flags Release, loader Bootstrap, override Editor y validador build; fixture insegura falló `CONFIG008`. Sin red/secret/remote config. |
| Tests Unity | `PASS` | EditMode `57/57` (56 del proyecto + 1 stub documental del paquete); PlayMode `5/5`, incluidos perfil Development, scene flow y save tras recarga/recreación. |
| Build Android/iOS | Android Development `PASS`; Release `BLOCKED`; iOS `NOT RUN` | APK Prompt 10: `60,310,101` bytes, SHA-256 `2c47d85cfe271bc8dde71979779dd7f36d45d09ce8746806a3505de43f9d3b80`, API 26/36, IL2CPP/ARM64. Release validó config y devolvió `3` por signing. Sin dispositivo para instalación/I/O físico. |
| CI GitHub | `NOT RUN` | Workflow estático + Unity manual/self-hosted definidos; no hay remoto, runner ni activación CI. |
| Paquetes | Verificado | Cinco pins directos oficiales; Addressables `4.0.1` y transitivos exactos, sin preview/SDK comercial/binario nativo nuevo. |
| Gameplay/assets finales | No existen | Prohibido escalar contenido antes del Vertical Slice. |

## Fuentes necesarias para reanudar después de Prompt 10

1. [`../AGENTS.md`](../AGENTS.md) y este estado.
2. [`ROADMAP.md`](ROADMAP.md), [`DECISIONS.md`](DECISIONS.md) y [`RISK_REGISTER.md`](RISK_REGISTER.md).
3. [`02_TECHNICAL_ARCHITECTURE.md`](02_TECHNICAL_ARCHITECTURE.md), [`VERSION_MATRIX.md`](VERSION_MATRIX.md) y [`DEPENDENCY_REGISTER.md`](DEPENDENCY_REGISTER.md).
4. [`ENGINEERING_STANDARDS.md`](ENGINEERING_STANDARDS.md), [`VALIDATION_PLAYBOOK.md`](VALIDATION_PLAYBOOK.md) y [`.agent/PLANS.md`](../.agent/PLANS.md).
5. [`RUNTIME_CONFIGURATION.md`](RUNTIME_CONFIGURATION.md), [`10_SAVE_SYSTEM.md`](10_SAVE_SYSTEM.md), [`18_TESTING.md`](18_TESTING.md) y [`CONTENT_PIPELINE.md`](CONTENT_PIPELINE.md) para reejecutar `scripts/validate` y conservar local-only.
6. [`MVP_SCOPE.md`](MVP_SCOPE.md) y el prompt humano actual para acotar el siguiente incremento sin inventar progreso.

## Bloqueos y decisiones humanas

- Licencia Unity batch: verificada con un proyecto desechable; no es bloqueo.
- Android Build Support y toolchain bundled están verificados mediante smoke real. La inspección inicial de ruta fue corregida en `PHASE_03_PREFLIGHT.md`.
- Licencia/titularidad del producto y terceros: H-001, bloquea incorporación de dependencias/assets dudosos.
- Bundle ID/company definitivos requieren decisión humana antes de stores; los placeholders son no publicables.
- Xcode y módulo iOS no disponibles localmente; no bloquearon F09, sí futura evidencia iOS.
- No había dispositivo Android conectado en F09; I/O real, pause/force-stop y falta de espacio permanecen `NOT RUN` para Gate C/F34–F35.
- GitHub remoto, runner Unity y activación CI no existen; no bloquean desarrollo local, sí evidencia Unity remota.
- Especialista factual, protocolo de playtests, territorios, modelo comercial y stores siguen pendientes; no son bloqueos para crear una foundation vacía.

## Reanudación inmediata

Ejecutar el preflight de Prompt 11 desde [`prompts/00_MASTER_CODEX_PROMPTS.md`](prompts/00_MASTER_CODEX_PROMPTS.md), contrastar Git/tests y reconciliarlo con el roadmap canónico. No duplicar AppConfig en Save, habilitar flags Release, añadir remote config, cambiar schema sin migración/tests, escalar contenido, crear push/signing/publicación ni instalar SDKs.
