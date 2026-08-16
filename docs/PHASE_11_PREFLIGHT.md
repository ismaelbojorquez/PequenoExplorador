# Preflight Prompt 11 — localización ES/EN

Fecha: 2026-08-16, `America/Mexico_City`.

## Estado inicial observado

- Directorio: raíz de `PequenoExplorador`; `AGENTS.md` y `docs/STATUS.md` presentes y leídos completos.
- Git: rama `main`; `git status --short --branch` mostró `## main...origin/main [ahead 2]` sin cambios; HEAD `d201d2d349ad65f63a039d55615b27b84fa6fe05` (`docs(status): record configured GitHub origin`). No había diff staged/unstaged.
- Estado contrastado: config Development/Release, save schema v1, Bootstrap y scene flow local existen; no existe gameplay. `Packages`, nueve asmdefs, ProjectSettings, Bootstrap/Content/Infrastructure/Presentation, escenas y tests fueron inventariados.
- Baseline real: `scripts/validate` devolvió `0` en 82.01 s; repository checks, compile/import, Addressables local, EditMode `57/57`, PlayMode `5/5` y APK Development pasaron.

## Lectura e inspección

Se leyeron completos `AGENTS.md`, `.agent/PLANS.md`, `docs/STATUS.md`, `docs/README.md`, `docs/14_UI_UX.md`, `docs/16_AUDIO.md`, `docs/CONTENT_PIPELINE.md`, `docs/CONTENT_SOURCES.md`, `docs/DECISIONS.md`, `docs/DEPENDENCY_REGISTER.md`, `docs/VERSION_MATRIX.md`, `docs/02_TECHNICAL_ARCHITECTURE.md`, `docs/RUNTIME_CONFIGURATION.md`, `docs/10_SAVE_SYSTEM.md`, `docs/18_TESTING.md`, `docs/VALIDATION_PLAYBOOK.md`, `docs/RISK_REGISTER.md`, `docs/ENGINEERING_STANDARDS.md`, `docs/CODE_REVIEW_RULES.md` y Prompt 11 del catálogo maestro.

No existía documento, paquete, settings, tabla, locale, servicio ni test de localización. El inventario de textos visibles encontró 13 valores hardcodeados en `Bootstrap.unity`, `Camp.unity`, `Jungle.unity`, `BootstrapStatusView` y `SceneTransitionView`; incluye producto/versión, aviso temporal, botones, mundos, carga/error y estados de arranque/save. Las strings de logs/errores/tests no son copy visible, pero se revisarán para no confundirlas con contenido.

## Intake oficial del paquete

Consulta: 2026-08-16.

| Requisito | Fuente oficial | Resultado | Impacto |
|---|---|---|---|
| Versión estable exacta | `https://packages.unity.com/com.unity.localization` | `dist-tags.latest=1.5.12`; tarball SHA-1 `b0a588a05f2a20af8e4afc33cf1c4591b7df5a28`. | Pin exacto; sin `latest`/rango/preview en manifest. |
| Compatibilidad/capacidades | `https://docs.unity3d.com/Packages/com.unity.localization@1.5/manual/index.html` | La documentación servida corresponde a `1.5.12` y cubre strings, Smart Strings/plurales, assets, pseudo e import/export. | Satisface el alcance sin SDK externo. |
| Licencia | `https://docs.unity3d.com/Packages/com.unity.localization@1.5/license/LICENSE.html` | Unity Companion License para proyectos dependientes de Unity. | No selecciona licencia del producto; H-001 sigue pendiente. |
| Transitivos declarados | Registry oficial | Addressables `1.25.0` y Newtonsoft JSON `3.0.2`; el proyecto fija Addressables directo `4.0.1`. | Import debe demostrar resolución exacta compatible y conservar el pin directo. |

No se observó necesidad de red runtime, permiso sensible ni binario nativo en el propósito del paquete. Esto se verificará sobre cache/lock y APK. Compatibilidad real con Unity `6000.3.22f1`, IL2CPP/ARM64 y coexistencia con Addressables `4.0.1` permanecen pendientes hasta import/tests/build; no se declaran PASS por documentación.

## Decisión de ejecución

La fase cruza paquete, save, assemblies, Content, Bootstrap, Presentation, escenas, Editor, tests y build. `.agent/PLANS.md` exige ExecPlan; se creó [`../.agent/execplans/p11-localization-pipeline.md`](../.agent/execplans/p11-localization-pipeline.md). No había cambios ajenos ni colisión. No se hizo instalación global, push, signing, publicación ni cambio de sistema.
