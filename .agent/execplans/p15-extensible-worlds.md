# ExecPlan — framework extensible de mundos locales

- Fase/Gate: Prompt 15 / Gate B
- Estado: Complete
- Creado/actualizado: 2026-08-16 14:10 America/Mexico_City
- Owner: World Systems Architect / Addressables Engineer

## Propósito y alcance

Hacer que Camp descubra Jungle mediante un catálogo de manifests y que entrada/salida carguen datos/escena/contenido local sin switches por bioma. Incluye schema, disponibilidad no comercial, sesión/use case, spawn/labels/budgets, validator y Jungle stub. Excluye gameplay, otro mundo real, entitlements, downloads, remote catalogs y cambio de save.

## Contexto y orientación

HEAD inicial `53e3fd4d3fc104182984dfcac33677b456923373`, rama `main`, árbol limpio. Scene flow actual es resiliente pero `SceneContentId`/`LocalSceneAddresses` fijan Camp/Jungle por enum/switch. `ContentCatalogAsset` y Bootstrap prueban el patrón authoring→runtime. Addressables `4.0.1` ya posee `SharedLocal`/`JungleLocal`, perfiles locales y scene handles con ownership único.

## Progreso

- [x] 2026-08-16 13:38 — preflight, inventario y baseline integral completados.
- [x] 2026-08-16 13:49 — contracts/runtime catalog/session y scene IDs semánticos implementados.
- [x] 2026-08-16 13:53 — authoring/manifest Jungle, setup/validator/spawn y selección placeholder creados.
- [x] 2026-08-16 14:02 — fixture de segundo mundo, locked/missing, tres ciclos, Addressables y APK validados.
- [x] 2026-08-16 14:10 — fuentes canónicas actualizadas; diff staged revisado y listo para el commit único de fase.

## Hallazgos

- `SceneContentId` es enum y `LocalSceneAddresses.For` usa un switch Camp/Jungle: contradice la extensibilidad solicitada.
- Jungle es una escena/address existente y local; puede preservarse sin migrar paquete o grupo.
- Save v3 no guarda un mundo activo; tolerar un ID retirado puede demostrarse como resultado `Missing` sin mutar progreso ni schema.
- El primer PlayMode post-cambio falló 2/11 por una expectativa de copy inglesa y dependencia de locale entre tests; se corrigió aislamiento/expectativa y la repetición pasó 11/11.
- El manifest necesitaba checkpoints explícitos además de spawn; se añadió `CheckpointId` antes de la validación integral.

## Decisiones

- 2026-08-16 — El scene loader recibirá un ID/address semántico aportado por manifest; Camp seguirá siendo infraestructura compartida, no un mundo comercial.
- 2026-08-16 — Disponibilidad será un estado de producto local separado; no se introduce entitlement, precio, SKU ni purchase service.
- 2026-08-16 — El `AssetReference` vive solo en authoring Content; Application recibe manifest readonly con address semántico, nunca GUID Unity.

## Plan de implementación

1. Reemplazar SceneContent enum/switch por value ID y transición parametrizada manteniendo ownership/cancel/retry.
2. Añadir WorldManifest, catálogo, disponibilidad, sesión y use case en Domain/Application.
3. Añadir ScriptableObjects/compilador Jungle, scene AssetReference, spawn/catálogos/cues/version/budget y wiring Bootstrap/Presentation.
4. Ampliar Addressables validator/build reports y pruebas EditMode/PlayMode sin añadir assembly.
5. Ejecutar pipeline integral, registrar artefactos y cerrar documentación/commit.

## Comandos y validación

- `scripts/validate` — baseline PASS en 1:20.53; EditMode 85/85, PlayMode 11/11 y APK Development.
- `scripts/validate-content` — PASS; world Development 1/1, `remote=false`; Release fixture falla `WORLD018` como guardrail.
- `scripts/build-addressables-local` — PASS; 41 locations, 821,576 bytes, `remoteCatalog=false`.
- `scripts/test-editmode` — PASS final, 89/89.
- `scripts/test-playmode` — primer intento FAIL 9/11; corrección de tests y repetición PASS 11/11.
- `scripts/validate` — PASS final código 0 en 120.04 s; APK 66,473,622 bytes, SHA-256 `07cd4ad69994f79790c0f8ea14c985c63d05a357e333cce466d29b3c1ec75c9c`.

## Recovery y seguridad

No cambiar paquetes, save schema, remote settings, signing ni otros biomas. Mantener assets de setup idempotentes y no editar `Library`. Ante fallo conservar artifacts, corregir fuente versionada y reejecutar el comando acotado. No limpiar ni revertir cambios ajenos.

## Resultados y retrospectiva

Selva se descubre/carga por datos; el scene loader ya no contiene switches de mundos. Una segunda fixture llega a `scene/test-ocean` con el mismo use case/coordinador. Locked/missing no cargan ni mutan progreso, retry conserva manifest, tres ciclos liberan handles y todo permanece offline. No se inventó un budget numérico: `estimatedInstalledBytes` aporta medición al gate de performance futuro. Release sigue correctamente bloqueado por Draft/PH_.
