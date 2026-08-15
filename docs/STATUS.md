# Estado vivo del proyecto

Actualizado: 2026-08-14 21:36 (`America/Mexico_City`). Git y archivos observados prevalecen si contradicen este resumen y obligan a actualizarlo.
Las evidencias `PHASE_00/01_PREFLIGHT.md` conservan referencias históricas al roadmap vigente cuando se escribieron; este archivo y `ROADMAP.md` contienen la asignación actual.

## Fase y Gate

- **Fase actual:** 03 — foundation Unity 6 URP reproducible y baseline móvil.
- **Estado de fase:** completa tras su commit; proyecto/import/tests/APK/emulador documentados.
- **Gate actual:** A — Foundation ready, **completo** bajo el alcance refinado del roadmap; el AAB Release pertenece a F11.
- **Siguiente fase:** 04 — prototipos de interacción y playtests seguros, sin escalar contenido.
- **ExecPlan activo:** ninguno. El plan cerrado queda en [`.agent/execplans/03-project-foundation.md`](../.agent/execplans/03-project-foundation.md).

## Capacidades verificadas

| Capacidad | Estado | Evidencia/limitación |
|---|---|---|
| Git baseline | Verificado | Fases 00–02 eran la base limpia; F03 debe confirmarse con último commit/status en cada preflight. |
| Producto/Vertical Slice/MVP | Especificado | Fuentes canónicas en visión, GDD y `MVP_SCOPE.md`; no implementado. |
| Contrato de agentes | Especificado | `AGENTS`, planes, estándares, review y playbook de F02. |
| Proyecto Unity | Verificado | `6000.3.22f1`, URP, Bootstrap y estructura mínima en raíz. |
| Compilación/tests Unity | `PASS` | Import batch código `0`; EditMode `2/2`. |
| Build Android/iOS | Android `PASS`; iOS `NOT RUN` | APK Development IL2CPP/ARM64/API36 ejecutado en emulador 16 KB; módulo iOS no instalado. |
| Paquetes | Verificado | Cuatro pins directos oficiales; lock y intake documentados; sin preview/SDK comercial. |
| Gameplay/assets finales | No existen | Prohibido escalar contenido antes del Vertical Slice. |

## Fuentes necesarias para Fase 04

1. [`../AGENTS.md`](../AGENTS.md) y este estado.
2. [`ROADMAP.md`](ROADMAP.md), [`DECISIONS.md`](DECISIONS.md) y [`RISK_REGISTER.md`](RISK_REGISTER.md).
3. [`TECHNICAL_ARCHITECTURE.md`](TECHNICAL_ARCHITECTURE.md), [`VERSION_MATRIX.md`](VERSION_MATRIX.md) y [`DEPENDENCY_REGISTER.md`](DEPENDENCY_REGISTER.md).
4. [`ENGINEERING_STANDARDS.md`](ENGINEERING_STANDARDS.md), [`VALIDATION_PLAYBOOK.md`](VALIDATION_PLAYBOOK.md) y [`.agent/PLANS.md`](../.agent/PLANS.md).
5. [`MVP_SCOPE.md`](MVP_SCOPE.md) para recordar que la foundation no autoriza producción masiva ni gameplay fuera del slice.

## Bloqueos y decisiones humanas

- Licencia Unity batch: verificada con un proyecto desechable; no es bloqueo.
- Android Build Support y toolchain bundled están verificados mediante smoke real. La inspección inicial de ruta fue corregida en `PHASE_03_PREFLIGHT.md`.
- Licencia/titularidad del producto y terceros: H-001, bloquea incorporación de dependencias/assets dudosos.
- Bundle ID/company definitivos requieren decisión humana antes de stores; los placeholders son no publicables.
- Xcode y módulo iOS no disponibles localmente; no bloquean F04, sí futura evidencia iOS.
- Especialista factual, protocolo de playtests, territorios, modelo comercial y stores siguen pendientes; no son bloqueos para crear una foundation vacía.

## Reanudación inmediata

Ejecutar preflight y contrastar último commit/status; repetir import y EditMode; leer alcance F04 y arquitectura. Crear ExecPlan si el prototipo cruza input/escena/UX/playtest. Implementar solo candidatos de interacción del Vertical Slice; no gameplay masivo, SDKs, publicación ni términos.
