# Preflight — Prompt/Fase 09

Fecha: 2026-08-16 09:18 CST (`America/Mexico_City`). Este registro describe hechos observados antes de editar persistencia.

## Git y estado

- Directorio: raíz `/Users/ismael/Developer/PequenoExplorador` observada localmente; la ruta no se copia a configuración runtime.
- Rama: `main`.
- HEAD: `fd791cc7d739c329f638072822ce87762eaaad82`, `test(gate-a): audit compilable foundation`.
- Gate A: `PASS` documentado y contrastado con implementación/tests.
- Cambio preexistente: `A  docs/prompts/00_MASTER_CODEX_PROMPTS.md`, staged y fuera de alcance; no se limpia, edita ni incorpora a F09.
- Diff unstaged inicial: vacío. El diff cached ajeno tiene trailing whitespace en sus líneas 3–4.

## Documentación e implementación inspeccionadas

Se leyeron completos `AGENTS.md`, `.agent/PLANS.md`, `STATUS`, índice, arquitectura, estándares, playbook, testing, decisiones, riesgos, políticas, dependencias y roadmap. `docs/10_SAVE_SYSTEM.md` no existía. Se inventariaron Packages, ProjectSettings, scripts, nueve asmdefs y código de Domain/Application/Infrastructure/Bootstrap/tests.

No existían `PlayerProgress`, `ISaveService`, filesystem de save, DTO, migración, autosave ni uso runtime de `Application.persistentDataPath`. La foundation real sí contiene bootstrap determinista, servicios locales seguros y scene flow Addressables.

## Baseline ejecutado

`scripts/validate` finalizó con código `0` antes de editar:

- repository/shell checks: `PASS`;
- compile/import y validador de fronteras: `PASS`;
- Addressables local: `PASS`;
- EditMode: `29/29`;
- PlayMode: `4/4`;
- APK Android Development: `PASS`.

## Serialización evaluada

El lock ya resuelve el módulo builtin `com.unity.modules.jsonserialize` `1.0.0`. Se selecciona `UnityEngine.JsonUtility` sin añadir paquete: compatible con el Editor fijado/IL2CPP y suficiente para DTOs explícitos con campos/arrays. Se prohíben diccionarios, polimorfismo, referencias Unity y nombres de tipos como contrato. SHA-256 aporta integridad accidental, no cifrado ni seguridad frente a un atacante con acceso al dispositivo.

## Desviación de secuencia

La cadena humana llama F08 al scene flow ya implementado; el roadmap canónico histórico asigna F08 al dominio del loop y mantiene F07 abierta. La solicitud actual autoriza expresamente Prompt 09. Este incremento crea solo la raíz mínima vacía requerida por save y no declara gameplay, loop ni contenido completados.
