# ExecPlan — Camp hub progresivo y primera mejora atómica

- Fase/Gate: Prompt 25 / Gate B permanece FAIL
- Estado: Complete
- Creado/actualizado: 2026-08-17 04:15 CST (`America/Mexico_City`)
- Owner: Hub/Progression Systems Engineer y Level Designer

## Propósito y alcance

Convertir Camp en un hub funcional con estaciones data-driven para expedición, álbum, personalización futura y acceso adulto separado. Añadir una sola mejora visual permanente —Mesa de observación→Rincón de exploración— con preview, confirmación, costo en Estrellas de Explorador, persistencia e idempotencia. No añadir compras reales, contenido educativo bloqueado, arte final, personalización funcional ni más edificios.

La aceptación exige que spend+unlock se construyan como una única mutación de `PlayerProgress`, se solicite un checkpoint, el estado sobreviva reload y migración v9→v10, y Presentation solo active variantes/acciones existentes.

## Contexto y orientación

HEAD inicial `10175314e00efa35648b0ac48bc06715d845cd01`, rama `main`, árbol limpio y ahead 24. Unity `6000.3.22f1`. `scripts/validate` inicial pasó repository/shell, compile, Addressables, EditMode `141/141`, PlayMode `24/24` y APK Development.

Camp actual es una escena Addressable placeholder sin coordinator propio. `SceneTransitionView` posee entrada/salida de mundo, `AlbumView` su acceso Camp-only, `EconomyView` el saldo y `MissionView` la misión; Bootstrap los compone. `PlayerProgress` schema v9 no guarda upgrades. `SpendStarsUseCase` solo muta wallet, de modo que no puede encadenarse a unlock sin riesgo de checkpoint intermedio.

Fuentes canónicas: `docs/01_GDD.md`, `docs/06_WORLD_DESIGN.md`, `docs/ECONOMY_REWARDS.md`, `docs/10_SAVE_SYSTEM.md`, `docs/14_UI_UX.md`, `docs/15_ART_DIRECTION.md`, `docs/ART_ASSET_REQUIREMENTS.md`, `docs/02_TECHNICAL_ARCHITECTURE.md`, `docs/18_TESTING.md` y este plan.

## Progreso

- [x] 2026-08-17 03:48 CST — preflight Git/documental, inventario de Camp y baseline completa ejecutados.
- [x] 2026-08-17 — contratos, progreso y migración save v10 implementados.
- [x] 2026-08-17 — compra atómica, authoring, catálogo y validadores implementados.
- [x] 2026-08-17 — hub/estaciones/preview/variantes y composición implementados.
- [x] 2026-08-17 — suites, documentación y validación integral completadas; diff listo para el commit de fase autorizado.

## Hallazgos

- Camp ya dispone de tres responsabilidades separadas en Bootstrap: world navigation, álbum y saldo/misión. El hub debe enlazarlas, no duplicarlas.
- `SpendStarsUseCase` confirma un snapshot solo de Economy. La compra Camp requiere un caso de uso propio que produzca wallet+unlock en el mismo `PlayerProgress` antes de `Commit`.
- La primera mejora canónica es `Mesa de observación`; debe seguir visual y nunca condicionar discovery, learning, misión o álbum.
- El acceso adulto todavía no tiene parental gate aprobado. La estación debe permanecer no interactiva/future y fuera del recorrido infantil, sin simular seguridad.
- El primer PlayMode detectó que `Render` anulaba el preview dentro del mismo frame; se cambió el ownership visual para no re-renderizar la variante persistida mientras el modal de preview está abierto.
- La baseline material de dos fixtures antiguos se normalizaba al importar. El generador ahora escribe `_BaseColor` sin tocar el alias `_Color`, evitando dirty drift ajeno futuro.

## Decisiones

- 2026-08-17 — usar cuatro estaciones data-driven: Expedición, Álbum, Personalización futura y Área adulta futura; las dos últimas no implementan feature ni compra.
- 2026-08-17 — usar preview→confirmación explícita en dos pasos; ningún tap de estación gasta estrellas.
- 2026-08-17 — persistir `unlockedCampUpgradeIds[]` en schema v10; variantes visuales se derivan de Content+progreso y no se guardan como flags duplicadas.
- 2026-08-17 — costo provisional de la primera mejora: 3 estrellas, alcanzable con discovery+mision fixture sin compra ni grind; queda sujeto a playtest/tuning.

## Plan de implementación

1. Añadir IDs/definitions/read models Camp y estado de upgrades inmutable a Domain/Application.
2. Crear `PurchaseCampUpgradeUseCase` que valide prerequisitos/costo/idempotencia y confirme un único snapshot wallet+unlock; migrar Save v9→v10.
3. Crear ScriptableObjects/catálogo/validator de estaciones, referencias, costos, ciclos y placeholders; integrar al pipeline.
4. Crear `CampHubView` y variantes `PH_` enlazadas a las fachadas de navegación/álbum existentes; mantener Parents separado y deshabilitado.
5. Cubrir EditMode/PlayMode/migración/ratios/roundtrip, actualizar docs y ejecutar `scripts/validate`.

## Comandos y validación

- `git status --short --branch && git branch --show-current && git log -1 --format=fuller` — PASS, limpio en `main`.
- `scripts/validate` — baseline PASS: EditMode `141/141`, PlayMode `24/24`, Addressables local y APK Development.
- `scripts/setup-camp` — PASS, cuatro estaciones/una mejora y wiring generado.
- `scripts/validate-camp` — PASS.
- `scripts/test-editmode` — primer run FAIL por constraint NUnit incompatible en un test nuevo; corregido. Final `147/147` PASS, incluido Release gate `CAMP005`.
- `scripts/test-playmode` — primer run FAIL por preview anulado; corregido. Final `25/25` PASS.
- `scripts/validate` — PASS completo: Addressables 47 locations/1,282,243 bytes y APK Development.
- APK final: `artifacts/builds/PequenoExplorador-development.apk`, 106,873,974 bytes, SHA-256 `bc0117ceec68cc73821a8ae337286bc5625b69daf38d46ded76a5fac0fb8f925`, 15.980 s incremental (cold previo 200.231 s), API 26/36, IL2CPP/ARM64. El comando fue repetible, pero el hash Development cambió entre runs por metadatos internos.
- `scripts/build-android-release` — `BLOCKED`/exit 2 por fixtures/world/learning `PH_` ya registrados; test Camp confirma además `CAMP005`. Signing externo seguiría bloqueado después de contenido.
- `adb devices -l` — `NOT RUN` en hardware: no se listó dispositivo físico.

## Recovery y seguridad

No tocar signing, remote, permisos, paquetes, stores ni compras reales. Mantener cambios limitados a Prompt 25 y no usar comandos Git destructivos. Generadores Editor deben ser idempotentes y conservar GUIDs. Ante fallo de migración, conservar v9 y corregir el nuevo paso puro; nunca reinterpretar migraciones anteriores. Ante fallo de compra, comprobar que el repositorio conserva exactamente el snapshot anterior.

## Resultados y retrospectiva

Camp enlaza las fachadas existentes sin duplicar navegación. La única mejora se compra con una mutación lógica atómica, persiste en schema v10 y se representa mediante variantes Content/scene. Las estaciones futuras no simulan funcionalidad ni compra. Release sigue fail-closed por placeholders y signing; Gate B continúa `FAIL`. El siguiente incremento es Prompt 26.
