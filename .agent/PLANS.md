# ExecPlans vivos

Un ExecPlan es el registro ejecutable y autocontenido de trabajo complejo. Permite que otra sesión continúe sin memoria del chat y explica tanto el objetivo como el estado real.

## Cuándo es obligatorio

Crear `.agent/execplans/<fase>-<slug>.md` antes de implementar cuando se cumpla cualquiera:

- feature o refactor cruza capas, asmdefs, escenas/sistemas o varias fases;
- migración de save, paquetes/SDKs, permisos, privacidad, monetización, build/release o recuperación riesgosa;
- trabajo esperado en más de una sesión o con pasos que deben poder revertirse;
- incertidumbre técnica que exige prototipo, hitos y decisión registrada;
- la solicitud humana lo exige.

No crear ExecPlan para corrección pequeña, cambio documental local o edición mecánica con validación directa. La razón es operacional: un plan vivo reduce pérdida de contexto en trabajo complejo, pero en cambios triviales añade estado duplicado y riesgo de quedar obsoleto.

## Reglas de mantenimiento

- Escribirlo antes de cambios materiales y enlazarlo desde [`docs/STATUS.md`](../docs/STATUS.md).
- Mantenerlo al día después de cada hito, hallazgo, decisión, fallo o cambio de rumbo; usar fecha y zona horaria.
- Marcar hechos observados, hipótesis y decisiones por separado.
- Registrar comandos exactos y resultados resumidos, sin secretos ni rutas personales.
- No declarar un paso completo sin evidencia. Usar `NOT RUN`, `FAIL` o `BLOCKED` cuando corresponda.
- Al cerrar, resumir resultados/deuda y conservarlo como evidencia histórica; `STATUS` deja de señalarlo como activo.
- Nunca crear planes ficticios o retroactivos solo para aparentar proceso.

## Formato obligatorio

Copiar esta estructura y reemplazar todo texto entre `<...>`:

```markdown
# ExecPlan — <resultado concreto>

- Fase/Gate: <fase y gate>
- Estado: Proposed | In Progress | Blocked | Complete
- Creado/actualizado: <YYYY-MM-DD HH:MM zona>
- Owner: <rol o persona>

## Propósito y alcance

<Valor observable, incluidos y excluidos, criterios de aceptación.>

## Contexto y orientación

<Estado actual verificado, fuentes canónicas, rutas/símbolos relevantes y términos que una sesión nueva necesita.>

## Progreso

- [ ] <fecha/hora — hito verificable>

## Hallazgos

- <evidencia inesperada, impacto y comando/archivo que la demuestra>

## Decisiones

- <fecha — decisión, razón, alternativas y consecuencias>

## Plan de implementación

1. <edición concreta, ubicación y resultado esperado>

## Comandos y validación

- `<comando exacto>` — <qué demuestra; resultado o NOT RUN + motivo>

## Recovery y seguridad

<Cómo detener/reanudar, checkpoints, operaciones peligrosas y recuperación no destructiva.>

## Resultados y retrospectiva

<Qué quedó, qué no, pruebas/build, deuda, bloqueos y siguiente acción.>
```

El plan describe resultados verificables, no microtareas decorativas. Si la realidad contradice el plan, se actualiza primero el plan y se conserva el hallazgo.
