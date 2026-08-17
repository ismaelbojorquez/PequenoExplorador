# ExecPlan — FTUE audiovisual contextual de la primera expedición

- Fase/Gate: Prompt 28 / Gate B permanece FAIL
- Estado: Complete
- Creado/actualizado: 2026-08-17 06:12 CST
- Owner: FTUE Design / Gameplay Tutorial Engineering

## Propósito y alcance

Implementar un tutorial data-driven y opcional dentro del flujo real Camp→Selva que enseñe movimiento, interacción, fotografía, discovery/estrellas/álbum y retorno a Camp. Debe presentar una sola instrucción breve, permitir skip/replay/recovery, persistir estado versionado y respetar ES/EN, subtítulos, safe area, Back y reduce-motion. Excluye padres, monetización, analytics remoto, contenido masivo y cambios de reglas de las features.

## Contexto y orientación

HEAD inicial `62925772d4dce39ef05d44119cb4b50b64604699`, `main`, árbol limpio. `scripts/validate` previo PASS: compile, Addressables, EditMode `158/158`, PlayMode `26/26` y APK Development. El loop real ya ofrece Camp, Selva, tap-to-move, interacción, fotografía, discovery, estrellas, misión, álbum y retorno, pero no hay `TutorialDefinition`, coordinador, elección de guía ni estado persistente. Save está en schema v11; Prompt 28 exige v12/migración pura. DesignSystem y audio/localización locales son las autoridades de presentación.

## Progreso

- [x] 2026-08-17 06:12 CST — preflight, inventario, documentos y baseline integral contrastados.
- [x] Definir estado/steps/triggers y persistencia v12 sin polling ni analytics.
- [x] Crear authoring, validator, UI audiovisual y composición del flujo real.
- [x] Añadir EditMode/PlayMode, recovery, ratios y evidencia de build.
- [x] Actualizar documentación, revisar diff y cerrar commit.

## Hallazgos

- `docs/README.md` conserva dos datos obsoletos de Prompt 18/nueve assemblies; se corregirán sin tratarlos como evidencia actual.
- No existe implementación tutorial ni selección de modo de guía; `PlayerPreferences` solo contiene guía enum previa, locale, audio y subtítulos.
- Android físico no está conectado; touch real debe quedar `NOT RUN`, no PASS.

## Decisiones

- 2026-08-17 — el “aha moment” es completar la primera foto/discovery real; no habrá tour previo desconectado.
- 2026-08-17 — steps observan señales semánticas explícitas del composition root y exponen gating permitido, sin buscar GameObjects por nombre ni introducir ramas internas en features.
- 2026-08-17 — no se guardará cada intento/tiempo/tap; solo versión, step actual y estado Completed/Skipped para reanudación y QA local mínimo.

## Plan de implementación

1. Auditar APIs de progreso, save, input, scene flow y presenters; fijar contratos y secuencia de 7 pasos como datos Content.
2. Implementar modelo/coordinador Application, repositorio sobre `PlayerProgress`, schema v12 y migración v11→v12.
3. Crear authoring/validator, claves ES/EN, cues de voz `PH_`, overlay spotlight/mano-flecha, skip/replay/help y debug reset Development.
4. Componer en Bootstrap usando outcomes semánticos y gating seguro; mantener Back/pause y recovery.
5. Probar transiciones, duplicates, resume/version/skip, flujo completo/no lector/locale/ratios y ejecutar APK.

## Comandos y validación

- `scripts/validate` — baseline PASS: `158/158`, `26/26`, APK `67,424,654` bytes, SHA-256 `c201a1aab9050ce9855a68f750f9a0c558a32ece624b3d71e31f5ab72c61c49f`.
- `scripts/validate-tutorial` — PASS; definition v1, siete steps, cues, safe area y targets.
- `scripts/validate` final — PASS; EditMode 165/165, PlayMode 28/28, Addressables 61 locations/1,920,120 bytes y APK Development 67,440,962 bytes, SHA-256 `b4c34cda8c21f4637dd42002b0ed0fd728cd41ab12e3de82f21d5fa5b6f12c18`.
- `adb devices -l` — ejecutado sin hardware listado; touch Android real `NOT RUN`.

## Recovery y seguridad

Setup/validator serán idempotentes y los assets nuevos usarán `PH_` con Release bloqueado. La migración preservará v11 y una versión tutorial futura reiniciará únicamente el tutorial, no el progreso de juego. No se añade red, analytics, permisos, SDK, IAP, ads, publicación o push. Ante fallo, conservar logs/artefactos ignorados y corregir la primera evidencia; no editar `Library`, limpiar cambios ajenos ni reescribir historia.

## Resultados y retrospectiva

El sistema quedó compuesto sin añadir assembly, dependencia, red ni analytics. El selector modal evita interacción accidental antes de elegir guía; después el overlay solo restringe las acciones necesarias y mantiene Back/pausa. Una prueba de reload se endureció para exigir una instancia Bootstrap nueva en lugar de observar transitoriamente la anterior. Validación integral PASS: EditMode 165/165, PlayMode 28/28 y APK Development reproducible. Narración humana, touch Android y playtest 4–9 quedan honestamente pendientes. Gate B sigue FAIL hasta Prompt 29 y nueva auditoría.
