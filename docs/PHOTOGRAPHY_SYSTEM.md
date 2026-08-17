# Fotografía ficticia asistida

Estado: Prompt 19 implementado para el único discovery Approved `discovery.jungle.keel-billed-toucan`. Es una cámara virtual dentro del mundo; no abre la cámara física, no comparte archivos y no solicita `android.permission.CAMERA`.

## Flujo y ownership

```text
tap + auto-approach
  → PhotographyInteractionAction
  → PhotographySceneRoot selecciona IPhotographable
  → InputMap.Photography + locomoción suspendida
  → PhotoTargetEvaluator (puro)
  → CapturePhotoUseCase
       1. valida frame y captura ID
       2. ejecuta DiscoverUseCase/idempotencia
       3. renderiza thumbnail acotada
       4. PhotoStore escribe binario/manifest
       5. PlayerProgress guarda solo metadata/referencia
```

Application posee `PhotoTarget`, `PhotoEvaluation`, evaluator, caso de uso y puertos. Presentation calcula viewport/LOS y renderiza; Infrastructure posee filesystem e índice local; Bootstrap compone y liga una sola raíz por expedición. Domain conserva `PhotoProgress`, nunca bytes, texturas, GameObjects ni paths físicos.

El discovery se confirma antes de render/storage: un fallo de miniatura devuelve `CapturedWithoutThumbnail`, conserva el progreso y permite a UI usar la imagen canónica. Solo una captura se procesa por raíz; taps concurrentes se ignoran mientras el shutter está ocupado. Cada captura lleva grant idempotente y una foto de menor o igual score no vuelve a renderizar ni reemplaza la mejor.

## Evaluación accesible

| Señal | Threshold actual | Resultado amable |
|---|---:|---|
| Cobertura de viewport | `≥ 0.08` | Si falta, centrar/acercar. |
| Distancia | `≤ 10 m` Unity | Si excede, `Acércate un poquito`. |
| Línea de visión | requerida | Si hay oclusión, centrar/buscar vista. |
| Offset normalizado al centro | `≤ 0.36` | Retícula ámbar y guía; sin castigo. |
| Alineación absoluta | `≥ 0.35` | Guía de centrado; tolera ambos lados del modelo. |

Los límites pertenecen al `PhotoTarget` authoring validado; no representan tamaño zoológico. La UI `PH_` ofrece `acércate/centra/listo`, shutter de `170×170`, salida de `180×110`, tarjeta localizada y retícula por iconografía/color. `reduce motion` omite flash; locomoción permanece suspendida en cámara y durante pausa/UI. Falta playtest infantil y Android físico antes de fijar tuning.

## Render, archivos y presupuesto

- `UnityPhotoThumbnailRenderer`: RenderTexture temporal `384×216`, `ARGB32`, depth 24; lectura a RGB24 y PNG. Restaura `targetTexture`/`RenderTexture.active` y libera ambos recursos en `finally`.
- No se usa `ScreenCapture`. El contador diagnóstico debe volver a cero tras captura/unload.
- Ubicación lógica: `Application.persistentDataPath/Photos/` mediante `LocalPhotoStore`; ningún path personal se versiona.
- Un archivo determinista por discovery+score: ID seguro con `.` convertido a `_`, score entero y extensión `.png`; índice `photos-index.json`. El manifest cambia atómicamente antes de borrar el archivo anterior y al iniciar se limpian PNG huérfanos.
- Límites: `512 KiB` por PNG, `64` entries y `32 MiB` totales. Temp se limpia al inicializar; escritura usa temp, flush y replace.
- Save schema v6 contiene ID, referencia relativa, score, ancho, alto y bytes. El store de fotos está separado del envelope JSON/backup.

El perfil Editor batch registra `PE_PHOTOGRAPHY_MEMORY` con peak estimado del RT+CPU+PNG y delta global orientativo. No equivale a profiling en dispositivo. Android físico, presión de memoria, falta real de espacio y calidad ASTC/PNG permanecen pendientes.

## Fallos, privacidad y Release

- `NotReady`, `Busy`, `Cancelled`, `Unavailable`, `ExistingPhotoKept`, `CapturedNew`, `CapturedRepeated` y `CapturedWithoutThumbnail` son outcomes explícitos.
- Development puede fallar exactamente el siguiente write; el simulador se compila fuera de Release.
- No se recopila imagen real, rostro, audio, ubicación, identificador infantil ni telemetría. La thumbnail contiene únicamente el render virtual local.
- No hay galería del dispositivo, compartir, red, cloud ni media scanner.
- El validator bloquea target/bounds faltantes, UI incompleta, touch targets pequeños, `ScreenCapture`, permiso CAMERA y cambio no medido del contrato `384×216`.
- La UI/retícula/tarjeta y cues usados son placeholders; bloquean Release aunque el tucán sea Approved. Audio final sigue pendiente de su ledger y derechos.

## Matriz automatizada

EditMode cubre thresholds/límites, oclusión, scoring, ID seguro, best-photo, storage fallback, idempotencia, cancelación y shutter concurrente. PlayMode cubre entrada por interacción, inválido→pista, válido→discovery, spam, pausa/UI, fallo de store, reduce motion, persistencia/repetición y unload sin recursos temporales. El build local valida ausencia de permiso físico y catálogo offline.

Procedimiento Development: interactuar con el tucán, encuadrar hasta `Listo`, capturar y volver a Camp. Para recovery de almacenamiento, conservar el save v6 y borrar solo `persistentDataPath/Photos` desde tooling autorizado; el álbum implementado usa fallback canónico sin perder discovery. `Reset progress` Development borra save y photo store; no está disponible en Release.
