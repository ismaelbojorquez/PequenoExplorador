# Estado vivo del proyecto

Actualizado: 2026-08-15 11:59 (`America/Mexico_City`). Git y archivos observados prevalecen si contradicen este resumen y obligan a actualizarlo.
Las evidencias `PHASE_00/01_PREFLIGHT.md` conservan referencias históricas al roadmap vigente cuando se escribieron; este archivo y `ROADMAP.md` contienen la asignación actual.

## Fase y Gate

- **Fase actual:** 07 — incremento de flujo aditivo y Addressables locales, sin gameplay.
- **Estado de fase:** incremento scene-flow/Addressables completado y validado. F07 sigue abierta por shell/safe area/input.
- **Auditoría Gate A:** [`audits/GATE_A_2026-08-15.md`](audits/GATE_A_2026-08-15.md) — `PASS` original y revalidación independiente sobre HEAD F07 `PASS`, sin Critical/Major abierto.
- **Gate actual:** B — Vertical slice playable, iniciado; el flujo actual es placeholder y no gameplay.
- **Siguiente fase:** completar los entregables restantes de F07 — shell landscape, safe areas y prototipo/playtest de interacción candidata— después de cerrar este incremento.
- **ExecPlan cerrado:** [`.agent/execplans/gate-a-revalidation-2026-08-15.md`](../.agent/execplans/gate-a-revalidation-2026-08-15.md). El plan F07 cerrado se conserva como evidencia histórica.

## Capacidades verificadas

| Capacidad | Estado | Evidencia/limitación |
|---|---|---|
| Git baseline | Verificado | Fases 00–06 forman la base auditada; debe contrastarse en cada preflight. |
| Producto/Vertical Slice/MVP | Especificado | Fuentes canónicas en visión, GDD y `MVP_SCOPE.md`; no implementado. |
| Contrato de agentes | Especificado | `AGENTS`, planes, estándares, review y playbook de F02. |
| Proyecto Unity | Verificado | `6000.3.22f1`, URP, Bootstrap y estructura mínima en raíz. |
| Pipeline local | `PASS` revalidado | `scripts/validate` código `0` en 109.43 s: checks, compile, catálogo local, EditMode, PlayMode y APK; segundo smoke Android código `0` en 62.22 s. JUnit/logs/manifests en `artifacts/`. |
| Fronteras/compilación | `PASS` | Nueve asmdefs, allowlist/cycles y placeholder validados; Domain/Application sin engine. |
| Lifecycle y servicios | `PASS` | Inicio secuencial, cierre inverso, idempotencia, retry, cancelación propia del host, cleanup del servicio en curso, perfiles fail-closed y limpieza de listeners cubiertos por tests. |
| Scene flow / Addressables | `PASS` | `4.0.1`; Boot→Camp↔Jungle local, exclusión/error/cancel/timeout, tres ciclos, un handle actual y cero tras shutdown; perfiles/grupos/labels validados, sin remoto. |
| Tests Unity | `PASS` | EditMode `29/29` (28 del proyecto + 1 stub documental del paquete); PlayMode `4/4`, incluido reload, tres ciclos, fallo/retry y cero handles tras shutdown. |
| Build Android/iOS | Android Development `PASS`; Release `BLOCKED`; iOS `NOT RUN` | Revalidación: dos APK IL2CPP/ARM64 API 26/36, `41,722,038` y `41,722,037` bytes; segundo SHA-256 `0a9ae311635ec636e4d70057d6c4a6a8c861d60727ed4d0571fbdc20930bb1e1`. Manifest/ELF/zipalign 16 KB pasan. El intento adicional de dispositivo en emulador oficial-no-soportado quedó `INCONCLUSIVE` por ANR del sistema invitado, no del juego. |
| CI GitHub | `NOT RUN` | Workflow estático + Unity manual/self-hosted definidos; no hay remoto, runner ni activación CI. |
| Paquetes | Verificado | Cinco pins directos oficiales; Addressables `4.0.1` y transitivos exactos, sin preview/SDK comercial/binario nativo nuevo. |
| Gameplay/assets finales | No existen | Prohibido escalar contenido antes del Vertical Slice. |

## Fuentes necesarias durante Fase 07

1. [`../AGENTS.md`](../AGENTS.md) y este estado.
2. [`ROADMAP.md`](ROADMAP.md), [`DECISIONS.md`](DECISIONS.md) y [`RISK_REGISTER.md`](RISK_REGISTER.md).
3. [`02_TECHNICAL_ARCHITECTURE.md`](02_TECHNICAL_ARCHITECTURE.md), [`VERSION_MATRIX.md`](VERSION_MATRIX.md) y [`DEPENDENCY_REGISTER.md`](DEPENDENCY_REGISTER.md).
4. [`ENGINEERING_STANDARDS.md`](ENGINEERING_STANDARDS.md), [`VALIDATION_PLAYBOOK.md`](VALIDATION_PLAYBOOK.md) y [`.agent/PLANS.md`](../.agent/PLANS.md).
5. [`18_TESTING.md`](18_TESTING.md) y [`CONTENT_PIPELINE.md`](CONTENT_PIPELINE.md) para reejecutar `scripts/validate` y conservar local-only.
6. [`MVP_SCOPE.md`](MVP_SCOPE.md), [`14_UI_UX.md`](14_UI_UX.md) y [`PLAYTEST_PLAN.md`](PLAYTEST_PLAN.md) para acotar el prototipo y su evidencia.

## Bloqueos y decisiones humanas

- Licencia Unity batch: verificada con un proyecto desechable; no es bloqueo.
- Android Build Support y toolchain bundled están verificados mediante smoke real. La inspección inicial de ruta fue corregida en `PHASE_03_PREFLIGHT.md`.
- Licencia/titularidad del producto y terceros: H-001, bloquea incorporación de dependencias/assets dudosos.
- Bundle ID/company definitivos requieren decisión humana antes de stores; los placeholders son no publicables.
- Xcode y módulo iOS no disponibles localmente; no bloquean F07, sí futura evidencia iOS.
- GitHub remoto, runner Unity y activación CI no existen; no bloquean desarrollo local, sí evidencia Unity remota.
- Especialista factual, protocolo de playtests, territorios, modelo comercial y stores siguen pendientes; no son bloqueos para crear una foundation vacía.

## Reanudación inmediata

Ejecutar preflight y distinguir el archivo ajeno no versionado bajo `docs/prompts/` del trabajo de fase. Continuar los entregables restantes de F07: shell/safe area y comparación de interacción candidata, consumiendo `ISceneFlowService` sin tocar handles/Addressables desde Presentation. No escalar contenido, activar remoto, crear push/signing/publicación ni instalar SDKs.
