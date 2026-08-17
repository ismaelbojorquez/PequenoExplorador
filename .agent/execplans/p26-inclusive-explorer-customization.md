# ExecPlan — personalización inclusiva y persistente del Explorer

- Fase/Gate: Prompt 26 / Gate B permanece FAIL
- Estado: Complete
- Creado/actualizado: 2026-08-17 05:07 CST
- Owner: Avatar Customization Engineer / Inclusive Design

## Propósito y alcance

Implementar ocho slots cosméticos sin género, catálogo mínimo local, preview antes de unlock/equip, transacción de Estrellas separada de equip y persistencia Save v11. Debe verse en Camp y Selva, degradar a defaults si se retira una definition y usar materiales compartidos + `MaterialPropertyBlock`. Se excluyen modelos finales, IAP, monetización, red y contenido masivo.

## Contexto y orientación

HEAD inicial `40c94b90a2aab3d1662a8f8fac99a3c0d0f5ac87`, rama `main`, árbol limpio. `scripts/validate` previo PASS: 147 EditMode, 25 PlayMode y APK Development. `PH_Explorer.prefab` conserva root/NavMesh/cámara y cuatro hijos visuales; no existe Customization. Camp ya expone `camp-station.customization` deshabilitada. Economy usa transaction IDs durables y Save v10 conserva Camp unlocks.

## Progreso

- [x] 2026-08-17 04:31 CST — preflight, documentos, prefab, servicios, tests y baseline contrastados.
- [x] 2026-08-17 04:48 CST — contratos, catálogo, use cases, PlayerProgress/DTO y migración v10→v11 implementados.
- [x] 2026-08-17 04:55 CST — autoría idempotente, 8 slots/20 opciones, rig sobre el prefab existente, preview Camp y UI cableados.
- [x] 2026-08-17 04:56 CST — validator, Release guard, ES/EN y debug Development-only verificados.
- [x] 2026-08-17 05:07 CST — EditMode 155/155, PlayMode 26/26, Addressables y APK Development PASS; docs actualizados.
- [x] 2026-08-17 05:14 CST — revisión final añadió detección de scripts faltantes en assets canónicos; prefab, Camp y Bootstrap validaron limpios. Diff/Markdown/configuración revisados; listo para commit.

## Hallazgos

- El prefab actual solo ofrece `PH_Body`, `PH_Head` y `PH_Backpack`; la extensión puede conservar su root y añadir un rig genérico por slot.
- La arquitectura canónica aún contiene referencias textuales heredadas a save v9 aunque runtime ya es v10; se corregirán solo donde toque esta fase.
- No existe Android físico conectado; la validación material/touch real deberá permanecer `NOT RUN` si continúa así.
- La primera ejecución PlayMode tras añadir la vista reveló resolución de localización antes de inicialización; se difirió el binding visual hasta `Open`, manteniendo `Awake` sin resolver texto.
- La copia de preview Camp heredaba `ExplorerLocomotionRoot` y rompía el conteo de roots; el setup elimina locomotion/NavMesh solo de esa instancia, no del prefab.

## Decisiones

- 2026-08-17 — ocho slots tipados y definitions readonly; categorías/compatibilidad usan IDs extensibles, no enums de género.
- 2026-08-17 — ownership de cosméticos y equipped por slot se añaden a Save v11; defaults permanecen en Content y nunca se copian al save.
- 2026-08-17 — unlock con estrellas construye saldo+transaction+ownership en un snapshot; equip es un commit separado y nunca gasta.
- 2026-08-17 — Presentation usa `sharedMaterial` y `MaterialPropertyBlock`; no crea material instances por opción.

## Plan de implementación

1. Añadir IDs/estado Domain, catálogo/use cases Application y Save DTO/migración v10→v11.
2. Añadir authoring Content para slots/cosméticos, defaults, tags, costos y requisito de progreso.
3. Añadir rig/presenters genéricos, panel Camp y bindings de escena/Explorer sin alterar locomoción.
4. Crear setup/validator CLI, fixtures mínimos, localización ES/EN y guard rails Release/debug.
5. Cubrir unit/integration, ejecutar pipeline completo, actualizar fuentes canónicas y commitear.

## Comandos y validación

- `scripts/validate` — baseline inicial PASS: 147/147 EditMode, 25/25 PlayMode, Addressables y APK Development.
- `scripts/setup-customization` — PASS dos veces consecutivas; salida `slots=8 cosmetics=20 saveSchema=11`.
- `scripts/validate-customization` — PASS Development; fixture Release devuelve `CUSTOM005` para `PH_`.
- `scripts/test-editmode` — PASS 155/155.
- `scripts/test-playmode` — PASS 26/26.
- `scripts/validate` final — PASS: repository/compile/Addressables/suites/APK.
- APK Development — 67,417,385 bytes; SHA-256 `f5e80305087e4dcb2c686310104d343fcfb2c188631663805fce3d572a7a918c`; build frío 203.586 s.
- `adb devices` — NOT RUN en hardware: lista vacía.

## Recovery y seguridad

No editar scene/prefab YAML ni GUIDs manualmente: el setup Editor será idempotente. Conservar todos los cambios existentes; ante fallo revisar el primer log bajo `artifacts/`. No tocar `Library`, no instalar dependencias, no activar la estación adulta, no crear IAP/red y no publicar/push. Save v10 siempre migra a v11 mediante un paso puro; schema futuro sigue read-only.

## Resultados y retrospectiva

La personalización quedó data-driven, persistente y sin género/IAP. Preview no muta; unlock y equip son separados; defaults recuperan IDs retirados; incompatibilidad no destruye selección; colores no instancian materiales. Todo visual sigue `PH_` y Release fail-closed. Gate B permanece FAIL porque faltan Prompts 27–29/journey; Prompt 27 es el siguiente.
