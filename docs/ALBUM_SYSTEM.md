# Álbum visual y enciclopedia infantil

Estado: baseline funcional de Prompt 20 para Selva y `VS-D-A01`. Es local/offline, no muta progreso y no amplía el catálogo. La presentación y el audio final siguen siendo placeholders bloqueados para Release.

## Fuente de verdad y flujo

```text
IContentCatalog (solo Approved) ─┐
DiscoveryProgressRepository ────┼─→ AlbumQueryService → AlbumSnapshot/view models → AlbumView
PhotoProgressRepository ────────┘                                      │
IPhotoStore.LoadAsync ← referencia validada del manifest ───────────────┘
```

`AlbumQueryService` vive en Application y construye read models a partir del catálogo, progreso y metadata de foto. Presentation no lee `PlayerProgress`, filesystem, `AssetDatabase`, `PlayerPrefs` ni Addressables directamente; tampoco concede discoveries. `LocalPhotoStore` sigue siendo owner del directorio `Photos` y solo entrega bytes de una referencia presente en su manifest y dentro del límite de `512 KiB`.

## Contratos y privacidad de contenido

- `AlbumSnapshot`: mundo, filtro de categoría, categorías, entries y contadores derivados.
- `AlbumCategoryViewModel`: ID, key localizada y conteo `descubiertos/total`.
- `AlbumEntryViewModel`: estado `Locked` o `Discovered`, visual/facts/audio/foto solo cuando corresponde.
- `AlbumFactViewModel`: uno de `Habitat`, `Diet`, `Size`, `Curiosity` o `Sound`, con key Approved o fallback explícito.
- `AlbumEntryMetadata`: referencias authoring a facts y habilitación explícita de audio; no contiene copy ni reglas mutables.

El query incluye exclusivamente discoveries y categorías `Approved`, incluso en Development. Una entry bloqueada conserva su ID técnico internamente para correlación, pero entrega a Presentation nombre/facts/visual/audio/foto vacíos. No revela contenido Draft ni convierte una miniatura ausente en bloqueo.

## Pantallas y estados

| Pantalla/estado | Conducta |
|---|---|
| Acceso Camp | Botón grande `Álbum`; solo disponible en Camp y fuera de transición. |
| Grid | Contadores reales, filtro por categoría y ocho celdas reutilizables por página. |
| Locked | Silueta cromática neutra, `Por descubrir` y pista general sin revelar facts. |
| Discovered | Nombre e imagen; carga la mejor foto local cuando existe. |
| Detalle | Nombre, foto/fallback, hábitat, dieta, tamaño, curiosidad, sonido y replay condicionado. |
| Loading | Mensaje localizado mientras se prepara el snapshot/carga de imagen. |
| Empty | Categoría sin entries elegibles; permite volver. |
| Error | Copy recuperable, sin excepción o ruta técnica. |
| Missing photo | Imagen canónica cromática y progreso intacto. |
| Missing fact | `Este dato está por confirmar`; nunca se inventa copy. |

El tucán tiene habitat/dieta/pico/sonido Approved. No existe claim Approved de tamaño, por lo que ese campo usa fallback. El cue actual es confirmación `PH_`, no vocalización: `HasPlayableAudio=false` y replay queda deshabilitado hasta existir un cue factual, con derechos y revisión.

## Carga, caché y lifecycle

- Cada refresh o cambio de detalle cancela el token anterior y aumenta una generación; un resultado tardío no actualiza otra celda.
- La UI mantiene como máximo ocho fotos decodificadas, acorde con la página visible. La expulsión destruye `Sprite` y `Texture2D`.
- Cerrar, cambiar de escena, cambiar locale, unbind o destruir Bootstrap cancela cargas y libera caché.
- Las celdas son un pool fijo de ocho, no se instancian por scroll/refresh. Con el único discovery actual no se justifica un sistema de virtualización externo.
- `Back` aplica `detalle → grid → cerrar álbum`; luego vuelve al manejo normal de pausa.

## Localización, accesibilidad y audio

Hay baseline ES/EN y pseudo Development para 27 claves nuevas: 26 `ui.album.*` y `content.category.discovery.animals`. Smart Strings forman contadores/página; no se concatenan frases. El cambio de locale vuelve a renderizar sin reiniciar.

El Canvas usa `ScaleWithScreenSize 1920×1080`, safe area única y targets mínimos `64×64`. El harness PlayMode recorre `4:3`, `16:9`, `20:9` y `16:10`, y exige best-fit en copy visible. Color siempre acompaña texto/estado. Esta baseline usa uGUI y `UnityEngine.UI.Text`, coherente con las pantallas heredadas; la migración a TMP y componentes finales pertenece al design system de Prompt 27 y no autoriza añadir fuentes/paquetes externos sin intake.

Replay usa `IAudioService` y solo se habilita cuando Content marca un cue factual Approved. Miniatura, copy y progreso son suficientes sin audio.

## Validación y aceptación

```sh
scripts/validate-album
scripts/test-editmode
scripts/test-playmode
scripts/validate
```

`AlbumValidationService` bloquea metadata incompleta, tamaño inventado, audio placeholder expuesto, wiring distinto de un `AlbumView`, falta de safe area, pool distinto de ocho, targets menores a `64×64` y acceso directo de Presentation a filesystem/AssetDatabase/Resources/ScreenCapture.

EditMode cubre contadores/filtros, Draft oculto, locked sin fuga, missing fact/photo, contenido retirado y lectura/cancelación del photo store. PlayMode cubre navegación Camp↔Álbum, actualización tras captura sin reinicio, detalle/fallback/replay, ES/EN/pseudo, cuatro ratios, Back y cleanup. Android físico sigue requerido para validar gesto, notch, fuente grande, legibilidad y memoria reales antes de Gate C.

## Límites

- No hay scroll masivo, 40 entradas, arte final, narración ni audio de especie final.
- No existe porcentaje, fecha límite, racha, presión por completar o acción comercial.
- El fallback cromático no es la ilustración canónica final; queda `PH_`/ReleaseBlocked.
- Gate B permanece `FAIL` hasta completar Fases 21–29 y repetir la auditoría end-to-end.
