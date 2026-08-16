# 08 — Sistema de aprendizaje y actividades

Taxonomía y métricas: [`04_EDUCATIONAL_DESIGN.md`](04_EDUCATIONAL_DESIGN.md). Cantidades: [`MVP_SCOPE.md`](MVP_SCOPE.md).

## Contrato Activity

Cada actividad data-driven declara objetivo educativo, descubrimiento relacionado, acción principal, estímulos, variantes de guía, pistas graduadas, feedback, condición de cierre, evidencia observable, fuente factual y recompensa. No recibe edad, cumpleaños ni nivel atribuido al niño.

Prompt 14 reserva `IActivityDefinition` con `ActivityId` tipado, pero no inventa authoring ni reglas antes de la actividad real. Los facts compartidos usan el catálogo/fuentes de [`CONTENT_MODEL.md`](CONTENT_MODEL.md); ninguna interfaz base convierte Draft en contenido educativo aprobado.

## Tipos de actividad MVP

El MVP contiene al menos cinco tipos aprobados de esta lista; el Vertical Slice implementa solo el primero.

| Tipo | Acción de juego | Concepto candidato | Anti-examen |
|---|---|---|---|
| Asociación visual | Relacionar el descubrimiento con rasgo, silueta o contexto. | Observación y clasificación. | Elementos físicos/visuales dentro de la escena. |
| Búsqueda por pista | Encontrar un objetivo usando audio, forma o relación. | Vocabulario e indagación. | La pista guía exploración, no pregunta abstracta. |
| Conteo contextual | Tocar/agrupar elementos visibles. | Conteo y correspondencia. | Sin cronómetro ni teclado numérico. |
| Orden o patrón | Organizar elementos del entorno. | Secuencia, comparación o patrón. | Manipulación directa y ejemplo opcional. |
| Escucha e identificación | Elegir imagen/objeto después de audio contextual. | Lenguaje y atención auditiva. | Repetición ilimitada y apoyo visual. |
| Comparación | Acercar/separar u ordenar por atributo aprobado. | Relaciones y descripción. | Feedback muestra relación, no puntuación. |

## Escalera de pistas

1. Repetir objetivo en el canal elegido.
2. Resaltar el área relevante sin marcar respuesta exacta.
3. Reducir distractores o demostrar una comparación.
4. Resolver acompañado y permitir repetir libremente.

No existe “game over”. Cada intento produce una reacción útil; la recompensa base y el progreso no disminuyen.

## Vertical Slice

Actividad `Reconoce al tucán`: después de fotografiar el discovery candidato, asociar su imagen con un rasgo visual aprobado entre opciones claramente distintas. `Más guía` demuestra una comparación y reduce opciones; `Guía estándar` ofrece pista bajo demanda. El hecho y el rasgo exacto quedan bloqueados hasta revisión factual.

## Rejugabilidad sana

- Variantes cambian posición, ejemplos o distractores aprobados, no el objetivo factual.
- Repetir es voluntario y no necesario para conservar progreso.
- No hay multiplicadores, combos, estrellas por perfección, rachas o recompensas por velocidad.

## Aceptación

- Objetivo puede expresarse con un verbo observable.
- Ambas guías permiten la misma actividad y recompensa.
- Un intento no resuelto conduce a pista útil en vez de castigo.
- La actividad parece una acción del mundo y no una hoja de examen superpuesta.
- Fuente y aprobación están enlazadas antes de Release.
