# Input táctil, safe area y adaptación de dispositivo

Contrato canónico de input iniciado en Prompt 13 y extendido en Prompt 16/17. Input System está fijado en `1.20.0`; `Explorer` alimenta tap-to-move y el detector contextual con la misma intención semántica, pero cámara, tuning y control siguen pendientes de playtest y no son finales.

## Dirección de dependencias

```text
Content InputActionAsset + GestureThresholdsAsset
                  ↓
Bootstrap compone y selecciona contexto
                  ↓
Infrastructure UnityInputService / UnitySafeAreaService / NoOpHaptics
                  ↓ intenciones y snapshots
Application IInputService / ISafeAreaService / IHapticsService
                  ↓
Presentation SafeAreaFitter / pausa Back / diagnóstico Development
```

Solo `UnityInputService` consulta Input System. Ningún componente de feature puede usar `UnityEngine.Input`, `Touchscreen.current` ni lectura global de dispositivos. `GestureRecognizer` es C# puro, usa hasta cinco slots preasignados y recibe tiempo/posición explícitos para tests.

## Mapas y gestos

| Mapa | Estado/owner | Gestos autorizados | Prohibición |
|---|---|---|---|
| `UI` | Camp, transición y pausa | tap, hold y drag de scroll | pinch y navegación de mundo |
| `Explorer` | Expedition Selva `PH_` | tap semántico a suelo válido | joystick, hold/drag como movimiento, pinch |
| `Photography` | futuro viewfinder | tap, drag y pinch | multitouch fuera del encuadre |
| `Parents` | futura área adulta | tap y drag de scroll; Back sale | shortcuts infantiles/bypass |
| `Debug` | aditivo Development, tecla F8 en Editor | toggle del overlay | ser mapa de producto o habilitarse en Release |

Todos los mapas de producto incluyen `Point`, `PrimaryPress` y `Back`. Android Back y Escape Editor producen una intención única. Back abre una pausa reversible, solicita checkpoint y un segundo Back o `Continuar` restaura el mapa previo; nunca sale de forma destructiva.

Thresholds locales actuales: tap máximo `0.35 s`, hold mínimo `0.65 s`, tolerancia tap `24 px`, inicio drag `32 px`, pinch delta `10 px`. Son authoring mínimo, no tuning final. El segundo contacto suprime taps de ambos dedos para evitar doble activación; pinch solo se clasifica en `Photography`/`Debug`.

## Safe area y ratios

`UnitySafeAreaService` es el único lector de `Screen.safeArea`, tamaño y orientación. Publica insets normalizados; cada Canvas posee exactamente un `SafeAreaFitter` sobre un único root `Safe Area`, sin offsets acumulativos. El validator exige targets actuales de al menos `64×64` píxeles de referencia.

| Preset | Viewport | Insets simulados | Estado automatizado |
|---|---:|---:|---|
| tablet 4:3 | 2048×1536 | 0 | EditMode + PlayMode |
| teléfono 16:9 | 1920×1080 | 0 | EditMode + PlayMode |
| teléfono 20:9 | 2400×1080 | 80 px laterales | EditMode + PlayMode |
| tablet 16:10 | 2560×1600 | 0 | EditMode + PlayMode |

Se prueban `LandscapeLeft` y `LandscapeRight` sobre el modelo. Esto verifica anchors y contratos, no sustituye Device Simulator visual ni hardware con notch/cutout/gestos del fabricante.

Prompt 20 reutiliza este servicio en un Canvas separado de álbum. PlayMode fuerza `1024×768`, `1280×720`, `1600×720` y `1280×800`, comprueba todos los botones activos/inactivos en `≥64×64` unidades lógicas y best-fit del copy visible ES/EN/pseudo. La prueba no certifica densidad, cutout o legibilidad física: Android real y fuente grande humana siguen pendientes antes de Gate C.

Prompt 26 añade un Canvas seguro de personalización y prueba botones/slots/opciones en `1024×768`, `1920×1080` y `2400×1080`, todos `≥64×64`. No presenta categorías de género: los ocho slots se seleccionan directamente y las opciones usan texto+swatch. La validación automatizada no certifica discriminación cromática, tono de piel bajo iluminación final, clipping ni ergonomía física.

## Haptics y diagnóstico

`IHapticsService` ofrece selección, confirmación y aviso suave, con switch explícito. La única implementación actual es `NoOpHapticsService`, desactivada por defecto y segura en plataformas no soportadas; no invoca vibración. Una implementación física futura exige preferencia adulta persistida, intensidad no invasiva, tests y revisión infantil.

El overlay de toque y viewport existe solo si `DevelopmentDiagnostics` está habilitado; Release tiene cero flags y no habilita `Debug`. Mouse soporta Editor mediante bindings `<Pointer>` sin diseñar UX desktop.

## Validación y límites

`scripts/compile` valida cinco mapas, acciones, asset de thresholds, wiring Bootstrap, un fitter por Canvas, tamaño de targets, AI Navigation `2.0.9`, root/prefab/NavMesh y ausencia de API legacy/`Touchscreen.current`. EditMode cubre clasificación/thresholds/cancelación/pinch/doble toque/allocations/rotación y estados/comandos de locomoción. PlayMode usa `InputTestFixture` para Back, mapas, tap real, multitouch, safe areas y ciclos Selva.

## Locomoción candidata Prompt 16

`ExplorerLocomotionController` es Application/BCL y recibe `IPathNavigator`; no conoce Unity. Presentation adapta raycast, `NavMeshAgent`, marker, animación procedural y cámara. Bootstrap encuentra una única raíz al completar la carga aditiva y la enlaza de forma explícita; no hay lookup por frame, singleton ni service locator.

| Parámetro `PH_` | Valor actual | Propósito |
|---|---:|---|
| Velocidad | `2.4 m/s` | Recorrido corto sin exigir sostener un control. |
| Aceleración | `8 m/s²` | Arranque legible sin respuesta brusca. |
| Giro | `420°/s` | Seguir path compacto; requiere observarse en hardware. |
| Radio/altura | `0.35 m` / `1.65 m` | Placeholder y claros del stub. |
| Stop / sample | `0.18 m` / `0.75 m` | Llegada estable y tolerancia del tap. |
| Cámara | offset `(0, 7.5, -6.5)`, damping `0.22 s` | Seguimiento automático; bounds `x[-8,8]`, `z[-9,7]`. |

Un tap nuevo reemplaza destino; tap inválido muestra marker cálido sin castigo y deja recuperar con el siguiente. Cambiar a `UI`/`Photography`, pause/focus o unload cancela path. `SetReduceMotion(true)` elimina bob y snappea cámara; la preferencia adulta aún no está conectada a Save.

Prompt 17 da prioridad al detector contextual sobre el suelo: si el rayo selecciona un target, el coordinador enfoca y solicita approach; si no, el mismo tap continúa como destino de suelo. El detector usa `RaycastNonAlloc` e índice collider→target construido al bind. UI/pause/unload suspenden tanto focus como path, y el botón cancelar limpia ambos. Contrato: [`INTERACTION_SYSTEM.md`](INTERACTION_SYSTEM.md).

Requerido antes de Gate C: al menos un Android físico, ambos landscapes, notch/cutout, gestos del sistema, interrupción/background, latencia, FPS/allocations y ergonomía con manos infantiles. Device Simulator y FPS Editor batch son diagnóstico, no evidencia física. Tap-to-move sigue candidato P-006 hasta el playtest comparativo.
