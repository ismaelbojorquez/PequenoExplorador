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
| `PH_FIXTURE_ANIMAL` / `interaction.fixture.animal` | cápsula/material local | `Placeholder / ReleaseBlocked` | Silueta animal aprobada, collider/punto, states de foco y provenance. |
| `PH_FIXTURE_PLANT` / `interaction.fixture.plant` | cilindro/material local | `Placeholder / ReleaseBlocked` | Planta aprobada y factual, escala/contraste/collider/punto. |
| `PH_FIXTURE_OBJECT` / `interaction.fixture.object` | cubo/material local | `Placeholder / ReleaseBlocked` | Prop final, razón de disponibilidad y feedback accesible. |
| `PH_UI_INTERACTION_PROMPT` + indicador | uGUI/primitives locales | `Placeholder / ReleaseBlocked` | Iconografía sin lectura/color único, foco/acción/cancel y revisión en safe areas. |

Los tres fixtures comparten el mismo núcleo y no representan especies/hechos. Materiales, geometría e icono se crearon dentro del proyecto; no se descargaron assets ni se asumió licencia externa. Copy y cues también siguen sujetos a revisión humana.

## Revisión visual VS-D-A01

El [expediente factual](VS_D_A01_TOUCAN_FACTUAL_DOSSIER.md) documenta un pico verde/naranja/rojo/azul, pero no autoriza copiar fotografías ni fija paleta, patrón, sexo, edad o subespecie. Antes de reemplazar `PH_FIXTURE_ANIMAL`, Art y especialista factual deben aprobar silhouette, pico, coloración, escala y PhotoTarget; Legal/Rights debe aprobar source files y runtime exports.

Ismael Bojórquez declaró `Arte: APPROVED`, `Rights: APPROVED` y referencia `Propia` el 2026-08-16. La declaración se conserva en `H-007-IB-2026-08-16`, pero todavía no existe un asset no-`PH_` con ID, source file, runtime export y ledger al que aplicarla. Por tanto, aprueba la intención/especificación y ownership declarado, no el fixture cápsula ni un entregable final inexistente.

Cambiar especie/subespecie devuelve a revisión todo el modelo/ilustración, collider/bounds fotográficos, icono y thumbnail canónico. Cambiar rasgo visual o paleta devuelve esos assets y cualquier actividad de reconocimiento a `Reviewed`; no invalida dieta/sonido salvo que también cambie la especie.

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
