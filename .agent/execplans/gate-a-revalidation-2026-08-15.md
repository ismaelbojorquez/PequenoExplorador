# ExecPlan — revalidación independiente Gate A

- Fase/Gate: revalidación Gate A sobre foundation actual
- Estado: Complete
- Creado/actualizado: 2026-08-15 11:59 CST (`America/Mexico_City`)
- Owner: auditor independiente Unity/build/testing/seguridad infantil

## Propósito y alcance

Revalidar que la foundation actual abre, compila, respeta fronteras, ejecuta suites y produce dos smoke APK Android reproducibles con configuración vigente. Se audita también el incremento placeholder F07 ya presente porque forma parte de HEAD, sin avanzar gameplay. Correcciones permitidas: pequeñas, deterministas y directamente justificadas por hallazgos.

## Contexto y orientación

HEAD de entrada `de04fa5f5ea6171619364d76d0a882455dea1a63`, rama `main`, con remoto `origin/main`. La auditoría Gate A original vive en `docs/audits/GATE_A_2026-08-15.md` y evaluó el commit F06; esta ejecución añadirá una revalidación separada al mismo registro porque comparte fecha. El repositorio ya avanzó a F07, desviación respecto al estado esperado del prompt.

Existe un archivo ajeno no versionado bajo `docs/prompts/`. Debe preservarse, excluirse del diff/commit y reportarse como causa de árbol no completamente limpio; no colisiona con los archivos de auditoría.

## Progreso

- [x] 2026-08-15 11:32 CST — preflight, Git, documentos, configuración, scripts y código relevante inspeccionados.
- [x] 2026-08-15 11:35 CST — baseline: repository checks, compile, EditMode 29/29 y PlayMode 4/4 pasan.
- [x] 2026-08-15 11:37 CST — pipeline completo y segundo smoke Android ejecutados; ambos pasan.
- [x] 2026-08-15 11:53 CST — APK/manifest/ELF/16 KB inspeccionados; runtime de emulador quedó inconcluso por ANR externo y se preservó evidencia.
- [x] 2026-08-15 11:59 CST — fuentes oficiales, toolchain, paquetes/licencias, arquitectura, perfiles y seguridad estática revalidados.
- [x] 2026-08-15 11:59 CST — matriz, status, riesgos y changelog cerrados; pendiente únicamente review/commit operacional.

## Hallazgos

- El estado esperado 00–06 no coincide con HEAD: F07 está commiteada. La auditoría se hará sobre el estado real sin revertir historia.
- `docs/prompts/00_MASTER_CODEX_PROMPTS.md` es ajeno, no versionado y anterior a la auditoría; permanece fuera del alcance.
- F07 versionó `Assets/AddressableAssetsData/link.xml` y su `.meta`, pero Addressables los elimina/regenera y cambia el GUID durante un build. Se retiraron e ignoraron como salidas transitorias; catálogo y link final siguen incluidos dentro del APK.
- El AVD local API 37/16 KB genera ANR en componentes del sistema incluso tras cold boot; Unity 6.3 declara oficialmente no soportados los emuladores Android. El proceso del juego no registró fatal, pero tampoco alcanzó evidencia `Ready` estable en esta revalidación: `INCONCLUSIVE`.

## Decisiones

- 2026-08-15 — conservar la auditoría histórica y anexar una sección de revalidación; no reemplazar evidencia previa ni crear una falsa auditoría con otra fecha.
- 2026-08-15 — usar dos builds consecutivos del mismo HEAD; hashes pueden variar por metadata temporal, pero configuración, tamaño, permisos y contenido deben ser coherentes.
- 2026-08-15 — aceptar `PASS` porque compile, validadores, tests y dos builds Android ejecutables pasan sin Critical/Major; no contar el intento de emulador como PASS ni como fallo del APK.

## Plan de implementación

1. Completar inspección estática/local y revalidación oficial con fecha.
2. Ejecutar `scripts/validate` y repetir `scripts/build-android-development`, verificando Git entre ambos.
3. Auditar reports, APK, manifest, ABIs, ELF/page alignment, contenido local y runtime en emulador 16 KB dos veces.
4. Corregir solo defectos pequeños; si queda Major/estructural o falla build ejecutable, emitir `FAIL`.
5. Actualizar evidencia canónica, revisar únicamente cambios propios y crear el commit solicitado sin push.

## Comandos y validación

- `scripts/check-repository` — `PASS`, 0.50 s; 55 Markdown (incluye archivo ajeno), 16 JSON, 1 workflow, secrets 0.
- `scripts/compile` — `PASS`, 22.93 s.
- `scripts/test-editmode` — `PASS`, 29/29, 120.48 s.
- `scripts/test-playmode` — `PASS`, 4/4, 59.23 s.
- `scripts/validate` — `PASS`, código 0, 109.43 s; compile, Addressables, EditMode 29/29, PlayMode 4/4 y APK.
- `scripts/build-android-development` repetido — `PASS`, código 0, 62.22 s.
- APK run 1 — 41,722,038 bytes; SHA-256 `03df45c6f5bfaaa9e54a56027d04bd85b88a6a9d9d03214da66d05efb1fe61ae`.
- APK run 2 — 41,722,037 bytes; SHA-256 `0a9ae311635ec636e4d70057d6c4a6a8c861d60727ed4d0571fbdc20930bb1e1`.
- `aapt2`/`zipalign -P 16`/`llvm-readelf` — `PASS`; API 26/36, solo ARM64, siete ELF `0x4000`, sin permiso sensible/`AD_ID` y contenido Addressables local.
- `scripts/build-addressables-local` tras GAR-001 — `PASS`, código 0, 44.11 s; outputs regenerados quedan ignorados y sin diff unstaged.
- Runtime AVD API 37 `PAGE_SIZE=16384` — `INCONCLUSIVE`; ANR de System UI/teléfono/servicios Google, sin fatal del juego pero sin evidencia `Ready` estable.
- Release — guard generó reporte `BLOCKED` por signing externo; AAB no ejecutado.

## Recovery y seguridad

No tocar el archivo ajeno, signing, remoto, stores, cuentas ni secretos. Artefactos permanecen bajo `artifacts/` ignorado. Si Unity/emulador queda ocupado, conservar logs, detener solo procesos iniciados por esta auditoría y reanudar desde el comando individual. No declarar PASS con un build fallido o no ejecutado.

## Resultados y retrospectiva

Veredicto `PASS`: foundation, arquitectura, perfiles, tests, contenido local y dos builds Android cumplen. La única corrección fue higiene de salida Addressables generada. No quedan Critical/Major; dispositivo físico, AAB Release/firma, CI remota e iOS permanecen pendientes honestos. El archivo ajeno bajo `docs/prompts/` impide declarar árbol global completamente limpio y queda preservado fuera del commit.
