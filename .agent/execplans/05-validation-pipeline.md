# ExecPlan — pipeline reproducible de validación y smoke

- Fase/Gate: 05 / B
- Estado: Complete
- Creado/actualizado: 2026-08-14 23:01 America/Mexico_City
- Owner: Unity Test/Build Engineer + DevSecOps Engineer

## Propósito y alcance

Crear un comando local reproducible que compile, valide configuración/contenido/fronteras, ejecute EditMode y PlayMode con reportes JUnit y produzca un APK Development con manifest/hash. Añadir wrappers Bash macOS/Linux, diagnósticos sanitizados bajo `artifacts/`, workflow GitHub seguro y documentación honesta. Signing, publicación, secretos, remoto y servicios obligatorios quedan excluidos.

Criterios: cualquier fallo devuelve código no cero y conserva evidencia; Release falla de forma deliberada hasta signing externo aprobado; Actions usan SHA completo, permisos mínimos y nunca `pull_request_target`; CI Unity permanece manual/self-hosted y no se presenta como ejecutada remotamente.

## Contexto y orientación

Base limpia: commit `7f41246a148cdb1354a885d70feb80f3fa71576a`, Unity `6000.3.22f1`, cuatro paquetes directos exactos y nueve assemblies validados. No existe remoto GitHub, `.github`, `scripts` ni `BuildTools`. El Editor local y módulo Android están disponibles; `shellcheck` y `actionlint` no están instalados.

Fuentes: `AGENTS.md`, `docs/STATUS.md`, `docs/VALIDATION_PLAYBOOK.md`, `docs/02_TECHNICAL_ARCHITECTURE.md`, `docs/ANDROID_RELEASE.md`, `ProjectSettings/ProjectVersion.txt`, `Packages/` y scripts Editor.

## Progreso

- [x] 2026-08-14 22:33 — preflight Git/OS/Unity/remotos/documentos/código completado; árbol limpio.
- [x] 2026-08-14 22:33 — baseline: compile, fronteras, EditMode 5/5 y PlayMode 1/1 pasan.
- [x] 2026-08-14 22:33 — Actions oficiales seleccionadas tras revisar SHA/licencia.
- [x] 2026-08-14 22:42 — BuildTools, wrappers, checks, workflow y documentación implementados.
- [x] 2026-08-14 22:58 — pipeline completo, fallo Release y Android smoke ejecutados.
- [x] 2026-08-14 23:01 — artefactos/configuración inspeccionados; revisión y commit son el cierre de fase.

## Hallazgos

- No hay remoto configurado; la creación/conexión de GitHub será guía humana, sin push.
- `shellcheck` y `actionlint` no están disponibles localmente; se usarán `bash -n`, parser YAML estándar y checks propios, reportando esas herramientas como `NOT RUN`.
- El runner Unity remoto no puede considerarse listo: no existen repo, runner ni activación/licencia CI.
- El primer compile detectó `PackageInfo` ambiguo y falló correctamente; se calificó la API y el siguiente compile pasó.
- El primer wrapper EditMode usó `-quit`, Unity salió sin XML y el conversor hizo fallar el wrapper; se eliminó `-quit` común y se comprobó NUnit/JUnit.
- NUnit incluye paths de máquina; los wrappers ahora sanitizan el XML antes de convertirlo y la búsqueda final no encuentra la ruta personal.

## Decisiones

- 2026-08-14 — la lógica Unity vive en `Assets/_Game/Editor/BuildTools`; shell solo localiza Editor, orquesta, sanitiza logs y transforma resultados.
- 2026-08-14 — `scripts/validate` incluye Android Development por defecto; una validación sin build requiere un comando individual, no un PASS parcial disfrazado.
- 2026-08-14 — Release es un placeholder que siempre falla antes de construir hasta integrar signing externo mediante decisión separada.
- 2026-08-14 — usar solo `actions/checkout` v4.3.1 SHA `34e114876b0b11c390a56381ad16ebd13914f8d5` y `actions/upload-artifact` v4.6.2 SHA `ea165f8d65b6e75b540449e92b4886f43607fa02`; ambos repos oficiales, licencia MIT verificada 2026-08-14.

## Plan de implementación

1. Crear BuildTools CLI para compile, contenido, Development y Release bloqueado; generar environment/build manifests.
2. Refactorizar el smoke legado para delegar al servicio de build sin duplicar perfiles.
3. Crear wrappers Bash, localizador portable, logs sanitizados, NUnit→JUnit y checks de Markdown/JSON/YAML/secrets.
4. Añadir workflow de checks estáticos y job Unity manual/self-hosted con Actions pinneadas y artefactos `always()`.
5. Documentar GitHub setup, ejecución/recovery y fuentes; actualizar status/roadmap/changelog/índice.
6. Ejecutar todos los wrappers, fallo Release controlado, smoke y validaciones; revisar/commit.

## Comandos y validación

- `git status --short --branch` — PASS, `## main` limpio.
- `git branch --show-current` — PASS, `main`.
- `git log -1 --format=fuller` — PASS, commit F04 esperado.
- `git remote -v` — sin salida; no existe remoto.
- Unity compile basal — PASS, código 0, 16.64 s.
- Boundary CLI basal — PASS, 12.58 s.
- EditMode basal — PASS 5/5, 20.31 s.
- PlayMode basal — PASS 1/1, 26.18 s.
- `git ls-remote` sobre repos oficiales — SHAs de tags confirmados.
- `scripts/check-repository` — PASS, 48 Markdown, 13 JSON/asmdefs, 1 workflow, 0 secretos detectados.
- `scripts/validate-content` — PASS, 11.27 s.
- `scripts/test-editmode` — PASS, 5/5; NUnit y JUnit válidos.
- `scripts/test-playmode` — PASS, 1/1; NUnit y JUnit válidos.
- `scripts/validate` final — PASS, código 0, 2:05.16; termina `PE_FULL_VALIDATION_OK` e incluye APK.
- APK Development — PASS, 57,046,302 bytes, SHA-256 `3d0a7385023e3c7d4f9772303027de2e448935bacfea73966ef71824f014b479`, build interno 25.090 s.
- Inspección APK — PASS: min/target/compile 26/36/36, solo ARM64, `INTERNET` Development + permiso interno, sin permisos sensibles/AD_ID; `zipalign -P 16` PASS.
- `scripts/build-android-release` — fallo controlado PASS del guard rail: wrapper observó código 3 en 34.754 s y reporte `BLOCKED`, sin signing.
- CI GitHub — NOT RUN: no existe remoto, runner ni activación.
- `shellcheck` / `actionlint` — NOT RUN: herramientas no instaladas; Bash/Psych/política propia sí ejecutados.

## Recovery y seguridad

`artifacts/` es el único destino del pipeline y queda ignorado. No limpiar otras rutas, no leer/imprimir valores secretos, no configurar firma, no crear remoto ni push. El workflow Unity solo corre manualmente en runner self-hosted etiquetado y activado fuera del repo. Si falla un subcomando, conservar log/reporte sanitizado y devolver su código; no continuar hacia una falsa aprobación.

## Resultados y retrospectiva

Objetivo logrado localmente: un comando reproduce todas las capas y conserva diagnósticos sin contaminar Git. El diseño fail-closed evitó fingir Release/CI. Los dos fallos tempranos demostraron códigos no cero y permitieron corregir compatibilidad/salida de tests. Resta habilitación humana de GitHub y signing, fuera del alcance.
