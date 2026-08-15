# Preflight — Fase 06: composition root y servicios

Fecha: 2026-08-14, `America/Mexico_City`. Evidencia obtenida antes de editar implementación.

## Estado inicial

- `AGENTS.md` y `docs/STATUS.md`: presentes y leídos completos; su ausencia habría bloqueado.
- Git: rama `main`, árbol limpio, sin diff staged/unstaged.
- Último commit: `d1b008006e27d77ffbef3801ced12cd8b96b5ecf`, `ci(unity): add reproducible validation pipeline`.
- Proyecto: Unity `6000.3.22f1`; manifest/lock exactos, nueve asmdefs, una escena Bootstrap; sin remotos ni dependencias nuevas.

## Lecturas e inspección

Se leyeron arquitectura, estándares, decisiones, validación, testing, status, riesgos, roadmap, ProjectVersion, manifest/lock y el código/configuración completos de Bootstrap, Shared, Application, Infrastructure, Presentation y tests. `Shared` solo contiene `.gitkeep`; las capas son markers y `DiagnosticBootstrap` no tiene lifecycle ni servicios. La escena tiene un único marcador diagnóstico.

## Baseline ejecutada

| Control | Resultado | Tiempo real | Evidencia |
|---|---|---:|---|
| Checks del repositorio | PASS | 0.295 s | 48 Markdown, 13 JSON/asmdefs, un workflow, cero secretos detectados. |
| Compile/validadores | PASS | 10.598 s | Código 0; fronteras y placeholders válidos. |
| EditMode | PASS | 12.373 s | 5/5, 0 failed; JUnit generado. |
| PlayMode | PASS | 14.877 s | 1/1, 0 failed; JUnit generado. |

Git permaneció limpio. La capacidad batch de F05 quedó contrastada, no heredada de su reporte.

## Desviación canónica

La orden vigente sitúa composition root/servicios inmediatamente después de F05, mientras `ROADMAP.md` reservaba F06 para shell UI. Se ejecuta como F06 por precedencia de la solicitud humana y se actualizará el roadmap de forma explícita, conservando 00–57 y el alcance Selva.
