# ExecPlan — fixture visual reproducible del tucán para revisión

- Fase/Gate: bridge posterior a Prompt 18; Gate B FAIL
- Estado: Complete
- Creado/actualizado: 2026-08-16 22:20 America/Mexico_City
- Owner: Senior Unity Technical Artist / Content Tools Engineer

## Propósito y alcance

Crear un prefab propio `VS_ToucanPicoCanoa` mediante primitives/materiales URP del proyecto, integrarlo solo como visual Development del interactable animal neutral, producir provenance/métricas/renders ignorados y validar que Release continúa bloqueado. No implementa fotografía, facts runtime, audio final, migración de IDs ni aprobación humana del asset.

## Contexto y orientación

HEAD inicial `b78e42c4718651e023cbde1e7185835bc5538cfb`, `main` limpio. `InteractionFoundationSetup` reconstruye Jungle con tres roots `PH_`; el animal usa `interaction.fixture.animal → discovery.jungle.placeholder`. El enum real es `Draft=0`, `Sourced=1`, `Reviewed=2`, `Approved=3`, `Rejected=4`. El dossier `VS-D-A01` tiene Product/Localization aprobado por Ismael Bojórquez, pero especialista factual y asset-specific signoff pendientes. El nuevo visual usará `visual.discovery.jungle.keel-billed-toucan`, será `Sourced`, `isPlaceholder=false` y no cambiará definitions de discovery/interacción a Approved.

## Progreso

- [x] 2026-08-16 21:56 — preflight, inventario, enum/wiring y baseline verificados.
- [x] 2026-08-16 22:03 — metadata, generator idempotente, prefab/materiales y scene integration implementados.
- [x] 2026-08-16 22:06 — validator, tests EditMode/PlayMode y render review CLI añadidos.
- [x] 2026-08-16 22:08 — seis renders inspeccionados; se corrigió warmup URP de la primera vista.
- [x] 2026-08-16 22:13 — pipeline completo, manifest, métricas y documentación verificados.

## Hallazgos

- El validator actual exige que los tres roots de interacción conserven prefijo `PH_`; el visual no-placeholder puede vivir como prefab hijo sin cambiar el contrato neutral ni los IDs persistidos.
- Reconstruir `InteractionFoundationSetup` generaba ruido en Bootstrap/planta/objeto. La integración final modifica únicamente el hijo visual del animal existente y conserva la cápsula como estado previo recuperable al rerun del setup base.
- PlayMode no referencia el assembly Content; comprobará el visual por hierarchy/renderers sin ampliar el grafo de asmdefs.

## Decisiones

- 2026-08-16 — mantener `interaction.fixture.animal` y `discovery.jungle.placeholder`: evita migración de Save/aliases antes del gate factual.
- 2026-08-16 — prefab propio no-placeholder en estado Sourced, pero root técnico y definitions continúan Draft/PH_: diferencia arte concreto revisable de aprobación editorial/runtime.
- 2026-08-16 — usar primitives Unity built-in y shared materials URP, sin meshes/media descargados; provenance y hashes se generan después de guardar los assets.
- 2026-08-16 — renders con Camera/RenderTexture Editor bajo `artifacts/review/toucan`, nunca ScreenCapture runtime.

## Plan de implementación

1. Añadir metadata runtime Content para ID, autoría, source, licencia, estado y bounds candidatos.
2. Añadir `ToucanFixtureSetup` Editor que crea materiales/prefab deterministas, actualiza únicamente el interactable animal de Jungle y escribe ledger/reporte/renders.
3. Mantener `InteractionFoundationSetup` sin hardcode de especie; el generator visual hace el mapping reemplazable en datos de escena.
4. Añadir validator Development/Release integrado al pipeline y tests de asset, idempotencia, scene wiring, métricas y bloqueo Release.
5. Ampliar PlayMode para exigir el visual candidato y conservar interacción/ciclos existentes.
6. Actualizar documentos canónicos, ejecutar validación completa y cerrar el plan.

## Comandos y validación

- `scripts/validate` — baseline inicial PASS: repository, compile, Addressables, EditMode 103/103, PlayMode 18/18 y APK.
- `scripts/apply-toucan-fixture` — PASS; assets, métricas y seis renders generados.
- `scripts/compile` — PASS.
- `scripts/test-editmode` — PASS, 106/106.
- `scripts/test-playmode` — PASS, 18/18; incluye tres ciclos Camp↔Jungle.
- `scripts/validate` final — PASS en 1:27.92; APK Development generado.
- Manifest final — PASS: sin `CAMERA`, micrófono, ubicación, contactos o `AD_ID`; zipalign 16 KB PASS.
- `scripts/build-android-release` — expected FAIL, código 2 con `TOUCAN019`; no produjo Release.
- Android físico — NOT RUN: `adb devices -l` no listó dispositivos.

## Recovery y seguridad

El generador actualiza rutas explícitas bajo `Assets/_Game/Content/Discoveries/Jungle/KeelBilledToucan` y regenera solo el root de interacción que el setup ya posee. No borrar assets existentes ni usar Git destructivo. Si Unity falla, conservar logs en `artifacts/`, corregir el primer error y reejecutar el método idempotente. Los renders son ignorados; ningún script escribe fuera de `Assets/`, `docs/` o `artifacts/` salvo temporales de Unity.

## Resultados y retrospectiva

El fixture es reconocible, propio y reproducible: 16 meshes/renderers, 4,931 vértices, 7,132 triángulos, siete materiales y bounds `2.973 × 2.425 × 1.100`. El primer render requería un frame de warmup URP; el tooling ahora renderiza dos veces antes de leer pixels. El aislamiento final evitó serializar Bootstrap/planta/objeto. El visual queda `Sourced`, no-placeholder, pero Prompt 19 y Release continúan bloqueados por especialista factual y firma visual asset-specific.
