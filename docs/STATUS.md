# Estado vivo del proyecto

Actualizado: 2026-08-15 00:00 (`America/Mexico_City`). Git y archivos observados prevalecen si contradicen este resumen y obligan a actualizarlo.
Las evidencias `PHASE_00/01_PREFLIGHT.md` conservan referencias históricas al roadmap vigente cuando se escribieron; este archivo y `ROADMAP.md` contienen la asignación actual.

## Fase y Gate

- **Fase actual:** 06 — composition root y servicios transversales seguros, sin gameplay.
- **Estado de fase:** completada y validada localmente.
- **Gate actual:** B — Vertical slice playable, iniciado; F06 establece lifecycle/puertos y no implementa el slice.
- **Siguiente fase:** 07 — shell landscape, safe areas y prototipo/playtest de interacción candidata.
- **ExecPlan activo:** ninguno; el plan cerrado se conserva en [`.agent/execplans/06-application-bootstrap-services.md`](../.agent/execplans/06-application-bootstrap-services.md).

## Capacidades verificadas

| Capacidad | Estado | Evidencia/limitación |
|---|---|---|
| Git baseline | Verificado | Fases 00–05 forman la base; debe contrastarse en cada preflight. |
| Producto/Vertical Slice/MVP | Especificado | Fuentes canónicas en visión, GDD y `MVP_SCOPE.md`; no implementado. |
| Contrato de agentes | Especificado | `AGENTS`, planes, estándares, review y playbook de F02. |
| Proyecto Unity | Verificado | `6000.3.22f1`, URP, Bootstrap y estructura mínima en raíz. |
| Pipeline local | `PASS` | Validación final `scripts/validate`, código `0` en 1:14.27 con caché: checks, compile/contenido/fronteras, EditMode `19/19`, PlayMode `2/2` y APK Development. JUnit/logs/manifests en `artifacts/`. |
| Fronteras/compilación | `PASS` | Nueve asmdefs, allowlist/cycles y placeholder validados; Domain/Application sin engine. |
| Lifecycle y servicios | `PASS` | Inicio secuencial, cierre inverso, idempotencia, retry, cancelación, perfiles fail-closed y limpieza de listeners cubiertos por tests. |
| Tests Unity | `PASS` | EditMode `19/19`; PlayMode `2/2`; dispose explícito, estado `Ready`, reload sin duplicados y fallo recuperable cubiertos. |
| Build Android/iOS | Android Development `PASS`; Release `BLOCKED`; iOS `NOT RUN` | APK F06 `57,069,510 bytes`, IL2CPP/ARM64 API 26/36 y zipalign 16 KB. Cada build registra su SHA-256 y commit de entrada en `artifacts/reports/android-development.json`; Release exige signing externo y el módulo iOS no está instalado. |
| CI GitHub | `NOT RUN` | Workflow estático + Unity manual/self-hosted definidos; no hay remoto, runner ni activación CI. |
| Paquetes | Verificado | Cuatro pins directos oficiales; lock y intake documentados; sin preview/SDK comercial. |
| Gameplay/assets finales | No existen | Prohibido escalar contenido antes del Vertical Slice. |

## Fuentes necesarias durante Fase 07

1. [`../AGENTS.md`](../AGENTS.md) y este estado.
2. [`ROADMAP.md`](ROADMAP.md), [`DECISIONS.md`](DECISIONS.md) y [`RISK_REGISTER.md`](RISK_REGISTER.md).
3. [`02_TECHNICAL_ARCHITECTURE.md`](02_TECHNICAL_ARCHITECTURE.md), [`VERSION_MATRIX.md`](VERSION_MATRIX.md) y [`DEPENDENCY_REGISTER.md`](DEPENDENCY_REGISTER.md).
4. [`ENGINEERING_STANDARDS.md`](ENGINEERING_STANDARDS.md), [`VALIDATION_PLAYBOOK.md`](VALIDATION_PLAYBOOK.md) y [`.agent/PLANS.md`](../.agent/PLANS.md).
5. [`18_TESTING.md`](18_TESTING.md) para reejecutar `scripts/validate` antes/después del shell.
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

Ejecutar el preflight de F07 y crear ExecPlan solo si el trabajo real cumple los criterios de `.agent/PLANS.md`. Construir el shell/safe area y comparar interacción candidata sin escalar contenido. Consumir `AppContext` mediante fachadas/casos de uso; no convertirlo en global ni mover reglas a `MonoBehaviour`. Mantener Release sin mocks/debug y no crear remoto, push, signing, secretos, publicación ni SDKs.
