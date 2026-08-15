# Preflight — Fase 05: validación, build y CI

Fecha: 2026-08-14, `America/Mexico_City`. Evidencia obtenida antes de editar.

## Estado inicial

- `AGENTS.md` y `docs/STATUS.md`: presentes y leídos completos; ausencia habría bloqueado.
- Git: `main`, árbol limpio, sin diff staged/unstaged.
- Último commit: `7f41246a148cdb1354a885d70feb80f3fa71576a`, `feat(core): establish modular assembly boundaries`.
- Remotos: ninguno (`git remote -v` sin salida); no se crea repo ni se hace push.
- Host: macOS 26.5.2, Darwin ARM64. El repo no versionará hostname ni rutas personales.
- Unity: binario `6000.3.22f1` ejecutable localizado a partir del pin; módulo Android previamente disponible.
- Herramientas: Git 2.50.1, `jq` y Ruby presentes; `shellcheck` y `actionlint` no disponibles.

## Lecturas e inspección

Se leyeron completos `AGENTS.md`, `.agent/PLANS.md`, validation playbook, arquitectura canónica, Android release, status, estándares/review, decisiones, riesgos, roadmap, README, ProjectVersion, manifest/lock, `.gitignore`, configuración raíz, todos los scripts Editor y tests. Se confirmó que no existen `.github/`, `scripts/` o `Assets/_Game/Editor/BuildTools`.

## Baseline ejecutada

| Control | Resultado | Tiempo real | Evidencia |
|---|---|---:|---|
| Import/compile Unity | PASS | 16.64 s | Código 0; sin error C#. |
| Fronteras | PASS | 12.58 s | `assemblies=9 cycles=0`. |
| EditMode | PASS | 20.31 s | 5/5, 0 failed. |
| PlayMode | PASS | 26.18 s | 1/1, 0 failed. |

Git permaneció limpio tras ejecutar Unity. Los PASS previos quedaron así contrastados, no heredados del reporte F04.

## Actions y licencias

Antes de crear el workflow se verificaron repositorios oficiales, tags y licencias:

| Action | Tag | SHA completo | Licencia | Fuente oficial |
|---|---|---|---|---|
| `actions/checkout` | `v4.3.1` | `34e114876b0b11c390a56381ad16ebd13914f8d5` | MIT | `https://github.com/actions/checkout` |
| `actions/upload-artifact` | `v4.6.2` | `ea165f8d65b6e75b540449e92b4886f43607fa02` | MIT | `https://github.com/actions/upload-artifact` |

No se autoriza Action de Unity de terceros. El job Unity será manual/self-hosted y quedará pendiente de repo, runner y activación humana.
