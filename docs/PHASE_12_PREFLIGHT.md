# Preflight Prompt 12 — framework de audio infantil

Fecha: 2026-08-16, `America/Mexico_City`.

## Estado inicial observado

- Directorio: raíz de `PequenoExplorador`; `AGENTS.md` y `docs/STATUS.md` presentes y leídos completos.
- Git: rama `main`; `git status --short --branch` mostró `## main...origin/main [ahead 3]` sin cambios. HEAD `caeb68fcfbea1e464094ad8f547ad7c56506d83b`, commit `feat(localization): establish Spanish English content pipeline`; diff staged/unstaged vacío.
- Baseline real: `scripts/validate` devolvió `0` en 117.39 s. Repository checks, compile/import, Addressables local, EditMode `62/62`, PlayMode `6/6` y APK Development español pasaron.
- Estado esperado contrastado: ES/EN/pseudo, config Development/Release, save schema v2, Bootstrap, scene flow y servicios existen. No hay gameplay ni audio final.

## Lectura e inspección

Se leyeron completos `AGENTS.md`, `.agent/PLANS.md`, `docs/STATUS.md`, `docs/README.md`, `docs/16_AUDIO.md`, `docs/AUDIO_REQUIREMENTS.md`, `docs/17_LOCALIZATION.md`, `docs/14_UI_UX.md`, `docs/02_TECHNICAL_ARCHITECTURE.md`, `docs/ENGINEERING_STANDARDS.md`, `docs/18_TESTING.md`, `docs/VALIDATION_PLAYBOOK.md`, `docs/ART_ASSET_REQUIREMENTS.md`, `docs/DECISIONS.md`, `docs/RISK_REGISTER.md` y el alcance humano actual.

Se inventariaron assemblies, Application/Content/Infrastructure/Presentation/Bootstrap, Save/Localization, escenas, tests, BuildTools, Addressables y `ProjectSettings/AudioManager.asset`. El proyecto contiene un único `AudioListener` en Bootstrap, salida estéreo 48 kHz, 32 voces reales y suspensión de output habilitada. No existen `.wav`, `.ogg`, `.mp3`, `.aif/.aiff`, `.mixer`, `AudioClip` ni `AudioSource` de plantilla: no hay archivo que eliminar.

## Decisión de ejecución

La fase cruza contratos Application, Save schema, Content authoring, Infrastructure Unity, Bootstrap/lifecycle, Presentation, escena, validadores, tests y Android. `.agent/PLANS.md` exige ExecPlan; se creó [`../.agent/execplans/p12-audio-framework.md`](../.agent/execplans/p12-audio-framework.md). No se instalará paquete, descargará audio, aceptará licencia, usará micrófono/red ni incorporará grabaciones humanas.
