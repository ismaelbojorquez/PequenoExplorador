# Estado vivo del proyecto

Actualizado: 2026-08-14 22:17 (`America/Mexico_City`). Git y archivos observados prevalecen si contradicen este resumen y obligan a actualizarlo.
Las evidencias `PHASE_00/01_PREFLIGHT.md` conservan referencias históricas al roadmap vigente cuando se escribieron; este archivo y `ROADMAP.md` contienen la asignación actual.

## Fase y Gate

- **Fase actual:** 04 — fronteras modulares de assemblies, sin gameplay.
- **Estado de fase:** completa al crear su commit; nueve assemblies, enforcement, tests y Android smoke verificados.
- **Gate actual:** B — Vertical slice playable, iniciado; F04 solo establece fronteras y no implementa el slice.
- **Siguiente fase:** 05 — shell landscape, navegación uGUI/TMP, safe areas y estados vacío/error, sin escalar contenido.
- **ExecPlan activo:** ninguno. Plan cerrado: [`.agent/execplans/04-modular-assembly-boundaries.md`](../.agent/execplans/04-modular-assembly-boundaries.md).

## Capacidades verificadas

| Capacidad | Estado | Evidencia/limitación |
|---|---|---|
| Git baseline | Verificado | Fases 00–03 son la base; F04 queda completa al commit y debe contrastarse en cada preflight. |
| Producto/Vertical Slice/MVP | Especificado | Fuentes canónicas en visión, GDD y `MVP_SCOPE.md`; no implementado. |
| Contrato de agentes | Especificado | `AGENTS`, planes, estándares, review y playbook de F02. |
| Proyecto Unity | Verificado | `6000.3.22f1`, URP, Bootstrap y estructura mínima en raíz. |
| Fronteras/compilación | `PASS` | Nueve asmdefs, allowlist/cycles CLI, compile batch código `0`; Domain/Application sin engine. |
| Tests Unity | `PASS` | EditMode `5/5`; PlayMode `1/1`; fixture inválida y diagnóstico temporal cubiertos. |
| Build Android/iOS | Android `PASS`; iOS `NOT RUN` | APK F04 `57,046,302 bytes`, IL2CPP/ARM64/API36 ejecutado en emulador 16 KB; módulo iOS no instalado. |
| Paquetes | Verificado | Cuatro pins directos oficiales; lock y intake documentados; sin preview/SDK comercial. |
| Gameplay/assets finales | No existen | Prohibido escalar contenido antes del Vertical Slice. |

## Fuentes necesarias para Fase 05

1. [`../AGENTS.md`](../AGENTS.md) y este estado.
2. [`ROADMAP.md`](ROADMAP.md), [`DECISIONS.md`](DECISIONS.md) y [`RISK_REGISTER.md`](RISK_REGISTER.md).
3. [`02_TECHNICAL_ARCHITECTURE.md`](02_TECHNICAL_ARCHITECTURE.md), [`VERSION_MATRIX.md`](VERSION_MATRIX.md) y [`DEPENDENCY_REGISTER.md`](DEPENDENCY_REGISTER.md).
4. [`ENGINEERING_STANDARDS.md`](ENGINEERING_STANDARDS.md), [`VALIDATION_PLAYBOOK.md`](VALIDATION_PLAYBOOK.md) y [`.agent/PLANS.md`](../.agent/PLANS.md).
5. [`MVP_SCOPE.md`](MVP_SCOPE.md) para recordar que el shell no autoriza producción masiva ni gameplay fuera del slice.

## Bloqueos y decisiones humanas

- Licencia Unity batch: verificada con un proyecto desechable; no es bloqueo.
- Android Build Support y toolchain bundled están verificados mediante smoke real. La inspección inicial de ruta fue corregida en `PHASE_03_PREFLIGHT.md`.
- Licencia/titularidad del producto y terceros: H-001, bloquea incorporación de dependencias/assets dudosos.
- Bundle ID/company definitivos requieren decisión humana antes de stores; los placeholders son no publicables.
- Xcode y módulo iOS no disponibles localmente; no bloquean F05, sí futura evidencia iOS.
- Especialista factual, protocolo de playtests, territorios, modelo comercial y stores siguen pendientes; no son bloqueos para crear una foundation vacía.

## Reanudación inmediata

Ejecutar el preflight y contrastar el commit/status; repetir compile, validador, EditMode y PlayMode. Leer F05 y la arquitectura canónica; crear ExecPlan si UI/safe area cruza múltiples escenas/sistemas. Implementar solo shell, navegación y estados temporales detrás de Presentation/Application; no gameplay, input definitivo, SDKs, publicación ni términos.
