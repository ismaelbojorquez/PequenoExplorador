# Estado vivo del proyecto

Actualizado: 2026-08-16 10:58 (`America/Mexico_City`). Git y archivos observados prevalecen si contradicen este resumen y obligan a actualizarlo.
Las evidencias `PHASE_00/01_PREFLIGHT.md` conservan referencias históricas al roadmap vigente cuando se escribieron; este archivo y `ROADMAP.md` contienen la asignación actual.

## Fase y Gate

- **Fase actual:** Prompt 11 — localización español/inglés y pseudo-localización completada; no equivale a Fase 11 del roadmap histórico.
- **Estado de fase:** `PASS` local. Localization `1.5.12`, ES/EN, pseudo Development, save schema v2, validators, 62 EditMode, 6 PlayMode y APK ES/EN fueron ejecutados. Dispositivo físico sigue `NOT RUN`.
- **Auditoría Gate A:** [`audits/GATE_A_2026-08-15.md`](audits/GATE_A_2026-08-15.md) — `PASS` original y revalidación independiente sobre HEAD F07 `PASS`, sin Critical/Major abierto.
- **Gate actual:** B — Vertical slice playable, iniciado; el flujo actual es placeholder y no gameplay.
- **Siguiente fase:** Prompt 12 — audio/narración placeholder, tras reejecutar preflight y baseline. No producir voz final ni contenido masivo.
- **ExecPlan activo:** ninguno. [El plan Prompt 11](../.agent/execplans/p11-localization-pipeline.md) queda cerrado como evidencia.

## Capacidades verificadas

| Capacidad | Estado | Evidencia/limitación |
|---|---|---|
| Git baseline | Verificado | Fases 00–06 forman la base auditada; debe contrastarse en cada preflight. |
| Producto/Vertical Slice/MVP | Especificado | Fuentes canónicas en visión, GDD y `MVP_SCOPE.md`; no implementado. |
| Contrato de agentes | Especificado | `AGENTS`, planes, estándares, review y playbook de F02. |
| Proyecto Unity | Verificado | `6000.3.22f1`, URP, Bootstrap y estructura mínima en raíz. |
| Pipeline local | `PASS` revalidado | Prompt 11: `scripts/validate` código `0` en 123.96 s; checks, compile/validators, catálogo local, EditMode `62/62`, PlayMode `6/6` y APK ES. JUnit/logs/manifests en `artifacts/`. |
| Fronteras/compilación | `PASS` | Nueve asmdefs, allowlist/cycles y placeholder validados; Domain/Application sin engine. |
| Lifecycle y servicios | `PASS` | Inicio secuencial, cierre inverso, idempotencia, retry, cancelación propia del host, cleanup del servicio en curso, perfiles fail-closed y limpieza de listeners cubiertos por tests. |
| Scene flow / Addressables | `PASS` | `4.0.1`; Boot→Camp↔Jungle local, tres ciclos/ownership y 2 grupos de escena + 6 Localization, todos local-only, sin endpoint. |
| Save local | `PASS` automatizado; dispositivo `NOT RUN` | Schema v2, `JsonUtility` builtin `1.0.0`, SHA-256, atomicidad/backup, v0→v1→v2, future read-only, autosave y locale ES/EN; sin PII/red. |
| Localización | `PASS` local | Unity Localization `1.5.12`; 25 keys, tablas `Shared`/`UI`/`Content`, asset tables `Voice`/`Illustrations`, ES/EN completos, pseudo Development, fallback seguro, CSV export y cambio live/persistido. |
| Config runtime | `PASS` | Dos assets tipados Development/Release, mapping readonly, cero flags Release, loader Bootstrap, override Editor y validador build; fixture insegura falló `CONFIG008`. Sin red/secret/remote config. |
| Tests Unity | `PASS` | EditMode `62/62` (61 del proyecto + 1 stub Addressables), 1.70 s; PlayMode `6/6`, 2.28 s. Incluye ES/EN/pseudo 1280×720/1920×1080, save y scene flow. |
| Build Android/iOS | Android Development ES/EN `PASS`; Release `BLOCKED`; iOS `NOT RUN` | Pre-commit: ES `65,931,773` bytes/SHA `8728eb3e…116c`; EN `65,930,915` bytes/SHA `87cc0a13…574b`; API 26/36, IL2CPP/ARM64, zipalign 16 KB. Release devolvió `3` sin signing. Sin dispositivo físico. |
| CI GitHub | `NOT RUN` | `origin` está configurado y `main` lo sigue; workflow remoto, branch protection, runner y activación CI no se inspeccionaron/ejecutaron. No hubo push. |
| Paquetes | Verificado | Localization `1.5.12` y AndroidJNI builtin `1.0.0` añadidos con pins exactos; Addressables `4.0.1`; sin preview/SDK comercial/binario nativo del paquete. |
| Gameplay/assets finales | No existen | Prohibido escalar contenido antes del Vertical Slice. |

## Fuentes necesarias para reanudar después de Prompt 11

1. [`../AGENTS.md`](../AGENTS.md) y este estado.
2. [`ROADMAP.md`](ROADMAP.md), [`DECISIONS.md`](DECISIONS.md) y [`RISK_REGISTER.md`](RISK_REGISTER.md).
3. [`02_TECHNICAL_ARCHITECTURE.md`](02_TECHNICAL_ARCHITECTURE.md), [`VERSION_MATRIX.md`](VERSION_MATRIX.md) y [`DEPENDENCY_REGISTER.md`](DEPENDENCY_REGISTER.md).
4. [`ENGINEERING_STANDARDS.md`](ENGINEERING_STANDARDS.md), [`VALIDATION_PLAYBOOK.md`](VALIDATION_PLAYBOOK.md) y [`.agent/PLANS.md`](../.agent/PLANS.md).
5. [`RUNTIME_CONFIGURATION.md`](RUNTIME_CONFIGURATION.md), [`10_SAVE_SYSTEM.md`](10_SAVE_SYSTEM.md), [`17_LOCALIZATION.md`](17_LOCALIZATION.md), [`18_TESTING.md`](18_TESTING.md) y [`CONTENT_PIPELINE.md`](CONTENT_PIPELINE.md) para reejecutar `scripts/validate` y conservar local-only.
6. [`MVP_SCOPE.md`](MVP_SCOPE.md) y el prompt humano actual para acotar el siguiente incremento sin inventar progreso.

## Bloqueos y decisiones humanas

- Licencia Unity batch: verificada con un proyecto desechable; no es bloqueo.
- Android Build Support y toolchain bundled están verificados mediante smoke real. La inspección inicial de ruta fue corregida en `PHASE_03_PREFLIGHT.md`.
- Licencia/titularidad del producto y terceros: H-001, bloquea incorporación de dependencias/assets dudosos.
- Bundle ID/company definitivos requieren decisión humana antes de stores; los placeholders son no publicables.
- Xcode y módulo iOS no disponibles localmente; no bloquearon F09, sí futura evidencia iOS.
- No había dispositivo Android conectado en Prompt 11; instalación, cambio de locale, I/O real, pause/force-stop y falta de espacio permanecen `NOT RUN` para Gate C/F34–F35.
- Traducción/revisión lingüística final y voces ES/EN requieren responsables humanos antes de contenido Release; pseudo solo reduce riesgo de layout.
- GitHub `origin` existe localmente; titularidad/visibilidad, branch protection, checks, runner Unity y activación CI requieren verificación humana antes de push o de atribuir evidencia remota.
- Especialista factual, protocolo de playtests, territorios, modelo comercial y stores siguen pendientes; no son bloqueos para crear una foundation vacía.

## Reanudación inmediata

Ejecutar el preflight de Prompt 12 desde [`prompts/00_MASTER_CODEX_PROMPTS.md`](prompts/00_MASTER_CODEX_PROMPTS.md), contrastar Git/tests y leer `17_LOCALIZATION`/`16_AUDIO`. No hardcodear copy, persistir pseudo, duplicar AppConfig en Save, habilitar flags Release, añadir remote config, cambiar schema sin migración/tests, producir narración final, escalar contenido, hacer push/signing/publicación ni instalar SDKs.
