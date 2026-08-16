# Input táctil, safe area y adaptación de dispositivo

Contrato canónico de Prompt 13. Input System está fijado en `1.20.0`; no se añadió paquete ni API legacy. Esta foundation emite intenciones y adapta pantalla, pero no implementa movimiento, cámara final, fotografía final ni UI de producto.

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
| `Explorer` | Expedition placeholder | tap semántico candidato a destino | joystick, hold/drag como movimiento, pinch |
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

## Haptics y diagnóstico

`IHapticsService` ofrece selección, confirmación y aviso suave, con switch explícito. La única implementación actual es `NoOpHapticsService`, desactivada por defecto y segura en plataformas no soportadas; no invoca vibración. Una implementación física futura exige preferencia adulta persistida, intensidad no invasiva, tests y revisión infantil.

El overlay de toque y viewport existe solo si `DevelopmentDiagnostics` está habilitado; Release tiene cero flags y no habilita `Debug`. Mouse soporta Editor mediante bindings `<Pointer>` sin diseñar UX desktop.

## Validación y límites

`scripts/compile` valida cinco mapas, acciones, asset de thresholds, wiring Bootstrap, un fitter por Canvas, tamaño de targets y ausencia de API legacy/`Touchscreen.current`. EditMode cubre clasificación/thresholds/cancelación/pinch/doble toque/allocations/rotación. PlayMode usa `InputTestFixture` para Back, mapas por escena, multitouch y safe areas.

Requerido antes de Gate C: al menos un Android físico, ambos landscapes, notch/cutout, gestos del sistema, interrupción/background, latencia y ergonomía con manos infantiles. Device Simulator es ayuda visual y no evidencia física. Tap-to-move sigue siendo candidato P-006 hasta el playtest comparativo; esta fase no lo fija.
