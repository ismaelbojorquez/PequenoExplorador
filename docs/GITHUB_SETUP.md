# Configuración humana de GitHub

Estado histórico del 2026-08-14: no había remoto configurado. Estado local observado el 2026-08-16: `origin` apunta a `https://github.com/ismaelbojorquez/PequenoExplorador.git` y `main` lo sigue; no se hizo push, no se inspeccionaron settings remotos/Actions/runners y no se aceptaron términos en Prompt 10.

## Crear, verificar o conectar

Antes del próximo push, una persona autorizada debe verificar que el `origin` actual tiene titularidad, visibilidad y licencia compatibles con H-001. No volver a ejecutar `git remote add origin` mientras ya exista. Si se reconstruye el repositorio en una máquina sin remoto, usar:

```sh
git remote add origin <URL_APROBADA>
git remote -v
git push -u origin main
```

No ejecutar el push si la titularidad, licencia, permisos o destino son ambiguos. Configurar un remoto local no demuestra que branch protection, required checks, secrets o runners estén activos.

## Protección de `main`

- exigir pull request y al menos una aprobación;
- bloquear force-push/deletion y exigir conversación resuelta;
- requerir el check `Repository checks`;
- no requerir inicialmente `Unity validation (manual self-hosted)`, porque es opt-in y aún no existe runner;
- restringir quién puede administrar reglas y bypasses.

## Runner Unity y activación

Provisionar un runner dedicado, parcheado y no compartido con workloads no confiables, con labels `self-hosted`, `macOS`, `ARM64`, `unity-6000.3.22f1`. Instalar/activar Unity según términos de la organización, limitar acceso al runner y fijar la variable de repositorio `UNITY_CI_ENABLED=true` solo después de probarlo. El workflow Unity se dispara únicamente de forma manual.

No se inventan secretos para la licencia. Si el mecanismo aprobado requiere credenciales, el owner debe documentar nombres/rotación, usar GitHub Environments y evitar exposición a forks; nunca guardar valores en repo, artifacts o logs. Signing Android permanece fuera de este workflow.

## Actions y supply chain

El workflow usa únicamente `actions/checkout` v4.3.1 y `actions/upload-artifact` v4.6.2, fijadas por SHA completo. Licencias MIT y tags/SHA se verificaron contra sus repositorios oficiales el 2026-08-14. Toda actualización requiere volver a revisar release, licencia y SHA; no reemplazar por tags flotantes ni integrar una Unity Action de terceros sin intake.
