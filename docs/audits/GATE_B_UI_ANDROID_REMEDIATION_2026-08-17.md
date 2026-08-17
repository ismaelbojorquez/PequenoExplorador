# REMEDIATION: BLOCKED — UI/lifecycle Android de Gate B

- Fecha: 2026-08-17 (`America/Mexico_City`).
- Entrada: `main@8cdd0bdc51a0b5ffce9227b3324a5a07856e6bda`, árbol limpio.
- Commit técnico: `a4238c73a21eeca7d0a2572015a9f7ab93205f11` (`fix(ui): enforce app state presentation lifecycle`).
- Auditoría que origina el trabajo: [`GATE_B_2026-08-17_PHYSICAL_AND_CHILD_UX.md`](GATE_B_2026-08-17_PHYSICAL_AND_CHILD_UX.md), `GATE B: FAIL`.
- Alcance: remediación técnica y evidencia; **no** es una nueva auditoría ni cambia el veredicto Gate B.

## 1. Resultado

La remediación de composición/lifecycle está implementada, compilada y cubierta por regresiones automatizadas. El código aplica ownership explícito y fail-closed a 13 superficies en 15 estados; diagnostics y controles de mutación quedan cerrados por defecto; Camp→Expedition→Camp, Back, teardown, ratios y recuperación de surface pasan en PlayMode.

El cierre físico está **BLOCKED**: el HONOR DNY-NX9 dejó de aparecer en `adb devices -l` antes de instalar el APK remediado. Por ello no se afirma que el defecto visual, touch o de rotación esté resuelto en Android real. El playtest infantil/no lector es `NOT RUN`. Gate B conserva su `FAIL` vigente y Prompt 31 continúa bloqueado.

## 2. Hallazgos y correcciones

| ID | Severidad original | Corrección técnica | Evidencia actual | Estado |
|---|---|---|---|---|
| GB3-001/004 | Major | `AppUiState`, coordinator de roots, alpha/interacción/raycast fail-closed, sorting explícito y diagnostics opt-in. | EditMode/PlayMode y validator pasan. | `PASS automatizado`; hardware `NOT RUN`. |
| GB3-002 | Major | Scene flow aplica `Transition`, `Camp` y `Expedition`; roots incompatibles se desactivan. | Prueba sobre escena real Camp→Expedition→Camp. | `PASS automatizado`; journey físico `NOT RUN`. |
| GB3-003 | Major | Adapter coalescido de safe area/surface/cámara en resize, orientación y focus. | Ratios y resume simulados pasan; espera compatible con batchmode. | `PASS automatizado`; rotación física `NOT RUN`. |
| Carrera teardown | Major potencial | Guards de shutdown y eventos edge-triggered evitan notificaciones tardías/overwrite de Transition. | Cinco fallos intermedios reproducidos, corregidos y suite repetida. | `PASS automatizado`. |
| GB3-005 | Major | No es corregible por código. | Sin facilitador/consentimiento/participante. | `NOT RUN`. |

No se añadió contenido, SDK, red, permiso, monetización ni bypass de Release. No se borró ni restauró progreso físico.

## 3. Arquitectura aplicada

`Application` define `AppUiState`, mapa semántico de input y acción Back. `Presentation` define la policy exhaustiva `estado → superficies/roles/sorting` y aplica visibilidad/raycast mediante `UiCompositionCoordinator`. `Bootstrap` traduce outcomes de scene flow/features a estados y es el único wiring owner. `SurfaceLifecycleAdapter` recupera layout, safe area y cámara tras cambios de surface/focus sin reiniciar la Activity.

Cada root se referencia de forma serializada; no hay polling ni lookup por nombre en runtime. Un root no autorizado queda con `alpha=0`, `interactable=false`, `blocksRaycasts=false` y `GraphicRaycaster` desactivado.

## 4. Matriz AppState → presentación

| Estado | Primario | Overlay permitido | Input | Back |
|---|---|---|---|---|
| Boot | Status | ninguno | UI | Ignore |
| Transition | SceneFlow | ninguno | UI | Ignore |
| Camp | Camp | Tutorial | UI | OpenPause |
| Expedition | mundo 3D | Interaction, Tutorial | Explorer | OpenPause |
| Interaction | mundo 3D | Interaction, Tutorial | Explorer | OpenPause |
| LearningActivity | Learning | Tutorial | UI | CloseSurface |
| Photography | Photography | Tutorial | Photography | CloseSurface |
| DiscoveryResult | Photography/card | Tutorial | Photography | CloseSurface |
| Album | Album | Tutorial | UI | CloseSurface |
| Missions | Missions | ninguno | UI | CloseSurface |
| CampUpgrade | Camp/preview | Tutorial | UI | CloseSurface |
| Customization | Customization | Tutorial | UI | CloseSurface |
| Pause | InputFoundation/pause | ninguno | UI | Resume |
| ErrorRecovery | Status | ninguno | UI | RetryOrStay |
| DevelopmentDiagnostics | InputFoundation/diagnostics | ninguno de producto | UI + acceso deliberado | CloseDiagnostics |

`Economy`, `AudioDiagnostics` y cualquier superficie no listada permanecen no raycastables. Tutorial solo aparece cuando está solicitado y nunca en Boot, Transition, Pause, Error o Diagnostics.

## 5. Validación automatizada

| Comando/caso | Resultado | Evidencia |
|---|---|---|
| `scripts/validate` | `PASS` | Repository/compile/validadores, Addressables local, EditMode, PlayMode y APK. |
| Addressables | `PASS` | 61 locations, 1,920,120 bytes, local-only, sin endpoint remoto. |
| EditMode | `PASS 172/172` | Policy exhaustiva, máximo un primario, input/Back y fail-closed diagnostics. |
| PlayMode | `PASS 31/31` | 28.333 s; composición, escena real, resize/resume, teardown y journey automatizado. |
| `scripts/check-repository` / `git diff --check` | `PASS` | Markdown/JSON/workflow/secret scan y whitespace. |

Los resultados automatizados demuestran contrato y regresión; no sustituyen framebuffer, hit-testing, cutout, lifecycle del OEM ni comprensión infantil.

## 6. APK candidato

| Campo | Valor |
|---|---|
| Archivo retenido | `artifacts/device-gate-b-remediation/PequenoExplorador-a4238c73-development.apk` |
| SHA-256 | `c3492324b77d91ebc062d5ad01dd14b4296c3a685bc9382d9a80b160a8db8adf` |
| Tamaño | 67,454,896 bytes |
| Commit embebido | `a4238c73a21eeca7d0a2572015a9f7ab93205f11` |
| Unity/perfil | `6000.3.22f1`, Development, locale inicial ES |
| Android | min 26, target 36, IL2CPP, ARM64, APK, sin signing externo |
| 16 KB | `PASS estático`: zipalign y LOAD ELF de siete `.so` a 16384 |
| Permisos | INTERNET + receiver interno; sin CAMERA/mic/location/contacts/storage/AD_ID/BILLING |

El candidato no fue instalado. El hash identifica lo que debe instalarse; no es evidencia física.

## 7. Matriz Android física

| Caso | Resultado | Bloqueo/límite |
|---|---|---|
| Respaldo save e instalación/hash extraído | `NOT RUN` | `adb devices -l` sin dispositivo. |
| Cinco force-stop/restart | `NOT RUN` | Candidato no instalado. |
| Journey completo | `BLOCKED` | Dispositivo desconectado; save limpio requiere autorización expresa para reset/restauración. |
| Segunda sesión/idempotencia | `NOT RUN` | Requiere primer journey válido. |
| Offline | `NOT RUN` | Dispositivo desconectado. |
| Back/background/lock | `NOT RUN` | Dispositivo desconectado. |
| LandscapeLeft↔Right | `NOT RUN` | Dispositivo desconectado. |
| ES/EN, audio, subtítulos, reduce motion | `NOT RUN` | Dispositivo desconectado y observación humana requerida. |
| Safe area/cutout/touch | `NOT RUN` | Dispositivo desconectado. |
| Persistencia/logs/crash/performance | `NOT RUN` | Candidato no instalado. |

La evidencia ignorada reproducible está en `artifacts/device-gate-b-remediation/REMEDIATION_EVIDENCE.md`. No se heredan los PASS del APK anterior `5c382e…` para este candidato.

## 8. Playtest infantil/no lector

`NOT RUN`. No existe consentimiento, facilitador, participante ni matriz agregada para esta remediación. El playtest debe seguir [`PLAYTEST_PLAN.md`](../PLAYTEST_PLAN.md) después de superar la matriz técnica física; Codex no puede atribuirse observación humana.

## 9. Riesgo residual y siguiente acción

Permanecen abiertos R-011, R-019, R-034, R-041, R-050 y R-054. Para desbloquear:

1. conectar/desbloquear el HONOR y autorizar ADB;
2. respaldar read-only el save y obtener autorización explícita si se necesita reset/restauración;
3. instalar y extraer exactamente el APK `c349232…` para comparar hash;
4. ejecutar cinco arranques y la matriz completa, conservando logs/capturas/video minimizados;
5. ejecutar el playtest consentido solo si el flujo físico es operable;
6. solicitar una reauditoría independiente que decida Gate B.

Hasta entonces: **Gate B = FAIL vigente, remediación = BLOCKED, Prompt 31 = no autorizado**.
