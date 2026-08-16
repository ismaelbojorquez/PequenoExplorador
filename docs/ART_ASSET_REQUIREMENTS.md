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
