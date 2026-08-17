# Playbook de validación y evidencia

Este documento define cómo probar y cómo hablar de resultados. Ninguna sesión hereda un PASS anterior sin contrastar el estado actual.

## Vocabulario obligatorio

| Estado | Uso |
|---|---|
| `PASS` | Comando/revisión pertinente ejecutado sobre el estado reportado y criterio satisfecho. |
| `FAIL` | Se ejecutó y el criterio no se satisfizo; registrar salida/impacto. |
| `BLOCKED` | Falta decisión, acceso, herramienta o condición externa necesaria. |
| `NOT RUN` | No se ejecutó; registrar motivo e impacto. Nunca equivale a PASS. |

“Compila” exige compilación real sin errores en target/configuración identificados. “Tests pasan” exige comando, suites y resultado. “Cumple” exige controles técnicos y revisión humana/política aplicables. “Listo” exige Definition of Done completa, no solo documentos.

## Comandos conocidos

Ejecutar desde la raíz del repositorio según alcance:

```sh
git status --short --branch
git branch --show-current
git log -1 --format=fuller
git diff --check
git diff --cached --check
git lfs fsck
```

Para inventario pueden usarse `rg --files`, `find` y `rg`; registrar el patrón relevante. Desde Fase 05 el comando local canónico, portable entre macOS/Linux, es:

```sh
scripts/validate

# Ejecuciones acotadas
scripts/check-repository
scripts/compile
scripts/validate-content
scripts/validate-localization
scripts/validate-album
scripts/build-addressables-local
scripts/test-editmode
scripts/test-playmode
scripts/build-android-development
scripts/build-android-locales
scripts/build-android-release  # fallo esperado: signing externo requerido
```

`scripts/validate` ejecuta checks estáticos, compile con fronteras/contenido, EditMode, PlayMode y APK Development. Localiza el Editor fijado o respeta `UNITY_EDITOR`; toda salida queda en `artifacts/` ignorado y los logs sustituyen rutas de máquina. Cada subcomando devuelve código no cero al fallar. Release devuelve `3` deliberadamente y permanece `BLOCKED`, no `PASS`. Formatos, archivos y recuperación detallados: [`18_TESTING.md`](18_TESTING.md) y [`20_ANDROID_RELEASE.md`](20_ANDROID_RELEASE.md).

## Secuencia mínima por cambio

1. Preflight de [`../AGENTS.md`](../AGENTS.md) y estado vivo.
2. Definir aceptación, riesgo y si requiere ExecPlan.
3. Inspeccionar archivos/tests/configuración actuales antes de editar.
4. Aplicar cambio mínimo y revisar diff durante el trabajo.
5. Ejecutar validación proporcional; conservar salida resumida y artefactos identificables.
6. Revisar [`CODE_REVIEW_RULES.md`](CODE_REVIEW_RULES.md).
7. Actualizar docs/decisiones/riesgos/status y revisar diff completo staged/unstaged.
8. Confirmar Git intencional; commit/push solo si están autorizados por separado.

## Matriz por tipo de cambio

| Cambio | Evidencia mínima |
|---|---|
| Solo documentación | UTF-8/Markdown, enlaces relativos, contradicciones, `git diff --check`, prueba de reanudación si afecta instrucciones. |
| Domain/Application | Compile + EditMode/unit tests afectados y regresión; verificar ausencia de `UnityEngine` en Domain. |
| Presentation/Content | Compile + EditMode y PlayMode/escena relevantes; validación de authoring/placeholders. |
| Catálogo data-driven | IDs/duplicados/aliases, referencias localización-audio-visual, states Development/Release, determinismo, reportes y resolución runtime; Android si cambia player. |
| Input/safe area | Validator de mapas/APIs/targets, EditMode de gestos/allocations/rotación, PlayMode `InputTestFixture`, ratios y Android; hardware físico si el Gate lo exige. |
| Locomoción/NavMesh | Validator de pin/prefab/surface/data/cámara/tuning, estados/allocations EditMode, tap válido/inválido/mapas/reduce-motion/unload x3 PlayMode y APK; touch/FPS físico si el Gate lo exige. |
| Interacción contextual | Catálogo/IDs/copy/cues, collider/punto NavMesh/targets, prioridad-rango-cancel-idempotencia EditMode, tap/approach/spam/unavailable/UI/destroy/unload PlayMode y APK; touch físico si el Gate lo exige. |
| Álbum/colección | `scripts/validate-album`, Approved/locked sin fuga, contadores/filtros, missing/removed, photo manifest/cancel/cache, navegación/detalle/Back, ES/EN/pseudo, 4:3–20:9 y APK; fuente grande/touch físico si el Gate lo exige. |
| Localización/copy | Validator de locales/tablas/keys/glifos, ES/EN no vacíos, Smart Strings, pseudo/layout, persistencia y APK por locale cuando cambie runtime. |
| AppConfig/perfiles | Validar ambos assets, mapping/rangos/duplicados, flags Release, override Editor, PlayMode del perfil seleccionado y ambos paths de build/guard. |
| Save/Infrastructure | Tests de round-trip, versión, migración, corrupción, cancelación/interrupción y PII. |
| Dependencia/SDK/permisos | Intake completo, diff de manifests/permisos/tráfico, licencia/SBOM, 16 KB y revisión infantil. |
| Android/iOS/build | Target/configuración/hash del artefacto, warnings, arquitectura, tamaño y prueba de dispositivo aplicable. |
| Release/política | Binario final, permisos/tráfico/SBOM, metadata, fuentes con fecha y aprobación humana; un documento solo no da PASS. |

Si la herramienta necesaria no existe, registrar `NOT RUN` o `BLOCKED`; no sustituirla por una comprobación más débil sin aclararlo.

## Prueba de reanudación

Una sesión nueva debe poder responder desde archivos, sin chat:

1. ¿Qué producto y límites gobiernan? — `AGENTS`, visión y MVP scope.
2. ¿Qué fase/Gate están activos y qué sigue? — `STATUS` y roadmap.
3. ¿Hay ExecPlan activo? — enlace explícito en `STATUS`; si no, debe decir “ninguno”.
4. ¿Qué está realmente disponible? — `STATUS`, Git, inventario y matrices; volver a ejecutar preflight.
5. ¿Qué bloquea y quién decide? — `STATUS`, decisiones y riesgos.
6. ¿Cómo valido y recupero? — este playbook y ExecPlan activo.

La prueba pasa solo si las respuestas son inequívocas, los enlaces resuelven y Git/archivos no contradicen el estado.

## Evidencia y reporte final

Registrar comandos, target, fecha cuando sea temporal, resultado y limitaciones. El reporte de fase usa los 14 apartados estándar: resultado; estado inicial; cambios; arquitectura; archivos; pruebas; resultados; build/artefactos; riesgos; deuda; pendientes humanos; hash; status Git; siguiente fase.

Antes de declarar terminado: implementación/entregable, tests, build requerido, documentación, Git y reporte deben satisfacer el DoD de `AGENTS`. Lo no aplicable se distingue de lo no ejecutado.
