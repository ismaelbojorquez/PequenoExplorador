# ExecPlan — persistencia local versionada, atómica y migrable

- Fase/Gate: Prompt/Fase 09, Gate B
- Estado: Complete
- Creado/actualizado: 2026-08-16 09:40 CST (America/Mexico_City)
- Owner: Senior Game Persistence Engineer

## Propósito y alcance

Crear la primera raíz de progreso local sin gameplay: contrato `PlayerProgress`, puertos de Application, DTO/envelope v1 en Infrastructure, migración pura, checksum, escritura `temp → flush → replace`, backup, recuperación, autosave y herramientas Editor. Quedan fuera cloud, cuentas, cifrado, contenido real, entitlements y UI parental final. Termina con compile, pruebas de fallos/migración, PlayMode, APK Development, documentación y commit aislado.

## Contexto y orientación

Gate A está en `PASS`; `fd791cc7d739c329f638072822ce87762eaaad82` contiene la revalidación y el commit anterior `de04fa5` implementa scene flow/Addressables. La cadena humana actual autoriza Prompt 09 aunque `docs/STATUS.md`/`ROADMAP.md` conservan una asignación histórica distinta para F07/F08; se registrará la desviación sin declarar esas entregas completadas. `docs/10_SAVE_SYSTEM.md` no existía al inicio y se creará como fuente canónica.

La foundation tiene nueve asmdefs. Domain/Application no referencian Unity; Infrastructure puede adaptar `Application.persistentDataPath` y `JsonUtility`; Bootstrap es el único composition root. El archivo preexistente staged `docs/prompts/00_MASTER_CODEX_PROMPTS.md` pertenece al usuario: no se edita ni se incluye en el commit de esta fase.

## Progreso

- [x] 2026-08-16 09:18 CST — preflight, inventario y baseline `scripts/validate` completados; pipeline código 0.
- [x] 2026-08-16 09:27 CST — modelo, puertos, DTO/codec/migración y filesystem atómico implementados.
- [x] 2026-08-16 09:31 CST — lifecycle/autosave, notice recuperable y herramientas Development integrados.
- [x] 2026-08-16 09:33 CST — EditMode `46/46` y PlayMode `5/5` cubren fallos, migración y recreación.
- [x] 2026-08-16 09:40 CST — documentación y `scripts/validate` completos; APK Development generado.
- [x] 2026-08-16 09:42 CST — diff completo revisado y commit F09 preparado por pathspec; el hash se reporta fuera del plan para evitar autorreferencia. El staged ajeno queda fuera.

## Hallazgos

- `docs/10_SAVE_SYSTEM.md` estaba ausente; no había `PlayerProgress`, save, `persistentDataPath` ni tests de persistencia.
- `docs/STATUS.md` mantiene F07 abierta y `ROADMAP.md` asigna F08 al dominio del loop, mientras la cadena maestra llama F08 al scene flow ya implementado. La solicitud humana actual gobierna este incremento; la documentación no afirmará que el loop existe.
- `docs/prompts/00_MASTER_CODEX_PROMPTS.md` ya estaba staged y presenta dos líneas con trailing whitespace. Se preserva fuera del alcance, por lo que puede impedir un árbol global limpio aun con un commit F09 aislado.

## Decisiones

- 2026-08-16 — usar `UnityEngine.JsonUtility`, módulo builtin `com.unity.modules.jsonserialize` `1.0.0` resuelto en el lock. Evita dependencia nueva, funciona en IL2CPP y sus límites son compatibles con DTOs cerrados, arrays, sin diccionarios/polimorfismo ni objetos Unity.
- 2026-08-16 — checksum SHA-256 sobre el payload JSON UTF-8 canónico; no se denomina cifrado ni protege contra manipulación maliciosa.
- 2026-08-16 — una versión futura vuelve el servicio read-only y nunca se sobrescribe; corrupción sin backup válido produce fallo recuperable y conserva evidencia.
- 2026-08-16 — la rotación normal usa replace atómico con backup; restaurar backup reemplaza primary sin rotar el primary corrupto sobre el backup válido.

## Plan de implementación

1. Añadir `PlayerProgress` y settings mínimos en Domain; puertos/resultados/coordinador en Application.
2. Añadir envelope/DTO v1, mapper/codec, migración v0→v1, SHA-256 y `LocalFileStore` en Infrastructure.
3. Componer save/autosave en Bootstrap, añadir checkpoints acotados de pause/quit y herramientas Editor de inspect/reset con confirmación.
4. Añadir fake in-memory con failpoints, pruebas exhaustivas EditMode y persistencia PlayMode con directorio temporal.
5. Completar documentación, matrices y recovery; ejecutar checks, suites y Android smoke.

## Comandos y validación

- `scripts/validate` — baseline 2026-08-16: código 0; compile, Addressables, EditMode 29/29, PlayMode 4/4 y APK Development.
- `scripts/test-editmode` — pendiente tras implementación.
- `scripts/test-playmode` — pendiente tras implementación.
- `scripts/validate` — código 0; `PE_FULL_VALIDATION_OK`, EditMode `46/46`, PlayMode `5/5`, APK Development PASS.
- `git diff --check` / diff cached limitado a F09 — PASS. El diff cached ajeno conserva dos espacios finales y se reporta por separado.

## Recovery y seguridad

No editar `Library`, artefactos ni el archivo staged ajeno. Los cambios se limitan a nuevas carpetas Save/Progress, integración explícita y docs. Si compile falla, ejecutar primero la suite individual y revisar `artifacts/logs`. Los tests físicos usan directorios temporales específicos y los eliminan solo tras validar su ruta. No se crean IDs personales, secretos, rutas personales, red ni permisos.

## Resultados y retrospectiva

Schema v1 y recovery quedaron implementados sin paquete nuevo, PII, red o gameplay. `scripts/validate` terminó código 0: compile, Addressables, EditMode `46/46`, PlayMode `5/5` y APK Development IL2CPP/ARM64 (`60,278,339` bytes; SHA-256 `523ff0d5debf5974643e4106eb8d0743ee03ffdd82e2f9ef4ef6adaf9728e011`). Release guard devolvió el código esperado 3. I/O en dispositivo físico fue `NOT RUN` porque `adb devices` estaba vacío.

Intentos fallidos conservados: compile inicial por referencias asmdef directas faltantes; EditMode inicial `42/43` por exigir la clase exacta `OperationCanceledException` cuando .NET produjo su subtipo válido `TaskCanceledException`. Las correcciones fueron de fronteras/expectativa y las repeticiones completas pasan. Deuda: falta de espacio, power-loss/force-stop real y upgrades publicados se prueban en F34/F35. El archivo staged ajeno impide afirmar árbol global limpio aunque el commit F09 pueda aislarse.
