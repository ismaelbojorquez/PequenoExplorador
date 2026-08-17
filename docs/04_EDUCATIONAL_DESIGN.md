# 04 — Diseño educativo

Fuente de verdad para objetivos, taxonomía y evidencia educativa. Las afirmaciones factuales requieren el flujo de [`CONTENT_SOURCES.md`](CONTENT_SOURCES.md).

## Enfoque

El juego practica observación, vocabulario y relaciones mediante acciones dentro del mundo. No diagnostica, califica, compara ni promete resultados académicos. “Aprendizaje” significa evidencia observable ligada a un objetivo pequeño, no completar una pantalla.

## Taxonomía de conceptos

| Dominio | Conceptos practicables | Evidencia en juego | Orientación, no perfil |
|---|---|---|---|
| Naturaleza | Ser vivo/no vivo, animal/planta/insecto, partes visibles, hábitat, conducta aprobada. | Señalar, asociar, describir o reconocer en contexto. | 4–5: diferencias evidentes; 6–7: categorías/relaciones; 8–9: múltiples pistas. |
| Pensamiento matemático | Conteo, correspondencia uno-a-uno, comparación, patrón y secuencia. | Organizar elementos o elegir conjunto justificable. | Cantidades y distractores crecen por variante, no por edad capturada. |
| Lenguaje | Vocabulario, escucha, descripción, relación palabra-imagen. | Reconocer término por audio/imagen o usarlo al contar la experiencia. | Audio y pictogramas siempre disponibles; texto amplía, no bloquea. |
| Indagación | Observar, predecir, probar, comparar y revisar. | Elegir dónde mirar y cambiar estrategia después de una pista. | La guía modela preguntas, no entrega una nota. |
| Socioemocional | Paciencia, cuidado, curiosidad y autonomía para pedir ayuda/descansar. | Solicitar pista, reintentar o parar sin costo. | Sin moralizar ni registrar rasgos personales. |

## Modos de asistencia

Se eligen manualmente y pueden cambiarse en cualquier momento; no se pregunta edad ni fecha de nacimiento.

| Modo | Presentación | Pistas | Texto |
|---|---|---|---|
| **Más guía** | Demostración audiovisual, objetivo único y resaltado inicial. | Aparecen antes; reduce opciones tras atasco y permite repetir voz. | Secundario, breve y acompañado de icono/audio. |
| **Guía estándar** | Objetivo audiovisual breve y exploración con menos señalización. | Bajo demanda y luego escalables; misma solución y recompensa. | Disponible como apoyo, nunca único canal. |

El modo no altera hechos, contenido disponible, economía ni valor de recompensa.

## Estructura de un objetivo educativo

Cada actividad declara:

1. concepto y verbo observable;
2. prerequisitos dentro del juego, no personales;
3. estímulo y acción esperada;
4. variantes `Más guía`/`Guía estándar`;
5. pistas graduadas y feedback no punitivo;
6. evidencia observable inmediata;
7. hecho/fuente aprobada si aplica;
8. riesgo de interpretación y criterio de revisión.

Prompt 14 hace ejecutable la trazabilidad mínima: cada `EducationalFactDefinition` enlaza copy localizada, claim atómico de revisión y `ContentSourceRecord` por ID. `Draft` puede apoyar tooling Development con watermark, pero solo `Approved` no-placeholder supera el validator Release; la aprobación técnica no reemplaza al revisor humano.

Prompt 23 hace ejecutable el ciclo de actividad sin convertirlo en examen: `TryAgain/Hint` reemplazan fallo, las pistas no reducen reward y solo se persisten conceptos vistos/completados agregados por día. La fixture visual abstracta no introduce un claim factual; cualquier actividad de animal sigue el expediente y gate editorial habitual.

## Evidencia, no examen

- Evidencia primaria: acción correcta después de observar una pista relevante.
- Evidencia secundaria: transferencia dentro de otra situación del juego o explicación espontánea en playtest.
- No se usan porcentajes, notas, “fallos”, comparaciones, cronómetros de presión ni dashboards de rendimiento infantil.
- El área adulta resume experiencias realizadas y conceptos explorados; no etiqueta habilidad, inteligencia ni nivel del niño.
- Las misiones usan cierre automático, no expiran y no premian velocidad ni ausencia de pistas. Su progreso orienta el loop; no es una calificación ni un registro conductual detallado.

## Hipótesis medibles

- Comprensión: al menos 4 de 5 participantes completan el loop después de la demostración con máximo una intervención del facilitador.
- Aprendizaje inmediato: al menos 3 de 5 muestran el concepto objetivo en una situación equivalente sin revelar la respuesta.
- Transferencia cualitativa: al menos 2 de 5 reconocen o explican espontáneamente la relación al final de sesión.
- Bienestar: cero señales persistentes de angustia causadas por feedback; toda frustración observada debe tener acción correctiva antes de escalar contenido.

Son criterios de diseño para playtest, no claims públicos ni evidencia científica. Véase [`PLAYTEST_PLAN.md`](PLAYTEST_PLAN.md).
