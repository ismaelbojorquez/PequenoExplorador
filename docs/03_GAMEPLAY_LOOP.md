# 03 — Gameplay loops

Todos los loops realizan `acción → descubrimiento → aprendizaje → recompensa`. Las cantidades de contenido se mantienen en [`MVP_SCOPE.md`](MVP_SCOPE.md).

## Loop de 30 segundos — microinteracción

La locomoción candidata que conecta este loop es `tap en suelo legible → marker inmediato → desplazamiento asistido → llegada`. Prompt 17 añade `tap en target → foco único → auto-acercamiento → acción contextual grande → resultado`, con cancelación y fallback amable. Un tap inválido/no disponible no consume recurso ni falla la misión; muestra feedback discreto y permite otro tap. Discovery sigue fuera hasta Prompt 18.

1. Percibir una pista audiovisual o un elemento interesante.
2. Tocar para acercarse, observar o enfocar.
3. Recibir reacción inmediata del mundo.
4. Identificar una relación sencilla o solicitar una pista.
5. Obtener feedback positivo y un avance visible.

**Aceptación:** intención comprensible en cinco segundos después del tutorial; reacción en cada acción; sin pantalla de nota; reintento inmediato y sin pérdida.

El contrato técnico y sus límites están en [`INTERACTION_SYSTEM.md`](INTERACTION_SYSTEM.md). Los tres fixtures actuales son neutrales `PH_`; no conceden progreso ni contienen afirmaciones educativas.

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

El contenido exacto está en [`MVP_SCOPE.md`](MVP_SCOPE.md). El recorrido implementado en Prompt 29 es `Camp → Selva → mover → enfocar tucán → actividad amable → fotografía virtual → discovery/fact → Estrellas → misión automática → álbum → mejora del rincón → Camp → checkpoint`. Usa un único tucán, una actividad, una misión y una mejora. La actividad y varios visuales/audio siguen `Sourced`/`PH_`, por lo que el APK Development es jugable pero Release continúa bloqueado. No se escala contenido hasta que Prompt 30 vuelva a auditar Gate B.

Los checkpoints se solicitan después de actividad/captura ya reconciliada, misión/reward, compra de mejora y retorno. Todos los repositorios convergen en un solo `AutosaveCoordinator`: `pending → in-flight → ISaveService.Current`; preferencias de idioma/audio se fusionan sobre el snapshot más reciente. Así, pause, locale, transición y retry no pueden reintroducir un snapshot anterior. La segunda sesión conserva FTUE completado y permite repetir actividad/foto sin volver a otorgar rewards únicos.

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

## Primera sesión guiada

El FTUE acompaña el loop real, no crea un tour paralelo: `Camp → Selva → mover → interactuar/actividad → fotografiar → discovery/estrella/misión → Camp → álbum`. El coordinador espera outcomes semánticos; una acción equivocada no castiga ni avanza. A 6 s (`Más guía`) o 12 s (`Guía estándar`) refuerza la misma acción con gesto/replay. Back, pausa, skip y reanudación evitan soft locks. En sesiones posteriores el tutorial completado/omitido permanece silencioso y se ofrece replay desde Camp.
