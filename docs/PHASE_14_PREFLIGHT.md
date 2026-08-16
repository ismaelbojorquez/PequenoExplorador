# Preflight — Prompt 14 contenido data-driven

Fecha: 2026-08-16 (`America/Mexico_City`). Alcance observado antes de editar.

## Estado inicial verificado

- Git: rama `main`, árbol limpio, `origin/main [ahead 5]`.
- HEAD: `493ee570ed825d4dc4f44d3a68a27b04cbf8e33c`, `feat(input): add accessible touch and device adaptation`.
- Unity: `6000.3.22f1`; paquetes existentes fijados, sin instalación o actualización para esta fase.
- Diff staged/unstaged: vacío; no hubo cambios ajenos ni colisión.

## Lectura e inventario

Se leyeron completos `AGENTS`, `STATUS`, índice, arquitectura, estándares, playbook, testing, diseño educativo, discovery, learning, missions, localización, pipeline/fuentes de contenido, requisitos de arte/audio, decisiones y riesgos. T-005 exige ScriptableObjects de authoring mapeados; Domain no conoce Unity.

El inventario confirmó AppConfig/audio como precedentes de authoring, nueve assemblies y un `ContentValidationService` limitado a config, localización, audio y cuatro metadata JSON `PH_`. No existían IDs tipados de contenido, definitions, catálogo runtime, aliases, reports de catálogo ni discoveries finales.

## Baseline ejecutada

`scripts/validate` terminó código `0`: repository checks, import/compile, Addressables local, EditMode `77/77`, PlayMode `10/10` y APK Development. APK: `66,067,652` bytes, SHA-256 `156b48646ad1d696a3e07f93090f1eb4536716703c0ef898782d04bd355a5a43`, API 26/36, IL2CPP/ARM64. Esta evidencia describe solo el estado anterior.

## Límites y plan

- El ejemplo factual seguirá Draft/placeholder y no afirmará especie/hecho aprobado.
- No se crearán reglas completas, contenido masivo, paquete, endpoint, save migration, permiso, signing o publicación.
- El cambio transversal activa [`p14-data-driven-content.md`](../.agent/execplans/p14-data-driven-content.md).
