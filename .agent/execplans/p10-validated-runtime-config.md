# ExecPlan — perfiles runtime tipados y validados

- Fase/Gate: Prompt 10; Gate B (no equivale a Fase 10 del roadmap histórico)
- Estado: Complete
- Creado/actualizado: 2026-08-16 10:14 America/Mexico_City
- Owner: Game Configuration Architect

## Propósito y alcance

Introducir configuración local mínima, tipada e inmutable para Development/Release, con ScriptableObjects de authoring en Content, mapping hacia Application, selección exclusiva en Bootstrap y validación que bloquee builds inseguros. Incluye nombre/versión técnica, seed, timeout de scene flow, debounce de autosave y flags ya usados por diagnóstico/mocks/fallo simulado. Excluye remote config, red, secrets, tuning de gameplay, preferencias parentales, permisos y cambios de paquetes.

Criterios: ambos perfiles existen una vez; Release no habilita diagnóstico, mocks, fallo simulado, cheats ni bypass parental; Application consume interfaces readonly; Save conserva solo preferencias mutables; tests EditMode/PlayMode y pipeline completo pasan; Release valida config y permanece bloqueado por signing.

## Contexto y orientación

HEAD inicial `aea45b53cd694813fba2c4c984a94a17f550127f`, rama `main`, árbol limpio. Unity `6000.3.22f1`. Fuentes: `AGENTS.md`, `docs/STATUS.md`, `docs/02_TECHNICAL_ARCHITECTURE.md`, `docs/10_SAVE_SYSTEM.md`, `docs/DECISIONS.md`, `docs/ENGINEERING_STANDARDS.md`, `docs/VALIDATION_PLAYBOOK.md` y Prompt 10 del catálogo. El roadmap canónico llama Fase 10 al audio; este incremento solo completa Prompt 10 de la cadena y no altera esa fase histórica.

Rutas clave: `Assets/_Game/Application/Configuration`, `Assets/_Game/Content/Configuration`, `Assets/_Game/Bootstrap`, `Assets/_Game/Editor/BuildTools` y suites bajo `Assets/_Game/Tests`.

## Progreso

- [x] 2026-08-16 09:57 — preflight, inventario de hardcodes y baseline completos.
- [x] 2026-08-16 10:05 — contratos Application, authoring/mapping Content y assets Development/Release implementados.
- [x] 2026-08-16 10:09 — selección/override Bootstrap y validación Editor/CLI/build integrados; compile incremental repetido `PASS`.
- [x] 2026-08-16 10:12 — tests de defaults, mapping, invalidez, Release seguro y perfil PlayMode añadidos; EditMode `56/56` y PlayMode `5/5` antes del caso adicional de budgets.
- [x] 2026-08-16 10:14 — fallo controlado, pipeline completo, guard Release, permisos y documentación cerrados.

## Hallazgos

- Baseline `scripts/validate` código `0` en 1:31.47; EditMode `46/46`, PlayMode `5/5`, APK `60,278,339` bytes SHA-256 `717c65eb16b6ccc3baa13ce170edc2b5efdf097ea60824d81b860e4e1d432cfc`.
- Primer compile incremental `FAIL`: la nueva referencia directa Editor→Application hizo que el namespace `PequenoExplorador.Application` ocultara llamadas `UnityEngine.Application` preexistentes. Se calificaron 11 usos; no era un fallo de assets ni runtime y el intento no cuenta como PASS.
- Fallo controlado de seguridad: se habilitó temporalmente `MockAds` en Release; `scripts/compile` devolvió `2` con `CONFIG008 Release forbids feature flag MockAds.`. El asset se restauró inmediatamente a cero flags antes de continuar.
- La selección actual deriva solo de `UNITY_EDITOR || PE_DEVELOPMENT_SERVICES`; seed `20260814`, scene timeout `20 s`, autosave debounce `500 ms`, producto/versión y diagnóstico están hardcodeados en Bootstrap.
- `PlayerSettings.bundleVersion` ya es `0.1.0`; min/target API y perfiles Addressables son build/content config distinta y no se migran a AppConfig.
- El build Release está deliberadamente bloqueado antes de validar contenido; debe validar ambos perfiles primero y después conservar el código `3` por signing.

## Decisiones

- 2026-08-16 — usar enums con valores numéricos explícitos como IDs estables de perfil/flag y assets locales `Resources` bajo Content; Bootstrap será el único loader runtime.
- 2026-08-16 — conservar configuración build-time Android/Addressables y preferencias parentales fuera de AppConfig; evita mezclar autoridad y duplicar Save.
- 2026-08-16 — no añadir paquete: ScriptableObject + BCL cubren el caso offline/IL2CPP.

## Plan de implementación

1. Crear `IAppConfig`, flags readonly, IDs y validación pura en Application.
2. Crear `AppConfigAsset`/mapper y dos assets locales en Content con valores mínimos actuales.
3. Reemplazar `BootstrapConfiguration`/hardcodes por loader tipado, override temporal de tests y flags explícitos.
4. Integrar validador de perfiles en `scripts/validate-content`, Development build y Release guard.
5. Ampliar tests y documentación; revisar assemblies/diff, ejecutar pipeline y commit solicitado.

## Comandos y validación

- `scripts/validate` — baseline `PASS`, código `0`, 1:31.47.
- `scripts/compile` — primer intento incremental `FAIL`, código `1`, colisión de nombre `Application`; corrección aplicada y repetición pendiente.
- `scripts/compile` con fixture Release insegura — `FAIL` esperado, código `2`, `CONFIG008`; fixture retirada.
- `scripts/compile` — repetición tras calificar API Unity `PASS`, código `0`.
- `scripts/test-editmode` — incremental `PASS`, `56/56`; se añadió después un caso separado de budgets y requiere repetición final.
- `scripts/test-playmode` — incremental `PASS`, `5/5`.
- `scripts/validate` — final `PASS`, código `0`, 3:27.34; EditMode `57/57`, PlayMode `5/5`, APK Development.
- `scripts/build-android-release` — config/Addressables `PASS`; salida global `BLOCKED` esperada, código `3`, signing ausente.
- `aapt2 dump badging/permissions` — API 36/ARM64, solo `INTERNET` + permiso interno non-exported; cero permisos sensibles/`AD_ID`.
- `adb devices` — sin dispositivo; instalación/I/O físico `NOT RUN`.
- `git diff --check && git diff --cached --check` — `PASS`; 76 paths intencionales, cero artifacts/cachés staged.

## Recovery y seguridad

No hay dependencias, red, secrets ni migración de save. Assets y tipos nuevos son aditivos hasta el cableado de Bootstrap. Si import/compile falla, conservar logs en `artifacts/`, corregir fuentes y reejecutar el comando acotado; no editar `Library`. El override de tests debe ser disposable y compilar solo en Editor/tests. Release permanece fail-closed y no se firma/publica.

## Resultados y retrospectiva

Dos perfiles locales mapean a interfaces readonly y Release lleva cero flags. Config inválida bloquea build con `CONFIG008`; Development conserva diagnóstico/mocks locales y el APK pasa. No se añadieron red, dependencia, secreto, gameplay ni cambio de save. APK: `60,310,101` bytes, SHA-256 `2c47d85cfe271bc8dde71979779dd7f36d45d09ce8746806a3505de43f9d3b80`. Release permanece bloqueado por signing humano; Android físico e iOS siguen `NOT RUN`.
