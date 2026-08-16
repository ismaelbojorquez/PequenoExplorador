# ExecPlan — framework de audio localizado y child-friendly

- Fase/Gate: Prompt 12 / Gate B
- Estado: Completed
- Creado/actualizado: 2026-08-16 11:10 America/Mexico_City
- Owner: Game Audio Systems Engineer / Child UX

## Propósito y alcance

Implementar reproducción semántica y desacoplada para cinco buses, siete categorías de cue, cola de voz, prioridades/cooldown, ducking, subtítulos/replay, settings persistidos y lifecycle móvil. Los únicos clips serán placeholders suaves generados algorítmicamente, inequívocos en tooling y bloqueados para Release. Excluye voz humana, catálogo animal, contenido factual, middleware/SDK, micrófono, red y producción musical final.

## Contexto y orientación

HEAD inicial `caeb68fcfbea1e464094ad8f547ad7c56506d83b`, rama `main`, árbol limpio. Unity `6000.3.22f1`; nueve assemblies; Save schema v2; Localization `1.5.12`; Bootstrap persistente y escenas Camp/Jungle aditivas. No había clips, mixer, AudioSource ni código de audio. Fuentes canónicas: `AGENTS.md`, `docs/STATUS.md`, `docs/16_AUDIO.md`, `docs/AUDIO_REQUIREMENTS.md`, `docs/17_LOCALIZATION.md`, `docs/14_UI_UX.md` y este plan.

## Progreso

- [x] 2026-08-16 — preflight Git/documental/implementación completo; no cambios ajenos ni audio de plantilla.
- [x] 2026-08-16 — `scripts/validate` baseline PASS en 117.39 s; EditMode `62/62`, PlayMode `6/6` y APK Development.
- [x] Contratos semánticos, mixer/cues y diez placeholders internos creados.
- [x] Servicio, queue/ducking/settings/subtítulos/replay/lifecycle y panel Development implementados.
- [x] Validators, EditMode/PlayMode y Android smoke ejecutados; clipping, objetos, addresses y blockers auditados.
- [x] Documentación canónica actualizada y pipeline completo PASS; falta solo commit/estado limpio de cierre.

## Hallazgos

- `AudioManager` usa estéreo 48 kHz, buffer DSP 1024, 32 voces reales y output suspension; no requiere personalización global para esta fase.
- El único `AudioListener` vive en Bootstrap, que persiste. Añadir drivers/sources a ese mismo root evita `DontDestroyOnLoad` disperso y duplicados entre Camp/Jungle.
- Save v2 conserva booleans Music/SFX/Narration, pero no Master/Ambience/Voice ni subtítulos; satisfacer el prompt exige schema v3 y migración v2→v3, no PlayerPrefs.
- Localization ya tiene asset table `Voice`, pero no archivos. El framework debe compartir keys conceptuales sin convertir asset tables vacías en aprobación de voz.

## Decisiones

- Mantener nueve assemblies. Application define intención/modelos; Content ScriptableObjects contienen authoring; Bootstrap mapea; Infrastructure posee Unity `AudioSource`/`AudioMixer`; Presentation solo solicita y muestra subtítulo.
- Un pool fijo de cuatro sources Effects limita acumulación; música, ambiente y voz usan un source exclusivo cada uno. La cota total es siete, muy por debajo de 32 voces reales.
- Ducking se aplica durante voz prioritaria a música/ambiente y se restaura en toda terminación/interrupción/shutdown; no sustituye mezcla humana final.
- Placeholders WAV mono 48 kHz/16-bit, cortos y a bajo nivel, se generan mediante tooling propio. Prefijo `PH_`, metadata `ReleaseBlocked`; ningún mensaje técnico se muestra al niño.
- Save schema v3 persiste volúmenes normalizados Master/Music/Ambience/Effects/Voice y subtítulos; migra booleans v2 a 1/0 y Ambience desde Music.

## Plan de implementación

1. Crear tipos BCL-only `IAudioService`, IDs, buses, categorías, prioridad, settings, resultado y subtitle model/event.
2. Extender Domain/Save a schema v3 y migración v2→v3 con defaults/conservación.
3. Crear `AudioCueDefinition`/catalog Content, mixer con cinco buses y placeholders/cues baseline mediante setup Editor idempotente.
4. Implementar scheduler puro, servicio Unity, pool, voice queue, cooldown, ducking, replay, focus/pause y cleanup.
5. Componer después de Localization, enlazar Presentation/panel Development y keys ES/EN de subtítulo.
6. Añadir validator CLI/Release report, EditMode/PlayMode y Android smoke.
7. Actualizar fuentes canónicas, revisar diff completo, commit único y post-commit smoke.

## Comandos y validación

- Preflight Git — PASS; `main`, limpio, HEAD registrado.
- `scripts/validate` baseline — PASS 117.39 s; compile, Addressables, EditMode `62/62`, PlayMode `6/6`, APK.
- Plan final: `scripts/validate-audio`, `scripts/compile`, `scripts/test-editmode`, `scripts/test-playmode`, `scripts/build-android-development`, `scripts/build-android-release`, `scripts/validate`, `git diff --check`, `git lfs fsck`.

## Recovery y seguridad

No editar `Library`, descargar clips ni reutilizar tonos del sistema. El setup solo crea rutas `Assets/_Game/Audio` y assets con IDs conocidos; si falla, conservar log y corregir el generador, no borrar cambios ajenos. Tests usan cues/AudioClips controlados. Release debe registrar placeholders pendientes y permanecer bloqueado por signing/contenido; no eliminar blockers para obtener un falso PASS.

## Resultados y retrospectiva

Cinco buses, siete cues y siete sources quedaron acotados; diez WAV `PH_` mono/48 kHz son locales, addressable-ready y `ReleaseBlocked`. Save avanzó a schema v3 mediante migración v2→v3. `scripts/validate-audio`, compile y full pipeline pasan; EditMode `70/70`, PlayMode `7/7`. APK Development: `66,037,223` bytes, SHA-256 `9564026c1dae24c69d3f96ff4ac46650267a2fad9f2677c63a9ddacc614ec046`, API 26/36, IL2CPP/ARM64 y zipalign 16 KB. Release terminó con código esperado `3` y además registró diez assets finales pendientes.

Fallos conservados: compile detectó módulo Audio no explícito, una colisión `Time`, constructor AppContext y tablas aún no regeneradas; el primer validator encontró enums authoring desplazados y el content validator copy diagnóstico serializado. Cada causa se corrigió y su comando aislado/full pipeline posterior pasó. No hubo audio externo, SDK, red, permiso sensible, gameplay, signing, push o publicación. Hardware auditivo/Android, contenido final/licencias y iOS siguen `NOT RUN`/humanos.
