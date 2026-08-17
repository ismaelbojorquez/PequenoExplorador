# GATE B: FAIL — auditoría física y UX infantil

- Fecha: 2026-08-17 (`America/Mexico_City`).
- Commit de entrada: `71f942fbff9e27e249652ea6af850fd33e01d675` (`fix(android): repair bootstrap scene serialization`).
- Rama/entrada: `main`, árbol limpio, 31 commits delante de `origin/main`.
- APK evaluado: `67,444,690` bytes, SHA-256 `5c382e6c3340f569350ef9ee765566fd0f0377d9403b847df5ae411c33253b80`.
- Dispositivo: HONOR DNY-NX9, Android 16, `2736×1264`, 560 dpi, cutout lateral 138 px.
- Alcance: Gate B del Vertical Slice Development; no es aprobación de Release, científica, legal ni de tienda.

## 1. Resultado

**GATE B: FAIL.** El crash posterior al splash permanece corregido y diez arranques físicos llegan a Camp, pero el journey no es jugable en hardware: Canvas/paneles de Camp, misión, economía, upgrade, audio, tutorial, diagnóstico y navegación permanecen simultáneamente visibles, se superponen y compiten por hit-testing. La transición a Expedición ocurre en logs, pero la vista sigue cubierta por Bootstrap/diagnósticos y no permite observar/mover/interactuar con Selva. La rotación en caliente a la orientación landscape opuesta deja la superficie negra durante al menos 12 s y no se recupera sin reiniciar la Activity.

No se ejecutó playtest infantil/no lector. El fallo físico ya impide reclutar responsablemente participantes: primero debe existir un flujo operable sin coaching ni controles ambiguos.

## 2. Preflight y evidencia de entrada

Se leyeron completos `AGENTS.md`, `STATUS`, la auditoría Gate B vigente, la adenda del crash, `PLAYTEST_PLAN` y testing. Git estaba limpio y el APK local inicial tenía el hash exigido. El APK extraído de `/data/app/.../base.apk` es byte por byte idéntico al evaluado (`cmp` 0 y mismo SHA-256); se preservó una copia ignorada en `artifacts/device-gate-b/evaluated-5c382e6c.apk`.

La evidencia entregada contenía cinco logs de startup, `journey-final.log`, `meminfo.txt` y `package.txt`, pero ninguna captura/video ni registro consentido de playtest. `journey-final.log` solo contiene Bootstrap→Camp: no registra Camp→Selva, actividad, fotografía, discovery, reward, misión, álbum o mejora.

## 3. Matriz de hallazgos

| ID | Severidad | Hallazgo | Estado / evidencia |
|---|---|---|---|
| GB3-001 | **Major** | UI runtime no mantiene exclusión visual por estado. Mission, Economy, Camp Upgrade, Audio Diagnostic, Tutorial, viewport Development y navegación se superponen; texto queda cortado y varios botones cubren la misma zona. | **OPEN / FAIL.** Reproducido en cinco capturas idénticas SHA-256 `299d48…` y tras reinicio en LandscapeRight `260966…`. La escena conserva múltiples roots activos; no es un defecto pequeño aislable en auditoría. |
| GB3-002 | **Major** | La transición Camp→Expedition completa en logs, pero Selva queda oculta por Bootstrap/diagnósticos; el niño no puede ejecutar move→interact→activity→photo. | **OPEN / FAIL.** `TransitionCompleted detail=Expedition`; captura `tap-expedition-2.png` SHA-256 `5df3b5…` muestra título/overlay/controles, no mundo jugable. |
| GB3-003 | **Major** | Rotación landscape en caliente produce pantalla negra persistente. | **OPEN / FAIL.** `rotation-right-12s.png` queda negra después de 12 s, proceso vivo, cero fatal; restaurar orientación no recupera la superficie. Reiniciar Activity recupera contenido. PlayerSettings declara Left y Right. |
| GB3-004 | **Major** | Hitboxes Development son ambiguos y pueden mutar progreso. El saldo visible cambió 4→6 durante la auditoría sin un grant intencional; el save contiene `economy-tx.debug.8` y `.9`. Un tap destinado al selector de idioma activó “Repetir guía”. | **OPEN / FAIL.** Current save: 6 estrellas, 9 debug grants, tutorial reiniciado a paso 0. No se restauró/borró progreso porque sería destructivo; requiere decisión humana tras la reparación. |
| GB3-005 | **Major** | Playtest consentido 4–9/no lector ausente. | **NOT RUN.** No hay protocolo ejecutado, participantes, matriz agregada ni evidencia bajo `artifacts/device-gate-b/`. No confundir simulación documental con playtest. |
| GB3-006 | Minor | La evidencia nombrada `journey-final.log` es solo otro arranque a Camp. | **OPEN.** Debe reemplazarse por recorrido trazable/capturas una vez corregida la UI. |
| GB3-007 | Minor | Un rebuild del mismo commit produce otro hash APK (`34613b…`) aunque conserva tamaño/configuración. | **OPEN.** El binario evaluado exacto `5c382e…` quedó preservado; documentar reproducibilidad/retención de artefactos antes de Release. |
| GB3-008 | Minor | Snapshot de Camp: PSS 362,196 KiB, RSS 513,416 KiB, Graphics 71,200 KiB; no existe serie journey/FPS/térmica. | **PARTIAL.** No hay budget aprobado para declarar FAIL por memoria, pero requiere profiling tras corregir roots simultáneos. |
| GB3-009 | Info | Crash `level0` no reapareció. | **PASS.** Cinco logs entregados + cinco rechecks independientes: diez PIDs distintos, `ApplicationReady`, Camp y cero fatal/ANR/SIGTRAP/corrupción. |
| GB3-010 | Info | APK Android conserva configuración/permisos acotados. | **PASS Development.** API 26/36, ARM64, zipalign 16 KB; solo INTERNET + permiso receiver interno, sin CAMERA/micrófono/ubicación/storage/AD_ID/BILLING. |

## 4. Matriz requerida

| Caso | Resultado | Evidencia / límite |
|---|---|---|
| APK exacto `5c382e…` | `PASS` | Local, instalado y extraído son byte-idénticos. |
| Cinco force-stop/restart entregados | `PASS` | 5/5 Ready→Camp, cero fatal. |
| Cinco force-stop/restart independientes | `PASS` | 5/5 Ready→Camp, cero fatal; capturas idénticas confirman superposición persistente. |
| Camp→Selva | `PARTIAL/FAIL` | Transición técnica completa, presentación de Selva bloqueada/oculta. |
| Move→interact→activity→photo | `NOT RUN` | Imposible desde UI física observada. |
| Discovery→stars→mission→album→upgrade→Camp | `NOT RUN` | Imposible recorrer; estado existente proviene de debug/automatización y no sirve como journey humano. |
| Segunda sesión/idempotencia física | `NOT RUN` | Primer journey no es ejecutable. Automatización local sí pasa, pero no sustituye hardware. |
| Offline | `PARTIAL PASS` | Wi‑Fi/datos desactivados y restaurados; boot→Camp pasa. Journey offline `NOT RUN`. |
| Back | `PARTIAL PASS` | Desde Expedición abre “Pausa tranquila”; continuar funciona. Resto de pantallas no alcanzable. |
| Background/resume | `PARTIAL PASS` | PID/superficie sobreviven Home→resume; journey no alcanzable. |
| Rotación left/right | `FAIL` | Switch en caliente deja superficie negra; requiere Activity restart. |
| Español/inglés | `FAIL/NOT RUN` | Español visible; selector solapado impide cambio confiable y el tap activa Tutorial. Inglés físico no demostrado. |
| Audio/subtítulos/reduce motion | `NOT RUN` | Panel audio visible/superpuesto, pero no hubo evaluación auditiva ni flujo alcanzable. |
| Safe area/touch | `FAIL` | Cutout conocido; texto, controles y hitboxes se solapan masivamente. |
| Persistencia | `PARTIAL` | Cinco arranques cargan save v12; feature journey no demostrado. Inputs ambiguos añadieron grants debug y reiniciaron tutorial. |
| Permisos/manifest | `PASS Development` | Sin permisos sensibles inesperados; `INTERNET` solo Development. |
| Logs/crash | `PASS` | Sin error Unity/fatal/ANR del paquete en startups/rechecks. |
| Playtest infantil/no lector | `NOT RUN` | No existe evidencia consentida. |

## 5. Suite técnica y build

`scripts/validate` sobre HEAD terminó `PE_FULL_VALIDATION_OK`: repository/compile/validadores, Addressables `4.0.1` local (61 locations, 15 archivos, 1,920,120 bytes, sin catálogo remoto), EditMode `169/169` en 5.479 s, PlayMode `29/29` en 28.055 s y APK Development.

Ese rebuild generó SHA-256 `34613b8c019174fbfe18329754782b41720d895e540c2872c2eb9ed8b5520b73`, 67,444,690 bytes, en 20.107 s. No sustituye el APK auditado: la matriz física corresponde exactamente a `5c382e6c3340f569350ef9ee765566fd0f0377d9403b847df5ae411c33253b80`.

## 6. Arquitectura y causa observada

La arquitectura de casos de uso/puertos sigue pasando tests; el fallo está en ownership de Presentation/Bootstrap y visibilidad/hit-testing real. Los roots persistentes no implementan una política de exclusión por estado de app/escena. Los tests validan cada pantalla aisladamente y ratios sintéticos, pero no capturan el framebuffer compuesto ni comprueban que solo las superficies autorizadas estén activas/raycastables en Camp, Expedition, Photography, Album, Learning y Pause.

La remediación necesita un owner de UI state/layers, reglas explícitas de visibilidad y raycast, diagnóstico detrás de un toggle que no intercepte producto, prueba framebuffer/hierarchy por estado y rotación física. Esto excede una corrección pequeña/determinista permitida a la auditoría; no se cambió gameplay ni contenido.

## 7. Acción obligatoria antes de repetir Gate B

1. Crear ExecPlan de remediación UI/lifecycle Android.
2. Definir una matriz `AppState → roots visibles/raycastables` y hacerla fail-closed.
3. Ocultar diagnostics/audio/debug grant/locale viewport por defecto; abrirlos solo mediante entrada Development deliberada y no superpuesta.
4. Asegurar que Camp y Expedition ocultan status/roots incompatibles y que Selva es visible/táctil.
5. Corregir recreación/surface lifecycle para ambas orientaciones landscape.
6. Añadir PlayMode/composición que detecte roots simultáneos, hitbox overlap crítico y framebuffer negro.
7. Restablecer el save físico solo con autorización humana; repetir el journey desde estado limpio sin debug grants.
8. Repetir `scripts/validate`, instalar el APK exacto, matriz física completa y luego playtest consentido/no lector.

Prompt 31 y el escalado de contenido permanecen bloqueados. Una nueva auditoría deberá decidir `PASS` o `FAIL`; este `FAIL` no puede convertirse en `CONDITIONAL` solo por mantener verde la suite automatizada.
