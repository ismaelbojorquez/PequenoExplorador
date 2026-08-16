# ExecPlan — catálogo de contenido data-driven validado

- Fase/Gate: Prompt 14 / Gate B
- Estado: Complete
- Creado/actualizado: 2026-08-16 13:31 America/Mexico_City
- Owner: Game Data Architect / Tools Engineer

## Propósito y alcance

Implementar IDs tipados, authoring ScriptableObject, mapping readonly, catálogo O(1), aliases, validación editorial/referencial y reportes reproducibles. El resultado mínimo permite resolver un discovery neutral `PH_` por ID sin cambiar sistemas centrales. Excluye reglas de discovery, mission, activity, reward, contenido factual final y producción masiva.

## Contexto y orientación

La baseline observada parte del commit `493ee570ed825d4dc4f44d3a68a27b04cbf8e33c`, rama `main`, árbol limpio. Unity es `6000.3.22f1`; Addressables `4.0.1`, Localization `1.5.12` e Input System `1.20.0` ya están fijados. Content contiene AppConfig, tablas, audio y placeholders, pero no definitions/catálogo. `ContentValidationService` valida config/localización/audio/metadata JSON; debe ampliarse sin añadir assemblies ni dependencias.

## Progreso

- [x] 2026-08-16 12:38 — preflight documental/Git, inventario de ScriptableObjects/validadores y baseline completa ejecutados.
- [x] 2026-08-16 13:05 — IDs/definitions/catálogo puro, authoring/mapping y assets mínimos implementados.
- [x] 2026-08-16 13:09 — tooling, inspector, reports y validación Development/Release integrados.
- [x] 2026-08-16 13:14 — Bootstrap resolvió discovery; Release Draft bloqueado y source traceability endurecida.
- [x] 2026-08-16 13:31 — pipeline completo y APK Development PASS; documentación canónica actualizada con la evidencia final.

## Hallazgos

- La decisión T-005 ya fija ScriptableObjects como authoring, nunca estado mutable; no requiere ADR para cambiar de dirección.
- No hay discovery final ni catálogo previo. El candidato tucán permanece Draft y no puede comunicar especie/hecho como contenido aprobado.
- El build Release ya es fail-closed por signing; Prompt 14 debe añadir un bloqueo anterior y accionable cuando exista contenido no Approved.

## Decisiones

- 2026-08-16 — IDs serán value objects tipados de Domain con namespaces textuales; los GUID Unity quedan solo como metadata de assets.
- 2026-08-16 — el catálogo runtime será readonly e indexado al compilar authoring; AssetDatabase se limita a Editor/validator.
- 2026-08-16 — el ejemplo será neutral, Draft y `PH_`; Development lo podrá resolver con watermark, mientras Release deberá rechazarlo.

## Plan de implementación

1. Añadir value IDs y contratos readonly de definitions/catálogo en Domain/Application.
2. Añadir ScriptableObjects, metadata editorial, aliases y compilador determinista en Content.
3. Generar un catálogo mínimo y referencias locales neutralizadas; cablearlo en Bootstrap sin búsquedas runtime.
4. Añadir inspector/generador explícito, validator CLI con JSON/Markdown y guard Release.
5. Cubrir IDs, duplicados, referencias, estados, mapping, aliases y determinismo; ejecutar Android smoke.

## Comandos y validación

- `scripts/validate` — baseline PASS; compile, Addressables, EditMode `77/77`, PlayMode `10/10`, APK Development `66,067,652` bytes.
- `scripts/validate-content` — PASS Development; reportes JSON/Markdown generados.
- `scripts/test-editmode` — PASS `85/85`.
- `scripts/test-playmode` — PASS `11/11`; Bootstrap resuelve `discovery.jungle.placeholder`.
- `scripts/validate` — PASS final en 3:51.84; APK 66,353,374 bytes, SHA-256 `d244ba03df0ab7c8699b012a9f6a484c63db4496ce7437f6f59c620f49298cea`.
- `scripts/build-android-release` — `BLOCKED` controlado, código 2, cinco `DATA025`; signing no alcanzado.

## Recovery y seguridad

No cambiar paquetes, save schema, remote catalogs, permisos, signing ni contenido final. Los assets se crean mediante tooling idempotente y se revisa su YAML. Ante fallo, conservar artifacts, corregir fuente/setup y reejecutar; no editar Library ni incorporar cambios ajenos.

## Resultados y retrospectiva

Foundation data-driven completada sin gameplay ni contenido factual final. Development compila/resuelve el placeholder con watermark; Release falla por item Draft antes de signing. El catálogo es determinista/O(1), reporta paths y no usa AssetDatabase runtime. Siguiente: Prompt 15, framework extensible de mundos.
