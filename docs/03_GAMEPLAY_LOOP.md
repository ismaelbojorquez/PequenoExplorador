# 03 — Gameplay loops

Todos los loops realizan `acción → descubrimiento → aprendizaje → recompensa`. Las cantidades de contenido se mantienen en [`MVP_SCOPE.md`](MVP_SCOPE.md).

## Loop de 30 segundos — microinteracción

La locomoción candidata que conecta este loop es `tap en suelo legible → marker inmediato → desplazamiento asistido → llegada`. Un tap inválido no consume recurso ni falla la misión; muestra feedback discreto y permite otro tap. Prompt 16 solo valida este tramo `PH_`; interacción y discovery empiezan en Prompt 17/18.

1. Percibir una pista audiovisual o un elemento interesante.
2. Tocar para acercarse, observar o enfocar.
3. Recibir reacción inmediata del mundo.
4. Identificar una relación sencilla o solicitar una pista.
5. Obtener feedback positivo y un avance visible.

**Aceptación:** intención comprensible en cinco segundos después del tutorial; reacción en cada acción; sin pantalla de nota; reintento inmediato y sin pérdida.

## Loop de 3 minutos — descubrimiento completo

1. Elegir o recibir una misión corta en campamento.
2. Explorar una zona acotada y localizar un objetivo.
3. Fotografiarlo con el viewfinder del juego.
4. Resolver una actividad contextual de 30–90 segundos.
5. Incorporar la ficha al álbum y recibir recompensa determinista.
6. Volver al mundo o al campamento con una elección neutral.

**Aceptación:** tiene principio/cierre reconocibles, una sola intención educativa y no necesita lectura para completarse en `Más guía`.

## Sesión de 15–30 minutos

1. Entrada al campamento y recuperación de contexto.
2. Una a tres misiones o exploración libre.
3. Alternancia entre mundo, fotografía y actividades; no más de dos actividades similares consecutivas por defecto.
4. Revisión o mejora del campamento/álbum.
5. Sugerencia neutral de descanso aproximadamente al minuto 10 y en puntos naturales posteriores.
6. Cierre voluntario con guardado local y bienvenida no condicionada para el próximo regreso.

**Aceptación:** el juego permite parar sin perder progreso; no premia duración, retorno diario o ignorar descansos; el adulto no necesita finalizar por el niño.

## Vertical Slice — recorrido canónico

El contenido exacto está en [`MVP_SCOPE.md`](MVP_SCOPE.md): misión `Conoce al tucán`, un descubrimiento animal candidato, una actividad de asociación visual, una mejora `Mesa de observación` y persistencia de todos esos estados. No se escala contenido hasta que ese recorrido cumpla los criterios del Gate B.

## Modos de juego

- **Misión guiada:** objetivo visible y secuencia corta.
- **Exploración libre:** sin temporizador; descubrimientos disponibles según progreso.
- **Campamento/álbum:** contemplación, personalización y revisión.

No hay modo examen. Las actividades están integradas en la aventura y se pueden repetir por curiosidad, sin grind obligatorio.

## Estados de interrupción

- Background, llamada o cierre: guardar último hito confirmado, no mitad de una decisión.
- Sin audio: duplicar instrucciones esenciales visualmente.
- Toque inválido: feedback discreto, sin penalización.
- Niño atascado: pista solicitada, pista automática configurable y salida a campamento.
