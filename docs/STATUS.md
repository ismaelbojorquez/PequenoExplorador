# Estado vivo del proyecto

Actualizado: 2026-08-14 23:01 (`America/Mexico_City`). Git y archivos observados prevalecen si contradicen este resumen y obligan a actualizarlo.
Las evidencias `PHASE_00/01_PREFLIGHT.md` conservan referencias históricas al roadmap vigente cuando se escribieron; este archivo y `ROADMAP.md` contienen la asignación actual.

## Fase y Gate

- **Fase actual:** 05 — completada; pipeline reproducible local/CI sin gameplay.
- **Estado de fase:** completa en commit F05; CI remota permanece `NOT RUN` hasta que exista remoto/runner.
- **Gate actual:** B — Vertical slice playable, iniciado; F05 aporta evidencia, no implementa el slice.
- **Siguiente fase:** 06 — shell landscape, navegación, safe areas y estados vacío/error.
- **ExecPlan activo:** ninguno. Plan F05 cerrado: [`.agent/execplans/05-validation-pipeline.md`](../.agent/execplans/05-validation-pipeline.md).

## Capacidades verificadas

| Capacidad | Estado | Evidencia/limitación |
|---|---|---|
| Git baseline | Verificado | Fases 00–05 forman la base; debe contrastarse en cada preflight. |
| Producto/Vertical Slice/MVP | Especificado | Fuentes canónicas en visión, GDD y `MVP_SCOPE.md`; no implementado. |
| Contrato de agentes | Especificado | `AGENTS`, planes, estándares, review y playbook de F02. |
| Proyecto Unity | Verificado | `6000.3.22f1`, URP, Bootstrap y estructura mínima en raíz. |
| Pipeline local | `PASS` | `scripts/validate`, código `0`: checks, compile/contenido/fronteras, EditMode `5/5`, PlayMode `1/1` y APK Development. JUnit/logs/manifests en `artifacts/`. |
| Fronteras/compilación | `PASS` | Nueve asmdefs, allowlist/cycles y placeholder validados; Domain/Application sin engine. |
| Tests Unity | `PASS` | EditMode `5/5`; PlayMode `1/1`; fixture inválida y diagnóstico temporal cubiertos. |
| Build Android/iOS | Android Development `PASS`; Release `BLOCKED`; iOS `NOT RUN` | APK F05 `57,046,302 bytes`, SHA-256 `3d0a7385…f014b479`, IL2CPP/ARM64 API 26/36 y zipalign 16 KB. Release exige signing externo; módulo iOS no instalado. |
| CI GitHub | `NOT RUN` | Workflow estático + Unity manual/self-hosted definidos; no hay remoto, runner ni activación CI. |
| Paquetes | Verificado | Cuatro pins directos oficiales; lock y intake documentados; sin preview/SDK comercial. |
| Gameplay/assets finales | No existen | Prohibido escalar contenido antes del Vertical Slice. |

## Fuentes necesarias para Fase 06

1. [`../AGENTS.md`](../AGENTS.md) y este estado.
2. [`ROADMAP.md`](ROADMAP.md), [`DECISIONS.md`](DECISIONS.md) y [`RISK_REGISTER.md`](RISK_REGISTER.md).
3. [`02_TECHNICAL_ARCHITECTURE.md`](02_TECHNICAL_ARCHITECTURE.md), [`VERSION_MATRIX.md`](VERSION_MATRIX.md) y [`DEPENDENCY_REGISTER.md`](DEPENDENCY_REGISTER.md).
4. [`ENGINEERING_STANDARDS.md`](ENGINEERING_STANDARDS.md), [`VALIDATION_PLAYBOOK.md`](VALIDATION_PLAYBOOK.md) y [`.agent/PLANS.md`](../.agent/PLANS.md).
5. [`18_TESTING.md`](18_TESTING.md) para reejecutar `scripts/validate` antes/después del shell.
6. [`MVP_SCOPE.md`](MVP_SCOPE.md) para recordar que el shell no autoriza producción masiva ni gameplay fuera del slice.

## Bloqueos y decisiones humanas

- Licencia Unity batch: verificada con un proyecto desechable; no es bloqueo.
- Android Build Support y toolchain bundled están verificados mediante smoke real. La inspección inicial de ruta fue corregida en `PHASE_03_PREFLIGHT.md`.
- Licencia/titularidad del producto y terceros: H-001, bloquea incorporación de dependencias/assets dudosos.
- Bundle ID/company definitivos requieren decisión humana antes de stores; los placeholders son no publicables.
- Xcode y módulo iOS no disponibles localmente; no bloquean F06, sí futura evidencia iOS.
- GitHub remoto, runner Unity y activación CI no existen; no bloquean desarrollo local, sí evidencia Unity remota.
- Especialista factual, protocolo de playtests, territorios, modelo comercial y stores siguen pendientes; no son bloqueos para crear una foundation vacía.

## Reanudación inmediata

Ejecutar el preflight y `scripts/validate`; después implementar solo el shell F06 detrás de Presentation/Application y componerlo en Bootstrap. Mantener Release bloqueado, placeholder identificado y cero gameplay/input definitivo. No crear remoto, push, signing, secretos, publicación ni SDKs.
