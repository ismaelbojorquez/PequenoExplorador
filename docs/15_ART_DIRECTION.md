# 15 — Dirección de arte

Dirección de producto; el contrato técnico de archivos vive en [`ART_ASSET_REQUIREMENTS.md`](ART_ASSET_REQUIREMENTS.md). No se crean assets finales en esta fase.

## Norte visual

Selva 2.5D/3D cálida y táctil: formas redondeadas, siluetas legibles, color natural estilizado y detalle concentrado en puntos de interés. Debe invitar a acercarse sin parecer parque temático saturado ni representación científica hiperrealista.

## Lenguaje visual

- Personaje explorador inclusivo y no estereotipado; personalización sin género obligatorio.
- La baseline Prompt 26 usa ocho slots y veinte primitives/colores `PH_` propios sobre el mismo prefab. `MaterialPropertyBlock` evita duplicar materiales; no constituye arte final ni aprobación de tonos, cabello, ropa o accesorios. El ledger y gaps están en [`ART_ASSET_REQUIREMENTS.md`](ART_ASSET_REQUIREMENTS.md).
- Animales con proporciones reconocibles y conducta solo después de aprobación factual; evitar antropomorfismo que contradiga el aprendizaje.
- Plantas/insectos claramente separables del fondo por silueta, profundidad y movimiento.
- Campamento acogedor construido con mejoras visibles y permanentes, sin iconografía de tienda.
- UI como cuaderno/campamento de campo: tarjetas simples, pictogramas consistentes y poco texto.

## Feedback y seguridad

- Acierto: reacción del mundo, brillo suave y gesto celebratorio breve.
- Reintento: animación que señala relación relevante; no cruces rojas, caras tristes o pérdida visual.
- Secretos: pistas sutiles pero redundantes; nunca solo diferencia de color.
- Sin flashes intensos, amenazas, daño, depredación explícita o cámaras bruscas.

## Necesidades humanas del Vertical Slice

- Una representación candidata del tucán, pendiente de revisión factual.
- Personaje/placeholders de exploración, claro de Selva y campamento modular.

Prompt 25 usa únicamente geometría propia `PH_`: suelo/anchors, mesa de observación y rincón de exploración con notas, soporte de foto, lupa y maceta. Antes de Release requieren concept, modelado/materiales, revisión de escala/silueta, optimización móvil y aprobación asset-specific; el cambio debe leerse como progreso acogedor, nunca storefront.
- Viewfinder, ficha de álbum, iconos de guía/pista/pausa y estados de actividad.
- `Mesa de observación` antes/después y un set mínimo de feedback.

Todo puede ser greybox o arte temporal con procedencia conocida hasta validar el loop. No producir el catálogo MVP antes de aprobar el Vertical Slice.

## Necesidades humanas del MVP

- Art director/lead, modelado/ilustración, rig/animación, UI y revisión cultural/factual.
- Hoja de escala, paleta accesible, biblioteca de materiales, guía de siluetas y checklist por discovery.
- Cada asset debe vincularse a un ID de [`CONTENT_SOURCES.md`](CONTENT_SOURCES.md) cuando comunique un hecho.

## Aceptación

Legibilidad en teléfono/tablet, coherencia con dirección técnica, procedencia aprobada, ausencia de estereotipos y validación con usuarios antes de escalar producción.

## UI Kit de expedición

Prompt 27 materializa el norte de cuaderno/campamento como chrome propio: radios amplios, tarjetas papel, sombras cortas solo en cards elevadas, badges de esmalte y acentos mango/cielo/hoja. La jerarquía depende de escala/espacio además de color; celebraciones duran poco y se omiten con reduce-motion. `PH_RoundedRect.png` se genera dentro del proyecto y los iconos se dibujan por código, sin media externa.

Siguen pendientes de arte humano: familia tipográfica con licencia y cobertura ES/EN/pseudo, set final de iconos, ilustración canónica/silueta de álbum, fondos/ornamentos Camp-Selva, estados de actividad, thumbnails cosméticos y revisión de contraste con arte final. Los componentes `PH_` continúan ReleaseBlocked; coherencia de chrome no equivale a aprobación de arte final.
