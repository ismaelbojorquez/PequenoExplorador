# Preflight Prompt 18 — discovery persistente

Fecha: 2026-08-16 15:36 (`America/Mexico_City`).

| Control | Evidencia observada | Resultado |
|---|---|---|
| Contrato | `AGENTS.md` y `docs/STATUS.md` leídos completos; ambos existen. | `PASS` |
| Git | `main`, árbol limpio, `ahead 9`; HEAD `f6a38be097b6827f4b6a319fc363d264b9be8310`. | `PASS` |
| Fuentes | Discovery, GDD/economía, Content Model/Pipeline, Save, Interaction, arquitectura, estándares, decisiones, riesgos, testing y review leídos. | `PASS` |
| Implementación | Inventariados Domain/Content/Interaction/Save, DTOs/migraciones, Bootstrap y tests; no existe feature Discovery runtime. | `PASS` |
| Save real | `LocalSaveService.CurrentSchemaVersion = 3`; DTO v3 guarda `discoveryIds` planos y Bootstrap registra `v0→v1→v2→v3`. | Verificado; Prompt 18 requiere schema v4 y migración pura `v3→v4`. |
| Catálogo | Un `discovery.jungle.placeholder` Draft/PH_ resuelve por ID; Release lo rechaza. | Development utilizable; Release `BLOCKED` correctamente. |
| Interacción | `interaction.fixture.animal` completa el núcleo genérico pero todavía no concede progreso. | Punto de integración confirmado. |
| Baseline | `scripts/validate` código `0` en `1:17.19`: checks, compile, Addressables, EditMode `99/99`, PlayMode `17/17` y APK Development. | `PASS` |
| Hardware | `adb devices` no se reinterpreta como evidencia: no hay dispositivo conectado en el estado heredado. | Android físico `NOT RUN`. |
| Colisiones | No había diff staged/unstaged ni cambios ajenos que aislar. | `PASS` |

Desviación documental detectada: la última línea de `02_TECHNICAL_ARCHITECTURE.md` todavía afirma que no existe interacción, aunque Prompt 17 la implementó. Se corregirá junto con la arquitectura de discovery; no altera la baseline ejecutada.
