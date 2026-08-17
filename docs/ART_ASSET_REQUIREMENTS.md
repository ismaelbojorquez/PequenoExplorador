# Requisitos de assets visuales

Los assets sonoros no se gobiernan aquí: IDs, formatos, locales, emoción, licencia y estado viven en [`AUDIO_REQUIREMENTS.md`](AUDIO_REQUIREMENTS.md). Ningún WAV `PH_` de Prompt 12 cuenta como arte/audio final del Vertical Slice.

Contrato inicial para el MVP Selva. No autoriza comprar, descargar ni generar assets y no fija todavía budgets que deban medirse en F12.

## Dirección y alcance

- Estilo 2.5D/3D amable, formas legibles, siluetas distintas y expresiones positivas; evitar realismo inquietante, violencia y estereotipos culturales.
- Un solo bioma: Selva. Personajes, flora, props, fondos, VFX y UI deben servir al loop definido; variantes meramente decorativas no justifican ampliar scope.
- Legibilidad prioritaria en teléfono landscape, tablet y pantallas con safe areas; interacción no debe depender solo de color.
- Diseñar para 4–9: targets grandes, contraste suficiente, poco ruido visual y estados normal/seleccionado/correcto/reintento inequívocos.

## Entregables por asset

Cada asset deberá registrar: ID estable, nombre descriptivo, autor/proveedor, licencia/contrato, fecha, source file, archivo de runtime, actividad propietaria, escala/pivot, materiales/texturas, LOD si aplica y estado de revisión pedagógica/cultural.

| Tipo | Fuente maestra preferida | Runtime candidato | Requisitos |
|---|---|---|---|
| Ilustración/UI | PSD/PSB o vector autorizado | Sprite atlas/PNG según prueba | Capas organizadas, alpha limpio, sin texto rasterizado salvo excepción. |
| Modelo 3D | FBX exportado + fuente DCC | Mesh Unity | Escala métrica, transforms limpios, pivots acordados, materiales limitados. |
| Textura | PSD/TIF/PNG fuente | ASTC/ETC2 según device matrix | Potencias/atlases cuando aporten; readable desactivado salvo necesidad medida. |
| Animación | Archivo DCC/FBX | Clips Unity | Nombres semánticos, loops sin salto, root motion definido. |
| VFX | Grafos/texturas propios | Sistema aprobado en F14 | Sin flashes intensos; motion reducible y overdraw medido. |
| Fuente | OTF/TTF con licencia y cobertura | TMP font asset | Español y glifos de localización; fallback documentado. |

## Ledger temporal Prompt 16

| ID/path | Tipo | Estado | Reemplazo humano requerido |
|---|---|---|---|
| `PH_Explorer.prefab` | primitives Unity, cuerpo/cabeza/mochila | `Placeholder / ReleaseBlocked` | Modelo inclusivo aprobado, escala/pivot, rig, materiales y provenance. |
| `PH_Explorer_Body.mat`, `PH_Explorer_Accent.mat` | materiales URP propios | `Placeholder / ReleaseBlocked` | Paleta final, contraste, revisión cultural/accesible y budgets. |
| `PH_Jungle_Ground.mat` + geometría de escena | suelo/límites/árboles primitivos | `Placeholder / ReleaseBlocked` | Claro Selva final, paths legibles, colisiones y occlusion/budgets. |
| `PH_Destination_Valid.mat`, `PH_Destination_Invalid.mat` | markers locales | `Placeholder / ReleaseBlocked` | Indicador accesible con forma/movimiento/audio, no dependiente solo de color. |
| bob procedural | animación técnica sin root motion | `Placeholder / ReleaseBlocked` | Idle/walk/turn finales, rig y decisión explícita root motion tras profiling. |

Estos assets fueron creados dentro del proyecto, sin descarga ni licencia externa. El `NavMeshData` es build data técnico binario, no arte y se regenera con el setup controlado.

## Ledger temporal Prompt 17

| ID/path | Tipo | Estado | Reemplazo humano requerido |
|---|---|---|---|
| `visual.discovery.jungle.keel-billed-toucan` / `interaction.jungle.keel-billed-toucan` | prefab propio `VS_ToucanPicoCanoa` | `Approved H-008/H-009` | Conservar silueta, collider/punto, bounds y provenance; cambio material exige nueva revisión. |
| `PH_FIXTURE_PLANT` / `interaction.fixture.plant` | cilindro/material local | `Placeholder / ReleaseBlocked` | Planta aprobada y factual, escala/contraste/collider/punto. |
| `PH_FIXTURE_OBJECT` / `interaction.fixture.object` | cubo/material local | `Placeholder / ReleaseBlocked` | Prop final, razón de disponibilidad y feedback accesible. |
| `PH_UI_INTERACTION_PROMPT` + indicador | uGUI/primitives locales | `Placeholder / ReleaseBlocked` | Iconografía sin lectura/color único, foco/acción/cancel y revisión en safe areas. |

Los tres fixtures comparten el mismo núcleo y no representan especies/hechos. Materiales, geometría e icono se crearon dentro del proyecto; no se descargaron assets ni se asumió licencia externa. Copy y cues también siguen sujetos a revisión humana.

## Revisión visual VS-D-A01

El [expediente factual](VS_D_A01_TOUCAN_FACTUAL_DOSSIER.md) documenta un pico verde/naranja/rojo/azul, pero no autoriza copiar fotografías ni fija sexo, edad o subespecie. H-008 aprobó Art/Rights/QA del asset y H-009 su fidelidad factual humana; cualquier cambio material de silhouette, pico, coloración, escala o PhotoTarget exige nueva revisión.

Ismael Bojórquez declaró `Arte: APPROVED`, `Rights: APPROVED` y referencia `Propia` en `H-007-IB-2026-08-16`. Después revisó el asset no-`PH_` concreto y sus seis renders y cerró Art, Rights y QA visual mediante `H-008-IB-2026-08-16`, con cambios solicitados: ninguno.

El incremento visual crea `visual.discovery.jungle.keel-billed-toucan` como obra propia reproducible con primitives Unity y siete materiales URP compartidos. El prefab `VS_ToucanPicoCanoa.prefab` tiene root estable, `VisualRoot`, trigger amplio, `VS_InteractionPoint`, `VS_PhotoAnchor` y bounds candidatos sin dependencia de fotografía. Estado real: visual/factual humano `APPROVED`, `EditorialState.Approved`, `isPlaceholder=false`; H-009 acepta fidelidad factual y que bounds/collider no comunican tamaño zoológico exacto.

| Entregable | Estado / evidencia |
|---|---|
| Prefab runtime candidato | `Assets/_Game/Content/Discoveries/Jungle/KeelBilledToucan/VS_ToucanPicoCanoa.prefab` |
| Materiales | `Toucan_Dark`, `Toucan_Yellow`, `Toucan_BillGreen`, `Toucan_BillOrange`, `Toucan_BillRed`, `Toucan_BillBlue`, `Toucan_EyeWhite`; shared + instancing |
| Geometría medida | 16 meshes/renderers; 4,931 vértices; 7,132 triángulos; bounds `2.973 × 2.425 × 1.100` |
| Memoria aproximada | 89,484 bytes por `Profiler.GetRuntimeMemorySizeLong` sobre meshes/materiales distintos en Editor; no equivale a peak de dispositivo |
| Provenance | JSON versionado con autor, tooling, licencia declarada, hashes y `externalMedia=false` |
| Renders de review | `artifacts/review/toucan/`: frontal, lateral, tres cuartos, Jungle 20:9 y siluetas clara/oscura |
| Aprobación visual | `H-008-IB-2026-08-16`: Art, Rights y QA del asset concreto `APPROVED`; commit revisado `427c09b0b48b0b8ec7348971db5eddbafc5d3707` |
| Aprobación factual | `H-009-IB-2026-08-16`: revisión humana por Ismael Bojórquez — Investigador; competencia declarada en búsqueda ampliada, sin atribuir credencial ornitológica |
| Pendiente | Animación/audio final, iconografía/card/viewfinder final, validación ornitológica externa recomendada y profiling Android físico |

Cambiar especie/subespecie devuelve a revisión todo el modelo/ilustración, collider/bounds fotográficos, icono y thumbnail canónico. Cambiar rasgo visual o paleta devuelve esos assets y cualquier actividad de reconocimiento a `Reviewed`; no invalida dieta/sonido salvo que también cambie la especie.

## Ledger temporal Prompt 19

| ID/path | Tipo | Estado | Reemplazo humano requerido |
|---|---|---|---|
| `PH_UI_PHOTOGRAPHY` / `PH_ASSIST_RETICLE` | uGUI, retícula/flash/guía/shutter | `Placeholder / ReleaseBlocked` | Iconografía final no dependiente de color, revisión 4:3–20:9/tablet y reduce motion. |
| `PH_DISCOVERY_CARD` | tarjeta localizada de resultado | `Placeholder / ReleaseBlocked` | Card/imagen canónica final, lectura prelectora y estados thumbnail/fallback. |
| thumbnail `384×216` local | captura virtual del render Approved | Runtime derivado, privado/local | QA de encuadre/compresión/legibilidad y budgets en Android; no requiere licencia nueva mientras solo renderice assets propios/aprobados. |

`VS_PhotoAnchor` y `CandidatePhotoBounds` aprobados se usan como authoring del `PhotoTarget`; no comunican medida zoológica. Cambiar bounds/anchor/visual de manera material reabre H-008/H-009 según el expediente. No se añadió media externa.

## Ledger temporal Prompt 20

| ID/path | Tipo | Estado | Reemplazo humano requerido |
|---|---|---|---|
| `PH_UI_ALBUM` | Canvas uGUI, grid/categorías/detalle/estados | `Placeholder / ReleaseBlocked` | Componentes de design system/TMP, iconografía final, contraste y revisión infantil en hardware. |
| `PH_ALBUM_ENTRY_*` | ocho cards reutilizables | `Placeholder / ReleaseBlocked` | Silueta locked no reveladora, imagen canónica e identidad visual final por categoría. |
| fallback cromático de ficha | `Image` sin sprite | `Placeholder / ReleaseBlocked` | Ilustración canónica Approved; no debe copiar la mejor foto ni revelar Draft. |
| mejor foto local | Sprite runtime desde PNG `384×216` | Derivado local, no versionado | QA de crop/compresión/legibilidad y profiling en Android; no introduce derechos externos. |

No se añadió fuente, ilustración, icono o media externa. La baseline reutiliza `LegacyRuntime.ttf` y primitives uGUI; la decisión T-006 de TMP queda por materializar dentro del design system de Prompt 27 con fuente licenciada/cobertura ES-EN-pseudo. Cambiar el visual Approved del tucán o su representación canónica reabre el alcance H-008/H-009 aplicable; cambiar solo el chrome `PH_` del álbum exige UI/Accessibility/QA, no aprobación zoológica.

## Ledger temporal Prompt 24

| ID/path | Tipo | Estado | Reemplazo/revisión humana requerida |
|---|---|---|---|
| `PH_Activity_ToucanChooseFood` | authoring de tres tarjetas coral/gris/amarilla | `Sourced`, `Placeholder`, `ReleaseBlocked` | Iconos propios inequívocos fruta/piedra/sombrero, contraste, targets y revisión Child UX/factual asset-specific. |
| `PH_UI_LEARNING` | panel uGUI con watermark, pista/replay/salida | `Placeholder / ReleaseBlocked` | Design system/TMP, no-lector, fuente grande y revisión multiratio física. |
| `learning-reaction.toucan.positive` | salto local suave del visual aprobado | `Placeholder / ReleaseBlocked` | Animación final, lectura positiva, clipping y revalidación si altera anatomía/rasgos. |
| `learning-reaction.toucan.neutral` | giro curioso sin castigo | `Placeholder / ReleaseBlocked` | Animación final neutral y playtest; reduce motion debe conservar estado sin movimiento. |

No se añadió media externa ni se modificó la geometría/material del prefab aprobado. El adapter anima temporalmente su transform y restaura posición/rotación; cualquier cambio visual material reabre H-008/H-009 y [`VS-A01`](VS_A01_TOUCAN_FEEDING_ACTIVITY.md).

## Ledger temporal Prompt 25

| ID lógico | Asset temporal | Estado | Reemplazo/revisión humana necesaria |
|---|---|---|---|
| `visual.camp.observation-table.before` | `PH_CampObservationTable_Before.prefab` | Placeholder propio | Mesa legible, escala infantil, materiales móviles y coherencia con Camp. |
| `visual.camp.observation-corner.after` | `PH_CampObservationCorner_After.prefab` | Placeholder propio | Rincón claramente mejorado sin apariencia de tienda; revisar silueta, props y clipping. |
| `visual.camp.layout` | `PH_CAMP_LAYOUT` + cuatro anchors | Placeholder propio | Level dressing, rutas/espacios táctiles, cámara y variantes de progreso. |
| `visual.ui.camp-hub` | `PH_UI_CAMP_HUB` | Placeholder propio | Migrar al design system/TMP en Prompt 27 y aprobar contraste, jerarquía y ratios. |

No se incorporó media externa. Estos assets son Development-only y el validator Release los bloquea hasta sustitución/aprobación.

## Ledger temporal Prompt 26

| ID lógico | Asset temporal | Estado | Reemplazo/revisión humana necesaria |
|---|---|---|---|
| `customization-slot.skin-tone` | color por `MaterialPropertyBlock` sobre `PH_Head` | Placeholder propio | Cuatro tonos bajo iluminación final, revisión cultural, contraste y shader de piel. |
| `customization-slot.hair` | spheres/capsule para rizos, ondas y dos chonguitos | Placeholder propio | Mallas/peinados inclusivos, hairline, clipping con cabeza/sombreros y rig. |
| `customization-slot.shirt/pants/shoes` | cuerpo/cubos coloreados | Placeholder propio | Prendas, siluetas, materiales, skinning y combinaciones sin clipping. |
| `customization-slot.hat/backpack/explorer-tool` | primitives para sombrero, mochilas, cámara/binoculares | Placeholder propio | Accesorios finales, straps/occlusion, lectura a distancia y compatibilidad con animación/fotografía. |
| `visual.ui.customization` | `PH_UI_CUSTOMIZATION`, swatches y preview de Camp | Placeholder propio | Design system/TMP Prompt 27, iconos, thumbnails, pose/rotación y revisión multiratio/Child UX. |

Son veinte opciones propias, sin media externa, y todas conservan metadata Draft/`PH_`/ReleaseBlocked. El prefab de locomoción no se rehízo: añadió ocho bindings sobre el mismo root. Color usa materiales compartidos; hair/hat/tool alternan roots. Antes de Release hacen falta modelo base inclusivo, arte final por slot, clipping matrix, LOD/batching, iconografía, paleta/contraste, provenance y aprobación asset-specific. Véase [`CUSTOMIZATION_SYSTEM.md`](CUSTOMIZATION_SYSTEM.md).

## Ledger temporal Prompt 27

| ID/path | Tipo | Estado | Reemplazo/revisión humana requerida |
|---|---|---|---|
| `PH_UI_DesignTokens.asset` | paleta, tipografía, spacing, targets y motion | Placeholder propio / ReleaseBlocked | Aprobación visual/accesibilidad con arte final y playtest 4–9. |
| `PH_RoundedRect.png` | sprite 64×64 generado localmente, nine-slice | Placeholder propio / ReleaseBlocked | Shape language/atlas final y profiling de batching. |
| `PH_UI_ComponentGallery.prefab` | botones, cards y estados TMP | Placeholder propio / ReleaseBlocked | Estados completos, iconografía final y QA ES/EN/pseudo/1.25. |
| `PH_UI_ICON` | iconos geométricos code-native | Placeholder propio / ReleaseBlocked | Set ilustrado final, consistencia cultural y reconocimiento prelector. |
| `LiberationSans SDF` | recurso oficial TMP provisional, OFL incluida | Provisional | Familia final licenciada, cobertura, hinting/legibilidad y atribución aplicable. |

No se incorporó media de internet ni se copió IP. Gaps abiertos: iconos/ilustraciones finales, siluetas locked, fondos/ornamentos, cards de discovery/actividad/misión, thumbnails cosméticos, animación final y revisión en hardware. El bridge de texto no cambia derechos de contenido.

## Git y organización futura

- Fuentes grandes de formatos cubiertos irán a Git LFS; `.meta` siempre en Git normal.
- No almacenar exports duplicados, caches DCC, paquetes descargados ni archivos sin licencia.
- Prompt 14 fija authoring bajo `Assets/_Game/Content/Data/Definitions`, `VisualAssetId` semántico y referencia local validada. GUID/path Unity siguen metadata, no identidad de negocio. Presets y límites numéricos esperan medición de vertical slice.
- Addressables será local-first: ningún asset del MVP dependerá de descarga de red.

## Criterios de aceptación

- Derechos y procedencia comprobables; ningún path personal ni secreto embebido.
- Import limpio, ausencia de warnings y dependencia explícita.
- Se ve correctamente en dispositivos/aspect ratios objetivo y dentro del budget aprobado.
- Contenido apto, accesible y revisado; no claims educativos o especies identificadas sin revisión de contenido.
