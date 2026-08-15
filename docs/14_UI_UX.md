# 14 — UI/UX infantil y adulta

Fuente de verdad de interacción y accesibilidad de producto. Los flujos específicos remiten al [`01_GDD.md`](01_GDD.md) y al [`PLAYTEST_PLAN.md`](PLAYTEST_PLAN.md).

## Principios

- Landscape, objetivos táctiles amplios y una acción principal por pantalla.
- Audio, icono y movimiento apoyan toda instrucción esencial; texto agrega detalle.
- Feedback inmediato, no punitivo y sin color como único significado.
- Navegación reversible; salida y pausa siempre accesibles desde estados seguros.
- Sin contadores ansiógenos, botones falsos, confirmaciones confusas, anuncios o ofertas infantiles.

## Modos de asistencia

En primera entrada se muestran dos tarjetas equivalentes, sin lenguaje de habilidad:

- **Más guía:** más demostración, resaltados tempranos, voz repetible y menos opciones cuando hay atasco.
- **Guía estándar:** menos señalización inicial y pistas bajo demanda.

No se pregunta edad/fecha de nacimiento, no se recomienda modo según comportamiento y el cambio es libre. La elección se guarda localmente como preferencia, no como perfil.

## Navegación candidata

- Tap-to-move con indicador de destino, camino válido y cancelar mediante nuevo toque.
- Interacción contextual con toque; mantener pulsado no es requisito principal.
- Viewfinder de fotografía con zona de éxito generosa y captura asistida opcional.
- F04 compara tap-to-move contra control directo simplificado; el resultado debe registrarse antes de implementación definitiva.

## Jerarquía de pantallas

1. Inicio/continuar y acceso adulto discreto.
2. Campamento: mundo, misión actual, álbum y personalización.
3. Selva: navegación, pista, interacción y pausa.
4. Actividad: objetivo, área manipulable, repetir audio, pista y salida.
5. Álbum: categorías, ficha y escucha.
6. Área adulta: progreso descriptivo, asistencia, audio, descansos, privacidad y controles futuros de monetización.

## Área adulta

- Acceso tras parental gate apropiado por plataforma; su diseño final requiere revisión de políticas.
- Resume misiones/discoveries y conceptos explorados, sin notas, ranking, inferencias de capacidad o tiempo usado como presión.
- Permite cambiar guía, volumen, reducción de movimiento, captura asistida, descansos y reset con confirmación.
- La “monetización preparada” es solo un espacio/contrato deshabilitado detrás del gate: sin precio, SKU, compra, ads o SDK en MVP.

## Accesibilidad mínima

- Contraste y tamaño validados, safe areas, escalado de texto y foco visible.
- Subtítulos/copy para voz; audio repetible; pistas visuales para sonidos.
- Reducir movimiento y flashes; cámara estable.
- No exigir velocidad, precisión fina, lectura, audio, percepción de color o multitouch.
- Tiempo suficiente y descansos neutrales; reentrada conserva contexto.

## Copy y tono

Verbos de invitación: “mira”, “escucha”, “prueba”, “¿qué notas?”. Evitar “fácil”, “mal”, “fallaste”, “última oportunidad”, “perfecto” como juicio, o cualquier referencia a edad/nivel.

## Aceptación

- Los tres recorridos de papel de [`PLAYTEST_PLAN.md`](PLAYTEST_PLAN.md) completan su intención sin contradicción.
- Toda pantalla tiene acción principal, salida segura, estado sin audio y fallback sin lectura.
- Ambos modos ofrecen mismo contenido/progreso.
- Área adulta no expone compras ni links al niño.
