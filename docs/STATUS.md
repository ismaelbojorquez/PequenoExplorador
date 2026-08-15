# Estado vivo del proyecto

Actualizado: 2026-08-14 (`America/Mexico_City`). Esta es la primera lectura después de `AGENTS.md`; Git y archivos observados prevalecen si la contradicen y obligan a actualizarla.
Las evidencias `PHASE_00/01_PREFLIGHT.md` conservan referencias históricas al roadmap vigente cuando se escribieron; este archivo y `ROADMAP.md` contienen la asignación actual.

## Fase y Gate

- **Fase actual:** 02 — contrato operativo de Codex y estándares de ingeniería.
- **Estado de fase:** completa documentalmente al cerrar su commit; verificar con `git log -1` y `git status`.
- **Gate actual:** A — Foundation ready, **en progreso**. La documentación no cierra el Gate.
- **Siguiente fase:** 03 — crear la foundation Unity reproducible, cerrar ADR del Editor, establecer capas/asmdefs/composition root y tests mínimos, y ejecutar smoke AAB vacío.
- **ExecPlan activo:** ninguno. Fase 03 debe evaluar y probablemente crear `.agent/execplans/03-project-foundation.md` antes de cambios materiales por ser transversal y riesgosa.

## Capacidades verificadas

| Capacidad | Estado | Evidencia/limitación |
|---|---|---|
| Git baseline | Verificado | Fases 00–01 commiteadas al entrar en F02; ejecutar preflight para hash/limpieza actual. |
| Producto/Vertical Slice/MVP | Especificado | Fuentes canónicas en visión, GDD y `MVP_SCOPE.md`; no implementado. |
| Contrato de agentes | Especificado | `AGENTS`, planes, estándares, review y playbook de F02. |
| Proyecto Unity | No existe | Sin `Assets/`, `Packages/` ni `ProjectSettings/`. |
| Compilación/tests Unity | `NOT RUN` | Proyecto y comandos reproducibles inexistentes. |
| Build Android/iOS | `NOT RUN` | No hay proyecto; iOS además carece de toolchain local completo según matriz. |
| Gameplay/assets finales/dependencias | No existen | Prohibido escalar contenido antes del Vertical Slice. |

## Fuentes necesarias para Fase 03

1. [`../AGENTS.md`](../AGENTS.md) y este estado.
2. [`ROADMAP.md`](ROADMAP.md), [`DECISIONS.md`](DECISIONS.md) y [`RISK_REGISTER.md`](RISK_REGISTER.md).
3. [`VERSION_MATRIX.md`](VERSION_MATRIX.md) y [`POLICY_SOURCE_REGISTER.md`](POLICY_SOURCE_REGISTER.md), revalidadas por temporalidad.
4. [`ENGINEERING_STANDARDS.md`](ENGINEERING_STANDARDS.md), [`VALIDATION_PLAYBOOK.md`](VALIDATION_PLAYBOOK.md) y [`.agent/PLANS.md`](../.agent/PLANS.md).
5. [`MVP_SCOPE.md`](MVP_SCOPE.md) para recordar que la foundation no autoriza producción masiva ni gameplay fuera del slice.

## Bloqueos y decisiones humanas

- Activación/licencia de Unity para batch build: pendiente de verificación en F03.
- Licencia/titularidad del producto y terceros: H-001, bloquea incorporación de dependencias/assets dudosos.
- Xcode y módulo iOS no disponibles localmente; no bloquean Android-first F03, sí futura evidencia iOS.
- Especialista factual, protocolo de playtests, territorios, modelo comercial y stores siguen pendientes; no son bloqueos para crear una foundation vacía.

## Reanudación inmediata

Ejecutar el preflight de `AGENTS`; confirmar árbol limpio y último commit; revalidar Editor/toolchain/fuentes oficiales; decidir/crear el ExecPlan real de F03; y no crear el proyecto hasta que pin, licencia y alcance de la foundation estén registrados. No instalar paquetes ni aceptar términos silenciosamente.
