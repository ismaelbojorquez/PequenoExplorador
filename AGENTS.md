# AGENTS.md

## Preflight obligatorio

Antes de modificar archivos: leer este archivo completo; leer los documentos relevantes de `docs/`; ejecutar `git status --short --branch`, `git branch --show-current` y `git log -1 --format=fuller`; inspeccionar implementación, configuración, pruebas y diff del alcance. Desde Fase 01, la ausencia de este archivo bloquea el trabajo.

## Reglas

- Preservar cambios ajenos y detenerse ante colisiones no aislables.
- Mantener Android-first, iOS-ready, landscape, offline-first y sin backend para el MVP Selva.
- No añadir SDKs de ads, IAP o analítica ni secretos sin decisión aprobada.
- No afirmar cumplimiento de tienda sin evidencia de build y revisión humana.
- Actualizar decisiones, riesgos, roadmap, changelog e índice con cada fase.
- Hacer commits de una sola fase y validar el diff completo.

Este archivo es intencionalmente conciso y evolucionará en Fase 02.
