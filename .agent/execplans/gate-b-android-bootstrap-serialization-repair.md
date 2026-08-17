# ExecPlan — reparar crash Android al cargar Bootstrap

- Fase/Gate: corrección posterior a Prompt 30 / Gate B
- Estado: Complete
- Creado/actualizado: 2026-08-17 11:08 `America/Mexico_City`
- Owner: Principal Unity/Build Engineer

## Propósito y alcance

Eliminar el crash nativo reproducido en Android al deserializar `level0`, impedir que escenas runtime vuelvan a contener `MonoScript` incrustados y demostrar el resultado mediante compile, validadores, EditMode, PlayMode, build e instalación/arranque repetido en el dispositivo conectado. No se amplía gameplay, contenido, permisos ni alcance de Gate B; el playtest infantil continúa fuera de esta corrección.

## Contexto y orientación

El repositorio inició limpio en `main`, HEAD `24e0d2075d78d59e86562195a9e69e0c04640364`. El APK Gate B local y el extraído del HONOR DNY-NX9 son byte a byte idénticos, SHA-256 `a633aace2f34449c69e848ed0adcf73585d88c742953cb30c3d0a3a809d88148`, y ambos pasan `unzip -t`. En el dispositivo, Unity `6000.3.22f1` aborta con `SIGTRAP`, `CachedReader::OutOfBoundsError`, `Position out of bounds` y declara corrupto `assets/bin/Data/level0` mientras carga un `MonoBehaviour`.

`Assets/_Game/Bootstrap/Bootstrap.unity` contiene seis documentos YAML `!u!115 MonoScript` locales y 191 componentes que los referencian. Cinco clases tienen scripts externos válidos; `CustomizationOptionButtonView` comparte `CustomizationButtons.cs` y carece de un asset de script propio. La reparación debe conservar los campos serializados de cada componente, no reconstruir la UI ni ocultar el defecto.

## Progreso

- [x] 2026-08-17 10:46 — Crash reproducido por `adb` sin borrar datos de la app.
- [x] 2026-08-17 10:50 — APK instalado extraído y comparado; hash e integridad ZIP coinciden con el local.
- [x] 2026-08-17 10:56 — Inventariados seis `MonoScript` locales y 191 referencias en Bootstrap.
- [x] 2026-08-17 10:58 — Scripts separados, 191 referencias reparadas y validator fail-closed integrado.
- [x] 2026-08-17 11:00 — Pipeline completo y APK nuevo producidos.
- [x] 2026-08-17 11:01 — APK instalado sin borrar datos; primer arranque físico permanece vivo, llega a `ApplicationReady` y `Camp`, sin crash.
- [x] 2026-08-17 11:08 — Auditoría/estado/riesgos/testing actualizados y plan cerrado. Segundo rearranque `NOT RUN`: el teléfono se desconectó de `adb`.

## Hallazgos

- El build pipeline informó éxito porque validaba producción del APK, no la deserialización de la escena en un Player real.
- El archivo APK no se dañó en transporte o instalación; la corrupción es interna al `level0` generado desde la escena.
- Los `MonoScript` incrustados se introdujeron en la fase de Design System y no eran detectados por los tests de escena existentes.

## Decisiones

- 2026-08-17 — Reparar referencias de script de forma determinista preservando la serialización de componentes; no recrear manualmente 191 componentes.
- 2026-08-17 — Separar cada `MonoBehaviour` persistido en su archivo homónimo y conservar el GUID del componente de slot existente.
- 2026-08-17 — Bloquear compile/build si cualquier escena habilitada contiene documentos `!u!115` locales.
- 2026-08-17 — Gate B permanece `FAIL` durante la reparación; solo puede volver a `CONDITIONAL` tras Android físico exitoso porque el playtest humano sigue pendiente.

## Plan de implementación

1. Separar `CustomizationSlotButtonView` y `CustomizationOptionButtonView` en archivos homónimos, preservando el GUID ya referenciado por la escena.
2. Añadir tooling Editor general para resolver un `MonoScript` local por clase/namespace/assembly, sustituir sus referencias por GUID externos y retirar los documentos locales.
3. Ejecutar la reparación sobre Bootstrap y verificar que Unity puede abrir/reserializar la escena.
4. Integrar un validador fail-closed al pipeline y un test EditMode que pruebe el caso inválido controlado.
5. Ejecutar `scripts/validate`, construir APK limpio, instalarlo y probar dos arranques más force-stop/restart en el teléfono.
6. Registrar evidencia en adenda Gate B, `STATUS`, riesgos, testing y changelog; revisar diff y commit.

## Comandos y validación

- `git status --short --branch` — entrada limpia en `main`; PASS.
- `adb logcat` + `monkey -p com.placeholder.pequenoexplorador` — crash `SIGTRAP` reproducido; FAIL de Android runtime.
- `shasum -a 256` local/extraído + `unzip -t` — APKs idénticos e integridad ZIP PASS.
- `rg '^--- !u!115 '` y conteo de `m_Script` — seis documentos/191 referencias inválidas observadas.
- `scripts/validate` — PASS: repository, compile/validadores, Addressables, EditMode `169/169`, PlayMode `29/29` y APK Development.
- APK final de pipeline — `67,444,690` bytes, SHA-256 `5c382e6c3340f569350ef9ee765566fd0f0377d9403b847df5ae411c33253b80`, API 26/36, IL2CPP/ARM64.
- instalación/primer arranque Android — PASS en HONOR DNY-NX9/Android 16 sobre el artefacto runtime-equivalente SHA-256 `a7173c3647b36559d67e41386ee84f744012346dd6c4bcb8ac6a88b4bfe89384`: PID `19641` vivo tras 15 s, `ApplicationReady` y `TransitionCompleted Camp`, cero firmas fatales/corrupción. El rebuild final solo añadió el ajuste Editor portable y regeneró metadata del APK.
- segundo force-stop/rearranque — `NOT RUN`: el dispositivo dejó de aparecer en `adb devices -l`; no se presenta como PASS.

## Recovery y seguridad

Git conserva la escena original; no se borran datos del dispositivo durante reparación. La instalación nueva usará `adb install -r`; `pm clear` queda prohibido salvo necesidad explícita porque elimina progreso. Si la reparación no resuelve cada clase de forma unívoca, debe abortar antes de escribir. Si Unity modifica assets ajenos al alcance, detenerse, inventariar y no incorporarlos.

## Resultados y retrospectiva

El crash de arranque queda corregido y reproducido positivamente en Player físico: la escena ya no contiene documentos `MonoScript` locales, Unity carga Bootstrap, inicializa servicios y entra a Camp. El pipeline ahora falla antes del build ante `!u!115` o `m_Script` local, y una fixture EditMode protege ambas reglas. Gate B vuelve a `CONDITIONAL`, no `PASS`: falta recorrer la matriz touch completa y ejecutar el playtest infantil/no lector. El segundo rearranque planeado quedó `NOT RUN` por desconexión del dispositivo y permanece como acción de la matriz, no como defecto conocido del arreglo.
