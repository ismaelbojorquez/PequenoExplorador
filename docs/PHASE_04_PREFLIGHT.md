# Preflight — Fase 04: fronteras modulares

Fecha: 2026-08-14, `America/Mexico_City`. Evidencia tomada antes de editar implementación.

## Estado inicial verificado

- Directorio: raíz esperada del repositorio, confirmada con `pwd` y `git rev-parse --show-toplevel`; la ruta personal no se versiona.
- `AGENTS.md`: presente y leído completo; exige ExecPlan para cambios transversales de asmdefs.
- Git: repositorio válido, rama `main`, árbol limpio (`## main`).
- Último commit: `e51f2502963627c8a988e4ee379ee1f6fa41ebdc`, `chore(unity): create Unity 6 URP Android project`.
- Diff staged/unstaged: vacío. No se detectaron cambios ajenos ni colisiones.
- Foundation: un asmdef runtime, uno Editor y uno EditMode; cuatro scripts C#; una escena Bootstrap habilitada; cero gameplay.

## Lecturas e inspección

Se leyeron completos `AGENTS.md`, `.agent/PLANS.md`, `README.md`, `docs/README.md`, `docs/STATUS.md`, `docs/ROADMAP.md`, `docs/TECHNICAL_ARCHITECTURE.md`, `docs/DECISIONS.md`, `docs/RISK_REGISTER.md`, `docs/ENGINEERING_STANDARDS.md`, `docs/CODE_REVIEW_RULES.md`, `docs/VALIDATION_PLAYBOOK.md`, `docs/DEPENDENCY_REGISTER.md`, `docs/VERSION_MATRIX.md`, `docs/MVP_SCOPE.md`, `docs/CHANGELOG.md`, manifest/lock, todos los scripts/asmdefs y ProjectSettings. Se inspeccionaron inventario, configuración, tests y diff en vez de heredar el reporte de F03.

## Importación y tests antes del cambio

| Comprobación | Resultado | Evidencia |
|---|---|---|
| Unity `6000.3.22f1` import/compile batch | PASS | Código 0; `/tmp/pequeno-phase04-baseline-compile.log`; sin error C# ni `Compilation failed`. |
| EditMode basal | PASS | `/tmp/pequeno-phase04-baseline-editmode.xml`: 2 total, 2 passed, 0 failed. |
| Estado tras import/tests | PASS | Git permaneció limpio; cachés/logs siguieron ignorados. |

El log de cierre batch contiene un mensaje no bloqueante sobre `build-server` y falta de SDK .NET después de `Batchmode quit successfully`; no produjo error de compilación ni salida distinta de cero. Se mantiene como hallazgo, no se oculta ni se interpreta como fallo de C#.

## Desviación de secuencia

El roadmap previo nombraba F04 como prototipos de interacción. La orden vigente requiere primero fronteras de assemblies y excluye gameplay; se ejecuta como alcance de F04 y la interacción se concentra en F07 junto con input. F05 conserva shell/safe area; no se renumeran fases silenciosamente ni se amplía el Vertical Slice.
