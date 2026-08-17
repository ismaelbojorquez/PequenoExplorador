# ExecPlan — álbum visual infantil desde catálogo, progreso y fotos

- Fase/Gate: Prompt 20 / Gate B `FAIL`
- Estado: Complete
- Creado/actualizado: 2026-08-17 00:19 `America/Mexico_City`
- Owner: Collection UI Engineer / diseñador de enciclopedia infantil

## Propósito y alcance

Construir un álbum local por mundo/categoría que lea exclusivamente `IContentCatalog`, progreso de discovery y metadata/binarios del photo store. Debe mostrar estados locked/discovered, detalle infantil, replay, fallback de foto y navegación Camp↔Álbum sin mutar progreso ni ampliar el catálogo. Incluye queries/read models, UI uGUI baseline, carga cancelable, tests y documentación; excluye diseño final, contenido masivo, economía y assets externos.

## Contexto y orientación

HEAD verificado `3a662898e8f043c74f43ab560819ef4ea3356fd5`, `main`, árbol limpio. Prompt 19 dejó save v6, `IPhotoStore`, una entrada Approved `discovery.jungle.keel-billed-toucan` y thumbnail `384×216`. Gate B continúa `FAIL`. Los documentos canónicos son `AGENTS.md`, `docs/STATUS.md`, `docs/07_DISCOVERY_SYSTEM.md`, `docs/14_UI_UX.md`, `docs/17_LOCALIZATION.md`, `docs/CONTENT_MODEL.md` y `docs/PHOTOGRAPHY_SYSTEM.md`.

## Progreso

- [x] 2026-08-17 00:19 — preflight Git/documental, inventario de código y skill UI completados.
- [x] 2026-08-17 00:19 — baseline: compile `PASS`, EditMode `112/112`, PlayMode `19/19`.
- [x] 2026-08-17 00:51 — read models/query y lectura manifest-validada/cancelable implementados sin exponer save/filesystem a Presentation.
- [x] 2026-08-17 00:51 — pantallas/estados/navegación ES/EN/pseudo cableados con safe area, pool/caché 8 y fallbacks seguros.
- [x] 2026-08-17 00:51 — validador build/CLI y cobertura EditMode/PlayMode añadidos; primer PlayMode detectó pseudo assertion y teardown Unity, corregidos y repetidos.
- [x] 2026-08-17 00:51 — pipeline completo/APK ejecutado; documentación/diff cerrados para commit.

## Hallazgos

- El catálogo solo contiene un discovery Approved; locked/discovered/missing-photo serán estados del mismo contenido y fixtures in-memory de tests, no nuevas entradas runtime.
- No existe claim Approved de tamaño para VS-D-A01. El detalle mostrará un fallback localizado “dato por confirmar”, sin inventar información factual.
- `IPhotoStore` solo escribe/borra; el álbum necesita ampliar el puerto con lectura binaria async validada y cancelable.
- La UI existente usa uGUI/`Text` y safe-area central. No se añadirá dependencia o fuente externa para forzar TMP durante esta baseline.

## Decisiones

- 2026-08-17 — usar un pool fijo pequeño de celdas reutilizables: evita instanciación por refresh y es proporcional al único contenido actual.
- 2026-08-17 — modelar campos de ficha mediante metadata authoring opcional en `DiscoveryDefinition`, no por sufijos/hardcodes del tucán.
- 2026-08-17 — una entrada locked no entrega a Presentation nombre, facts, audio ni referencia de foto; evita revelación accidental.
- 2026-08-17 — refrescar el read model al abrir/volver al grid y tras cambios de locale; el progreso recién capturado aparece sin reinicio.

## Plan de implementación

1. Añadir metadata de álbum readonly a definitions y compiler, con referencias opcionales habitat/dieta/tamaño/curiosidad validadas.
2. Crear `AlbumQueryService`, snapshots y view models filtrados por world/category Approved.
3. Extender `IPhotoStore` con lectura segura/cancelable y resultados explícitos; adaptar stores local/memory/failure.
4. Crear `AlbumView`/celdas/detail y setup Editor idempotente; cablear Bootstrap/SceneTransitionView para Camp↔Álbum.
5. Añadir localización ES/EN/pseudo, validador y tests de queries, filtros, removed/missing y lifecycle UI.
6. Actualizar documentación, ejecutar suite/build, revisar diff y crear el commit solicitado.

## Comandos y validación

- `scripts/compile` — baseline `PASS`.
- `scripts/test-editmode` — baseline `PASS 112/112`.
- `scripts/test-playmode` — baseline `PASS 19/19`.
- `scripts/validate-album` — final `PASS`.
- `scripts/validate` — final `PASS`, código `0`, `275.58 s`; EditMode `117/117`, PlayMode `21/21`, APK Development.

## Recovery y seguridad

No borrar assets ni progreso. El setup Editor debe conservar GUIDs y modificar solo el root `PH_UI_ALBUM`/campos propios. Si Unity falla, conservar logs en `artifacts/`, ejecutar primero el comando aislado y no limpiar cambios ajenos. No push, paquetes, red, permisos ni contenido factual nuevo.

## Resultados y retrospectiva

Se entregó un álbum local read-only que muestra el único discovery Approved bloqueado/descubierto, actualiza tras captura sin reinicio y degrada de forma segura ante fact/foto/audio ausentes. La prueba inicial reveló dos supuestos inválidos: pseudo no conserva español literal y `?.` no respeta el null sobrecargado de objetos Unity destruidos; ambos quedaron corregidos y PlayMode final pasó `21/21`. No se añadió contenido, paquete, red o permiso. La baseline visual sigue `PH_`; TMP/arte/audio final y hardware real se conservan como deuda explícita, y Gate B sigue `FAIL`.
