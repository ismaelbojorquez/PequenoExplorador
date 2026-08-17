# 01 — Game Design Document

Documento canónico de experiencia. Visión: [`00_PRODUCT_VISION.md`](00_PRODUCT_VISION.md). Cantidades y prioridades: [`MVP_SCOPE.md`](MVP_SCOPE.md). Loops: [`03_GAMEPLAY_LOOP.md`](03_GAMEPLAY_LOOP.md).

## Fantasía del jugador

“Soy una pequeña persona exploradora: encuentro vida y pistas, tomo fotografías dentro del juego, aprendo a reconocer lo que vi y hago más acogedor mi campamento.” El adulto no es árbitro; puede revisar progreso local y ajustar asistencia desde un área separada.

## Estructura del juego

1. **Campamento:** punto seguro para elegir misión, ver álbum, personalizar y salir.
2. **Selva:** espacio explorable acotado con descubrimientos y secretos.
3. **Fotografía:** viewfinder ficticio; no usa cámara ni permisos del dispositivo.
4. **Álbum:** registro visual de descubrimientos y hechos aprobados.
5. **Actividades:** interacciones breves ligadas a lo descubierto.
6. **Misiones:** objetivos con principio y cierre que conectan exploración, aprendizaje y campamento.
7. **Progreso local:** conserva descubrimientos, misiones, preferencias y mejoras sin cuenta ni red.

## Flujo de primera sesión

1. Pantalla de entrada accesible y elección manual entre `Más guía` y `Guía estándar`.
2. Llegada al campamento y misión corta con demostración audiovisual.
3. Exploración de un espacio pequeño usando control candidato tap-to-move.
4. Encuentro, fotografía dentro del juego y actividad contextual.
5. Registro en álbum, recompensa determinista y una mejora del campamento.
6. Opción neutral: continuar, revisar álbum, descansar o salir.

## Control candidato

**Tap-to-move** sigue siendo candidato, no decisión cerrada: tocar suelo válido propone destino, muestra confirmación visual y permite cancelar/corregir. Prompt 16 aporta un prototipo `PH_` reversible con NavMesh y cámara asistida para poder medirlo; no convierte el control en UX final. El playtest P-006 debe compararlo con movimiento directo simplificado y validar comprensión, precisión motriz, latencia y fatiga antes de aprobar producción. Un toque sobre un elemento cercano priorizará interacción desde Prompt 17.

## Progresión y economía

- Progreso por descubrimientos, actividades y misiones completadas; nunca por tiempo consecutivo o días de retorno.
- Única moneda virtual: **Estrellas de Explorador**, ganadas de forma determinista por discovery, misión, actividad o hito de colección; el tucán del Vertical Slice concede una sola estrella provisional.
- No se pierden ni compran, no caducan y no existen conversiones, moneda premium, azar ni ventaja pagada. Sus únicos usos permitidos son mejoras visuales de Camp y cosméticos, nunca contenido educativo esencial.
- Personalización se organiza por ocho slots sin género: tono de piel, cabello, camiseta, pantalón, zapatos, sombrero, mochila y herramienta. Siempre existen defaults gratuitos; preview, unlock y equip son pasos separados. Véase [`CUSTOMIZATION_SYSTEM.md`](CUSTOMIZATION_SYSTEM.md).

La primera mejora verificable es `Mesa de observación → Rincón de exploración`, con costo provisional de 3 Estrellas. El niño ve un preview y confirma; saldo insuficiente produce una sugerencia amable sin enlace comercial. La mejora es visual, permanente y no condiciona Selva, álbum, facts o actividades. El contrato ejecutable y mapa de estaciones viven en [`CAMP_SYSTEM.md`](CAMP_SYSTEM.md).
- Las reglas, fuentes/usos e idempotencia son canónicas en [`ECONOMY_REWARDS.md`](ECONOMY_REWARDS.md).
- La dificultad se adapta por el modo de guía y por pistas solicitadas, no por edad inferida ni perfil oculto.
- Repetir una actividad conserva su valor lúdico, pero no genera loops explotables o contadores compulsivos.

## Feedback

- Acierto: animación breve, sonido amable y explicación contextual opcional.
- Intento no resuelto: reconocer la acción, mostrar qué observar y ofrecer una pista; nunca “incorrecto”, pérdida, ranking o burla.
- Tras intentos repetidos: reducir opciones, demostrar una relación o permitir completar acompañado. La recompensa base no disminuye.
- Todo feedback importante combina al menos dos canales entre imagen, movimiento, audio y texto.

## Descansos y cierre

Después de aproximadamente tres actividades o diez minutos, el campamento ofrece una pausa neutral. No bloquea, no culpa y no promete recompensa por ignorarla. El niño puede salir desde cualquier estado seguro; el progreso confirmado se guarda automáticamente.

## Tono

Curioso, cálido, concreto y respetuoso. El narrador invita (“miremos”, “¿qué notas?”), no examina (“demuestra”, “fallaste”). Humor visual suave, sin peligro intenso, violencia, sarcasmo ni infantilización excesiva.

## Contratos de producto data-driven

No son clases C#; son responsabilidades que la arquitectura futura representará con datos validados.

| Contrato | Responsabilidad | Se relaciona con | No debe conocer |
|---|---|---|---|
| **Discovery** | Identidad, categoría, condiciones para observar/fotografiar, ficha aprobada y estado de álbum. | World, Activity, Reward | UI concreta, SDKs o save físico. |
| **Activity** | Objetivo educativo, interacción, variantes de guía, pistas, feedback y condición de cierre. | Discovery, Mission, Reward | Edad real, monetización o escena específica. |
| **Mission** | Secuencia legible de objetivos, dependencias, cierre y recompensa determinista. | Discovery, Activity, World, Camp Upgrade | Calendario real, rachas o tienda. |
| **Reward** | Qué progreso reconoce, cantidad determinista y destino permitido. | Mission, Album, Camp Upgrade | Aleatoriedad pagada, FOMO o ads. |
| **World** | Zonas, puntos interactivos, disponibilidad, navegación y secretos. | Discovery, Mission | Hechos no aprobados o estado global mutable fuera de sesión. |
| **Camp Upgrade** | Requisito, costo blando, cambio visual/funcional y persistencia. | Reward, Mission | Precio real, SKU o compra infantil. |

Relación principal: `World ofrece Discovery → Discovery habilita Activity → Mission coordina ambos → Reward actualiza Album/Camp Upgrade → persistencia conserva el resultado`.

## Fuera de alcance

Las exclusiones Won’t están en [`MVP_SCOPE.md`](MVP_SCOPE.md). Ningún documento de sistema puede introducir cantidades o features adicionales sin actualizar primero esa fuente canónica.
