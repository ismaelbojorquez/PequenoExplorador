# ExecPlan — fronteras modulares compilables y verificadas

- Fase/Gate: 04 / B
- Estado: Complete
- Creado/actualizado: 2026-08-14 22:20 America/Mexico_City
- Owner: Principal Unity Architect

## Propósito y alcance

Sustituir el asmdef runtime único de la foundation por nueve assemblies pragmáticos: Domain, Application, Content, Infrastructure, Presentation, Bootstrap, Editor, Tests.EditMode y Tests.PlayMode. El resultado debe compilar sin ciclos, demostrar la dirección de dependencias con markers mínimos, conservar exactamente el diagnóstico visual temporal y bloquear por test/CLI referencias prohibidas. No se implementan gameplay, scene flow, save, UI de producto, servicios concretos, SDKs ni dependencias.

Criterios de aceptación: Domain y Application sin `UnityEngine`; Presentation sin Infrastructure ni adaptadores de plataforma; Bootstrap como único composition root; Editor fuera del player; 8–12 assemblies; compile, EditMode, PlayMode y Android smoke ejecutados o reportados honestamente; documentación y Git coherentes.

## Contexto y orientación

La base es el commit `e51f2502963627c8a988e4ee379ee1f6fa41ebdc` en `main`, limpio al iniciar. Unity está fijado por ADR-0001 y `ProjectVersion.txt` en `6000.3.22f1`; paquetes exactos en `Packages/manifest.json`. La foundation tiene un asmdef runtime, uno Editor y uno EditMode; `Assets/_Game/Bootstrap/Bootstrap.unity` es el único entry point y `DiagnosticBootstrap` no contiene gameplay.

Fuentes canónicas: `AGENTS.md`, `docs/STATUS.md`, `docs/ROADMAP.md`, `docs/02_TECHNICAL_ARCHITECTURE.md`, `docs/ENGINEERING_STANDARDS.md`, `docs/VALIDATION_PLAYBOOK.md`, `docs/DECISIONS.md` y `docs/RISK_REGISTER.md`.

## Progreso

- [x] 2026-08-14 21:57 — preflight Git/documentación/configuración/código completado; árbol limpio y commit F03 contrastado.
- [x] 2026-08-14 21:57 — import/compile basal código 0 y EditMode basal 2/2.
- [x] 2026-08-14 22:07 — nueve assemblies, markers, validador y suites implementados.
- [x] 2026-08-14 22:13 — compile, validador CLI, EditMode, PlayMode y Android smoke pasaron.
- [x] 2026-08-14 22:20 — fuentes de verdad y validaciones estáticas actualizadas; diff/commit son el cierre inmediato.

## Hallazgos

- La foundation compila y sus dos tests EditMode pasan antes del cambio. El cierre batch registra ruido de build-server/.NET SDK después de la salida correcta, sin error de compilación ni código distinto de cero.
- `docs/ROADMAP.md` asignaba F04 a prototipos de interacción. La solicitud vigente inserta primero las fronteras modulares; se documentará la desviación sin implementar interacción ni renumerar las 58 fases.
- El primer import posterior detectó referencias duplicadas a TestRunner: `optionalUnityReferences: TestAssemblies` ya aporta los runners en Test Framework 1.6. Se retiraron las referencias explícitas, sin recurrir a `overrideReferences`; el fallo queda registrado y se repite compile.
- La primera ejecución del validador rechazó nombres de assemblies de test que no coincidían con sus namespaces. Se normalizaron a `PequenoExplorador.Tests.EditMode` y `PequenoExplorador.Tests.PlayMode`, alineados con el requisito y la convención.
- La primera suite EditMode ejecutó 5 tests y falló 1 por una limitación de `Has.Count` sobre `IReadOnlyList` en NUnit incluido; las cuatro reglas restantes pasaron. La aserción se cambió a comparar `definitions.Count`, sin debilitar el criterio.

## Decisiones

- 2026-08-14 — usar exactamente nueve assemblies. Es suficiente para expresar las capas y separar tests/Editor sin fragmentar por feature inexistente.
- 2026-08-14 — validar archivos asmdef reales y una fixture inválida en memoria. Esto prueba tanto configuración como lógica de rechazo sin romper temporalmente el proyecto.
- 2026-08-14 — conservar la escena y `DiagnosticBootstrap` sin cambios funcionales; Bootstrap es el único assembly autorizado a conocer los adaptadores concretos futuros.

## Plan de implementación

1. Crear un asmdef por capa y markers mínimos que prueben referencias permitidas; retirar el asmdef runtime monolítico.
2. Ajustar Editor/EditMode, añadir PlayMode y crear un validador Editor con reglas puras, carga de asmdefs y entry point CLI.
3. Probar el grafo real, ciclo, aislamiento de Unity/Editor y una referencia Presentation→Infrastructure inválida mediante fixture controlada.
4. Importar, compilar, ejecutar ambas suites y repetir el APK Development fuera del worktree.
5. Actualizar arquitectura, estándares, playbook, decisiones, changelog, estado, índice y evidencia; revisar diff y cerrar con el commit solicitado.

## Comandos y validación

- `git status --short --branch` — confirmó `## main` limpio al inicio.
- `git branch --show-current` — confirmó `main`.
- `git log -1 --format=fuller` — confirmó el commit F03 esperado.
- `"$UNITY_EDITOR" -batchmode -nographics -quit -projectPath "$(pwd)" -logFile /tmp/pequeno-phase04-baseline-compile.log` — PASS, código 0.
- `"$UNITY_EDITOR" -batchmode -nographics -projectPath "$(pwd)" -runTests -testPlatform EditMode -testResults /tmp/pequeno-phase04-baseline-editmode.xml -logFile /tmp/pequeno-phase04-baseline-editmode.log` — PASS, 2/2.
- Primer compile posterior — FAIL, referencias duplicadas de TestRunner; configuración corregida antes de continuar.
- Segundo compile posterior — PASS, código 0.
- Primera ejecución del validador — FAIL, nombres/namespace de tests incoherentes; configuración corregida.
- Primer EditMode posterior — FAIL, 4/5; fallo de sintaxis de aserción compatible corregido.
- Compile final — PASS, código 0, sin errores C#.
- Validador CLI final — PASS, `PE_ASSEMBLY_BOUNDARIES_OK assemblies=9 cycles=0`.
- EditMode final — PASS, 5/5; incluye grafo real, fixture Presentation→Infrastructure y ciclo controlado.
- PlayMode final — PASS, 1/1; escena Bootstrap mantiene diagnóstico temporal.
- Android Development — PASS; APK `57,046,302 bytes`, SHA-256 `a4572df93cbcda6aaa07369f5edd0a0e77ca51e3ed1f6dc50fef463b52a4903b`, API 26/36, IL2CPP/ARM64, `zipalign -P 16`.
- Emulador Android 16 KB — PASS; instalación/launch, page size 16384, proceso activo, diagnóstico landscape visible y sin fatal en logcat.
- Enlaces/UTF-8/asmdef JSON/metas/basura/secretos/diff/LFS — PASS; `git lfs fsck OK`.

## Recovery y seguridad

No borrar assets válidos ni tocar paquetes/ProjectSettings salvo cambio necesario y revisado. Los nuevos archivos son aditivos salvo el reemplazo acotado del asmdef monolítico; Git conserva su recuperación. Los builds/logs/resultados van a `/tmp`. Si Unity falla, detenerse con el último estado compilable, registrar el error y no presentar `NOT RUN` como PASS. No hacer push, publicar, aceptar términos ni usar rutas personales dentro de archivos ejecutables del proyecto.

## Resultados y retrospectiva

El runtime monolítico fue sustituido por seis capas físicas y tres assemblies de tooling/tests. Domain/Application no ven Unity; Presentation solo ve Application; Bootstrap es el único runtime que ve los tres adaptadores. Editor y tests no aparecen en `ManagedStripped` del player. El validador real y las fixtures controladas evitan ciclos, nuevas referencias no revisadas y `overrideReferences`.

Los dos fallos intermedios de configuración de TestRunner/naming y el fallo de aserción NUnit quedaron corregidos y registrados; el estado final pasa compile, CLI, 5 EditMode, 1 PlayMode y Android. No se añadieron gameplay, dependencias ni permisos. Deuda: los markers se retiran o reemplazan cuando cada capa obtenga un contrato real; toda modificación del grafo exige decisión y actualización del guardrail. Siguiente trabajo: F05 shell/safe area, manteniendo F07 como gate de interacción/input.
