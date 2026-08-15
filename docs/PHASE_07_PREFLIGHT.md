# Fase 07 — Preflight scene flow y Addressables locales

Fecha: 2026-08-15 01:07 CST (`America/Mexico_City`). Evidencia observada antes de editar; no sustituye un nuevo preflight.

## Git y estado esperado

| Control | Resultado |
|---|---|
| Directorio | Raíz del repositorio compartido; proyecto Unity existente, no anidado. La ruta personal no se registra. |
| `AGENTS.md` | Existe y se leyó completo; no hubo bloqueo. |
| `git status --short --branch` | `## main`, limpio. |
| `git branch --show-current` | `main`. |
| `git log -1 --format=fuller` | `31a6835ad2558e2cf7afc72a90f6ca21e8fba4c4`, `test(gate-a): audit compilable foundation`. |
| Remoto/diff | Sin remoto; staged/unstaged/untracked vacíos. |
| Gate A | `docs/STATUS.md` y auditoría real declaraban `PASS`, sin Critical/Major abierto. |

Se leyeron completos `AGENTS.md`, `.agent/PLANS.md`, status, arquitectura, decisiones, Gate A, dependencias, validación/testing/Android, mundo y fuentes de contenido. Se inspeccionaron manifest/lock, ProjectVersion, Build Settings, los nueve asmdefs, todo Bootstrap/Infrastructure/Application relacionado, BuildTools, wrappers y tests. No se asumió correcta la fase previa por su reporte.

Estado real inicial: `Bootstrap` era la única escena, no había scene flow ni Addressables, y los docs los declaraban ausentes. No aparecieron cambios ajenos ni colisión.

## Baseline ejecutada antes de editar

| Comando | Resultado |
|---|---|
| `scripts/compile` | `PASS`, 13.881 s. |
| `scripts/test-editmode` | `PASS`, 21/21, 15.031 s. |
| `scripts/test-playmode` | `PASS`, 2/2, 15.274 s. |

## Intake oficial Addressables

Consulta: 2026-08-15. No se usó `latest` en manifest ni versión preview.

| Fuente oficial | Hecho observado | Conclusión |
|---|---|---|
| [Unity Registry](https://packages.unity.com/com.unity.addressables) | `dist-tags.latest=4.0.1`; paquete sin sufijo preview, `unity: 6000.0`, tarball SHA-1 `37a0b4bd16b0a191e1e08e9b62908ca4284b0f76`. | Pin candidato `4.0.1`, compatible con `6000.3.22f1`; aceptación condicionada a pruebas/build. |
| [Manual Addressables 4.0](https://docs.unity3d.com/Packages/com.unity.addressables@4.0/manual/index.html) | API/tooling para carga asíncrona y contenido local/remoto. | Solo se autoriza el modo local; remoto no se configura. |
| [Manual Unity 6.0 del paquete](https://docs.unity3d.com/6000.0/Documentation/Manual/com.unity.addressables.html) | El índice general aún publicaba `2.7.6`. | Se registra la discrepancia; Registry/tarball vigentes y evidencia local prevalecen para el pin explícito. |
| Tarball oficial `4.0.1` | Unity Companion License; transitivo Scriptable Build Pipeline `4.0.0`; sin `.so`, `.aar`, `.dylib` o SDK comercial. | Sin nueva librería nativa/permiso/dato. Módulos UnityWebRequest no se usan para remoto; catálogo/paths se validan locales. |

Impacto infantil: el paquete no incorpora analytics, ads, identificadores, cuenta ni backend. El APK Development conserva `INTERNET` de la baseline Unity, pero runtime settings no tiene endpoint y el arranque offline se prueba. Esto es control técnico, no asesoría legal ni aprobación de store.
