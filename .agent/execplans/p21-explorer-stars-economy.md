# ExecPlan — economía simple de Estrellas de Explorador

- Fase/Gate: Prompt 21 / Gate B FAIL
- Estado: Complete
- Creado/actualizado: 2026-08-17 01:27 America/Mexico_City
- Owner: Game Economy Engineer

## Propósito y alcance

Implementar una única moneda virtual ganable, persistente e idempotente: Estrellas de Explorador. Incluye wallet, grants y gastos, reward definitions data-driven, ledger técnico acotado, integración con el discovery VS-D-A01, display localizado y validación Development/Release. Excluye IAP, tienda, precios definitivos, moneda premium, recompensas aleatorias, rachas, temporizadores y contenido nuevo.

## Contexto y orientación

HEAD inicial `ee3df2504a6309da2006972e41c41967951ea9f9` en `main`, árbol limpio. Unity `6000.3.22f1`; baseline previo PASS: compile, EditMode 117/117 y PlayMode 21/21. Save v6 ya reserva `int stars`, pero no existen wallet, operaciones, reward definitions ni ledger. Discovery/fotografía usan `AutosaveCoordinator.Latest`; `CapturePhotoUseCase` es el punto de integración normal. GDD aún llama al candidato “materiales de exploración” y debe ser sustituido por la decisión humana actual.

## Progreso

- [x] 2026-08-17 01:20 — preflight, inventario y baseline ejecutados sin editar.
- [x] 2026-08-17 01:10 — Domain/Application y save v7 con migración v6→v7 implementados.
- [x] 2026-08-17 01:14 — catálogo, composición, discovery grant, UI y debug Development cableados.
- [x] 2026-08-17 01:18 — validator, wrappers, EditMode/PlayMode y documentación canónica añadidos.
- [x] 2026-08-17 01:27 — `scripts/validate` PASS completo, manifest/diff/artefactos inspeccionados.

## Hallazgos

- `PlayerProgress.Stars` es solo un contador reservado; `ProcessedDiscoveryGrantIds` pertenece a Discovery y no puede reutilizarse como ledger económico.
- Un grant disparado solo cuando Discovery devuelve `First` puede perderse si el proceso cae entre commits. La recuperación usará una transaction key determinista por discovery y se intentará también en repeticiones; idempotencia evita duplicados.
- El ledger reciente no puede ser la autoridad de idempotencia: se conservarán keys técnicas procesadas por separado y un ledger acotado solo para diagnóstico.

## Decisiones

- 2026-08-17 — Save v7 añade transaction keys económicas y hasta 32 entradas recientes; mantiene `stars` por compatibilidad de migración.
- 2026-08-17 — La recompensa fixture Approved será data-driven y de 1 estrella, valor provisional del Vertical Slice; no fija precios ni tuning post-MVP.
- 2026-08-17 — Sources permitidos: discovery, misión, actividad y colección; usos permitidos: mejoras de Camp y cosméticos. Ningún flujo con dinero real.

## Plan de implementación

1. Crear value objects, intent/outcomes, use cases y repositorio sobre `PlayerProgress`/autosave.
2. Migrar save v6→v7 y probar serialización, future-version, atomicidad lógica e idempotencia.
3. Crear reward asset/catalog mínimo, validator y wiring exclusivo del composition root.
4. Conectar fotografía/discovery con transaction key determinista y display localizado reducible.
5. Actualizar fuentes canónicas, ejecutar compile/tests/Addressables/APK y revisar permisos/diff.

## Comandos y validación

- `scripts/compile` — baseline PASS.
- `scripts/test-editmode` — baseline PASS 117/117.
- `scripts/test-playmode` — baseline PASS 21/21.
- `scripts/check-repository` — PASS, Markdown 93, JSON 20, workflows 1, secrets 0 y Bash OK.
- `scripts/validate-economy` — PASS; currency 1, reward 1, premium/purchases/debug Release 0.
- `scripts/validate` — PASS; compile, Addressables local, EditMode 122/122, PlayMode 22/22 y APK Android Development.
- `git diff --check && git diff --cached --check` — PASS en cierre; solo archivos intencionales de Prompt 21.

## Recovery y seguridad

No se instalarán dependencias, no se tocarán SDKs/IAP/ads, no habrá push ni signing. Los cambios se limitan a esta fase. Si una migración o build falla, conservar evidencia en `artifacts/`, corregir sin reescribir historia y mantener `STATUS`/este plan como In Progress.

## Resultados y retrospectiva

Wallet y operations idempotentes quedan desacoplados de UI/IAP/Ads. Save v7 conserva saldos v6 y añade metadata económica vacía; foto/discovery concede una estrella y reintenta sin duplicar. APK Development: 106,455,036 bytes, SHA-256 `372719dbf8656ca2ea19637c9b4abad72c16f0190adc6683c57b49dc32e2b12c`, API 26/36, IL2CPP ARM64. El incremento de tamaño, tuning/playtest, hardware Android y preferencia reduce-motion persisten como deuda. Gate B sigue FAIL; siguiente Prompt 22.
