# Preflight — Prompt 13 input y adaptación móvil

Fecha: 2026-08-16 (`America/Mexico_City`). Alcance observado antes de editar.

## Estado inicial verificado

- Directorio: raíz `/Users/ismael/Developer/PequenoExplorador` observada localmente; no se versionó la ruta.
- Git: `main`, limpio, `origin/main [ahead 4]`.
- HEAD: `ea023e34786119b7a9f4ee664075184eef928170`, `feat(audio): add localized child friendly audio framework`.
- Diff staged/unstaged: vacío; no hubo cambios ajenos ni colisión.
- Unity: `6000.3.22f1`; Input System directo `1.20.0` coincide en manifest/lock y T-013. No se instaló ni actualizó paquete.
- ProjectSettings: Input System only (`activeInputHandler: 1`), autorrotación solo landscape left/right.

## Lectura e inventario

Se leyeron completos `AGENTS.md`, `STATUS`, UI/UX, arquitectura, gameplay loop, decisiones, estándares, testing, playbook y reglas de ExecPlan. La accesibilidad canónica estaba integrada en `14_UI_UX.md`; no existía contrato técnico separado.

El inventario de runtime encontró un solo `InputSystemUIInputModule` creado por scene-flow y cero `UnityEngine.Input`, `Input.Get*`, `Touchscreen.current`, `Keyboard.current` o `Mouse.current`. No existían action asset, gestos, safe-area service, haptics ni tests Input. Camp/Jungle seguían placeholders sin gameplay.

## Baseline ejecutada

`scripts/validate` finalizó código `0` en `1:15.07`: repository checks, compile/validators, Addressables local, EditMode `70/70`, PlayMode `7/7` y APK Development. Esta evidencia describe únicamente el estado anterior; no se heredó como resultado de Prompt 13.

## Desviaciones y decisiones

- El estado esperado se confirmó: servicios/save/config/localización/audio compilables y repo limpio.
- Device Simulator no estaba fijado; no se añadió dependencia. Se decidió harness puro/PlayMode y mantener hardware real requerido antes de Gate C.
- El cambio cruza cinco capas/escena/tests y activó el ExecPlan [`p13-touch-input.md`](../.agent/execplans/p13-touch-input.md).
