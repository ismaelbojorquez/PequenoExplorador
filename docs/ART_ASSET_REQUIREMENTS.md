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
