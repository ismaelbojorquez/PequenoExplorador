# 06 — Diseño de mundo

La Selva es el único mundo del MVP. Cantidades: [`MVP_SCOPE.md`](MVP_SCOPE.md). Dirección visual: [`15_ART_DIRECTION.md`](15_ART_DIRECTION.md).

## Función del mundo

La Selva convierte los objetivos educativos en motivos para explorar. Cada espacio debe comunicar dónde se puede ir, qué reacciona y qué merece observarse, sin llenar la pantalla de marcadores.

## Topología

- **Campamento:** hub seguro con misión, álbum, personalización, asistencia, controles y acceso adulto.
- **Zona Vertical Slice:** claro compacto, recorrido legible, un encuentro animal candidato y retorno directo.
- **MVP:** una Selva conectada compuesta por sectores pequeños reutilizables. La cantidad final de sectores depende de que los descubrimientos canónicos quepan sin saturación; no se comprometen biomas adicionales.
- Atajos y reaperturas reducen recorrido repetitivo. No hay daño, muerte, persecución intensa ni pérdida de inventario.

## Reglas de colocación

- Un punto de interés principal visible o sugerido por encuadre/sonido a la vez.
- Interactivos distinguidos por forma, movimiento y audio, no solo color.
- Caminos válidos tienen señal consistente y retorno claro.
- Descubrimientos educativos aparecen en contexto plausible solo después de aprobación factual.
- Secretos recompensan observación, no precisión extrema, horario real o volver cada día.

## Campamento

El campamento representa progreso sin convertirse en city builder.

- Mejoras son deterministas, permanentes y visibles.
- Personalización cosmética usa opciones ganadas dentro del juego; sin rarezas, cajas o comparación social.
- La primera mejora del Vertical Slice es `Mesa de observación`, que habilita revisar el descubrimiento y evidencia persistencia.
- El MVP incluye el número canónico de mejoras y opciones definido en [`MVP_SCOPE.md`](MVP_SCOPE.md).

## Secretos y objetos especiales

- Un secreto es una observación opcional con pista justa y recompensa fija.
- Un objeto especial es contenido coleccionable contextual, no moneda premium.
- Ninguno bloquea la misión principal ni desaparece por tiempo.

## Estados de mundo data-driven

Cada sector define puntos de entrada/salida, descubrimientos disponibles, interacciones, requisitos de misión, feedback ambiental, navegación y fallback si un objetivo no carga. El mundo consulta progreso; no decide recompensas, hechos educativos ni persistencia física.

## Post-MVP explícito

Otros biomas, ciclo día/noche, clima dinámico, encuentros procedurales y mundo conectado a servicios remotos son post-MVP sin cantidad comprometida.
