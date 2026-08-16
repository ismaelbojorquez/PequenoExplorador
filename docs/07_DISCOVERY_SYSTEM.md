# 07 — Sistema de descubrimiento

Discovery es la unión entre exploración, fotografía ficticia, álbum y aprendizaje. Alcance cuantitativo: [`MVP_SCOPE.md`](MVP_SCOPE.md).

## Contrato Discovery

Cada descubrimiento data-driven necesita:

- ID estable, categoría (`animal`, `planta`, `insecto`, `objeto especial`) y nombre aprobado;
- referencia a su entrada factual y estado de aprobación;
- condiciones de aparición/observación y zona propietaria;
- encuadre accesible y criterios de fotografía indulgentes;
- ficha de álbum con imagen, audio, texto opcional y uno o dos hechos aprobados;
- actividad relacionada opcional/obligatoria según misión;
- recompensa determinista y estado persistente;
- fallback si el contenido factual o visual no está aprobado.

El contrato técnico base está en [`CONTENT_MODEL.md`](CONTENT_MODEL.md): `DiscoveryDefinitionAsset` se mapea a `DiscoveryDefinition` readonly y se resuelve mediante `DiscoveryId` en O(1). Categorías/tags son IDs extensibles, no enums. Prompt 14 incluye solo `discovery.jungle.placeholder`, neutral y Draft; no implementa estados, fotografía ni álbum.

## Estados

`No visto → Detectado → Fotografiado → Explorado`

- **Detectado:** el niño provocó o percibió una pista.
- **Fotografiado:** el viewfinder del juego reconoció encuadre suficiente.
- **Explorado:** completó la actividad relacionada o revisó la ficha, según contrato.
- Los estados no bajan ni expiran; pedir pistas no reduce recompensa.

## Fotografía dentro del juego

- No usa cámara real, galería, ubicación ni permisos del dispositivo.
- El objetivo ocupa una zona amplia del encuadre; no se califica composición.
- `Más guía` muestra marco, dirección y autoenfoque más evidentes.
- `Guía estándar` conserva confirmación y pista bajo demanda.
- Si hay dificultad motriz, una opción permite captura asistida al mantener el objetivo visible brevemente.

## Álbum

- Organiza por categorías sin porcentajes que generen presión.
- Fichas bloqueadas no muestran siluetas alarmantes ni fechas límite.
- Una ficha aprobada permite escuchar nombre, ver imagen y revisar hechos; texto nunca es el único canal.
- El área adulta puede ver conceptos explorados y contenido encontrado, no notas o inferencias sobre capacidad.

## Vertical Slice

Discovery candidato: **tucán pico iris (`Ramphastos sulfuratus`)**, con especie y hechos pendientes de aprobación factual antes de producción. Debe poder detectarse, fotografiarse, asociarse visualmente en una actividad, aparecer en álbum y persistir. El nombre científico no se presenta obligatoriamente al niño.

## Aceptación del sistema

- Fotografía se comprende sin leer y no requiere precisión frustrante.
- Estado del álbum coincide tras cerrar/reabrir la sesión de papel o futura implementación.
- Ningún hecho llega a estado Release sin `Approved` en [`CONTENT_SOURCES.md`](CONTENT_SOURCES.md).
- Descubrimiento y recompensa siguen funcionando offline y sin monetización.
