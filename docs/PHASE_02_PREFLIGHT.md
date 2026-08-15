# Fase 02 — Evidencia de preflight del contrato operativo

Fecha: 2026-08-14 (`America/Mexico_City`). Alcance: instrucciones de larga duración; sin Unity, código, paquetes ni gameplay.

## Estado inicial verificado

- `pwd`: raíz esperada del workspace; la ruta personal no se persiste.
- Inventario: 31 archivos fuera de `.git`; solo baseline raíz y documentación de Fases 00–01.
- `AGENTS.md`: presente y leído completo; no hubo bloqueo.
- `git status --short --branch`: `## main`, limpio.
- `git branch --show-current`: `main`.
- `git log -1 --format=fuller`: `1fa9dc09bd491ac098adad0143c3b437250670ea`, `docs(product): define educational game and MVP`.
- `git diff` y `git diff --cached`: sin salida antes de editar.
- `Assets/`, `Packages/` y `ProjectSettings/`: ausentes; no existe implementación, configuración o test Unity que inspeccionar.

## Lectura e inspección

Se leyeron completos `AGENTS.md`, README/índice, visión, GDD, loops, educación, mundo, discovery, learning, misiones, UI/UX, arte, audio, MVP scope, fuentes factuales, playtests, decisiones, riesgos, roadmap, matrices, requisitos de assets/audio, changelog, evidencias previas y archivos raíz de configuración.

## Crítica de normas previas

- El preflight y los límites generales eran accionables, pero `AGENTS.md` no definía estado vivo, jerarquía canónica, reglas C#/Unity, dependencias, ExecPlans, review, evidencia, `NOT RUN`, recovery ni DoD completo.
- No había comandos inventados, pero tampoco una lista explícita que evitara inventarlos antes del proyecto.
- El roadmap anterior asignaba F02 a crear Unity y F03 a scaffolding. La instrucción humana actual ocupa F02 con el contrato operativo; se reconcilia F03 como foundation técnica conjunta para conservar Gate A y las 58 fases.
- No se detectaron cambios ajenos, colisiones, archivos basura o contradicción material con el estado esperado.

## Prueba de reanudación documental

Partiendo sin memoria del chat, `AGENTS.md` conduce a `STATUS.md`; desde allí se identifica producto/límites, F02 y Gate A en progreso, F03 como siguiente acción, ausencia de ExecPlan activo y de proyecto/build, bloqueos humanos, fuentes que revalidar y playbook/recovery. La comprobación automatizada de esas respuestas pasó; no demuestra compilación ni sustituye el preflight de la próxima sesión.
